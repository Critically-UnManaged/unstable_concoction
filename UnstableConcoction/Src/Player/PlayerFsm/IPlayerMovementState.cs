using Godot;

namespace UnstableConcoction.Player.PlayerFsm;

public interface IPlayerMovementState
{
    void Enter();
    void Exit();
    void PhysicsProcess(float delta);
    
}

public abstract class PlayerMovementMovementState: IPlayerMovementState
{
    protected PlayerMovementFsm Fsm { get; private set; }
    protected CharacterBody2D Player => Fsm.Player;
    protected Sprite2D Sprite => Fsm.Sprite;
    protected AnimationTree Animation => Fsm.Animation;
    protected Vector2 CurrentVelocity { get; set; }

    public PlayerMovementMovementState(PlayerMovementFsm fsm)
    {
        Fsm = fsm;
    }
    
    public abstract void Enter();
    public abstract void Exit();
    public abstract void PhysicsProcess(float delta);
}
