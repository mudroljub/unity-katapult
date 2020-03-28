using System.Collections.Generic;
using UnityEngine;

public class CubeWall : MonoBehaviour
{
    public List<Rigidbody> boxRigidBodies = new List<Rigidbody>();
    public Dictionary<Transform, Vector3> cubeWallPositions = new Dictionary<Transform, Vector3>();
    public List<GameObject> middleRowBoxes = new List<GameObject>();

    private void Awake()
    {
        foreach(Transform child in transform)
        {
            if (child.gameObject.tag == "cube")
            {
                cubeWallPositions.Add(child.transform, child.position);
            }
        }
    }

    // reset all boxes to initial positions
    public void Reset()
    {
        foreach(KeyValuePair<Transform, Vector3> pos in cubeWallPositions)
        {
            pos.Key.position = pos.Value;
            pos.Key.rotation = Quaternion.identity;
            Rigidbody rigidBody = pos.Key.GetComponent<Rigidbody>();
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
        }
    }

    public void ShowCenterRowOnly()
    {
        Reset();
        // Hide all boxes first
        foreach (Rigidbody rigidBody in boxRigidBodies)
        {
            rigidBody.gameObject.SetActive(false);
        }

        foreach (GameObject centerBox in middleRowBoxes)
        {
            centerBox.SetActive(true);
        }
    }

    public void ShowAllBoxes()
    {
        foreach (Rigidbody rigidBody in boxRigidBodies)
        {
            rigidBody.gameObject.SetActive(true);
        }
    }
}
