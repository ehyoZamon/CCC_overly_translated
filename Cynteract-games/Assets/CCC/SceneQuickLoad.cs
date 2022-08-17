using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
public class SceneQuickLoad : MonoBehaviour
{
    [MenuItem("Scenes/CCC")]
    public static void OpenCCC()
    {
        string scenePath = Application.dataPath + "/CCC/Scenes/CCC.unity";
        EditorSceneManager.OpenScene(scenePath);
    }
    [MenuItem("Scenes/Games/Tunnelrunner")]
    public static void OpenTunnelrunner()
    {
        string scenePath = Application.dataPath + "/Games/Tunnelrunner/TunnelrunnerMain.unity";
        EditorSceneManager.OpenScene(scenePath);

    }
    [MenuItem("Scenes/Games/Sphererunner")]
    public static void OpenSphererunner()
    {
        string scenePath = Application.dataPath + "/Games/Sphererunner/Sphererunner.unity";
        EditorSceneManager.OpenScene(scenePath);
    }
    [MenuItem("Scenes/Games/Cannongame")]
    public static void OpenCannongame()
    {
        string scenePath = Application.dataPath + "/Games/Cannongame/Cannongame.unity";
        EditorSceneManager.OpenScene(scenePath);
    }
    [MenuItem("Scenes/Games/RoadCrosser")]
    public static void OpenRoadCrosser()
    {
        string scenePath = Application.dataPath + "/Games/RoadCrosser/RoadCrosser.unity";
        EditorSceneManager.OpenScene(scenePath);
    }
    [MenuItem("Scenes/Games/Platformer")]
    public static void OpenPlatformer()
    {
        string scenePath = Application.dataPath + "/Games/Platformer/Platformer.unity";
        EditorSceneManager.OpenScene(scenePath);
    }
    [MenuItem("Scenes/Games/SpaceInvaders")]
    public static void OpenSpaceInvaders()
    {
        string scenePath = Application.dataPath + "/Games/SpaceInvaders/Scenes/SpaceInvaders.unity";
        EditorSceneManager.OpenScene(scenePath);
    }
    [MenuItem("Scenes/Usermanagement")]
    public static void OpenUsermanagement()
    {
        string scenePath = Application.dataPath + "/Database/UserManagement/UserManagement.unity";
        EditorSceneManager.OpenScene(scenePath);
    }
}
#endif