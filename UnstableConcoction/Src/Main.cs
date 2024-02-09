using Godot;

namespace UnstableConcoction;

public partial class Main : Node
{
    [Export] private PackedScene _initialScene = null!;
    
    public override void _Ready()
    {
        StartGame();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("reset"))
        {
            StartGame();
        }
    }
    
    private void StartGame()
    {
        var children = GetChildren();
        foreach (Node child in children)
        {
            if (child.Name != "Ui")
            {
                child.QueueFree();
            }
        }

        CallDeferred(nameof(InstantiateScene));
    }
    
    public void InstantiateScene()
    {
        Node? newScene = _initialScene.Instantiate();
        AddChild(newScene);
    }
}