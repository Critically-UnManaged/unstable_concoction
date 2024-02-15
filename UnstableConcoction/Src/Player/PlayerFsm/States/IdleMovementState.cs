using Godot;
using Serilog;
using UnstableConcoction.Extensions;

namespace UnstableConcoction.Player.PlayerFsm.States;

public class IdleMovementState : PlayerMovementMovementState
{
    public IdleMovementState(PlayerMovementFsm fsm) : base(fsm)
    {
    }

    private static bool ShouldTransitionToRun =>
        Input.IsActionPressed("run_left") || Input.IsActionPressed("run_right");
    
    private static bool ShouldTransitionToJump =>
        Input.IsActionJustPressed("jump");


    public override void Enter()
    {
        Animation.SetCondition(AnimConditions.IS_IDLE, true);
        CurrentVelocity = Vector2.Zero;
        Fsm.UpdateVelocity(CurrentVelocity);
    }

    public override void Exit()
    {
        Animation.SetCondition(AnimConditions.IS_IDLE, false);
    }

    public override void PhysicsProcess(float delta)
    {
        if (ShouldTransitionToJump)
        {
            Fsm.ChangeState(new JumpMovementState(Fsm));
        }
        
        if (ShouldTransitionToRun)
        {
            Fsm.ChangeState(new RunningMovementState(Fsm));
        }
    }

    public override string ToString() => nameof(IdleMovementState);
}