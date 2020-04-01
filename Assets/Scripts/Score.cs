using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Text scoreText;
    public Catapult catapult;
    public Wall wall;

    void Update()
    {
        byte collapsed = wall.CheckCollapsed();
        scoreText.text = "Ispaljeno: " + catapult.ballsLaunched
            + "\nOboreno: " + collapsed
            + "\nREZULTAT: " + (collapsed - catapult.ballsLaunched);
    }
}
