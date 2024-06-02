using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CustomRP
{
    public partial class CameraRenderer
    {
#if UNITY_EDITOR
        private static readonly ShaderTagId[] s_legacyShaderTagIds =
        {
            new("Always"),
            new("ForwardBase"),
            new("PrepassBase"),
            new("Vertex"),
            new("VertexLMRGBM"),
            new("VertexLM"),
        };

        private static Material s_errorMaterial;
        private const String c_gizmoSampleName = "Draw Gizmos";

        /// <summary>
        /// 编辑器模式下，使用ErrorShader绘制那些暂未支持材质的物体
        /// </summary>
        private void DrawUnsupportedObjects()
        {
            if (s_errorMaterial == null)
            {
                s_errorMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));
            }

            var sortingSettings = new SortingSettings(m_camera);
            var drawingSettings = new DrawingSettings(s_legacyShaderTagIds[0], sortingSettings);
            for (var i = 1; i < s_legacyShaderTagIds.Length; ++i)
            {
                drawingSettings.SetShaderPassName(i, s_legacyShaderTagIds[i]);
            }

            drawingSettings.overrideMaterial = s_errorMaterial;

            var filteringSettings = FilteringSettings.defaultValue;
            m_context.DrawRenderers(m_cullingResults, ref drawingSettings, ref filteringSettings);
        }

        /// <summary>
        /// 编辑器模式下，绘制Gizmos
        /// </summary>
        private void DrawGizmos()
        {
            if (Handles.ShouldRenderGizmos())
            {
                // m_commandBuffer.BeginSample(c_gizmoSampleName);
                m_context.DrawGizmos(m_camera, GizmoSubset.PreImageEffects);
                m_context.DrawGizmos(m_camera, GizmoSubset.PostImageEffects);
                // m_commandBuffer.EndSample(c_gizmoSampleName);
            }
        }

        private void PrepareForSceneView()
        {
            if (m_camera.cameraType == CameraType.SceneView)
            {
                ScriptableRenderContext.EmitWorldGeometryForSceneView(m_camera);
            }
        }

#endif
    }
}