using UnityEngine;
using System.Collections;

public class txNGUIButton : txUIObject
{
	protected UIButton	  mButton;
	protected bool		  mHandleInput = true;
	public txNGUIButton()
	{
		mType = UI_TYPE.UT_NGUI_BUTTON;
	}
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		mButton = mObject.GetComponent<UIButton>();
		if (mButton == null)
		{
			mButton = mObject.AddComponent<UIButton>();
		}
		if(mBoxCollider == null)
		{
			mBoxCollider = mObject.AddComponent<BoxCollider>();
		}
	}
	// 当按钮需要改变透明度或者附加颜色变化时,需要禁用按钮的颜色渐变
	public void setFadeColour(bool fade){mButton.mFadeColour = fade;}
	public UIButton getButton() {return mButton;}
	public override void setHandleInput(bool enable)
	{
		base.setHandleInput(enable);
		mHandleInput = enable;
		mButton.SetState(mHandleInput ? UIButtonColor.State.Normal : UIButtonColor.State.Disabled, true);
	}
	public override bool getHandleInput()
	{
		return base.getHandleInput() && mHandleInput;
	}
}