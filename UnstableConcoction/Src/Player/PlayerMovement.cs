using Godot;
using Serilog;
using UnstableConcoction.Extensions;

namespace UnstableConcoction.Player;

[GlobalClass()]
public partial class PlayerMovement: Node
{
    [Export][ExportGroup("Physics")] public float Speed = 600;
    [Export][ExportGroup("Physics")] public float JumpHeight = 64;
    [Export][ExportGroup("Physics")] public float JumpTimeToPeak = 0.2f;
    [Export][ExportGroup("Physics")] public float JumpTimeToFall = 0.2f;
    [Export][ExportGroup("Physics")] public float CoyoteTimeDuration = 0.5f;
    [Export] [ExportGroup("Physics")] public float WallJumpForce = 300;
    [Export] [ExportGroup("Physics")] public float WallJumpInputIgnoreDuration = 0.5f;
    
    [Export][ExportGroup("Dependencies")] public CharacterBody2D Player = null!;
    [Export][ExportGroup("Dependencies")] private Sprite2D _sprite = null!;
    [Export][ExportGroup("Dependencies")] private AnimationTree _tree = null!;

    private float JumpVelocity => (float)((2.0 * JumpHeight) / JumpTimeToPeak) * -1.0f;
    private float JumpGravity => (float)((-2.0 * JumpHeight) / (JumpTimeToPeak * JumpTimeToPeak)) * -1.0f;
    private float FallGravity => (float)((-2.0 * JumpHeight) / (JumpTimeToFall * JumpTimeToFall)) * -1.0f;
    private float Gravity => Player.Velocity.Y < 0.0f ? JumpGravity : FallGravity;
    private float _coyoteTimeCounter;
    private float _wallJumpInputIgnoreTimer;
    private WallJumpDirection _lastWallJumpDirection = WallJumpDirection.None;

    
    public override void _PhysicsProcess(double delta)
    {
        Player.Velocity = Player.Velocity.WithYPlus(Gravity * (float)delta);
        
        if (_lastWallJumpDirection == WallJumpDirection.None && _wallJumpInputIgnoreTimer <= 0.0f)
        {
            Player.Velocity = Player.Velocity.WithX(GetHorizontalVelocity() * Speed);
        }
        
        _coyoteTimeCounter = Player.IsOnFloor() ? CoyoteTimeDuration : _coyoteTimeCounter - (float)delta;
        _wallJumpInputIgnoreTimer = _wallJumpInputIgnoreTimer > 0 ? _wallJumpInputIgnoreTimer - (float)delta : 0;
        
        if (Input.IsActionJustPressed("jump"))
        {
            Jump();
        }
        
        if (Player.IsOnFloor())
        {
            _lastWallJumpDirection = WallJumpDirection.None;
        }
        
        UpdateAnimations();
        Player.MoveAndSlide();
    }

    private float GetHorizontalVelocity()
    {
        float horizontal = 0.0f;
        
        if (Input.IsActionPressed("run_left"))
        {
            Log.Debug("Override horizontal velocity to -1.0f (left)");
            horizontal -= 1.0f;
        }
        if (Input.IsActionPressed("run_right"))
        {
            Log.Debug("Override horizontal velocity to 1.0f (right)");
            horizontal += 1.0f;
        }
        
        return horizontal;
    }

    private void Jump()
    {
        bool isOnLeftWall = IsOnWallLeft();
        bool isOnRightWall = !isOnLeftWall && Player.IsOnWall();
        
        if ((isOnLeftWall || isOnRightWall) && !Player.IsOnFloor())
        {
            Log.Debug("Performing wall jump");
            WallJumpDirection currentWallSide = isOnLeftWall ? WallJumpDirection.Left : WallJumpDirection.Right;

            if (_lastWallJumpDirection == currentWallSide && _lastWallJumpDirection != WallJumpDirection.None) return;
            float horizontalForceDirection = isOnLeftWall ? 1 : -1;
            
            Player.Velocity = Player.Velocity.WithY(JumpVelocity);
            Player.Velocity = Player.Velocity.WithX(WallJumpForce * horizontalForceDirection);
        
            _lastWallJumpDirection = currentWallSide;
            _wallJumpInputIgnoreTimer = WallJumpInputIgnoreDuration;
            return;
        }
        
        if (_coyoteTimeCounter > 0.0f)
        {
            Log.Debug("Performing coyote time jump");
            Player.Velocity = Player.Velocity.WithY(JumpVelocity);
            _coyoteTimeCounter = 0.0f;
            _lastWallJumpDirection = WallJumpDirection.None;
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

    private void UpdateAnimations()
    {
        if (!Player.Velocity.X.IsAbout(0.0f))
        {
            _sprite.FlipH = Player.Velocity.X < 0.0f;
        }
        
        if (!Player.IsOnFloor() && _tree.GetCondition(PlayerAnimationsConditions.IS_JUMPING))
        {
            return;
        }
        
        if (!Player.IsOnFloor() && !_tree.GetCondition(PlayerAnimationsConditions.IS_JUMPING))
        {
            _tree.SetCondition(PlayerAnimationsConditions.IS_IDLE, false);
            _tree.SetCondition(PlayerAnimationsConditions.IS_RUNNING, false);
            _tree.SetCondition(PlayerAnimationsConditions.IS_JUMPING, true);
            return;
        }
        
        if (_tree.GetCondition(PlayerAnimationsConditions.IS_JUMPING) && Player.IsOnFloor())
        {
            _tree.SetCondition(PlayerAnimationsConditions.IS_JUMPING, false);
            _tree.SetCondition(PlayerAnimationsConditions.IS_IDLE, true);
            return;
        }

        if (!Player.Velocity.X.IsAbout(0.0f) && Player.IsOnFloor())
        {
            _tree.SetCondition(PlayerAnimationsConditions.IS_IDLE, false);
            _tree.SetCondition(PlayerAnimationsConditions.IS_RUNNING, true);
            return;
        }
        
        _tree.SetCondition(PlayerAnimationsConditions.IS_RUNNING, false);
        _tree.SetCondition(PlayerAnimationsConditions.IS_JUMPING, false);
        _tree.SetCondition(PlayerAnimationsConditions.IS_IDLE, true);
    }
    
    private enum WallJumpDirection
    {
        None,
        Left,
        Right
    }
}