using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Rotorz.ReorderableList;

[CustomEditor(typeof(StateObjectHolder))]
public class StateObjectDrawer : Editor
{
    private SerializedProperty scriptProp;
    private SerializedProperty prefabListProp;
    private SerializedProperty bundleListPorp;
    private SerializedProperty stateLoadTypeProp;
    string[] stateLoadTypeKeys = Enum.GetNames(typeof(StateLoadType));
    private void OnEnable()
    {
        scriptProp = serializedObject.FindProperty("m_Script");
        prefabListProp = serializedObject.FindProperty("prefabList");
        bundleListPorp = serializedObject.FindProperty("bundleList");
        stateLoadTypeProp = serializedObject.FindProperty("stateLoadType");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(scriptProp);
        DrawStateLoadType();
        DrawToolButtons();
        switch ((StateLoadType)stateLoadTypeProp.enumValueIndex)
        {
            case StateLoadType.Prefab:
                DrawPrefabList();
                break;
            case StateLoadType.Bundle:
                DrawBundleLists();
                break;
            default:
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawToolButtons()
    {
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("去重", EditorStyles.miniButton))
            {

            }
            else if (GUILayout.Button("更新",EditorStyles.miniButton))
            {

            }
            else if (GUILayout.Button("加载", EditorStyles.miniButton))
            {

            }
            else if (GUILayout.Button("保存", EditorStyles.miniButton))
            {

            }
            else if (GUILayout.Button("排序", EditorStyles.miniButton))
            {

            }
        }
    }

    private void DrawStateLoadType()
    {
        stateLoadTypeProp.enumValueIndex = GUILayout.Toolbar(stateLoadTypeProp.enumValueIndex, stateLoadTypeKeys, EditorStyles.toolbarButton);
    }
    private void DrawPrefabList()
    {
        ReorderableListGUI.Title("预制体列表");
        ReorderableListGUI.ListField(prefabListProp);
    }
    private void DrawBundleLists()
    {
        ReorderableListGUI.Title("资源包列表");
        ReorderableListGUI.ListField(bundleListPorp);
    }
}
