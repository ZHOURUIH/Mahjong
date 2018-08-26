using System;
using System.Collections;
using System.Collections.Generic;

public class SCStartGame : SocketPacket
{
	protected BYTES mDice = new BYTES(2);
	protected INT mPlayerCount = new INT();
	// 以下数组是将二维数组合成了一维数组
	protected INTS mPlayerIDList = new INTS(GameDefine.MAX_PLAYER_COUNT);
	protected BYTES mHandInList = new BYTES(GameDefine.MAX_PLAYER_COUNT * GameDefine.MAX_HAND_IN_COUNT);
	protected BYTES mHuaList = new BYTES(GameDefine.MAX_PLAYER_COUNT * GameDefine.MAX_HUA_COUNT);
	public SCStartGame(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mDice);
	}
	public override void execute()
	{
		GameScene gameScene = mGameSceneManager.getCurScene();
		if(gameScene.getSceneType() != GAME_SCENE_TYPE.GST_MAHJONG)
		{
			return;
		}
		List<int> playerIDList = new List<int>();
		List<List<MAHJONG>> handInList = new List<List<MAHJONG>>();
		List<List<MAHJONG>> huaList = new List<List<MAHJONG>>();
		for(int i = 0; i < mPlayerCount.mValue; ++i)
		{
			playerIDList.Add(mPlayerIDList.mValue[i]);
			handInList.Add(new List<MAHJONG>());
			for(int j = 0; j < GameDefine.MAX_HAND_IN_COUNT; ++j)
			{
				MAHJONG mah = (MAHJONG)mHandInList.mValue[GameDefine.MAX_HAND_IN_COUNT * i + j];
				if(mah == MAHJONG.M_MAX)
				{
					break;
				}
				handInList[i].Add(mah);
			}
			huaList.Add(new List<MAHJONG>());
			for (int j = 0; j < GameDefine.MAX_HAND_IN_COUNT; ++j)
			{
				MAHJONG mah = (MAHJONG)mHuaList.mValue[GameDefine.MAX_HUA_COUNT * i + j];
				if (mah == MAHJONG.M_MAX)
				{
					break;
				}
				huaList[i].Add(mah);
			}
		}
		mMahjongSystem.startMahjong(playerIDList, handInList, huaList);

		// 跳转到掷骰子流程
		CommandGameSceneChangeProcedure cmd = newCmd(out cmd);
		cmd.mProcedure = PROCEDURE_TYPE.PT_MAHJONG_RUNNING_DICE;
		pushCommand(cmd, mGameSceneManager.getCurScene());

		// 通知麻将场景开始掷骰子
		CommandMahjongSceneNotifyDice cmdDice = newCmd(out cmdDice);
		cmdDice.mDice = mDice.mValue;
		pushCommand(cmdDice, gameScene);
	}
}