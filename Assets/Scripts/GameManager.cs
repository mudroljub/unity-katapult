using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject terrain;
    Coroutine activeCoroutine;

    [Header("Scene References")]
    public Camera mainCam;
    public Camera uiCam;

    [SerializeField] private CubeWall cubeWall;
    [SerializeField] public Catapult catapult;
    [SerializeField] public CannonBall cannonBall;

    private void Start()
    {
        Reset();
    }

    public void Reset()
    {
        if(activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }
        catapult.Reset();
        cannonBall.Reset();
    }

    public void LaunchFreePlayCannonBall()
    {
        catapult.launchSpeed = Catapult.LAUNCH_SPEED;
        catapult.throwCalled = true;
        activeCoroutine = StartCoroutine(DoProcessFreePlayLaunch());
    }

    #region FreePlay
    public IEnumerator DoProcessFreePlayLaunch()
    {
        yield return new WaitWhile(() => { return catapult.throwCalled; });
        catapult.Launch();
        activeCoroutine = null;
    }
    #endregion

    private void Update()
    {
        // TODO: move to catapult
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Reset();
            LaunchFreePlayCannonBall();
        }
    }
}
