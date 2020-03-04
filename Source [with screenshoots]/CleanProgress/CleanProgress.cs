using System.IO;
using UnityEngine;
using UnityEditor;

public class CleanProgress : ScriptableObject
{
    [MenuItem("Pillbox M/Clean User Progress", false, 0)]
    public static void CleanUserProgress()
    {
        bool wipe = true;
        if (EditorApplication.isPlaying)
        {
            wipe = EditorUtility.DisplayDialog("Wipe", "Do you want to stop Playing to Clean user progress?", "Yes", "No");
        }

        if (wipe && EditorUtility.DisplayDialog("Wipe", "Your Game progress would be wiped out\nAre you sure to continue?", "Yes", "No"))
        {
            EditorApplication.isPlaying = false;
            try
            {
                PlayerPrefs.DeleteKey("Save");
                Debug.Log ("User progress cleaned");
            }
            catch (System.Exception)
            {
                // ignored
            }
        }
    }
}
