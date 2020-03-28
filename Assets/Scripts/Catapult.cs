using UnityEngine;

public class Catapult : MonoBehaviour
{
    public float springForce = 15000f;
    public Vector3 defaultForce = new Vector3(-0.8f, 0.6f, 0.0f);
    public float defaultVelocity = 13f;

    public CannonBall cannonBall;
    public GameObject catapultArm;
    public Transform launchVector;
    public GameObject rope;

    public const float LAUNCH_SPEED_FREEPLAY = 5f;
    public float DEFAULT_LAUNCH_ANGLE = 45;
    public float launchSpeed = 0.5f;

    public float currentArmAngle = 0f;
    public float launchAngle;

    private Quaternion armInitRotation;
    public bool throwCalled = false;
    public bool launched = false;

    [Header("Internal References")]
    public Transform cannonBallPos;

    private void Awake()
    {
        armInitRotation = catapultArm.transform.rotation;
    }

    public void Reset()
    {
        launched = throwCalled = false;
        launchAngle = DEFAULT_LAUNCH_ANGLE;
        currentArmAngle = 0;
        rope.SetActive(true);
        catapultArm.transform.rotation = armInitRotation;
        cannonBall.SetPosition(catapultArm.transform, cannonBallPos.position);
    }

    public void Launch(Vector3 forceVector, float velocity)
    {
        launched = true;
        cannonBall.Launch(forceVector, velocity);
    }

    private void Update()
    {
        if (throwCalled)
        {
             if (currentArmAngle >= launchAngle)
             {
                throwCalled = false;
                return;
             }
            float total = Time.deltaTime * DEFAULT_LAUNCH_ANGLE * launchSpeed;
             currentArmAngle += total;
             catapultArm.transform.Rotate(-Vector3.up, total );
        }
    }
}
