using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CommandRoomLeave : Command
{
	public Character mCharacter;
	public override void init()
	{
		base.init();
		mCharacter = null;
	}
	public override void execute()
	{
		Room room = mReceiver as Room;
		room.notifyPlayerLeave(mCharacter);
		// 通知界面
		mScriptAllCharacterInfo.notifyCharacterLeave(mCharacter);
		// 通知玩家已经离开房间
		pushCommand<CommandCharacterNotifyLeave>(mCharacter);
		// 如果不是玩家自己,则需要销毁玩家
		if (mCharacter.getType() != CHARACTER_TYPE.CT_MYSELF)
		{
			CommandCharacterManagerDestroy cmd = newCmd(out cmd);
			cmd.mGUID = mCharacter.getCharacterData().mGUID;
			pushCommand(cmd, mCharacterManager);
		}
	}
	public override string showDebugInfo()
	{
		CharacterData data = mCharacter.getCharacterData();
		return this.GetType().ToString() + ": guid : " + data.mGUID + ", name : " + data.mName;
	}
}