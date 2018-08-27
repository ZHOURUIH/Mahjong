using System;
using System.Collections;
using System.Collections.Generic;

public class SCOtherPlayerBackToMahjongHall : SocketPacket
{
	public INT mOtherPlayerGUID = new INT();
	public SCOtherPlayerBackToMahjongHall(PACKET_TYPE type)
		: base(type) { }
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
		CommandRoomLeave cmdLeave = newCmd(out cmdLeave);
		cmdLeave.mCharacter = mCharacterManager.getCharacter(mOtherPlayerGUID.mValue);
		pushCommand(cmdLeave, room);
	}
}