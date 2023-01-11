using UnityEngine;

public class CharacterEquippableItem : MonoBehaviour
{
    [SerializeField] private Transform leftHandIKTarget;
    [SerializeField] private Transform rightHandIKTarget;

    public Transform LeftHandIKTarget => leftHandIKTarget;
    public Transform RightHandIKTarget => rightHandIKTarget;

    public int ItemID { get; private set; }
    

    public void Init(int itemID)
    {
        ItemID = itemID;
    }

    public virtual void OnAttack()
    {
        
    }
}
