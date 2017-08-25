using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace StateLoader
{
    [CreateAssetMenu(menuName = "生成/状态对象")]
    public class StateObjectHolder : ScriptableObject
    {
        public StateLoadType stateLoadType = StateLoadType.Bundle;
        public List<PrefabGroup> prefabList = new List<PrefabGroup>();
        public List<BundleGroup> bundleList = new List<BundleGroup>();
        /// <summary>
        /// 默认只从一个加载方案中加载
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public StateItem[] GetCurrentStateItems(string stateName)
        {
            List<StateItem> items = new List<StateItem>();
            switch (stateLoadType)
            {
                case StateLoadType.Prefab:
                    var pitems = LoadPrefabGroupsItems(stateName);
                    if (pitems != null)
                    {
                        items.AddRange(pitems);
                    }
                    break;
                case StateLoadType.Bundle:
                    var bitems = LoadBundleListGroupsItems(stateName);
                    if (bitems != null)
                    {
                        items.AddRange(bitems);
                    }
                    break;
            }
            return items.ToArray();
        }
        public StateItem[] GetAllStateItems(string stateName)
        {
            List<StateItem> items = new List<StateItem>();
            var pitems = LoadPrefabGroupsItems(stateName);
            if (pitems != null)
            {
                items.AddRange(pitems);
            }
            var bitems = LoadBundleListGroupsItems(stateName);
            if (bitems != null)
            {
                items.AddRange(bitems);
            }
            return items.ToArray();
        }
        /// <summary>
        /// 获取所有需要缓存的状态 
        /// </summary>
        /// <returns></returns>
        public List<string> GetNeedCatchStates()
        {
            var list = new List<string>();
            foreach (var item in prefabList)
            {
                if(item.catchItems)
                {
                    list.Add(item.stateName);
                }
            }
            foreach (var item in bundleList)
            {
                if (item.catchItems)
                {
                    list.Add(item.stateName);
                }
            }
            return list;
        } 
        private StateItem[] LoadPrefabGroupsItems(string stateName,List<string> loadedKeys = null)
        {
            if (loadedKeys == null)
            {
                loadedKeys = new List<string>() { stateName };
            }
            else if(!loadedKeys.Contains(stateName))
            {
                loadedKeys.Add(stateName);
            }
            else
            {
                return null;
            }

            List<StateItem> items = new List<StateItem>();
            var find0 = prefabList.FindAll(x => x.stateName == stateName);
            if (find0 != null)
            {
                var groups = find0.ToArray();
                foreach (var gitem in groups)
                {
                    if (gitem != null)
                    {
                        foreach (var sitem in gitem.itemList)
                        {
                            if (sitem.prefab != null)
                            {
                                items.Add(sitem);
                            }
                        }
                        ///subState
                        foreach (var item in gitem.subStateNames)
                        {
                            var subItems = LoadPrefabGroupsItems(item, loadedKeys);
                            if (subItems != null)
                            {
                                items.AddRange(subItems);
                            }
                        }
                    }
                }
            }
            return items.ToArray();
        }
        private StateItem[] LoadBundleListGroupsItems(string stateName, List<string> loadedKeys = null)
        {
            if (loadedKeys == null)
            {
                loadedKeys = new List<string>() { stateName };
            }
            else if (!loadedKeys.Contains(stateName))
            {
                loadedKeys.Add(stateName);
            }
            else
            {
                return null;
            }

            List<StateItem> items = new List<StateItem>();
            var find = bundleList.FindAll(x => x.stateName == stateName);
            if (find != null)
            {
                var groups = find .ToArray();

                foreach (var bitem in groups)
                {
                    foreach (var sitem in bitem.itemList)
                    {
                        if (!string.IsNullOrEmpty(sitem.assetName) && !string.IsNullOrEmpty(sitem.assetBundleName))
                        {
                            items.Add(sitem);
                        }
                    }
                    ///subState
                    foreach (var item in bitem.subStateNames)
                    {
                        var subItems = LoadBundleListGroupsItems(item, loadedKeys);
                        if (subItems != null)
                        {
                            items.AddRange(subItems);
                        }
                    }
                }
            }
            return items.ToArray();
        }
    }
}