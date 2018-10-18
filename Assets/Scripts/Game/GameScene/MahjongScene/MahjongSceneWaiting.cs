using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MahjongSceneWaiting : SceneProcedure
{
	public MahjongSceneWaiting(PROCEDURE_TYPE type, GameScene gameScene)
		:
	base(type, gameScene)
	{ }
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		// 显示布局
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_ALL_CHARACTER_INFO);
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_MAHJONG_BACK_FRAME);
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_MAHJONG_FRAME);

		// 通知房间开始等待玩家加入
		MahjongScene mahjongScene = mGameScene as MahjongScene;
		mahjongScene.getRoom().notifyStartWait();

		// 设置显示房间号
		CharacterMyself myself = mCharacterManager.getMyself();
		mScriptMahjongFrame.setRoomID(myself.getCharacterData().mRoomID);
		mScriptMahjongFrame.notifyInfo("正在等待其他玩家准备");
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		mScriptMahjongFrame.notifyInfo("");
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		// 在房间中按1键添加一个机器人
		if(mInputManager.getKeyCurrentDown(KeyCode.Alpha1))
		{
			mSocketManager.sendMessage<CSAddMahjongRobot>();
		}
	}
}