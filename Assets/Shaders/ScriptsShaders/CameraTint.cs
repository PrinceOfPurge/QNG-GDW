using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Scripts : MonoBehaviour
{
    public Material NormalLUT;
    //shader script goes here
    public Shader awesomeShader = null;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, NormalLUT);
        //Copies source texture into destination render texture with a shader,
        //this is mostly used for implementing post-processing effects
        //the currentLUT changes which makes the colour gradient change
    }
}
