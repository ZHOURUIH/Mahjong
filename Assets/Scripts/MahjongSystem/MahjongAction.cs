using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class MahjongAction
{
	public Character mActionPlayer;		// 需要操作的玩家
	public Character mDroppedPlayer;	// 打出牌的玩家
	public ACTION_TYPE mType;			// 操作类型
	public MAHJONG mMah;				// 打出的牌
}

public class MahjongActionHu : MahjongAction
{
	public List<HU_TYPE> mHuList;       // 胡的所有类型
	public MahjongActionHu()
	{
		mType = ACTION_TYPE.AT_HU;
	}
}

public class MahjongActionPeng : MahjongAction
{
	public MahjongActionPeng()
	{
		mType = ACTION_TYPE.AT_PENG;
	}
}

public class MahjongActionGang : MahjongAction
{
	public MahjongActionGang()
	{
		mType = ACTION_TYPE.AT_GANG;
	}
}

public class MahjongActionPass : MahjongAction
{
	public MahjongActionPass()
	{
		mType = ACTION_TYPE.AT_PASS;
	}
}