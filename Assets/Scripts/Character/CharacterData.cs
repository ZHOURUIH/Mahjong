using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PengGangInfo
{
	public ACTION_TYPE mType;
	public MAHJONG mMahjong;
}

public class CharacterData
{
	public CharacterData()
	{
		mClientID = -1;
		mGUID = -1;
		mName = "";
		mPosition = PLAYER_POSITION.PP_MAX;
		mHead = 0;
		mReady = false;
		mBanker = false;
		mPengGangList = new PengGangInfo[CommonDefine.MAX_PENG_TIMES];
		for (int i = 0; i < CommonDefine.MAX_PENG_TIMES; ++i)
		{
			mPengGangList[i] = new PengGangInfo();
			mPengGangList[i].mType = ACTION_TYPE.AT_MAX;
			mPengGangList[i].mMahjong = MAHJONG.M_MAX;
		}
		mHandIn = new List<MAHJONG>();
		mDropList = new List<MAHJONG>();
	}
	public int				mClientID;	// 客户端ID,由角色管理器自动生成
	public int				mGUID;		// 玩家唯一ID,由服务器发送过来的
	public string			mName;
	public int				mMoney;
	public PLAYER_POSITION	mPosition;
	public int				mHead;
	public bool				mReady;     // 是否已经准备
	public bool				mBanker;    // 是否为庄家
	public PengGangInfo[]	mPengGangList;
	public List<MAHJONG>	mHandIn;
	public List<MAHJONG>	mDropList;
}
