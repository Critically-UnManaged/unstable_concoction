using Godot;

namespace UnstableConcoction.Extensions;

public static class FloatExtensions
{
    public static bool IsEqualApprox(this float value, float compare)
    {
        return Mathf.IsEqualApprox(value, compare);
    }
}