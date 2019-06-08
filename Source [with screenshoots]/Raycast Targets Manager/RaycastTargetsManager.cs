/* ---------------------------------------
 * Author:          Oleg Pshenin (oleg.pshenin@gmail.com)
 * Project:         Raycast Targets Manager (https://github.com/olegpshenin/RaycastTargetsManager)
 * part of:         Unity Workflow Optimization (https://github.com/olegpshenin/UWO)
 *
 * Date:            27-April-19
 * Studio:          Pillbox Game Studio
 * 
 * This project is released under the MIT license.
 * -------------------------------------*/

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace PB
{
    public class RaycastTargetsManager : EditorWindow
    {
        private const string TITLE_TEXT = "Raycast Targets Manager";
        private const string HEADER_LABEL_TEXT = "List of all raycast targets inside ";
        private const string WITH_CONNECTION_TEXT = "Conencted to:";
        private const string WITHOUT_CONNECTION_TEXT = "No connection";
        private const string ENABLE_ALL_BUTTON_TEXT = "Enable Connected";
        private const string DISABLE_ALL_BUTTON_TEXT = "Disable All";
        private const string APPLY_BUTTON_TEXT = "Apply";
        private const string CANCEL_BUTTON_TEXT = "Cancel";

        private static Graphic[] _graphicComponents;
        private static Dictionary<Graphic, Selectable> _connectedSelectablesDictionary = new Dictionary<Graphic, Selectable>();
        private static bool[] _toggleArray;
        private static Transform _root;
        private static Vector2 _scrollPos;


        [MenuItem("GameObject/Manage Raycast Targets", false, -3)]
        private static void ManageRaycastTargets(MenuCommand menuCommand)
        {
            if ((menuCommand.context as GameObject) != null)
            {
                _root = (menuCommand.context as GameObject).transform;
            }
            else if (Selection.gameObjects.Length == 1)
            {
                _root = Selection.gameObjects[0].transform;
            }

            if (_root == null)
            {
                Debug.LogError("Select an object at first");
                return;
            }

            InitArrays();
            RefillToggleArray();
            ShowWindow();
        }

        private static void InitArrays()
        {
            _graphicComponents = _root.GetComponentsInChildren<Graphic>(true);

            _connectedSelectablesDictionary.Clear();
            var allSelectables = _root.GetComponentsInChildren<Selectable>(true);
            foreach (var selectable in allSelectables)
            {
                if (selectable.targetGraphic != null)
                {
                    if (!_connectedSelectablesDictionary.ContainsKey(selectable.targetGraphic))
                    {
                        _connectedSelectablesDictionary.Add(selectable.targetGraphic, selectable);
                    }
                }
            }
        }

        private static void ShowWindow()
        {
            var window = GetWindow<RaycastTargetsManager>();
            window.titleContent = new GUIContent(TITLE_TEXT);
            window.minSize = new Vector2(514, 356);
            window.Show();
        }

        private static void RefillToggleArray()
        {
            _toggleArray = new bool[_graphicComponents.Length];
            for (int i = 0; i < _toggleArray.Length; i++)
            {
                _toggleArray[i] = _graphicComponents[i].raycastTarget;
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField(HEADER_LABEL_TEXT + _root.gameObject.name + ":", EditorStyles.boldLabel);

            ShowComponentsList();
            ShowButtons();
        }

        private float GetToogleWidth()
        {
            return 15f;
        }

        private float GetComponentsWidth()
        {
            return GetScalableWidth() * 0.5f;
        }

        private float GetConnectionTextWidth()
        {
            return 100f;
        }

        private float GetConnectionComponentsWidth()
        {
            return GetScalableWidth() * 0.5f;
        }

        private float GetScalableWidth()
        {
            return Screen.width - GetToogleWidth() - GetConnectionTextWidth() - 50f;
        }

        private void ShowComponentsList()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height - 90f));
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleLeft;

            for (int i = 0; i < _graphicComponents.Length; i++)
            {
                EditorGUILayout.BeginHorizontal(style, GUILayout.ExpandWidth(false));

                EditorGUILayout.ObjectField(_graphicComponents[i], typeof(Graphic), false, GUILayout.Width(GetComponentsWidth()));

                if (_connectedSelectablesDictionary.ContainsKey(_graphicComponents[i]))
                {
                    GUILayout.Label(WITH_CONNECTION_TEXT, GUILayout.Width(GetConnectionTextWidth()));
                    EditorGUILayout.ObjectField(_connectedSelectablesDictionary[_graphicComponents[i]], typeof(Selectable), false, GUILayout.Width(GetConnectionComponentsWidth()));
                }
                else
                {
                    GUILayout.Label(WITHOUT_CONNECTION_TEXT, GUILayout.Width(GetConnectionTextWidth()));
                    GUILayout.Label("", GUILayout.Width(GetConnectionComponentsWidth()));
                }

                GUILayout.FlexibleSpace();

                _toggleArray[i] = EditorGUILayout.Toggle(_toggleArray[i], GUILayout.Width(GetToogleWidth()));

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();
        }

        private void ShowButtons()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(GUILayout.Width(Screen.width / 2 - 5f));

            if (GUILayout.Button(ENABLE_ALL_BUTTON_TEXT))
            {
                EnableAllWithConnection();
            }

            if (GUILayout.Button(DISABLE_ALL_BUTTON_TEXT))
            {
                ChangeStateOfAllRaycastTargets(false);
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUILayout.Width(Screen.width / 2 - 5f));

            if (GUILayout.Button(APPLY_BUTTON_TEXT))
            {
                ApplyRaycastTargetStates();
                Close();
            }

            if (GUILayout.Button(CANCEL_BUTTON_TEXT))
            {
                Close();
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void ChangeStateOfAllRaycastTargets(bool state)
        {
            for (int i = 0; i < _graphicComponents.Length; i++)
            {
                _toggleArray[i] = state;
            }
        }

        private void EnableAllWithConnection()
        {
            for (int i = 0; i < _toggleArray.Length; i++)
            {
                var haveConnection = _connectedSelectablesDictionary.ContainsKey(_graphicComponents[i]);
                _toggleArray[i] = haveConnection;
            }
        }

        private void ApplyRaycastTargetStates()
        {
            for (int i = 0; i < _graphicComponents.Length; i++)
            {
                Undo.RecordObjects(_graphicComponents, "Applying raycast states");
                _graphicComponents[i].raycastTarget = _toggleArray[i];
            }
        }
    }
}