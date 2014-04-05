using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UploadCenter.Classes
{
	public class Config
	{

		public bool AllowRegister { get; set; }
		public bool AllowRemoteUpload { get; set; }
		public bool AllowPcUpload { get; set; }
		public long? MaxFileSizeBytes{ get; set; }
		
		/// <summary>
		/// ".jpg;.rar;.zip;"
		/// </summary>
		public string AllowedExtentions { get; set; }

		/// <summary>
		/// In minutes
		/// </summary>
		public int? GuestFileDeleteTimeout { get; set; }

		/// <summary>
		/// In minutes
		/// </summary>
		public int? MemberFileDeleteTimeout { get; set; }

		public long? GuestDownloadSpeedLimit { get; set; }

		public long? MemberDownloadSpeedLimit { get; set; }



	}
}