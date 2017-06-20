using System;
using System.Collections;
using System.Collections.Generic;

public class SCAskAction : SocketPacket
{
	protected BYTES mAction = new BYTES(4);
	protected INTS mActionPlayer = new INTS(4);
	protected INTS mDroppedPlayer = new INTS(4);
	protected BYTES mMahjong = new BYTES(4);
	protected BYTES mHuList = new BYTES(CommonDefine.MAX_HU_COUNT);     // 当有胡操作时,该数组中才会有值
	public SCAskAction(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mAction);
		pushParam(mActionPlayer);
		pushParam(mDroppedPlayer);
		pushParam(mMahjong);
		pushParam(mHuList);
	}
	public override void execute()
	{
		List<MahjongAction> actionList = new List<MahjongAction>();
		for(int i = 0; i < 4; ++i)
		{
			Character player = mCharacterManager.getCharacterByGUID(mActionPlayer.mValue[i]);
			Character droppedPlayer = mCharacterManager.getCharacterByGUID(mDroppedPlayer.mValue[i]);
			ACTION_TYPE type = (ACTION_TYPE)mAction.mValue[i];
			if(type != ACTION_TYPE.AT_MAX)
			{
				MAHJONG mah = (MAHJONG)mMahjong.mValue[i];
				List<HU_TYPE> huList = null;
				if (type == ACTION_TYPE.AT_HU)
				{
					huList = new List<HU_TYPE>();
					for (int j = 0; j < CommonDefine.MAX_HU_COUNT; ++j)
					{
						if (mHuList.mValue[i] == 0)
						{
							break;
						}
						huList.Add((HU_TYPE)mHuList.mValue[i]);
					}
				}
				actionList.Add(new MahjongAction(type, player, droppedPlayer, mah, huList));
			}
		}
		CommandCharacterAskAction cmd = new CommandCharacterAskAction();
		cmd.mActionList = actionList;
		mCommandSystem.pushCommand(cmd, mCharacterManager.getMyself());
	}
}