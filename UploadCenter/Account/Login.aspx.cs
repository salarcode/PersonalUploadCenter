using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using UploadCenter.Database;

namespace UploadCenter.Account
{
	public partial class Login : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void btnLogin_Click(object sender, EventArgs e)
		{
			var username = txtUsername.Text.Trim();
			var password = txtPassword.Text;
			var isValid = true;

			if (username.Length ==0)
			{
				AddError("Username is too short");
				isValid = false;
			}
			if (password.Length == 0)
			{
				AddError("Password is too short");
				isValid = false;
			}

			if (!isValid)
			{
				return;
			}


			using (var db = new UploadDb())
			{
				var user = db.Users.FirstOrDefault(a => a.UserName == username && a.Password == password);

				if (user == null)
				{
					AddError("Username or password is invalid.");
					return;
				}

				FormsAuthentication.SetAuthCookie(user.UserID.ToString(), true);
				Response.Redirect("/account/");
			}
		}

		void AddError(string msg)
		{
			vldErrorsBox.Visible = true;
			vldErrors.Text += msg + "<br/>";
		}
	}
}