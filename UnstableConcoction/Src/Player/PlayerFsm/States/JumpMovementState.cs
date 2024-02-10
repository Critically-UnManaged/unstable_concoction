using Godot;
using Serilog;
using UnstableConcoction.Extensions;

namespace UnstableConcoction.Player.PlayerFsm.States;

public class JumpMovementState : PlayerMovementMovementState
{
    public JumpMovementState(PlayerMovementFsm fsm) : base(fsm)
    {
    }

    private bool ShouldTransitionToIdle =>
        Player.IsOnFloor();

    private bool ShouldTransitionToWallJump => 
        Player.IsOnWall() && Input.IsActionJustPressed("jump");
   

    public override void Enter()
    {
        Log.Debug("Entering JumpMovementState...");
        Animation.SetCondition(AnimConditions.IS_IDLE, false);
        Animation.SetCondition(AnimConditions.IS_RUNNING, false);
        Animation.SetCondition(AnimConditions.IS_JUMPING, true);
        
        CurrentVelocity = Fsm.CurrentVelocity.WithY(Fsm.JumpVelocity);
        Fsm.UpdateVelocity(CurrentVelocity);
    }

    public override void Exit()
    {
        Log.Debug("Exiting JumpMovementState...");
        Animation.SetCondition(AnimConditions.IS_JUMPING, false);
    }

    public override void PhysicsProcess(float delta)
    {
        if (ShouldTransitionToIdle)
        {
            Fsm.ChangeState(new IdleMovementState(Fsm));
            return;
        }

        if (ShouldTransitionToWallJump)
        {
            Fsm.ChangeState(new WallJumpMovementState(Fsm));
            return;
        }
    }
    
    public override string ToString() => nameof(JumpMovementState);
}