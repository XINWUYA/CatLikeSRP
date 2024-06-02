using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomRP
{
    public partial class CameraRenderer
    {
        private ScriptableRenderContext m_context;
        private Camera m_camera;
        private CullingResults m_cullingResults;

        private const String c_commandBufferName = "Camera Renderer";
        private const String c_opaqueSampleName = "Draw Opaque";
        private const String c_transparentSampleName = "Draw Transparent";
        private readonly CommandBuffer m_commandBuffer = new CommandBuffer { name = c_commandBufferName };
        private static readonly ShaderTagId s_unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");

        public void Render(ScriptableRenderContext context, Camera camera)
        {
            m_context = context;
            m_camera = camera;

#if UNITY_EDITOR
            PrepareForSceneView();
#endif

            if (!Cull())
                return;

            Setup();
            DrawVisibleObjects();
#if UNITY_EDITOR
            DrawUnsupportedObjects();
            DrawGizmos();
#endif
            Submit();
        }

        private void Setup()
        {
            m_context.SetupCameraProperties(m_camera);
            m_commandBuffer.ClearRenderTarget(true, true, Color.clear);
            m_commandBuffer.BeginSample(c_commandBufferName);
            ExecuteCommandBuffer();
        }

        private void DrawVisibleObjects()
        {
            var sortingSettings = new SortingSettings(m_camera);
            var drawingSettings = new DrawingSettings(s_unlitShaderTagId, sortingSettings);
            var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

            // 绘制不透明物体
            // m_commandBuffer.BeginSample(c_opaqueSampleName);
            {
                // filteringSettings.renderQueueRange = RenderQueueRange.opaque;
                m_context.DrawRenderers(m_cullingResults, ref drawingSettings, ref filteringSettings);
            }
            // m_commandBuffer.EndSample(c_opaqueSampleName);

            // 绘制天空
            m_context.DrawSkybox(m_camera);

            // 绘制透明物体
            // m_commandBuffer.BeginSample(c_transparentSampleName);
            {
                sortingSettings.criteria = SortingCriteria.CommonTransparent;
                drawingSettings.sortingSettings = sortingSettings;
                filteringSettings.renderQueueRange = RenderQueueRange.transparent;
                m_context.DrawRenderers(m_cullingResults, ref drawingSettings, ref filteringSettings);
            }
            // m_commandBuffer.EndSample(c_transparentSampleName);
        }

        /// <summary>
        /// 提交绘制命令
        /// </summary>
        private void Submit()
        {
            m_commandBuffer.EndSample(c_commandBufferName);
            ExecuteCommandBuffer();

            m_context.Submit();
        }

        private void ExecuteCommandBuffer()
        {
            m_context.ExecuteCommandBuffer(m_commandBuffer);
            m_commandBuffer.Clear();
        }

        private bool Cull()
        {
            if (!m_camera.TryGetCullingParameters(out ScriptableCullingParameters parameters))
                return false;

            m_cullingResults = m_context.Cull(ref parameters);
            return true;
        }
    }
}