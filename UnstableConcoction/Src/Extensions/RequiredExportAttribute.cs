using System;

namespace UnstableConcoction.Extensions;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class RequiredExportAttribute: Attribute
{
}