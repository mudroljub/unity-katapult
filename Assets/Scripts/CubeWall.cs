using System.Collections.Generic;
using UnityEngine;

public class CubeWall : MonoBehaviour
{
    Dictionary<Transform, Vector3> initialPositions = new Dictionary<Transform, Vector3>();

    void Awake()
    {
        InitPositions();
    }

    void InitPositions()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.tag == "cube")
            {
                initialPositions.Add(child.transform, child.position);
            }
        }
    }

    void Reset()
    {
        foreach(KeyValuePair<Transform, Vector3> cube in initialPositions)
        {
            cube.Key.position = cube.Value;
            cube.Key.rotation = Quaternion.identity;
            Rigidbody rigidBody = cube.Key.GetComponent<Rigidbody>();
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Home))
        {
            Reset();
        }
    }
}
