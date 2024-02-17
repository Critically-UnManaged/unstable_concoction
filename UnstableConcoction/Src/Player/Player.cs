using Godot;
using UnstableConcoction.Player.Movement;

namespace UnstableConcoction.Player;

[GlobalClass]
public partial class Player : CharacterBody2D
{
    [Export]
    public PlayerMovement PlayerMovement { get; private set; } = null!;
}