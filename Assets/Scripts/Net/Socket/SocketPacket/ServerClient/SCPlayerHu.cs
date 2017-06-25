using System;
using System.Collections;
using System.Collections.Generic;

public class SCPlayerHu : SocketPacket
{
	public INT mDroppedPlayerGUID = new INT();
	public BYTE mMahjong = new BYTE();
	public BYTES mHuList = new BYTES(CommonDefine.MAX_HU_COUNT);
	public SCPlayerHu(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mDroppedPlayerGUID);
		pushParam(mMahjong);
		pushParam(mHuList);
	}
	public override void execute()
	{
		// 先设置结果
		GameScene gameScene = mGameSceneManager.getCurScene();
		if(gameScene.getType() != GAME_SCENE_TYPE.GST_MAHJONG)
		{
			return;
		}
		MahjongScene mahjongScene = gameScene as MahjongScene;
		Room room = mahjongScene.getRoom();
		CommandRoomEnd cmdEnd = new CommandRoomEnd();
		cmdEnd.mHuPlayer = mCharacterManager.getMyself();
		cmdEnd.mMahjong = (MAHJONG)mMahjong.mValue;
		cmdEnd.mHuList = new List<HU_TYPE>();
		int huCount = mHuList.mValue.Length;
		for (int i = 0; i < huCount; ++i)
		{
			HU_TYPE huType = (HU_TYPE)mHuList.mValue[i];
			if(huType == HU_TYPE.HT_NONE)
			{
				break;
			}
			cmdEnd.mHuList.Add(huType);
		}
		mCommandSystem.pushCommand(cmdEnd, room);
	}
}