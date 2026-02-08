using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    //we assign all the renderers here through the inspector
    [SerializeField]
    private List<Renderer> renderers;
    [SerializeField]
    private Color color = Color.white;

    //helper list to cache all the materials ofd this object
    private List<Material> materials;

    //Gets all the materials from each renderer
    private void Awake()
    {
        materials = new List<Material>();
        foreach (var renderer in renderers)
        {
            //A single child-object might have mutliple materials on it
            //that is why we need to all materials with "s"
            materials.AddRange(new List<Material>(renderer.materials));
        }
    }

    public void ToggleHighlight(bool val)
    {
        foreach (var material in materials)
        {
            if (val)
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color);
            }
            else
            {
                material.SetColor("_EmissionColor", Color.black);
                material.DisableKeyword("_EMISSION");
            }
        }
    }
}
