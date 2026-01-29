using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RenderMetaballs : ScriptableRendererFeature
{
    class RenderMetaballsPass : ScriptableRenderPass
    {
        const string SmallRTName = "_MetaballRTSmall";
        const string LargeRTName = "_MetaballRTLarge";
        const string LargeRT2Name = "_MetaballRTLarge2";

        public Material BlitMaterial;
        public int Downsample = 4;

        RTHandle _smallRT;
        RTHandle _largeRT;
        RTHandle _largeRT2;
        RTHandle _cameraColor;

        Material _blurMaterial;
        Material _copyDepthMaterial;

        FilteringSettings _filteringSettings;
        ProfilingSampler _profilingSampler;
        List<ShaderTagId> _shaderTags = new();
        RenderStateBlock _renderStateBlock;

        public RenderMetaballsPass(
            string tag,
            RenderPassEvent passEvent,
            string[] shaderTags,
            RenderQueueType queueType,
            int layerMask,
            int downsample)
        {
            renderPassEvent = passEvent;
            _profilingSampler = new ProfilingSampler(tag);

            _filteringSettings = new FilteringSettings(
                queueType == RenderQueueType.Transparent ? RenderQueueRange.transparent : RenderQueueRange.opaque,
                layerMask);

            if (shaderTags != null && shaderTags.Length > 0)
            {
                foreach (var s in shaderTags)
                    _shaderTags.Add(new ShaderTagId(s));
            }
            else
            {
                _shaderTags.Add(new ShaderTagId("UniversalForward"));
            }

            _renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);

            _blurMaterial = new Material(Shader.Find("Hidden/KawaseBlur"));
            _copyDepthMaterial = new Material(Shader.Find("Hidden/BlitToDepth"));
            Downsample = Mathf.Max(1, downsample);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            _cameraColor = renderingData.cameraData.renderer.cameraColorTargetHandle;

            var descSmall = renderingData.cameraData.cameraTargetDescriptor;
            descSmall.depthBufferBits = 0;
            descSmall.width /= Downsample;
            descSmall.height /= Downsample;

            var descLarge = renderingData.cameraData.cameraTargetDescriptor;
            descLarge.depthBufferBits = 0;

            RenderingUtils.ReAllocateIfNeeded(ref _smallRT, descSmall, name: SmallRTName);
            RenderingUtils.ReAllocateIfNeeded(ref _largeRT, descLarge, name: LargeRTName);
            RenderingUtils.ReAllocateIfNeeded(ref _largeRT2, descLarge, name: LargeRT2Name);

            ConfigureTarget(_smallRT);
            ConfigureClear(ClearFlag.All, Color.clear);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var sorting = renderingData.cameraData.defaultOpaqueSortFlags;
            var drawSettings = CreateDrawingSettings(_shaderTags, ref renderingData, sorting);

            var cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, _profilingSampler))
            {
                // Draw metaballs into small RT
                context.DrawRenderers(
                    renderingData.cullResults,
                    ref drawSettings,
                    ref _filteringSettings,
                    ref _renderStateBlock
                );

                // Upscale to large RT
                Blitter.BlitCameraTexture(cmd, _smallRT, _largeRT, BlitMaterial, 0);

                // Blur
                cmd.SetGlobalVector("_Offsets", new Vector4(1.5f, 2f, 2.5f, 3f));
                Blitter.BlitCameraTexture(cmd, _largeRT, _largeRT2, _blurMaterial, 0);

                // Composite back to camera
                Blitter.BlitCameraTexture(cmd, _largeRT2, _cameraColor, BlitMaterial, 0);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            _smallRT?.Release();
            _largeRT?.Release();
            _largeRT2?.Release();
        }
    }

    public Material blitMaterial;
    public RenderObjects.RenderObjectsSettings renderObjectsSettings =
        new RenderObjects.RenderObjectsSettings();

    [Range(1, 16)]
    public int downsamplingAmount = 4;

    RenderMetaballsPass _pass;

    public override void Create()
    {
        var filter = renderObjectsSettings.filterSettings;

        _pass = new RenderMetaballsPass(
            renderObjectsSettings.passTag,
            renderObjectsSettings.Event,
            filter.PassNames,
            filter.RenderQueueType,
            filter.LayerMask,
            downsamplingAmount)
        {
            BlitMaterial = blitMaterial
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_pass);
    }
}
