using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
namespace StateLoader
{
    public abstract class StateItemDrawer : PropertyDrawer
    {
        protected SerializedProperty resetProp;
        protected SerializedProperty instanceIDProp;
        protected SerializedProperty positionProp;
        protected SerializedProperty rotationProp;
        protected void FindCommonPropertys(SerializedProperty property)
        {
            resetProp = property.FindPropertyRelative("reset");
            instanceIDProp = property.FindPropertyRelative("instanceID");
            positionProp = property.FindPropertyRelative("position");
            rotationProp = property.FindPropertyRelative("rotation");
        }
        protected void TryHideItem()
        {
            var gitem = EditorUtility.InstanceIDToObject(instanceIDProp.intValue);
            if (gitem != null)
            {
                var prefab = PrefabUtility.GetPrefabParent(gitem);
                if (prefab != null)
                {
                    var root = PrefabUtility.FindPrefabRoot((GameObject)prefab);
                    if (root != null)
                    {
                        PrefabUtility.ReplacePrefab(gitem as GameObject, root, ReplacePrefabOptions.ConnectToPrefab);
                    }
                }
                GameObject.DestroyImmediate(gitem);
            }
        }
    }


    [CustomPropertyDrawer(typeof(PrefabStateItem))]
    public class PrefabStateItemDrawer : StateItemDrawer
    {
        protected SerializedProperty prefabProp;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            FindCommonPropertys(property);
            prefabProp = property.FindPropertyRelative("prefab");
            return (property.isExpanded ? resetProp.boolValue ? 4 : 2 : 1) * EditorGUIUtility.singleLineHeight;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (prefabProp.objectReferenceValue != null)
            {
                label = new GUIContent(prefabProp.objectReferenceValue.name);
            }
            var rect = new Rect(position.x, position.y, position.width * 0.9f, EditorGUIUtility.singleLineHeight);
            var str = prefabProp.objectReferenceValue == null ? "" : prefabProp.objectReferenceValue.name;
            if (GUI.Button(rect, str, EditorStyles.toolbarDropDown))
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

            if (prefabProp.objectReferenceValue == null)
            {
                EditorGUI.HelpBox(rect, "丢失", MessageType.Error);
            }

            rect = new Rect(position.max.x - position.width * 0.1f, position.y, position.width * 0.1f, EditorGUIUtility.singleLineHeight);
            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                    if (rect.Contains(Event.current.mousePosition))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    }
                    break;
                case EventType.DragPerform:
                    if (rect.Contains(Event.current.mousePosition))
                    {
                        Debug.Log(DragAndDrop.objectReferences.Length);
                        if (DragAndDrop.objectReferences.Length > 0)
                        {
                            var obj = DragAndDrop.objectReferences[0];
                            if (obj is GameObject){
                                prefabProp.objectReferenceValue = obj;
                            }
                            DragAndDrop.AcceptDrag();
                        }
                        Event.current.Use();
                    }
                    break;
                case EventType.DragExited:
                    break;
            }
            if (GUI.Button(rect, "[-]", EditorStyles.objectField))
            {
                if (prefabProp.objectReferenceValue != null)
                {
                    EditorGUIUtility.PingObject(prefabProp.objectReferenceValue);
                }
            }
            //prefabProp.objectReferenceValue = EditorGUI.ObjectField(rect1, prefabProp.objectReferenceValue, typeof(GameObject), false);
            rect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
            if (property.isExpanded)
            {
                EditorGUI.PropertyField(rect, resetProp, true);
                if (resetProp.boolValue)
                {
                    rect.y += EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(rect, positionProp);
                    rect.y += EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(rect, rotationProp);
                }
            }
            //var idRect = new Rect(position.x + position.width * 0.3f, position.y, position.width * 0.3f, EditorGUIUtility.singleLineHeight);
            //EditorGUI.LabelField(idRect, IDProp.stringValue);
        }
        protected void TryCreateItem()
        {
            if (prefabProp.objectReferenceValue == null){
                return;
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
                instanceIDProp.intValue = go.GetInstanceID();
            }
        }
    }

    [CustomPropertyDrawer(typeof(BundleStateItem))]
    public class BundleStateItemDrawer : StateItemDrawer
    {
        private SerializedProperty assetNameProp;
        private SerializedProperty assetBundleNameProp;
        private SerializedProperty guidProp;
        private SerializedProperty goodProp;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            assetNameProp = property.FindPropertyRelative("assetName");
            assetBundleNameProp = property.FindPropertyRelative("assetBundleName");
            guidProp = property.FindPropertyRelative("guid");
            goodProp = property.FindPropertyRelative("good");

            FindCommonPropertys(property);
            return (property.isExpanded ? resetProp.boolValue ? 6 : 4 : 1) * EditorGUIUtility.singleLineHeight;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (Event.current.type == EventType.Repaint)
            {
                var path0 = AssetDatabase.GUIDToAssetPath(guidProp.stringValue);
                var obj0 = AssetDatabase.LoadAssetAtPath<GameObject>(path0);
                goodProp.boolValue = obj0 != null;
            }

            var rect = new Rect(position.x, position.y, position.width * 0.9f, EditorGUIUtility.singleLineHeight);
         
            if (GUI.Button(rect, assetNameProp.stringValue,EditorStyles.toolbarDropDown))
            {
                property.isExpanded = !property.isExpanded;
                if (property.isExpanded)
                {
                    TryCreateItem(assetBundleNameProp.stringValue, assetNameProp.stringValue);
                   
                }
                else
                {
                    TryHideItem();
                }
            }
            if (!goodProp.boolValue)
            {
                EditorGUI.HelpBox(rect, "丢失", MessageType.Error);
            }

            rect = new Rect(position.max.x - position.width * 0.1f, position.y, position.width * 0.1f, EditorGUIUtility.singleLineHeight);
            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                    if (rect.Contains(Event.current.mousePosition))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    }
                    break;
                case EventType.DragPerform:
                    if (rect.Contains(Event.current.mousePosition))
                    {
                        Debug.Log(DragAndDrop.objectReferences.Length);
                        if (DragAndDrop.objectReferences.Length > 0)
                        {
                            var obj = DragAndDrop.objectReferences[0];
                            if (obj is GameObject)
                            {
                                var path = AssetDatabase.GetAssetPath(obj);
                                guidProp.stringValue = AssetDatabase.AssetPathToGUID(path);
                                assetNameProp.stringValue = obj.name;
                                var importer = AssetImporter.GetAtPath(path);
                                assetBundleNameProp.stringValue = importer.assetBundleName;
                            }
                            DragAndDrop.AcceptDrag();
                        }
                        Event.current.Use();
                    }
                    break;
                case EventType.DragExited:
                    break;
            }

            if (GUI.Button(rect, "[-]", EditorStyles.objectField))
            {
                if (!string.IsNullOrEmpty(guidProp.stringValue))
                {
                    var path = AssetDatabase.GUIDToAssetPath(guidProp.stringValue);
                    var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    EditorGUIUtility.PingObject(obj);
                }
            }

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
        }
        protected void TryCreateItem(string assetBundleName, string assetName)
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
                guidProp.stringValue = AssetDatabase.AssetPathToGUID(asset[0]);
                instanceIDProp.intValue = go.GetInstanceID();
            }
            else
            {
                goodProp.boolValue = false;
            }
        }
    }
}