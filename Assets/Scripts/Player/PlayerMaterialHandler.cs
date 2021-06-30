using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 *  Class: PlayerMaterialHandler
 *  
 *  Description:
 *  This script handles the player material to apply some cool effects on him.
 *  
 *  Author: Thomas Voce
*/
public class PlayerMaterialHandler : MonoBehaviour
{
    // this material are used to reset the materials at the end of an animation
    [SerializeField] private List<Material> originMaterials;

    private List<Material> materials = new List<Material>();

    [SerializeField] private Material burnMat, iceMat;

    // first of all, I load all materials of the player
    private void Start()
    {
        foreach(Renderer r in GetComponentsInChildren<Renderer>())
            foreach (Material m in r.materials)
                materials.Add(m);
    }

    // when player receive a BURN attack, I lerp materials with burn material
    public void burnMaterials()
    {
        foreach (Material m in materials)
        {
            m.Lerp(m, burnMat, 0.8f);
        }
    }

    // when player receive a FROZEN attack, I lerp materials with frozen material
    public void frozenMaterials()
    {
        foreach (Material m in materials)
        {
            m.Lerp(m, iceMat, 0.8f);
        }
    }

    // when player is in invulnerability state, I change the alpha materials
    public void setTransparencyAlpha(float alpha)
    {
        foreach (Material m in materials)
        {
            Color textureColor = m.color;
            textureColor.a = alpha;
            m.color = textureColor;
        }
    }

    // at the end of an animation, I reset the material copying properties from the original material
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

    // I have to change mode material (changing some property of the material) in order to apply transparency (change alpha)
    public void ToFadeMode()
    {
        foreach (Material m in materials)
        {
            ToFadeMode(m);
        }
    }

    // this method is not more required because resetting the material, the opaque mode will be automatically resetted 

    //private static void ToOpaqueMode(Material _material)
    //{
    //    _material.SetOverrideTag("RenderType", "");
    //    _material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
    //    _material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
    //    _material.DisableKeyword("_ALPHABLEND_ON");
    //    _material.renderQueue = -1;
    //}

    // change mode material to FADE manually
    private static void ToFadeMode(Material _material)
    {
        _material.SetOverrideTag("RenderType", "Transparent");
        _material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        _material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        _material.EnableKeyword("_ALPHABLEND_ON");
        _material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }
}
