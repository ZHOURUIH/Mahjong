using System;
using System.Collections;
using System.Collections.Generic;

public class SCOtherPlayerLeaveRoom : SocketPacket
{
	public INT mPlayerGUID = new INT();		// 离开房间的玩家GUID
	public SCOtherPlayerLeaveRoom(PACKET_TYPE type)
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
		Character player = mCharacterManager.getCharacterByGUID(mPlayerGUID.mValue);
		GameScene gameScene = mGameSceneManager.getCurScene();
		if (gameScene.getType() != GAME_SCENE_TYPE.GST_MAHJONG)
		{
			return;
		}
		MahjongScene mahjongScene = gameScene as MahjongScene;
		CommandRoomLeave cmdLeave = mCommandSystem.newCmd<CommandRoomLeave>();
		cmdLeave.mCharacter = player;
		mCommandSystem.pushCommand(cmdLeave, mahjongScene.getRoom());
	}
}