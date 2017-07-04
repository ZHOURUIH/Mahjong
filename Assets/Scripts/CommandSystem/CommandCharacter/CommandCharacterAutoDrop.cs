using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CommandCharacterAutoDrop : Command
{
	public override void init()
	{
		base.init();
	}
	public override void execute()
	{
		Character character = (mReceiver) as Character;
		CharacterData data = character.getCharacterData();
		int dropIndex = -1;
		// 如果还未打缺一色,则需要打最少的一色牌
		SortedDictionary<MAHJONG_HUASE, List<MAHJONG>> huaseList = GameUtility.getHuaseList(GameUtility.toMahjongGroup(data.mHandIn));
		if(huaseList.Count > 2)
		{
			MAHJONG_HUASE minHuase = MAHJONG_HUASE.MH_MAX;
			foreach(var item in huaseList)
			{
				if(minHuase == MAHJONG_HUASE.MH_MAX || minHuase > item.Key)
				{
					minHuase = item.Key;
				}
			}
			MAHJONG dropMah = huaseList[minHuase][huaseList[minHuase].Count - 1];
			int handInCount = data.mHandIn.Count;
			for(int i = 0; i < handInCount; ++i)
			{
				if(data.mHandIn[i] == dropMah)
				{
					dropIndex = i;
					break;
				}
			}
		}
		// 如果已经打缺了,则打最后一张
		if(dropIndex == -1)
		{
			dropIndex = data.mHandIn.Count - 1;
		}
		CommandCharacterDrop cmd = mCommandSystem.newCmd<CommandCharacterDrop>();
		cmd.mIndex = dropIndex;
		cmd.mMah = data.mHandIn[dropIndex];
		mCommandSystem.pushCommand(cmd, character);
	}
}