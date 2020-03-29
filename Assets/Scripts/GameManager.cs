using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject terrain;

    [Header("Scene References")]
    public Camera mainCam;
    public Camera uiCam;

    [SerializeField] private CubeWall cubeWall;
    [SerializeField] public Catapult catapult;
    [SerializeField] public CannonBall cannonBall;
}
