using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class txUGUIText : txUIObject
{
	protected Text mText;
	public txUGUIText()
	{
		;
	}
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		mText = mObject.GetComponent<Text>();
		if (mText == null)
		{
			mText = mObject.AddComponent<Text>();
		}
	}
	public override int getDepth(){return mText.depth; }
	public void setText(string text) { mText.text = text; }
	public string getText() { return mText.text; }
	public override float getAlpha()
	{
		if (mText == null)
		{
			return 0.0f;
		}
		return mText.color.a;
	}
	public override void setAlpha(float alpha)
	{
		if(mText == null)
		{
			return;
		}
		Color color = mText.color;
		color.a = alpha;
		mText.color = color;
	}
}