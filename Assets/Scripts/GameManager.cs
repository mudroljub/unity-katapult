using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject terrain;
    public float cannonBallMass = 25f;
    public float springForce = 15000f;

    private static GameManager instance;

    Coroutine activeCoroutine;

    [Header("Scene References")]
    public Camera mainCam;
    public Camera uiCam;

    [SerializeField] private CubeWall cubeWall;
    [SerializeField] public Catapult catapult;
    [SerializeField] public CannonBall cannonBall;

    [ReadOnly] [SerializeField] private float springK;

    public float SpringK
    {
        get
        {
            return springK;
        }
        set
        {
            springK = value;
        }
    }

    #region Physics Formulas

    // Find the instantaneous velocity at the time of the cannonball's launch from the Catapault Arm
    // Formula: √(springK / m) * angle² - (g * √2)
    public float InitialVelocity()
    {
        float velocity = Mathf.Sqrt(((springK / cannonBall.Mass) * Mathf.Pow((catapult.DEFAULT_LAUNCH_ANGLE * Mathf.Deg2Rad), 2)) - (Physics.gravity.y * Mathf.Sqrt(2f)));
        return velocity;
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

        float velocity = InitialVelocity();
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

    private void Update()
    {
        UpdateCannonBallMass(cannonBallMass);
        UpdateSpringForce(springForce);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Reset();
            LaunchFreePlayCannonBall();
        }
    }
}
