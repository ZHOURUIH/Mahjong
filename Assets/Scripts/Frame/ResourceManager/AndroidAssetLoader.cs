using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AndroidAssetLoader : FrameComponent
{
	protected static AndroidJavaClass mUnityPlayer;
	protected static AndroidJavaObject mCurrentActivity;
	public AndroidAssetLoader(string name)
		:base(name)
	{
		mUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		mCurrentActivity = mUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
	}
	public override void destroy()
	{
		mUnityPlayer.Dispose();
		mCurrentActivity.Dispose();
		base.destroy();
	}
	// 相对于StreamingAssets的路径
	public static byte[] loadAsset(string path)
	{
		byte[] buffer = mCurrentActivity.Call<byte[]>("loadAsset", path);
		return buffer;
	}
	public static string loadTxtAsset(string path)
	{
		string str = mCurrentActivity.Call<string>("loadTxtAsset", path);
		return str;
	}
	public static bool isAssetExist(string path)
	{
		bool exist = mCurrentActivity.Call<bool>("isAssetExist", path);
		return exist;
	}
	public static void findAssets(string path, ref List<string> fileList, List<string> patterns, bool recursive)
	{
		string pattern = "";
		int patternCount = patterns != null ? patterns.Count : 0;
		for (int i = 0; i < patternCount; ++i)
		{
			pattern += patterns[i];
			if (i != patternCount - 1)
			{
				pattern += " ";
			}
		}
		AndroidJavaObject fileListObject = mCurrentActivity.Call<AndroidJavaObject>("startFindAssets", path, pattern, recursive);
		int maxFileCount = 1024;
		for (int i = 0; i < maxFileCount; ++i)
		{
			string fileName = mCurrentActivity.Call<string>("nextAsset", fileListObject, i);
			if (fileName == "")
			{
				break;
			}
			fileList.Add(fileName);
		}
	}
	//-------------------------------------------------------------------------------------------------------------------------------------------
	// 以下函数只能用于Android平台的persistentDataPath目录操作,path为绝对路径
	public static byte[] loadFile(string path)
	{
		checkPersistenDataPath(path);
		byte[] buffer = mCurrentActivity.Call<byte[]>("loadFile", path);
		return buffer;
	}
	public static string loadTxtFile(string path)
	{
		checkPersistenDataPath(path);
		string str = mCurrentActivity.Call<string>("loadTxtFile", path);
		return str;
	}
	public static void writeFile(string path, byte[] buffer, int writeCount, bool appendData)
	{
		checkPersistenDataPath(path);
		mCurrentActivity.Call("writeFile", path, buffer, writeCount, appendData);
	}
	public static void writeTxtFile(string path, string str, bool appendData)
	{
		checkPersistenDataPath(path);
		mCurrentActivity.Call("writeTxtFile", path, str, appendData);
	}
	public static bool isDirExist(string path)
	{
		checkPersistenDataPath(path);
		bool exist = mCurrentActivity.Call<bool>("isDirExist", path);
		return exist;
	}
	public static bool isFileExist(string path)
	{
		checkPersistenDataPath(path);
		bool exist = mCurrentActivity.Call<bool>("isFileExist", path);
		return exist;
	}
	public static int getFileSize(string path)
	{
		checkPersistenDataPath(path);
		int size = mCurrentActivity.Call<int>("getFileSize", path);
		return size;
	}
	public static void findFiles(string path, ref List<string> fileList, List<string> patterns, bool recursive)
	{
		checkPersistenDataPath(path);
		string pattern = "";
		int patternCount = patterns != null ? patterns.Count : 0;
		for(int i = 0; i < patternCount; ++i)
		{
			pattern += patterns[i];
			if(i != patternCount - 1)
			{
				pattern += " ";
			}
		}
		AndroidJavaObject fileListObject = mCurrentActivity.Call<AndroidJavaObject>("startFindFiles", path, pattern, recursive);
		int maxFileCount = 1024;
		for(int i = 0; i < maxFileCount; ++i)
		{
			string fileName = mCurrentActivity.Call<string>("nextFile", fileListObject, i);
			if(fileName == "")
			{
				break;
			}
			fileList.Add(CommonDefine.F_STREAMING_ASSETS_PATH + fileName);
		}
	}
	public static void createDirectory(string path)
	{
		checkPersistenDataPath(path);
		mCurrentActivity.Call("createDirectory", path);
	}
	//------------------------------------------------------------------------------------------------------------------------------------------------
	protected static void checkPersistenDataPath(string path)
	{
		if (!StringUtility.startWith(path, Application.persistentDataPath))
		{
			logError("path must start with Application.persistentDataPath");
		}
	}
}
