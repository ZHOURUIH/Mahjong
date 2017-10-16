using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PengGangInfo
{
	public ACTION_TYPE mType;
	public MAHJONG mMahjong;
}

public class CharacterData : CharacterBaseData
{
	public CharacterData()
	{
		mGUID = GameDefine.INVALID_ID;
		mPosition = PLAYER_POSITION.PP_MAX;
		mHead = 0;
		mReady = false;
		mBanker = false;
		mPengGangList = new List<PengGangInfo>();
		mHandIn = new List<MAHJONG>();
		mDropList = new List<MAHJONG>();
		mHuaList = new List<MAHJONG>();
	}
	public int				mMoney;
	public int				mHead;
	public PLAYER_POSITION	mServerPosition; // 服务器中的位置,在服务器中庄家的位置是MYSELF
	public PLAYER_POSITION	mPosition;		 // 转换到客户端的位置,在客户端中自己的位置一直都是MYSELF
	public bool				mReady;     // 是否已经准备
	public bool				mBanker;    // 是否为庄家
	public List<PengGangInfo> mPengGangList;
	public List<MAHJONG>	mHandIn;
	public List<MAHJONG>	mDropList;
	public List<MAHJONG>	mHuaList;
	public int				mRoomID;	// 房间号
}
