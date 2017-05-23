using UnityEngine;
using System.Collections;

public class CommandGameSceneStopAudio : Command
{
	public SOUND_DEFINE mSound;
	public CommandGameSceneStopAudio(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
	public override void execute()
	{
		GameScene gameScene = (mReceiver) as GameScene;
		GameSceneComponentAudio audioComponent = gameScene.getFirstActiveComponent<GameSceneComponentAudio>();
		if (audioComponent != null)
		{
			audioComponent.stop(mSound);
		}
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : sound : " + mSound;
	}
}