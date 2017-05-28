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
		string path = CommonDefine.R_KEY_FRAME_PATH;
		if(mResourceManager.mLoadSource == 1)
		{
			path = path.ToLower();
		}
		List<string> fileList = mResourceManager.getFileOrBundleList(path);
		int fileCount = fileList.Count;
		for(int i = 0; i < fileCount; ++i)
		{
			string fileNameNoSuffix = StringUtility.getFileNameNoSuffix(fileList[i], true);
			mResourceManager.loadResourceAsync<GameObject>(path + fileNameNoSuffix, onKeyFrameLoaded, true);
		}
	}
	public void destroy()
	{
		mCurveList.Clear();
	}
	//------------------------------------------------------------------------------------------------------------------
	protected void onKeyFrameLoaded(UnityEngine.Object res)
	{
		GameObject keyFrameObject = UnityUtility.instantiatePrefab(mManagerObject, res as GameObject);
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
