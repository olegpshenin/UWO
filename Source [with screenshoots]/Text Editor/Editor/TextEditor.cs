/* ---------------------------------------
 * Author:          Oleg Pshenin (oleg.pshenin@gmail.com)
 * Project:         Text Editor (https://github.com/olegpshenin/TextEditor)
 * Date:            06-May-19
 * Studio:          Pillbox Game Studio
 * 
 * This project is released under the MIT license.
 * -------------------------------------*/

using System.IO;
using UnityEditor;
using UnityEngine;

namespace PB
{
    public class TextEditor : EditorWindow
    {
        private Vector2 _scrollPos;
        private static string _content;
        private static Object _selectedObject;
        private static string _path;

        [MenuItem("Assets/Edit Text", true)]
        private static bool ValidateEditTextAsset()
        {
            return CurrentSelectionValidation();
        }

        [MenuItem("Assets/Edit Text")]
        private static void EditTextAsset(MenuCommand menuCommand)
        {
            ReadFile();
            ShowWindow();
        }

        private static bool CurrentSelectionValidation()
        {
            _path = "";

            var selectedObjects = Selection.objects;
            if (selectedObjects.Length == 1)
            {
                _selectedObject = selectedObjects[0];
                _path = AssetDatabase.GetAssetPath(_selectedObject.GetInstanceID());

                if (_path.Length > 0)
                {
                    if (File.Exists(_path))
                    {
                        return true;
                    }
                }
            }

            _selectedObject = null;
            return false;
        }

        private static void ShowWindow()
        {
            var window = GetWindow<TextEditor>();
            window.titleContent = new GUIContent("Text Editor");
            window.Show();
        }

        private static void ReadFile()
        {
            StreamReader reader = new StreamReader(_path);
            _content = reader.ReadToEnd();
            reader.Close();
        }

        private static void WriteToFile()
        {
            Undo.RecordObject(_selectedObject, "Editing Text File");

            StreamWriter writer = new StreamWriter(_path, false);
            writer.Write(_content);
            writer.Close();

            AssetDatabase.ImportAsset(_path);
        }

        private void OnGUI()
        {
            if (_selectedObject != null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(_path, EditorStyles.boldLabel);
                EditorGUILayout.ObjectField(_selectedObject, typeof(object), false, GUILayout.Width(200f));
                EditorGUILayout.EndHorizontal();

                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
                _content = EditorGUILayout.TextArea(_content);
                EditorGUILayout.EndScrollView();

                ShowButtons();
            }
            else
            {

                EditorGUILayout.LabelField("You need to select an asset at first!", EditorStyles.boldLabel);
            }
        }

        private void ShowButtons()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();

            if (GUILayout.Button("Update selection"))
            {
                UpdateSelection();
            }

            if (GUILayout.Button("Clean all"))
            {
                _content = "";
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();

            if (GUILayout.Button("Save and Close"))
            {
                WriteToFile();
                Close();
            }

            if (GUILayout.Button("Cancel and Close"))
            {
                Close();
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void OnSelectionChange()
        {
            Debug.Log("selection changed");
            if (_selectedObject == null)
            {
                UpdateSelection();
            }
        }

        private static void UpdateSelection()
        {
            if (CurrentSelectionValidation())
                EditTextAsset(null);
        }
    }
}