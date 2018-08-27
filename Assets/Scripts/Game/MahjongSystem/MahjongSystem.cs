using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// 本局麻将的状态
public enum MAHJONG_PLAY_STATE
{
	MPS_WAITING,				// 正在等待玩家进入或准备
	MPS_DICE,					// 正在掷骰子
	MPS_GET_START,				// 正在开局拿牌
	MPS_NORMAL_GAMING,			// 正在进行正常的麻将游戏
	MPS_ENDING,					// 本局麻将结束
}

public class MahjongSystem : FrameComponent
{
	protected Dictionary<int, CharacterOther> mPlayerIDList;	// key是玩家GUID,value是玩家对象,只用于查找
	protected List<CharacterOther> mPlayerPositionList;          // 玩家列表,保存着玩家之间的顺序,顺序为服务器中玩家的位置
	protected MAHJONG_PLAY_STATE mPlayState;            // 当前麻将游戏的状态
	protected byte[] mDice;                              // 骰子的值
	protected PLAYER_POSITION mBankerPos;				// 本局庄家的位置
	protected PLAYER_POSITION mCurAssignPos;			// 开局发牌时当前发到牌的玩家的位置,为服务器中的位置
	protected float mCurInterval;                       // 当前间隔时间计时
	protected List<List<MAHJONG>> mHandInList;
	protected List<List<MAHJONG>> mHuaList;
	public MahjongSystem(string name)
		:base(name)
	{
		mPlayerIDList = new Dictionary<int, CharacterOther>();
		mPlayerPositionList = new List<CharacterOther>();
		mDice = new byte[GameDefine.DICE_COUNT];
	}
	public override void init()
	{
		base.init();
	}
	public override void destroy()
	{
		base.destroy();
	}
	public override void update(float elapsedTime)
	{
		// 开始拿牌时,需要由麻将系统给玩家分发牌
		if(mPlayState == MAHJONG_PLAY_STATE.MPS_GET_START)
		{
			mCurInterval -= elapsedTime;
			// 从庄家开始发牌
			if (mCurInterval <= 0.0f)
			{
				mCurInterval += GameDefine.ASSIGN_MAHJONG_INTERVAL;
				// 给玩家发牌
				CommandCharacterGetStart cmd = newCmd(out cmd);
				cmd.mMahjong = mHandInList[(int)mCurAssignPos][0];
				pushCommand(cmd, mPlayerPositionList[(int)mCurAssignPos]);
				mHandInList[(int)mCurAssignPos].RemoveAt(0);
				// 如果到房主的牌发完了,则退出发牌
				if(mCurAssignPos == mBankerPos && mHandInList[(int)mCurAssignPos].Count == 0)
				{
					mPlayState = MAHJONG_PLAY_STATE.MPS_NORMAL_GAMING;
					// 牌拿完时需要重新排列,发放花牌
					int playerCount = mPlayerPositionList.Count;
					for (int i = 0; i < playerCount; ++i)
					{
						pushCommand<CommandCharacterReorderMahjong>(mPlayerPositionList[i]);

						int huaCount = mHuaList[i].Count;
						for (int j = 0; j < huaCount; ++j)
						{
							CommandCharacterGetHua cmdHua = newCmd(out cmdHua);
							cmdHua.mMah = mHuaList[i][j];
							pushCommand(cmdHua, mPlayerPositionList[i]);
						}
					}
					// 通知服务器开局麻将已经拿完了
					mSocketNetManager.sendMessage<CSGetStartMahjongDone>();
					return;
				}
				else
				{
					mCurAssignPos = (PLAYER_POSITION)(((int)mCurAssignPos + 1) % (int)PLAYER_POSITION.PP_MAX);
				}
			}
		}
	}
	public byte[] getDice() { return mDice; }
	public void setDice(byte[] dice)
	{
		mDice[0] = dice[0];
		mDice[1] = dice[1];
	}
	public void startMahjong(List<int> playerIDList, List<List<MAHJONG>> handInList, List<List<MAHJONG>> huaList)
	{
		int playerCount = playerIDList.Count;
		for(int i = 0; i < playerCount; ++i)
		{
			CharacterOther player = mCharacterManager.getCharacter(playerIDList[i]) as CharacterOther;
			mPlayerIDList.Add(player.getCharacterData().mGUID, player);
			mPlayerPositionList.Add(player);
		}
		mHandInList = handInList;
		mHuaList = huaList;
	}
	public void notifyGetStartMahjong()
	{
		mPlayState = MAHJONG_PLAY_STATE.MPS_GET_START;
	}
}