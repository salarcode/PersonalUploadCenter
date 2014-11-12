using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using UploadCenter.Classes;
using UploadCenter.Database;

namespace UploadCenter
{
	public partial class Remote : System.Web.UI.Page
	{
		private const string BrowserUserAgent = " Mozilla/5.0 (Windows NT 6.3; WOW64; rv:30.0) Gecko/20100101 Firefox/30.0";
		public class RemoteCookie
		{
			public string Name { get; set; }
			public string Content { get; set; }
			public string Path { get; set; }
		}


		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void btnRemoteUpload_Click(object sender, EventArgs e)
		{
			try
			{
				var url = txtRemoteUrl.Text.Trim();
				Uri uri;
				if (string.IsNullOrWhiteSpace(url) || !Uri.TryCreate(url, UriKind.Absolute, out uri))
				{
					AddError("Please enter a valid Url!");
					return;
				}

				var fileName = Path.GetFileName(url);
				var newName = txtNewName.Text.Trim();
				if (newName.Length > 0)
				{
					fileName = newName;
				}

				var referer = txtReferrer.Text.Trim();

				// temporary file name
				var fileTempAddress = UploadedFileManager.MapToPhysicalPath(fileName + Guid.NewGuid().ToString());
				long sizeInBytes = 0;

				var cookies = Deserialize(txtCookies.Value);
				try
				{
					using (var file = new FileStream(fileTempAddress, FileMode.Create))
					{
						Download(uri, referer, cookies, file);
						sizeInBytes = file.Length;
					}
				}
				catch (Exception)
				{
					System.IO.File.Delete(fileTempAddress);
					throw;
				}

				// done!
				using (var db = new UploadDb())
				{
					var extension = Path.GetExtension(fileName);
					var user = UserManager.GetUser();
					var isPublic = txtVisibility.Value == "1";

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
						System.IO.File.Move(fileTempAddress, filePath);

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
						try
						{
							System.IO.File.Delete(fileTempAddress);
						}
						catch { }

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

		MemoryStream Download(Uri url, string referer, IList<RemoteCookie> cookies)
		{
			var wr = WebRequest.Create(url);
			var hwr = wr as HttpWebRequest;
			if (hwr != null)
			{
				hwr.UserAgent = BrowserUserAgent;

				if (!string.IsNullOrWhiteSpace(referer))
					hwr.Referer = referer;

				if (cookies != null && cookies.Count > 0)
				{
					hwr.CookieContainer = new CookieContainer();
					foreach (var remoteCookie in cookies)
					{
						try
						{
							var c = new Cookie(remoteCookie.Name, remoteCookie.Content, remoteCookie.Path);
							hwr.CookieContainer.Add(url, c);
						}
						catch { }
					}
				}
			}

			var resData = new MemoryStream();
			try
			{
				var res = wr.GetResponse();
				using (var resNetData = res.GetResponseStream())
				{
					StreamEx.CopyFromNetStream(resNetData, resData, false, true);
				}

				return resData;
			}
			catch (Exception)
			{
				resData.Dispose();
				throw;
			}
		}

		void Download(Uri url, string referer, IList<RemoteCookie> cookies, Stream destination)
		{
			var wr = WebRequest.Create(url);
			var hwr = wr as HttpWebRequest;
			if (hwr != null)
			{
				hwr.UserAgent = BrowserUserAgent;

				if (!string.IsNullOrWhiteSpace(referer))
					hwr.Referer = referer;

				if (cookies != null && cookies.Count > 0)
				{
					hwr.CookieContainer = new CookieContainer();
					foreach (var remoteCookie in cookies)
					{
						try
						{
							var c = new Cookie(remoteCookie.Name, remoteCookie.Content, remoteCookie.Path);
							hwr.CookieContainer.Add(url, c);
						}
						catch { }
					}
				}
			}

			try
			{
				var res = wr.GetResponse();
				using (var resNetData = res.GetResponseStream())
				{
					StreamEx.CopyFromNetStream(resNetData, destination, true, true);
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		void AddError(string msg)
		{
			vldErrorsBox.Visible = true;
			vldErrors.Text += msg + "<br/>";
		}

		private IList<RemoteCookie> Deserialize(string objString)
		{
			if (string.IsNullOrWhiteSpace(objString))
				return null;
			var js = new JsonSerializer();
			using (var r = new StringReader(objString))
			using (var jr = new JsonTextReader(r))
			{
				return js.Deserialize<IList<RemoteCookie>>(jr);
			}
		}

		string Serialize(IList<RemoteCookie> obj)
		{
			if (obj == null || obj.Count == 0)
				return "";
			var js = new JsonSerializer();
			using (var w = new StringWriter())
			{
				js.Serialize(w, obj);
				return w.ToString();
			}
		}

	}
}