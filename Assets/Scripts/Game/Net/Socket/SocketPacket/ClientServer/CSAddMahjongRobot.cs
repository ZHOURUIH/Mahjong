using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class CSAddMahjongRobot : SocketPacket
{
	public CSAddMahjongRobot(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams(){}
}