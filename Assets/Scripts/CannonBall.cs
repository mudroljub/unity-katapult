using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public Rigidbody rigidBody;
    public bool inAir = false;

    void Start()
    {
        rigidBody.mass = 20f;
    }

    public void Reset()
    {
        inAir = false;
        rigidBody.useGravity = true;
        rigidBody.drag = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Terrain":
                inAir = false;
                rigidBody.drag = 2;
                break;

            case "cube":
                Debug.Log("Hit wall");
                break;
        }
    }


}
