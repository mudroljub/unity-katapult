using System.Collections.Generic;
using UnityEngine;

public class CubeWall : MonoBehaviour
{
    public Dictionary<Transform, Vector3> cubeWallPositions = new Dictionary<Transform, Vector3>();

    private void Awake()
    {
        // init wall
        foreach(Transform child in transform)
        {
            if (child.gameObject.tag == "cube")
            {
                cubeWallPositions.Add(child.transform, child.position);
            }
        }
    }

    // reset to initial positions
    public void Reset()
    {
        foreach(KeyValuePair<Transform, Vector3> cube in cubeWallPositions)
        {
            cube.Key.position = cube.Value;
            cube.Key.rotation = Quaternion.identity;
            Rigidbody rigidBody = cube.Key.GetComponent<Rigidbody>();
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("kolizija");
    //    switch (collision.gameObject.tag)
    //    {
    //        case "ball":
    //            Debug.Log("Zid pogodjen");
    //            break;

    //        case "Terrain":
    //            Debug.Log("Dodirnut pod");
    //            break;
    //    }
    //}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Home))
        {
            Reset();
        }
    }
}
