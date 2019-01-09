using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IScrollContainer
{
	void initOrigin();
	float getIndex();
	void setIndex(int index);
	float getOriginAlpha();
	Vector3 getOriginPosition();
	Vector3 getOriginScale();
	Vector3 getOriginRotation();
	float getPosFromLast(IScrollContainer lastItem);
	float getControlValue();
	void setControlValue(float value);
}

public interface IScrollItem
{
	void lerp(IScrollContainer curItem, IScrollContainer nextItem, float percent);
	void setCurControlValue(float value);
	float getCurControlValue();
	void setVisible(bool visible);
}

public enum SCROLL_STATE
{
	SS_NONE,			// 无
	SS_SCROLL_TARGET,	// 自动匀速滚动到目标点
	SS_DRAGING,			// 鼠标拖动
	SS_SCROLL_TO_STOP,	// 鼠标抬起后自动减速到停止
}

public class Scroll : GameBase
{
	protected LayoutScript mScript;
	protected DRAG_DIRECTION mDragDirection;        // 拖动方向,横向或纵向
	protected bool mMouseDown = false;              // 鼠标是否在窗口内按下,鼠标抬起或者离开窗口都会设置为false,鼠标按下时,跟随鼠标移动,鼠标放开时,按惯性移动
	protected float mAttenuateFactor = 2.0f;        // 移动速度衰减系数,鼠标在放开时移动速度会逐渐降低,衰减系数越大.速度降低越快
	protected txNGUITexture mBackground;            // 用于检测鼠标的按下,移动,抬起
	protected List<IScrollContainer> mContainerList;// 容器列表,用于获取位置缩放旋转等属性
	protected List<IScrollItem> mItemList;          // 物体列表,用于显示
	protected float mTargetOffsetValue;             // 本次移动的目标值
	protected float mCurOffsetValue;                // 整体的偏移值,并且会与每一项的原始偏移值叠加
	protected float mFocusSpeedThreshhold = 1.0f;   // 开始聚焦的速度阈值,当滑动速度正在自由降低的阶段时,速度低于该值则会以恒定速度自动聚焦到一个最近的项
	protected bool mLoop;                           // 是否循环滚动
	protected bool mItemOnCenter = true;            // 最终停止时是否聚焦到某一项上
	protected int mDefaultFocus;                    // 容器默认聚焦的下标
	protected float mMaxControlValue;               // 物体列表中最大的控制值
	protected SCROLL_STATE mState;                  // 当前状态
	protected float mScrollSpeed;                   // 当前滚动速度
	protected float mDragSensitive = 0.01f;         // 拖动的灵敏度
	public Scroll(LayoutScript script)
	{
		mScript = script;
		mLoop = false;
		mState = SCROLL_STATE.SS_NONE;
		mContainerList = new List<IScrollContainer>();
		mItemList = new List<IScrollItem>();
	}
	public void setBackground(txNGUITexture background)
	{
		mBackground = background;
		mBackground.setReceiveScreenMouse(true);
		mBackground.setOnMouseDown(onMouseDown);
		mBackground.setOnScreenMouseUp(onScreenMouseUp);
		mBackground.setOnMouseMove(onMouseMove);
		mBackground.setOnMouseStay(onMouseStay);
		mScript.registeBoxCollider(mBackground);
	}
	public int getFocusIndex()
	{
		float curOffset = mContainerList[mDefaultFocus].getControlValue() - mCurOffsetValue;
		return getItemIndex(curOffset, true, mLoop);
	}
	public void setDragDirection(DRAG_DIRECTION direction) { mDragDirection = direction; }
	public void setLoop(bool loop) { mLoop = loop; }
	public void setItemList<T>(List<T> itemList) where T : IScrollItem
	{
		float singleInterval = 1.0f;
		int itemCount = itemList.Count;
		for (int i = 0; i < itemCount; ++i)
		{
			itemList[i].setCurControlValue(i * singleInterval);
			mItemList.Add(itemList[i]);
		}
		if (mLoop)
		{
			mMaxControlValue = mItemList[mItemList.Count - 1].getCurControlValue() + singleInterval;
		}
		else
		{
			mMaxControlValue = mItemList[mItemList.Count - 1].getCurControlValue();
		}
	}
	public void setContainerList<T>(List<T> containerList, int defaultFocus) where T : IScrollContainer
	{
		mContainerList.Clear();
		int containerCount = containerList.Count;
		for (int i = 0; i < containerCount; ++i)
		{
			containerList[i].setIndex(i);
			containerList[i].initOrigin();
			containerList[i].setControlValue(i);
			mContainerList.Add(containerList[i]);
		}
		mDefaultFocus = defaultFocus;
	}
	public void update(float elapsedTime)
	{
		// 自动匀速滚动到目标点
		if (mState == SCROLL_STATE.SS_SCROLL_TARGET)
		{
			int preSign = sign(mTargetOffsetValue - mCurOffsetValue);
			mCurOffsetValue += elapsedTime * mScrollSpeed;
			int curSign = sign(mTargetOffsetValue - mCurOffsetValue);
			int speedSign = sign(mScrollSpeed);
			if (isFloatEqual(mTargetOffsetValue, mCurOffsetValue) || preSign != curSign && preSign == speedSign)
			{
				mCurOffsetValue = mTargetOffsetValue;
				mState = SCROLL_STATE.SS_NONE;
			}
			updateItem(mCurOffsetValue);
		}
		// 鼠标拖动
		else if (mState == SCROLL_STATE.SS_DRAGING)
		{
			float curOffset = mContainerList[mDefaultFocus].getControlValue() - mCurOffsetValue;
			if (mDragDirection == DRAG_DIRECTION.DD_HORIZONTAL)
			{
				scroll(curOffset + elapsedTime * mScrollSpeed);
			}
			else if (mDragDirection == DRAG_DIRECTION.DD_VERTICAL)
			{
				scroll(curOffset + elapsedTime * mScrollSpeed);
			}
		}
		// 鼠标抬起后自动减速到停止
		else if (mState == SCROLL_STATE.SS_SCROLL_TO_STOP)
		{
			if (!isFloatZero(mScrollSpeed))
			{
				mScrollSpeed = lerp(mScrollSpeed, 0.0f, elapsedTime * mAttenuateFactor, 0.1f);
				float curOffset = mContainerList[mDefaultFocus].getControlValue() - mCurOffsetValue;
				scroll(curOffset + elapsedTime * mScrollSpeed);
				// 当速度小于一定值时才开始选择聚焦到某一项
				if (mItemOnCenter)
				{
					if (Mathf.Abs(mScrollSpeed) < mFocusSpeedThreshhold)
					{
						int focusIndex = getItemIndex(curOffset, true, mLoop);
						if (focusIndex < 0 || focusIndex >= mItemList.Count)
						{
							focusIndex = 0;
						}
						float focusTime = Mathf.Abs((curOffset - mItemList[focusIndex].getCurControlValue()) / mScrollSpeed);
						clampMax(ref focusTime, 1.0f);
						scroll(focusIndex, focusTime);
						mState = SCROLL_STATE.SS_SCROLL_TARGET;
					}
				}
				// 逐渐减速到0
				else
				{
					if (isFloatZero(mScrollSpeed))
					{
						mState = SCROLL_STATE.SS_NONE;
					}
				}
			}
			// 被意外停止,则回到初始状态
			else
			{
				if (mItemOnCenter)
				{
					float curOffset = mContainerList[mDefaultFocus].getControlValue() - mCurOffsetValue;
					int focusIndex = getItemIndex(curOffset, true, mLoop);
					scroll(focusIndex, 0.3f);
					mState = SCROLL_STATE.SS_SCROLL_TARGET;
				}
				else
				{
					mState = SCROLL_STATE.SS_NONE;
				}
			}
		}
	}
	// 直接设置到指定位置
	public void scroll(float offset)
	{
		updateItem(mContainerList[mDefaultFocus].getControlValue() - offset);
	}
	public void scroll(int index)
	{
		scroll(mItemList[index].getCurControlValue());
	}
	// 滚动到指定下标
	public void scroll(int index, float time)
	{
		clamp(ref index, 0, mItemList.Count - 1);
		// 设置目标值
		mTargetOffsetValue = mContainerList[mDefaultFocus].getControlValue() - mItemList[index].getCurControlValue();
		if (mLoop)
		{
			if (Mathf.Abs(mTargetOffsetValue - mCurOffsetValue) > mMaxControlValue / 2.0f)
			{
				clampValue(ref mTargetOffsetValue, 0.0f, mMaxControlValue, mMaxControlValue);
				clampValue(ref mCurOffsetValue, 0.0f, mMaxControlValue, mMaxControlValue);
			}
		}
		mScrollSpeed = (mTargetOffsetValue - mCurOffsetValue) / time;
		if (!isFloatZero(mScrollSpeed))
		{
			mState = SCROLL_STATE.SS_SCROLL_TARGET;
		}
	}
	public float getCurOffsetValue() { return mCurOffsetValue; }
	public void setItemOnCenter(bool center) { mItemOnCenter = center; }
	public void setDragSensitive(float sensitive) { mDragSensitive = sensitive; }
	public void setFocusSpeedThreshhold(float threshold) { mFocusSpeedThreshhold = threshold; }
	//---------------------------------------------------------------------------------------------------------------------------------------
	protected void updateItem(float controlValue)
	{
		// 变化时需要随时更新当前值
		mCurOffsetValue = controlValue;
		if (mLoop)
		{
			clampValue(ref mCurOffsetValue, 0.0f, mMaxControlValue, mMaxControlValue);
		}
		int itemCount = mItemList.Count;
		for (int i = 0; i < itemCount; ++i)
		{
			float value = mItemList[i].getCurControlValue() + mCurOffsetValue;
			if(mLoop)
			{
				clampValue(ref value, 0, mMaxControlValue, mMaxControlValue);
			}
			else
			{
				clamp(ref value, 0, mMaxControlValue);
			}
			int containerIndex = getContainerIndex(value, false, mLoop);
			int nextcontainerIndex = containerIndex + 1;
			if(!isInRange(containerIndex, 0, mContainerList.Count - 1) || !isInRange(nextcontainerIndex, 0, mContainerList.Count - 1))
			{
				mItemList[i].setVisible(false);
				continue;
			}
			else
			{
				mItemList[i].setVisible(true);
			}
			float curItemOffsetValue = mContainerList[containerIndex].getControlValue();
			float nextItemOffsetValue = mContainerList[nextcontainerIndex].getControlValue();
			float percent = inverseLerp(curItemOffsetValue, nextItemOffsetValue, value);
			checkInt(ref percent);
			clamp(ref percent, 0.0f, 1.0f);
			mItemList[i].lerp(mContainerList[containerIndex], mContainerList[nextcontainerIndex], percent);
		}
	}
	// 根据controlValue查找在ItemList中的对应下标
	protected int getItemIndex(float controlValue, bool nearest, bool loop)
	{
		if (loop)
		{
			clampValue(ref controlValue, 0.0f, mMaxControlValue, mMaxControlValue);
		}
		int index = -1;
		int itemCount = mItemList.Count;
		for (int i = 0; i < itemCount; ++i)
		{
			if (isFloatEqual(mItemList[i].getCurControlValue(), controlValue))
			{
				index = i;
				break;
			}
			// 找到第一个比controlValue大的项
			if (mItemList[i].getCurControlValue() >= controlValue)
			{
				if (!nearest)
				{
					index = i - 1;
				}
				else
				{
					if (i - 1 > 0)
					{
						if (Mathf.Abs(mItemList[i].getCurControlValue() - controlValue) > Mathf.Abs(mItemList[i - 1].getCurControlValue() - controlValue))
						{
							index = i;
						}
						else
						{
							index = i - 1;
						}
					}
					else
					{
						index = i;
					}
				}
				break;
			}
		}
		return index;
	}
	// 根据controlValue查找在ContainerList中的对应下标,nearest为true则表示查找离该controlValue最近的下标
	protected int getContainerIndex(float controlValue, bool nearest, bool loop)
	{
		if (loop)
		{
			clampValue(ref controlValue, 0.0f, mMaxControlValue, mMaxControlValue);
		}
		int index = -1;
		int containerCount = mContainerList.Count;
		for (int i = 0; i < containerCount; ++i)
		{
			if (isFloatEqual(mContainerList[i].getControlValue(), controlValue))
			{
				index = i;
				break;
			}
			// 找到第一个比controlValue大的项
			if (mContainerList[i].getControlValue() >= controlValue)
			{
				if(!nearest)
				{
					index = i - 1;
				}
				else
				{
					if (i - 1 > 0)
					{
						if (Mathf.Abs(mContainerList[i].getControlValue() - controlValue) > Mathf.Abs(mContainerList[i - 1].getControlValue() - controlValue))
						{
							index = i;
						}
						else
						{
							index = i - 1;
						}
					}
					else
					{
						index = i;
					}
				}
				break;
			}
		}
		return index;
	}
	protected void onMouseDown(Vector2 mousePos)
	{
		mMouseDown = true;
		mState = SCROLL_STATE.SS_DRAGING;
		mScrollSpeed = 0.0f;
	}
	// 鼠标在屏幕上抬起
	protected void onScreenMouseUp(Vector2 mousePos)
	{
		mMouseDown = false;
		// 正在拖动时鼠标抬起,则开始逐渐减速到0
		if (mState == SCROLL_STATE.SS_DRAGING)
		{
			mState = SCROLL_STATE.SS_SCROLL_TO_STOP;
		}
	}
	protected void onMouseMove(Vector2 mousePos, Vector2 moveDelta, float moveSpeed)
	{
		// 鼠标未按下时不允许改变移动速度
		if (!mMouseDown)
		{
			return;
		}
		if (mDragDirection == DRAG_DIRECTION.DD_HORIZONTAL)
		{
			mScrollSpeed = sign(moveDelta.x) * moveSpeed * mDragSensitive;
		}
		else if(mDragDirection == DRAG_DIRECTION.DD_VERTICAL)
		{
			mScrollSpeed = sign(moveDelta.y) * moveSpeed * mDragSensitive;
			logInfo("move speed:" + floatToString(mScrollSpeed));
		}
		else
		{
			return;
		}
	}
	protected void onMouseStay(Vector2 mousePos)
	{
		if (!mMouseDown)
		{
			return;
		}
		mScrollSpeed = 0.0f;
	}
}