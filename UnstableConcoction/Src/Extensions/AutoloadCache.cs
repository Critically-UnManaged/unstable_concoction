using System;
using System.Collections.Generic;

namespace UnstableConcoction.Extensions;

public static class AutoloadCache
{
    private static readonly Dictionary<Type, object> Cache = new();
    public static void Write(Type type, object value) => Cache.Add(type, value);
    public static object Read(Type type) => Cache[type];
    public static bool Has(Type type) => Cache.ContainsKey(type);
}