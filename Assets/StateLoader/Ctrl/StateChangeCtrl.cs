using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace StateLoader
{
    public class StateChangeCtrl
    {
        private StateObjectHolder hold;
        private string currState = "";
        private static List<GameObject> loadedObjs = new List<GameObject>();
        private Queue<StateItem> needDownLand = new Queue<StateItem>();
        private bool log = true;
        private float totalCount = 1;
        private Dictionary<string, List<StateItem>> itemsDic = new Dictionary<string, List<StateItem>>();
        public event OnStateProgressEvent onStateChanged;
        public event OnStateComplete onStateComplete;
        private List<StateItem> CurrentItems
        {
            get
            {
                if (!itemsDic.ContainsKey(currState))
                {
                    if (hold.GetCurrentStateItems(currState) == null)
                    {
                        return new List<StateItem>();
                    }
                    else
                    {
                        itemsDic[currState] = new List<StateItem>();
                        itemsDic[currState].AddRange(hold.GetCurrentStateItems(currState));
                    }
                }

                return itemsDic[currState];
            }
        }
        //private List<StateItem> LastItems
        //{
        //    get
        //    {
        //        if (!itemsDic.ContainsKey(lastState))
        //        {
        //            if (hold.GetCurrentStateItems(lastState) == null)
        //            {
        //                return new List<StateItem>();
        //            }
        //            else
        //            {
        //                itemsDic[lastState] = new List<StateItem>();
        //                itemsDic[lastState].AddRange(hold.GetCurrentStateItems(lastState));
        //            }
        //        }
        //        return itemsDic[lastState];
        //    }
        //}

        private ItemLoadCtrl itemLoadCtrl;
        public StateChangeCtrl(StateObjectHolder hold)
        {
            this.hold = hold;
            itemLoadCtrl = new ItemLoadCtrl();
        }
        public void ChangeState(string state)
        {
            if (currState != state)
            {
                if (log)
                {
                    Debug.Log("当前状态：" + state);
                }
                currState = state;
                CreateObjects(currState);
            }
        }
        private void CreateObjects(string state)
        {
            ResetLoadingState(state);
            totalCount = needDownLand.Count;
            if (totalCount > 0)
            {
                AsynDownLand(null, null);
            }
        }
        private void AsynDownLand(string err, GameObject item)
        {
            if (item != null)
            {
                loadedObjs.Add(item);
            }

            int count = needDownLand.Count;
            if (count == 0)
            {
                if (onStateComplete != null)
                    onStateComplete();
            }
            else
            {
                var info = needDownLand.Dequeue();
                //当加载的对象多时，使用进度显示
                if (onStateChanged != null)
                    onStateChanged(info, (int)((totalCount - needDownLand.Count) * 100 / totalCount));
                itemLoadCtrl.LoadGameObject(info, AsynDownLand);
            }
        }
        private void ResetLoadingState(string state)
        {
            itemLoadCtrl.CansaleLoadAllLoadingObjs();
#if !UNITY_EDITOR
        log = false;
#endif
            foreach (var item in loadedObjs)
            {
                if (item != null) GameObject.DestroyImmediate(item);
            }
            loadedObjs.Clear();

            needDownLand.Clear();
            foreach (var item in CurrentItems)
            {
                needDownLand.Enqueue(item);
            }
        }

    }
}