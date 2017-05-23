using UnityEngine;
using System.Collections;

public class CommandCharacterPeng : Command
{
	public Character mDroppedPlayer;
	public MAHJONG mMahjong;
	public CommandCharacterPeng(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
	public override void execute()
	{
		Character character = (mReceiver) as Character;
		character.pengMahjong(mMahjong);
		
		// 从已经打出的牌中拿走已经碰的那张牌
		CommandCharacterTakeDrop cmdTakeDrop = new CommandCharacterTakeDrop();
		mCommandSystem.pushCommand(cmdTakeDrop, mDroppedPlayer);

		CharacterData data = character.getCharacterData();
		// 通知布局
		ScriptMahjongHandIn handIn = mLayoutManager.getScript(LAYOUT_TYPE.LT_MAHJONG_HAND_IN) as ScriptMahjongHandIn;
		handIn.notifyPengOrGang(data.mPosition, data.mPengGangList);

		// 然后重新排列玩家手里的牌
		CommandCharacterReorderMahjong cmdReorder = new CommandCharacterReorderMahjong();
		mCommandSystem.pushCommand(cmdReorder, character);
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo() + " : mahjong : " + mMahjong + ", dropped player : " + mDroppedPlayer.getName();
	}
}