using System.Globalization;
using Chickensoft.AutoInject;
using Godot;
using SuperNodes.Types;
using UnstableConcoction.Player.Movement;

namespace UnstableConcoction.Tools;

[SuperNode(typeof(Dependent))]
public partial class PlayerPhysicsPanel : PanelContainer
{
    public override partial void _Notification(int what);
    
    [Export] private LineEdit _speedInput = null!;
    [Export] private LineEdit _jumpHeightInput = null!;
    [Export] private LineEdit _jumpTimeToPeakInput = null!;
    [Export] private LineEdit _jumpTimeToFallInput = null!;
    [Export] private LineEdit _coyoteTimeDurationInput = null!;
    [Export] private LineEdit _wallJumpForceInput = null!;
    [Export] private AnimationPlayer _animationPlayer = null!;
    
    private bool _visible;
    private bool _firstTime = true;

    [Dependency] private Player.Player Pc => DependOn<Player.Player>();
    private PlayerMovement Player => Pc.PlayerMovement;
   

    public override void _Ready()
    {
        _speedInput.FocusExited += UpdateSpeed;
        _jumpHeightInput.FocusExited += UpdateJumpHeight;
        _jumpTimeToPeakInput.FocusExited += UpdateJumpTimeToPeak;
        _jumpTimeToFallInput.FocusExited += UpdateJumpTimeToFall;
        _coyoteTimeDurationInput.FocusExited += UpdateCoyoteTimeDuration;
        _wallJumpForceInput.FocusExited += UpdateWallJumpForce;
    }

    private void UpdateSpeed()
    {
        if (float.TryParse(_speedInput.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float speed))
        {
            Player.RunMaxSpeed = speed;
        }
        
    }
    
    private void UpdateJumpHeight()
    {
        if (float.TryParse(_jumpHeightInput.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float jumpHeight))
        {
            Player.JumpHeight = jumpHeight;
        }
    }
    
    private void UpdateJumpTimeToPeak()
    {
        if (float.TryParse(_jumpTimeToPeakInput.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float jumpTimeToPeak))
        {
            Player.JumpTimeToPeak = jumpTimeToPeak;
        }
    }
    
    private void UpdateJumpTimeToFall()
    {
        if (float.TryParse(_jumpTimeToFallInput.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float jumpTimeToFall))
        {
            Player.JumpTimeToFall = jumpTimeToFall;
        }
    }
    
    private void UpdateCoyoteTimeDuration()
    {
        if (float.TryParse(_coyoteTimeDurationInput.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float coyoteTimeDuration))
        {
            Player.CoyoteTimeDuration = coyoteTimeDuration;
        }
    }
    
    private void UpdateWallJumpForce()
    {
        if (float.TryParse(_wallJumpForceInput.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float wallJumpForce))
        {
            Player.WalljumpHorizontalForce = wallJumpForce;
        }
    }
    
    public override void _UnhandledInput(InputEvent @event)
    {
        if (!Input.IsActionJustPressed("physics_panel")) return;

        if (_firstTime)
        {
            _speedInput.Text = Player?.RunMaxSpeed.ToString(CultureInfo.InvariantCulture);
            _jumpHeightInput.Text = Player?.JumpHeight.ToString(CultureInfo.InvariantCulture);
            _jumpTimeToPeakInput.Text = Player?.JumpTimeToPeak.ToString(CultureInfo.InvariantCulture);
            _jumpTimeToFallInput.Text = Player?.JumpTimeToFall.ToString(CultureInfo.InvariantCulture);
            _coyoteTimeDurationInput.Text = Player?.CoyoteTimeDuration.ToString(CultureInfo.InvariantCulture);
            _wallJumpForceInput.Text = Player?.WalljumpHorizontalForce.ToString(CultureInfo.InvariantCulture);
            _firstTime = false;
        }
        
        _animationPlayer.Play(_visible ? "slide_out" : "slide_in");
        _visible = !_visible;
    }
}