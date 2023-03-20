using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using static Gems;
using System;

[CustomEditor(typeof(GemManager))]
public class GemManagerEditor : Editor
{
    SerializedProperty gemMesh;
    SerializedProperty gemName;


    Material[] gemMaterials;

    private void OnEnable()
    {
        serializedObject.Update();

        gemMesh = serializedObject.FindProperty("CubeRenderer");
        if (gemMesh != null)
        {
            gemMesh.objectReferenceValue = (target as GemManager).GetComponentInChildren<MeshRenderer>();
        }

        gemName = serializedObject.FindProperty("gemType"); 

        gemMaterials = Resources.LoadAll<Material>("GemMaterials");

        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        base.OnInspectorGUI();

        //ChangeMaterial(gemName.enumValueIndex);

        serializedObject.ApplyModifiedProperties();
    }

    void ChangeMaterial(int number)
    {
        MeshRenderer mesh = gemMesh.objectReferenceValue as MeshRenderer;
            mesh.material = gemMaterials[number];
    }
}
