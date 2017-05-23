using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class txUIStaticSprite : txUIObject
{
	public UISprite mSprite;
	public txUIStaticSprite()
	{
		mType = UI_OBJECT_TYPE.UBT_STATIC_SPRITE;
	}
	public override void init(GameLayout layout, GameObject go)
	{
		base.init(layout, go);
		mSprite = mObject.GetComponent<UISprite>();
		if (mSprite == null)
		{
			mSprite = mObject.AddComponent<UISprite>();
		}
	}
	public string getSpriteName()
	{
		if (mSprite == null)
		{
			return "";
		}
		return mSprite.spriteName;
	}
	public void setSpriteName(string name)
	{
		if (mSprite == null)
		{
			return;
		}
		mSprite.spriteName = name;
	}
	public Vector2 getWindowSize()
	{
		if (mSprite == null)
		{
			return Vector2.zero;
		}
		return new Vector2(mSprite.width, mSprite.height);
	}
	public void setWindowSize(Vector2 size)
	{
		if (mSprite == null)
		{
			return;
		}
		mSprite.SetDimensions((int)size.x, (int)size.y);
	}
	public override void setAlpha(float alpha)
	{
		if (mSprite == null)
		{
			return;
		}
		mSprite.alpha = alpha;
	}
	public override float getAlpha()
	{
		if (mSprite == null)
		{
			return 0.0f;	
		}
		return mSprite.alpha;
	}
}