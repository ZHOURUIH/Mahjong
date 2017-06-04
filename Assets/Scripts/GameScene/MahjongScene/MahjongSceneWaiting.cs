using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MahjongSceneWaiting : SceneProcedure
{
	public MahjongSceneWaiting()
	{ }
	public MahjongSceneWaiting(PROCEDURE_TYPE type, GameScene gameScene)
		:
	base(type, gameScene)
	{ }
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_ALL_CHARACTER_INFO);
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_MAHJONG_BACK_FRAME);
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_ADD_PLAYER);
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_MAHJONG_FRAME);
		// 通知麻将系统开始新的一局
		CommandMahjongSystemState cmdState = new CommandMahjongSystemState();
		cmdState.mPlayState = MAHJONG_PLAY_STATE.MPS_WAITING;
		mCommandSystem.pushCommand(cmdState, mMahjongSystem);
		// 进入麻将场景后自己自动加入到本局麻将中,并且设置自己为庄家
		CharacterMyself myself = mCharacterManager.getMyself();
		myself.getCharacterData().mBanker = true;
		CommandMahjongSystemJoin cmdJoin = new CommandMahjongSystemJoin();
		cmdJoin.mCharacter = myself;
		mCommandSystem.pushCommand(cmdJoin, mMahjongSystem);

		// 设置显示房间号
		ScriptMahjongFrame mahjongFrame = mLayoutManager.getScript(LAYOUT_TYPE.LT_MAHJONG_FRAME) as ScriptMahjongFrame;
		mahjongFrame.setRoomID(myself.getCharacterData().mRoomID);
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_ADD_PLAYER);
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		if (Input.GetKeyDown(KeyCode.Alpha0))
		{
			CharacterMyself myself = mCharacterManager.getMyself();
			CommandCharacterReady cmd = new CommandCharacterReady();
			mCommandSystem.pushCommand(cmd, myself);
		}
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			string name = "player0";
			Character character = mCharacterManager.getCharacter(name);
			// 未加入,则创建角色并加入游戏
			if (character == null)
			{
				CommandCharacterManagerCreateCharacter cmdCreate = new CommandCharacterManagerCreateCharacter();
				cmdCreate.mName = name;
				cmdCreate.mCharacterType = CHARACTER_TYPE.CT_OTHER;
				mCommandSystem.pushCommand(cmdCreate, mCharacterManager);
				character = cmdCreate.mResultCharacter;
				CharacterData data = character.getCharacterData();
				data.mGUID = 1;
				data.mMoney = 200;
				data.mHead = 1;
				CommandMahjongSystemJoin cmdJoin = new CommandMahjongSystemJoin();
				cmdJoin.mCharacter = character;
				mCommandSystem.pushCommand(cmdJoin, mMahjongSystem);
			}
			// 已加入,则准备完毕
			else
			{
				CommandCharacterReady cmd = new CommandCharacterReady();
				mCommandSystem.pushCommand(cmd, character);
			}
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			string name = "player1";
			Character character = mCharacterManager.getCharacter(name);
			// 未加入,则创建角色并加入游戏
			if (character == null)
			{
				CommandCharacterManagerCreateCharacter cmdCreate = new CommandCharacterManagerCreateCharacter();
				cmdCreate.mName = name;
				cmdCreate.mCharacterType = CHARACTER_TYPE.CT_OTHER;
				mCommandSystem.pushCommand(cmdCreate, mCharacterManager);
				character = cmdCreate.mResultCharacter;
				CharacterData data = character.getCharacterData();
				data.mGUID = 2;
				data.mMoney = 200;
				data.mHead = 2;
				CommandMahjongSystemJoin cmdJoin = new CommandMahjongSystemJoin();
				cmdJoin.mCharacter = character;
				mCommandSystem.pushCommand(cmdJoin, mMahjongSystem);
			}
			// 已加入,则准备完毕
			else
			{
				CommandCharacterReady cmd = new CommandCharacterReady();
				mCommandSystem.pushCommand(cmd, character);
			}
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			string name = "player2";
			Character character = mCharacterManager.getCharacter(name);
			// 未加入,则创建角色并加入游戏
			if (character == null)
			{
				CommandCharacterManagerCreateCharacter cmdCreate = new CommandCharacterManagerCreateCharacter();
				cmdCreate.mName = name;
				cmdCreate.mCharacterType = CHARACTER_TYPE.CT_OTHER;
				mCommandSystem.pushCommand(cmdCreate, mCharacterManager);
				character = cmdCreate.mResultCharacter;
				CharacterData data = character.getCharacterData();
				data.mGUID = 3;
				data.mMoney = 200;
				data.mHead = 3;
				CommandMahjongSystemJoin cmdJoin = new CommandMahjongSystemJoin();
				cmdJoin.mCharacter = character;
				mCommandSystem.pushCommand(cmdJoin, mMahjongSystem);
			}
			// 已加入,则准备完毕
			else
			{
				CommandCharacterReady cmd = new CommandCharacterReady();
				mCommandSystem.pushCommand(cmd, character);
			}
		}
	}
	public void notifyAllPlayerReady()
	{
		CommandGameSceneChangeProcedure cmd = new CommandGameSceneChangeProcedure();
		cmd.mProcedure = PROCEDURE_TYPE.PT_MAHJONG_RUNNING_DICE;
		mCommandSystem.pushCommand(cmd, mGameScene);
	}
}