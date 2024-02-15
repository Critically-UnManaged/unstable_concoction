using Godot;

namespace UnstableConcoction;

[GlobalClass]
public partial class SceneManager : Node
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
        Node? newScene = _initialScene.Instantiate();
        AddChild(newScene);
    }
}