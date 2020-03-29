/*
- ispaljuje na puštanje dugmeta, od dužine držanja zavisi snaga
- okretanje levo desno
*/
using UnityEngine;
using System.Collections;

public class Catapult : MonoBehaviour
{
    public float springForce = 15000f;
    public Vector3 defaultForce = new Vector3(-0.8f, 0.6f, 0.0f);
    public float defaultVelocity = 13f;

    public CannonBall cannonBall;
    public GameObject catapultArm;
    public Transform launchVector;

    public const float launchSpeed = 5f;
    public float launchAngle = 45;
    public float currentArmAngle = 0f;

    public bool ballLaunched = false;
    public bool launched = false;
    Quaternion armInitRotation;
    Coroutine activeCoroutine;

    [Header("Internal References")]
    public Transform cannonBallPos;

    void Start()
    {
        armInitRotation = catapultArm.transform.rotation;
        Reset();
    }

    void Reset()
    {
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        launched = ballLaunched = false;
        currentArmAngle = 0;
        catapultArm.transform.rotation = armInitRotation;
        cannonBall.Reset(catapultArm.transform, cannonBallPos.position);
    }

    float InstantaneousVelocity()
    {
        float mass = cannonBall.rigidBody.mass;
        float angle = launchAngle * Mathf.Deg2Rad;
        // formula: √(springForce / m) * angle² - (g * √2)
        return Mathf.Sqrt(springForce / mass * Mathf.Pow(angle, 2) - Physics.gravity.y * Mathf.Sqrt(2f));
    }

    public void Launch()
    {
        launched = true;
        cannonBall.Launch(launchVector.up, InstantaneousVelocity());
    }

    public IEnumerator DoProcessLaunch()
    {
        yield return new WaitWhile(() => ballLaunched);
        Launch();
        activeCoroutine = null;
    }

    public void LaunchCannonBall()
    {
        ballLaunched = true;
        activeCoroutine = StartCoroutine(DoProcessLaunch());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Reset();
            LaunchCannonBall();
        }

        if (ballLaunched)
        {
            if (currentArmAngle >= launchAngle)
            {
                ballLaunched = false;
                return;
            }

            float total = Time.deltaTime * launchAngle * launchSpeed;
            currentArmAngle += total;
            catapultArm.transform.Rotate(-Vector3.up, total);
        };
    }
}
