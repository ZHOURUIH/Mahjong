using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainSceneLoading : SceneProcedure
{
	public MainSceneLoading()
	{ }
	public MainSceneLoading(PROCEDURE_TYPE type, GameScene gameScene)
		:
	base(type, gameScene)
	{ }
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		// 加载所有布局
		LayoutTools.LOAD_LAYOUT_SHOW(LAYOUT_TYPE.LT_MAIN_FRAME, 0);
		LayoutTools.LOAD_LAYOUT_SHOW(LAYOUT_TYPE.LT_CHARACTER, 1);
		LayoutTools.LOAD_LAYOUT_SHOW(LAYOUT_TYPE.LT_BILLBOARD, 1);
		LayoutTools.LOAD_LAYOUT_SHOW(LAYOUT_TYPE.LT_ROOM_MENU, 1);

		CommandGameSceneChangeProcedure cmd = new CommandGameSceneChangeProcedure(true, true);
		cmd.mProcedure = PROCEDURE_TYPE.PT_MAIN_RUNNING;
		mCommandSystem.pushDelayCommand(cmd, mGameScene);

		// 创建玩家自己
		CommandCharacterManagerCreateCharacter cmdCreate = new CommandCharacterManagerCreateCharacter();
		cmdCreate.mCharacterType = CHARACTER_TYPE.CT_MYSELF;
		cmdCreate.mName = "自己";
		mCommandSystem.pushCommand(cmdCreate, mCharacterManager);
		CharacterMyself myself = cmdCreate.mResultCharacter as CharacterMyself;
		CharacterData data = myself.getCharacterData();
		data.mGUID = 0;
		data.mMoney = 100;
		data.mHead = 0;
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		;
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		;
	}
}