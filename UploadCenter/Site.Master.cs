using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UploadCenter
{
	public partial class Site : System.Web.UI.MasterPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack && Application["HasAdmin"] != null)
			{
				var path = "/account/register.aspx";
				if (Request.FilePath.ToLower() != path)
				{
					Response.Redirect(path);
				}
			}
		}
	}
}