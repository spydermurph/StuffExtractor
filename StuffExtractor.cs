using System;
using System.IO;

namespace StuffExtractor
{
	/// <summary>
	/// Summary description for FileWorker.
	/// </summary>
	public class FileWorker
	{
		private static BinaryReader oReader = null;
		private static BufferedStream oStream = null;
		private static int numFiles = 0;
		private static int [] fileSize = null;
		private static int [] filePathSize = null;
		private static string [] filePath = null;
		
		public BinaryReader FS
		{
			get
			{
				return oReader;
			}
			set
			{
				oReader = value;
			}
		}

		public FileWorker()
		{
		}

		public void LoadFile(string filePath)
		{
			//Stream inputStream = File.OpenRead(@"C:\Program Files\CCP\eve\resMusic.stuff");
			Stream inputStream = File.OpenRead(filePath);
			oStream = new BufferedStream(inputStream);
			oReader = new BinaryReader(oStream);
			LoadFileInfo();
		}

		public char[] byteToChar(byte[] tmp)
		{
			char[] ret = new char[tmp.Length];
			for(int i = 0; i < tmp.Length; i++)
			{
				ret[i] = (char)tmp[i];
			}
			return ret;
		}


		public void LoadFileInfo()
		{
			numFiles = oReader.ReadInt32();
			Console.WriteLine(numFiles);
			fileSize = new int[numFiles];
			filePathSize = new int[numFiles];
			filePath = new string[numFiles];
			for(int i = 0; i < numFiles; i++)
			{
				fileSize[i] = oReader.ReadInt32();
				filePathSize[i] = oReader.ReadInt32();
				filePath[i] = new string(byteToChar(oReader.ReadBytes(filePathSize[i])));
				oReader.ReadByte();
				Console.WriteLine(fileSize[i]+":"+filePathSize[i]+":"+filePath[i]);
			}
		}

		public void ExtractFile()
		{
			for(int i = 0; i < numFiles; i++)
			{
				Console.WriteLine("Extracting file: " + filePath[i]);
				SaveFile(oReader.ReadBytes(fileSize[i]), filePath[i]);
			}
		}

		public void SaveFile(byte[] file, string filePath)
		{
			string fileName = filePath.Substring(filePath.LastIndexOf("\\") + 1);
			string fileDir = filePath.Substring(0, filePath.LastIndexOf("\\") + 1);
			if(!Directory.Exists(fileDir))
			{
				Directory.CreateDirectory(fileDir);
			}
			if(File.Exists(filePath))
			{
				File.Delete(filePath);
			}
			Stream outputStream = File.Create(filePath);
			BufferedStream oStream = new BufferedStream(outputStream);
			BinaryWriter oWriter = new BinaryWriter(oStream);
			oWriter.Write(file);
			oWriter.Flush();
			oWriter.Close();
		}

		public void ListFiles()
		{
			Console.WriteLine("Number of files : " + numFiles);
			Console.WriteLine("file size:file path size:file path");
			for(int i = 0; i < numFiles; i++)
			{
				Console.WriteLine(fileSize[i]+":"+filePathSize[i]+":"+filePath[i]);
			}
		}

		public static void Main(string[] args)
		{
			if(args.Length == 0 || args.Length > 2)
			{
				Console.WriteLine("Usage: StuffExtractor file");
				Console.WriteLine("   or: StuffExtractor -p file");
				return;
			}
			FileWorker oWorker = new FileWorker();
			if(args[0].Equals("-p"))
			{
				oWorker.LoadFile(args[1]);
				oWorker.ListFiles();
			}
			else
			{
				oWorker.LoadFile(args[0]);
				oWorker.ExtractFile();
				Console.ReadLine();
				oReader.Close();
			}
		}
	}
}
