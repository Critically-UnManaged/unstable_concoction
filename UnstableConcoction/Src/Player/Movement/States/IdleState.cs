using Godot;
using UnstableConcoction.Extensions;

namespace UnstableConcoction.Player.Movement.States;

public class IdleState: MovementState
{
    private bool _isDecelerating;
    
    private bool ShouldTransitionToRun =>
        (Input.IsActionPressed("run_left") || Input.IsActionPressed("run_right")) && CurrentVelocity.X.IsEqualApprox(0f);
    
    private bool ShouldTransitionToJump =>
        Input.IsActionJustPressed("jump");

    private bool ShouldTransitionToFall =>
        !PlayerMovement.Player.IsOnFloor() && CurrentVelocity.X.IsEqualApprox(0f);
    
    public IdleState(PlayerMovement playerMovement) : base(playerMovement)
    {
    }

    public override void Enter()
    {
        Animation.SetCondition(AnimConditions.IS_IDLE, true);
        _isDecelerating = !CurrentVelocity.X.IsEqualApprox(0f);
        if (!_isDecelerating)
        {
            CurrentVelocity = Vector2.Zero;
        }
    }

    public override void Exit()
    {
        Animation.SetCondition(AnimConditions.IS_IDLE, false);
    }

    public override void Update(float delta)
    {
        if (_isDecelerating)
        {
            CurrentVelocity = CurrentVelocity.WithX(
                Mathf.Lerp(CurrentVelocity.X, 0f, PlayerMovement.RunFriction)
            );
            
            if (CurrentVelocity.X.IsEqualApprox(0f))
            {
                _isDecelerating = false;
            }
        }
        
        if (ShouldTransitionToRun)
        {
            TransitionRequested?.Invoke(new RunState(PlayerMovement));
            return;
        }
        
        if (ShouldTransitionToJump)
        {
            TransitionRequested?.Invoke(new JumpState(PlayerMovement));
        }
        
        if (ShouldTransitionToFall)
        {
            TransitionRequested?.Invoke(new FallState(PlayerMovement));
        }
    }

    public override string ToString() => "Idling";
}