using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[VolumeComponentMenuForRenderPipeline("Custom/Pixelize", typeof(UniversalRenderPipeline))]
public class PixelizeComponent : VolumeComponent, IPostProcessComponent
{
    public IntParameter screenHeight = new(256);
    
    public bool IsActive()
    {
        return true;
    }

    public bool IsTileCompatible()
    {
        return false;
    }
}
