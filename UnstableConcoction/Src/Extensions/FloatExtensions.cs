using Godot;

namespace UnstableConcoction.Extensions;

public static class FloatExtensions
{
    public static bool IsAbout(this float value, float compare)
    {
        return Mathf.Abs(value - compare) < 0.01f;
    }
}