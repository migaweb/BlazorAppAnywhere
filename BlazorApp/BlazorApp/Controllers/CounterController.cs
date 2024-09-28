using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CounterController : ControllerBase
{
  private static int counter = 0;
  [HttpGet("IncrementCounter")]
  public async Task<int> IncrementCounter()
  {
    counter++;

    return counter;
  }
}
