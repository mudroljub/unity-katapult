﻿/*
- okretanje levo desno
- da ne nestaje djule
*/
using UnityEngine;
using System.Collections;

public class Catapult : MonoBehaviour
{
    public float springForce = 15000f;
    public float launchAngle = 45;
    public float launchSpeed = 10f;
    public float rotationSpeed = 1f;

    public CannonBall cannonBall;
    public GameObject catapultArm;

    [Header("Internal References")]
    public Transform cannonBallPos;
    public Transform launchVector;

    private float currentArmAngle = 45f;
    public bool launching = false;
    public bool lowering = false;
    private float initialForce;
 
    private Quaternion armInitRotation;
    private Coroutine activeCoroutine;

    void Start()
    {
        armInitRotation = catapultArm.transform.rotation;
        initialForce = springForce;
        Reset();
    }

    void Reset()
    {
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        //launching = false;
        springForce = initialForce;
        cannonBall.Reset(catapultArm.transform, cannonBallPos.position);
    }

    float InstantaneousVelocity()
    {
        float mass = cannonBall.rigidBody.mass;
        float angle = launchAngle * Mathf.Deg2Rad;
        // √(springForce / m) * angle² - (g * √2)
        return Mathf.Sqrt(springForce / mass * Mathf.Pow(angle, 2) - Physics.gravity.y * Mathf.Sqrt(2f));
    }

    void Launch()
    {
        cannonBall.Launch(launchVector.up, InstantaneousVelocity());
    }

    IEnumerator DoProcessLaunch()
    {
        yield return new WaitWhile(() => launching);
        Launch();
        activeCoroutine = null;
    }

    void LaunchCannonBall()
    {
        launching = true;
        activeCoroutine = StartCoroutine(DoProcessLaunch());
    }

    void MoveArmUp()
    {
        if (currentArmAngle >= launchAngle)
        {
            launching = false;
            return;
        }

        float step = Time.deltaTime * 500;
        currentArmAngle += step;
        catapultArm.transform.Rotate(-Vector3.up, step);
    }

    void MoveArmDown()
    {
        if (currentArmAngle <= 0)
        {
            lowering = false;
            return;
        }

        float step = Time.deltaTime * 50;
        currentArmAngle -= step;
        catapultArm.transform.Rotate(-Vector3.down, step);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Reset();
            lowering = true;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            springForce += 100;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            LaunchCannonBall();
            lowering = false;
        }

        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
        transform.Rotate(0, rotation, 0);

        if (launching) MoveArmUp();
        if (lowering) MoveArmDown();
    }
}
