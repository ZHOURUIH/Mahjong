using UnityEngine;
using System.Collections;

// 开局时的拿牌
public class CommandCharacterGetStart : Command
{
	public MAHJONG mMahjong;
	public CommandCharacterGetStart(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
	public override void execute()
	{
		Character character = mReceiver as Character;
		character.getMahjongStart(mMahjong);
		// 通知布局
		ScriptMahjongHandIn handIn = mLayoutManager.getScript(LAYOUT_TYPE.LT_MAHJONG_HAND_IN) as ScriptMahjongHandIn;
		handIn.notifyGetMahjongStart(character.getCharacterData().mPosition, mMahjong);
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo() + " : mahjong : " + mMahjong;
	}
}