using UnityEngine;

public class Cube : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "ball":
                Debug.Log("Kocka pogodjena");
                break;

            case "Terrain":
                Debug.Log("Dodirnut pod");
                break;
        }
    }
}
