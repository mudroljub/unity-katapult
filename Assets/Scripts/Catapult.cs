/*
 * da ne nestaje djule
 * da se vrte točkovi
*/
using UnityEngine;
using System.Collections;

public class Catapult : MonoBehaviour
{
    public float springForce = 5000;
    public float launchSpeed = 10f;
    public float rotationSpeed = 15f;
    public float speed = 1f;
    public float maxAngle = 315;
    public float minAngle = 272;

    public CannonBall cannonBall;
    public GameObject catapultArm;

    [Header("Internal References")]
    public Transform cannonBallPos;
    public Transform launchVector;

    public bool launching = false;
    public bool lowering = false;
    private float initialForce; 
    private Coroutine activeCoroutine;

    void Start()
    {
        initialForce = springForce;
        Reset();
    }

    void Reset()
    {
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        launching = false;
        springForce = initialForce;
        cannonBall.Reset(catapultArm.transform, cannonBallPos.position);
    }

    float InstantaneousVelocity()
    {
        float launchAngle = catapultArm.transform.rotation.eulerAngles.x - 270;
        float mass = cannonBall.rigidBody.mass;
        float angle = launchAngle * Mathf.Deg2Rad;
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
        if (catapultArm.transform.rotation.eulerAngles.x >= maxAngle)
        {
            launching = false;
            return;
        }

        float step = Time.deltaTime * 500;
        catapultArm.transform.Rotate(-Vector3.up, step);
    }

    void MoveArmDown()
    {
        if (catapultArm.transform.rotation.eulerAngles.x <= minAngle)
        {
            lowering = false;
            return;
        }

        float step = Time.deltaTime * 50;
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

        // turn left / right
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, rotation, 0);
        // move
        float translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        transform.Translate(-translation, 0, 0);

        if (launching) MoveArmUp();
        if (lowering) MoveArmDown();
    }
}
