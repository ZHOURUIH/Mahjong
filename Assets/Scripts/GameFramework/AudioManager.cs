using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : GameBase
{
	protected Dictionary<string, AudioClip> mAudioClipList;	// 音效资源列表
	protected List<string> mAudioFlieName;
	protected int mLoadedCount;
	public AudioManager()
	{
		mAudioClipList = new Dictionary<string, AudioClip>();
		mAudioFlieName = new List<string>();
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
	public void createAudio(string fileName, bool load = true, bool async = true)
	{
		if (mAudioClipList.ContainsKey(fileName))
		{
			UnityUtility.logError("error : audio has already loaded! file name : " + fileName);
			return;
		}		
		mAudioClipList.Add(fileName, null);
		mAudioFlieName.Add(fileName);
		if(load)
		{
			loadAudio(fileName, async);
		}
	}
	// 加载所有已经注册的音效
	public void loadAll(bool async)
	{
		int audioClipCount = mAudioClipList.Count;
		for (int i = 0; i < audioClipCount; i++)
		{
			if (mAudioClipList[mAudioFlieName[i]] == null)
			{
				loadAudio(mAudioFlieName[i], async);
			}
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
	public float getLoadedPercent()
	{
		return (float)mLoadedCount / (float)mAudioClipList.Count;
	}
	//--------------------------------------------------------------------------------------------------------------------------------------
	protected void onAudioLoaded(UnityEngine.Object res)
	{
		if (res != null)
		{
			mAudioClipList[res.name] = res as AudioClip;
		}
		++mLoadedCount;
	}
	// name为Sound下相对路径,不带后缀
	protected void loadAudio(string name, bool async)
	{
		if (async)
		{
			mResourceManager.loadResourceAsync<AudioClip>(CommonDefine.R_SOUND_PATH + name, onAudioLoaded, false);
		}
		else
		{
			AudioClip audio = mResourceManager.loadResource<AudioClip>(CommonDefine.R_SOUND_PATH + name, false);
			if (audio != null)
			{
				mAudioClipList[audio.name] = audio;
			}
			++mLoadedCount;
		}
	}
}