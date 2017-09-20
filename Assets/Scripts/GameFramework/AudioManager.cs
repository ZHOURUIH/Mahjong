using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioInfo
{
	public AudioClip mClip;
	public string mAudioName;	// 音效名,不含路径和后缀名
	public string mAudioPath;	// 相对于Sound的路径
}

public class AudioManager : GameBase
{
	protected Dictionary<string, AudioInfo> mAudioClipList;	// 音效资源列表
	protected int mLoadedCount;
	public AudioManager()
	{
		mAudioClipList = new Dictionary<string, AudioInfo>();
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
		string audioName = StringUtility.getFileNameNoSuffix(fileName, true);
		if (!mAudioClipList.ContainsKey(audioName))
		{
			AudioInfo newInfo = new AudioInfo();
			newInfo.mAudioName = StringUtility.getFileNameNoSuffix(fileName, true);
			newInfo.mAudioPath = StringUtility.getFilePath(fileName);
			newInfo.mClip = null;
			mAudioClipList.Add(audioName, newInfo);
		}
		AudioInfo info = mAudioClipList[audioName];
		if (load && info.mClip == null)
		{
			loadAudio(info.mAudioName, info.mAudioPath, async);
		}
	}
	// 加载所有已经注册的音效
	public void loadAll(bool async)
	{
		foreach(var item in mAudioClipList)
		{
			if(item.Value.mClip == null)
			{
				loadAudio(item.Value.mAudioName, item.Value.mAudioPath, async);
			}
		}
	}
	public float getAudioLength(string name)
	{
		if (!mAudioClipList.ContainsKey(name) || mAudioClipList[name] == null)
		{
			return 0.0f;
		}
		return mAudioClipList[name].mClip.length;
	}
	// volume范围0-1
	public void playClip(AudioSource source, string name, bool loop, float volume)
	{
		if (source == null)
		{
			return;
		}
		if (!mAudioClipList.ContainsKey(name) || mAudioClipList[name].mClip == null)
		{
			return;
		}
		AudioClip clip = mAudioClipList[name].mClip;
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
	public void pauseClip(AudioSource source)
	{
		if (source == null)
		{
			return;
		}
		source.Pause();
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
	protected void onAudioLoaded(UnityEngine.Object res, object userData)
	{
		if (res != null)
		{
			mAudioClipList[res.name].mClip = res as AudioClip;
		}
		++mLoadedCount;
	}
	// name为Resource下相对路径,不带后缀
	protected void loadAudio(string name, string path, bool async)
	{
		if(!mAudioClipList.ContainsKey(name))
		{
			UnityUtility.logError("音效还未注册,请使用createAudio注册音效");
			return;
		}
		if(path != "" && !path.EndsWith("/"))
		{
			path += "/";
		}
		string fullName = CommonDefine.R_SOUND_PATH + path + name;
		if (async)
		{
			mResourceManager.loadResourceAsync<AudioClip>(fullName, onAudioLoaded, false);
		}
		else
		{
			AudioClip audio = mResourceManager.loadResource<AudioClip>(fullName, false);
			if (audio != null)
			{
				mAudioClipList[audio.name].mClip = audio;
			}
			++mLoadedCount;
		}
	}
}