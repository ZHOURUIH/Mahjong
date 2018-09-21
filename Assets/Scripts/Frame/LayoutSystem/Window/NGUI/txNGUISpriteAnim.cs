using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class txNGUISpriteAnim : txNGUISprite
{
	protected List<string> mTextureNameList;
	protected string		mTextureSetName;
	protected SpriteAnimCallBack mPlayEndCallback;  // 一个序列播放完时的回调函数,只在非循环播放状态下有效
	protected SpriteAnimCallBack mPlayingCallback;  // 一个序列正在播放时的回调函数
	public AnimControl mControl;
	public txNGUISpriteAnim()
	{
		mType = UI_TYPE.UT_NGUI_SPRITE_ANIM;
		mControl = new AnimControl();
		mTextureNameList = new List<string>();
	}
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		string spriteName = getSpriteName();
		string textureSetName = spriteName.Substring(0, spriteName.LastIndexOf('_'));
		setTextureSet(textureSetName);
		mControl.setPlayEndCallback(onPlayEnd);
		mControl.setPlayingCallback(onPlaying);
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
		if (mTextureNameList.Count == 0)
		{
			setSpriteName("");
		}
		mControl.update(elapsedTime);
	}
	public string getTextureSetName() { return mTextureSetName; }
	public int getTextureFrameCount() { return mTextureNameList.Count; }
	public void setTextureSet(string textureSetName)
	{
		mTextureNameList.Clear();
		mTextureSetName = textureSetName;
		if (mSprite != null && mSprite.atlas != null)
		{
			Dictionary<string, UISpriteData> nameList = new Dictionary<string,UISpriteData>();
			List<UISpriteData> sprites = mSprite.atlas.spriteList;
			for (int i = 0, imax = sprites.Count; i < imax; ++i)
			{
				if (sprites[i].name.StartsWith(mTextureSetName))
				{
					nameList.Add(sprites[i].name, sprites[i]);
				}
			}
			int index = 0;
			while(true)
			{
				string name = mTextureSetName + "_" + StringUtility.intToString(++index);
				if(nameList.ContainsKey(name))
				{
					mTextureNameList.Add(name);
				}
				else
				{
					break;
				}
			}
		}
		mControl.setFrameCount(getTextureFrameCount());
	}
	public void setPlayEndCallback(SpriteAnimCallBack callback)
	{
		// 设置前先调用之前的回调通知被中断了
		SpriteAnimCallBack curCallback = mPlayEndCallback;
		mPlayEndCallback = null;
		// 如果回调函数当前不为空,则是中断了更新
		if (curCallback != null)
		{
			curCallback(this, null, true);
		}
		mPlayEndCallback = callback;
	}
	public void setPlayingCallback(SpriteAnimCallBack callback)
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
	//--------------------------------------------------------------------------------------------------------
	protected void onPlaying(AnimControl control, int frame, bool isPlaying)
	{
		mSprite.spriteName = mTextureNameList[mControl.getCurFrameIndex()];
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
		SpriteAnimCallBack temp = mPlayEndCallback;
		mPlayEndCallback = null;
		if (temp != null && callback)
		{
			temp(this, null, isBreak);
		}
	}
}