using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace StateLoader
{
    public abstract class StateItem
    {
        protected string _id;
        public abstract string ID { get; }
        public bool reset;
        public Vector3 position;
        public Vector3 rotation;
    }
}