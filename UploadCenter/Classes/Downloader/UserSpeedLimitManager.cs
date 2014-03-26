using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Salar.ResumableDownload
{
	/// <summary>
	/// Managing user downloads speed limit
	/// </summary>
	/// <authors>
	/// Salar Khalilzadeh
	/// http://www.salarcode.com
	/// http://blog.salarcode.com
	/// https://salarblogsources.svn.codeplex.com/svn/Projects/Salar.ResumableDownload/
	/// </authors>
	public static class UserSpeedLimitManager
	{
		private class DownloadInfo
		{
			public string UserIP;
			public int SpeedLimit;
			public DownloadDataInfo DataInfo;
		}

		private static volatile List<DownloadInfo> _userDownloadInfo = new List<DownloadInfo>();

		/// <summary>
		/// Adding a new download to the list!
		/// </summary>
		public static void StartNewDownload(DownloadDataInfo dataInfo, string userIP, int userSpeedLimit = 0)
		{
			if (userIP == null || dataInfo == null)
				throw new ArgumentNullException();

			dataInfo.UserId = userIP;
			dataInfo.Finished += DataInfoFinished;
			lock (_userDownloadInfo)
			{
				_userDownloadInfo.Add(new DownloadInfo { UserIP = userIP, DataInfo = dataInfo, SpeedLimit = userSpeedLimit });
			}

			// changing all the downloads speed beglogs to this user
			ApplySpeedLimit(userIP, userSpeedLimit);
		}

		static void DataInfoFinished(DownloadDataInfo dataInfo)
		{
			if (dataInfo != null)
			{
				var userIP = dataInfo.UserId;
				var guid = dataInfo.DataId;
				RemoveDownloadInfoByDataId(guid);
				RefreshUserLimitState(userIP);
			}
		}

		/// <summary>
		/// Limiting all downloads which belogs to a user
		/// </summary>
		public static void ApplySpeedLimit(string userIP, int bytesPerSecond)
		{
			if (bytesPerSecond > 0)
			{
				var liveDownsCount = _userDownloadInfo.Count(x => x.UserIP == userIP);
				if (liveDownsCount == 0)
					return;

				var speadedBytes = (bytesPerSecond + 1) / liveDownsCount;

				// millisecods, this should help spreading speed equally through time
				var spreadedSleep = 1000 / (liveDownsCount * 2);

				var downInfo = _userDownloadInfo.Where(x => x.UserIP == userIP).ToList();
				for (int i = downInfo.Count - 1; i >= 0; i--)
				{
					var x = downInfo[i];
					Thread.Sleep(spreadedSleep);
					x.SpeedLimit = bytesPerSecond;
					x.DataInfo.LimitTransferSpeed(speadedBytes);
				}
			}
			else
			{
				var downInfo = _userDownloadInfo.Where(x => x.UserIP == userIP).ToList();
				for (int i = downInfo.Count - 1; i >= 0; i--)
				{
					var x = downInfo[i];
					x.SpeedLimit = 0;
					x.DataInfo.LimitTransferSpeed(0);
				}
			}
		}

		/// <summary>
		/// Recalculate user speed limit for all active downloads
		/// </summary>
		public static void RefreshUserLimitState(string userIP)
		{
			if (userIP == null)
				return;

			var downInfo = _userDownloadInfo.FirstOrDefault(x => x.UserIP == userIP);
			if (downInfo != null)
			{
				var bytesPerSecond = downInfo.SpeedLimit;

				// reapply the speed to all
				ApplySpeedLimit(userIP, bytesPerSecond);
			}
		}

		private static void RemoveDownloadInfoByDataId(Guid dataId)
		{
			lock (_userDownloadInfo)
			{
				for (int i = _userDownloadInfo.Count - 1; i >= 0; i--)
				{
					var t = _userDownloadInfo[i];
					if (t.DataInfo.DataId == dataId)
					{
						_userDownloadInfo.RemoveAt(i);
					}
				}
			}
		}

	}
}
