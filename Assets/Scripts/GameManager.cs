using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject terrain;

    private static GameManager instance;

    Coroutine activeCoroutine;

    [Header("Scene References")]
    public Camera mainCam;
    public Camera uiCam;

    [SerializeField] private CubeWall cubeWall;
    [SerializeField] public Catapult catapult;
    [SerializeField] public CannonBall cannonBall;

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
        if(activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }

        catapult.Reset();
        cannonBall.Reset();
        cubeWall.Reset();
    }

    public static GameManager GetInstance()
    {
        return instance;
    }

    public void LaunchFreePlayCannonBall()
    {
        catapult.launchSpeed = Catapult.LAUNCH_SPEED_FREEPLAY;
        catapult.throwCalled = true;
        activeCoroutine = StartCoroutine(DoProcessFreePlayLaunch());
    }

    #region FreePlay
    public IEnumerator DoProcessFreePlayLaunch()
    {
        yield return new WaitWhile(() => { return catapult.throwCalled; });

        float velocity = Velocity_At_Time_Of_Launch();
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
        CalculateDeltaTime();
    }

    public void ShowSpringChange()
    {
        CalculateDeltaTime();
    }

}
