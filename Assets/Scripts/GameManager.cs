using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Start Params")]
    public List<GameObject> markers = new List<GameObject>();
    public GameObject terrain;

    private static GameManager instance;

    [Header("Configuration Parameters")]
    [SerializeField] private float camLerpSpeed = 0.1f;

    Coroutine activeCoroutine;

    [Header("Scene References")]
    public Camera mainCam;
    public Camera uiCam;
    public Action OnEditorFocus;
    public bool sliderLockOut = false;

    [SerializeField] private CubeWall cubeWall;
    [SerializeField] public Catapult catapult;
    [SerializeField] public CannonBall cannonBall;
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
        return deltaTime;
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

    public void Reset()
    {
        sliderLockOut = false;

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

    public static GameManager GetInstance()
    {
        return instance;
    }

    // Reset defaults and initiate Free Play Box Mode
    public void ExecuteFreePlayBoxMode()
    {
        cubeWall.gameObject.SetActive(true);
        cubeWall.ShowCenterRowOnly();
        cubeWall.SetKinematic(false);

        catapult.launchSpeed = Catapult.LAUNCH_SPEED_FREEPLAY;

        springArrow.SetArrowTransform(cannonBall.SpringPoint_Centered);
        gravArrow.SetArrowTransform(cannonBall.CenterWeightPoint);

        springArrow.Show();
        gravArrow.Show();

        CalculateDeltaTime();
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
        CalculateDeltaTime();
    }

    public void ShowSpringChange()
    {
        springArrow.ChangeArrowWeight(SpringForce);
        CalculateDeltaTime();
    }

}
