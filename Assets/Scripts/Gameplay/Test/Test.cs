using Fusion;
using UnityEngine;

public class Test : ContextBehaviour
{
    [SerializeField] private float offset;
    
    [Networked]
    public NetworkBool UnderWater { get; private set; }
    
    public override void FixedUpdateNetwork()
    {
        float y = Ocean.Instance.GetWaterHeightAtPosition(transform.position);

        float k = (transform.position.y - y);
        
        if (k < offset)
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
