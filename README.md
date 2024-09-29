# Custom elements with Blazor
Using Blazor custom elements in a Web Forms application.

## Notes
### Application state
If using several Blazor components in the same Web Forms page, they all share the same application state.

### Local storage
The same Local storage and Session storage can be accessed by both the Web Forms app and the Blazor Custom Elements.

### Parameters
Use parameters in the component to initialize the component. Parameters can also be manipulated during the
lifetime of the page.

Blazor custom elements support dynamic updates to parameters. So when the name parameter, below example,
is updated the UI will reflect the change (if the name is displayed in the UI markup).
```html
<blazor-greeting name="Initial Value"></blazor-greeting>
```
```js
var blazorGreeting = document.querySelector('blazor-greeting');

// Set the value of name
blazorGreeting.setAttribute('name', 'Updated Name');

// Or use direct property access
blazorGreeting.name = 'Updated Name Again';
```

If the parameter is an EventCallback it cannot be used with JavaScript, see more about events below.
```csharp
[Parameter]
public EventCallback<string> OnNameUpdated { get; set; }
```

### Policy suggestions
#### Naming convention
Name components as blazor-{{your-component-name}} to be able to easily find them in the application.
See naming convention below.
```csharp
  builder.RootComponents.RegisterCustomElement<AnotherComponent>("blazor-component");
```
```html
    <blazor-component></blazor-component>
  ```
#### Events
Make sure the Blazor component can be used both as a Blazor component and as a Custom Element.
When used as a Custom Element events are implemented with JSInterop and Javascript in the Web Forms page.

> It would be beneficial for Blazor components to remain self-contained and avoid unnecessary 
> communication with the Web Forms page, as excessive event handling can lead to overly complex 
> and cluttered JavaScript.

Relying on too many events between the Blazor component and the Web Forms page can introduce several
unwanted issues:
  - Increased complexity as each event introduces more interaction points, making the code harder
    to maintain and debug. This can make it hard to to track the flow of data and to understand
    how the different parts of the system interact.
  - It can cause performance overhead.
  - The JavaScript becomes bloated with increased risk of bugs.

The following code initializes the components event listeners in the Web Forms application.
```csharp
    // Reference to a C# object that is passed to JavaScript to invoke 
    // C# methods on the object from JavaScript.
    _dotNetHelper = DotNetObjectReference.Create(BlazorEventHelper);
    try
    {
      // Invokes the setupBlazorEventListeners in the client page.
      await JSRuntime.InvokeVoidAsync("setupBlazorEventListener", _dotNetHelper);
    }
    catch { }
```
C# methods that needs to be called from JavaScript needs to be decorated with the JSInvokable attribute.
```csharp
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
```
In the Web Forms page the listeners are set up.
```js
    function setupBlazorEventListener() {

      // Remove any existing event listeners to avoid duplicates
      // Must prevent memory leask and unexpected behavior from duplicates
      // to keep the app stable.
      document.removeEventListener("blazor-event", blazorEventHandler);
      document.removeEventListener("blazor-error", blazorErrorHandler);

      // Define event handlers
      function blazorEventHandler(e) {
        // Need to specify the .NET assembly where the 
        DotNet.invokeMethodAsync("BlazorApp.Client", "HandleBlazorEvent", e.detail.message);
      }

      function blazorErrorHandler(e) {
        DotNet.invokeMethodAsync("BlazorApp.Client", "HandleBlazorError", e.detail.message);
      }

      document.addEventListener("blazor-event", blazorEventHandler);
      document.addEventListener("blazor-error", blazorErrorHandler);

      // Might be needed or not?
      window.onunload = function () {
        document.removeEventListener("blazor-event", blazorEventHandler);
        document.removeEventListener("blazor-error", blazorErrorHandler);
      }
    }
```

## Setup
- The following nuget package should be referenced from the WASM project.

  ```bash
  dotnet add package Microsoft.AspNetCore.Components.CustomElements
  ```
- Register any Blazor component in the Program.cs file or by using an extension method.
  ```csharp
  builder.RootComponents.RegisterCustomElement<AnotherComponent>("blazor-component");
  ```
  > Custom elements names should use **kebab case**. Prefix them with **blazor**.

  > The following won't work: <span style="color:red">**blazorcomponent**</span>, <span style="color:red">**BLAZOR-COMPONENT**</span>, <span style="color:red">**BlazorComponent**</span>.  
  
  > These will work: <span style="color:darkgreen">**blazor-component**</span>, <span style="color:darkgreen">**blazor-another-component**</span>.

- Publish the Blazor Web assembly project to the root (or another folder) in the Web Forms application.
  > {{repository-path}}\BlazorAppAnywhere\WebApplication\WebApplication

- When the project is published the **wwwroot** folder will contain all the files needed to server the Blazor WebAssembly assets.
  > - wwwroot
  >   - _content
  >   - _framework
  >   - css (and other files needed)

- Reference Blazor WebAssembly in Web Forms. Add the following reference inside the &lt;head&gt; section 
  of, e.g. the Site.Master.
  ```html
  <html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Web Forms with Blazor</title>
        <script src="wwwroot/_framework/blazor.webassembly.js"></script>
    </head>
    <body>
        <form id="form1" runat="server">
            <asp:ContentPlaceHolder ID="MainContent" runat="server">
            </asp:ContentPlaceHolder>
        </form>
    </body>
  </html>

  ```
- On a page in the application use the Blazor WebAssembly component.
  ```html
    <blazor-component></blazor-component>
  ```
- Add mime types for WebAssembly .dat files in the Web.config file for the Web Forms application. 
  ```xml
    <system.webServer>
      <staticContent>
        <mimeMap fileExtension=".dat" mimeType="application/octet-stream" />
      </staticContent>
    </system.webServer>
  ```
- Update the Web.config file to rewrite URLs for the blazor script to be able to load all necessary files.
  The rewriting rules will map **/_framework** to **/wwwroot/_framework** and **/_content** to **/wwwroot/_content**.
  ```html
    <system.webServer>
        <rewrite>
          <rules>
            <rule name="Rewrite Framework Files" stopProcessing="true">
              <match url="^_framework/(.*)" />
              <action type="Rewrite" url="wwwroot/_framework/{R:1}" />
            </rule>
            <rule name="Rewrite content Files" stopProcessing="true">
              <match url="^_content/(.*)" />
              <action type="Rewrite" url="wwwroot/_content/{R:1}" />
            </rule>
          </rules>
        </rewrite>
        <staticContent>
          <mimeMap fileExtension=".dat" mimeType="application/octet-stream" />
        </staticContent>
      </system.webServer>
  ```
- If using a backend API for the Blazor WASM components, ensure that the necessary CORS policys are set correctly in the app setup, Program.cs.
  ```csharp
  builder.Services.AddCors(options =>
  {
    options.AddPolicy("AllowLocalhost",
              builder =>
              {
                builder.AllowAnyOrigin()
                       .AllowAnyHeader()
                       .AllowAnyMethod();
              });
  });

  //-- 

  app.UseCors("AllowLocalhost");
  ```
- The application can now be run.

