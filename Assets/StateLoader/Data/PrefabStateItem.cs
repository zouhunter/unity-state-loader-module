using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class PrefabStateItem : StateItem
{
    public GameObject prefab;
    public override string ID
    {
        get
        {
            var name = prefab == null ? "Null" : prefab.name;
            if (!reset) {
                return name;
            }
            else
            {
                return string.Format("[{0}][{1}][{2}]", name, JsonUtility.ToJson(position), JsonUtility.ToJson(rotation));
            }
        }
    }
}
