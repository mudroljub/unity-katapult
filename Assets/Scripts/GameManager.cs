using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Camera mainCam;
    public Camera uiCam;
    public Catapult catapult;
    public Wall wall;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.End))
        {
            byte total = wall.CheckCollapsed();
            print("Ukupno je oboreno " + total);
            print("Ukupno je ispaljeno " + catapult.ballsLaunched);
        }
    }
}
