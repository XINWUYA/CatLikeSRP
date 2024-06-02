using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomRP
{
    public class CustomRenderPipeline : RenderPipeline
    {
        CameraRenderer m_cameraRenderer = new CameraRenderer();

        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            for (int i = 0; i < cameras.Length; ++i)
            {
                m_cameraRenderer.Render(context, cameras[i]);
            }
        }

        protected override void Render(ScriptableRenderContext context, List<Camera> cameras)
        {
            for (int i = 0; i < cameras.Count; ++i)
            {
                m_cameraRenderer.Render(context, cameras[i]);
            }
        }
    }
}