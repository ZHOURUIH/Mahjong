using System;
using System.Collections;
using System.Collections.Generic;

public class SCRoomList : SocketPacket
{
	protected SHORT mAllRoomCount = new SHORT();
	protected BYTE mRoomCount = new BYTE();
	protected INTS mRoomID = new INTS(GameDefine.MAX_REQUEST_ROOM_COUNT);
	protected BYTES mOwnerName = new BYTES(GameDefine.MAX_NAME_LENGTH * GameDefine.MAX_REQUEST_ROOM_COUNT);
	protected BYTES mCurCount = new BYTES(GameDefine.MAX_REQUEST_ROOM_COUNT);
	protected BYTES mMaxCount = new BYTES(GameDefine.MAX_REQUEST_ROOM_COUNT);
	public SCRoomList(PACKET_TYPE type)
		:base(type){}
	protected override void fillParams()
	{
		pushParam(mAllRoomCount);
		pushParam(mRoomCount);
		pushParam(mRoomID);
		pushParam(mOwnerName);
		pushParam(mCurCount);
		pushParam(mMaxCount);
	}
	public override void execute()
	{
		byte[] nameBytes = new byte[GameDefine.MAX_NAME_LENGTH];
		int count = mRoomCount.mValue;
		List<RoomInfo> roomInfoList = new List<RoomInfo>();
		for(int i = 0; i < count; ++i)
		{
			RoomInfo info = new RoomInfo();
			info.mID = mRoomID.mValue[i];
			BinaryUtility.memcpy(nameBytes, mOwnerName.mValue, 0, i * GameDefine.MAX_NAME_LENGTH, GameDefine.MAX_NAME_LENGTH);
			info.mOwnerName = BinaryUtility.bytesToString(nameBytes, BinaryUtility.getGB2312());
			info.mCurCount = mCurCount.mValue[i];
			info.mMaxCount = mMaxCount.mValue[i];
			roomInfoList.Add(info);
		}
		CommandMainSceneNotifyRoomList cmd = newCmd(out cmd);
		cmd.mRoomInfoList = roomInfoList;
		cmd.mAllRoomCount = mAllRoomCount.mValue;
		pushCommand(cmd, mGameSceneManager.getCurScene());
	}
}