using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptFreeMatchTip : LayoutScript
{
	protected txNGUITexture mBackMask;
	protected txNGUITexture mBackground;
	protected txNGUIText mFreeMatchLabel;
	protected txNGUIButton mCancelMatchButton;
	protected txNGUIText mCancelButtonLabel;
	public ScriptFreeMatchTip(string name, GameLayout layout)
		:
		base(name, layout)
	{
		;
	}
	public override void assignWindow()
	{
		newObject(out mBackMask, "BackMask");
		newObject(out mBackground, "Background");
		newObject(out mFreeMatchLabel, mBackground, "FreeMatchLabel");
		newObject(out mCancelMatchButton, mBackground, "CancelMatchButton");
		newObject(out mCancelButtonLabel, mCancelMatchButton, "Label");
	}
	public override void init()
	{
		registeBoxColliderNGUI(mCancelMatchButton, onCancelClicked, onButtonPress);
	}
	public override void onReset()
	{
		LT.SCALE_WINDOW(mCancelMatchButton);
	}
	public override void onShow(bool immediately, string param)
	{
		;
	}
	public override void onHide(bool immediately, string param)
	{
		;
	}
	public override void update(float elapsedTime)
	{
		;
	}
	//-----------------------------------------------------------------------------------
	protected void onCancelClicked(GameObject obj)
	{
		// 向服务器发送取消匹配的消息
	}
	protected void onButtonPress(GameObject obj, bool press)
	{
		txUIObject window = mLayout.getUIObject(obj);
		LT.SCALE_WINDOW(window, window.getScale(), press ? new Vector2(1.2f, 1.2f) : Vector2.one, 0.2f);
	}
}