using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.SessionState;
using Salar.ResumableDownload;
using UploadCenter.Classes;
using UploadCenter.Database;

namespace UploadCenter
{
	/// <summary>
	/// Summary description for ResumableDownload
	/// </summary>
	public class ResumableDownload : IHttpHandler, IRequiresSessionState
	{
		/// <summary>
		/// 2 MB limit
		/// </summary>
		private const int DownloadLimit = 2 * 1024 * 1024;

		public void ProcessRequest(HttpContext context)
		{
			var request = context.Request;
			var response = context.Response;

			// Accepting user request
			var idStr = request.QueryString["id"];
			try
			{
				int id;
				if (!int.TryParse(idStr, out id))
				{
					InvalidRequest(context, "Invalid request!");
					return;
				}
				UploadedFile uploadedFile;
				using (var db = new UploadDb())
				{
					db.Configuration.AutoDetectChangesEnabled = false;
					db.Configuration.ProxyCreationEnabled = false;

					var file = db.Files.FirstOrDefault(a => a.UploadedFileID == id);
					if (file == null)
					{
						InvalidRequest(context, "File does not exists!");
						response.StatusCode = 404;
						return;
					}
					uploadedFile = file;
				}

				//SiteException.LogException(new Exception(
				//	string.Format("UploadedFileID:{0}, IsPublic:{1}, UploadDate:{2}, Filename:{3}",
				//		uploadedFile.UploadedFileID,
				//		uploadedFile.IsPublic,
				//		uploadedFile.UploadDate,
				//		uploadedFile.Filename)));

				if (uploadedFile.IsPublic == false)
				{
					// check the owner
					var user = UserManager.GetUser();
					if (user == null)
					{
						var succeed = UserManager.BasicAuthorize(context);
						if (!succeed)
						{
							return;
						}
						user = UserManager.GetUser();
					}

					// not the file owner!
					if (user == null || user.UserID != uploadedFile.UserId)
					{
						context.Response.Clear();
						context.Response.Write("You do not have access to download this file!");
						context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
						context.Response.Flush();
						context.Response.End();
						return;
					}
				}

				// file path
				var fileName = UploadedFileManager.MapToPhysicalPath(uploadedFile);

				// reading file info
				var fileInfo = new FileInfo(fileName);
				var fileLength = fileInfo.Length;

				// Download information class
				using (var downloadInfo = new DownloadDataInfo(fileName))
				{
					downloadInfo.DisplayFileName = UploadedFileManager.GetUrlFileName(uploadedFile);

					// Reading request download range
					var requestedRanges = HeadersParser.ParseHttpRequestHeaderMultipleRange(context.Request, fileLength);

					// apply the ranges to the download info
					downloadInfo.InitializeRanges(requestedRanges);

					string etagMatched;
					int outcomeStausCode = 200;

					// validating the ranges specified
					if (!HeadersParser.ValidatePartialRequest(context.Request, downloadInfo, out etagMatched, ref outcomeStausCode))
					{
						// the request is invalid, this is the invalid code
						context.Response.StatusCode = outcomeStausCode;

						// show to the client what is the real ETag
						if (!string.IsNullOrEmpty(etagMatched))
							context.Response.AppendHeader("ETag", etagMatched);

						// stop the preoccess
						// but don't hassle with error messages
						return;
					}

					// user ID, or IP or anything you use to identify the user
					var userIP = context.Request.UserHostAddress;

					// limiting the download speed manager and the speed limit
					UserSpeedLimitManager.StartNewDownload(downloadInfo, userIP, DownloadLimit);

					// limiting the download speed manager and the speed limit
					//downloadInfo.LimitTransferSpeed(DownloadLimit);

					// It is very important to destory the DownloadProcess object
					// Here the using block does it for us.
					using (var process = new DownloadProcess(downloadInfo))
					{
						var state = DownloadProcess.DownloadProcessState.None;
						try
						{
							// start the download
							state = process.ProcessDownload(context.Response);
						}
						catch (HttpException)
						{
							// preventing: 
							// System.Web.HttpException (0x800703E3): The remote host closed the connection. The error code is 0x800703E3.
						}

						// checking the state of the download
						if (state == DownloadProcess.DownloadProcessState.PartFinished)
						{
							// all parts of download are finish, do something here!
							using (var db = new UploadDb())
							{
								var dbFile = db.Files.FirstOrDefault(a => a.UploadedFileID == uploadedFile.UploadedFileID);
								if (dbFile != null)
								{
									dbFile.Downloaded++;
									dbFile.LastDownload = DateTime.Now.ToUniversalTime();
									db.SaveChanges();
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				SiteException.LogException(ex, "ID: " + idStr);
				throw;
			}
		}

		public void InvalidRequest(HttpContext context, string message = "Invalid Request!")
		{
			context.Response.Write(message);
			context.Response.StatusCode = 500;
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}