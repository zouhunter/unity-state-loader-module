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
        public event OnStateProgressEvent onStateChanged;
        public event OnStateComplete onStateComplete;

        private StateObjectHolder hold;
        private string currState = "";
        private string lastState = "";
        private Queue<StateItem> needDownLand = new Queue<StateItem>();
        private bool log = true;
        private float totalCount = 1;
        private Dictionary<string, List<StateItem>> itemsDic = new Dictionary<string, List<StateItem>>();
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
        private List<StateItem> LastItems
        {
            get
            {
                if (!itemsDic.ContainsKey(lastState))
                {
                    if (hold.GetCurrentStateItems(lastState) == null)
                    {
                        return new List<StateItem>();
                    }
                    else
                    {
                        itemsDic[lastState] = new List<StateItem>();
                        itemsDic[lastState].AddRange(hold.GetCurrentStateItems(lastState));
                    }
                }
                return itemsDic[lastState];
            }
        }
        private ItemLoadCtrl itemLoadCtrl;
        private Dictionary<string, GameObject> loadedDic = new Dictionary<string, GameObject>();
        private List<GameObject> delyDestroyObjects = new List<GameObject>();
        private List<string> catchStates;
        public StateChangeCtrl(StateObjectHolder hold)
        {
            this.hold = hold;
            itemLoadCtrl = new ItemLoadCtrl();
            catchStates = hold.GetNeedCatchStates();
        }
        public void ChangeState(string state)
        {
            if (currState != state)
            {
                if (log){
                    Debug.Log("当前状态：" + state);
                }
                lastState = currState;
                currState = state;
                ResetLoadingState();
                CreateObjects();
            }  
		
        }
        private void CreateObjects()
        {
            totalCount = needDownLand.Count;
            if (totalCount > 0)
            {
                AsynDownLand(null,null, null);
            }
			else
            {
                OnComplete();
            }
        }
        private void AsynDownLand(string id,string err, GameObject item)
        {
            if (id != null && item != null)
            {
                loadedDic.Add(id,item);
            }

            int count = needDownLand.Count;
            if (count == 0)
            {
               OnComplete();
            }
            else
            {
                var info = needDownLand.Dequeue();
                //当加载的对象多时，使用进度显示
                if (onStateChanged != null)
                    onStateChanged(info, (int)((totalCount - needDownLand.Count) * 100 / totalCount));
                Debug.Log(info.ID);
                itemLoadCtrl.LoadGameObject(info, AsynDownLand);
            }
        }
		///结束
		private void OnComplete()
		{
			 if (onStateComplete != null)
                    onStateComplete();
				
			 while (delyDestroyObjects.Count > 0)
                {
                    var obj = delyDestroyObjects[0];
                    if (obj != null){
                        GameObject.DestroyImmediate(obj);
                    }
                    delyDestroyObjects.RemoveAt(0);
                }
		}
        /// <summary>
        /// 计算当前需要下载的资源
        /// </summary>
        /// <param name="state"></param>
        private void ResetLoadingState()
        {
            //缓存上一次的资源
            if(catchStates.Contains(lastState))
            {

            }
            else
            {
                needDownLand.Clear();
                var loadedKeys = new string[loadedDic.Count];
                loadedDic.Keys.CopyTo(loadedKeys, 0);

                ///删除新状态下不再需要的对象
                foreach (var item in loadedKeys)
                {
                    var info = CurrentItems.Find(x => x.ID == item);
                    if (info == null)
                    {
                        if (loadedDic[item] != null)
                        {
                            delyDestroyObjects.Add(loadedDic[item]);
                        }
                        loadedDic.Remove(item);

                        if (log) Debug.Log("销毁1：" + item);
                    }
                    else
                    {
                        if (log) Debug.Log("保留：" + item);
                    }
                }

                itemLoadCtrl.CansaleLoadAllLoadingObjs();
            }

            ///记录需要加载的资源
            for (int i = 0; i < CurrentItems.Count; i++)
            {
                var info = CurrentItems[i];
                if (!loadedDic.ContainsKey(info.ID)){
                    needDownLand.Enqueue(info);
                }
            }
        }

    }
}