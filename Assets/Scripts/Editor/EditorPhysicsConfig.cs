using UnityEditor;
using UnityEngine;

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
        //if (GUI.Button(new Rect(60, 100, 150, 50), "Launch Catapault"))
        //{
        //    GameManager gm = GameManager.GetInstance();
        //    gm.Reset();
        //    gm.LaunchFreePlayCannonBall();
        //}
#endif
    }

}
