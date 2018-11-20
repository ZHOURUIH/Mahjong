using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class txNGUISprite : txUIObject
{
	protected UISprite mSprite;
	public txNGUISprite()
	{
		mType = UI_TYPE.UT_NGUI_SPRITE;
	}
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		mSprite = mObject.GetComponent<UISprite>();
		if (mSprite == null)
		{
			mSprite = mObject.AddComponent<UISprite>();
		}
	}
	public UIAtlas getAtlas()
	{
		return mSprite.atlas;
	}
	public string getSpriteName()
	{
		return mSprite.spriteName;
	}
	public virtual void setAtlas(UIAtlas atlas)
	{
		if(mSprite.atlas != atlas)
		{
			mSprite.atlas = atlas;
		}
	}
	public void setSpriteName(string name, bool useSize, bool makeEvenSize = false)
	{
		mSprite.spriteName = name;
		if (useSize && name != "")
		{
			setWindowSize(getSpriteSize(makeEvenSize));
		}
	}
	public Vector2 getSpriteSize(bool makeEvenSize = false)
	{
		UISpriteData spriteData = mSprite.GetAtlasSprite();
		if (spriteData != null)
		{
			int width = spriteData.width;
			int height = spriteData.height;
			if (makeEvenSize)
			{
				width += width % 2;
				height += height % 2;
			}
			return new Vector2(width, height);
		}
		return Vector2.zero;
	}
	public Vector2 getWindowSize()
	{
		return new Vector2(mSprite.width, mSprite.height);
	}
	public void setWindowSize(Vector2 size)
	{
		mSprite.SetDimensions((int)size.x, (int)size.y);
	}
	public override void setDepth(int depth)
	{
		mSprite.depth = depth;
		base.setDepth(depth);
	}
	public override int getDepth()
	{
		return mSprite.depth;
	}
	public override void setAlpha(float alpha)
	{
		mSprite.alpha = alpha;
	}
	public override float getAlpha()
	{
		return mSprite.alpha;
	}
	public override void setFillPercent(float percent)
	{
		mSprite.fillAmount = percent;
	}
	public UISprite getSprite() { return mSprite; }
}