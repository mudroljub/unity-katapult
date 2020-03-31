using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public Rigidbody rigidBody;
    public bool inAir = false;

    void Start()
    {
        rigidBody.mass = 20f;
    }

    public void Place(Transform armTransform, Vector3 position)
    {
        inAir = false;
        rigidBody.useGravity = true;
        rigidBody.drag = 0;
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        transform.parent = armTransform;
        transform.position = position;
    }

    public void Launch(Vector3 forceVector, float velocity)
    {
        transform.SetParent(null);
        rigidBody.constraints = RigidbodyConstraints.None;
        rigidBody.AddForce(forceVector * (velocity * rigidBody.mass), ForceMode.Impulse);
        inAir = true;
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    switch (collision.gameObject.tag)
    //    {
    //        case "Terrain":
    //            inAir = false;
    //            rigidBody.drag = 2;
    //            break;

    //        case "cube":
    //            Debug.Log("Hit wall");
    //            break;
    //    }
    //}

}
