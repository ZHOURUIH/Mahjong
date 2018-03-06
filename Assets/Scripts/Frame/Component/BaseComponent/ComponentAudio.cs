using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ComponentAudio : GameComponent
{
	protected AudioSource mAudioSource;
	public ComponentAudio(Type type, string name)
		:
		base(type, name)
	{
		;
	}
	public override void init(ComponentOwner owner)
	{
		base.init(owner);
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
	public virtual void play(string name, bool isLoop, float volume)
	{
		// 先确定音频源已经设置
		if (mAudioSource == null)
		{
			assignAudioSource();
		}
		if (name == "")
		{
			stop();
		}
		else
		{
			mAudioManager.playClip(mAudioSource, name, isLoop, volume);
		}
	}
	public void play(SOUND_DEFINE sound, bool loop, float volume)
	{
		play(mAudioManager.getAudioName(sound), loop, volume);
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
}