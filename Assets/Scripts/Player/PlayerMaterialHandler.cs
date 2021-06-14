using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 *  Class: PlayerMaterialHandler
 *  
 *  Description:
 *  This script handles the player material to apply some cool effects to him.
 *  
 *  Author: Thomas Voce
*/
public class PlayerMaterialHandler : MonoBehaviour
{
    [SerializeField] private List<Material> originMaterials;

    private List<Material> materials = new List<Material>();

    [SerializeField]
    private Material burnMat, iceMat;

    private void Start()
    {
        foreach(Renderer r in GetComponentsInChildren<Renderer>())
            foreach (Material m in r.materials)
                materials.Add(m);
    }

    public void burnMaterials()
    {
        foreach (Material m in materials)
        {
            m.Lerp(m, burnMat, 0.8f);
        }
    }

    public void frozenMaterials()
    {
        foreach (Material m in materials)
        {
            m.Lerp(m, iceMat, 0.8f);
        }
    }

    public void setTransparencyAlpha(float alpha)
    {
        foreach (Material m in materials)
        {
            Color textureColor = m.color;
            textureColor.a = alpha;
            m.color = textureColor;
        }
    }

    public void resetMaterials()
    {
        for(int i = 0; i < materials.Count; ++i) 
            for (int j = 0; j < originMaterials.Count; ++j)
                if (materials[i].name.Split(new []{' ' })[0].Equals(originMaterials[j].name))
                {
                    materials[i].CopyPropertiesFromMaterial(originMaterials[j]);
                    break;
                }
    }

    public void ToFadeMode()
    {
        foreach (Material m in materials)
        {
            ToFadeMode(m);
        }
    }

    //private static void ToOpaqueMode(Material _material)
    //{
    //    _material.SetOverrideTag("RenderType", "");
    //    _material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
    //    _material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
    //    _material.DisableKeyword("_ALPHABLEND_ON");
    //    _material.renderQueue = -1;
    //}

    private static void ToFadeMode(Material _material)
    {
        _material.SetOverrideTag("RenderType", "Transparent");
        _material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        _material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        _material.EnableKeyword("_ALPHABLEND_ON");
        _material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }

    //private void Reset()
    //{
    //    resetMaterials();
    //}

    //private void Awake()
    //{
    //    Messenger.AddListener(GlobalVariables.RESET, Reset);
    //}

    //private void OnDestroy()
    //{
    //    Messenger.RemoveListener(GlobalVariables.RESET, Reset);
    //}
}
