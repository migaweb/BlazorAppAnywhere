namespace BlazorApp.Client;

public record State(string CurrentMessage)
{
}

public class StateStore
{
  private State _state = new ("Initial state");
  private Action _listeners = null!;

  public State GetState()
  {
    return _state;
  }

  public void UpdateState(string newMessage)
  {
    _state = _state with { CurrentMessage = newMessage };
    BroadcastStateChanged();
  }

  public void AddStateChangeListeners(Action listener)
  {
    _listeners += listener;
  }

  public void RemoveStateChangeListeners(Action listener)
  {
#pragma warning disable CS8601 // Possible null reference assignment.
    _listeners -= listener;
#pragma warning restore CS8601 // Possible null reference assignment.
  }

  private void BroadcastStateChanged()
  {
    _listeners.Invoke();
  }
}
