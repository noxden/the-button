using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorHandler : MonoBehaviour
{
    public static ColorHandler instance { get; private set; }

    [SerializeField]
    private List<Material> palette;
    private List<Color> originalColors;
    private void Awake()
    {
        //> Singleton Setup
        if (instance == null) { instance = this; }
        else { DestroyImmediate(this); }
    }

    private void Start()
    {
        originalColors = new List<Color>();
        foreach (Material mat in palette)
            originalColors.Add(mat.color);
    }

    private void OnApplicationQuit()
    {
        ResetMaterialChanges();
    }

    public void InvertPalette()
    {
        foreach (Material mat in palette)
        {
            mat.color = Invert(mat.color);
        }
    }

    private Color Invert(Color input)
    {
        return new Color(1f - input.r, 1f - input.g, 1f - input.b, input.a);
    }

    private void ResetMaterialChanges()     // TODO: Should be called whenever the current scene is closed as well. Maybe in OnDisable()?
    {
        //> Reset materials / cleanup
        // for (int i = 0; i < palette.Count; i++)  //< Technically, this would be better code, but as only the entire palette gets inverted anyways, ...
        // {
        //     if (palette[i].color != originalColors[i])
        //     {
        //         palette[i].color = Invert(palette[i].color);
        //     }
        // }

        if (palette[0].color != originalColors[0])  //< ...This code is less resource-heavy.
            InvertPalette();
    }
}
