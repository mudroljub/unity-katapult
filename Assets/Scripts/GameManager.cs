using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public Camera mainCam;
    public Camera uiCam;
    public Catapult catapult;
    public Cube cube;

    Dictionary<Cube, Vector3> initialPositions = new Dictionary<Cube, Vector3>();

    void Start()
    {
        InitWall(7);
    }

    void InitRow(int y)
    {
        for (byte i = 0; i < 10; i++)
        {
            Cube newCube = Instantiate(cube, new Vector3(140, y, -10 + i * 2), Quaternion.identity);
            initialPositions.Add(newCube, new Vector3(140, y, -10 + i * 2));
        }
    }

    void InitWall(byte rows)
    {
        for (byte j = 0; j < rows; j++) InitRow(1 + j * 2);
    }

    void CheckDistance()
    {
        byte counter = 0;
        foreach (KeyValuePair<Cube, Vector3> item in initialPositions)
        {
            float dist = Vector3.Distance(item.Key.transform.position, item.Value);
            if (dist > 2) counter++;
        }
        print("Ukupno je oboreno " + counter);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.End))
        {
            CheckDistance();
        }
    }
}
