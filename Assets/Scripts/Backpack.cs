using Fusion;

public class Backpack : ChestInventory
{
    [Networked]
    private TickTimer lifeTimer { get; set; }
    
    public void LoadItems(ItemListData[] items)
    {
        for (int i = 0; i < items.Length; i++)
        {
            AddItem(items[i].ItemID, items[i].Stack);
        }
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
