using Godot;

namespace UnstableConcoction.Extensions;

public static class AnimationTreeExtensions
{
    public static void SetCondition(this AnimationTree animationTree, string condition, bool value)
    {
        animationTree.Set($"parameters/conditions/{condition}", value);
    }
    
    public static bool GetCondition(this AnimationTree animationTree, string condition)
    {
        return (bool) animationTree.Get($"parameters/conditions/{condition}");
    }
}