using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class txNGUITextureAnim : txNGUITexture
{
	protected List<Texture> mTextureNameList;
	protected string mTextureSetName;
	protected bool mUseTextureSize;
	protected TextureAnimCallBack mPlayEndCallback;  // 一个序列播放完时的回调函数,只在非循环播放状态下有效
	protected TextureAnimCallBack mPlayingCallback;  // 一个序列正在播放时的回调函数
	public AnimControl mControl;
	public txNGUITextureAnim()
	{
		mType = UI_TYPE.UT_NGUI_TEXTURE_ANIM;
		mTextureNameList = new List<Texture>();
		mControl = new AnimControl();
		mUseTextureSize = false;
	}
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		string textureName = getTextureName();
		int index = textureName.LastIndexOf('_');
		if (index >= 0)
		{
			string textureSetName = textureName.Substring(0, index);
			setTextureSet(textureSetName);
		}
		mControl.setPlayEndCallback(onPlayEnd);
		mControl.setPlayingCallback(onPlaying);
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
		if (mTextureNameList.Count == 0)
		{
			setTexture(null, false);
		}
		mControl.update(elapsedTime);
	}
	public string getTextureSetName() { return mTextureSetName; }
	public int getTextureFrameCount() { return mTextureNameList.Count; }
	public void setUseTextureSize(bool useSize) { mUseTextureSize = useSize; }
	public void setTextureSet(string textureSetName)
	{
		mTextureNameList.Clear();
		mTextureSetName = textureSetName;
		string subPath = mTexture.mUserData;
		if(subPath != "")
		{
			subPath += "/";
		}
		string path = CommonDefine.R_TEXTURE_ANIM_PATH + subPath + mTextureSetName;
		string preName = path + "/" + mTextureSetName + "_";
		for (int i = 0; ; ++i)
		{
			string name = preName + StringUtility.intToString(i);
			Texture tex = mResourceManager.loadResource<Texture>(name, false);
			if (tex == null)
			{
				break;
			}
			mTextureNameList.Add(tex);
		}
		mControl.setFrameCount(getTextureFrameCount());
	}
	public void setPlayEndCallback(TextureAnimCallBack callback)
	{
		TextureAnimCallBack curCallback = mPlayEndCallback;
		mPlayEndCallback = null;
		// 如果回调函数当前不为空,则是中断了更新
		if (curCallback != null)
		{
			curCallback(this, null, true);
		}
		mPlayEndCallback = callback;
	}
	public void setPlayingCallback(TextureAnimCallBack callback)
	{
		mPlayingCallback = callback;
	}
	public void play()
	{
		mControl.play();
	}
	public void pause()
	{
		mControl.pause();
	}
	public void stop(bool resetStartIndex = true, bool callback = true, bool isBreak = true)
	{
		mControl.stop(resetStartIndex, callback, isBreak);
	}
	public void setInterval(float interval)
	{
		mControl.setInterval(interval);
	}
	public void setStartIndex(int startIndex)
	{
		mControl.setStartIndex(startIndex);
	}
	public void setLoop(LOOP_MODE loop)
	{
		mControl.setLoop(loop);
	}
	public void setAutoHide(bool autoHide)
	{
		mControl.setAutoHide(autoHide);
	}
	public void setPlayDirection(bool direction)
	{
		mControl.setPlayDirection(direction);
	}
	public int getStartIndex()
	{
		return mControl.getStartIndex();
	}
	public void setCurFrameIndex(int index)
	{
		mControl.setCurFrameIndex(index);
	}
	public int getRealEndIndex()
	{
		return mControl.getRealEndIndex();
	}
	public LOOP_MODE getLoop()
	{
		return mControl.getLoop();
	}
	public bool getPlayDirection()
	{
		return mControl.getPlayDirection();
	}
	public int getCurFrameIndex()
	{
		return mControl.getCurFrameIndex();
	}
	//---------------------------------------------------------------------------------------------------------------------------------------------------
	protected void onPlaying(AnimControl control, int frame, bool isPlaying)
	{
		if(mControl.getCurFrameIndex() < 0 || mControl.getCurFrameIndex() >= mTextureNameList.Count)
		{
			return;
		}
		setTexture(mTextureNameList[mControl.getCurFrameIndex()], mUseTextureSize);
		if (mPlayingCallback != null)
		{
			mPlayingCallback(this, null, false);
		}
	}
	protected void onPlayEnd(AnimControl control, bool callback, bool isBreak)
	{
		// 正常播放完毕后根据是否重置下标来判断是否自动隐藏
		if (!isBreak && mControl.getAutoResetIndex())
		{
			setActive(false);
		}
		TextureAnimCallBack temp = mPlayEndCallback;
		mPlayEndCallback = null;
		if (temp != null && callback)
		{
			temp(this, null, isBreak);
		}
	}
}