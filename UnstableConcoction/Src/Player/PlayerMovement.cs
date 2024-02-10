using System;
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
    [Export][ExportGroup("Physics")] public float WallJumpForce = 300;
    [Export][ExportGroup("Physics")] public float WallJumpInputIgnoreDuration = 0.5f;
    
    [Export][RequiredExport][ExportGroup("Dependencies")]
    public CharacterBody2D Player = null!;
    
    [Export][RequiredExport][ExportGroup("Dependencies")]
    private Sprite2D _sprite = null!;
    
    [Export][RequiredExport][ExportGroup("Dependencies")]
    private AnimationTree _tree = null!;

    private float JumpVelocity => (float)((2.0 * JumpHeight) / JumpTimeToPeak) * -1.0f;
    private float JumpGravity => (float)((-2.0 * JumpHeight) / (JumpTimeToPeak * JumpTimeToPeak)) * -1.0f;
    private float FallGravity => (float)((-2.0 * JumpHeight) / (JumpTimeToFall * JumpTimeToFall)) * -1.0f;
    private float Gravity => Player.Velocity.Y < 0.0f ? JumpGravity : FallGravity;
    public float CoyoteTimeCounter { get; private set; }
    public float WallJumpInputIgnoreTimer { get; private set; }
    public WallJumpDirection LastWallJumpDirection { get; private set; } = WallJumpDirection.None;
    public bool DidCoyoteJump { get; private set; }


    public override void _PhysicsProcess(double delta)
    {
        if (Player.IsOnFloor())
        {
            CoyoteTimeCounter = CoyoteTimeDuration;
            LastWallJumpDirection = WallJumpDirection.None;
            DidCoyoteJump = false;
        }
        
        Player.Velocity = Player.Velocity.WithYPlus(Gravity * (float)delta);
        
        if (LastWallJumpDirection == WallJumpDirection.None && WallJumpInputIgnoreTimer <= 0.0f)
        {
            Player.Velocity = Player.Velocity.WithX(GetHorizontalVelocity() * Speed);
        }
        
        if (!Player.IsOnFloor() && CoyoteTimeCounter > 0.0f)
        {
            CoyoteTimeCounter = Math.Clamp(CoyoteTimeCounter - (float)delta, 0.0f, CoyoteTimeDuration);
        }
        
        WallJumpInputIgnoreTimer = WallJumpInputIgnoreTimer > 0 ? WallJumpInputIgnoreTimer - (float)delta : 0;
        
        if (Input.IsActionJustPressed("jump"))
        {
            TryJump();
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

    private void TryJump()
    {
        if (CoyoteTimeCounter > 0.0f && !DidCoyoteJump)
        {
            Log.Information("Performing coyote time jump");
            Jump();
            return;
        }
        
        if (Player.IsOnFloor())
        {
            Log.Information("Performing regular jump");
            Jump();
            return;
        }

        HandleWallJump();
    }

    private void Jump()
    {
        Player.Velocity = Player.Velocity.WithY(JumpVelocity);
        LastWallJumpDirection = WallJumpDirection.None;
    }
    
    private void HandleWallJump()
    {
        bool isOnLeftWall = IsOnWallLeft();
        bool isOnRightWall = !isOnLeftWall && Player.IsOnWall();
        
        if ((isOnLeftWall || isOnRightWall) && !Player.IsOnFloor())
        {
            Log.Debug("Performing wall jump");
            WallJumpDirection currentWallSide = isOnLeftWall ? WallJumpDirection.Left : WallJumpDirection.Right;

            if (LastWallJumpDirection == currentWallSide && LastWallJumpDirection != WallJumpDirection.None) return;
            float horizontalForceDirection = isOnLeftWall ? 1 : -1;
            
            Player.Velocity = Player.Velocity.WithY(JumpVelocity);
            Player.Velocity = Player.Velocity.WithX(WallJumpForce * horizontalForceDirection);
        
            LastWallJumpDirection = currentWallSide;
            WallJumpInputIgnoreTimer = WallJumpInputIgnoreDuration;
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
        
        if (!Player.IsOnFloor() && _tree.GetCondition(AnimConditions.IS_JUMPING))
        {
            return;
        }
        
        if (!Player.IsOnFloor() && !_tree.GetCondition(AnimConditions.IS_JUMPING))
        {
            _tree.SetCondition(AnimConditions.IS_IDLE, false);
            _tree.SetCondition(AnimConditions.IS_RUNNING, false);
            _tree.SetCondition(AnimConditions.IS_JUMPING, true);
            return;
        }
        
        if (_tree.GetCondition(AnimConditions.IS_JUMPING) && Player.IsOnFloor())
        {
            _tree.SetCondition(AnimConditions.IS_JUMPING, false);
            _tree.SetCondition(AnimConditions.IS_IDLE, true);
            return;
        }

        if (!Player.Velocity.X.IsAbout(0.0f) && Player.IsOnFloor())
        {
            _tree.SetCondition(AnimConditions.IS_IDLE, false);
            _tree.SetCondition(AnimConditions.IS_RUNNING, true);
            return;
        }
        
        _tree.SetCondition(AnimConditions.IS_RUNNING, false);
        _tree.SetCondition(AnimConditions.IS_JUMPING, false);
        _tree.SetCondition(AnimConditions.IS_IDLE, true);
    }

    public override void _Ready()
    {
        this.ValidateRequiredExports();
    }

    public enum WallJumpDirection
    {
        None,
        Left,
        Right
    }
}