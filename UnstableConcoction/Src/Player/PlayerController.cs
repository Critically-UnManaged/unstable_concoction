using Godot;
using UnstableConcoction.Player.PlayerFsm;

namespace UnstableConcoction.Player;

public partial class PlayerController : CharacterBody2D
{
    public PlayerMovementFsm? Fsm { get; private set; }

    public override void _Ready()
    {
        Fsm = GetNode<PlayerMovementFsm>("PlayerMovementFsm");
    }
}