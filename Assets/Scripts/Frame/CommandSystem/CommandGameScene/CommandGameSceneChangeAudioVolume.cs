using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CommandGameSceneChangeAudioVolume : Command
{
	public float mVolume;
	public override void init()
	{
		base.init();
		mVolume = 0.0f;
	}
	public override void execute()
	{
		GameScene gameScene = mReceiver as GameScene;
		GameSceneComponentAudio audio = gameScene.getFirstActiveComponent<GameSceneComponentAudio>();
		if(audio != null)
		{
			audio.setVolume(mVolume);
		}
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : volume : " + mVolume;
	}
}