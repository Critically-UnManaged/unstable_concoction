using Godot;
using Serilog;
using UnstableConcoction.Extensions;

namespace UnstableConcoction.Player.PlayerFsm.States;

public class FallingMovementState: PlayerMovementMovementState
{
    public FallingMovementState(PlayerMovementFsm fsm) : base(fsm)
    {
    }

    public override void Enter()
    {
        Log.Debug("Entering FallingMovementState...");
        Animation.SetCondition(AnimConditions.IS_RUNNING, false);
        Animation.SetCondition(AnimConditions.IS_IDLE, false);
        Animation.SetCondition(AnimConditions.IS_JUMPING, true);
    }

    public override void Exit()
    {
        Log.Debug("Exiting FallingMovementState...");
        Animation.SetCondition(AnimConditions.IS_JUMPING, false);
    }

    public override void PhysicsProcess(float delta)
    {
        float horizontalInput = (Input.GetActionStrength("run_right") - Input.GetActionStrength("run_left")) * 0.5f;
        CurrentVelocity = Fsm.CurrentVelocity.WithX(horizontalInput * Fsm.Speed);
        Fsm.UpdateVelocity(CurrentVelocity);

        if (Player.IsOnFloor())
        {
            Fsm.ChangeState(new IdleMovementState(Fsm));
        }
    }
    
    public override string ToString() => nameof(FallingMovementState);
}