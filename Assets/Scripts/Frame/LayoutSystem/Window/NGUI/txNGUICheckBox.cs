using UnityEngine;
using System.Collections;

public class txNGUICheckBox : txUIObject
{
	protected UIToggle mToggle;
	
	public txNGUICheckBox()
	{
		mType = UI_TYPE.UT_NGUI_CHECK_BOX;
	}
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		mToggle = mObject.GetComponent<UIToggle>();
	}
	public UIToggle getToggle() {return mToggle;}
	public void setChecked(bool check)
	{
		mToggle.value = check;
	}
	public bool getChecked()
	{
		return mToggle.value;
	}
}