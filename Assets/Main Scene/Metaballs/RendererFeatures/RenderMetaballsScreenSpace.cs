using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RenderMetaballsScreenSpace : ScriptableRendererFeature
{
    class RenderMetaballsDepthPass : ScriptableRenderPass
    {
        const string MetaballDepthRTName = "_MetaballDepthRT";
        public Material WriteDepthMaterial;

        RTHandle _metaballDepthRT;
        FilteringSettings _filteringSettings;
        RenderStateBlock _renderStateBlock;
        ProfilingSampler _profilingSampler;
        List<ShaderTagId> _shaderTagIdList = new();

        public RenderMetaballsDepthPass(string tag, RenderPassEvent passEvent, string[] shaderTags,
            RenderQueueType queueType, int layerMask)
        {
            renderPassEvent = passEvent;
            _profilingSampler = new ProfilingSampler(tag);

            _filteringSettings = new FilteringSettings(
                queueType == RenderQueueType.Transparent ? RenderQueueRange.transparent : RenderQueueRange.opaque,
                layerMask);

            if (shaderTags != null && shaderTags.Length > 0)
            {
                foreach (var s in shaderTags)
                    _shaderTagIdList.Add(new ShaderTagId(s));
            }
            else
            {
                _shaderTagIdList.Add(new ShaderTagId("UniversalForward"));
            }

            _renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;

            RenderingUtils.ReAllocateIfNeeded(
                ref _metaballDepthRT,
                desc,
                name: MetaballDepthRTName
            );

            ConfigureTarget(_metaballDepthRT);
            ConfigureClear(ClearFlag.All, Color.clear);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var sorting = renderingData.cameraData.defaultOpaqueSortFlags;
            var drawSettings = CreateDrawingSettings(_shaderTagIdList, ref renderingData, sorting);
            drawSettings.overrideMaterial = WriteDepthMaterial;

            var cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, _profilingSampler))
            {
                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref _filteringSettings,
                    ref _renderStateBlock);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    class RenderMetaballsScreenSpacePass : ScriptableRenderPass
    {
        const string RT1Name = "_MetaballRT";
        const string RT2Name = "_MetaballRT2";

        public Material BlitMaterial;
        public int BlurPasses;
        public float BlurDistance;

        RTHandle _rt1;
        RTHandle _rt2;
        RTHandle _cameraColor;

        Material _blurMaterial;
        ProfilingSampler _profilingSampler;

        public RenderMetaballsScreenSpacePass(string tag, RenderPassEvent passEvent)
        {
            renderPassEvent = passEvent;
            _profilingSampler = new ProfilingSampler(tag);
            _blurMaterial = new Material(Shader.Find("Hidden/KawaseBlur"));
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            _cameraColor = renderingData.cameraData.renderer.cameraColorTargetHandle;

            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;

            RenderingUtils.ReAllocateIfNeeded(ref _rt1, desc, name: RT1Name);
            RenderingUtils.ReAllocateIfNeeded(ref _rt2, desc, name: RT2Name);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, _profilingSampler))
            {
                Blitter.BlitCameraTexture(cmd, _cameraColor, _rt1);

                cmd.SetGlobalFloat("_BlurDistance", BlurDistance);

                for (int i = 0; i < BlurPasses; i++)
                {
                    cmd.SetGlobalFloat("_Offset", 1.5f + i);
                    Blitter.BlitCameraTexture(cmd, _rt1, _rt2, _blurMaterial, 0);
                    (_rt1, _rt2) = (_rt2, _rt1);
                }

                Blitter.BlitCameraTexture(cmd, _rt1, _cameraColor, BlitMaterial, 0);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    public RenderPassEvent Event = RenderPassEvent.AfterRenderingOpaques;
    public Material BlitMaterial;
    public Material WriteDepthMaterial;
    public int BlurPasses = 1;
    public float BlurDistance = 0.5f;

    RenderMetaballsDepthPass _depthPass;
    RenderMetaballsScreenSpacePass _screenPass;

    public override void Create()
    {
        _depthPass = new RenderMetaballsDepthPass("MetaballsDepth", Event, null,
            RenderQueueType.Transparent, ~0)
        {
            WriteDepthMaterial = WriteDepthMaterial
        };

        _screenPass = new RenderMetaballsScreenSpacePass("MetaballsScreen", Event)
        {
            BlitMaterial = BlitMaterial,
            BlurPasses = BlurPasses,
            BlurDistance = BlurDistance
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_depthPass);
        renderer.EnqueuePass(_screenPass);
    }
}
