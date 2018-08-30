using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class RoomInfo
{
	public int mID;
	public string mOwnerName;
	public int mCurCount;
	public int mMaxCount;
}

public class RoomSystem : FrameComponent
{
	protected List<RoomInfo> mRoomInfoList;
	public RoomSystem(string name)
		: base(name)
	{
		mRoomInfoList = new List<RoomInfo>();
	}
	public override void init()
	{
		base.init();
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
	}
	public void clearRoomInfo()
	{
		mRoomInfoList.Clear();
	}
	public void addRoomInfo(RoomInfo info)
	{
		mRoomInfoList.Add(info);
	}
	public List<RoomInfo> getRoomInfoList()
	{
		return mRoomInfoList;
	}
}