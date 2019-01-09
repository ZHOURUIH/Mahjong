using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class txNGUISpriteAnim : txNGUISprite, INGUIAnimation
{
	protected List<string> mTextureNameList;
	protected List<Vector2> mTexturePosList;
	protected string mTextureSetName;
	protected bool mUseTextureSize;
	protected bool mMakeEvenSize;		// 是否将尺寸调整为2的倍数
	protected List<TextureAnimCallBack> mPlayEndCallback;  // 一个序列播放完时的回调函数,只在非循环播放状态下有效
	protected List<TextureAnimCallBack> mPlayingCallback;  // 一个序列正在播放时的回调函数
	protected AnimControl mControl;
	public txNGUISpriteAnim()
	{
		mControl = new AnimControl();
		mTextureNameList = new List<string>();
		mPlayEndCallback = new List<TextureAnimCallBack>();
		mPlayingCallback = new List<TextureAnimCallBack>();
		mMakeEvenSize = false;
		mUseTextureSize = false;
	}
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		string spriteName = getSpriteName();
		if(spriteName != null && spriteName != "")
		{
			int index = spriteName.LastIndexOf('_');
			if (index >= 0)
			{
				string textureSetName = spriteName.Substring(0, index);
				setTextureSet(textureSetName);
			}
		}
		mControl.setPlayEndCallback(onPlayEnd);
		mControl.setPlayingCallback(onPlaying);
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
		if (mTextureNameList.Count == 0)
		{
			setSpriteName("", false, false);
		}
		mControl.update(elapsedTime);
	}
	public override void setAtlas(UIAtlas atlas)
	{
		// 改变图集时先停止播放
		stop();
		base.setAtlas(atlas);
	}
	public string getTextureSet() { return mTextureSetName; }
	public int getTextureFrameCount() { return mTextureNameList.Count; }
	public void setUseTextureSize(bool useSize, bool makeEvenSize = false)
	{
		mUseTextureSize = useSize;
		mMakeEvenSize = makeEvenSize;
	}
	public void setTexturePosList(List<Vector2> posList) { mTexturePosList = posList; }
	public void setTextureSet(string textureSetName)
	{
		setTextureSet(textureSetName, "");
	}
	public void setTextureSet(string textureSetName, string subPath)
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
				string name = mTextureSetName + "_" + intToString(index++);
				if(nameList.ContainsKey(name))
				{
					mTextureNameList.Add(name);
				}
				else
				{
					break;
				}
			}
			if(getTextureFrameCount() == 0)
			{
				logError("invalid sprite anim! atals : " + mSprite.atlas.name + ", anim set : " + textureSetName);
			}
		}
		mControl.setFrameCount(getTextureFrameCount());
	}
	public LOOP_MODE getLoop() { return mControl.getLoop(); }
	public float getInterval() { return mControl.getInterval(); }
	public float getSpeed() { return mControl.getSpeed(); }
	public int getStartIndex() { return mControl.getStartIndex(); }
	public PLAY_STATE getPlayState() { return mControl.getPlayState(); }
	public bool getPlayDirection() { return mControl.getPlayDirection(); }
	public int getEndIndex() { return mControl.getEndIndex(); }
	public bool getAutoHide() { return mControl.getAutoResetIndex(); }
	// 获得实际的终止下标,如果是自动获得,则返回最后一张的下标
	public int getRealEndIndex() { return mControl.getRealEndIndex(); }
	public void setLoop(LOOP_MODE loop) { mControl.setLoop(loop); }
	public void setInterval(float interval) { mControl.setInterval(interval); }
	public void setSpeed(float speed) { mControl.setSpeed(speed); }
	public void setPlayDirection(bool direction) { mControl.setPlayDirection(direction); }
	public void setAutoHide(bool autoHide) { mControl.setAutoHide(autoHide); }
	public void setStartIndex(int startIndex) { mControl.setStartIndex(startIndex); }
	public void setEndIndex(int endIndex) { mControl.setEndIndex(endIndex); }
	public void stop(bool resetStartIndex = true, bool callback = true, bool isBreak = true) { mControl.stop(resetStartIndex, callback, isBreak); }
	public void play() { mControl.play(); }
	public void pause() { mControl.pause(); }
	public int getCurFrameIndex() { return mControl.getCurFrameIndex(); }
	public void setCurFrameIndex(int index) { mControl.setCurFrameIndex(index); }
	public void addPlayEndCallback(TextureAnimCallBack callback, bool clear = true)
	{
		if (clear)
		{
			List<TextureAnimCallBack> curCallback = new List<TextureAnimCallBack>(mPlayEndCallback);
			mPlayEndCallback.Clear();
			// 如果回调函数当前不为空,则是中断了更新
			foreach (var item in curCallback)
			{
				item(this, true);
			}
		}
		mPlayEndCallback.Add(callback);
	}
	public void addPlayingCallback(TextureAnimCallBack callback)
	{
		mPlayingCallback.Add(callback);
	}
	//--------------------------------------------------------------------------------------------------------
	protected void onPlaying(AnimControl control, int frame, bool isPlaying)
	{
		if(mControl.getCurFrameIndex() >= mTextureNameList.Count)
		{
			return;
		}
		setSpriteName(mTextureNameList[mControl.getCurFrameIndex()], mUseTextureSize, mMakeEvenSize);
		if (mTexturePosList != null)
		{
			int positionIndex = (int)(frame / (float)mTextureNameList.Count * mTexturePosList.Count + 0.5f);
			setLocalPosition(mTexturePosList[positionIndex]);
		}
		foreach (var item in mPlayingCallback)
		{
			item(this, false);
		}
	}
	protected void onPlayEnd(AnimControl control, bool callback, bool isBreak)
	{
		// 正常播放完毕后根据是否重置下标来判断是否自动隐藏
		if (!isBreak && mControl.getAutoResetIndex())
		{
			setActive(false);
		}
		List<TextureAnimCallBack> temp = new List<TextureAnimCallBack>(mPlayEndCallback);
		mPlayEndCallback.Clear();
		if (temp.Count > 0 && callback)
		{
			foreach(var item in temp)
			{
				item(this, isBreak);
			}
		}
	}
}