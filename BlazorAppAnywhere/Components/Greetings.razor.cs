using Microsoft.AspNetCore.Components;

namespace BlazorAppAnywhere.Components;

public partial class Greetings : ComponentBase
{
  [Parameter] public string Name { get; set; } = string.Empty;

  private int _counter = 0;

  private void IncrementCounter()
  {
    _counter++;
  }
}
