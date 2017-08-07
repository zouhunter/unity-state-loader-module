using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Rotorz.ReorderableList;
[CustomPropertyDrawer(typeof(StateGroup),true)]
public class StateGroupDrawer : PropertyDrawer
{
    const float widthBt = 20;
    protected SerializedProperty stateNameProp;
    protected SerializedProperty itemListProp;
    protected DragAdapt dragAdapt;
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        stateNameProp = property.FindPropertyRelative("stateName");
        itemListProp = property.FindPropertyRelative("itemList");
        dragAdapt = new DragAdapt(itemListProp, itemListProp.type);
        return EditorGUIUtility.singleLineHeight +  ReorderableListGUI.CalculateListFieldHeight(itemListProp);
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(rect, stateNameProp);
        var rectLeft = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight *1.3f, position.width, position.height - EditorGUIUtility.singleLineHeight);
        ReorderableListGUI.ListFieldAbsolute(rectLeft, dragAdapt);
    }
}
