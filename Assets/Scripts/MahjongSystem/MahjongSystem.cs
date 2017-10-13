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
	MPS_WAIT_FOR_ACTION,		// 正在等待玩家选择对当前打出牌的行为
	MPS_ENDING,					// 本局麻将结束
}

public class WaitActionInfo
{
	public Character mPlayer;				// 等待确认操作的玩家
	public List<MahjongAction> mActionList;	// 玩家可选择的操作
	public Character mDroppedPlayer;		// 打出这张牌的玩家
	public MAHJONG mMahjong;				// 当前麻将
	public MahjongAction mConfirmedAction;	// 玩家确认选择的一种操作
}

public class MahjongSystem : CommandReceiver
{
	protected Dictionary<int, Character> mPlayerIDList;	// key是玩家GUID,value是玩家对象,只用于查找
	protected Dictionary<PLAYER_POSITION, Character> mPlayerPositionList;          // 玩家列表,保存着玩家之间的顺序
	protected MAHJONG_PLAY_STATE mPlayState;            // 当前麻将游戏的状态
	protected List<MAHJONG> mMahjongPool;				// 当前麻将池中的麻将
	protected int[] mDice;                              // 骰子的值
	protected PLAYER_POSITION mBankerPos;				// 本局庄家的位置
	protected PLAYER_POSITION mCurAssignPos;			// 开局发牌时当前发到牌的玩家的位置
	protected float mCurInterval;                       // 当前间隔时间计时
	protected Dictionary<Character, WaitActionInfo> mWaitList;
	public MahjongSystem()
		:
		base(typeof(MahjongSystem).ToString())
	{
		mPlayerIDList = new Dictionary<int, Character>();
		mPlayerPositionList = new Dictionary<PLAYER_POSITION, Character>();
		for(int i = 0; i < (int)PLAYER_POSITION.PP_MAX; ++i)
		{
			mPlayerPositionList.Add((PLAYER_POSITION)i, null);
		}
		mMahjongPool = new List<MAHJONG>();
		mDice = new int[GameDefine.DICE_COUNT];
		mWaitList = new Dictionary<Character, WaitActionInfo>();
	}
	public void init()
	{
		notifyPlayState(MAHJONG_PLAY_STATE.MPS_WAITING);
	}
	public override void destroy()
	{
		base.destroy();
	}
	public void update(float elapsedTime)
	{
		// 开始拿牌时,需要由麻将系统给玩家分发牌
		if(mPlayState == MAHJONG_PLAY_STATE.MPS_GET_START)
		{
			mCurInterval -= elapsedTime;
			// 从庄家开始发牌
			if (mCurInterval <= 0.0f)
			{
				mCurInterval = GameDefine.ASSIGN_MAHJONG_INTERVAL;
				Character curPlayer = mPlayerPositionList[mCurAssignPos];
				// 给玩家发牌
				CommandCharacterGetStart cmd = mCommandSystem.newCmd<CommandCharacterGetStart>();
				cmd.mMahjong = requestGet();
				mCommandSystem.pushCommand(cmd, curPlayer);

				bool isDone = false;
				int palyerHandInCount = curPlayer.getCharacterData().mHandIn.Count;
				// 如果是庄家,需要拿够14张牌
				if (mCurAssignPos == mBankerPos)
				{
					isDone = (palyerHandInCount == GameDefine.MAX_HAND_IN_COUNT);
				}
				// 不是庄家则拿13张牌
				else
				{
					isDone = (palyerHandInCount == GameDefine.MAX_HAND_IN_COUNT - 1);
				}
				// 牌拿完时需要重新排列
				if (isDone)
				{
					CommandCharacterReorderMahjong cmdReorder = mCommandSystem.newCmd<CommandCharacterReorderMahjong>();
					mCommandSystem.pushCommand(cmdReorder, curPlayer);

					// 如果是庄家拿完了牌,则进入正常游戏流程
					if(mCurAssignPos == mBankerPos)
					{
						CommandMahjongSceneNotifyStartDone cmdStartDone = mCommandSystem.newCmd<CommandMahjongSceneNotifyStartDone>();
						mCommandSystem.pushCommand(cmdStartDone, mGameSceneManager.getCurScene());

						// 通知玩家打出一张牌
						CommandCharacterAskDrop cmdAskDrop = mCommandSystem.newCmd<CommandCharacterAskDrop>();
						mCommandSystem.pushCommand(cmdAskDrop, curPlayer);
						return;
					}
				}
				mCurAssignPos = (PLAYER_POSITION)(((int)mCurAssignPos + 1) % (int)PLAYER_POSITION.PP_MAX);
			}
		}
	}
	public bool canPlayerJoin()
	{
		return mPlayerIDList.Count < GameDefine.MAX_PLAYER_COUNT;
	}
	public bool canPlayerJoin(Character player)
	{
		if(player == null || mPlayerIDList.ContainsKey(player.getCharacterData().mGUID))
		{
			return false;
		}
		return canPlayerJoin();
	}
	public Character getCharacterByPosition(PLAYER_POSITION pos)
	{
		return mPlayerPositionList[pos];
	}
	public Dictionary<int, Character> getAllPlayer()
	{
		return mPlayerIDList;
	}
	public int[] getDice()
	{
		return mDice;
	}
	//-----------------------------------------------------------------------------------------------------------
	// 事件通知
	public void notifyPlayState(MAHJONG_PLAY_STATE state)
	{
		mPlayState = state;
		// 进入等待流程时,就是开始新一局的麻将游戏,需要重置麻将系统的数据
		if(mPlayState == MAHJONG_PLAY_STATE.MPS_WAITING)
		{
			mPlayerIDList.Clear();
			List<PLAYER_POSITION> posList = new List<PLAYER_POSITION>(mPlayerPositionList.Keys);
			int posCount = posList.Count;
			for (int i = 0; i < posCount; ++i)
			{
				mPlayerPositionList[posList[i]] = null;
			}
			mMahjongPool.Clear();
			int diceCount = mDice.Length;
			for (int i = 0; i < diceCount; ++i)
			{
				mDice[i] = 0;
			}
			mBankerPos = PLAYER_POSITION.PP_MAX;
			mCurAssignPos = PLAYER_POSITION.PP_MAX;
			mCurInterval = 0.0f;
		}
		// 开始掷骰子时,需要计算出掷骰子的结果
		else if (mPlayState == MAHJONG_PLAY_STATE.MPS_DICE)
		{
			generateDiceRet(ref mDice);
		}
		// 开始拿牌时,需要重置麻将池
		else if (mPlayState == MAHJONG_PLAY_STATE.MPS_GET_START)
		{
			resetMahjongPool();
			// 判断当前谁是庄家
			foreach(var item in mPlayerPositionList)
			{
				if(item.Value.getCharacterData().mBanker)
				{
					mBankerPos = item.Key;
					mCurAssignPos = mBankerPos;
					break;
				}
			}
			if(mBankerPos == PLAYER_POSITION.PP_MAX)
			{
				UnityUtility.logError("not find banker!");
			}
		}
		else if(mPlayState == MAHJONG_PLAY_STATE.MPS_NORMAL_GAMING)
		{
			mWaitList.Clear();
		}
		else if(mPlayState == MAHJONG_PLAY_STATE.MPS_WAIT_FOR_ACTION)
		{
			;
		}
		else if(mPlayState == MAHJONG_PLAY_STATE.MPS_ENDING)
		{
			;
		}
	}
	public bool notifyPlayerJoin(Character player)
	{
		if(player == null)
		{
			return false;
		}
		int guid = player.getCharacterData().mGUID;
		if(mPlayerIDList.ContainsKey(guid))
		{
			return false;
		}
		// 添加到查找列表中
		mPlayerIDList.Add(guid, player);
		// 在位置列表中找到一个空的位置,然后放入玩家
		List<PLAYER_POSITION> posList = new List<PLAYER_POSITION>(mPlayerPositionList.Keys);
		int posCount = posList.Count;
		for (int i = 0; i < posCount; ++i)
		{
			PLAYER_POSITION pos = posList[i];
			if (mPlayerPositionList[pos] == null)
			{
				mPlayerPositionList[pos] = player;
				player.getCharacterData().mPosition = pos;
				return true;
			}
		}
		return false;
	}
	public bool notifyPlayerQuit(Character player)
	{
		if(player == null)
		{
			return false;
		}
		CharacterData data = player.getCharacterData();
		if(!mPlayerIDList.ContainsKey(data.mGUID))
		{
			return false;
		}
		mPlayerIDList.Remove(data.mGUID);
		PLAYER_POSITION position = data.mPosition;
		if(mPlayerPositionList[position] != player)
		{
			return false;
		}
		mPlayerPositionList[position] = null;
		return true;
	}
	// 返回值为是否全部都准备
	public bool notifyPlayerReady(Character player)
	{
		int playerCount = 0;
		bool allReady = true;
		List<PLAYER_POSITION> posList = new List<PLAYER_POSITION>(mPlayerPositionList.Keys);
		int posCount = posList.Count;
		for (int i = 0; i < posCount; ++i)
		{
			if(mPlayerPositionList[posList[i]] != null)
			{
				++playerCount;
				if (!mPlayerPositionList[posList[i]].getCharacterData().mReady)
				{
					allReady = false;
					break;
				}
			}
		}
		// 如果人数未满,则不能开始游戏
		if (playerCount != (int)PLAYER_POSITION.PP_MAX || !allReady)
		{
			return false;
		}
		return true;
	}
	public void notifyPlayerDrop(Character player, MAHJONG mah)
	{
		// 正常游戏过程中,玩家打了一张牌后,判断其他玩家是否有碰或者杠,该下家摸牌
		if(mPlayState == MAHJONG_PLAY_STATE.MPS_NORMAL_GAMING)
		{
			// 判断其他玩家是否可以碰或者杠
			bool hasAction = false;
			foreach (var item in mPlayerIDList)
			{
				if (item.Value != player)
				{
					List<MahjongAction> checkActionList = new List<MahjongAction>();
					CharacterData data = item.Value.getCharacterData();
					// 是否可胡
					if (GameUtility.canHu(data.mHandIn, mah))
					{
						List<HU_TYPE> huList = GameUtility.generateHuType(data.mHandIn, mah, data.mPengGangList, false, false);
						checkActionList.Add(new MahjongAction(ACTION_TYPE.AT_HU, item.Value, player, mah, huList));
					}
					// 是否可杠
					if (GameUtility.canGang(data.mHandIn, mah))
					{
						checkActionList.Add(new MahjongAction(ACTION_TYPE.AT_GANG, item.Value, player, mah));
					}
					// 是否可碰
					if (GameUtility.canPeng(data.mHandIn, mah))
					{
						checkActionList.Add(new MahjongAction(ACTION_TYPE.AT_PENG, item.Value, player, mah));
					}
					if (checkActionList.Count > 0)
					{
						hasAction = true;
						// 添加pass操作
						checkActionList.Add(new MahjongAction(ACTION_TYPE.AT_PASS, item.Value, player, mah));
						askPlayerAction(item.Value, player, mah, checkActionList);
					}
				}
			}
			// 没有人需要这张牌,则该下家摸牌
			if (!hasAction)
			{
				if(mMahjongPool.Count > 0)
				{
					PLAYER_POSITION nextPosition = (PLAYER_POSITION)(((int)(player.getCharacterData().mPosition) + 1) % (int)PLAYER_POSITION.PP_MAX);
					Character nextPlayer = getCharacterByPosition(nextPosition);
					CommandCharacterGet cmdGet = mCommandSystem.newCmd<CommandCharacterGet>();
					cmdGet.mMahjong = requestGet();
					mCommandSystem.pushCommand(cmdGet, nextPlayer);
				}
				// 牌已经摸完了,则本局为平局
				else
				{
					//End;
				}
			}
		}
	}
	public void notifyGet(Character player, MAHJONG mah)
	{
		// 判断是否可胡或者可杠
		CharacterData data = player.getCharacterData();
		List<MahjongAction> actionList = new List<MahjongAction>();
		// 是否可胡
		if (GameUtility.canHu(data.mHandIn))
		{
			List<HU_TYPE> huList = GameUtility.generateHuType(data.mHandIn, mah, data.mPengGangList, true, true);
			actionList.Add(new MahjongAction(ACTION_TYPE.AT_HU, player, player, mah, huList));
		}
		// 是否可以杠
		else if (GameUtility.canGang(data.mHandIn))
		{
			actionList.Add(new MahjongAction(ACTION_TYPE.AT_GANG, player, player, mah));
		}
		// 摸了一张自己碰的牌,可以开杠
		else
		{
			int pengIndex = 0;
			if(player.hasPeng(mah, ref pengIndex))
			{
				actionList.Add(new MahjongAction(ACTION_TYPE.AT_GANG, player, player, mah));
			}
		}
		if (actionList.Count > 0)
		{
			// 如果有可以操作的行为,则还需要添加Pass行为
			actionList.Add(new MahjongAction(ACTION_TYPE.AT_PASS, player, null, MAHJONG.M_MAX));
			askPlayerAction(player, player, mah, actionList);
		}
		else
		{
			// 没有任何操作则通知玩家需要打一张牌出来
			CommandCharacterAskDrop cmdAskDrop = mCommandSystem.newCmd<CommandCharacterAskDrop>();
			mCommandSystem.pushCommand(cmdAskDrop, player);
		}
	}
	// 询问玩家要选择哪种操作
	public void askPlayerAction(Character player, Character droppedPlayer, MAHJONG mah, List<MahjongAction> actionList)
	{
		if(actionList.Count == 0)
		{
			UnityUtility.logError("has no action");
			return;
		}
		// 将行为放入列表
		WaitActionInfo info = new WaitActionInfo();
		info.mPlayer = player;
		info.mDroppedPlayer = droppedPlayer;
		info.mActionList = actionList;
		info.mMahjong = mah;
		mWaitList.Add(info.mPlayer, info);
		// 设置状态为等待玩家确认操作
		notifyPlayState(MAHJONG_PLAY_STATE.MPS_WAIT_FOR_ACTION);
		// 询问玩家进行什么操作
		CommandCharacterAskAction cmd = mCommandSystem.newCmd<CommandCharacterAskAction>();
		cmd.mActionList = actionList;
		mCommandSystem.pushCommand(cmd, player);
	}
	// 玩家请求确认操作
	public void playerConfirmAction(Character player, ACTION_TYPE type)
	{
		if(!mWaitList.ContainsKey(player))
		{
			UnityUtility.logError("player has no action : name : " + player.getName() + ", action : " + type);
		}
		MahjongAction action = null;
		List<MahjongAction> actionList = mWaitList[player].mActionList;
		int actionCount = actionList.Count;
		for (int i = 0; i < actionCount; ++i)
		{
			if(actionList[i].mType == type)
			{
				action = actionList[i];
				break;
			}
		}
		if (action == null)
		{
			return;
		}
		mWaitList[player].mConfirmedAction = action;
		// 胡牌的优先级最高,如果有玩家选择胡牌,则忽略其他玩家的操作
		if (action.mType == ACTION_TYPE.AT_HU)
		{
			// 游戏状态设置为正常游戏
			notifyPlayState(MAHJONG_PLAY_STATE.MPS_NORMAL_GAMING);
			CommandCharacterHu cmd = mCommandSystem.newCmd<CommandCharacterHu>();
			mCommandSystem.pushCommand(cmd, player);
			
			// 有玩家胡牌后则结束游戏
			//End;
		}
		else
		{
			bool allConfirm = true;
			Character highestActionPlayer = null;
			MahjongAction highestAction = null;
			foreach (var wait in mWaitList)
			{
				if (wait.Value.mConfirmedAction == null)
				{
					allConfirm = false;
					break;
				}
				if (highestAction == null || highestAction.mType > wait.Value.mConfirmedAction.mType)
				{
					highestAction = wait.Value.mConfirmedAction;
					highestActionPlayer = wait.Value.mPlayer;
				}
			}
			// 如果全部玩家都已经确认操作了,允许优先级最高的操作进行
			if (allConfirm)
			{
				// 先获得信息,因为在设置状态时会将列表清空
				WaitActionInfo info = mWaitList[highestActionPlayer];
				// 游戏状态设置为正常游戏
				notifyPlayState(MAHJONG_PLAY_STATE.MPS_NORMAL_GAMING);
				if (highestAction.mType == ACTION_TYPE.AT_GANG)
				{
					// 自己摸的牌开杠
					if(info.mDroppedPlayer == info.mPlayer)
					{
						;
					}
					// 别人打出牌开杠
					else
					{
						;
					}
					CommandCharacterGang cmd = mCommandSystem.newCmd<CommandCharacterGang>();
					cmd.mDroppedPlayer = info.mDroppedPlayer;
					cmd.mMahjong = info.mMahjong;
					mCommandSystem.pushCommand(cmd, info.mPlayer);

					// 还有牌,玩家杠了一张牌以后需要再摸一张
					if (mMahjongPool.Count > 0)
					{
						CommandCharacterGet cmdGet = mCommandSystem.newCmd<CommandCharacterGet>();
						cmdGet.mMahjong = requestGet();
						mCommandSystem.pushCommand(cmdGet, info.mPlayer);
					}
					// 没有牌了则平局
					else
					{
						//End;
					}
				}
				else if (highestAction.mType == ACTION_TYPE.AT_PENG)
				{
					CommandCharacterPeng cmd = mCommandSystem.newCmd<CommandCharacterPeng>();
					cmd.mDroppedPlayer = info.mDroppedPlayer;
					cmd.mMahjong = info.mMahjong;
					mCommandSystem.pushCommand(cmd, info.mPlayer);

					CommandCharacterAskDrop cmdAskDrop = mCommandSystem.newCmd<CommandCharacterAskDrop>();
					mCommandSystem.pushCommand(cmdAskDrop, info.mPlayer);
				}
				else if (highestAction.mType == ACTION_TYPE.AT_PASS)
				{
					// 如果是自己摸了一张牌,选择了pass,则需要自己打一张牌出来
					if (info.mDroppedPlayer == info.mPlayer)
					{
						CommandCharacterAskDrop cmd = mCommandSystem.newCmd<CommandCharacterAskDrop>();
						mCommandSystem.pushCommand(cmd, info.mPlayer);
					}
					else
					{
						// 还有牌则通知下一家摸牌
						if(mMahjongPool.Count > 0)
						{
							PLAYER_POSITION nextPosition = (PLAYER_POSITION)(((int)(info.mDroppedPlayer.getCharacterData().mPosition) + 1) % (int)PLAYER_POSITION.PP_MAX);
							CommandCharacterGet cmdGet = mCommandSystem.newCmd<CommandCharacterGet>();
							cmdGet.mMahjong = requestGet();
							mCommandSystem.pushCommand(cmdGet, getCharacterByPosition(nextPosition));
						}
						// 没有牌了则平局
						else
						{
							//End;
						}
					}
				}
			}
		}
	}
	//------------------------------------------------------------------------------------------------------------------------
	protected void generateDiceRet(ref int[] dice)
	{
		int diceCount = dice.Length;
		for (int i = 0; i < diceCount; ++i)
		{
			dice[i] = MathUtility.randomInt(0, GameDefine.MAX_DICE_VALUE);
		}
	}
	protected void resetMahjongPool()
	{
		for(int i = 0; i < (int)MAHJONG.M_MAX; ++i)
		{
			for (int j = 0; j < GameDefine.MAX_SINGLE_COUNT; ++j)
			{
				mMahjongPool.Add((MAHJONG)i);
			}
		}
		// 打乱麻将的顺序
		// 复制一份麻将池,然后从中随机取出放入麻将池中,直到取完
		int mahjongCount = mMahjongPool.Count;
		List<MAHJONG> tempPool = new List<MAHJONG>(mMahjongPool);
		for(int i = 0; i < mahjongCount; ++i)
		{
			int randIndex = MathUtility.randomInt(0, tempPool.Count - 1);
			mMahjongPool[i] = tempPool[randIndex];
			// 填充临时麻将池中的空隙
			tempPool[randIndex] = tempPool[mahjongCount - i - 1];
			tempPool.RemoveAt(tempPool.Count - 1);
		}
	}
	// 请求从麻将池中拿一张牌
	protected MAHJONG requestGet()
	{
		if(mMahjongPool.Count > 0)
		{
			MAHJONG mah = mMahjongPool[mMahjongPool.Count - 1];
			mMahjongPool.RemoveAt(mMahjongPool.Count - 1);
			return mah;
		}
		return MAHJONG.M_MAX;
	}
}