using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Character : MovableObject
{
	protected CHARACTER_TYPE	mCharacterType;// 角色类型
	protected CharacterData		mCharacterData;	//玩家数据

	public Character(CHARACTER_TYPE type, string name)
		:
		base(name)
	{
		mCharacterType = type;
		mCharacterData = null;
	}
	public virtual void init(int clientID, int guid)
	{
		if (null == mCharacterData)
		{
			mCharacterData = new CharacterData();
		}
		mCharacterData.mClientID = clientID;
		mCharacterData.mGUID = guid;
		mCharacterData.mName = mName;
		initComponents();
	}
	public override void initComponents()
	{
        ;
	}
	public override void update(float elaspedTime)
	{
		// 先更新自己的所有组件
		base.updateComponents(elaspedTime);
	}
	public virtual void notifyComponentChanged(GameComponent component) {}
	public CharacterData getCharacterData() { return mCharacterData; }
	public CHARACTER_TYPE getType() { return mCharacterType; }
	// 碰指定牌
	public virtual void pengMahjong(MAHJONG mahjong)
	{
		GameUtility.pengMahjong(ref mCharacterData.mHandIn, mahjong);
		addPeng(mahjong);
	}
	// 杠指定牌
	public virtual void gangMahjong(MAHJONG mahjong, Character dropPlayer)
	{
		int pengIndex = -1;
		bool isAlreadyPeng = hasPeng(mahjong, ref pengIndex);
		if (isAlreadyPeng)
		{
			mCharacterData.mPengGangList[pengIndex].mType = ACTION_TYPE.AT_GANG;
		}
		else
		{
			GameUtility.gangMahjong(ref mCharacterData.mHandIn, mahjong);
			addGang(mahjong);
		}
		// 如果是自己摸了一张牌,则需要将自己摸的牌拿出来,如果是其他人打出的牌,则不进行操作
		if (dropPlayer == this)
		{
			int handInCount = mCharacterData.mHandIn.Count;
			for (int j = 0; j < handInCount; ++j)
			{
				if (mCharacterData.mHandIn[j] == mahjong)
				{
					mCharacterData.mHandIn.RemoveAt(j);
					break;
				}
			}
		}
	}
	// 摸一张牌
	public virtual void getMahjong(MAHJONG mah)
	{
		mCharacterData.mHandIn.Add(mah);
	}
	// 打出一张牌
	public void dropMahjong(MAHJONG mah)
	{
		mCharacterData.mDropList.Add(mah);
		mCharacterData.mHandIn.Remove(mah);
	}
	public void takeDrop()
	{
		mCharacterData.mDropList.RemoveAt(mCharacterData.mDropList.Count - 1);
	}
	// 开局时的拿牌
	public void getMahjongStart(MAHJONG mah)
	{
		mCharacterData.mHandIn.Add(mah);
	}
	// 重新排列手里的牌
	public void reorderMahjong()
	{
		mCharacterData.mHandIn.Sort();
	}
	public bool hasPeng(MAHJONG mahjong, ref int pengIndex)
	{
		int count = mCharacterData.mPengGangList.Count;
		for (int i = 0; i < count; ++i)
		{
			if (mCharacterData.mPengGangList[i].mMahjong == mahjong)
			{
				pengIndex = i;
				return true;
			}
		}
		return false;
	}
	//---------------------------------------------------------------------------------------------------------
	protected void addPeng(MAHJONG mahjong)
	{
		PengGangInfo info = new PengGangInfo();
		info.mMahjong = mahjong;
		info.mType = ACTION_TYPE.AT_PENG;
		mCharacterData.mPengGangList.Add(info);
	}
	protected void addGang(MAHJONG mahjong)
	{
		PengGangInfo info = new PengGangInfo();
		info.mMahjong = mahjong;
		info.mType = ACTION_TYPE.AT_GANG;
		mCharacterData.mPengGangList.Add(info);
	}
}
