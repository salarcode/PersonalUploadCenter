using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UploadCenter.Classes;
using UploadCenter.Database;
using WebGrease.Css.Extensions;

namespace UploadCenter.Account
{
	public partial class Default : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			var user = UserManager.GetUser();
			if (user == null)
			{
				Response.Redirect("Login.aspx", true);
				return;
			}
			using (var db = new UploadDb())
			{
				var files = db.Files
					.OrderByDescending(a => a.UploadedFileID)
					.Where(a => a.UserId == user.UserID)
					.ToList();
				ucFiles.Files = files;
				ucFiles.Files.ForEach(a =>
				{
					a.UploaderUsername = user.UserName;
					a.VisitorIsOwner = true;
				});
				ucFiles.DataBind();

				

				if (files.Count == 0)
				{
					boxMessage.Visible = true;
					boxCount.Visible = false;
				}
				else
				{
					var totalSize = files.Sum(a => a.FileSize);
					lblFilesCount.Text = UploadedFileManager.GetFileSizeString(totalSize) + " for " + files.Count + " file(s).";
				}
			}
		}
	}
}