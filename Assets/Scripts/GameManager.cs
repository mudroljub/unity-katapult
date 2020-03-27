using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Start Params")]
    public float DistanceFromGround_At_TimeOFLaunch;     // The minimum distance between the cannon ball and the ground to initiate the SlopeDown learning step
    public GameObject arrowBasicFrontPivotPrefab;
    public GameObject arrowBasicBackPivotPrefab;
    public UIController uiController;

    public List<GameObject> markers = new List<GameObject>();
    public GameObject terrain;

    private static GameManager instance;
    [SerializeField] private PopupNotification popup;

    [Header("Configuration Parameters")]
    [SerializeField] private float camLerpSpeed = 0.1f;

    Coroutine activeCoroutine;

    [Header("Scene References")]
    public Camera mainCam;
    public Camera uiCam;
    public Action OnEditorFocus;
    public bool sliderLockOut = false;

    [SerializeField] private CubeWall cubeWall;
    [SerializeField] public TargetBoard targetBoard;
    [SerializeField] public Catapult catapult;
    [SerializeField] public CannonBall cannonBall;
    [SerializeField] public BasketballHoop ballHoop;
    [SerializeField] private ArrowIndicator gravArrow;
    [SerializeField] private ArrowIndicator tensionArrow;
    [SerializeField] private ArrowIndicator springArrow;
    [SerializeField] private ArrowIndicator resultArrow;
    [SerializeField] private DistanceGizmo distanceGizmo;
    [SerializeField] private EnergyIndicator energyWidget;

    [ReadOnly] [SerializeField] private Transform tensionPoint;
    [ReadOnly] [SerializeField] private Transform springPoint;
    [ReadOnly] [SerializeField] private Transform weightPoint;
    [ReadOnly] [SerializeField] private Transform resultPoint;

    public LayerMask terrainLayer;
    [SerializeField] private List<ArrowIndicator> vectorArrows = new List<ArrowIndicator>();

    [ReadOnly] [SerializeField] private float springK;
    [ReadOnly] [SerializeField] private float springForce;

    public float SpringK
    {
        get
        {
            return springK;
        }
        set
        {
            springK = value;
            Vector3 springVector = CalculateSVector();
            SpringForce = springVector.magnitude;
        }
    }

    public float SpringForce
    {
        get
        {
            return springForce;
        }
        set
        {
            springForce = value;
            ShowSpringChange();
        }
    }

    [ReadOnly] [SerializeField] private float tensionForce;
    public float TensionForce
    {
        get
        {
            return tensionForce;
        }
        set
        {
            tensionForce = value;
            tensionArrow.ChangeArrowWeight(tensionForce);
        }
    }

    [ReadOnly] [SerializeField] private float resultForce;
    public float ResultForce
    {
        get
        {
            return resultForce;
        }
        set
        {
            resultForce = value;
            if (resultArrow.isActiveAndEnabled)
            {
                resultArrow.ChangeArrowWeight(resultForce);
            }
        }
    }

    #region Physics Formulas

    // The Elastic Potential Energy of the Cannonball as a result of the recoiled Catapult Arm 
    // Formula: ( 0.5 * springK * angle² )
    private float Delta_Elastic_Potential_Energy()
    {
        return 0.5f * SpringK * Mathf.Pow(catapult.DEFAULT_LAUNCH_ANGLE * Mathf.Deg2Rad, 2);
    }

    // The difference of Gravitational Potential Energy 
    // Formula: ( m * g * √2/2)
    private float Delta_Gravitational_Potential_Energy()
    {
        return cannonBall.WeightForce * (Mathf.Sqrt(2) / 2f);
    }

    // The ratio of (Delta Elastic Potential Energy) over (Delta Gravitation Potential Energy)
    // Formula: DEPE / DGPE
    private float ratio_Of_DEPE_Over_DGPE()
    {
        float ratio = Delta_Elastic_Potential_Energy() / Delta_Gravitational_Potential_Energy();
        return ratio;
    }

    // Equation for Kinetic Energy at time of launch 
    // Formula: (0.5 * m * vI²)
    private float Kinetic_Energy_At_Launch()
    {
        float vel = Velocity_At_Time_Of_Launch();
        return 0.5f * cannonBall.Mass * Mathf.Pow(vel, 2);
    }

    // Find the instantaneous velocity at the time of the cannonball's launch from the Catapault Arm
    // Formula: √(springK / m) * angle² - (g * √2)
    public float Velocity_At_Time_Of_Launch()
    {
        float velocity = Mathf.Sqrt(((springK / cannonBall.Mass) * Mathf.Pow((catapult.DEFAULT_LAUNCH_ANGLE * Mathf.Deg2Rad), 2)) - (Physics.gravity.y * Mathf.Sqrt(2f)));

        return velocity;
    }

    // Delta Time is equal to the Delta Vertical Velocity divided by the vertical acceleration (gravity)
    // Formula: ((vVertF - vVertI) / g)
    private float CalculateDeltaTime()
    {
        float velocity = Velocity_At_Time_Of_Launch();
        float vertVelocity = velocity * Mathf.Cos(catapult.DEFAULT_LAUNCH_ANGLE * Mathf.Deg2Rad);
        float vI = vertVelocity;
        float vF = -vertVelocity;
        float deltaV = vF - vI;
        float deltaTime = deltaV / Physics.gravity.y;
        uiController.physicsUIPanel.timeText.text = Math.Round(deltaTime, 2).ToString() + " s";
        return deltaTime;
    }

    /// <summary>
    /// Distance is calculated using the horizontal component of velocity multiplies by delta time 
    /// Formula: ( vIh * dt )
    /// </summary>
    public void CalculateDistance()
    {
        float horizVelocity = Velocity_At_Time_Of_Launch() * Mathf.Sin(catapult.DEFAULT_LAUNCH_ANGLE * Mathf.Deg2Rad);
        float deltaTime = CalculateDeltaTime();
        float horizontalDistance = horizVelocity * deltaTime;

        // Stretch the distance gizmo to show the pre-calculated horizontal distance
        distanceGizmo.StretchGizmo(horizontalDistance);

        // Update the Distance Text on the UI Physics Info (Top Left UI)  
        uiController.physicsUIPanel.distanceText.text = Math.Round(horizontalDistance, 2).ToString() + " m";
    }

    // Force that is the combined Normal and Centrifugal force of the catapult spoon on the cannonball as the arm rises
    // Formula: (Nx+Cx, Ny+Cy)
    private Vector3 CalculateSVector()
    {
        return (NormalVector() + CentrifugalVector()) * new Vector3(-1, 1, 0); // Multiply x by -1 to horizontally flip the S Vector to the catapult's local orientation
    }

    // Spring Force vector that is perpendicular to the catapault arm (Force that acts on the cannonball from the catapult spoon as the arm rises)
    private Vector2 NormalVector()
    {
        float angle = catapult.ArmAngleRadians;
        Vector2 normalizedNormal = new Vector2(0, 1);   // Perpendicular 90 degrees to catapault arm

        return normalizedNormal * ratio_Of_DEPE_Over_DGPE() * cannonBall.WeightForce * (1 - (2 * angle) / Mathf.PI);
    }

    // Centrifugal Force vector that is Parallel to Catapault arm (Force that keeps the cannonball horizontally in the spoon as the arm rises)
    private Vector2 CentrifugalVector()
    {
        float angle = catapult.ArmAngleRadians;
        Vector2 normalizedCentrifugal = new Vector2(1, 0);  // Parallel 0 degrees to catapault arm

        return normalizedCentrifugal * (ratio_Of_DEPE_Over_DGPE() * cannonBall.WeightForce * ((2 * angle) / Mathf.PI));
    }

    #endregion

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Reset();
    }

    public void Update()
    {
        // Update UI
        if (cannonBall.paused)
        {
            uiController.physicsUIPanel.velocityText.text = (float)Math.Round(cannonBall.currentVelocity.magnitude, 2) + " m/s";
        }
        else
        {
            uiController.physicsUIPanel.velocityText.text = (float)Math.Round(cannonBall.rigidBody.velocity.magnitude, 2) + " m/s";
        }

        if (catapult.launched)
        {
            // Constantly raycast to detect knocked over boxes, once the cannonball is launched in Freeplay Box Mode
            cubeWall.CalculateScore();
        }
    }

    public void Reset()
    {
        sliderLockOut = false;
        popup.Reset();
        HideGizmoAndWidgets();

        if(activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }

        foreach(GameObject marker in markers)
        {
            Destroy(marker);
        }
        markers.Clear();

        foreach(ArrowIndicator arrow in vectorArrows)
        {
            Destroy(arrow.gameObject);
        }
        vectorArrows.Clear();

        // Reset Camera view transform
        mainCam.transform.position = catapult.startCamTransform.position;
        mainCam.transform.rotation = catapult.startCamTransform.rotation;

        catapult.Reset();
        cannonBall.Reset();
        cubeWall.Reset();
    }

    private void HideGizmoAndWidgets()
    {
        gravArrow.Hide();
        springArrow.Hide();
        tensionArrow.Hide();
        resultArrow.Hide();
        distanceGizmo.Hide();
        energyWidget.Hide();

        foreach (ArrowIndicator arrow in vectorArrows)
        {
            arrow.Hide();
        }
    }

    public static GameManager GetInstance()
    {
        return instance;
    }

    public static bool HasInstance()
    {
        return instance;
    }

    public void ExecuteVectorLearningMode()
    {
        cubeWall.gameObject.SetActive(false);
        targetBoard.gameObject.SetActive(false);
        ballHoop.gameObject.SetActive(false);

        uiController.physicsUIPanel.ShowAllUIs();

        catapult.launchSpeed = Catapult.LAUNCH_SPEED_LESSON;
    }

    public void ExecuteEnergyLearningMode()
    {
        cubeWall.gameObject.SetActive(false);
        targetBoard.gameObject.SetActive(false);
        ballHoop.gameObject.SetActive(false);

        uiController.physicsUIPanel.ShowAllUIs();

        catapult.launchSpeed = Catapult.LAUNCH_SPEED_LESSON;
    }

    // Reset defaults and initiate Free Play Target Mode
    public void ExecuteFreePlayTargetMode()
    {
        cubeWall.gameObject.SetActive(true);
        cubeWall.ShowAllBoxes();

        targetBoard.gameObject.SetActive(true);
        ballHoop.gameObject.SetActive(false);
        cubeWall.SetKinematic(true);

        // Hide Distance UI during Free play
        uiController.physicsUIPanel.ShowAllUIs();
        uiController.physicsUIPanel.distanceWidget.SetActive(false);

        catapult.launchSpeed = Catapult.LAUNCH_SPEED_FREEPLAY;

        InitializeFreePlayTargetMode();
    }

    // Initialize FreePlay Target Mode
    public void InitializeFreePlayTargetMode()
    {
        Vector3 SVector = CalculateSVector();

        springArrow.SetArrowTransform(cannonBall.SpringPoint_Centered);
        springArrow.transform.up = SVector.normalized;
        SpringK = SVector.magnitude;

        gravArrow.SetArrowTransform(cannonBall.CenterWeightPoint);

        springArrow.Show();
        gravArrow.Show();

        CalculateForces();
    }

    // Reset defaults and initiate Free Play Box Mode
    public void ExecuteFreePlayBoxMode()
    {
        cubeWall.gameObject.SetActive(true);
        cubeWall.ShowCenterRowOnly();

        targetBoard.gameObject.SetActive(false);
        ballHoop.gameObject.SetActive(false);
        cubeWall.SetKinematic(false);

        // Hide Distance UI during Free play
        uiController.physicsUIPanel.ShowAllUIs();
        uiController.physicsUIPanel.distanceWidget.SetActive(false);
        uiController.boxesUI.Reset();

        catapult.launchSpeed = Catapult.LAUNCH_SPEED_FREEPLAY;

        springArrow.SetArrowTransform(cannonBall.SpringPoint_Centered);
        gravArrow.SetArrowTransform(cannonBall.CenterWeightPoint);

        springArrow.Show();
        gravArrow.Show();

        CalculateForces();
    }

    // Reset defaults and initiate Free Play Basketball Mode
    public void ExecuteBasketballMode()
    {
        cubeWall.gameObject.SetActive(true);
        cubeWall.ShowAllBoxes();
        targetBoard.gameObject.SetActive(false);
        ballHoop.gameObject.SetActive(true);
        ballHoop.Reset();
        cubeWall.SetKinematic(true);

        // Hide Distance UI during Free play
        uiController.physicsUIPanel.ShowAllUIs();
        uiController.physicsUIPanel.distanceWidget.SetActive(false);

        catapult.launchSpeed = Catapult.LAUNCH_SPEED_FREEPLAY;

        springArrow.SetArrowTransform(cannonBall.SpringPoint_Centered);
        gravArrow.SetArrowTransform(cannonBall.CenterWeightPoint);

        springArrow.Show();
        gravArrow.Show();

        CalculateForces();
    }

    public void LaunchFreePlayCannonBall()
    {
        catapult.throwCalled = true;
        activeCoroutine = StartCoroutine(DoProcessFreePlayLaunch());
    }

    #region FreePlay
    public IEnumerator DoProcessFreePlayLaunch()
    {
        yield return new WaitWhile(() => { return catapult.throwCalled; });

        float velocity = Velocity_At_Time_Of_Launch();
        float ratio = ratio_Of_DEPE_Over_DGPE();
        catapult.ThrowBall(catapult.launchVector.up, velocity);

        activeCoroutine = null;
    }
    #endregion

    public void UpdateCannonBallMass(float _mass)
    {
        if (!cannonBall.Mass.Equals(_mass))
        {
            cannonBall.Mass = _mass;
        }
    }

    public void UpdateSpringForce(float _springK)
    {
        if (!springK.Equals(_springK))
        {
            SpringK = _springK;
        }
    }

    // Show animation related to change of mass, also update opposing and like forces
    public void ShowMassChange()
    {
        gravArrow.ChangeArrowWeight(cannonBall.WeightForce);
        CalculateForces();
    }

    public void ShowSpringChange()
    {
        springArrow.ChangeArrowWeight(SpringForce);
        CalculateForces();
    }

    private void CalculateResultVector()
    {
        // Scale the Spring and Weight Unit Vectors by their force
        Vector3 springVector = catapult.springVector.transform.up * SpringForce;
        Vector3 weightVector = cannonBall.CenterWeightPoint.transform.up * cannonBall.WeightForce;

        // Combine both scaled vectors to get the scaled Resultant Force
        Vector3 resultVector = springVector + weightVector;

        // Orient the Result Gizmo Arrow to align with the resultant vector above (visual)
        catapult.resultVector.up = resultVector.normalized;

        // Update resultant force (magnitude of the result vector calculated above)
        ResultForce = resultVector.magnitude;
    }


    // calculate effect on all forces
    public void CalculateForces()
    {
        CalculateDeltaTime();
    }
   
}
