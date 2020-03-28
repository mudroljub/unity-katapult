using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject terrain;
    Coroutine activeCoroutine;

    [Header("Scene References")]
    public Camera mainCam;
    public Camera uiCam;

    [SerializeField] private CubeWall cubeWall;
    [SerializeField] public Catapult catapult;
    [SerializeField] public CannonBall cannonBall;

    // formula: √(springForce / m) * angle² - (g * √2)
    public float InstantaneousVelocity()
    {
        float springForce = catapult.springForce;
        float mass = cannonBall.rigidBody.mass;
        float angle = catapult.DEFAULT_LAUNCH_ANGLE * Mathf.Deg2Rad;
        float velocity = Mathf.Sqrt(springForce / mass * Mathf.Pow(angle, 2) - Physics.gravity.y * Mathf.Sqrt(2f));
        return velocity;
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
        catapult.Launch(catapult.launchVector.up, InstantaneousVelocity());
        activeCoroutine = null;
    }
    #endregion

    private void Update()
    {
        // TODO: move to catapult
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Reset();
            LaunchFreePlayCannonBall();
        }
    }
}
