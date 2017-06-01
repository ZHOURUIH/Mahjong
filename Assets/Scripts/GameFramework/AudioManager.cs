using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : GameBase
{
	protected Dictionary<string, AudioClip> mAudioClipList;	// 音效资源列表
	protected int mLoadedCount;
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
		fileName = StringUtility.getFileNameNoSuffix(fileName, true);
		mAudioClipList.Add(fileName, null);
		bool async = true;
		if (async)
		{
			mResourceManager.loadResourceAsync<AudioClip>(CommonDefine.R_SOUND_PATH + fileName, onAudioLoaded, false);
		}
		else
		{
			AudioClip audio = mResourceManager.loadResource<AudioClip>(CommonDefine.R_SOUND_PATH + fileName, false);
			mAudioClipList[audio.name] = audio;
			++mLoadedCount;
		}
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
	public bool isLoadDone()
	{
		return mLoadedCount == mAudioClipList.Count;
	}
	//--------------------------------------------------------------------------------------------------------------------------------------
	protected void onAudioLoaded(UnityEngine.Object res)
	{
		mAudioClipList[res.name] = res as AudioClip;
		++mLoadedCount;
	}
}