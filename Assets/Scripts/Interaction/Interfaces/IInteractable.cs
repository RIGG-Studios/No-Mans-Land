using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{ 
    string LookAtID { get; }
    string ID { get; }
    
    
    void LookAtInteract();

    void StopLookAtInteract();

    bool ButtonInteract(NetworkPlayer networkPlayer);
    void StopButtonInteract();
}
