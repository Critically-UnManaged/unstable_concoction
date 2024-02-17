using Godot;
using Serilog;
using UnstableConcoction.Extensions;

namespace UnstableConcoction.Player.Movement.States;

public class WallJumpState: MovementState
{
    private bool ShouldTransitionToIdle =>
        PlayerMovement.IsOnFloor;
    
    private bool ShouldTransitionToAnotherWallJump =>
        PlayerMovement.IsOnWall && Input.IsActionJustPressed("jump");

    public WallJumpDirection LastWallSlide { get; private set; }
    public WallJumpState(PlayerMovement playerMovement, WallJumpDirection lastWallSlide = WallJumpDirection.None) : base(playerMovement)
    {
        LastWallSlide = lastWallSlide;
    }

    public override void Enter()
    {
        Animation.SetCondition(AnimConditions.IS_LANDING, false);
        Animation.SetCondition(AnimConditions.IS_JUMPING, true);
        
        WallJumpDirection currentWallSlide = IsOnWallLeft() ? WallJumpDirection.Left : WallJumpDirection.Right;
        
        if (LastWallSlide == currentWallSlide)
        {
            Log.Debug("We are jumping on the same wall, so we are sliding down instead");
            TransitionRequested?.Invoke(new FallState(PlayerMovement));
            return;
        }
        
        LastWallSlide = currentWallSlide;
        float horizontalForceDirection = IsOnWallLeft() ? 1 : -1;
        CurrentVelocity = CurrentVelocity.WithY(PlayerMovement.JumpVelocity);
        CurrentVelocity = CurrentVelocity.WithX(PlayerMovement.WalljumpHorizontalForce * horizontalForceDirection);
    }

    public override void Exit()
    {
        Animation.SetCondition(AnimConditions.IS_JUMPING, false);
        Animation.SetCondition(AnimConditions.IS_LANDING, true);
    }

    public override void Update(float delta)
    {
        if (ShouldTransitionToIdle)
        {
            TransitionRequested?.Invoke(new IdleState(PlayerMovement));
        }
        
        if (ShouldTransitionToAnotherWallJump)
        {
            TransitionRequested?.Invoke(new WallJumpState(PlayerMovement, LastWallSlide));
        }
    }
    
    private bool IsOnWallLeft()
    {
        for (int i = 0; i < PlayerMovement.Player.GetSlideCollisionCount(); i++)
        {
            KinematicCollision2D? collision = PlayerMovement.Player.GetSlideCollision(i);
            // Check if the collision normal is pointing right, indicating a collision on the left
            if (collision.GetNormal().X > 0)
            {
                return true;
            }
        }
        return false;
    }
    
    public override string ToString() => "Walljumping";
}

public enum WallJumpDirection
{
    None,
    Left,
    Right
}