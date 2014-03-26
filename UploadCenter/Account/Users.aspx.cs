using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UploadCenter.Classes;
using UploadCenter.Database;

namespace UploadCenter.Account
{
	public partial class Users : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!UserManager.UserIsAdmin())
			{
				Response.Redirect("Login.aspx");
				return;
			}
			using (var db = new UploadDb())
			{
				var users = db.Users.ToList();
				grdUsers.DataSource = users;
				grdUsers.DataBind();
			}
		}
	}
}