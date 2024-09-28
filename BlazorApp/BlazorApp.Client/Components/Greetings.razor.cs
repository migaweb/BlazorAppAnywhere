using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace BlazorApp.Client.Components;

public partial class Greetings : ComponentBase
{
  [Inject] HttpClient Http { get; set; } = null!;
  [Inject] IJSRuntime JSRuntime { get; set; } = null!;
  [Inject] BlazorEventHelper BlazorEventHelper { get; set; } = null!;
  [Parameter] public string Name { get; set; } = string.Empty;

  private int _counter = 0;
  private string _localStorageValue = string.Empty;
  private string _eventFromWebForms = string.Empty;
  private string _messageFromWebForms = string.Empty;
  private DotNetObjectReference<BlazorEventHelper>? _dotNetHelper;
  private bool _isLoading = true;

  protected override async Task OnAfterRenderAsync(bool firstRender)
  {
    if (firstRender)
    {
      BlazorEventHelper.OnReceivedEvent += HandleBlazorEvent;
      BlazorEventHelper.OnErrorEvent += HandleBlazorError;
      _dotNetHelper = DotNetObjectReference.Create(BlazorEventHelper);
      await JSRuntime.InvokeVoidAsync("setupBlazorEventListener", _dotNetHelper);
      _localStorageValue = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "WebFormsValue");

      _isLoading = false;
      StateHasChanged();
    }
    await base.OnAfterRenderAsync(firstRender);
  }

  public async Task HandleBlazorEvent(string message)
  {
    // Handle the data received from the web forms application
    _eventFromWebForms = message;
    await IncrementCounter();
    StateHasChanged();

    Console.WriteLine($"Received from Web Forms: {message}");
  }

  public async Task HandleBlazorError(string message)
  {
    Console.WriteLine($"Error event from Web Forms: {message}");
    // Handle the data received from the web forms application
    _messageFromWebForms = message;

    StateHasChanged();
  }

  private async Task DispatchEventToWebForms()
  {
    await JSRuntime.InvokeVoidAsync("dispatchCustomBlazorEvent", "custom-event", "Data from Blazor");
  }

  private async Task IncrementCounter()
  {
    _counter = await Http.GetFromJsonAsync<int>("api/Counter/IncrementCounter");
  }

}

public class BlazorEventHelper
{
  public static event Func<string, Task>? OnReceivedEvent;
  public static event Func<string, Task>? OnErrorEvent;

  [JSInvokable]
  public static Task HandleBlazorEvent(string name)
  {
    OnReceivedEvent?.Invoke(name);

    return Task.CompletedTask;
  }

  [JSInvokable]
  public static Task HandleBlazorError(string name)
  {
    OnErrorEvent?.Invoke(name);

    return Task.CompletedTask;
  }
}



