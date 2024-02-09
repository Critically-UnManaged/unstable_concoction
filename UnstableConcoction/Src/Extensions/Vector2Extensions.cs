using Vector2 = Godot.Vector2;

namespace UnstableConcoction.Extensions;

public static class Vector2Extensions
{
    /// <summary>
    /// Replaces the X value of the vector with the given value.
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="x"></param>
    /// <returns></returns>
    public static Vector2 WithX(this Vector2 vector, float x)
    {
        return new Vector2(x, vector.Y);
    }
    
    /// <summary>
    /// Replaces the Y value of the vector with the given value.
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static Vector2 WithY(this Vector2 vector, float y)
    {
        return new Vector2(vector.X, y);
    }
    
    /// <summary>
    /// Adds the given value to the X value of the vector.
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="x"></param>
    /// <returns></returns>
    public static Vector2 WithXPlus(this Vector2 vector, float x)
    {
        return new Vector2(vector.X + x, vector.Y);
    }
    
    /// <summary>
    /// Adds the given value to the Y value of the vector.
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static Vector2 WithYPlus(this Vector2 vector, float y)
    {
        return new Vector2(vector.X, vector.Y + y);
    }
    
    /// <summary>
    /// Subtracts the given value from the X value of the vector.
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="x"></param>
    /// <returns></returns>
    public static Vector2 WithXMinus(this Vector2 vector, float x)
    {
        return new Vector2(vector.X - x, vector.Y);
    }
    
    /// <summary>
    /// Subtracts the given value from the Y value of the vector.
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static Vector2 WithYMinus(this Vector2 vector, float y)
    {
        return new Vector2(vector.X, vector.Y - y);
    }
}