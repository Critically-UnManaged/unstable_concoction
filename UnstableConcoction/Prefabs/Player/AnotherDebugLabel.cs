using Godot;
using UnstableConcoction.Player.PlayerFsm;

namespace UnstableConcoction.Prefabs.Player;

public partial class AnotherDebugLabel : Control
{
    
    [Export] public Label PlayerStateLabel { get; private set; } = null!;
    [Export] public Label IsOnWallLabel { get; private set; } = null!;
    
    [Export] public PlayerMovementFsm PlayerFsm { get; private set; } = null!;

    public override void _Process(double delta)
    {
        PlayerStateLabel.Text = PlayerFsm.CurrentState?.ToString()?.Replace("MovementState", "");
        IsOnWallLabel.Text = PlayerFsm.Player.IsOnWall() ? "On Wall" : "Not On Wall";
    }
}