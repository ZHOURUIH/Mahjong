using System;
using System.Collections;
using System.Collections.Generic;

public class SCOtherPlayerContinueGame : SocketPacket
{
	public INT mOtherPlayerGUID = new INT();
	public BOOL mBanker = new BOOL();
	public SCOtherPlayerContinueGame(PACKET_TYPE type)
		: base(type) { }
	protected override void fillParams()
	{
		pushParam(mOtherPlayerGUID);
		pushParam(mBanker);
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
		Character player = mCharacterManager.getCharacter(mOtherPlayerGUID.mValue);
		CommandRoomJoin cmdJoin = newCmd(out cmdJoin);
		cmdJoin.mCharacter = player;
		pushCommand(cmdJoin, room);

		CommandCharacterNotifyBanker cmdBanker = newCmd(out cmdBanker);
		cmdBanker.mBanker = mBanker.mValue;
		pushCommand(cmdBanker, player);
	}
}