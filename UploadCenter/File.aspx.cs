using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UploadCenter.Classes;
using UploadCenter.Database;

namespace UploadCenter
{
	public partial class File : System.Web.UI.Page
	{

		public UploadedFile UploadedFile { get; set; }


		protected void Page_Load(object sender, EventArgs e)
		{
			var idStr = Request.QueryString["id"];
			int id;
			if (!int.TryParse(idStr, out id))
			{
				Response.Redirect("/");
				return;
			}
			using (var db = new UploadDb())
			{
				var file = db.Files.Include("User").FirstOrDefault(a => a.UploadedFileID == id);
				if (file == null)
				{
					Response.Write("File not found!");
					Response.StatusCode = 404;
					Response.End();
					return;
				}
				var user = UserManager.GetUser();
				if (!file.IsPublic)
				{
					if (user == null || file.UserId != user.UserID)
					{
						Response.Redirect("/", true);
						return;
					}
				}

				bool visitorIsOwner = user != null && user.UserID == file.UserId;

				var uploaderUsername = "";
				if (file.User != null)
				{
					uploaderUsername = file.User.UserName;
				}
				UploadedFile = file;


				file.VisitorIsOwner = visitorIsOwner;
				file.UploaderUsername = uploaderUsername;
				ucFiles.Files = new List<UploadedFile>() { file };
				ucFiles.DataBind();

			}
		}

		//protected void btnDelete_Click(object sender, EventArgs e)
		//{
		//	using (var db = new UploadDb())
		//	{
		//		var file = db.Files.FirstOrDefault(a => a.UploadedFileID == UploadedFile.UploadedFileID);
		//		if (file == null)
		//		{
		//			Response.Write("File not found!");
		//			Response.StatusCode = 404;
		//			return;
		//		}
		//		var user = UserManager.GetUser();
		//		if (file.UserId == null)
		//		{
		//			Response.Write("You do not have the permission to delete!");
		//			Response.StatusCode = 500;
		//			return;
		//		}
		//		else
		//		{
		//			if (user == null || user.UserID != file.UserId)
		//			{
		//				Response.Write("You do not have the permission to delete!");
		//				Response.StatusCode = 500;
		//				return;
		//			}
		//		}

		//		var fileAddress = UploadedFileManager.MapToPhysicalPath(file);
		//		try
		//		{
		//			System.IO.File.Delete(fileAddress);

		//			db.Files.Remove(file);
		//			db.SaveChanges();
		//		}
		//		catch (Exception)
		//		{

		//		}
		//	}
		//	Response.Redirect("/");
		//}
	}
}