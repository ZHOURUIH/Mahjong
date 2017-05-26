using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ReloadTexture
{
	[MenuItem("Reload/Texture")]
	static public void reloadDitherTexture()
	{
		List<string> reloadList = Dither.getDitherList();
		string prePath = CommonDefine.A_RESOURCE_PATH + "Texture/TextureAnim/";
		int pathCount = reloadList.Count;
		for(int i = 0; i < pathCount; ++i)
		{
			reloadPath(prePath + reloadList[i] + "/");
		}
	}
	//----------------------------------------------------------------------------------------------------------------------------
	static protected void reloadPath(string path)
	{
		List<string> files = new List<string>();
		List<string> patterns = new List<string>();
		patterns.Add(".png");
		FileUtility.findFiles(path, ref files, patterns);
		int fileCount = files.Count;
		for(int i = 0; i < fileCount; ++i)
		{
			reloadTexture(CommonDefine.P_ASSETS_PATH + path + files[i]);
		}
	}
	static protected void reloadTexture(string name)
	{
		// 重新导入图片,触发DitherTexture中的处理函数
		TextureImporter texture = AssetImporter.GetAtPath(name) as TextureImporter;
		AssetDatabase.ImportAsset(name);
	}
}
