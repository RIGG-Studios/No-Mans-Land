
public class PlayerInteractionInputHandler : InputBase
{
    private PlayerInteractionHandler _playerInteraction;
    
    public override void Awake()
    {
        base.Awake();

        _playerInteraction = GetComponent<PlayerInteractionHandler>();
    }

    private void Start()
    {
        InputActions.Player.Interact.performed += ctx => _playerInteraction.TryButtonInteract();
        InputActions.Player.Escape.performed += ctx => _playerInteraction.TryExitInteract();
    }
}
