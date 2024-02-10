using Godot;
using UnstableConcoction.Extensions;
using UnstableConcoction.Interactions;

namespace UnstableConcoction.Player;

[GlobalClass]
public partial class PlayerInteraction: Node2D
{
    [Export][RequiredExport] private Area2D _interactionArea = null!;
    private IInteractable? _interactable;

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("interact") && _interactable != null)
        {
            _interactable.Interact();
        }
    }
    
    public override void _Ready()
    {
        this.ValidateRequiredExports();
        _interactionArea.BodyEntered += OnBodyEntered;
    }

    public override void _ExitTree()
    {
        _interactionArea.BodyExited -= OnBodyExited;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is IInteractable interactable)
        {
            _interactable = interactable;
        }
    }
    
    private void OnBodyExited(Node2D body)
    {
        if (body is IInteractable interactable && interactable == _interactable)
        {
            _interactable = null;
        }
    }
}