using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UploadCenter.Classes;
using UploadCenter.Database;

namespace UploadCenter
{
	public partial class FromPc : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected override void OnError(EventArgs e)
		{
			base.OnError(e);
		}

		protected void btnFromPcUpload_Click(object sender, EventArgs e)
		{
			try
			{
				if (!filePcUpload.HasFile)
				{
					AddError("No file is selected!");
					return;
				}
				using (var db = new UploadDb())
				{
					var fileName = Path.GetFileName(filePcUpload.PostedFile.FileName);
					var extension = Path.GetExtension(fileName);
					var sizeInBytes = filePcUpload.PostedFile.ContentLength;
					var user = UserManager.GetUser();
					var isPublic = txtVisibility.Value == "1";

					var newName = txtNewName.Text.Trim();
					if (newName.Length > 0)
					{
						fileName = newName;
					}

					var newFile = new UploadedFile
					{
						Comment = txtRemoteComment.Text,
						Extension = extension,
						Filename = fileName,
						Downloaded = 0,
						FileSize = sizeInBytes,
						LastDownload = null,
						UploadDate = DateTime.Now.ToUniversalTime(),
						UserId = (user != null) ? user.UserID : (int?)null,
						UploadedFileID = 0,
						IsPublic = isPublic
					};

					try
					{
						db.Files.Add(newFile);
						db.SaveChanges();

						var filePath = UploadedFileManager.MapToPhysicalPath(newFile);
						filePcUpload.PostedFile.SaveAs(filePath);

						Response.Redirect("file.aspx?id=" + newFile.UploadedFileID);
					}
					catch (ThreadAbortException ex)
					{
					}
					catch (Exception ex)
					{
						if (newFile.UploadedFileID > 0)
						{
							db.Files.Remove(newFile);

							db.SaveChanges();
						}

						AddError("An unhandled error occured.");
						AddError(ex.Message);
					}
				}
			}
			catch (Exception ex)
			{
				AddError(ex.Message);
			}
		}

		void AddError(string msg)
		{
			vldErrorsBox.Visible = true;
			vldErrors.Text += msg + "<br/>";
		}
	}
}