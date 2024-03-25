using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraRenderer
{
    private ScriptableRenderContext m_context;
    private Camera m_camera;

    private const String m_commandBufferName = "Camera Renderer";
    private CommandBuffer m_commandBuffer = new() {name = m_commandBufferName };
    private CullingResults m_cullingResults;

    //private static ShaderTagId m_unlitShaderTagId = new ShaderTagId("Always");
    private static ShaderTagId m_unlitShaderTagId = new ShaderTagId("ForwardBase");

    public void Render(ScriptableRenderContext context, Camera camera)
    {
        m_context = context;
        m_camera = camera;

        if (!Cull())
            return;
        
        Setup();
        DrawVisibleObjects();
        Submit();
    }

    void Setup()
    {
        m_context.SetupCameraProperties(m_camera);
        m_commandBuffer.ClearRenderTarget(true, true, Color.clear);
        m_commandBuffer.BeginSample(m_commandBufferName);
        ExecuteCommandBuffer();
    }

    void DrawVisibleObjects()
    {
        var sortingSettings = new SortingSettings(m_camera);
        var drawingSettings = new DrawingSettings(m_unlitShaderTagId, sortingSettings);
        var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
        m_context.DrawRenderers(m_cullingResults, ref drawingSettings, ref filteringSettings);
        
        
        m_context.DrawSkybox(m_camera);
        
        //m_commandBuffer.BeginSample("Transparent");
        {
            sortingSettings.criteria = SortingCriteria.CommonTransparent;
            drawingSettings.sortingSettings = sortingSettings;
            filteringSettings.renderQueueRange = RenderQueueRange.transparent;
            m_context.DrawRenderers(m_cullingResults, ref drawingSettings, ref filteringSettings);
        }
        //m_commandBuffer.EndSample("Transparent");
    }

    void Submit()
    {
        m_commandBuffer.EndSample(m_commandBufferName);
        ExecuteCommandBuffer();
        
        m_context.Submit();
    }

    void ExecuteCommandBuffer()
    {
        m_context.ExecuteCommandBuffer(m_commandBuffer);
        m_commandBuffer.Clear();
    }

    bool Cull()
    {
        if (m_camera.TryGetCullingParameters(out ScriptableCullingParameters parameters))
        {
            m_cullingResults = m_context.Cull(ref parameters);
            return true;
        }
        return false;
    }
}
