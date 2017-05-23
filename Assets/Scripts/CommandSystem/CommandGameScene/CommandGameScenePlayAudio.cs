using UnityEngine;
using System.Collections;

public class CommandGameScenePlayAudio : Command
{
	public SOUND_DEFINE mSound;
	public string		mSoundFileName;
	public bool			mLoop = false;
	public float		mVolume = 1.0f;
	public CommandGameScenePlayAudio(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
	public override void execute()
	{
		GameScene gameScene = (mReceiver) as GameScene;
		GameSceneComponentAudio audioComponent = gameScene.getFirstActiveComponent<GameSceneComponentAudio>();
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
		return this.GetType().ToString() + " : sound : " + mSound + ", name : " + soundName + ", loop : " + mLoop + ", volume : " + mVolume;
	}
}