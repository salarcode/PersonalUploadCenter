using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UploadCenter.Classes;
using UploadCenter.Database;
using WebGrease.Css.Extensions;

namespace UploadCenter
{
	public partial class Search : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			var query = (Request.QueryString["q"]??"").Trim();
			if (query.Length == 0)
			{
				Response.Redirect("/", true);
				return;
			}
			query = query.ToLower();

			using (var db = new UploadDb())
			{
				const int itemsNo = 5;
				var user = UserManager.GetUser();
				var userId = UserManager.GetUserId() ?? -1;
				bool isAdmin = user != null && user.IsAdmin;

				ucFiles.Files = db.Files.Include("User")
					.OrderByDescending(a => a.UploadedFileID)
					.Where(a => a.Filename.ToLower().Contains(query))
					.Where(a => a.IsPublic || a.UserId == userId)
					.ToList();

				if (ucFiles.Files.Count > 0)
				{
					ucFiles.Files.ForEach(a =>
					{
						a.UploaderUsername = (a.User != null) ? a.User.UserName : "";
						a.VisitorIsOwner = isAdmin || a.UserId == userId;
					});
					ucFiles.DataBind();
				}
				else
				{
					AddError("Nothing found!");
				}
			}
		}

		void AddError(string msg)
		{
			vldErrorsBox.Visible = true;
			vldErrors.Text += msg + "<br/>";
		}

	}
}