using UnityEngine;
using System.Collections;

public class CommandGameSceneStopAudio : Command
{
	public override void init()
	{
		base.init();
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
		return this.GetType().ToString();
	}
}