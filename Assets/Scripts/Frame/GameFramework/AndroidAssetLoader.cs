using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 用于加载Android平台下的资源
public class AndroidAssetLoader : FrameComponent
{
	protected static AndroidJavaObject mAssetLoader;
	public AndroidAssetLoader(string name)
		:base(name)
	{
#if !UNITY_EDITOR && UNITY_ANDROID
		mAssetLoader = AndroidPluginManager.getMainActivity().Get<AndroidJavaObject>("mAssetLoader");
#endif
	}
	// 相对于StreamingAssets的路径
	public static byte[] loadAsset(string path)
	{
		byte[] buffer = mAssetLoader.Call<byte[]>("loadAsset", path);
		return buffer;
	}
	public static string loadTxtAsset(string path)
	{
		string str = mAssetLoader.Call<string>("loadTxtAsset", path);
		return str;
	}
	public static bool isAssetExist(string path)
	{
		bool exist = mAssetLoader.Call<bool>("isAssetExist", path);
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
		AndroidJavaObject fileListObject = mAssetLoader.Call<AndroidJavaObject>("startFindAssets", path, pattern, recursive);
		int maxFileCount = 1024;
		for (int i = 0; i < maxFileCount; ++i)
		{
			string fileName = mAssetLoader.Call<string>("nextAsset", fileListObject, i);
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
		byte[] buffer = mAssetLoader.CallStatic<byte[]>("loadFile", path);
		return buffer;
	}
	public static string loadTxtFile(string path)
	{
		checkPersistenDataPath(path);
		string str = mAssetLoader.CallStatic<string>("loadTxtFile", path);
		return str;
	}
	public static new void writeFile(string path, byte[] buffer, int writeCount, bool appendData)
	{
		checkPersistenDataPath(path);
		mAssetLoader.CallStatic("writeFile", path, buffer, writeCount, appendData);
	}
	public static new void writeTxtFile(string path, string str, bool appendData)
	{
		checkPersistenDataPath(path);
		mAssetLoader.CallStatic("writeTxtFile", path, str, appendData);
	}
	public static new bool isDirExist(string path)
	{
		checkPersistenDataPath(path);
		bool exist = mAssetLoader.CallStatic<bool>("isDirExist", path);
		return exist;
	}
	public static new bool isFileExist(string path)
	{
		checkPersistenDataPath(path);
		bool exist = mAssetLoader.CallStatic<bool>("isFileExist", path);
		return exist;
	}
	public static new int getFileSize(string path)
	{
		checkPersistenDataPath(path);
		int size = mAssetLoader.CallStatic<int>("getFileSize", path);
		return size;
	}
	public static new void findFiles(string path, ref List<string> fileList, List<string> patterns, bool recursive)
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
		AndroidJavaObject fileListObject = mAssetLoader.CallStatic<AndroidJavaObject>("startFindFiles", path, pattern, recursive);
		int maxFileCount = 1024;
		for(int i = 0; i < maxFileCount; ++i)
		{
			string fileName = mAssetLoader.CallStatic<string>("nextFile", fileListObject, i);
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
		mAssetLoader.CallStatic("createDirectory", path);
	}
	//------------------------------------------------------------------------------------------------------------------------------------------------
	protected static void checkPersistenDataPath(string path)
	{
		if (!startWith(path, CommonDefine.F_PERSISTENT_DATA_PATH)
			&& !startWith(path + "/", CommonDefine.F_PERSISTENT_DATA_PATH))
		{
			logError("path must start with " + CommonDefine.F_PERSISTENT_DATA_PATH + ", path : " + path);
		}
	}
}
