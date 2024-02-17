using System.Data;
using Chickensoft.AutoInject;
using Godot;
using Serilog;
using SuperNodes.Types;
using UnstableConcoction.GdBinds;
using UnstableConcoction.Player;

namespace UnstableConcoction.Di;

[SuperNode(typeof(Provider))]
[GlobalClass]
public partial class NodesProvider: Node, IProvide<Player.Player>, IProvide<PhantomCamera2DWrapper>
{
    public override partial void _Notification(int what);
    public override void _Ready() => Provide();
    
    public Player.Player Value()
    {
        var playerGroup = GetTree().GetNodesInGroup("Player");
        
        switch (playerGroup.Count)
        {
            case 0:
                Log.Error("No player found in the scene");
                throw new NoNullAllowedException();
            case > 1:
                Log.Error("More than one player found in the scene");
                throw new ConstraintException();
            default:
                return (Player.Player) playerGroup[0];
        }
    }

    PhantomCamera2DWrapper IProvide<PhantomCamera2DWrapper>.Value()
    {
        var cameraGroup = GetTree().GetNodesInGroup("Camera");
        
        switch (cameraGroup.Count)
        {
            case 0:
                Log.Error("No phantom camera found in the scene");
                throw new NoNullAllowedException();
            default:
                return (PhantomCamera2DWrapper) cameraGroup[0];
        }
    }
}