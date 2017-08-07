using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace StateLoader
{
    public delegate void OnLoadItemEvent(string err, GameObject item);
    public delegate void OnStateProgressEvent(StateItem info, int progress);
    public delegate void OnStateComplete();
}