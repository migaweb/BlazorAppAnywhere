<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

  <div id="app">
    <my-greeting name="Some name" />
  </div>

  <div style="border:solid 1px red;margin-top:10px; padding: 10px;">
    <h3>Web Forms</h3>
    <div>
      <b>Event received in Web Forms:</b> <span id="message"></span>
    </div>
    <button type="button" onclick="triggerBlazorEvent()">Trigger Blazor Event</button>
    <br />
    <input id="text-message" type="text" value="" placeholder="Enter text to send to Blazor" />
    <button type="button" onclick="triggerBlazorError()">Send text</button>
  </div>
  <script type="text/javascript">
    function setLocalStorage(key, value) {
      localStorage.setItem(key, value);
    }

    function dispatchCustomBlazorEvent(eventName, eventData) {
      const event = new CustomEvent(eventName, { detail: eventData });
      document.dispatchEvent(event);
    }

    function setupBlazorEventListener() {
      document.addEventListener("blazor-event", function (e) {
        // Call the Blazor component method with the event data
        DotNet.invokeMethodAsync("BlazorApp.Client", "HandleBlazorEvent", e.detail.message);
      });
      document.addEventListener("blazor-error", function (e) {
        // Call the Blazor component method with the event data
        DotNet.invokeMethodAsync("BlazorApp.Client", "HandleBlazorError", e.detail.message);
      });
    }

    function triggerBlazorEvent() {
      const eventData = { message: "Data from Web Forms" }; // Example data to send
      const event = new CustomEvent("blazor-event", { detail: eventData });
      document.dispatchEvent(event); // Dispatch the event
    }

    function triggerBlazorError() {
      const eventData = { message: document.getElementById('text-message').value }; // Example data to send
      const event = new CustomEvent("blazor-error", { detail: eventData });
      document.dispatchEvent(event); // Dispatch the event
    }

    // Handle the event (e.detail will have the data sent from Blazor)
    document.addEventListener('custom-event', function (e) {
      document.getElementById('message').innerText = e.detail;
      //alert("Event received in Web Forms: " + e.detail);
    });
  </script>
</asp:Content>
