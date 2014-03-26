using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using UploadCenter.Classes;

namespace UploadCenter.Database
{
	public partial class UploadedFile
	{
		[NotMapped]
		public string FileSizeName
		{
			get
			{
				if (FileSize<=0)
					return "";
				return UploadedFileManager.GetFileSizeString(FileSize);
			}
		}

		[NotMapped]
		public string DisplayFileExtension
		{
			get
			{
				if (string.IsNullOrWhiteSpace(Extension))
					return "";

				return Extension.Replace(".", "").ToLower();
			}
		}

		[NotMapped]
		public bool VisitorIsOwner { get; set; }

		[NotMapped]
		public string UploaderUsername { get; set; }
	}
}