public class Test : ContextBehaviour
{
    public override void Render()
    {
        if (!Object.HasInputAuthority)
        {
            return;
        }
        
        float y = Ocean.Instance.GetWaterHeightAtPosition(transform.position);
        
        if (transform.position.y - y < 0)
        {
            Context.PostProcessing.EnablePostProcessing(ScenePostProcessing.PostProcessingTypes.UnderWater);
        }
        else
        {    
            Context.PostProcessing.DisablePostProcessing(ScenePostProcessing.PostProcessingTypes.UnderWater);
        }
    }
}
