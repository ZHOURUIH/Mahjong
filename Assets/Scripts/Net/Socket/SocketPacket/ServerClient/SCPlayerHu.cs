using System;
using System.Collections;
using System.Collections.Generic;

public class SCPlayerHu : SocketPacket
{
	public INTS mHuPlayerGUID = new INTS(CommonDefine.MAX_PLAYER_COUNT - 1);
	public INT mDroppedPlayerGUID = new INT();
	public BYTE mMahjong = new BYTE();
	public BYTES mHuList = new BYTES(CommonDefine.MAX_HU_COUNT * (CommonDefine.MAX_PLAYER_COUNT - 1));
	public SCPlayerHu(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mHuPlayerGUID);
		pushParam(mDroppedPlayerGUID);
		pushParam(mMahjong);
		pushParam(mHuList);
	}
	public override void execute()
	{
		// 通知房间保存胡牌结果
		GameScene gameScene = mGameSceneManager.getCurScene();
		if (gameScene.getType() != GAME_SCENE_TYPE.GST_MAHJONG)
		{
			return;
		}
		MahjongScene mahjongScene = gameScene as MahjongScene;
		Room room = mahjongScene.getRoom();
		for(int i = 0; i < CommonDefine.MAX_PLAYER_COUNT - 1; ++i)
		{
			List<HU_TYPE> huList = new List<HU_TYPE>();
			for(int j = 0; j < CommonDefine.MAX_HU_COUNT; ++i)
			{
				HU_TYPE huType = (HU_TYPE)mHuList.mValue[i * CommonDefine.MAX_HU_COUNT + j];
				if(huType == HU_TYPE.HT_NONE)
				{
					break;
				}
				huList.Add(huType);
			}
			CommandRoomPlayerHu cmdHu = new CommandRoomPlayerHu();
			cmdHu.mHuPlayer = mCharacterManager.getCharacterByGUID(mHuPlayerGUID.mValue[i]);
			cmdHu.mDroppedPlayer = mCharacterManager.getCharacterByGUID(mDroppedPlayerGUID.mValue);
			cmdHu.mHuList = huList;
			cmdHu.mMahjong = (MAHJONG)mMahjong.mValue;
		}
	}
}