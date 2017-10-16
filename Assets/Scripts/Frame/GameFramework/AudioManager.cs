using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioInfo
{
	public AudioClip mClip;
	public string mAudioName;	// 音效名,不含路径和后缀名
	public string mAudioPath;   // 相对于Sound的路径
	public string mSuffix;		// 后缀名
	public bool mIsResource;	// 是否为固定资源,如果为false则是通过链接加载的,可以是网络链接也可以是本地链接
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
		foreach(var item in mAudioClipList)
		{
			// 只销毁通过链接加载的音频
			if(!item.Value.mIsResource)
			{
				GameObject.Destroy(item.Value.mClip);
			}
		}
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
			newInfo.mAudioName = audioName;
			newInfo.mAudioPath = StringUtility.getFilePath(fileName);
			newInfo.mClip = null;
			newInfo.mIsResource = true;
			newInfo.mSuffix = "";
			mAudioClipList.Add(audioName, newInfo);
		}
		AudioInfo info = mAudioClipList[audioName];
		if (load && info.mClip == null)
		{
			loadAudio(info, async);
		}
	}
	public void createStreamingAudio(string url, bool load = true)
	{
		string audioName = StringUtility.getFileNameNoSuffix(url, true);
		if (!mAudioClipList.ContainsKey(audioName))
		{
			AudioInfo newInfo = new AudioInfo();
			newInfo.mAudioName = audioName;
			newInfo.mAudioPath = StringUtility.getFilePath(url);
			newInfo.mClip = null;
			newInfo.mIsResource = false;
			newInfo.mSuffix = StringUtility.getFileSuffix(url);
			mAudioClipList.Add(audioName, newInfo);
		}
		AudioInfo info = mAudioClipList[audioName];
		if (load && info.mClip == null)
		{
			loadAudio(info, true);
		}
	}
	// 加载所有已经注册的音效
	public void loadAll(bool async)
	{
		foreach(var item in mAudioClipList)
		{
			if(item.Value.mClip == null)
			{
				loadAudio(item.Value, async);
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
			string name = StringUtility.getFileNameNoSuffix(res.name, true);
			mAudioClipList[name].mClip = res as AudioClip;
		}
		++mLoadedCount;
	}
	// name为Resource下相对路径,不带后缀
	protected void loadAudio(AudioInfo info, bool async)
	{
		string path = info.mAudioPath;
		if (path != "" && !path.EndsWith("/"))
		{
			path += "/";
		}
		if(info.mIsResource)
		{
			string fullName = CommonDefine.R_SOUND_PATH + path + info.mAudioName;
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
		else
		{
			string url = path + info.mAudioName + info.mSuffix;
			mResourceManager.loadAssetsFromUrl<AudioClip>(url, onAudioLoaded, null);
		}
	}
}