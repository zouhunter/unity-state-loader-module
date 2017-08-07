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
    public override string IDName
    {
        get
        {
            return prefab.name;
        }
    }
}
