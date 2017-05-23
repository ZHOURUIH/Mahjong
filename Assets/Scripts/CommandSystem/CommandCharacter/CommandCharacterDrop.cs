using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CommandCharacterDrop : Command
{
	public MAHJONG mMah;
	public int mIndex;
	public CommandCharacterDrop(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
	public override void execute()
	{
		Character character = (mReceiver) as Character;
		CharacterData data = character.getCharacterData();
		character.dropMahjong(mMah);

		// 打出一张牌后,锁定玩家手里的牌,玩家不能点击手里的麻将
		ScriptMahjongHandIn handIn = mLayoutManager.getScript(LAYOUT_TYPE.LT_MAHJONG_HAND_IN) as ScriptMahjongHandIn;
		handIn.notifyCanDrop(false);
		handIn.notifyDropMahjong(data.mPosition, mMah, mIndex);
		// 确认麻将操作按钮已经隐藏
		ScriptPlayerAction playerAction = mLayoutManager.getScript(LAYOUT_TYPE.LT_PLAYER_ACTION) as ScriptPlayerAction;
		playerAction.notifyActionAsk(null);
		ScriptMahjongDrop scriptDrop = mLayoutManager.getScript(LAYOUT_TYPE.LT_MAHJONG_DROP) as ScriptMahjongDrop;
		scriptDrop.notifyDropMahjong(data.mPosition, data.mDropList, mMah);
		// 通知重新排列麻将
		CommandCharacterReorderMahjong cmd = new CommandCharacterReorderMahjong();
		mCommandSystem.pushCommand(cmd, character);
		// 通知麻将系统
		CommandMahjongSystemDrop cmdDrop = new CommandMahjongSystemDrop();
		cmdDrop.mPlayer = character;
		cmdDrop.mMahjong = mMah;
		mCommandSystem.pushCommand(cmdDrop, mMahjongSystem);
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo() + ", mahjong : " + mMah;
	}
}