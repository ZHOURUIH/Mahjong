using System;
using System.Collections;
using System.Collections.Generic;

public class SCPlayerHu : SocketPacket
{
	public INTS mHuPlayerGUID = new INTS(GameDefine.MAX_PLAYER_COUNT - 1);
	public BYTES mHuList = new BYTES(GameDefine.MAX_HU_COUNT * (GameDefine.MAX_PLAYER_COUNT - 1));
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
		pushParam(mHuList);
	}
	public override void execute()
	{
		// 通知房间保存胡牌结果
		GameScene gameScene = mGameSceneManager.getCurScene();
		if (gameScene.getSceneType() != GAME_SCENE_TYPE.GST_MAHJONG)
		{
			return;
		}
		MahjongScene mahjongScene = gameScene as MahjongScene;
		Room room = mahjongScene.getRoom();
		for(int i = 0; i < GameDefine.MAX_PLAYER_COUNT - 1; ++i)
		{
			List<HU_TYPE> huList = new List<HU_TYPE>();
			for(int j = 0; j < GameDefine.MAX_HU_COUNT; ++i)
			{
				HU_TYPE huType = (HU_TYPE)mHuList.mValue[i * GameDefine.MAX_HU_COUNT + j];
				if(huType == HU_TYPE.HT_MAX)
				{
					break;
				}
				huList.Add(huType);
			}
			if(huList.Count > 0)
			{
				CommandRoomPlayerHu cmdHu = newCmd(out cmdHu);
				cmdHu.mHuList = huList;
				cmdHu.mPlayer = mCharacterManager.getCharacter(mHuPlayerGUID.mValue[i]);
				pushCommand(cmdHu, room);
			}
		}
	}
}