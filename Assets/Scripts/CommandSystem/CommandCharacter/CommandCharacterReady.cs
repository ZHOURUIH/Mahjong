using UnityEngine;
using System.Collections;

public class CommandCharacterReady : Command
{
	public CommandCharacterReady(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
	public override void execute()
	{
		Character character = (mReceiver) as Character;
		CharacterData data = character.getCharacterData();
		// 如果已经准备,则直接返回
		if(data.mReady)
		{
			return;
		}
		data.mReady = true;
		// 通知麻将系统玩家已经准备
		CommandMahjongSystemReady cmd = new CommandMahjongSystemReady();
		cmd.mCharacter = character;
		mCommandSystem.pushCommand(cmd, mMahjongSystem);
	}
}