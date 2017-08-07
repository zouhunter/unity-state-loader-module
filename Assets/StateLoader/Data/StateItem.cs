using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
public abstract class StateItem  {
    public abstract string IDName { get; }
    public bool reset;
    public Vector3 position;
    public Vector3 rotation;
}
