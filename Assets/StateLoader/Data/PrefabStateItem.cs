using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace StateLoader
{
    [System.Serializable]
    public class PrefabStateItem : StateItem
    {
#if UNITY_EDITOR
        public int instanceID;
#endif
        public GameObject prefab;
        public override string ID
        {
            get
            {
                if (string.IsNullOrEmpty(_id))
                {
                    var name = prefab == null ? "Null" : prefab.name;
                    if (!reset)
                    {
                        _id = name;
                    }
                    else
                    {
                        _id = string.Format("[{0}][{1}][{2}]", name, position.GetHashCode(), rotation.GetHashCode());
                    }
                }
                return _id;
            }
        }
    }
}
