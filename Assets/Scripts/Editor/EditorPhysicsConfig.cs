using System;
using UnityEditor;
using UnityEngine;

/* TODO: ukinuti ovaj prozor, prebaciti pucanje na katapult
    - razdvojiti zavisnot od GameManager-a
*/

public class EditorPhysicsConfig : EditorWindow
{
    public static float CannonBallMass = 1;
    public static float SpringK;
    private GameManager gm;

    // Make Window accessible from Window Panel
    [MenuItem("Window/Physics Config Window")]
    public static void ShowWindow()
    {
        GetWindow(typeof(EditorPhysicsConfig));
    }

    private void OnGUI()
    {
#if UNITY_EDITOR
        GUILayout.Space(40f);

        GUILayoutOption[] massSliderLayout = { GUILayout.Width(350), GUILayout.Height(18) };
        CannonBallMass = EditorGUILayout.Slider("Cannonball Mass", CannonBallMass, 1f, 20, massSliderLayout);

        GUILayoutOption[] springSliderLayout = { GUILayout.Width(350), GUILayout.Height(18) };
        SpringK = EditorGUILayout.Slider("Spring Force", SpringK, 250, 15000, springSliderLayout);

        if (GUI.Button(new Rect(60, 100, 150, 50), GetText(gm)))
        {
            if (gm.catapult.launched)
            {
                Reset();
            }
            else
            {
                LaunchCatapult();
            }
        }
#endif
    }

    private string GetText(GameManager gm)
    {
        gm = GameManager.GetInstance();
        return gm.catapult.launched ? "Restart" : "Launch Catapault";
    }

    private void Update()
    {
        gm = GameManager.GetInstance();
        gm.UpdateCannonBallMass(CannonBallMass);
        gm.UpdateSpringForce(SpringK);
    }

    private void Reset()
    {
        gm.Reset();
        gm.ExecuteFreePlayBoxMode();
    }

    private void LaunchCatapult()
    {
        gm.LaunchFreePlayCannonBall();
    }

}
