using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;
using System.IO;

public class MahInfo
{
	public MAHJONG mMah;
	public int mCount;
	public MahInfo(MAHJONG mah, int count)
	{
		mMah = mah;
		mCount = count;
	}
};

public class GameUtility : GameBase
{
	protected static int mIDMaker;
	public void init()
	{
		;
	}
	public static float calcuteConfigExpression(GAME_DEFINE_STRING CommonDefine, float variableValue)
	{
		string variableStr = "(" + StringUtility.floatToString(variableValue, 2) + ")";
		string expression = mGameConfig.getStringParam(CommonDefine);
		expression = expression.Replace("i", variableStr);
		float expressionValue = MathUtility.calculateFloat(expression);
		return expressionValue;
	}
	public static int makeID() { return ++mIDMaker; }
	// handInMah必须是从小到大的有序数组
	public static bool canHu(List<MAHJONG> handInMah, MAHJONG mah)
	{
		//复制一份列表
		List<MAHJONG> temp = new List<MAHJONG>(handInMah);
		// 然后别人打出的牌加入其中
		temp.Add(mah);
		return canHu(temp);
	}
	public static bool canHu(List<MAHJONG> handInMah)
	{
		List<List<MAHJONG>> devide = null;
		return canHu(toMahjongGroup(handInMah), ref devide);
	}
	public static List<MahInfo> toMahjongGroup(List<MAHJONG> list)
	{
		SortedDictionary<MAHJONG, MahInfo> groupMap = new SortedDictionary<MAHJONG, MahInfo>();
		int listSize = list.Count;
		for (int i = 0; i < listSize; ++i)
		{
			MAHJONG mah = list[i];
			if (groupMap.ContainsKey(mah))
			{
				groupMap[mah].mCount += 1;
			}
			else
			{
				groupMap.Add(mah, new MahInfo(mah, 1));
			}
		}
		return new List<MahInfo>(groupMap.Values);
	}

	// 得到指定牌的花色
	public static MAHJONG_HUASE getHuaSe(MAHJONG mah)
	{
		if(mah >= MAHJONG.M_FENG_DONG && mah <= MAHJONG.M_FENG_BAI)
		{
			return MAHJONG_HUASE.MH_FENG;
		}
		else
		{
			int huaseIndex = (mah - MAHJONG.M_TONG1) / 9;
			return MAHJONG_HUASE.MH_TONG + huaseIndex;
		}
	}

	public static bool isShunzi(List<MahInfo> mahjongList, int startIndex)
	{
		if (mahjongList.Count <= startIndex + 2)
		{
			return false;
		}
		if (getHuaSe(mahjongList[startIndex].mMah) == MAHJONG_HUASE.MH_FENG ||
			getHuaSe(mahjongList[startIndex + 1].mMah) == MAHJONG_HUASE.MH_FENG ||
			getHuaSe(mahjongList[startIndex + 2].mMah) == MAHJONG_HUASE.MH_FENG)
		{
			return false;
		}
		// 必须是同花色的
		MAHJONG_HUASE huase0 = getHuaSe(mahjongList[startIndex].mMah);
		MAHJONG_HUASE huase1 = getHuaSe(mahjongList[startIndex + 1].mMah);
		MAHJONG_HUASE huase2 = getHuaSe(mahjongList[startIndex + 2].mMah);
		if (huase0 != huase1 || huase1 != huase2)
		{
			return false;
		}
		if (mahjongList[startIndex].mMah + 1 != mahjongList[startIndex + 1].mMah ||
			mahjongList[startIndex + 1].mMah + 1 != mahjongList[startIndex + 2].mMah)
		{
			return false;
		}
		return true;
	}

	public static SortedDictionary<MAHJONG_HUASE, List<MAHJONG>> getHuaseList(List<MahInfo> mahjongList, bool includeFeng = false)
	{
		SortedDictionary<MAHJONG_HUASE, List<MAHJONG>> huaseList = new SortedDictionary<MAHJONG_HUASE, List<MAHJONG>>();
		int size = mahjongList.Count;
		for (int i = 0; i < size; ++i)
		{
			MAHJONG_HUASE huase = getHuaSe(mahjongList[i].mMah);
			if (!huaseList.ContainsKey(huase))
			{
				if(includeFeng || huase != MAHJONG_HUASE.MH_FENG)
				{
					List<MAHJONG> temp = new List<MAHJONG>();
					temp.Add(mahjongList[i].mMah);
					huaseList.Add(huase, temp);
				}
			}
			else
			{
				huaseList[huase].Add(mahjongList[i].mMah);
			}
		}
		return huaseList;
	}

	public static bool canHu(List<MahInfo> mahjongList, ref List<List<MAHJONG>> devideList)
	{
		// 必须至少缺一色牌才能胡,
		SortedDictionary<MAHJONG_HUASE, List<MAHJONG>> huaseList = getHuaseList(mahjongList);
		if (huaseList.Count > 2)
		{
			return false;
		}
		int allCount = 0;
		int size = mahjongList.Count;
		for (int i = 0; i < size; ++i)
		{
			allCount += mahjongList[i].mCount;
		}
		if (allCount > 2)
		{
			// 取出所有可能的顺子和三个相同的,逐一判断是否可胡
			int index = 0;
			while (true)
			{
				// 没找到顺子,则退出循环
				if (index >= mahjongList.Count)
				{
					break;
				}
				// 判断是否有三个相同的
				if (mahjongList[index].mCount >= 3)
				{
					// 先备份列表
					List<MahInfo> beforeList = new List<MahInfo>();
					int curCount = mahjongList.Count;
					for (int i = 0; i < curCount; ++i)
					{
						MahInfo info = new MahInfo(mahjongList[i].mMah, mahjongList[i].mCount);
						beforeList.Add(info);
					}
					// 移除三个相同的,判断去除后是否可胡
					MAHJONG curMah = mahjongList[index].mMah;
					mahjongList[index].mCount -= 3;
					if (mahjongList[index].mCount == 0)
					{
						mahjongList.RemoveAt(index);
					}
					if (canHu(mahjongList, ref devideList))
					{
						if(devideList != null)
						{
							List<MAHJONG> temp = new List<MAHJONG>();
							temp.Add(curMah);
							temp.Add(curMah);
							temp.Add(curMah);
							devideList.Add(temp);
						}
						return true;
					}
					// 不能胡则还原回去
					else
					{
						mahjongList = beforeList;
					}
				}
				// 判断是否有顺子
				if (isShunzi(mahjongList, index))
				{
					// 先备份列表
					List<MahInfo> beforeList = new List<MahInfo>();
					int curCount = mahjongList.Count;
					for (int i = 0; i < curCount; ++i)
					{
						MahInfo info = new MahInfo(mahjongList[i].mMah, mahjongList[i].mCount);
						beforeList.Add(info);
					}
					MAHJONG mah0 = mahjongList[index].mMah;
					MAHJONG mah1 = mahjongList[index + 1].mMah;
					MAHJONG mah2 = mahjongList[index + 2].mMah;
					// 移除该顺子,判断去除后是否可胡,需要从后往前判断,避免移除后影响下标
					mahjongList[index + 2].mCount -= 1;
					if (mahjongList[index + 2].mCount == 0)
					{
						mahjongList.RemoveAt(index + 2);
					}
					mahjongList[index + 1].mCount -= 1;
					if (mahjongList[index + 1].mCount == 0)
					{
						mahjongList.RemoveAt(index + 1);
					}
					mahjongList[index].mCount -= 1;
					if (mahjongList[index].mCount == 0)
					{
						mahjongList.RemoveAt(index);
					}
					// 如果可以胡,则直接返回true
					if (canHu(mahjongList, ref devideList))
					{
						if (devideList != null)
						{
							List<MAHJONG> temp = new List<MAHJONG>();
							temp.Add(mah0);
							temp.Add(mah1);
							temp.Add(mah2);
							devideList.Add(temp);
						}
						return true;
					}
					// 不能胡,则需要将顺子还原到列表中,然后继续寻找下一个顺子
					else
					{
						mahjongList = beforeList;
					}
				}
				// 判断三个相同和顺子后都不能胡,就继续往后找
				++index;
			}
			// 遍历到最后一个顺子都没发现可以胡,则不能胡
			return false;
		}
		// 判断最后剩的两张牌是否为将牌
		else
		{
			bool ret = (mahjongList.Count == 1 && mahjongList[0].mCount == 2);
			if (ret)
			{
				if (devideList != null)
				{
					List<MAHJONG> temp = new List<MAHJONG>();
					temp.Add(mahjongList[0].mMah);
					temp.Add(mahjongList[0].mMah);
					devideList.Add(temp);
				}
			}
			return ret;
		}
	}
	public static bool canPeng(List<MAHJONG> handInMah, MAHJONG mah)
	{
		List<MahInfo> infoList = toMahjongGroup(handInMah);
		int count = infoList.Count;
		for (int i = 0; i < count; ++i)
		{
			if (infoList[i].mCount >= 2 && infoList[i].mMah == mah)
			{
				return true;
			}
		}
		return false;
	}
	public static bool canGang(List<MAHJONG> handInMah, MAHJONG mah)
	{
		List<MahInfo> infoList = toMahjongGroup(handInMah);
		int count = infoList.Count;
		for (int i = 0; i < count; ++i)
		{
			if (infoList[i].mCount == 3 && infoList[i].mMah == mah)
			{
				return true;
			}
		}
		return false;
	}
	public static bool canGang(List<MAHJONG> handInMah)
	{
		List<MahInfo> infoList = toMahjongGroup(handInMah);
		int count = infoList.Count;
		for (int i = 0; i < count; ++i)
		{
			if (infoList[i].mCount == 4)
			{
				return true;
			}
		}
		return false;
	}
	public static void pengMahjong(ref List<MAHJONG> handInMah, MAHJONG mah)
	{
		// 碰的前提是之前检测过可以碰
		int mahCount = handInMah.Count;
		for (int i = 0; i < mahCount - 1; ++i)
		{
			if (handInMah[i] == mah)
			{
				// 从后往前删除
				handInMah.RemoveAt(i + 1);
				handInMah.RemoveAt(i);
				break;
			}
		}
	}
	public static void gangMahjong(ref List<MAHJONG> handInMah, MAHJONG mah)
	{
		// 杠的前提是之前检测过可以杠
		int mahCount = handInMah.Count;
		for (int i = 0; i < mahCount - 2; ++i)
		{
			if (handInMah[i] == mah)
			{
				handInMah.RemoveAt(i + 2);
				handInMah.RemoveAt(i + 1);
				handInMah.RemoveAt(i);
				break;
			}
		}
	}
	// handInIncludeDrop表示handInMah中是否已经包含了dropMah
	public static List<HU_TYPE> generateHuType(List<MAHJONG> handInMah, MAHJONG dropMah, PengGangInfo[] gangPengList, bool isSelfGet, bool handInIncludeDrop)
	{
		// 将数组转换为列表
		List<MAHJONG> handInList = new List<MAHJONG>();
		List<MAHJONG> pengs = new List<MAHJONG>();
		List<MAHJONG> gangs = new List<MAHJONG>();
		int handInCount = handInMah.Count;
		for (int i = 0; i < handInCount; ++i)
		{
			handInList.Add(handInMah[i]);
		}
		// 如果handInMah中不包含dropMah,则需要加到列表中
		if(!handInIncludeDrop)
		{
			int curCount = handInList.Count;
			for(int i = 0; i < curCount; ++i)
			{
				if(handInList[i] >= dropMah)
				{
					handInList.Insert(i, dropMah);
				}
			}
		}
		int gangPengCount = gangPengList.Length;
		for (int i = 0; i < gangPengCount; ++i)
		{
			if (gangPengList[i].mType == ACTION_TYPE.AT_GANG)
			{
				pengs.Add(gangPengList[i].mMahjong);
			}
			else if(gangPengList[i].mType == ACTION_TYPE.AT_PENG)
			{
				gangs.Add(gangPengList[i].mMahjong);
			}
		}
		// 判断胡牌类型
		List<HU_TYPE> huList = new List<HU_TYPE>();
		// 是否为清一色
		if (isQingYiSe(handInList, pengs, gangs))
		{
			huList.Add(HU_TYPE.HT_QINGYISE);
		}
		// 如果不满足任意一种番型,则为平胡
		if(huList.Count == 0)
		{
			huList.Add(HU_TYPE.HT_NORMAL);
		}
		return huList;
	}
	protected static bool isQingYiSe(List<MAHJONG> handInMah, List<MAHJONG> pengList, List<MAHJONG> gangList)
	{
		int handInCount = handInMah.Count;
		MAHJONG_HUASE curHuase = getHuaSe(handInMah[0]);
		for (int i = 0; i < handInCount; ++i)
		{
			if (getHuaSe(handInMah[i]) != curHuase)
			{
				return false;
			}
		}
		int pengCount = pengList.Count;
		for (int i = 0; i < pengCount; ++i)
		{
			if(getHuaSe(pengList[i]) != curHuase)
			{
				return false;
			}
		}
		int gangCount = gangList.Count;
		for (int i = 0; i < gangCount; ++i)
		{
			if (getHuaSe(gangList[i]) != curHuase)
			{
				return false;
			}
		}
		return true;
	}
}