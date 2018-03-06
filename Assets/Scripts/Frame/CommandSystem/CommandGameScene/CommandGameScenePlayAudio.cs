using UnityEngine;
using System.Collections;

public class CommandGameScenePlayAudio : Command
{
	public SOUND_DEFINE mSound;
	public string mSoundFileName;
	public bool mLoop = false;
	public float mVolume = 1.0f;
	public bool mUseVolumeCoe = true;       // 是否启用数据表格中的音量系数
	public override void init()
	{
		base.init();
		mSound = SOUND_DEFINE.SD_MAX;
		mSoundFileName = "";
		mLoop = false;
		mVolume = 1.0f;
		mUseVolumeCoe = true;
	}
	public override void execute()
	{
		GameScene gameScene = mReceiver as GameScene;
		GameSceneComponentAudio audioComponent = gameScene.getFirstActiveComponent<GameSceneComponentAudio>();
		if (audioComponent != null)
		{
			string soundName = mSound != SOUND_DEFINE.SD_MAX ? mAudioManager.getAudioName(mSound) : mSoundFileName;
			if (mUseVolumeCoe)
			{
				mVolume *= mAudioManager.getVolumeScale(mSound);
			}
			audioComponent.play(soundName, mLoop, mVolume);
		}
	}
	public override string showDebugInfo()
	{
		string soundName = mSound != SOUND_DEFINE.SD_MAX ? mAudioManager.getAudioName(mSound) : mSoundFileName;
		return this.GetType().ToString() + " : sound : " + mSound + ", name : " + soundName + ", loop : " + mLoop + ", volume : " + mVolume;
	}
}