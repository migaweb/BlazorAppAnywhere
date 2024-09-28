using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication
{
  public partial class _Default : Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      string key = "WebFormsValue";
      string value = "myValue: " + DateTime.Now.Ticks;
      string script = $"setLocalStorage('{key}', '{value}');";

      ClientScript.RegisterStartupScript(this.GetType(), "setLocalStorage", script, true);
    }
  }
}