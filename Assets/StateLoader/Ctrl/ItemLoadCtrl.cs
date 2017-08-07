using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class ItemLoadCtrl
{
#if AssetBundleTools
    private AssetBundleLoader assetLoader;
#endif
    private List<string> _loadingKeys = new List<string>();
    private List<string> _cansaleKeys = new List<string>();

    public ItemLoadCtrl()
    {
#if AssetBundleTools
        assetLoader = AssetBundleLoader.Instence;
#endif
    }
    public ItemLoadCtrl(string url, string menu)
    {
#if AssetBundleTools
        assetLoader = AssetBundleLoader.GetInstance(url, menu);
#endif
    }

    /// <summary>
    /// 创建对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="assetName"></param>
    /// <param name="onCreate"></param>
    public void LoadGameObject(StateItem itemInfo, OnLoadItemEvent onLoad)
    {
        if (_cansaleKeys.Contains(itemInfo.IDName)) _cansaleKeys.RemoveAll(x => x == itemInfo.IDName);

        if (!_loadingKeys.Contains(itemInfo.IDName))
        {
            _loadingKeys.Add(itemInfo.IDName);
            var bInfo = itemInfo as BundleStateItem;
            var pInfo = itemInfo as PrefabStateItem;

            if (bInfo != null)
            {
                LoadGameObject(bInfo, onLoad);
            }
            else if (pInfo != null)
            {
                LoadGameObject(pInfo, onLoad);
            }
        }
    }
    /// <summary>
    /// 取消创建对象
    /// </summary>
    /// <param name="idName"></param>
    public void CansaleLoadObject(string idName)
    {
        _cansaleKeys.Add(idName);
    }
    public void CansaleLoadAllLoadingObjs()
    {
        _cansaleKeys.AddRange(_loadingKeys);
    }
    /// <summary>
    /// BundleInfo创建对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="assetName"></param>
    /// <param name="onCreate"></param>
    private void LoadGameObject(BundleStateItem trigger, OnLoadItemEvent onLoad)
    {
#if AssetBundleTools
        assetLoader.LoadAssetFromUrlAsync<GameObject>(trigger.assetBundleName, trigger.assetName, (x) =>
        {
            string err = null;
            GameObject item = null;
            if (x != null)
            {
                item= CreateInstance(x, trigger);
                _loadingKeys.Remove(trigger.IDName);
            }
            else
            {
                err = trigger.IDName + "-->空";
            }

            if (onLoad != null)
            {
                onLoad.Invoke(err,item);
            }
        });
#endif
    }
    /// <summary>
    /// PrefabInfo创建对象
    /// </summary>
    /// <param name="iteminfo"></param>
    private void LoadGameObject(PrefabStateItem trigger, OnLoadItemEvent onLoad)
    {
        string err = null;
        GameObject item = null;
        if (trigger.prefab != null)
        {
            item = CreateInstance(trigger.prefab, trigger);
            _loadingKeys.Remove(trigger.IDName);
        }
        else
        {
            err = trigger.IDName + "-->空";
        }
        if (onLoad != null)
        {
            onLoad.Invoke(err, item);
        }
    }

    /// <summary>
    /// 获取对象实例
    /// </summary>
    private GameObject CreateInstance(GameObject prefab, StateItem trigger)
    {
        if (_cansaleKeys.Contains(trigger.IDName))
        {
            _cansaleKeys.Remove(trigger.IDName);
            return null;
        }

        if (prefab == null || trigger == null)
        {
            return null;
        }

        GameObject go = GameObject.Instantiate(prefab);

        go.SetActive(true);
        if (trigger.reset)
        {
            go.transform.localPosition = trigger.position;
            go.transform.localEulerAngles = trigger.rotation;
        }
        return go;
    }

}
