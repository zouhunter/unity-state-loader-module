using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Rotorz.ReorderableList;
namespace StateLoader
{
    [CustomEditor(typeof(StateObjectHolder))]
    public class StateObjectDrawer : Editor
    {
        private SerializedProperty scriptProp;
        private SerializedProperty prefabListProp;
        private SerializedProperty bundleListPorp;
        private SerializedProperty stateLoadTypeProp;
        string[] stateLoadTypeKeys = Enum.GetNames(typeof(StateLoadType));
        private static List<GameObject> created = new List<GameObject>();
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
                    RemoveDouble();
                }
                else if (GUILayout.Button("更新", EditorStyles.miniButton))
                {
                    QuickUpdate();
                }
                else if (GUILayout.Button("加载", EditorStyles.miniButton))
                {
                    GroupPreview();
                }
                else if (GUILayout.Button("保存", EditorStyles.miniButton))
                {
                    CloseAllCreated();
                }
                else if (GUILayout.Button("排序", EditorStyles.miniButton))
                {
                    SortAllBundles();
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

        /// <summary>
        /// 去重复（需要判断 预制体/reset/position/rotation）
        /// 而不同的group之间则需要删除重复的对象，公用唯一的对象
        /// </summary>
        private void RemoveDouble()
        {
            StateObjectHolder obj = target as StateObjectHolder;
            for (int i = 0; i < obj.prefabList.Count; i++)
            {
                var item = obj.prefabList[i];
            compair: List<string> temp = new List<string>();
                for (int j = 0; j < item.itemList.Count; j++)
                {
                    var targetItem = item.itemList[j];
                    var ID = targetItem.ID;

                    if (!temp.Contains(ID))
                    {
                        temp.Add(ID);
                    }
                    else
                    {
                        item.itemList.Remove(targetItem);
                        goto compair;
                    }
                }

            }
            for (int i = 0; i < obj.bundleList.Count; i++)
            {
                var item = obj.bundleList[i];
            compair: List<string> temp = new List<string>();
                for (int j = 0; j < item.itemList.Count; j++)
                {
                    var targetItem = item.itemList[j];
                    var ID = targetItem.ID;

                    if (!temp.Contains(ID))
                    {
                        temp.Add(ID);
                    }
                    else
                    {
                        item.itemList.Remove(targetItem);
                        goto compair;
                    }
                }

            }
            EditorUtility.SetDirty(target);
        }
        /// <summary>
        /// 更新bundle名
        /// </summary>
        private void QuickUpdate()
        {
            StateObjectHolder obj = target as StateObjectHolder;
            for (int i = 0; i < obj.bundleList.Count; i++)
            {
                foreach (var item in obj.bundleList[i].itemList)
                {
                    if (item.prefab == null)
                    {
                        UnityEditor.EditorUtility.DisplayDialog("空对象", item.ID + "预制体为空", "确认");
                        continue;
                    }

                    string assetPath = UnityEditor.AssetDatabase.GetAssetPath(item.prefab);

                    UnityEditor.AssetImporter importer = UnityEditor.AssetImporter.GetAtPath(assetPath);
                    item.assetName = item.prefab.name;

                    if (string.IsNullOrEmpty(item.assetName))
                    {
                        UnityEditor.EditorUtility.DisplayDialog("提示", "预制体" + item.assetName + "没有assetBundle标记", "确认");
                        return;
                    }
                    else
                    {
                        item.assetBundleName = importer.assetBundleName;
                    }
                }

            }
            EditorUtility.SetDirty(target);

        }
        /// <summary>
        /// 分类预览
        /// </summary>
        private void GroupPreview()
        {
            Dictionary<string, GameObject> parentDic = new Dictionary<string, GameObject>();

            StateObjectHolder obj = target as StateObjectHolder;
            foreach (var item in obj.prefabList)
            {
                if (!parentDic.ContainsKey(item.stateName))
                {
                    var parent = new GameObject(item.stateName);
                    parentDic.Add(item.stateName, parent);
                    created.Add(parent);
                }
                foreach (var pitem in item.itemList)
                {
                    if (pitem.prefab != null)
                    {
                        var pInstence = PrefabUtility.InstantiatePrefab(pitem.prefab);
                        (pInstence as GameObject).transform.SetParent(parentDic[item.stateName].transform);
                        created.Add(pInstence as GameObject);
                    }
                }
            }

            foreach (var item in obj.bundleList)
            {
                if (!parentDic.ContainsKey(item.stateName))
                {
                    var parent = new GameObject(item.stateName);
                    parentDic.Add(item.stateName, parent);
                    created.Add(parent);
                }
                foreach (var pitem in item.itemList)
                {
                    if (pitem.prefab != null)
                    {
                        var pInstence = PrefabUtility.InstantiatePrefab(pitem.prefab);
                        (pInstence as GameObject).transform.SetParent(parentDic[item.stateName].transform);
                        created.Add(pInstence as GameObject);
                    }
                }
            }
            EditorUtility.SetDirty(obj);
        }
        /// <summary>
        /// 保存并关闭
        /// </summary>
        private void CloseAllCreated()
        {
            if (created == null)
            {
                return;
            }
            TrySaveAllPrefabs();
            for (int i = 0; i < created.Count; i++)
            {
                if (created[i] != null)
                {
                    DestroyImmediate(created[i]);
                }
            }
            created.Clear();
        }
        private void SortAllBundles()
        {
            StateObjectHolder obj = target as StateObjectHolder;
            foreach (var item in obj.bundleList)
            {
                for (int i = 0; i < item.itemList.Count; i++)
                {
                    for (int j = i; j < item.itemList.Count - i - 1; j++)
                    {
                        var itemj = item.itemList[j];
                        var itemj1 = item.itemList[j + 1];
                        if (string.Compare(itemj.assetName, itemj1.assetName) > 0)
                        {
                            var temp = itemj;
                            item.itemList[j] = item.itemList[j + 1];
                            item.itemList[j + 1] = temp;
                        }
                    }
                }
            }

            foreach (var item in obj.prefabList)
            {
                for (int i = 0; i < item.itemList.Count; i++)
                {
                    for (int j = i; j < item.itemList.Count - i - 1; j++)
                    {
                        var itemj = item.itemList[j];
                        var itemj1 = item.itemList[j + 1];

                        if (itemj.prefab != null && itemj1.prefab != null)
                        {
                            if (string.Compare(itemj.prefab.name, itemj1.prefab.name) > 0)
                            {
                                var temp = itemj;
                                item.itemList[j] = item.itemList[j + 1];
                                item.itemList[j + 1] = temp;
                            }
                        }

                    }
                }
            }

            EditorUtility.SetDirty(obj);
        }
        private void TrySaveAllPrefabs()
        {
            foreach (var item in created)
            {
                var prefab = PrefabUtility.GetPrefabParent(item);
                if (prefab != null)
                {
                    var root = PrefabUtility.FindPrefabRoot((GameObject)prefab);
                    if (root != null)
                    {
                        PrefabUtility.ReplacePrefab(item, root, ReplacePrefabOptions.ConnectToPrefab);
                    }
                }
            }
        }
    }
}