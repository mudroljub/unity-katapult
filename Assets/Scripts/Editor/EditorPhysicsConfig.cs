using UnityEditor;
using UnityEngine;

/* TODO: ukinuti ovaj prozor, prebaciti pucanje na katapult
*/

public class EditorPhysicsConfig : EditorWindow
{
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
        GUILayoutOption[] springSliderLayout = { GUILayout.Width(350), GUILayout.Height(18) };

        if (GUI.Button(new Rect(60, 100, 150, 50), "Launch Catapault"))
        {
            GameManager gm = GameManager.GetInstance();
            gm.Reset();
            gm.LaunchFreePlayCannonBall();
        }
#endif
    }

}
