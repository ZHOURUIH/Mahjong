using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptPlayerAction : LayoutScript
{
	protected txNGUIButton[] mAction;
	public ScriptPlayerAction(string name, GameLayout layout)
		:
		base(name, layout)
	{
		mAction = new txNGUIButton[(int)ACTION_TYPE.AT_MAX];
	}
	public override void assignWindow()
	{
		int count = mAction.Length;
		for (int i = 0; i < count; ++i)
		{
			newObject(out mAction[i], "Action" + i);
		}
	}
	public override void init()
	{
		int count = mAction.Length;
		for (int i = 0; i < count; ++i)
		{
			registeBoxColliderNGUI(mAction[i], onActionClicked);
		}
	}
	public override void onReset()
	{
		notifyActionAsk(null);
	}
	public override void onShow(bool immediately, string param)
	{
		int count = mAction.Length;
		for (int i = 0; i < count; ++i)
		{
			LT.SCALE_WINDOW(mAction[i], Vector2.one);
		}
	}
	public override void onHide(bool immediately, string param)
	{
		;
	}
	public override void update(float elapsedTime)
	{
		;
	}
	public void notifyActionAsk(List<MahjongAction> actionList)
	{
		// 先隐藏所有行为
		int count = mAction.Length;
		for (int i = 0; i < count; ++i)
		{
			LT.ACTIVE_WINDOW(mAction[i], false);
		}
		// 显示可以操作的行为
		if(actionList != null)
		{
			int actionCount = actionList.Count;
			for (int i = 0; i < actionCount; ++i)
			{
				LT.ACTIVE_WINDOW(mAction[(int)actionList[i].mType]);
			}
		}
	}
	//------------------------------------------------------------------------------------------------------
	protected void onActionClicked(GameObject obj)
	{
		ACTION_TYPE action = ACTION_TYPE.AT_MAX;
		int count = mAction.Length;
		for (int i = 0; i < count; ++i)
		{
			if (mLayout.getUIObject(obj) == mAction[i])
			{
				action = (ACTION_TYPE)i;
				break;
			}
		}
		if(action != ACTION_TYPE.AT_MAX)
		{
			CSConfirmAction confirm = mSocketManager.createPacket<CSConfirmAction>();
			confirm.mAction.mValue = (byte)action;
			mSocketManager.sendMessage(confirm);
			afterActionSelected();
			// 通知其他布局行为已经选择
			mScriptMahjongHandIn.notifyAbleToPengOrGang(PLAYER_POSITION.PP_MYSELF, MAHJONG.M_MAX);
		}
	}
	protected void afterActionSelected()
	{
		notifyActionAsk(null);
	}
}