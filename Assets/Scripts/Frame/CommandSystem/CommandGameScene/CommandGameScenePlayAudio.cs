using UnityEngine;
using System.Collections;

public class CommandGameScenePlayAudio : Command
{
	public SOUND_DEFINE mSound;
	public string mSoundFileName;
	public bool mLoop = false;
	public float mVolume = 1.0f;
	public override void init()
	{
		base.init();
		mSound = SOUND_DEFINE.SD_MAX;
		mSoundFileName = "";
		mLoop = false;
		mVolume = 1.0f;
	}
	public override void execute()
	{
		GameScene gameScene = mReceiver as GameScene;
		GameSceneComponentAudio audioComponent = gameScene.getFirstActiveComponent<GameSceneComponentAudio>();
		if (audioComponent != null)
		{
			string soundName = mSound != SOUND_DEFINE.SD_MAX ? ComponentAudio.getAudioName(mSound) : mSoundFileName;
			audioComponent.play(soundName, mLoop, mVolume);
		}
	}
	public override string showDebugInfo()
	{
		string soundName = mSound != SOUND_DEFINE.SD_MAX ? ComponentAudio.getAudioName(mSound) : mSoundFileName;
		return this.GetType().ToString() + " : sound : " + mSound + ", name : " + soundName + ", loop : " + mLoop + ", volume : " + mVolume;
	}
}