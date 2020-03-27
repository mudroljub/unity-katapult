using System;
using UnityEditor;
using UnityEngine;

public class EditorPhysicsConfig : EditorWindow
{
    public static EditorPhysicsConfig instance;
    public static float CannonBallMass;
    public static float SpringK;

    // Make Window accessible from Window Panel
    [MenuItem("Window/Physics Config Window")]
    public static void ShowWindow()
    {
        GetWindow(typeof(EditorPhysicsConfig));
    }

    private void Awake()
    {
        instance = this;
        CannonBallMass = GameManager.DEFAULT_CANNON_BALL_MASS;

         // Forces the Editor to the aspect ratio that is best used to view the learning module
        GameViewUtils.SwitchToSize(new Vector2(16, 9), GameViewUtils.GameViewSizeType.AspectRatio, "Unity Physics Aspect");
    }

    const float vertStart = 140;
    private void OnGUI()
    {
#if UNITY_EDITOR
        GameManager gm = GameManager.GetInstance();

        GUILayout.Space(vertStart + 40f);

        GUILayoutOption[] massSliderLayout = { GUILayout.Width(350), GUILayout.Height(18) };
        CannonBallMass = EditorGUILayout.Slider("Cannonball Mass", CannonBallMass, 1f, 20, massSliderLayout);

        GUILayoutOption[] springSliderLayout = { GUILayout.Width(350), GUILayout.Height(18) };
        SpringK = EditorGUILayout.Slider("Spring Force", SpringK, 250, 15000, springSliderLayout);

        GUI.enabled = true;

        if (GUI.Button(new Rect(60, 240, 150, 50), GetText(gm)))
        {
            Action callBack = GetCallBack(gm);
            callBack();
        }
#endif
    }

    private void StartPlayEditor()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = true;
#endif       
    }

    private string GetText(GameManager gm)
    {
        if (gm.catapult.launched)
        {
            return "Restart";
        }
        else
        {
            return "Launch Catapault";
        }
    }

    // Returns the correct callback for the execution button based on state of game
    private Action GetCallBack(GameManager gm)
    {
        if (gm.catapult.launched)
        {
            return ResetFreePlayEnvironment;
        }
        else
        {
            return LaunchCatapult;
        }
    }

    private bool IsFreePlay()
    {
        return true;
    }

    private void Update()
    {
        GameManager gm = GameManager.GetInstance();
        if (gm != null)
        {
            if (!gm.lessonStarted)
            {
                StartLesson();
            }

            gm.UpdateCannonBallMass(CannonBallMass);
            gm.UpdateSpringForce(SpringK);
        }
    }

    // Starts a physics lesson that is chosen
    private void StartLesson()
    {
        StartFreePlayBox();
    }

    private void ResetFreePlayEnvironment()
    {
        GameManager.GetInstance().Reset();
        ResetFreePlayBoxes();
    }

    private void LaunchCatapult()
    {
        GameManager.GetInstance().LaunchFreePlayCannonBall();
    }

    private void StartFreePlayBox()
    {
        if (GameManager.GetInstance() != null)
        {
            GameManager.GetInstance().lessonStarted = true;
            GameManager.GetInstance().CurrentPhysicsMode = PhysicsMode.FreePlayBox;
        }
        else
        {
            Debug.Log("Click [Start Playing] Button on the Physics Config Panel to Start the lesson");
        }
    }

    private void ResetFreePlayBoxes()
    {
        GameManager.GetInstance().ExecuteFreePlayBoxMode();
    }
}
