using System.Globalization;
using Godot;
using UnstableConcoction.Extensions;
using UnstableConcoction.Player;

namespace UnstableConcoction.Prefabs.Player;

public partial class DebugInformation : Control
{
    [Export][RequiredExport] private Label _isOnAirLabel = null!;
    [Export][RequiredExport] private Label _isOnWallLabel = null!;
    [Export][RequiredExport] private Label _coyoteTimeLabel = null!;
    [Export][RequiredExport] private Label _wallJumpIgnoreInputTime = null!;
    
    [Export][RequiredExport] private PlayerMovement _playerMovement = null!;
    [Export][RequiredExport] private CharacterBody2D _body = null!;
    
    public override void _Ready()
    {
        this.ValidateRequiredExports();
    }

    public override void _Process(double delta)
    {
        _isOnAirLabel.Text = (!_body.IsOnFloor()).ToString();
        _isOnWallLabel.Text = _body.IsOnWall().ToString();
        _coyoteTimeLabel.Text = _playerMovement.CoyoteTimeCounter.ToString(CultureInfo.InvariantCulture);
        _wallJumpIgnoreInputTime.Text = _playerMovement.WallJumpInputIgnoreTimer.ToString(CultureInfo.InvariantCulture);
    }
}