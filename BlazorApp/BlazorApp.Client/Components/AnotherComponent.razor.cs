using Microsoft.AspNetCore.Components;

namespace BlazorApp.Client.Components;

public partial class AnotherComponent : CustomElementComponentBase, IDisposable
{
  [Inject] private StateStore StateStore { get; set; } = null!;
  private string _newMessage = "Another state";

  private void UpdateState()
  {
    StateStore.UpdateState(_newMessage);
  }

  public void Dispose()
  {
    StateStore.RemoveStateChangeListeners(StateHasChanged);
  }

  protected override void OnAfterRender(bool firstRender)
  {
    if (firstRender)
      StateStore.AddStateChangeListeners(StateHasChanged);
    
    base.OnAfterRender(firstRender);
  }
}

public class CustomElementComponentBase : ComponentBase
{
  [Inject] protected IEnvironmentService EnvironmentService { get; set; } = null!;
  protected RuntimeEnvironment _environment;

  protected override void OnInitialized()
  {
    _environment = EnvironmentService.GetEnvironment();
    base.OnInitialized();
  }
}

public enum RuntimeEnvironment
{
  Blazor,
  WebForms
}

public interface IEnvironmentService
{
  RuntimeEnvironment GetEnvironment();
}

public class BlazorEnvironmentService : IEnvironmentService
{
  public RuntimeEnvironment GetEnvironment() => RuntimeEnvironment.Blazor;
}

public class WebFormsEnvironmentService : IEnvironmentService
{
  public RuntimeEnvironment GetEnvironment() => RuntimeEnvironment.WebForms;
}
