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
	public virtual void init(int id)
	{
		if (null == mCharacterData)
		{
			mCharacterData = new CharacterData();
		}
		mCharacterData.mClientID = id;
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
		// 找到一个空的位置,设置为碰的状态
		int maxTimes = mCharacterData.mPengGangList.Length;
		for (int i = 0; i < maxTimes; ++i)
		{
			if(mCharacterData.mPengGangList[i].mType == ACTION_TYPE.AT_MAX)
			{
				mCharacterData.mPengGangList[i].mType = ACTION_TYPE.AT_PENG;
				mCharacterData.mPengGangList[i].mMahjong = mahjong;
				break;
			}
		}
	}
	// 杠指定牌
	public virtual void gangMahjong(MAHJONG mahjong, Character dropPlayer)
	{
		int maxTimes = mCharacterData.mPengGangList.Length;
		for (int i = 0; i < maxTimes; ++i)
		{
			// 如果是自己摸了一张已经碰的牌来开杠
			if(mCharacterData.mPengGangList[i].mMahjong == mahjong
				&& mCharacterData.mPengGangList[i].mType == ACTION_TYPE.AT_PENG)
			{
				mCharacterData.mPengGangList[i].mType = ACTION_TYPE.AT_GANG;
				// 将手里摸的牌拿出
				int handInCount = mCharacterData.mHandIn.Count;
				for(int j = 0; j < handInCount; ++j)
				{
					if(mCharacterData.mHandIn[j] == mahjong)
					{
						mCharacterData.mHandIn.RemoveAt(j);
						break;
					}
				}
				break;
			}
			// 自己手里有三张牌
			else if(mCharacterData.mPengGangList[i].mMahjong == MAHJONG.M_MAX
				&& mCharacterData.mPengGangList[i].mType == ACTION_TYPE.AT_MAX)
			{
				GameUtility.gangMahjong(ref mCharacterData.mHandIn, mahjong);
				mCharacterData.mPengGangList[i].mType = ACTION_TYPE.AT_GANG;
				mCharacterData.mPengGangList[i].mMahjong = mahjong;
				// 自己摸了一张牌后开杠
				if (dropPlayer == this)
				{
					// 将手里摸的牌拿出
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
				// 别人打出一张牌开杠,不做其他操作
				else
				{
					;
				}
				break;
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
}
