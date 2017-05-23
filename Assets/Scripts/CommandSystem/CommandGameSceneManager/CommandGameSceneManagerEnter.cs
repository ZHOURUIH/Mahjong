using UnityEngine;
using System.Collections;

public class CommandGameSceneManagerEnter : Command
{
	public GAME_SCENE_TYPE mSceneType;
	public CommandGameSceneManagerEnter(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
	public override void execute()
	{
		GameSceneManager sceneManager = mReceiver as GameSceneManager;
		sceneManager.enterScene(mSceneType);
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : scene : " + mSceneType;
	}
}