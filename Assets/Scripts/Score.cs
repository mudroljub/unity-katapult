using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Catapult catapult;
    public Wall wall;
    public Text scoreText;
    public Text totalScore;

    void Update()
    {
        byte collapsed = wall.CheckCollapsed();
        scoreText.text = "Ispaljeno: " + catapult.ballsLaunched
            + "\nOboreno: " + collapsed;
        totalScore.text = "REZULTAT: " + (collapsed - catapult.ballsLaunched);
    }
}
