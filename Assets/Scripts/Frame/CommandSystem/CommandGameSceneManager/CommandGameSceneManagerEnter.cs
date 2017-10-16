using UnityEngine;
using System.Collections;

public class CommandGameSceneManagerEnter : Command
{
	public GAME_SCENE_TYPE mSceneType;
	public override void init()
	{
		base.init();
		mSceneType = GAME_SCENE_TYPE.GST_MAX;
	}
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