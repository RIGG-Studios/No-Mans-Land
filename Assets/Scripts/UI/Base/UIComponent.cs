using NoMansLand.Scene;
using UnityEngine;

public class UIComponent : MonoBehaviour
{
    [SerializeField] private string id;

    public string ID => id;
    public bool IsEnabled { get; private set; }
    public SceneContext Context { get; private set; }


    public void Init(SceneContext context)
    {
        Context = context;
    }
    
    
    public virtual void Enable()
    {
        IsEnabled = true;
    }

    public virtual void Disable()
    {
        IsEnabled = false;
    }
    
    
}
