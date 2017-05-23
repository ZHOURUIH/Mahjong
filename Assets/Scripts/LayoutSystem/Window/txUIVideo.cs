using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using RenderHeads.Media.AVProVideo;

public class txUIVideo : txUIObject
{
	protected UITexture			   mTexture;
	protected MediaPlayer		   mMediaPlayer;
	protected string			   mFileName;
	protected VideoPlayEndCallback mVideoEndCallback;
	protected bool				   mReady = false;
	// 刚设置视频文件,还未加载时,要设置播放状态就需要先保存状态,然后等到视频准备完毕后再设置
	protected bool				   mNextLoop = false;
	protected float				   mNextRate = 1.0f;
	protected PLAY_STATE		   mNextState = PLAY_STATE.PS_NONE;
	protected bool				   mAutoShowOrHide = true;
	public txUIVideo()
	{
		mType = UI_OBJECT_TYPE.UBT_VIDEO;
	}
	public override void init(GameLayout layout, GameObject go)
	{
		base.init(layout, go);
		mTexture = mObject.GetComponent<UITexture>();
		if (mTexture == null)
		{
			mTexture = mObject.AddComponent<UITexture>();
		}
		mMediaPlayer = mObject.GetComponent<MediaPlayer>();
		if(mMediaPlayer == null)
		{
			mMediaPlayer = mObject.AddComponent<MediaPlayer>();
		}
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
		if (mReady && mMediaPlayer.Control.IsPlaying())
		{
			if (mMediaPlayer != null)
			{
				if (mMediaPlayer.TextureProducer != null)
				{
					Texture texture = mMediaPlayer.TextureProducer.GetTexture();
					if (texture != null)
					{
						if (mMediaPlayer.TextureProducer.RequiresVerticalFlip())
						{
							mTexture.flip = UITexture.Flip.Vertically;
						}
						else
						{
							mTexture.flip = UIBasicSprite.Flip.Nothing;
						}
						mTexture.mainTexture = texture;
					}
				}
			}
			else
			{
				mTexture.mainTexture = null;
			}
		}
	}
	public void setPlayState(PLAY_STATE state, bool autoShowOrHide = true)
	{
		if (mReady)
		{
			if (state == PLAY_STATE.PS_PLAY)
			{
				play(autoShowOrHide);
			}
			else if (state == PLAY_STATE.PS_PAUSE)
			{
				pause();
			}
			else if (state == PLAY_STATE.PS_STOP)
			{
				stop(autoShowOrHide);
			}
		}
		else
		{
			mNextState = state;
			mAutoShowOrHide = autoShowOrHide;
		}
	}
	public void setFileName(string file)
	{
		setVideoEndCallback(null);
		if (!file.StartsWith(CommonDefine.A_VIDEO_PATH))
		{
			file = CommonDefine.A_VIDEO_PATH + file;
		}
		notifyVideoReady(false);
		mFileName = StringUtility.getFileName(ref file);
		mMediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToDataFolder, file, false);
		mMediaPlayer.Events.RemoveAllListeners();
		mMediaPlayer.Events.AddListener(onVideoEvent);
	}
	public string getFileName()
	{
		return mFileName;
	}
	public void setLoop(bool loop)
	{
		if (mReady)
		{
			mMediaPlayer.Control.SetLooping(loop);
		}
		else
		{
			mNextLoop = loop;
		}
	}
	public bool getLoop()
	{
		return mMediaPlayer.m_Loop;
	}
	public void setRate(float rate)
	{
		if (mReady)
		{
			MathUtility.clamp(ref rate, 0.0f, 4.0f);
			if (!MathUtility.isFloatEqual(rate, getRate()))
			{
				mMediaPlayer.Control.SetPlaybackRate(rate);
			}
		}
		else
		{
			mNextRate = rate;
		}
	}
	public float getRate()
	{
		if (mMediaPlayer.Control == null)
		{
			return 0.0f;
		}
		return mMediaPlayer.Control.GetPlaybackRate();
	}
	public float getVideoLength()
	{
		if (mMediaPlayer.Info == null)
		{
			return 0.0f;
		}
		return mMediaPlayer.Info.GetDurationMs() * 0.001f;
	}
	public void setVideoEndCallback(VideoPlayEndCallback callback)
	{
		// 重新设置回调之前,先调用之前的回调
		clearAndCallEvent(true);
		mVideoEndCallback = callback;
	}
	//----------------------------------------------------------------------------------------------------------------------------------------
	protected void notifyVideoReady(bool ready)
	{
		mReady = ready;
		if(mReady)
		{
			setPlayState(mNextState, mAutoShowOrHide);
			setLoop(mNextLoop);
			setRate(mNextRate);
		}
		else
		{
			mNextState = PLAY_STATE.PS_NONE;
			mNextRate = 1.0f;
			mNextLoop = false;
		}
	}
	protected void clearAndCallEvent(bool isBreak)
	{
		VideoPlayEndCallback temp = mVideoEndCallback;
		mVideoEndCallback = null;
		if (temp != null)
		{
			temp(mFileName, isBreak);
		}
	}
	protected void onVideoEvent(MediaPlayer player, MediaPlayerEvent.EventType eventType, ErrorCode errorCode)
	{
		if (eventType == MediaPlayerEvent.EventType.FinishedPlaying)
		{
			// 播放完后设置为停止状态
			clearAndCallEvent(false);
		}
		else if (eventType == MediaPlayerEvent.EventType.ReadyToPlay)
		{
			// 视频准备完毕时,设置实际的状态
			if (mMediaPlayer.Control == null)
			{
				UnityUtility.logError("video is ready, but MediaPlayer.Control is null!");
			}
			notifyVideoReady(true);
		}
		else if(eventType == MediaPlayerEvent.EventType.Error)
		{
			UnityUtility.logError("error code : " + errorCode);
		}
	}
	protected void play(bool autoShow = true)
	{
		if (mMediaPlayer.Control != null)
		{
			if (autoShow)
			{
				mTexture.enabled = true;
			}
			if (!mMediaPlayer.Control.IsPlaying())
			{
				mMediaPlayer.Play();
			}
		}
	}
	protected void pause()
	{
		if (mMediaPlayer.Control != null && !mMediaPlayer.Control.IsPaused())
		{
			mMediaPlayer.Pause();
		}
	}
	protected void stop(bool autoHide = true)
	{
		// 只有在播放时才能停止,停止并不是真正地停止视频,只是将视频暂停,并且移到视频开始位置
		if (mMediaPlayer.Control != null)
		{
			if (mMediaPlayer.Control.IsPlaying())
			{
				mMediaPlayer.Rewind(true);
			}
			if (autoHide)
			{
				mTexture.enabled = false;
			}
		}
	}
}