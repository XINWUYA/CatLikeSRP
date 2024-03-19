using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraRenderer
{
    ScriptableRenderContext m_context;
    Camera m_camera;

    public void Render(ScriptableRenderContext context, Camera camera)
    {
        m_context = context;
        m_camera = camera;

        Setup();
        DrawVisibleObjects();
        Submit();
    }

    void Setup()
    {
        m_context.SetupCameraProperties(m_camera);
    }

    void DrawVisibleObjects()
    {
        m_context.DrawSkybox(m_camera);
    }

    void Submit()
    {
        m_context.Submit();
    }
}
