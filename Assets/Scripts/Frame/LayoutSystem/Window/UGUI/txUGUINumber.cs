using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class txUGUINumber : txUGUIStaticImage
{
	protected int mMaxCount = 0;
	protected string[] mSpriteNameList;     // 前10个是0~9,第11个是小数点
	protected Sprite[] mSpriteList;
	protected List<txUGUIStaticImage> mNumberList;
	protected string mNumberStyle = "";
	protected int mInterval = 5;
	protected DOCKING_POSITION mDockingPosition = DOCKING_POSITION.DP_LEFT;
	protected string mNumber = "";
	public txUGUINumber()
	{
		mType = UI_TYPE.UT_UGUI_NUMBER;
		mSpriteNameList = new string[11];
		mSpriteList = new Sprite[11];
		mNumberList = new List<txUGUIStaticImage>();
	}
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		if (mImage == null)
		{
			return;
		}
		string imageName = mImage.sprite.name;
		int lastPos = imageName.LastIndexOf('_');
		if (lastPos == -1)
		{
			return;
		}
		mNumberStyle = imageName.Substring(0, lastPos);
		string path = CommonDefine.R_NUMBER_STYLE_PATH + mNumberStyle;
		List<string> fileList = mResourceManager.getFileList(path, true);
		for (int i = 0; i < 10; ++i)
		{
			mSpriteNameList[i] = mNumberStyle + "_" + StringUtility.intToString(i);
			// 在atlas中查找对应名字的图片
			if (fileList.Contains(mSpriteNameList[i].ToLower()))
			{
				string resourceName = path + "/" + mSpriteNameList[i];
				mSpriteList[i] = UnityUtility.texture2DToSprite(mResourceManager.loadResource<Texture2D>(resourceName, false));
			}
		}
		mSpriteNameList[10] = mNumberStyle + "_dot";
		if (fileList.Contains(mSpriteNameList[10]))
		{
			string resourceName = path + "/" + mSpriteNameList[10];
			mSpriteList[10] = UnityUtility.texture2DToSprite(mResourceManager.loadResource<Texture2D>(resourceName, false));
		}
		setMaxCount(10);
		mImage.enabled = false;
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
			width += (int)mNumberList[i].mImage.sprite.rect.width;
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
			mNumberList[i].setSprite(mSpriteList[intPart[i] - '0']);
		}
		// 小数点和小数部分
		if (dotPos != -1)
		{
			mNumberList[dotPos].setSprite(mSpriteList[10]);
			string floatPart = mNumber.Substring(dotPos + 1, mNumber.Length - dotPos - 1);
			for (int i = 0; i < floatPart.Length; ++i)
			{
				mNumberList[i + dotPos + 1].setSprite(mSpriteList[floatPart[i] - '0']);
			}
		}
		// 根据当前窗口的大小调整所有数字的大小
		Vector2 numberSize = Vector2.zero;
		float numberScale = 0.0f;
		int numberLength = mNumber.Length;
		if (numberLength > 0)
		{
			numberSize.y = windowSize.y;
			Sprite sprite = mSpriteList[mNumber[0] - '0'];
			float inverseHeight = 1.0f / sprite.rect.height;
			float ratio = sprite.rect.width * inverseHeight;
			numberSize.x = ratio * numberSize.y;
			numberScale = windowSize.y * inverseHeight;
		}
		if (dotPos != -1)
		{
			mNumberList[dotPos].setWindowSize(mSpriteList[10].rect.size * numberScale);
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
			mNumberList.Add(mLayout.getScript().createObject<txUGUIStaticImage>(this, name, false));
		}
		refreshNumber();
	}
	public void setNumber(int num, int limitLen = 0)
	{
		setNumber(StringUtility.intToString(num, limitLen));
	}
	public void setNumber(string num)
	{
		mNumber = StringUtility.checkFloatString(num);
		// 设置的数字字符串不能超过最大数量
		if (mNumber.Length > mMaxCount)
		{
			mNumber = mNumber.Substring(0, mMaxCount);
		}
		refreshNumber();
	}
	public int getMaxCount() { return mMaxCount; }
	public string getNumber() { return mNumber; }
	public int getInterval() { return mInterval; }
	public string getNumberStyle() { return mNumberStyle; }
	public DOCKING_POSITION getDockingPosition() { return mDockingPosition; }
}