using UnityEngine;

public class CharacterEquippableItem : MonoBehaviour
{
    [SerializeField] private Transform leftHandIKTarget;
    [SerializeField] private Transform rightHandIKTarget;

    public Transform LeftHandIKTarget => leftHandIKTarget;
    public Transform RightHandIKTarget => rightHandIKTarget;

    public int ItemID { get; private set; }
    public NetworkPlayer Player { get; private set; }

    public void Init(int itemID, NetworkPlayer player)
    {
        ItemID = itemID;
        Player = player;
    }

    public virtual void OnAttack() { }
    public virtual void OnReload() { }
}
