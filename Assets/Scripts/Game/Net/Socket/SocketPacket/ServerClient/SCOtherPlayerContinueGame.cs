using System;
using System.Collections;
using System.Collections.Generic;

public class SCOtherPlayerContinueGame : SocketPacket
{
	public INT mOtherPlayerGUID = new INT();
	public BOOL mBanker = new BOOL();
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
		CommandRoomJoin cmdJoin = mCommandSystem.newCmd<CommandRoomJoin>();
		cmdJoin.mCharacter = player;
		mCommandSystem.pushCommand(cmdJoin, room);

		CommandCharacterNotifyBanker cmdBanker = mCommandSystem.newCmd<CommandCharacterNotifyBanker>();
		cmdBanker.mBanker = mBanker.mValue;
		mCommandSystem.pushCommand(cmdBanker, player);
	}
}