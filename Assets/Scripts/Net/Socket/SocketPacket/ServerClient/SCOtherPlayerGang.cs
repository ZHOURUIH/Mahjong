using System;
using System.Collections;
using System.Collections.Generic;

public class SCOtherPlayerGang : SocketPacket
{
	public INT mOtherPlayerGUID = new INT();
	public INT mDroppedPlayerGUID = new INT();
	public BYTE mMahjong = new BYTE();
	public SCOtherPlayerGang(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mOtherPlayerGUID);
		pushParam(mDroppedPlayerGUID);
		pushParam(mMahjong);
	}
	public override void execute()
	{
		GameScene gameScene = mGameSceneManager.getCurScene();
		if(gameScene.getType() != GAME_SCENE_TYPE.GST_MAHJONG)
		{
			return;
		}
		CommandCharacterGang cmdGang = new CommandCharacterGang();
		cmdGang.mDroppedPlayer = mCharacterManager.getCharacterByGUID(mDroppedPlayerGUID.mValue);
		cmdGang.mMahjong = (MAHJONG)mMahjong.mValue;
		mCommandSystem.pushCommand(cmdGang, mCharacterManager.getCharacterByGUID(mOtherPlayerGUID.mValue));
	}
}