
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// ReSharper disable InconsistentNaming

public class PixelizePass : ScriptableRenderPass
{
    private Material pixelizeMaterial;
    private PixelizeComponent pixelizeEffect;

    private RTHandle cameraColorTargetHandle;
    private RTHandle pixelTargetHandle;
    
    private static readonly int BlockCount = Shader.PropertyToID("_BlockCount");
    private static readonly int BlockSize = Shader.PropertyToID("_BlockSize");
    private static readonly int HalfBlockSize = Shader.PropertyToID("_HalfBlockSize");

    public PixelizePass(Material pixelizeMaterial)
    {
        this.pixelizeMaterial = pixelizeMaterial;
        
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public void SetTarget(RTHandle cameraColorTargetHandle)
    {
        this.cameraColorTargetHandle = cameraColorTargetHandle;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        VolumeStack stack = VolumeManager.instance.stack;
        pixelizeEffect = stack.GetComponent<PixelizeComponent>();
        
        var descriptor = renderingData.cameraData.cameraTargetDescriptor;

        CommandBuffer cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, new ProfilingSampler("Pixelize Pass")))
        {
            SetupPixelize(cmd, renderingData.cameraData.camera.aspect, descriptor);
        }

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        
        CommandBufferPool.Release(cmd);
    }

    private void SetupPixelize(CommandBuffer cmd, float cameraAspect, RenderTextureDescriptor descriptor)
    {
        var pixelScreenHeight = pixelizeEffect.screenHeight.value;
        var pixelScreenWidth = (int)(pixelScreenHeight * cameraAspect + 0.5f);

        pixelizeMaterial.SetVector(BlockCount, new Vector2(pixelScreenWidth, pixelScreenHeight));
        pixelizeMaterial.SetVector(BlockSize, new Vector2(1.0f / pixelScreenWidth, 1.0f / pixelScreenHeight));
        pixelizeMaterial.SetVector(HalfBlockSize, new Vector2(0.5f / pixelScreenWidth, 0.5f / pixelScreenHeight));

        //descriptor.height = pixelScreenHeight;
        //descriptor.width = pixelScreenWidth;

        //pixelTargetHandle = RTHandles.Alloc(descriptor: descriptor, filterMode: FilterMode.Point);
        Blitter.BlitCameraTexture(
            cmd, 
            cameraColorTargetHandle, 
            cameraColorTargetHandle,
            pixelizeMaterial,
            0);
        /*Blitter.BlitCameraTexture(
            cmd, 
            pixelTargetHandle, 
            cameraColorTargetHandle);*/
    }
}