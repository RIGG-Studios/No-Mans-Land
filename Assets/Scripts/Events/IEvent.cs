using UnityEngine.Events;

public interface IEvent
{
    UnityEvent onEventInvoked { get; }


    void InvokeEvent();
}
