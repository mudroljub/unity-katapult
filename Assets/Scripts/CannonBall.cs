using System;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public Transform LeftWeightPoint;
    public Transform CenterWeightPoint;
    public Transform TensionPoint;
    public Transform SpringPoint_LeftOffset;
    public Transform SpringPoint_Centered;
    public Transform ResultPoint_RightOffset;
    public Transform ResultPoint_Centered;
    public Transform PeakHightVelocityMarker;
    public Transform EnergyWidgetMarker_LeftOffset;
    public Transform EnergyWidgetMarker_RightOffset;
    public Transform EnergyWidgetMarker_TopOffset;

    public Rigidbody rigidBody;
    public Vector3 currentVelocity;
    public Vector3 currentAngularVelocity;

    public Vector3 prevPosition;
    public Vector3 currPos;
    public bool paused = false;
    public bool inAir = false;
    public float heightFromGround;
    public bool maxHeightReached = false;
    public Transform step4CamTransform;
    public Transform step5CamTransform;

    int framesSinceHit = 0;

    [ReadOnly] [SerializeField] private float mass = 30;
    public bool PlayTargetHitEffect = false;

    public enum TargetColor
    {
        Undefined = -1,
        Black = 0,
        White = 1,
        Blue = 2,
        Red = 3,
        Yellow = 4
    }

    public TargetColor highestHitColor = TargetColor.Undefined;

    // Calculate Gravitational Potential Energy (m * g * h) -> mass x gravity x height
    public float Gravitational_Potential_Energy()
    {
        return Mathf.Abs((float)Math.Round(mass * Physics.gravity.y * heightFromGround, 2));
    }

    // Weight of the Cannonball in Neutons (mass x gravity)
    public float WeightForce
    {
        get
        {
            return Mathf.Abs(Mathf.Round(mass * Physics.gravity.y));  // return the rounded Absolute value of the the Cannon Balls Weight in Force Newtons
        }
    }

    public float Mass
    {
        get
        {
            return mass;
        }
        set
        {
            mass = value;
            rigidBody.mass = mass;
            GameManager.GetInstance().ShowMassChange();
        }
    }

    public void PauseInAir()
    {
        currentVelocity = rigidBody.velocity;
        currentAngularVelocity = rigidBody.angularVelocity;
        rigidBody.isKinematic = true;
        paused = true;
    }

    public void Reset()
    {
        paused = false;
        inAir = false;
        PlayTargetHitEffect = false;
        maxHeightReached = false;
        transform.rotation = Quaternion.identity;
        rigidBody.useGravity = true;
        rigidBody.isKinematic = false;
        rigidBody.drag = 0;
        highestHitColor = TargetColor.Undefined;
        framesSinceHit = 0;
    }

    public void Resume()
    {
        paused = false;
        rigidBody.isKinematic = false;
        rigidBody.velocity = currentVelocity;
        rigidBody.angularVelocity = currentAngularVelocity;
    }

    private void Update()
    {
        // always calculate height from ground
        heightFromGround = Mathf.Abs(transform.position.y - GameManager.GetInstance().terrain.transform.position.y);
    }

    private void LateUpdate()
    {
        if(inAir)
        {
            prevPosition = transform.position;
            transform.rotation = Quaternion.identity;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Terrain":
                inAir = false;
                rigidBody.drag = 2;
                break;

            case "Yellow":
                if (framesSinceHit == 0)
                {
                    highestHitColor = TargetColor.Yellow;
                }
                break;

            case "Red":
                if(framesSinceHit == 0 && (int)highestHitColor < (int)TargetColor.Red)
                {
                    highestHitColor = TargetColor.Red;
                }
                break;

            case "Blue":
                if (framesSinceHit == 0 && (int)highestHitColor < (int)TargetColor.Blue)
                {
                    highestHitColor = TargetColor.Blue;
                }
                break;

            case "White":
                if (framesSinceHit == 0 && (int)highestHitColor < (int)TargetColor.White)
                {
                    highestHitColor = TargetColor.White;
                }

                break;

            case "Black":
                if (framesSinceHit == 0 && (int)highestHitColor < (int)TargetColor.Black)
                {
                    highestHitColor = TargetColor.Black;
                }

                break;
        }
    }


}
