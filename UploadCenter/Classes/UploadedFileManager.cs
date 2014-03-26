using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using UploadCenter.Database;

namespace UploadCenter.Classes
{
	public class UploadedFileManager
	{
		private const string fileFormat = "{0}-{1}";
		private const string filesPath = "~/Files/{0}";
		static string RemoveBadCharacters(string fileName)
		{
			return fileName.Replace(" ", "_");
		}

		public static string GetUrlFileName(UploadedFile uploadedFile)
		{
			if (uploadedFile == null)
				return "";
			return RemoveBadCharacters(uploadedFile.Filename);
		}

		public static string GetFileSizeString(long sizeInBytes)
		{
			if (sizeInBytes < 1000)
				return sizeInBytes.ToString() + " Bytes";

			double size = sizeInBytes / 1024.00;
			if (size < 1000)
			{
				return size.ToString("0.0") + " KB";
			}
			else if (size > 1000 && size < 1000000)
			{
				return (size/1024.00).ToString("0.0") + " MB";
			}
			else if (size > 1000000)
			{
				return (size/1048576.00).ToString("0.0") + " GB";
			}
			return size.ToString("0.0");

		}

		public static string MapToPhysicalPath(string fileName)
		{
			var context = HttpContext.Current;
			fileName = RemoveBadCharacters(fileName);
			fileName = string.Format(filesPath, fileName);

			var path = context.Server.MapPath(fileName);
			var pathDir = Path.GetDirectoryName(path);
			if (pathDir != null && !Directory.Exists(pathDir))
			{
				Directory.CreateDirectory(pathDir);
			}
			return path;
		}

		public static string MapToPhysicalPath(UploadedFile file)
		{
			var context = HttpContext.Current;
			var fileName = RemoveBadCharacters(file.Filename);
			fileName = string.Format(fileFormat, file.UploadedFileID, fileName);
			fileName = string.Format(filesPath, fileName);

			var path = context.Server.MapPath(fileName);
			var pathDir = Path.GetDirectoryName(path);
			if (pathDir != null && !Directory.Exists(pathDir))
			{
				Directory.CreateDirectory(pathDir);
			}
			return path;
		}

		//public static string MapToVirtualPath(UploadedFile file)
		//{
		//	var context = HttpContext.Current;
		//	var fileName = RemoveBadCharacters(file.Filename);
		//	fileName = string.Format(fileFormat, file.UploadedFileID, fileName);
		//	fileName = string.Format(filesPath, fileName);

		//	return fileName;
		//}

		
	}
}