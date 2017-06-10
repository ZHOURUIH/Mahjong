using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Room : CommandReceiver
{
	protected bool mCanJoin = false;
	protected int mID;
	protected List<Character> mNoneJoinPlayerList;							// 还未进入等待流程时就接收到的玩家列表
	protected Dictionary<int, Character> mPlayerIDList;						// key是玩家GUID,value是玩家对象,只用于查找
	protected SortedDictionary<PLAYER_POSITION, Character> mPlayerPositionList;	// 玩家列表,保存着玩家之间的顺序
	public Room(int id)
		:
		base("room")
	{
		mID = id;
		mNoneJoinPlayerList = new List<Character>();
		mPlayerIDList = new Dictionary<int, Character>();
		mPlayerPositionList = new SortedDictionary<PLAYER_POSITION, Character>();
	}
	public int getRoomID() { return mID; }
	public Dictionary<int, Character> getPlayerList() { return mPlayerIDList; }
	// 通知房间有玩家加入
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
	// 通知房间有玩家离开
	public void notifyPlayerLeave(Character player)
	{
		mPlayerIDList.Remove(player.getCharacterData().mGUID);
		mPlayerPositionList.Remove(player.getCharacterData().mPosition);
		int count = mNoneJoinPlayerList.Count;
		for (int i = 0; i < count; ++i)
		{
			if (mNoneJoinPlayerList[i] == player)
			{
				mNoneJoinPlayerList.RemoveAt(i);
				break;
			}
		}
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
	//退出房间中的所有玩家
	public void leaveAllRoomPlayer()
	{
		// 需要复制一份列表
		Dictionary<int, Character> listCopy = new Dictionary<int, Character>(mPlayerIDList);
		foreach (var item in listCopy)
		{
			// 离开房间时会自动销毁其他玩家
			CommandRoomLeave cmdLeave = new CommandRoomLeave();
			cmdLeave.mCharacter = item.Value;
			mCommandSystem.pushCommand(cmdLeave, this);
		}
	}
}