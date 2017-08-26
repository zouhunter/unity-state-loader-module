using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace StateLoader
{
    [System.Serializable]
    public class BundleStateItem : StateItem
    {
#if UNITY_EDITOR
        public int instanceID;
        public bool good;
        public string guid;
#endif
        public string assetName;
        public string assetBundleName;
        public override string ID
        {
            get
            {
                if(string.IsNullOrEmpty(_id))
                {
                    if (!reset)
                    {
                        _id = string.Format("[{0}][{1}]", assetBundleName, assetName);
                    }
                    else
                    {
                        _id = string.Format("[{0}][{1}][{2}][{3}]", assetBundleName, assetName, position.GetHashCode(), rotation.GetHashCode());
                    }
                }
                return _id;
            }
        }
    }
}