using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CommandRoomJoin : Command
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
		GameScene gameScene = mGameSceneManager.getCurScene();
		if(gameScene.getSceneType() != GAME_SCENE_TYPE.GST_MAHJONG)
		{
			return;
		}
		// 重新计算客户端位置
		CharacterData data = mCharacter.getCharacterData();
		CharacterData myselfData = mCharacterManager.getMyself().getCharacterData();
		data.mPosition = GameUtility.serverPositionToClientPosition(data.mServerPosition, myselfData.mServerPosition);
		// 通知房间有玩家加入本局麻将
		room.notifyPlayerJoin(mCharacter);
		// 只能在麻将场景的等待流程才能通知加入
		if (gameScene.atProcedure(PROCEDURE_TYPE.PT_MAHJONG_WAITING))
		{
			ScriptAllCharacterInfo allInfo = mLayoutManager.getScript(LAYOUT_TYPE.LT_ALL_CHARACTER_INFO) as ScriptAllCharacterInfo;
			allInfo.notifyCharacterJoin(mCharacter);
		}
	}
	public override string showDebugInfo()
	{
		CharacterData data = mCharacter.getCharacterData();
		return this.GetType().ToString() + ": guid : " + data.mGUID + ", name : " + data.mName;
	}
}