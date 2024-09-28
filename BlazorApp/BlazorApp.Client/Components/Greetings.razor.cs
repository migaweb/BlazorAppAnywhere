using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace BlazorApp.Client.Components;

public partial class Greetings : ComponentBase
{
  [Inject] HttpClient Http { get; set; } = null!;
  [Parameter] public string Name { get; set; } = string.Empty;

  private int _counter = 0;

  private async Task IncrementCounter()
  {
    _counter = await Http.GetFromJsonAsync<int>("api/Counter/IncrementCounter");
  }
}
