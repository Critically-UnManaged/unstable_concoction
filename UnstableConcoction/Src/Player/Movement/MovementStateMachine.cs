using UnstableConcoction.Player.Movement.States;

namespace UnstableConcoction.Player.Movement;

public class MovementStateMachine
{
    private readonly PlayerMovement _playerMovement;

    private MovementState? _currentState;

    public MovementState CurrentState
    {
        get => _currentState!;
        set => TransitionTo(value);
    }

    private void TransitionTo(MovementState state)
    {
        _currentState?.Exit();
        _currentState = state;
        _currentState.TransitionRequested = TransitionTo;
        _currentState.Enter();
    }

    public MovementStateMachine(PlayerMovement playerMovement)
    {
        _playerMovement = playerMovement;
        CurrentState = new IdleState(playerMovement);
    }
    
    public void Update(float delta)
    {
        _currentState?.Update(delta);
    }
}