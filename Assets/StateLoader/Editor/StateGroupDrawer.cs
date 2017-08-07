using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Rotorz.ReorderableList;
using System.IO;
using System;

[CustomPropertyDrawer(typeof(StateGroup), true)]
public class StateGroupDrawer : PropertyDrawer
{
    const float widthBt = 20;
    protected SerializedProperty stateNameProp;
    protected SerializedProperty itemListProp;
    protected DragAdapt dragAdapt;
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        stateNameProp = property.FindPropertyRelative("stateName");
        itemListProp = property.FindPropertyRelative("itemList");
        dragAdapt = new DragAdapt(itemListProp, itemListProp.type);
        return EditorGUIUtility.singleLineHeight + ReorderableListGUI.CalculateListFieldHeight(itemListProp);
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(rect, stateNameProp);
        var rectLeft = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 1.3f, position.width, position.height - EditorGUIUtility.singleLineHeight);
        ReorderableListGUI.ListFieldAbsolute(rectLeft, dragAdapt);
        var loadRect = new Rect(position.x + position.width * 0.8f, position.y + position.height - EditorGUIUtility.singleLineHeight, position.width * 0.1f, EditorGUIUtility.singleLineHeight);
        if (GUI.Button(loadRect, "O", EditorStyles.miniButtonRight))
        {
            LoadObjectsToProperty();
        }
    }
    private void LoadObjectsToProperty()
    {
        if (Selection.activeObject != null)
        {
            if (ProjectWindowUtil.IsFolder(Selection.activeInstanceID))
            {
                var path = AssetDatabase.GetAssetPath(Selection.activeObject);
                List<string> objects = new List<string>();
                Recursive(path, "prefab", true, (x) => { objects.Add(x); });

                foreach (var item in objects)
                {
                    string rootpath = item.Replace(Application.dataPath, "Assets");
                    itemListProp.InsertArrayElementAtIndex(0);
                    var prop = itemListProp.GetArrayElementAtIndex(0);
                    var prefabProp = prop.FindPropertyRelative("prefab");
                    prefabProp.objectReferenceValue = AssetDatabase.LoadAssetAtPath<GameObject>(rootpath);
                    if (itemListProp.type == typeof(BundleStateItem).ToString())
                    {
                        AssetImporter importer = AssetImporter.GetAtPath(rootpath);
                        var assetNameProp = prop.FindPropertyRelative("assetName");
                        var assetBundleNameProp = prop.FindPropertyRelative("assetBundleName");
                        assetNameProp.stringValue = prefabProp.objectReferenceValue.name;
                        assetBundleNameProp.stringValue = importer.assetBundleName;
                    }
                }
            }
        }
    }
    /// <summary>
    /// 遍历目录及其子目录
    /// </summary>
    public static void Recursive(string path, string fileExt, bool deep = true, Action<string> action = null)
    {
        string[] names = Directory.GetFiles(path);
        foreach (string filename in names)
        {
            string ext = Path.GetExtension(filename);
            if (ext.ToLower().Contains(fileExt.ToLower()))
                action(filename.Replace('\\', '/'));
        }
        if (deep)
        {
            string[] dirs = Directory.GetDirectories(path);
            foreach (string dir in dirs)
            {
                Recursive(dir, fileExt, deep, action);
            }
        }

    }
}
