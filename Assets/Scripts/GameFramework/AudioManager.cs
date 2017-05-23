using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : GameBase
{
	Dictionary<string, AudioClip> mAudioClipList;	// 音效资源列表
	public AudioManager()
	{
		mAudioClipList = new Dictionary<string, AudioClip>();
	}
	public virtual void init()
	{
		;
	}
	public void destroy()
	{
		mAudioClipList.Clear();
	}
	public virtual void update(float elapsedTime)
	{}
	// 参数为Sound下的相对路径,并且不带后缀
	public void createAudio(string fileName)
	{
		if (mAudioClipList.ContainsKey(fileName))
		{
			UnityUtility.logError("error : audio has already loaded! file name : " + fileName);
			return;
		}
		AudioClip audioClip = mResourceManager.loadResource<AudioClip>(CommonDefine.R_SOUND_PATH + fileName, false);
		mAudioClipList.Add(fileName, audioClip);
	}
	// volume范围0-1
	public void playClip(AudioSource source, string name, bool loop, float volume)
	{
		if (source == null)
		{
			return;
		}
		if (!mAudioClipList.ContainsKey(name))
		{
			return;
		}
		AudioClip clip = mAudioClipList[name];
		if(clip == null)
		{
			return;
		}
		source.enabled = true;
		source.clip = clip;
		source.loop = loop;
		source.volume = volume;
		source.Play();
	}
	public void stopClip(AudioSource source)
	{
		if (source == null)
		{
			return;
		}
		source.Stop();
	}

	public void setVolume(AudioSource source, float volume)
	{
		if (source == null)
		{
			return;
		}
		source.volume = volume;
	}
	public void setLoop(AudioSource source, bool loop)
	{
		if (source == null)
		{
			return;
		}
		source.loop = loop;
	}
}