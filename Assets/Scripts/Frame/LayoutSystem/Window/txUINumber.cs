using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class txUINumber : txUIStaticSprite
{
	protected int					 mMaxCount = 0;
	protected string[]				 mSpriteNameList = new string[11];		// 前10个是0~9,第11个是小数点
	protected UISpriteData[]		 mSpriteDataList = new UISpriteData[11];
	protected List<txUIStaticSprite> mNumberList = new List<txUIStaticSprite>();
	protected string                 mNumberStyle = "";
	protected int                    mInterval = 5;
	protected DOCKING_POSITION		 mDockingPosition = DOCKING_POSITION.DP_LEFT;
	protected string				 mNumber = "";
	public txUINumber()
	{
		mType = UI_OBJECT_TYPE.UBT_NUMBER;
	}
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		if(mSprite == null || mSprite.atlas == null)
		{
			return;
		}
		// 将atlas中的所有图片放到一个map中,方便查找
		Dictionary<string, UISpriteData> spriteMap = new Dictionary<string, UISpriteData>();
		List<UISpriteData> sprites = mSprite.atlas.spriteList;
		int spriteCount = sprites.Count;
		for (int i = 0; i < spriteCount; ++i)
		{
			spriteMap.Add(sprites[i].name, sprites[i]);
		}
		// 获得所有数字图片的名字和图片
		string spriteName = getSpriteName();
		int lastPos = spriteName.LastIndexOf('_');
		if (lastPos == -1)
		{
			return;
		}
		mNumberStyle = spriteName.Substring(0, lastPos);
		for (int i = 0; i < 10; ++i)
		{
			mSpriteNameList[i] = mNumberStyle + "_" + StringUtility.intToString(i);
			// 在atlas中查找对应名字的图片
			if (spriteMap.ContainsKey(mSpriteNameList[i]))
			{
				mSpriteDataList[i] = spriteMap[mSpriteNameList[i]];
			}
		}
		mSpriteNameList[10] = mNumberStyle + "_dot";
		if (spriteMap.ContainsKey(mSpriteNameList[10]))
		{
			mSpriteDataList[10] = spriteMap[mSpriteNameList[10]];
		}
		setMaxCount(10);
		mSprite.spriteName = "";
	}
	public int getContentWidth()
	{
		int width = 0;
		for (int i = 0; i < mNumberList.Count; ++i)
		{
			if (i >= mNumber.Length)
			{
				break;
			}
			width += mNumberList[i].mSprite.width;
		}
		width += mInterval * (mNumber.Length - 1);
		return width;
	}
	protected void refreshNumber()
	{
		Vector2 windowSize = getWindowSize();
		// 整数部分
		int dotPos = mNumber.LastIndexOf('.');
		if (mNumber.Length > 0 && (dotPos == 0 || dotPos == mNumber.Length - 1))
		{
			UnityUtility.logError("error : number can not start or end with dot!");
			return;
		}
		string intPart = dotPos != -1 ? mNumber.Substring(0, dotPos) : mNumber;
		for (int i = 0; i < intPart.Length; ++i)
		{
			mNumberList[i].setSpriteName(mSpriteNameList[intPart[i] - '0']);
		}
		// 小数点和小数部分
		if(dotPos != -1)
		{
			mNumberList[dotPos].setSpriteName(mSpriteNameList[10]);
			string floatPart = mNumber.Substring(dotPos + 1, mNumber.Length - dotPos - 1);
			for(int i = 0; i < floatPart.Length; ++i)
			{
				mNumberList[i + dotPos + 1].setSpriteName(mSpriteNameList[floatPart[i] - '0']);
			}
		}
		// 调整所有数字的大小,此处的aspectRatio可能没有更新
		Vector2 numberSize = Vector2.zero;
		float numberScale = 0.0f;
		int numberLength = mNumber.Length;
		if (numberLength > 0)
		{
			int firstNumber = mNumber[0] - '0';
			numberSize.y = windowSize.y;
			UISpriteData spriteData = mSpriteDataList[firstNumber];
			float inverseHeight = 1.0f / spriteData.height;
			float ratio = (float)spriteData.width * inverseHeight;
			numberSize.x = ratio * numberSize.y;
			numberScale = windowSize.y * inverseHeight;
		}
		if (dotPos != -1)
		{
			Vector2 dotTextureSize = new Vector2(mSpriteDataList[10].width, mSpriteDataList[10].height);
			mNumberList[dotPos].setWindowSize(dotTextureSize * numberScale);
		}
		for (int i = 0; i < numberLength; ++i)
		{
			if (mNumber[i] != '.')
			{
				mNumberList[i].setWindowSize(numberSize);
			}
		}
		// 调整窗口位置,隐藏不需要显示的窗口
		int contentWidth = getContentWidth();
		Vector2 pos = Vector2.zero;
		if (mDockingPosition == DOCKING_POSITION.DP_RIGHT)
		{
			pos = new Vector2(windowSize.x - contentWidth, 0);
		}
		else if (mDockingPosition == DOCKING_POSITION.DP_CENTER)
		{
			pos = new Vector2((windowSize.x - contentWidth) * 0.5f, 0);
		}
		int count = mNumberList.Count;
		for (int i = 0; i < count; ++i)
		{
			mNumberList[i].setActive(i < numberLength);
			if (i < numberLength)
			{
				Vector2 size = mNumberList[i].getWindowSize();
				mNumberList[i].setLocalPosition(pos - windowSize * 0.5f + size * 0.5f);
				pos.x += size.x + mInterval;
			}
		}
	}
	public void setInterval(int interval)
	{
		mInterval = interval;
		refreshNumber();
	}
	public void setDockingPosition(DOCKING_POSITION position)
	{
		mDockingPosition = position;
		refreshNumber();
	}
	public void setMaxCount(int maxCount)
	{
		if (mMaxCount == maxCount)
		{
			return;
		}
		mMaxCount = maxCount;
		// 设置的数字字符串不能超过最大数量
		if (mNumber.Length > mMaxCount)
		{
			mNumber = mNumber.Substring(0, mMaxCount);
		}
		mNumberList.Clear();
		for (int i = 0; i < mMaxCount + 1; ++i)
		{
			string name = mName + "_" + StringUtility.intToString(i);
			mNumberList.Add(mLayout.getScript().createObject<txUIStaticSprite>(this, name, false));
			mNumberList[i].mSprite.atlas = mSprite.atlas;
			mNumberList[i].mSprite.depth = mSprite.depth + 1;
		}
		refreshNumber();
	}
	public void setNumber(int num, int limitLen = 0)
	{
		setNumber(StringUtility.intToString(num, limitLen));
	}
	public void setNumber(string num)
	{
		mNumber = StringUtility.checkFloatString(num, "");
		// 设置的数字字符串不能超过最大数量
		if (mNumber.Length > mMaxCount)
		{
			mNumber = mNumber.Substring(0, mMaxCount);
		}
		refreshNumber();
	}
	public int getMaxCount()
	{
		return mMaxCount;
	}
	public string getNumber()
	{
		return mNumber;
	}
	public int getInterval()
	{
		return mInterval;
	}
	public string getNumberStyle()
	{
		return mNumberStyle;
	}
	public DOCKING_POSITION getDockingPosition()
	{
		return mDockingPosition;
	}
}