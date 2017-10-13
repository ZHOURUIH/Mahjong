using UnityEngine;
using System.Collections;

public class CommandGameSceneStopAudio : Command
{
	public SOUND_DEFINE mSound;
	public override void init()
	{
		base.init();
		mSound = SOUND_DEFINE.SD_MAX;
	}
	public override void execute()
	{
		GameScene gameScene = (mReceiver) as GameScene;
		GameSceneComponentAudio audioComponent = gameScene.getFirstActiveComponent<GameSceneComponentAudio>();
		if (audioComponent != null)
		{
			audioComponent.stop();
		}
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : sound : " + mSound;
	}
}