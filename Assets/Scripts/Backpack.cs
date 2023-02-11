using Fusion;

public class Backpack : ChestInventory
{
    [Networked]
    private TickTimer lifeTimer { get; set; }

    [Networked(OnChanged = nameof(OnOwnerChanged), OnChangedTargets = OnChangedTargets.All)] 
    private NetworkString<_16> ownerName { get; set; }

    public void LoadItems(NetworkString<_16> ownerName, ItemListData[] items)
    {
        for (int i = 0; i < items.Length; i++)
        {
            AddItem(items[i].ItemID, items[i].Stack);
        }

        this.ownerName = ownerName;
    }

    private static void OnOwnerChanged(Changed<Backpack> changed)
    {
        changed.Behaviour.SetHeaderText(changed.Behaviour.ownerName.ToString());
    }

    public override void Spawned()
    {
        base.Spawned();
        
        lifeTimer = TickTimer.CreateFromSeconds(Runner, Context.Config.backpackLife);
    }

    public override void FixedUpdateNetwork()
    {
        if (lifeTimer.ExpiredOrNotRunning(Runner))
        {
            Runner.Despawn(Object);
        }
    }
}
