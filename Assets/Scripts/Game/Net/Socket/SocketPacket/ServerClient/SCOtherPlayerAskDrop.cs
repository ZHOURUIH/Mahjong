using System;
using System.Collections;
using System.Collections.Generic;

public class SCOtherPlayerAskDrop : SocketPacket
{
	public INT mOtherPlayerGUID = new INT();
	public SCOtherPlayerAskDrop(PACKET_TYPE type)
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
		CharacterOther otherPlayer = mCharacterManager.getCharacter(mOtherPlayerGUID.mValue) as CharacterOther;
		ScriptMahjongFrame mahjongFrame = mLayoutManager.getScript(LAYOUT_TYPE.LT_MAHJONG_FRAME) as ScriptMahjongFrame;
		mahjongFrame.notifyInfo("正在等待玩家" + otherPlayer.getName() + "打出一张牌");
	}
}