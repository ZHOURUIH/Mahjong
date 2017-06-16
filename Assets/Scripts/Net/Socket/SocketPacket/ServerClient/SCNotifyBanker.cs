using System;
using System.Collections;
using System.Collections.Generic;

public class SCNotifyBanker : SocketPacket
{
	protected int mPlayerGUID;		// 庄家ID
	public SCNotifyBanker(PACKET_TYPE type)
		:
		base(type)
	{
		;
	}
	public override void read(byte[] data)
	{
		int index = 0;
		mPlayerGUID = BinaryUtility.readInt(data, ref index);
	}
	public override void write(byte[] data)
	{
		int index = 0;
		BinaryUtility.writeInt(data, ref index, mPlayerGUID);
	}
	public override int getSize()
	{
		return sizeof(int);
	}
	public override void execute()
	{
		// 通知房间中的所有玩家
		GameScene gameScene = mGameSceneManager.getCurScene();
		if(gameScene.getType() != GAME_SCENE_TYPE.GST_MAHJONG)
		{
			return;
		}
		Room room = (gameScene as MahjongScene).getRoom();
		Dictionary<int, Character> playerList = room.getPlayerList();
		foreach(var item in playerList)
		{
			CommandCharacterNotifyBanker cmdBanker = new CommandCharacterNotifyBanker();
			cmdBanker.mBanker = (item.Value.getCharacterData().mGUID == mPlayerGUID);
			mCommandSystem.pushCommand(cmdBanker, item.Value);
		}
	}
}