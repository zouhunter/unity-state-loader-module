using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BundleStateItem : StateItem
{
#if UNITY_EDITOR
    public GameObject prefab;
#endif
    public string assetName;
    public string assetBundleName;
    public override string IDName
    {
        get
        {
            return assetBundleName + assetName;
        }
    }
}
