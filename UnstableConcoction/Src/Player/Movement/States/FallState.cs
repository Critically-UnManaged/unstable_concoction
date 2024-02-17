using Godot;
using UnstableConcoction.Extensions;

namespace UnstableConcoction.Player.Movement.States;

public class FallState: MovementState
{
    private bool ShouldTransitionToRunning => 
        PlayerMovement.Player.IsOnFloor() && (Input.IsActionPressed("run_left") || Input.IsActionPressed("run_right"));
    
    private bool ShouldTransitionToIdle =>
        PlayerMovement.Player.IsOnFloor();
    
    
    public FallState(PlayerMovement playerMovement) : base(playerMovement)
    {
    }

    public override void Enter()
    {
        Animation.SetCondition(AnimConditions.IS_LANDING, false);
        Animation.SetCondition(AnimConditions.IS_JUMPING, true);
    }

    public override void Exit()
    {
        Animation.SetCondition(AnimConditions.IS_JUMPING, false);
        Animation.SetCondition(AnimConditions.IS_LANDING, true);
    }

    public override void Update(float delta)
    {
        float inducedHorizontal = (GetHorizontalInput() * PlayerMovement.RunMaxSpeed) * PlayerMovement.AirControl;
        CurrentVelocity = CurrentVelocity.WithX(inducedHorizontal);
        
        if (ShouldTransitionToRunning)
        {
            TransitionRequested?.Invoke(new RunState(PlayerMovement));
            return;
        }
        
        if (ShouldTransitionToIdle)
        {
            TransitionRequested?.Invoke(new IdleState(PlayerMovement));
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

    public override string ToString() => "Falling";
}