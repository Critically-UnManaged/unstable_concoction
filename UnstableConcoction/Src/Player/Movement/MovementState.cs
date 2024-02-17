using System;
using Godot;

namespace UnstableConcoction.Player.Movement;

public abstract class MovementState
{
    protected PlayerMovement PlayerMovement { get; }
    protected AnimationTree Animation => PlayerMovement.AnimationTree;
    public Action<MovementState>? TransitionRequested;
    
    /// <summary>
    /// Represents the real velocity of the player object.
    /// </summary>
    protected Vector2 CurrentVelocity
    {
        get => PlayerMovement.Player.Velocity;
        set => UpdateRealVelocity(value);
    }
    
    /// <summary>
    /// Represents the velocity that the player is trying to induce into the player object.
    /// This value should then be added to the current velocity to some extent, depending on the state.
    /// </summary>
    protected Vector2 PlayerInducedVelocity
    {
        get;
        set;
    }

    public abstract void Enter();
    public abstract void Exit();
    public abstract void Update(float delta);
    
    private void UpdateRealVelocity(Vector2 velocity)
    {
        PlayerMovement.Player.Velocity = velocity;
    }
    
    public MovementState(PlayerMovement playerMovement)
    {
        PlayerMovement = playerMovement;
    }

    public abstract override string ToString();
}