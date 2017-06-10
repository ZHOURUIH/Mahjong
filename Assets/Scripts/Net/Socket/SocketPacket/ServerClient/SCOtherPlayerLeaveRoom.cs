using System;
using System.Collections;
using System.Collections.Generic;

public class SCOtherPlayerLeaveRoom : SocketPacket
{
	protected int mGUID;		// 离开房间的玩家GUID
	public SCOtherPlayerLeaveRoom(PACKET_TYPE type)
		:
		base(type)
	{
		;
	}
	public override void read(byte[] data)
	{
		int index = 0;
		mGUID = BinaryUtility.readInt(data, ref index);
	}
	public override void write(byte[] data)
	{
		int index = 0;
		BinaryUtility.writeInt(data, ref index, mGUID);
	}
	public override int getSize()
	{
		return sizeof(int);
	}
	public override void execute()
	{
		Character player = mCharacterManager.getCharacterByGUID(mGUID);
		GameScene gameScene = mGameSceneManager.getCurScene();
		if (gameScene.getType() != GAME_SCENE_TYPE.GST_MAHJONG)
		{
			return;
		}
		MahjongScene mahjongScene = gameScene as MahjongScene;
		CommandRoomLeave cmdLeave = new CommandRoomLeave();
		cmdLeave.mCharacter = player;
		mCommandSystem.pushCommand(cmdLeave, mahjongScene.getRoom());
	}
}