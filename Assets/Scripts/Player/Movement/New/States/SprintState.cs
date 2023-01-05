
public class SprintState : MoveState
{
    private float _sprintFOV;
    
    public override void Init(PlayerMovementHandler movement, StateTypes type)
    {
        base.Init(movement, type);

        MovementSpeed = movement.sprintSpeed;
        _sprintFOV = movement.sprintFOV;

        CameraController = movement.cameraLook;
    }


    public override void OnUpdate()
    {
        CameraController.SetFOV(_sprintFOV);
    }
}
