using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostEffectCamera : MonoBehaviour
{
    //material that's applied when doing postprocessing
    [SerializeField]
    private Material m_postprocessMaterial = null;

    private Camera m_camera;

    private void Start()
    {
        //get the camera and tell it to render a depthnormals texture
        m_camera = GetComponent<Camera>();
        m_camera.depthTextureMode = m_camera.depthTextureMode | DepthTextureMode.DepthNormals;
    }

    //method which is automatically called by unity after the camera is done rendering
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //draws the pixels from the source texture to the destination texture
        Graphics.Blit(source, destination, m_postprocessMaterial);
    }
}
