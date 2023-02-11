using Fusion;
using UnityEngine;
using NoMansLand.Scene;

public interface IContextBehaviour
{
    public SceneContext Context { get; set; }
}


public abstract class ContextBehaviour : NetworkBehaviour, IContextBehaviour
{
    public SceneContext Context { get; set; }

    protected virtual void Awake()
    {
        Scene scene = FindObjectOfType<Scene>();

        Context = scene.Context;
    }
}

public abstract class ContextSimulationBehaviour : SimulationBehaviour, IContextBehaviour
{
    public SceneContext Context { get; set; }
}

