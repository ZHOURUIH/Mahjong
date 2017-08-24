using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MahjongSceneEnding : SceneProcedure
{
	public MahjongSceneEnding()
	{ }
	public MahjongSceneEnding(PROCEDURE_TYPE type, GameScene gameScene)
		:
	base(type, gameScene)
	{ }
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		// 设置本局结果
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_GAME_ENDING);
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_GAME_ENDING);
		// 清空房间中所有玩家的麻将数据
		MahjongScene mahjongScene = mGameScene as MahjongScene;
		mahjongScene.getRoom().clearAllPlayerMahjongData();
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		;
	}
}