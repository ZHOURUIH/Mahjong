using UnityEngine;
using System.Collections;

public class CommandCharacterReady : Command
{
	public bool mReady;
	public CommandCharacterReady(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
	public override void execute()
	{
		Character character = (mReceiver) as Character;
		// 发送消息通知服务器玩家已经准备
		CSReady packetReady = mSocketNetManager.createPacket(PACKET_TYPE.PT_CS_READY) as CSReady;
		packetReady.mReady = mReady;
		mSocketNetManager.sendMessage(packetReady);
	}
}