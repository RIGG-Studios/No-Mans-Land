using UnityEngine;


[CreateAssetMenu]
public class LobbyTypes : ScriptableObject
{
    public string name;

    [TextArea]
    public string description;
}
