using Godot;
using Serilog;
using UnstableConcoction.Extensions;

namespace UnstableConcoction.Player.PlayerFsm.States;

public class RunningMovementState : PlayerMovementMovementState
{
    public float CoyoteTimeCounter { get; private set; }
    
    public RunningMovementState(PlayerMovementFsm fsm) : base(fsm)
    {
    }

    public override void Enter()
    {
        Log.Debug("Entering RunningMovementState...");
        Animation.SetCondition(AnimConditions.IS_IDLE, false);
        Animation.SetCondition(AnimConditions.IS_JUMPING, false);
        Animation.SetCondition(AnimConditions.IS_RUNNING, true);
        CoyoteTimeCounter = Fsm.CoyoteTimeDuration;
    }

    public override void Exit()
    {
        Log.Debug("Exiting RunningMovementState...");
        Animation.SetCondition(AnimConditions.IS_RUNNING, false);
    }

    public override void PhysicsProcess(float delta)
    {
        if (CoyoteTimeCounter > 0.0f && !Player.IsOnFloor())
        {
            CoyoteTimeCounter = Mathf.Clamp(CoyoteTimeCounter - delta, 0.0f, Fsm.CoyoteTimeDuration);
        }
        
        float horizontalInput = Input.GetActionStrength("run_right") - Input.GetActionStrength("run_left");
        CurrentVelocity = Player.Velocity.WithX(horizontalInput * Fsm.Speed);
        Fsm.UpdateVelocity(CurrentVelocity);
        
        if (!Player.IsOnFloor() && CoyoteTimeCounter <= 0.0f)
        {
            Fsm.ChangeState(new FallingMovementState(Fsm));
            return;
        }
        
        if (Input.IsActionJustPressed("jump"))
        {
            Fsm.ChangeState(new JumpMovementState(Fsm));
            return;
        }

        if (Player.IsOnFloor())
        {
            CoyoteTimeCounter = Fsm.CoyoteTimeDuration;
        }
        
        if (Player.IsOnFloor() && horizontalInput.IsAbout(0.0f))
        {
            Fsm.ChangeState(new IdleMovementState(Fsm));
        }
    }
    
    public override string ToString() => nameof(RunningMovementState);
}