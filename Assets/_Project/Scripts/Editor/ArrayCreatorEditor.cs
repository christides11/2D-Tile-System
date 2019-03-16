using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class ArrayCreatorEditor : EditorWindow
{
    TileCollection tc;

    [MenuItem("Window/TextureArray Creator")]
    public static void OpenWindow()
    {
        EditorWindow.GetWindow(typeof(ArrayCreatorEditor));
    }

    void OnGUI()
    {
        tc = (TileCollection)EditorGUILayout.ObjectField("Tile Collection", tc, typeof(TileCollection), false);

        if (!tc)
        {
            return;
        }

        
    }
}
