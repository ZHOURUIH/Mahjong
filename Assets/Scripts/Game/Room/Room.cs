using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ResultInfo
{
	public Character mPlayer;
	public List<HU_TYPE> mHuList;
}
public class Room : CommandReceiver
{
	protected bool mCanJoin = false;
	protected int mID;
	protected List<Character> mNoneJoinPlayerList;							// 还未进入等待流程时就接收到的玩家列表
	protected Dictionary<int, Character> mPlayerIDList;						// key是玩家GUID,value是玩家对象,只用于查找
	protected SortedDictionary<PLAYER_POSITION, Character> mPlayerPositionList; // 玩家列表,保存着玩家之间的顺序
	protected List<ResultInfo> mResultInfoList;
	protected int mMahjongPoolSize = 0;		// 当前麻将池中麻将的个数
	public Room(int id)
		:
		base(typeof(Room).ToString())
	{
		mID = id;
		mNoneJoinPlayerList = new List<Character>();
		mPlayerIDList = new Dictionary<int, Character>();
		mPlayerPositionList = new SortedDictionary<PLAYER_POSITION, Character>();
		mResultInfoList = new List<ResultInfo>();
	}
	public int getRoomID() { return mID; }
	public Dictionary<int, Character> getPlayerList() { return mPlayerIDList; }
	public List<ResultInfo> getResultInfoList() { return mResultInfoList; }
	public SortedDictionary<PLAYER_POSITION, Character> getPlayerPositionList() { return mPlayerPositionList; }
	public int getMahjongPoolSize() { return mMahjongPoolSize; }
	public void setMahjongPoolSize(int count) { mMahjongPoolSize = count; }
	// 通知房间有玩家加入
	public void notifyPlayerJoin(Character player)
	{
		if(mPlayerIDList.ContainsKey(player.getCharacterData().mGUID))
		{
			return;
		}
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
			CommandRoomJoin cmd = newCmd(out cmd);
			cmd.mCharacter = mNoneJoinPlayerList[i];
			pushCommand(cmd, this);
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
			CommandRoomLeave cmdLeave = newCmd(out cmdLeave);
			cmdLeave.mCharacter = item.Value;
			pushCommand(cmdLeave, this);
		}
	}
	// 有玩家胡牌
	public void notifyPlayerHu(Character player, List<HU_TYPE> huList)
	{
		ResultInfo result = new ResultInfo();
		result.mHuList = huList;
		result.mPlayer = player;
		mResultInfoList.Add(result);
	}
	public void clearAllPlayerMahjongData()
	{
		foreach(var item in mPlayerIDList)
		{
			item.Value.clearMahjongData();
		}
	}
}