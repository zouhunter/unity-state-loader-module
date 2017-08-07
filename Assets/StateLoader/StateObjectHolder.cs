using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
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
                var pitems = LoadPrefabGroupsItems (stateName);
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
    private StateItem[] LoadPrefabGroupsItems(string stateName)
    {
        List<StateItem> items = new List<StateItem>();
        var find0 = prefabList.FindAll(x => x.stateName == stateName);
        var groups = find0 == null ? null : find0.ConvertAll<StateGroup>(x => x).ToArray();
        if (groups != null)
        {
            foreach (var gitem in groups)
            {
                var pg = gitem as PrefabGroup;
                foreach (var sitem in pg.itemList)
                {
                    if (sitem.prefab != null)
                    {
                        items.Add(sitem);
                    }
                }
            }
        }
        return items.ToArray();
    }
    private StateItem[] LoadBundleListGroupsItems(string stateName)
    {
        List<StateItem> items = new List<StateItem>();
        var find = bundleList.FindAll(x => x.stateName == stateName);
        var groups = find == null ? null : find.ConvertAll<StateGroup>(x => x).ToArray();
        if (groups != null)
        {
            foreach (var bitem in groups)
            {
                var pg = bitem as BundleGroup;
                foreach (var sitem in pg.itemList)
                {
                    if (!string.IsNullOrEmpty(sitem.assetName) && !string.IsNullOrEmpty(sitem.assetBundleName))
                    {
                        items.Add(sitem);
                    }
                }
            }
        }
        return items.ToArray();
    }
}
