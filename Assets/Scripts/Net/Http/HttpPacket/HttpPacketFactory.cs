using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class HttpPacketFactory
{
	protected static Dictionary<HTTP_PACKET, string> PacketTypeList;
	protected static Dictionary<string, HTTP_PACKET> PacketAdressList;
	public HttpPacketFactory()
	{
		PacketTypeList = new Dictionary<HTTP_PACKET, string>();
		PacketAdressList = new Dictionary<string, HTTP_PACKET>();
	}
	public void init()
	{
		//PacketTypeList.Add(HTTP_PACKET.HP_START_GAME, typeof(HttpPacketStartGame).ToString());
		//PacketTypeList.Add(HTTP_PACKET.HP_SET_DRAG, typeof(HttpPacketSetDrag).ToString());
		//PacketTypeList.Add(HTTP_PACKET.HP_GET_SPEED, typeof(HttpPacketGetSpeed).ToString());
		//PacketTypeList.Add(HTTP_PACKET.HP_GET_COUNT_DOWN, typeof(HttpPacketGetCountDown).ToString());
		//PacketTypeList.Add(HTTP_PACKET.HP_GET_DISTANCE, typeof(HttpPacketGetDistance).ToString());
		//PacketTypeList.Add(HTTP_PACKET.HP_STOP_GAME, typeof(HttpPacketStopGame).ToString());
		//PacketTypeList.Add(HTTP_PACKET.HP_CHANGE_VIDEO, typeof(HttpPacketSetVideo).ToString());
		//PacketTypeList.Add(HTTP_PACKET.HP_GET_VIDEO_LIST, typeof(HttpPacketGetVideoList).ToString());
		//PacketTypeList.Add(HTTP_PACKET.HP_LOCK_SPEED, typeof(HttpPacketLockSpeed).ToString());

		//PacketAdressList.Add("/dndl.txtj.com/v1/start", HTTP_PACKET.HP_START_GAME);
		//PacketAdressList.Add("/dndl.txtj.com/v1/setdrag", HTTP_PACKET.HP_SET_DRAG);
		//PacketAdressList.Add("/dndl.txtj.com/v1/getspd", HTTP_PACKET.HP_GET_SPEED);
		//PacketAdressList.Add("/dndl.txtj.com/v1/getcountdown", HTTP_PACKET.HP_GET_COUNT_DOWN);
		//PacketAdressList.Add("/dndl.txtj.com/v1/getdis", HTTP_PACKET.HP_GET_DISTANCE);
		//PacketAdressList.Add("/dndl.txtj.com/v1/stop", HTTP_PACKET.HP_STOP_GAME);
		//PacketAdressList.Add("/dndl.txtj.com/v1/changemap", HTTP_PACKET.HP_CHANGE_VIDEO);
		//PacketAdressList.Add("/dndl.txtj.com/v1/videolist", HTTP_PACKET.HP_GET_VIDEO_LIST);
		//PacketAdressList.Add("/dndl.txtj.com/v1/lockspeed", HTTP_PACKET.HP_LOCK_SPEED);
	}
	public static HttpPacket createPacket(string address)
	{
		HTTP_PACKET typePacket = HTTP_PACKET.HP_NONE;
		if (PacketAdressList.ContainsKey(address))
		{
			typePacket = PacketAdressList[address];
		}
		if (typePacket != HTTP_PACKET.HP_NONE)
		{
			Type typeT = Type.GetType(PacketTypeList[typePacket]);
			object[] tParams = new object[] { typePacket, address };
			HttpPacket newPacket = UnityUtility.createInstance<HttpPacket>(typeT, tParams);
			return newPacket;
		}

		return null;
	}
}