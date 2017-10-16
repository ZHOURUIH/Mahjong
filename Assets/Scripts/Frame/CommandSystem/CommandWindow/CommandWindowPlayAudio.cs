using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CommandWindowPlayAudio : Command
{
	public static Dictionary<SOUND_DEFINE, float> mVolumeCoe;
	public SOUND_DEFINE mSound;
	public string mSoundFileName;
	public bool mLoop = false;
	public float mVolume = 1.0f;
	public bool mUseVolumeCoe = true;		// 是否启用数据表格中的音量系数
	public CommandWindowPlayAudio()
	{ 
		if(mVolumeCoe == null)
		{
			mVolumeCoe = new Dictionary<SOUND_DEFINE, float>();
			int dataCount = mDataBase.getDataCount(DATA_TYPE.DT_GAME_SOUND);
			for(int i = 0; i < dataCount; ++i)
			{
				DataGameSound gameSound = mDataBase.queryData(DATA_TYPE.DT_GAME_SOUND, i) as DataGameSound;
				if (!mVolumeCoe.ContainsKey((SOUND_DEFINE)gameSound.mSoundID))
				{
					mVolumeCoe.Add((SOUND_DEFINE)gameSound.mSoundID, gameSound.mVolumeCoe);
				}
			}
		}
	}
	public override void init()
	{
		base.init();
		mSound = SOUND_DEFINE.SD_MIN;
		mSoundFileName = "";
		mLoop = false;
		mVolume = 1.0f;
		mUseVolumeCoe = true;
	}
	public override void execute()
	{
		txUIObject window = mReceiver as txUIObject;
		WindowComponentAudio audioComponent = window.getFirstActiveComponent<WindowComponentAudio>();
		if (audioComponent != null)
		{
			string soundName;
			if (mSound != SOUND_DEFINE.SD_MIN)
			{
				soundName = ComponentAudio.getAudioName(mSound);
			}
			else
			{
				soundName = mSoundFileName;
			}
			if (mUseVolumeCoe && mVolumeCoe.ContainsKey(mSound))
			{
				mVolume *= mVolumeCoe[mSound];
			}
			audioComponent.play(soundName, mLoop, mVolume);
		}
	}
	public override string showDebugInfo()
	{
		string soundName;
		if (mSound != SOUND_DEFINE.SD_MIN)
		{
			soundName = ComponentAudio.getAudioName(mSound);
		}
		else
		{
			soundName = mSoundFileName;
		}
		return this.GetType().ToString() + " : sound : " + mSound + ", name : " + soundName + ", loop : " + mLoop + ", volume : " + mVolume + ", sound file name : " + mSoundFileName + ", use volume coe : " + mUseVolumeCoe;
	}
}