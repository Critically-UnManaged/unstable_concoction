using System;
using System.Globalization;
using Chickensoft.AutoInject;
using Godot;
using SuperNodes.Types;
using UnstableConcoction.Player;
using UnstableConcoction.Player.PlayerFsm;

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

    [Dependency] private PlayerController Pc => DependOn<PlayerController>();
    private PlayerMovementFsm? Player => Pc.Fsm;
   

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
            if (Player != null) Player.Speed = speed;
        }
        
    }
    
    private void UpdateJumpHeight()
    {
        if (float.TryParse(_jumpHeightInput.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float jumpHeight))
        {
            if (Player != null) Player.JumpHeight = jumpHeight;
        }
    }
    
    private void UpdateJumpTimeToPeak()
    {
        if (float.TryParse(_jumpTimeToPeakInput.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float jumpTimeToPeak))
        {
            if (Player != null) Player.JumpTimeToPeak = jumpTimeToPeak;
        }
    }
    
    private void UpdateJumpTimeToFall()
    {
        if (float.TryParse(_jumpTimeToFallInput.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float jumpTimeToFall))
        {
            if (Player != null) Player.JumpTimeToFall = jumpTimeToFall;
        }
    }
    
    private void UpdateCoyoteTimeDuration()
    {
        if (float.TryParse(_coyoteTimeDurationInput.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float coyoteTimeDuration))
        {
            if (Player != null) Player.CoyoteTimeDuration = coyoteTimeDuration;
        }
    }
    
    private void UpdateWallJumpForce()
    {
        if (float.TryParse(_wallJumpForceInput.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out float wallJumpForce))
        {
            if (Player != null) Player.WallJumpForce = wallJumpForce;
        }
    }
    
    public override void _UnhandledInput(InputEvent @event)
    {
        if (!Input.IsActionJustPressed("physics_panel")) return;

        if (_firstTime)
        {
            _speedInput.Text = Player?.Speed.ToString(CultureInfo.InvariantCulture);
            _jumpHeightInput.Text = Player?.JumpHeight.ToString(CultureInfo.InvariantCulture);
            _jumpTimeToPeakInput.Text = Player?.JumpTimeToPeak.ToString(CultureInfo.InvariantCulture);
            _jumpTimeToFallInput.Text = Player?.JumpTimeToFall.ToString(CultureInfo.InvariantCulture);
            _coyoteTimeDurationInput.Text = Player?.CoyoteTimeDuration.ToString(CultureInfo.InvariantCulture);
            _wallJumpForceInput.Text = Player?.WallJumpForce.ToString(CultureInfo.InvariantCulture);
            _firstTime = false;
        }
        
        _animationPlayer.Play(_visible ? "slide_out" : "slide_in");
        _visible = !_visible;
    }
}