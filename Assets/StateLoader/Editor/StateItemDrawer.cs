using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public abstract class StateItemDrawer: PropertyDrawer
{
    protected SerializedProperty resetProp;
    protected SerializedProperty prefabProp;
    protected SerializedProperty positionProp;
    protected SerializedProperty rotationProp;
    static Dictionary<int, List<GameObject>> created = new Dictionary<int, List<GameObject>>();

    protected void FindPropertys(SerializedProperty property)
    {
        resetProp = property.FindPropertyRelative("reset");
        prefabProp = property.FindPropertyRelative("prefab");
        positionProp = property.FindPropertyRelative("position");
        rotationProp = property.FindPropertyRelative("rotation");
    }
    protected void TryHideItem()
    {
        var id = prefabProp.objectReferenceInstanceIDValue;
        if (created.ContainsKey(id))
        {
            var item = created[id];
           
            foreach (var gitem in item)
            {
                if (gitem != null)
                {
                    var prefab = PrefabUtility.GetPrefabParent(gitem);
                    if (prefab != null)
                    {
                        var root = PrefabUtility.FindPrefabRoot((GameObject)prefab);
                        if (root != null)
                        {
                            PrefabUtility.ReplacePrefab(gitem, root, ReplacePrefabOptions.ConnectToPrefab);
                        }
                    }
                    GameObject.DestroyImmediate(gitem.gameObject);
                }
            }
           
        }
        created.Remove(id);
    }
    protected bool TryCreateItem()
    {
        if (prefabProp.objectReferenceValue == null)
        {
            return false;
        }
        GameObject gopfb = prefabProp.objectReferenceValue as GameObject;
        if (gopfb != null)
        {
            GameObject go = PrefabUtility.InstantiatePrefab(gopfb) as GameObject;
            if (resetProp.boolValue)
            {
                go.transform.localPosition = positionProp.vector3Value;
                go.transform.localEulerAngles = rotationProp.vector3Value;
            }
            var id = prefabProp.objectReferenceInstanceIDValue;
            if(created.ContainsKey(id))
            {
                created[id].Add(go);
            }
            else
            {
                created.Add(id, new List<GameObject>() { go });
            }
            return true;
        }
        return false;
    }
    protected GameObject TryCreateItem(string assetBundleName,string assetName)
    {
        var asset = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
        if (asset != null && asset.Length > 0)
        {
            var gopfb = AssetDatabase.LoadAssetAtPath<GameObject>(asset[0]);
            GameObject go = PrefabUtility.InstantiatePrefab(gopfb) as GameObject;
            if (resetProp.boolValue)
            {
                go.transform.localPosition = positionProp.vector3Value;
                go.transform.localEulerAngles = rotationProp.vector3Value;
            }
            var id = prefabProp.objectReferenceInstanceIDValue;
            if (created.ContainsKey(id))
            {
                created[id].Add(go);
            }
            else
            {
                created.Add(id, new List<GameObject>() { go });
            }
            return gopfb;
        }
        return null;
    }
}

[CustomPropertyDrawer(typeof(PrefabStateItem))]
public class PrefabStateItemDrawer : StateItemDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      
        FindPropertys(property);
        return (property.isExpanded ? resetProp.boolValue ? 4 : 2 : 1) * EditorGUIUtility.singleLineHeight;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (prefabProp.objectReferenceValue != null)
        {
            label = new GUIContent(prefabProp.objectReferenceValue.name);
        }
        var rect = new Rect(position.x, position.y, position.width * 0.3f, EditorGUIUtility.singleLineHeight);
        var rect1 = rect;
        rect1.x += position.width * 0.6f;
        if (GUI.Button(rect, label))
        {
            property.isExpanded = !property.isExpanded;
            if (property.isExpanded)
            {
               TryCreateItem();
            }
            else
            {
                TryHideItem();
            }
        }
        
        prefabProp.objectReferenceValue = EditorGUI.ObjectField(rect1, prefabProp.objectReferenceValue, typeof(GameObject), false);

        var rect2 = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
        if (property.isExpanded)
        {
            EditorGUI.PropertyField(rect2, resetProp, true);
            if (resetProp.boolValue)
            {
                rect2.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect2,positionProp);
                rect2.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect2, rotationProp);
            }
        }
        //var idRect = new Rect(position.x + position.width * 0.3f, position.y, position.width * 0.3f, EditorGUIUtility.singleLineHeight);
        //EditorGUI.LabelField(idRect, IDProp.stringValue);
    }
}

[CustomPropertyDrawer(typeof(BundleStateItem))]
public class BundleStateItemDrawer : StateItemDrawer
{
    private SerializedProperty assetNameProp;
    private SerializedProperty assetBundleNameProp;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        assetNameProp = property.FindPropertyRelative("assetName");
        assetBundleNameProp = property.FindPropertyRelative("assetBundleName");

        FindPropertys(property);
        return (property.isExpanded ? resetProp.boolValue?6:4 : 1) * EditorGUIUtility.singleLineHeight;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (prefabProp.objectReferenceValue != null)
        {
            label = new GUIContent(prefabProp.objectReferenceValue.name);
        }
        var rect = new Rect(position.x, position.y, position.width * 0.3f, EditorGUIUtility.singleLineHeight);
        var rect1 = rect;
        rect1.x += position.width * 0.6f;
        if (GUI.Button(rect, label))
        {
            property.isExpanded = !property.isExpanded;
            if (property.isExpanded)
            {
                if (!TryCreateItem())
                {
                    prefabProp.objectReferenceValue =
                      TryCreateItem(assetBundleNameProp.stringValue, assetNameProp.stringValue);
                }
            }
            else
            {
                TryHideItem();
            }
        }

        prefabProp.objectReferenceValue = EditorGUI.ObjectField(rect1, prefabProp.objectReferenceValue, typeof(GameObject), false);

        var rect2 = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
        if (property.isExpanded)
        {
           

            EditorGUI.PropertyField(rect2, assetNameProp, true);
            rect2.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect2, assetBundleNameProp, true);
            rect2.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect2, resetProp, true);

            if (resetProp.boolValue)
            {
                rect2.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect2, positionProp);
                rect2.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect2, rotationProp);
            }
        }
        //var idRect = new Rect(position.x + position.width * 0.3f, position.y, position.width * 0.3f, EditorGUIUtility.singleLineHeight);
        //EditorGUI.LabelField(idRect, IDProp.stringValue);

    }
}