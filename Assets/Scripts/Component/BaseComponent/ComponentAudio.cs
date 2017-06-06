using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ComponentAudio : GameComponent
{
	protected static Dictionary<SOUND_DEFINE, string>	mSoundDefineMap;	// 音效定义与音效名的映射
	protected static Dictionary<string, int>			mAudioTypeMap;		// 存储各个音效的类型
	protected Dictionary<int, string>					mCurPlayList;		// 正在播放的各类型音效
	protected string		mSoundOwner;	// 音效所有者类型名
	protected int			mMaxChannel;	// 通道最大值,也就是通道数量
	protected AudioSource	mAudioSource;
	public ComponentAudio(Type type, string name)
		:
		base(type, name)
	{
		if (mSoundDefineMap == null)
		{
			mSoundDefineMap = new Dictionary<SOUND_DEFINE, string>();
		}
		if (mAudioTypeMap == null)
		{
			mAudioTypeMap = new Dictionary<string, int>();
		}
		mCurPlayList = new Dictionary<int, string>();
		mMaxChannel = 1;	// 暂时只有一个通道,因为Unity中一个物体只能添加一个AudioSource
	}
	public override void init(ComponentOwner owner)
	{
		base.init(owner);
		// 通知子类设置自己的音效类型
		setSoundOwner();
		// 如果音效还未加载,则加载所有音效,此处只是注册
		if (mAudioTypeMap.Count == 0)
		{
			int dataCount = mDataBase.getDataCount(DATA_TYPE.DT_GAME_SOUND);
			for (int i = 0; i < dataCount; ++i)
			{
				DataGameSound soundData = mDataBase.queryData(DATA_TYPE.DT_GAME_SOUND, i) as DataGameSound;
				string soundName = StringUtility.charArrayToString(soundData.mSoundFileName);
				mAudioTypeMap.Add(soundName, soundData.mSoundType);
				mSoundDefineMap.Add((SOUND_DEFINE)(soundData.mSoundID), soundName);
				mAudioManager.createAudio(soundName, false);
			}
		}

		// 清空所有类型正在播放的音效
		for (int i = 0; i < mMaxChannel; ++i)
		{
			mCurPlayList.Add(i, "");
		}
	}
	public void setAudioSource(AudioSource source)
	{
		mAudioSource = source;
		mAudioSource.playOnAwake = false;
	}
	public override void update(float elapsedTime) { }
	public override void setBaseType() { mBaseType = typeof(ComponentAudio); }
	public override bool isType(Type type)
	{
		return type == typeof(ComponentAudio);
	}
	public virtual void setSoundOwner() { }

	public void setLoop(bool loop = false)
	{
		mAudioManager.setLoop(mAudioSource, loop);
	}
	public void setVolume(float vol)
	{
		mAudioManager.setVolume(mAudioSource, vol);
	}
	public int getAudioChannel(string audioFileName)
	{
		if (mAudioTypeMap.ContainsKey(audioFileName))
		{
			return mAudioTypeMap[audioFileName];
		}
		return mMaxChannel;
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
		// 如果该通道正在播放音效,则先停止正在播放的音效
		int channel = getAudioChannel(name);
		if (channel == mMaxChannel)
		{
			return;
		}
		if (mCurPlayList.ContainsKey(channel))
		{
			if (mCurPlayList[channel] != "")
			{
				stop(mCurPlayList[channel]);
			}
		}
		else
		{
			UnityUtility.logError("error : can not find audio channel in cur play list! channel : " + channel);
		}
		// 播放音效
		mAudioManager.playClip(mAudioSource, name, isLoop, volume);
		// 记录下当前正在播放的音效
		mCurPlayList[channel] = name;
	}
	public void play(SOUND_DEFINE sound, bool loop, float volume)
	{
		play(getAudioName(sound), loop, volume);
	}

	public virtual void stop(string name)
	{
		int channel = getAudioChannel(name);
		if (mCurPlayList.ContainsKey(channel))
		{
			stop(channel);
		}
		else
		{
			UnityUtility.logError("error : can not find audio channel in cur play list! channel : " + channel);
		}
	}
	public void stop(SOUND_DEFINE audio)
	{
		if (mSoundDefineMap.ContainsKey(audio))
		{
			stop(mSoundDefineMap[audio]);
		}
	}
	// 暂时停止所有通道的音效
	public void stop(int channel)
	{
		mAudioManager.stopClip(mAudioSource);
	}
}