using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

public class FileUtility : GameBase
{
	public void init() { }
	// 打开一个二进制文件,fileName为绝对路径
	public static void openFile(string fileName, ref byte[] fileBuffer, ref int fileSize)
	{
		if(!fileName.StartsWith(CommonDefine.F_ASSETS_PATH))
		{
			UnityUtility.logError("fileName should be a absolute path!");
			return;
		}
		FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
		fileSize = (int)fs.Length;
		fileBuffer = new byte[fileSize];
		fs.Read(fileBuffer, 0, fileSize);
		fs.Close();
	}
	// 打开一个文本文件,fileName为绝对路径
	public static void openTxtFile(string fileName, ref string fileBuffer)
	{
		if (!fileName.StartsWith(CommonDefine.F_ASSETS_PATH))
		{
			UnityUtility.logError("fileName should be a absolute path!");
			return;
		}
		StreamReader streamReader = new StreamReader(fileName, Encoding.UTF8);
		fileBuffer = streamReader.ReadToEnd();
		streamReader.Close();
	}
	// 写一个文本文件,fileName为绝对路径,content是写入的字符串
	public static void writeFile(string fileName, string content)
	{
		// 检测路径是否存在,如果不存在就创建一个
		string path = StringUtility.getFilePath(fileName);
		if(!isDirExist(path))
		{
			createDir(path);
		}
		StreamWriter writer = new StreamWriter(fileName, false, Encoding.UTF8);
		writer.Write(content);
		writer.Close();
	}
	public static bool isDirExist(string dir)
	{
		if (!dir.StartsWith(CommonDefine.F_ASSETS_PATH))
		{
			UnityUtility.logError("dir should be a absolute path!");
			return false;
		}
		return Directory.Exists(dir);
	}
	public static bool isFileExist(string fileName)
	{
		if (!fileName.StartsWith(CommonDefine.F_ASSETS_PATH))
		{
			UnityUtility.logError("fileName should be a absolute path!");
			return false;
		}
		return File.Exists(fileName);
	}
	public static void createDir(string dir)
	{
		Directory.CreateDirectory(dir);
	}
	// path为相对于Assets的相对路径
	public static void findFiles(string path, ref List<string> fileList, List<string> pattern)
	{
		path = CommonDefine.F_ASSETS_PATH + path;
		if(!isDirExist(path))
		{
			UnityUtility.logError("path is invalid! path : " + path);
			return;
		}
		DirectoryInfo folder = new DirectoryInfo(path);
		FileInfo[] fileInfoList = folder.GetFiles();
		int fileCount = fileInfoList.Length;
		int patternCount = pattern != null ? pattern.Count : 0;
		for (int i = 0; i < fileCount; ++i)
		{
			string fileName = fileInfoList[i].Name;
			// 如果需要过滤后缀名,则判断后缀
			if (patternCount > 0)
			{
				for (int j = 0; j < patternCount; ++j)
				{
					if (fileName.EndsWith(pattern[j]))
					{
						fileList.Add(fileName);
					}
				}
			}
			// 不需要过滤,则直接放入列表
			else
			{
				fileList.Add(fileName);
			}
		}
	}
	// 得到指定目录下的所有第一级子目录
	// path为相对于Assets的相对路径
	public static bool findDirectory(string path, ref List<string> dirList)
	{
		path = CommonDefine.F_ASSETS_PATH + path;
		if(!isDirExist(path))
		{
			return false;
		}
		string[] ret = Directory.GetDirectories(path);
		int count = ret.Length;
		for (int i = 0; i < count; ++i)
		{
			dirList.Add(StringUtility.getFolderName(ret[i]));
		}
		return true;
	}
}