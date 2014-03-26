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
	public partial class Default : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			using (var db = new UploadDb())
			{
				const int itemsNo = 5;
				var userId = UserManager.GetUserId() ?? -1;
				var user = UserManager.GetUser();
				bool isAdmin = user != null && user.IsAdmin;

				ucFiles.Files = db.Files.Include("User")
					.OrderByDescending(a => a.UploadedFileID)
					.Take(itemsNo)
					.Where(a => a.IsPublic).ToList();
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
					boxLatestFiles.Visible = false;
				}
			}
		}
	}
}