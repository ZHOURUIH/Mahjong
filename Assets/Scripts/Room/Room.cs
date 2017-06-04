using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Room : CommandReceiver
{
	protected bool mCanJoin;
	protected int mID;
	protected List<Character> mNoneJoinPlayerList;							// 还未进入等待流程时就接收到的玩家列表
	protected Dictionary<int, Character> mPlayerIDList;						// key是玩家GUID,value是玩家对象,只用于查找
	protected Dictionary<PLAYER_POSITION, Character> mPlayerPositionList;	// 玩家列表,保存着玩家之间的顺序
	public Room(int id)
		:
		base("room")
	{
		mID = id;
		mCanJoin = false;
	}
	public void notifyPlayerJoin(Character player)
	{
		if (mCanJoin)
		{
			mPlayerIDList.Add(player.getCharacterData().mGUID, player);
			mPlayerPositionList.Add(player.getCharacterData().mPosition, player);
		}
		else
		{
			mNoneJoinPlayerList.Add(player);
		}
	}
	public int getRoomID()
	{
		return mID;
	}
	// 通知房间开始等待玩家加入
	public void notifyStartWait()
	{
		mCanJoin = true;
		// 将未加入的玩家加入到房间中
		int count = mNoneJoinPlayerList.Count;
		for(int i = 0; i < count; ++i)
		{
			CommandRoomJoin cmd = new CommandRoomJoin();
			cmd.mCharacter = mNoneJoinPlayerList[i];
			mCommandSystem.pushCommand(cmd, this);
		}
		mNoneJoinPlayerList.Clear();
	}
}