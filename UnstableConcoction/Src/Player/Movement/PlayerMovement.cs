using Godot;
using UnstableConcoction.Extensions;

namespace UnstableConcoction.Player.Movement;

[GlobalClass]
public partial class PlayerMovement: Node
{
    #region Dependencies
    [ExportGroup("Dependencies")]
    [Export]
    public Player Player { get; private set; } = null!;
    
    [ExportGroup("Dependencies")]
    [Export]
    public AnimationTree AnimationTree { get; private set; } = null!;
    
    [ExportGroup("Dependencies")]
    [Export]
    public Sprite2D Sprite { get; private set; } = null!;
    #endregion

    #region GeneralPhysics
    [ExportGroup("General physics")]
    [Export(PropertyHint.Range, "0,1,")]
    public float AirControl { get; private set; } = 0.8f;
    
    #endregion
    
    #region Running physics
    [ExportGroup("Running physics")]
    [Export]
    public float RunMaxSpeed { get; set; } = 500f;
    
    [ExportGroup("Running physics")]
    [Export(PropertyHint.Range, "0,1,")]
    public float RunAcceleration { get; private set; } = 0.1f;
    
    
    [ExportGroup("Running physics")]
    [Export(PropertyHint.Range, "0,1,")]
    public float RunFriction { get; private set; } = 0.3f;
    
    [ExportGroup("Running physics")]
    [Export]
    public float CoyoteTimeDuration { get; set; } = 0.2f;
    #endregion

    #region Jumping physics
    [ExportGroup("Jumping Physics")] 
    [Export]
    public float JumpHeight { get; set; } =  32;
    
    [ExportGroup("Jumping Physics")]
    [Export]
    public float MinimumJumpForce { get; set; } = -100;
    
    [ExportGroup("Jumping Physics")]
    [Export]
    public float JumpTimeToPeak { get; set; } = 0.2f;
    
    [ExportGroup("Jumping Physics")]
    [Export]
    public float JumpTimeToFall { get; set; } = 0.14f;
    
    [ExportGroup("Jumping Physics")]
    [Export]
    public float WalljumpHorizontalForce { get; set; } = 400;
    #endregion
    
    public float FullJumpTime => JumpTimeToPeak + JumpTimeToFall;
    public float JumpVelocity => (float)((2.0 * JumpHeight) / JumpTimeToPeak) * -1.0f;
    public float JumpGravity => (float)((-2.0 * JumpHeight) / (JumpTimeToPeak * JumpTimeToPeak)) * -1.0f;
    public float FallGravity => (float)((-2.0 * JumpHeight) / (JumpTimeToFall * JumpTimeToFall)) * -1.0f;
    public float Gravity => Player.Velocity.Y < 0.0f ? JumpGravity : FallGravity;
    
    public MovementStateMachine StateMachine { get; private set; } = null!;
    public bool IsOnFloor => Player.IsOnFloor();
    public bool IsOnWall => Player.IsOnWall();


    public override void _Ready()
    {
        StateMachine = new MovementStateMachine(this);
    }

    public override void _PhysicsProcess(double delta)
    {
        ApplyGravity(delta);
        UpdateFacing();
        StateMachine.Update((float)delta);
        Player.MoveAndSlide();
    }

    private void UpdateFacing()
    {
        if (Player.Velocity.X.IsEqualApprox(0.0f))
        {
            return;
        }
        
        Sprite.FlipH = Player.Velocity.X < 0.0f;
    }
    
    private void ApplyGravity(double delta)
    {
        Player.Velocity = Player.Velocity.WithYPlus(Gravity * (float)delta);
    }
}