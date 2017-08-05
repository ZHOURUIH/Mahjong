using System;
using System.Collections;
using System.Collections.Generic;

public class SCOtherPlayerContinueGame : SocketPacket
{
	public INT mOtherPlayerGUID = new INT();
	public SCOtherPlayerContinueGame(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mOtherPlayerGUID);
	}
	public override void execute()
	{
		GameScene gameScene = mGameSceneManager.getCurScene();
		if(!gameScene.atProcedure(PROCEDURE_TYPE.PT_MAHJONG_WAITING))
		{
			return;
		}
		MahjongScene mahjongScene = gameScene as MahjongScene;
		Room room = mahjongScene.getRoom();
		CommandRoomJoin cmdJoin = mCommandSystem.newCmd<CommandRoomJoin>();
		cmdJoin.mCharacter = mCharacterManager.getCharacterByGUID(mOtherPlayerGUID.mValue);
		mCommandSystem.pushCommand(cmdJoin, room);
	}
}