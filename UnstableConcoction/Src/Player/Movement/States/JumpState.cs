using Godot;
using UnstableConcoction.Extensions;

namespace UnstableConcoction.Player.Movement.States;

public class JumpState: MovementState
{
    private bool ShouldTransitionToIdle =>
        PlayerMovement.IsOnFloor;
    
    private bool ShouldTransitionToRunning =>
        PlayerMovement.IsOnFloor && !CurrentVelocity.X.IsEqualApprox(0f) && IsPressingMovementKeys;
    
    private bool ShouldTransitionToWallJump =>
        PlayerMovement.IsOnWall && Input.IsActionJustPressed("jump");
    
    private static bool IsPressingMovementKeys =>
        Input.IsActionPressed("run_left") || Input.IsActionPressed("run_right");
    
    private bool ReleasedJump { get; set; }

    public float HoldingJumpTimer { get; private set; }
    public float AirTimer { get; private set; }
    
    public static float MinimumAirTime => 0.05f;
    
    public JumpState(PlayerMovement playerMovement) : base(playerMovement)
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
        AirTimer += delta;
        if (Input.IsActionJustReleased("jump"))
        {
            ReleasedJump = true;
        }
        
        if (!ReleasedJump && HoldingJumpTimer < PlayerMovement.JumpTimeToPeak)
        {
            HoldingJumpTimer += delta;
            
            // keep going up until the Jump velocity is reached
            if (CurrentVelocity.Y > PlayerMovement.JumpVelocity)
            {
                float weight = Mathf.Clamp(HoldingJumpTimer / PlayerMovement.JumpTimeToPeak, 0f, 1f);
                float vertical = CalculateJumpVelocity(weight);
                CurrentVelocity = CurrentVelocity.WithY(vertical);
            }
        }

        if (AirTimer <= MinimumAirTime)
        {
            return;
        }
        
        if (IsPressingMovementKeys)
        {
            float weight = Mathf.Clamp(AirTimer / PlayerMovement.FullJumpTime, 0f, 1f);
            float horizontal = CalculateInducedHorizontal(weight);
            CurrentVelocity = CurrentVelocity.WithX(horizontal);
        }
        
        if (ShouldTransitionToRunning)
        {
            TransitionRequested?.Invoke(new RunState(PlayerMovement));
            return;
        }
        
        if (ShouldTransitionToIdle)
        {
            TransitionRequested?.Invoke(new IdleState(PlayerMovement));
            return;
        }
        
        if (ShouldTransitionToWallJump)
        {
            TransitionRequested?.Invoke(new WallJumpState(PlayerMovement));
        }
    }
    
    private float CalculateJumpVelocity(float weight)
    {
        float force = Mathf.Min(PlayerMovement.MinimumJumpForce, CurrentVelocity.Y);
        return Mathf.Lerp(force, PlayerMovement.JumpVelocity, weight);
    }

    private float CalculateInducedHorizontal(float weight)
    {
        float horizontalAdjustment = 0f;
        
        if (Input.IsActionPressed("run_right"))
        {
            horizontalAdjustment += 1f * PlayerMovement.RunMaxSpeed * PlayerMovement.AirControl;
        }
        
        if (Input.IsActionPressed("run_left"))
        {
            horizontalAdjustment -= 1f * PlayerMovement.RunMaxSpeed * PlayerMovement.AirControl;
        }
        
        horizontalAdjustment = Mathf.Lerp(horizontalAdjustment, 0f, weight);
        float intendedVelocity = CurrentVelocity.X + horizontalAdjustment;

        intendedVelocity = CurrentVelocity.X switch
        {
            // going left
            < 0 => Mathf.Max(intendedVelocity, CurrentVelocity.X),
            // going right
            > 0 => Mathf.Min(intendedVelocity, CurrentVelocity.X),
            _ => intendedVelocity
        };

        return intendedVelocity;
    }
    
    public override string ToString() => "Jumping";
}