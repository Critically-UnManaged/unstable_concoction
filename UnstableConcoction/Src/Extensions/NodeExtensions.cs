using System;
using Godot;

namespace UnstableConcoction.Extensions;

public static class NodeExtensions
{
    public static T Autoload<T>(this Node node) where T : Node
        => TryAutoload<T>(node) ?? throw new InvalidOperationException(
            "No singleton found for type " + typeof(T).Name
        );
    
    public static T? TryAutoload<T>(this Node node)
    {
        Type type = typeof(T);

        if (AutoloadCache.Has(type))
        {
            return (T) AutoloadCache.Read(type);
        }
        
        Node root = node.GetNode("/root/")!;
        foreach (Node autoload in (Godot.Collections.Array<Node>)root.GetChildren())
        {
            if (autoload is not T singleton) continue;
            AutoloadCache.Write(type, singleton);
            return singleton;
        }

        return default;
    }
}