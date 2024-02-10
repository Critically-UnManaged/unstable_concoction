using Godot;
using Serilog;
using UnstableConcoction.Extensions;

namespace UnstableConcoction.Autoloads;

public partial class SerilogInitializer: Node
{
    public override void _Ready()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.Godot()
            .CreateLogger();
    }
}