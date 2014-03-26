using System;
using System.Linq;
using System.Web.Security;
using System.Web.UI;
using UploadCenter.Database;

namespace UploadCenter.Account
{
	public partial class Register : Page
	{

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				using (var db = new UploadDb())
				{
					var isAdminRegister = !db.Users.Any(a => a.IsAdmin);
					lblRegisterAdminHead.Visible = isAdminRegister;
					lblRegisterHead.Visible = !isAdminRegister;
				}
			}
		}

		protected void btnRegister_Click(object sender, EventArgs e)
		{
			var username = txtUsername.Text.Trim();
			var password = txtPassword.Text;
			var email = txtEmail.Text.Trim();
			var isValid = true;

			if (txtPasswordConfirm.Text != password)
			{
				AddError("Password confirm is not correct!");
				isValid = false;
			}

			if (txtEmailConfirm.Text != email)
			{
				AddError("Email confirm is not correct!");
				isValid = false;
			}
			if (username.Length < 2)
			{
				AddError("Username is too short");
				isValid = false;
			}
			if (password.Length < 2)
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
				var hasAdmin = db.Users.Any(a => a.IsAdmin);

				var user = new User()
				{
					Email = email,
					IsAdmin = !hasAdmin,
					Password = password,
					UserName = username
				};

				db.Users.Add(user);
				db.SaveChanges();

				if (!hasAdmin)
				{
					Application.Remove("HasAdmin");
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