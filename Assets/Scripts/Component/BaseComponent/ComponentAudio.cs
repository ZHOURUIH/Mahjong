using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ComponentAudio : GameComponent
{
	protected static Dictionary<SOUND_DEFINE, string>	mSoundDefineMap;	// 音效定义与音效名的映射
	protected string		mSoundOwner;	// 音效所有者类型名
	protected AudioSource	mAudioSource;
	public ComponentAudio(Type type, string name)
		:
		base(type, name)
	{
		if (mSoundDefineMap == null)
		{
			mSoundDefineMap = new Dictionary<SOUND_DEFINE, string>();
		}
	}
	public override void init(ComponentOwner owner)
	{
		base.init(owner);
		// 通知子类设置自己的音效类型
		setSoundOwner();
		// 如果音效还未加载,则加载所有音效,此处只是注册
		if (mSoundDefineMap.Count == 0)
		{
			int dataCount = mDataBase.getDataCount(DATA_TYPE.DT_GAME_SOUND);
			for (int i = 0; i < dataCount; ++i)
			{
				DataGameSound soundData = mDataBase.queryData(DATA_TYPE.DT_GAME_SOUND, i) as DataGameSound;
				string soundName = BinaryUtility.bytesToString(soundData.mSoundFileName);
				string audioName = StringUtility.getFileNameNoSuffix(soundName, true);
				mSoundDefineMap.Add((SOUND_DEFINE)(soundData.mSoundID), audioName);
				mAudioManager.createAudio(soundName, false);
			}
		}
	}
	public override void update(float elapsedTime) { }
	public void setLoop(bool loop = false)
	{
		mAudioManager.setLoop(mAudioSource, loop);
	}
	public void setVolume(float vol)
	{
		mAudioManager.setVolume(mAudioSource, vol);
	}
	public static string getAudioName(SOUND_DEFINE soundDefine)
	{
		if (mSoundDefineMap.ContainsKey(soundDefine))
		{
			return mSoundDefineMap[soundDefine];
		}
		return "";
	}
	public virtual void play(string name, bool isLoop, float volume)
	{
		// 先确定音频源已经设置
		if(mAudioSource == null)
		{
			assignAudioSource();
		}
		// 播放音效
		mAudioManager.playClip(mAudioSource, name, isLoop, volume);
	}
	public void play(SOUND_DEFINE sound, bool loop, float volume)
	{
		play(getAudioName(sound), loop, volume);
	}
	// 暂时停止所有通道的音效
	public void stop()
	{
		mAudioManager.stopClip(mAudioSource);
	}
	//--------------------------------------------------------------------------------------------------------------------------
	protected override void setBaseType() { mBaseType = typeof(ComponentAudio); }
	protected override bool isType(Type type) { return type == typeof(ComponentAudio); }
	protected virtual void assignAudioSource() { }
	protected void setAudioSource(AudioSource source)
	{
		mAudioSource = source;
		if (mAudioSource != null)
		{
			mAudioSource.playOnAwake = false;
		}
	}
	protected virtual void setSoundOwner() { }
}