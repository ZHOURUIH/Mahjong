using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;

public class KeyFrameManager : GameBase
{
	protected GameObject mManagerObject = null;
	protected Dictionary<string, AnimationCurve> mCurveList = new Dictionary<string, AnimationCurve>();
	public KeyFrameManager()
	{}
	public AnimationCurve getKeyFrame(string name)
	{
		if (mCurveList.ContainsKey(name))
		{
			return mCurveList[name];
		}
		else 
		{
			if (mCurveList.ContainsKey(name.ToLower()))
			{
				return mCurveList[name.ToLower()];
			}	
		}
		return null;
	}
	public void init()
	{
		// 查找关键帧管理器物体
		mManagerObject = UnityUtility.getGameObject(mGameFramework.getGameFrameObject(), "KeyFrameManager");
		if (mManagerObject == null)
		{
			UnityUtility.logError("error: can not find KeyFrameManager!");
			return;
		}
		List<string> fileList = new List<string>();
		List<string> patterns = new List<string>();
		patterns.Add(".prefab");
		patterns.Add(CommonDefine.ASSET_BUNDLE_SUFFIX);
		string prefabPath = "";
		string filePath = "";
		// 如果在Resources文件夹中找不到,则需要到StreamingAssets中找
		if (FileUtility.isDirExist(CommonDefine.F_ASSETS_PATH + CommonDefine.A_KEY_FRAME_PATH))
		{
			filePath = CommonDefine.A_KEY_FRAME_PATH;
			prefabPath = CommonDefine.R_KEY_FRAME_PATH;
		}
		else
		{
			filePath = CommonDefine.A_BUNDLE_KEY_FRAME_PATH;
			prefabPath = CommonDefine.R_KEY_FRAME_PATH.ToLower();
		}
		FileUtility.findFiles(filePath, ref fileList, patterns);
		int fileCount = fileList.Count;
		for (int i = 0; i < fileCount; ++i)
		{
			string fileName = fileList[i];
			string prefabName = StringUtility.getFileNameNoSuffix(ref fileName);
			// 实例化该预设文件,并挂接到关键帧管理器下面
			GameObject keyFrameObject = UnityUtility.instantiatePrefab(mManagerObject, prefabPath + prefabName);
			// 查找关键帧曲线,加入列表中
			TweenScale tweenScale = keyFrameObject.GetComponent<TweenScale>();
			if (tweenScale == null)
			{
				UnityUtility.logError("object in KeyFrame folder must has TweenScale!");
				return;
			}
			AnimationCurve curve = tweenScale.animationCurve;
			if (curve != null)
			{
				mCurveList.Add(keyFrameObject.name, curve);
			}
			else
			{
				UnityUtility.logError("object in KeyFrame folder must has TweenScale and AnimationCurve!");
			}
		}
	}
	public void destroy()
	{
		mCurveList.Clear();
	}
}
