using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MahjongSceneRunning : SceneProcedure
{
	public MahjongSceneRunning()
	{ }
	public MahjongSceneRunning(PROCEDURE_TYPE type, GameScene gameScene)
		:
	base(type, gameScene)
	{ }
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		// 游戏开始时取消所有玩家的准备标记
		MahjongScene mahjongScene = mGameScene as MahjongScene;
		Room room = mahjongScene.getRoom();
		Dictionary<int, Character> playerList = room.getPlayerList();
		foreach(var item in playerList)
		{
			item.Value.getCharacterData().mReady = false;
		}
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_MAHJONG_FRAME);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_ALL_CHARACTER_INFO);
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		;
	}
}