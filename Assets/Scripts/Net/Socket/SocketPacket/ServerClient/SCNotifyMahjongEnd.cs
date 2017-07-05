using System;
using System.Collections;
using System.Collections.Generic;

public class SCNotifyMahjongEnd : SocketPacket
{
	INTS mCharacterGUIDList = new INTS(CommonDefine.MAX_PLAYER_COUNT);
	INTS mMoneyDeltaList = new INTS(CommonDefine.MAX_PLAYER_COUNT);
	public SCNotifyMahjongEnd(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mCharacterGUIDList);
		pushParam(mMoneyDeltaList);
	}
	public override void execute()
	{
		GameScene gameScene = mGameSceneManager.getCurScene();
		if (gameScene.getType() != GAME_SCENE_TYPE.GST_MAHJONG)
		{
			return;
		}
		MahjongScene mahjongScene = gameScene as MahjongScene;
		Room room = mahjongScene.getRoom();
		Dictionary<Character, int> moneyDeltaList = new Dictionary<Character, int>();
		for (int i = 0; i < CommonDefine.MAX_PLAYER_COUNT; ++i)
		{
			moneyDeltaList.Add(mCharacterManager.getCharacterByGUID(mCharacterGUIDList.mValue[i]), mMoneyDeltaList.mValue[i]);
		}
		CommandRoomEnd cmdEnd = mCommandSystem.newCmd<CommandRoomEnd>();
		cmdEnd.mMoneyDeltaList = moneyDeltaList;
		mCommandSystem.pushCommand(cmdEnd, room);
	}
}