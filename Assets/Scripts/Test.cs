using Fusion;

public class Test : ContextBehaviour
{
    [Networked]
    public NetworkBool UnderWater { get; private set; }
    
    public override void FixedUpdateNetwork()
    {
        float y = Ocean.Instance.GetWaterHeightAtPosition(transform.position);
        
        if (transform.position.y - y < 0)
        {
            if (Object.HasInputAuthority)
            {
                Context.PostProcessing.EnablePostProcessing(ScenePostProcessing.PostProcessingTypes.UnderWater);
            }

            UnderWater = true;
        }
        else
        {
            if (Object.HasInputAuthority)
            {
                Context.PostProcessing.DisablePostProcessing(ScenePostProcessing.PostProcessingTypes.UnderWater);
            }

            UnderWater = false;
        }
    }
}
