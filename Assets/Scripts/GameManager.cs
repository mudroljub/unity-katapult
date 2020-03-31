using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Camera mainCam;
    public Camera uiCam;
    public Catapult catapult;
    public Cube cube;

    void InitRow(int y)
    {
        for (int i = 0; i < 10; i++)
        {
            Instantiate(cube, new Vector3(140, y, -10 + i * 2), Quaternion.identity);
        }
    }

    void InitWall()
    {
        for (int j = 0; j < 7; j++) InitRow(1 + j * 2);
    }

    void Start()
    {
        InitWall();
    }
}
