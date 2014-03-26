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
	public partial class FilesDetail : System.Web.UI.UserControl
	{
		public FilesDetail()
		{
			RedirectAfterDeleteLocation = "/";
		}
		public IList<UploadedFile> Files { get; set; }
		public bool RedirectAfterDelete { get; set; }
		public string RedirectAfterDeleteLocation { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{
			ReloadFilesList();
		}

		void ReloadFilesList()
		{
			rptRepeat.DataSource = Files;
			rptRepeat.DataBind();
		}

		protected void rptRepeat_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			if (e.CommandName == "Delete")
			{
				var idString = e.CommandArgument.ToString();
				int id;
				if (!int.TryParse(idString, out id))
					return;

				using (var db = new UploadDb())
				{
					var file = db.Files.FirstOrDefault(a => a.UploadedFileID == id);
					if (file == null)
					{
						Response.Write("File not found!");
						Response.StatusCode = 404;
						return;
					}
					var user = UserManager.GetUser();
					if (user == null || (user.UserID != file.UserId && !user.IsAdmin))
					{
						Response.Write("You do not have the permission to delete!");
						Response.StatusCode = 500;
						return;
					}
					if (file.UserId == null && !user.IsAdmin)
					{
						Response.Write("You do not have the permission to delete!");
						Response.StatusCode = 500;
						return;
					}

					var fileAddress = UploadedFileManager.MapToPhysicalPath(file);
					try
					{
						System.IO.File.Delete(fileAddress);

						db.Files.Remove(file);
						db.SaveChanges();
						if (RedirectAfterDelete)
						{
							Response.Redirect(RedirectAfterDeleteLocation);
						}
						else
						{
							var notDisplayFile = Files.FirstOrDefault(a => a.UploadedFileID == file.UploadedFileID);
							if (notDisplayFile != null)
								Files.Remove(notDisplayFile);

							ReloadFilesList();
						}
					}
					catch (Exception ex)
					{
						AddError(ex.Message);
					}
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