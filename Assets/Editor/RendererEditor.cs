using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class RendererEditor : Editor
{

    [MenuItem("Tools/RendererSettingWithNoLightAndShadow")]
    public static void RenderSettingWithNoLightAndShadow()
    {
        Transform transform = Selection.activeTransform;


        foreach (var render in transform.GetComponentsInChildren<Renderer>())
        {
            render.shadowCastingMode = ShadowCastingMode.Off;
            render.receiveShadows = false;
        }

    }



}
