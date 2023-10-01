using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BoxedConvexCollider))]
public class BoxedConvexColliderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        BoxedConvexCollider myS = (BoxedConvexCollider)target;
        if (myS.colliderCalculated)
        {
            EditorGUILayout.LabelField("Mesh Count: " + myS.colliderMeshCount);
            if(myS.colliderMeshCount >= 500)
            {
                EditorGUILayout.HelpBox("Mesh count is over 500. This may cause some performance issues. Consider lowering the mesh precision or increase the collider minimum size.", MessageType.Warning);
            }
            EditorGUILayout.LabelField("Mesh Precision: " + myS.colliderMeshPrecision);
            EditorGUILayout.LabelField("Mesh Minimum Size: " + myS.colliderMeshMinSize);
            myS._colliderIsTrigger = EditorGUILayout.Toggle("Is Trigger:", myS.colliderIsTrigger);
            if (myS.colliderIsTrigger != myS._colliderIsTrigger)
            {
                myS.colliderIsTrigger = myS._colliderIsTrigger;
                myS.ReloadTrigger();
            }
            myS._colliderEnableCollider = EditorGUILayout.Toggle("Enable Collider:", myS.colliderEnableCollider);
            if (myS.colliderEnableCollider != myS._colliderEnableCollider)
            {
                myS.colliderEnableCollider = myS._colliderEnableCollider;
                myS.ReloadEnable();
            }
            myS.colliderShowCollider = EditorGUILayout.Toggle("Show Collider:", myS.colliderShowCollider);
            myS.ReloadShow();
        }
        if (GUILayout.Button("Calculate Colliders"))
        {
            myS.CalculateCollider();
        }
        if (GUILayout.Button("Remove Colliders"))
        {
            myS.RemoveColliders();
        }
        if (myS.colliderCalculated)
        {
            if(GUILayout.Button("Preview Collider Min Size Change"))
            {
                myS.PreviewColliderMinSizeChange();
            }
            if (GUILayout.Button("Execute Collider Min Size Change"))
            {
                myS.ExecuteColliderMinSizeChange();
            }
        }
    }
}
