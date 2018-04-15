using System;
using System.Collections;
using System.Collections.Generic;

public class SCNotifyBanker : SocketPacket
{
	public INT mPlayerGUID = new INT();		// 庄家ID
	public SCNotifyBanker(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mPlayerGUID);
	}
	public override void execute()
	{
		// 通知房间中的所有玩家
		GameScene gameScene = mGameSceneManager.getCurScene();
		if(gameScene.getSceneType() != GAME_SCENE_TYPE.GST_MAHJONG)
		{
			return;
		}
		Room room = (gameScene as MahjongScene).getRoom();
		Dictionary<int, Character> playerList = room.getPlayerList();
		foreach(var item in playerList)
		{
			CommandCharacterNotifyBanker cmdBanker = newCmd(out cmdBanker);
			cmdBanker.mBanker = (item.Value.getCharacterData().mGUID == mPlayerGUID.mValue);
			pushCommand(cmdBanker, item.Value);
		}
	}
}