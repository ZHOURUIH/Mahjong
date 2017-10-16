using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AssetInfo : GameBase
{
	public string mAssetName;						// 资源文件名,带相对于StreamingAssets的相对路径,带后缀
	public AssetBundleInfo mParentAssetBundle;		// 资源所属的AssetBundle
	public UnityEngine.Object mAssetObject;			// 资源
	public AssetInfo(AssetBundleInfo parent, string name)
	{
		mParentAssetBundle = parent;
		mAssetName = name;
		mAssetObject = null;
	}
}