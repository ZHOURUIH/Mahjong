using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptMessageOK : LayoutScript
{
	protected txNGUITexture mBackground;
	protected txNGUIText mMessage;
	protected txNGUIButton mOKButton;
	protected txNGUIText mButtonLabel;
	public ScriptMessageOK(string name, GameLayout layout)
		:
		base(name, layout)
	{}
	public override void assignWindow()
	{
		newObject(out mBackground, "Background");
		newObject(out mMessage, mBackground, "Message");
		newObject(out mOKButton, mBackground, "OKButton");
		newObject(out mButtonLabel, mOKButton, "ButtonLabel");
	}
	public override void init()
	{
		registeBoxColliderNGUI(mOKButton, onOKClick, onButtonPress);
	}
	public override void onReset()
	{
		LayoutTools.SCALE_WINDOW(mOKButton);
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
	public void setMessage(string message)
	{
		mMessage.setLabel(message);
	}
	public void setButtonLabel(string label)
	{
		mButtonLabel.setLabel(label);
	}
	//-------------------------------------------------------------------------------------------------------
	protected void onOKClick(GameObject button)
	{
		LayoutTools.HIDE_LAYOUT(mType);
	}
	protected void onButtonPress(GameObject button, bool press)
	{
		txUIObject obj = mLayout.getUIObject(button);
		LayoutTools.SCALE_WINDOW(obj, obj.getScale(), press ? new Vector2(1.2f, 1.2f) : Vector2.one, 0.2f);
	}
}