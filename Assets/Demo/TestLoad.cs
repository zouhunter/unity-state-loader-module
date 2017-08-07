using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class TestLoad : MonoBehaviour {
    public StateObjectHolder hold;
    private StateChangeCtrl changeCtrl;
    private string[] states = {"状态一","状态二"};
	// Use this for initialization
	void Start () {
        changeCtrl = new StateChangeCtrl(hold);
    }

    // Update is called once per frame
    void OnGUI () {
        foreach (var item in states)
        {
            if (GUILayout.Button(item))
            {
                changeCtrl.ChangeState(item);
            }
        }
	}
}
