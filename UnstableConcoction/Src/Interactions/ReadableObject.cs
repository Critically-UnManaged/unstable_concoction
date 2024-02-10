using Godot;
using Serilog;

namespace UnstableConcoction.Interactions;

public partial class ReadableObject : Node2D, IInteractable
{
    [Export] private string _text = string.Empty;
    
    public void Interact()
    {
        Log.Information(_text);
    }
}