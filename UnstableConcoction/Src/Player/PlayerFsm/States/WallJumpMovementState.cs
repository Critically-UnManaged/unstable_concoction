using Godot;
using Serilog;
using UnstableConcoction.Extensions;

namespace UnstableConcoction.Player.PlayerFsm.States;

public class WallJumpMovementState: PlayerMovementMovementState
{
    public WallJumpDirection LastWallSlide { get; private set; }
    
    public WallJumpMovementState(PlayerMovementFsm fsm, WallJumpDirection lastWallSlide = WallJumpDirection.None) : base(fsm)
    {
        LastWallSlide = lastWallSlide;
    }

    public override void Enter()
    {
        Log.Debug("Entering WallJumpMovementState...");
        Animation.SetCondition(AnimConditions.IS_JUMPING, false);
        Animation.SetCondition(AnimConditions.IS_RUNNING, false);
        Animation.SetCondition(AnimConditions.IS_IDLE, true);
        Animation.SetCondition(AnimConditions.IS_IDLE, false); //retrigger jump animation
        Animation.SetCondition(AnimConditions.IS_JUMPING, true);
        
        WallJumpDirection currentWallSlide = IsOnWallLeft() ? WallJumpDirection.Left : WallJumpDirection.Right;
        
        if (LastWallSlide == currentWallSlide)
        {
            Log.Debug("We are jumping on the same wall, so we are sliding down instead");
            Fsm.ChangeState(new FallingMovementState(Fsm));
            return;
        }
        
        LastWallSlide = currentWallSlide;
        float horizontalForceDirection = IsOnWallLeft() ? 1 : -1;
        CurrentVelocity = Fsm.CurrentVelocity.WithY(Fsm.JumpVelocity);
        CurrentVelocity = CurrentVelocity.WithX(Fsm.Speed * horizontalForceDirection);
        Fsm.UpdateVelocity(CurrentVelocity);
    }

    public override void Exit()
    {
        Log.Debug("Exiting WallJumpMovementState...");
    }

    public override void PhysicsProcess(float delta)
    {
        if (Player.IsOnFloor())
        {
            Fsm.ChangeState(new IdleMovementState(Fsm));
            return;
        }

        if (Player.IsOnWall() && Input.IsActionJustPressed("jump"))
        {
            Fsm.ChangeState(new WallJumpMovementState(Fsm, LastWallSlide));
        }
    }
    
    private bool IsOnWallLeft()
    {
        for (int i = 0; i < Player.GetSlideCollisionCount(); i++)
        {
            KinematicCollision2D? collision = Player.GetSlideCollision(i);
            // Check if the collision normal is pointing right, indicating a collision on the left
            if (collision.GetNormal().X > 0)
            {
                return true;
            }
        }
        return false;
    }
    
    public override string ToString() => nameof(WallJumpMovementState);
}

public enum WallJumpDirection
{
    None,
    Left,
    Right
}