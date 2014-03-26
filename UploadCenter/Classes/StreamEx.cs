using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace UploadCenter.Classes
{
	public class StreamEx
	{
		public static long CopyBufferedStream(Stream src, Stream dest, bool flushToDestination = false, bool seekToBeginning = true)
		{
			if (src == null || dest == null)
				return -1;

			if (src.CanSeek && src.Length == 0)
				return -1;

			if (!dest.CanWrite)
				return -1;

			// seek to the beginning if possible
			if (seekToBeginning && src.CanSeek)
				src.Seek(0, SeekOrigin.Begin);

			// Declare variables
			const int maxBlockReadFromWeb = 1024;
			int readed = -1;
			var buffer = new byte[maxBlockReadFromWeb];
			long readLen = 0;

			// Read the stream and write it into memory
			while ((int)(readed = src.Read(buffer, 0, maxBlockReadFromWeb)) > 0)
			{
				dest.Write(buffer, 0, readed);
				readLen += readed;

				if (flushToDestination)
				{
					dest.Flush();
				}
			}
			return readLen;
		}

		public static long CopyFromNetStream(Stream src, Stream dest, bool flushToDestination = false, bool seekToBeginning = true)
		{
			if (src == null || dest == null)
				return -1;

			if (src.CanSeek && src.Length == 0)
				return -1;

			if (!dest.CanWrite)
				return -1;

			// seek to the beginning if possible
			if (seekToBeginning && src.CanSeek)
				src.Seek(0, SeekOrigin.Begin);

			// Declare variables
			const int maxBlockReadFromWeb = 8192;
			int readed = -1;
			var buffer = new byte[maxBlockReadFromWeb];
			long readLen = 0;

			// Read the stream and write it into memory
			while ((int)(readed = src.Read(buffer, 0, maxBlockReadFromWeb)) > 0)
			{
				dest.Write(buffer, 0, readed);
				readLen += readed;

				if (flushToDestination)
				{
					dest.Flush();
				}
			}
			return readLen;
		}
	}
}