using Godot;
using UnstableConcoction.Extensions;

namespace UnstableConcoction.Player.Movement.States;

public class RunState: MovementState
{
    private bool ShouldTransitionToIdle =>
        PlayerMovement.Player.IsOnFloor() && !Input.IsActionPressed("run_left") && !Input.IsActionPressed("run_right");
    
    private bool ShouldTransitionToJump =>
        Input.IsActionJustPressed("jump");

    private bool ShouldTransitionToFall =>
        !PlayerMovement.Player.IsOnFloor() && AirRunningTimer > PlayerMovement.CoyoteTimeDuration;
    
    public float AirRunningTimer { get; private set; }
    
    public RunState(PlayerMovement playerMovement) : base(playerMovement)
    {
    }

    public override void Enter()
    {
        Animation.SetCondition(AnimConditions.IS_RUNNING, true);
    }

    public override void Exit()
    {
        Animation.SetCondition(AnimConditions.IS_RUNNING, false);
    }

    public override void Update(float delta)
    {
        AirRunningTimer = PlayerMovement.IsOnFloor ? 0f : AirRunningTimer + delta;
        
        CurrentVelocity = CurrentVelocity.WithX(
            Mathf.Lerp(CurrentVelocity.X, GetHorizontalInput() * PlayerMovement.RunMaxSpeed,
                PlayerMovement.RunAcceleration)
        );
        
        if (ShouldTransitionToJump)
        {
            TransitionRequested?.Invoke(new JumpState(PlayerMovement));
            return;
        }
        
        if (ShouldTransitionToIdle)
        {
            TransitionRequested?.Invoke(new IdleState(PlayerMovement));
        }

        if (ShouldTransitionToFall)
        {
            TransitionRequested?.Invoke(new FallState(PlayerMovement));
        }
    }

    private static float GetHorizontalInput()
    {
        float horizontalInput = 0f;
        
        if (Input.IsActionPressed("run_right"))
        {
            horizontalInput += 1f;
        }

        if (Input.IsActionPressed("run_left"))
        {
            horizontalInput -= 1f;
        }

        return horizontalInput;
    }

    public override string ToString() => "Running";
}