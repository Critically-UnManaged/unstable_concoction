using Godot;
using Serilog;
using UnstableConcoction.Extensions;
using UnstableConcoction.Player.PlayerFsm.States;

namespace UnstableConcoction.Player.PlayerFsm;

[GlobalClass]
public partial class PlayerMovementFsm: Node
{
    #region Exports

    [Export]
    [ExportGroup("Physics")]
    public float Speed { get; set; } = 400;

    [Export]
    [ExportGroup("Physics")] 
    public float JumpHeight { get; set; } =  64;
    [Export]
    [ExportGroup("Physics")] 
    public float JumpTimeToPeak { get; set; } = 0.3f;
    [Export]
    [ExportGroup("Physics")] 
    public float JumpTimeToFall { get; set; } = 0.25f;
    [Export]
    [ExportGroup("Physics")] 
    public float CoyoteTimeDuration { get; set; } = 0.3f;
    [Export]
    [ExportGroup("Physics")] 
    public float WallJumpForce { get; set; } = 300;
    [Export][ExportGroup("Physics")] 
    public float WallJumpInputIgnoreDuration { get; private set; } = 0.5f;
    
    [Export]
    [RequiredExport]
    [ExportGroup("Dependencies")]
    public CharacterBody2D Player { get; private set; } = null!;
    
    [Export]
    [RequiredExport]
    [ExportGroup("Dependencies")]
    public Sprite2D Sprite { get; private set; } = null!;
    
    [Export]
    [RequiredExport]
    [ExportGroup("Dependencies")]
    public AnimationTree Animation { get; private set; } = null!;

    #endregion

    public IPlayerMovementState? CurrentState { get; private set; }
    public Vector2 CurrentVelocity { get; private set; }

    public float JumpVelocity => (float)((2.0 * JumpHeight) / JumpTimeToPeak) * -1.0f;
    public float JumpGravity => (float)((-2.0 * JumpHeight) / (JumpTimeToPeak * JumpTimeToPeak)) * -1.0f;
    public float FallGravity => (float)((-2.0 * JumpHeight) / (JumpTimeToFall * JumpTimeToFall)) * -1.0f;
    public float Gravity => Player.Velocity.Y < 0.0f ? JumpGravity : FallGravity;

    public override void _Ready()
    {
        this.ValidateRequiredExports();
        ChangeState(new IdleMovementState(this));
    }
    
    public void ChangeState(IPlayerMovementState newMovementState)
    {
        Log.Debug("Changing state to {StateName}", newMovementState.ToString());
        CurrentState?.Exit();
        CurrentState = newMovementState;
        CurrentState.Enter();
    }
    
    public void UpdateVelocity(Vector2 velocity)
    {
        CurrentVelocity = velocity;
    }

    public override void _PhysicsProcess(double delta)
    {
        ApplyGravity(delta);
        CurrentState?.PhysicsProcess((float)delta);
        
        Player.Velocity = CurrentVelocity;
        HandleFacing();
        Player.MoveAndSlide();
    }

    private void ApplyGravity(double delta)
    {
        CurrentVelocity = CurrentVelocity.WithYPlus(Gravity * (float)delta);
    }

    private void HandleFacing()
    {
        if (CurrentVelocity.X.IsAbout(0.0f))
        {
            return;
        }
        
        Sprite.FlipH = CurrentVelocity.X < 0.0f;
    }
}