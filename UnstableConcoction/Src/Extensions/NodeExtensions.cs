using System;
using System.Reflection;
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

    public static void ValidateRequiredExports(this Node node)
    {
        // Reflecting fields
        FieldInfo[] fields = node.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        ValidateMembers(node, fields);

        // Reflecting properties
        var properties = node.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        ValidateMembers(node, properties);
    }
    
    private static void ValidateMembers(Node node, MemberInfo[] members)
    {
        foreach (MemberInfo member in members)
        {
            bool isRequiredExport = Attribute.IsDefined(member, typeof(RequiredExportAttribute));
            bool isExported = Attribute.IsDefined(member, typeof(ExportAttribute));

            // Proceed with validation only if both RequiredExport and Export attributes are present
            if (!isRequiredExport || !isExported) continue;
            object? value = null;
            
            if (member.MemberType == MemberTypes.Field)
            {
                value = ((FieldInfo)member).GetValue(node);
            }
            
            else if (member.MemberType == MemberTypes.Property)
            {
                var propInfo = (PropertyInfo)member;
                // Ensure the property has a getter before trying to get its value
                if (propInfo.GetGetMethod(nonPublic: true) != null)
                {
                    value = propInfo.GetValue(node);
                }
            }


            if (value == null || (value is Node && !node.IsInsideTree()))
            {
                throw new InvalidOperationException($"Required exported member '{member.Name}' in {node.Name} is not set was freed.");
            }
        }
    }
}