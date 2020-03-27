using UnityEditor;
using UnityEngine;

/* TODO: ukinuti ovaj prozor, prebaciti pucanje na katapult
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

        if (GUI.Button(new Rect(60, 100, 150, 50), "Launch Catapault"))
        {
            //if (gm.catapult.launched)
            //{
            //    gm.Reset();
            //    gm.ExecuteFreePlayBoxMode();
            //}
            //else
            //{
                gm.Reset();
                gm.LaunchFreePlayCannonBall();
            //}
        }
#endif
    }

    private void Update()
    {
        gm = GameManager.GetInstance();
        gm.UpdateCannonBallMass(CannonBallMass);
        gm.UpdateSpringForce(SpringK);
    }

}
