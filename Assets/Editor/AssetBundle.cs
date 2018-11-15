using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEditor;

class AssetBuildBundleInfo
{
	public string assetName;            // 带Resources下相对路径,带后缀
	public string bundleName;           // 所属AssetBundle
	public List<string> dependencies;   // 所有依赖的AssetBundle
	public void AddDependence(string dep)
	{
		if (dependencies == null)
		{
			dependencies = new List<string>();
		}
		dependencies.Add(dep);
	}
}

public class AssetBundlePack : GameBase
{
	protected const string mAssetMenuRoot = "AssetBundle/";
	// key为AssetBundle名,带Resources下相对路径,带后缀,Value是该AssetBundle中包含的所有Asset
	private static Dictionary<string, List<AssetBuildBundleInfo>> mAssetBundleMap = new Dictionary<string, List<AssetBuildBundleInfo>>();
	[MenuItem(mAssetMenuRoot + "pack/Android")]
	public static void packAssetBundleAndroid()
	{
		packAssetBundle(BuildTarget.Android);
	}
	[MenuItem(mAssetMenuRoot + "pack/Windows")]
	public static void packAssetBundleWindows()
	{
		packAssetBundle(BuildTarget.StandaloneWindows);
	}
	[MenuItem(mAssetMenuRoot + "pack/IOS")]
	public static void packAssetBundleiOS()
	{
		packAssetBundle(BuildTarget.iOS);
	}
	[MenuItem(mAssetMenuRoot + "pack/Lunix")]
	public static void packAssetBundleLinux()
	{
		packAssetBundle(BuildTarget.StandaloneLinux);
	}
	// subPath为以Asset开头的相对路径
	public static void packAssetBundle(BuildTarget target)
	{
		DateTime time0 = DateTime.Now;
		// 清理输出目录
		createOrClearOutPath();
		// 清理之前设置过的bundleName
		clearAssetBundleName();
		// 设置bunderName
		mAssetBundleMap.Clear();
		List<string> resList = new List<string>();
		getAllSubResDirs(CommonDefine.P_RESOURCE_PATH, resList);
		foreach (string dir in resList)
		{
			setAssetBundleName(dir);
		}
		// 打包
		BuildPipeline.BuildAssetBundles(CommonDefine.P_STREAMING_ASSETS_PATH, BuildAssetBundleOptions.ChunkBasedCompression, target);
		AssetDatabase.Refresh();

		// 构建依赖关系
		AssetBundle assetBundle = AssetBundle.LoadFromFile(CommonDefine.F_STREAMING_ASSETS_PATH + "StreamingAssets");
		AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
		string[] assetBundleNameList = manifest.GetAllAssetBundles();
		// 遍历所有AB
		foreach (string bundle in assetBundleNameList)
		{
			string bundleName = bundle;
			string[] deps = manifest.GetAllDependencies(bundleName);
			if (mAssetBundleMap.ContainsKey(bundleName))
			{
				rightToLeft(ref bundleName);
				// 遍历当前AB的所有依赖项
				foreach (string dep in deps)
				{
					string depName = dep;
					rightToLeft(ref depName);
					List<AssetBuildBundleInfo> infoList = mAssetBundleMap[bundleName];
					foreach (AssetBuildBundleInfo info in infoList)
					{
						info.AddDependence(depName);
					}
				}
			}
		}

		// 生成XML
		XMLDocument doc = new XMLDocument();
		doc.startObject("files", true);
		foreach (var item in mAssetBundleMap)
		{
			int count = item.Value.Count;
			for (int i = 0; i < count; ++i)
			{
				AssetBuildBundleInfo info = item.Value[i];
				doc.startObject("file");
				doc.createElement("bundleName", info.bundleName);
				doc.createElement("assetName", info.assetName);
				if (info.dependencies != null)
				{
					doc.startObject("deps");
					foreach (string dep in info.dependencies)
					{
						doc.createElement("dep", dep);
					}
					doc.endObject("deps");
				}
				doc.endObject("file", true);
			}
		}
		doc.endObject("files");

		FileStream fs = new FileStream(CommonDefine.P_STREAMING_ASSETS_PATH + "StreamingAssets.xml", FileMode.Create);
		byte[] data = Encoding.UTF8.GetBytes(doc.ToString());
		fs.Write(data, 0, data.Length);
		fs.Flush();
		fs.Close();

		UnityUtility.messageBox("资源打包结束! 耗时 : " + (DateTime.Now - time0), false);
	}
	protected static void findAssetBundleBuild(string path, ref AssetBundleBuild[] list)
	{
		Dictionary<string, List<string>> assetBundleList = new Dictionary<string, List<string>>();
		List<string> dirList = new List<string>();
		getAllSubResDirs(path, dirList);
		int dirCount = dirList.Count;
		for (int i = 0; i < dirCount; ++i)
		{
			string[] files = Directory.GetFiles(dirList[i]);
			int fileCount = files.Length;
			for (int j = 0; j < fileCount; ++j)
			{
				// .asset文件和.meta不打包
				if (endWith(files[j], ".meta", false) || endWith(files[j], ".asset", false))
				{
					continue;
				}
				string bundleName = getFileAssetBundleName(files[j]);
				if (!assetBundleList.ContainsKey(bundleName))
				{
					assetBundleList.Add(bundleName, new List<string>());
				}
				assetBundleList[bundleName].Add(files[j]);
			}
		}
		list = new AssetBundleBuild[assetBundleList.Count];
		int index = 0;
		foreach (var item in assetBundleList)
		{
			AssetBundleBuild build = new AssetBundleBuild();
			build.assetBundleName = item.Key;
			int assetCount = item.Value.Count;
			build.assetNames = new string[assetCount];
			for (int i = 0; i < assetCount; ++i)
			{
				build.assetNames[i] = item.Value[i];
			}
			list[index++] = build;
		}
	}
	// 获得一个文件的所属AssetBundle名,file是以Assets开头的相对路径
	protected static string getFileAssetBundleName(string file)
	{
		string bundleName = "";
		// prefab和unity(但是一般情况下unity场景文件不打包)单个文件打包,就是直接替换后缀名
		if (endWith(file, ".prefab") || endWith(file, ".unity"))
		{
			bundleName = file.Substring(CommonDefine.P_RESOURCE_PATH.Length);
			bundleName = getFileNameNoSuffix(bundleName) + CommonDefine.ASSET_BUNDLE_SUFFIX;
		}
		// 其他文件的AssetBundle就是所属文件夹
		else
		{
			string pathUnderResources = getFilePath(file).Substring(CommonDefine.P_RESOURCE_PATH.Length);
			bundleName = pathUnderResources + CommonDefine.ASSET_BUNDLE_SUFFIX;
		}
		bundleName = bundleName.ToLower();
		return bundleName;
	}
	// 判断一个路径是否是不需要打包的路径
	protected static bool isUnpackPath(string path)
	{
		string pathUnderResources = path.Substring(CommonDefine.P_RESOURCE_PATH.Length);
		int unpackCount = GameDefine.mUnPackFolder.Length;
		for (int i = 0; i < unpackCount; ++i)
		{
			// 如果该文件夹是不打包的文件夹,则直接返回
			if (startWith(pathUnderResources, GameDefine.mUnPackFolder[i], false))
			{
				return true;
			}
		}
		return false;
	}
	// fullPath是以Asset开头的路径
	protected static void setAssetBundleName(string fullPath)
	{
		string[] files = Directory.GetFiles(fullPath);
		if (files == null || files.Length == 0)
		{
			return;
		}
		if (isUnpackPath(fullPath))
		{
			return;
		}
		foreach (string file in files)
		{
			// .asset文件和.meta不打包
			if (file.EndsWith(".meta") || file.EndsWith(".asset"))
			{
				continue;
			}
			AssetImporter importer = AssetImporter.GetAtPath(file);
			if (importer != null)
			{
				string fileName = file.ToLower();
				rightToLeft(ref fileName);
				string bundleName = getFileAssetBundleName(fileName);
				importer.assetBundleName = bundleName;
				EditorUtility.UnloadUnusedAssetsImmediate();

				// 存储bundleInfo
				AssetBuildBundleInfo info = new AssetBuildBundleInfo();
				info.assetName = fileName.Substring(CommonDefine.P_RESOURCE_PATH.Length);   // 去除Asset/Resources/前缀,只保留Resources下相对路径
				info.bundleName = bundleName;
				if (!mAssetBundleMap.ContainsKey(info.bundleName))
				{
					mAssetBundleMap.Add(info.bundleName, new List<AssetBuildBundleInfo>());
				}
				mAssetBundleMap[info.bundleName].Add(info);
			}
			else
			{
				Debug.LogFormat("Set AssetName Fail, File:{0}, Msg:Importer is null", file);
			}
		}
	}
	// 递归获取所有子目录文件夹
	protected static void getAllSubResDirs(string fullPath, List<string> dirList)
	{
		if ((dirList == null) || (string.IsNullOrEmpty(fullPath)))
		{
			return;
		}

		string[] dirs = Directory.GetDirectories(fullPath);
		if (dirs != null && dirs.Length > 0)
		{
			for (int i = 0; i < dirs.Length; ++i)
			{
				getAllSubResDirs(dirs[i], dirList);
			}
		}
		dirList.Add(fullPath);
	}
	// 创建和清理输出目录
	protected static void createOrClearOutPath()
	{
		if (!Directory.Exists(CommonDefine.P_STREAMING_ASSETS_PATH))
		{
			Directory.CreateDirectory(CommonDefine.P_STREAMING_ASSETS_PATH);
		}
		else
		{
			// 查找目录下的所有第一级子目录
			string[] dirList = Directory.GetDirectories(CommonDefine.P_STREAMING_ASSETS_PATH);
			int dirCount = dirList.Length;
			for (int i = 0; i < dirCount; ++i)
			{
				// 只删除不需要保留的目录
				if (!isKeepFolderOrMeta(getFolderName(dirList[i])))
				{
					Directory.Delete(dirList[i], true);
				}
			}
			// 查找目录下的所有第一级文件
			string[] files = Directory.GetFiles(CommonDefine.P_STREAMING_ASSETS_PATH);
			int fileCount = files.Length;
			for (int i = 0; i < fileCount; ++i)
			{
				if (!isKeepFolderOrMeta(getFileName(files[i])))
				{
					File.Delete(files[i]);
				}
			}
		}
	}
	protected static bool isKeepFolderOrMeta(string name)
	{
		int count = GameDefine.mKeepFolder.Length;
		for (int i = 0; i < count; ++i)
		{
			if (GameDefine.mKeepFolder[i] == name || GameDefine.mKeepFolder[i] + ".meta" == name)
			{
				return true;
			}
		}
		return false;
	}
	// 清理之前设置的bundleName
	protected static void clearAssetBundleName()
	{
		string[] bundleNames = AssetDatabase.GetAllAssetBundleNames();
		int length = bundleNames.Length;
		for (int i = 0; i < length; ++i)
		{
			AssetDatabase.RemoveAssetBundleName(bundleNames[i], true);
			UnityUtility.logInfo("remove asset bundle name : " + bundleNames[i]);
		}
	}
}