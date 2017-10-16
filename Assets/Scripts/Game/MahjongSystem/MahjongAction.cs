using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class MahjongAction
{
	public ACTION_TYPE mType;           // 操作类型
	public Character mActionPlayer;		// 需要操作的玩家
	public Character mDroppedPlayer;	// 打出牌的玩家
	public MAHJONG mMah;                // 打出的牌
	public List<HU_TYPE> mHuList;       // 胡的所有类型,只有胡类型才会有值
	public MahjongAction()
	{
		;
	}
	public MahjongAction(ACTION_TYPE type, Character actionPlayer, Character droppedPlayer, MAHJONG mahjong, List<HU_TYPE> huList = null)
	{
		mType = type;
		mActionPlayer = actionPlayer;
		mDroppedPlayer = droppedPlayer;
		mMah = mahjong;
		mHuList = huList;
	}
}