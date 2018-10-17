using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioInfo
{
	public AudioClip mClip;
	public string mAudioName;   // 音效名,不含路径和后缀名
	public string mAudioPath;   // 相对于Sound的路径
	public string mSuffix;      // 后缀名
	public LOAD_STATE mState;
	public bool mIsResource;    // 是否为固定资源,如果为false则是通过链接加载的,可以是网络链接也可以是本地链接
}

public class AudioManager : FrameComponent
{
	protected Dictionary<string, AudioInfo> mAudioClipList;     // 音效资源列表
	protected Dictionary<SOUND_DEFINE, string> mSoundDefineMap; // 音效定义与音效名的映射
	protected Dictionary<SOUND_DEFINE, float> mVolumeScale;
	protected int mLoadedCount;
	public AudioManager(string name)
		: base(name)
	{
		mAudioClipList = new Dictionary<string, AudioInfo>();
		mSoundDefineMap = new Dictionary<SOUND_DEFINE, string>();
		mVolumeScale = new Dictionary<SOUND_DEFINE, float>();
	}
	public override void init()
	{
		List<SoundData> soundDataList;
		mSQLiteSound.queryAll(out soundDataList);
		int dataCount = soundDataList.Count;
		for (int i = 0; i < dataCount; ++i)
		{
			SoundData soundData = soundDataList[i];
			string audioName = StringUtility.getFileNameNoSuffix(soundData.mFileName, true);
			SOUND_DEFINE soundID = (SOUND_DEFINE)(soundData.mID);
			mSoundDefineMap.Add(soundID, audioName);
			if (!mVolumeScale.ContainsKey(soundID))
			{
				mVolumeScale.Add(soundID, soundData.mVolumeScale);
			}
			registeAudio(soundData.mFileName);
		}
	}
	public override void destroy()
	{
		foreach (var item in mAudioClipList)
		{
			// 只销毁通过链接加载的音频
			if (!item.Value.mIsResource)
			{
				UnityUtility.destroyGameObject(item.Value.mClip);
			}
		}
		mAudioClipList.Clear();
		base.destroy();
	}
	public override void update(float elapsedTime)
	{ }
	public void createStreamingAudio(string url, bool load = true)
	{
		string audioName = StringUtility.getFileNameNoSuffix(url, true);
		if (!mAudioClipList.ContainsKey(audioName))
		{
			AudioInfo newInfo = new AudioInfo();
			newInfo.mAudioName = audioName;
			newInfo.mAudioPath = StringUtility.getFilePath(url);
			newInfo.mClip = null;
			newInfo.mState = LOAD_STATE.LS_UNLOAD;
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
		foreach (var item in mAudioClipList)
		{
			if (item.Value.mClip == null && item.Value.mState == LOAD_STATE.LS_UNLOAD)
			{
				loadAudio(item.Value, async);
			}
		}
	}
	public void loadAudio(string fileName, bool async = true)
	{
		string audioName = StringUtility.getFileNameNoSuffix(fileName, true);
		if (mAudioClipList.ContainsKey(audioName))
		{
			loadAudio(mAudioClipList[audioName], async);
		}
	}
	public void loadAudio(SOUND_DEFINE sound, bool async = true)
	{
		if (!mSoundDefineMap.ContainsKey(sound))
		{
			return;
		}
		string audioName = mSoundDefineMap[sound];
		if (mAudioClipList.ContainsKey(audioName))
		{
			loadAudio(mAudioClipList[audioName], async);
		}
	}
	public float getAudioLength(string name)
	{
		if (!mAudioClipList.ContainsKey(name) || mAudioClipList[name] == null || mAudioClipList[name].mClip == null)
		{
			return 0.0f;
		}
		return mAudioClipList[name].mClip.length;
	}
	// volume范围0-1,loadAudio表示如果音效未加载,则加载音效
	public void playClip(AudioSource source, string name, bool loop, float volume, bool load = true)
	{
		if (source == null)
		{
			return;
		}
		if (!mAudioClipList.ContainsKey(name))
		{
			return;
		}
		AudioClip clip = mAudioClipList[name].mClip;
		// 如果音效为空,则尝试加载
		if (clip == null)
		{
			if (load && mAudioClipList[name].mState == LOAD_STATE.LS_UNLOAD)
			{
				loadAudio(mAudioClipList[name], false);
				clip = mAudioClipList[name].mClip;
			}
			else
			{
				return;
			}
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
	public float getVolumeScale(SOUND_DEFINE sound)
	{
		if (mVolumeScale.ContainsKey(sound))
		{
			return mVolumeScale[sound];
		}
		return 1.0f;
	}
	public string getAudioName(SOUND_DEFINE soundDefine)
	{
		if (mSoundDefineMap.ContainsKey(soundDefine))
		{
			return mSoundDefineMap[soundDefine];
		}
		return "";
	}
	//--------------------------------------------------------------------------------------------------------------------------------------
	// 参数为Sound下的相对路径,并且不带后缀
	public void registeAudio(string fileName)
	{
		string audioName = StringUtility.getFileNameNoSuffix(fileName, true);
		if (!mAudioClipList.ContainsKey(audioName))
		{
			AudioInfo newInfo = new AudioInfo();
			newInfo.mAudioName = audioName;
			newInfo.mAudioPath = StringUtility.getFilePath(fileName);
			newInfo.mClip = null;
			newInfo.mState = LOAD_STATE.LS_UNLOAD;
			newInfo.mIsResource = true;
			newInfo.mSuffix = "";
			mAudioClipList.Add(audioName, newInfo);
		}
	}
	protected void onAudioLoaded(UnityEngine.Object res, object userData)
	{
		if (res != null)
		{
			string name = StringUtility.getFileNameNoSuffix(res.name, true);
			mAudioClipList[name].mClip = res as AudioClip;
			mAudioClipList[name].mState = LOAD_STATE.LS_LOADED;
		}
		++mLoadedCount;
	}
	// name为Resource下相对路径,不带后缀
	protected void loadAudio(AudioInfo info, bool async)
	{
		if(info.mClip != null || info.mState != LOAD_STATE.LS_UNLOAD)
		{
			return;
		}
		info.mState = LOAD_STATE.LS_LOADING;
		string path = info.mAudioPath;
		if (path != "" && !path.EndsWith("/"))
		{
			path += "/";
		}
		if (info.mIsResource)
		{
			string fullName = CommonDefine.R_SOUND_PATH + path + info.mAudioName;
			if (async)
			{
				mResourceManager.loadResourceAsync<AudioClip>(fullName, onAudioLoaded, null, false);
			}
			else
			{
				AudioClip audio = mResourceManager.loadResource<AudioClip>(fullName, false);
				if (audio != null)
				{
					info.mClip = audio;
					info.mState = LOAD_STATE.LS_LOADED;
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