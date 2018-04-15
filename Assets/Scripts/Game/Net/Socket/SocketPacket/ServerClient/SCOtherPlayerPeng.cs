using System;
using System.Collections;
using System.Collections.Generic;

public class SCOtherPlayerPeng : SocketPacket
{
	public INT mOtherPlayerGUID = new INT();
	public INT mDroppedPlayerGUID = new INT();
	public BYTE mMahjong = new BYTE();
	public SCOtherPlayerPeng(PACKET_TYPE type)
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
		if(gameScene.getSceneType() != GAME_SCENE_TYPE.GST_MAHJONG)
		{
			return;
		}
		// 清空提示信息
		mScriptMahjongFrame.notifyInfo("");
		CommandCharacterPeng cmdGang = newCmd(out cmdGang);
		cmdGang.mDroppedPlayer = mCharacterManager.getCharacter(mDroppedPlayerGUID.mValue);
		cmdGang.mMahjong = (MAHJONG)mMahjong.mValue;
		pushCommand(cmdGang, mCharacterManager.getCharacter(mOtherPlayerGUID.mValue));
	}
}