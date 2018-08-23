using UnityEngine;
using System.Collections;

// 完全使用NGUI的Button功能
public class txNGUIButton : txUIObject
{
	protected UIButton	  mButton;
	public txNGUIButton()
	{
		mType = UI_TYPE.UT_NGUI_BUTTON;
	}
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		mButton = mObject.GetComponent<UIButton>();
		setFadeColour(true);
	}
	// 当按钮需要改变透明度或者附加颜色变化时,需要禁用按钮的颜色渐变
	public void setFadeColour(bool fade)
	{
		mButton.mFadeColour = fade;
		mButton.mUseState = fade;
	}
	public UIButton getButton() {return mButton;}
	public override void setHandleInput(bool enable)
	{
		base.setHandleInput(enable);
		mButton.SetState(enable ? UIButtonColor.State.Normal : UIButtonColor.State.Disabled, true);
	}
}