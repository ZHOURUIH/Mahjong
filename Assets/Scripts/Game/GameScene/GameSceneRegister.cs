using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameSceneRegister : GameBase
{
	public void registerAllGameScene()
	{
		registeGameScene(typeof(StartScene), GAME_SCENE_TYPE.GST_START);
		registeGameScene(typeof(MainScene), GAME_SCENE_TYPE.GST_MAIN);
		registeGameScene(typeof(MahjongScene), GAME_SCENE_TYPE.GST_MAHJONG);
	}
	//-------------------------------------------------------------------------------------------------------------
	protected void registeGameScene(Type scene, GAME_SCENE_TYPE type)
	{
		mGameSceneManager.registeGameScene(scene, type);
	}
}