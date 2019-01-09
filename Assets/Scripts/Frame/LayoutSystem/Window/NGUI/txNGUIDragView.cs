using UnityEngine;
using System.Collections;

public enum DRAG_DIRECTION
{
	DD_HORIZONTAL,
	DD_VERTICAL,
	DD_FREE,
}

public enum CLAMP_TYPE
{
	CT_CENTER,	// 限制中心点不能超过父窗口的区域
	CT_EDGE,	// 限制边界不能超过父窗口的区域
}

public delegate void OnDragCallback();

public class txNGUIDragView : txNGUITexture
{
	protected float mMoveSpeed;
	protected Vector3 mMoveNormal;
	protected DRAG_DIRECTION mDragDirection;
	protected CLAMP_TYPE mClampType;
	protected Vector3 mMinRelativePos;         // 左边界和下边界或者窗口中心的最小值,具体需要由mClampType决定,-1到1的相对值,相对于父窗口的宽高
	protected Vector3 mMaxRelativePos;         // 右边界和上边界或者窗口中心的最大值,具体需要由mClampType决定,-1到1的相对值,相对于父窗口的宽高
	protected bool mMouseDown = false;         // 鼠标是否在窗口内按下,鼠标抬起或者离开窗口都会设置为false,鼠标按下时,跟随鼠标移动,鼠标放开时,按惯性移动
	protected float mMoveSpeedScale = 1.0f;
	protected float mAttenuateFactor = 5.0f;   // 移动速度衰减系数,鼠标在放开时移动速度会逐渐降低,衰减系数越大.速度降低越快
	protected OnDragCallback mDragingCallback;
	protected OnDragCallback mReleaseDragCallback;
	protected OnDragCallback mPositionChangeCallback;
	protected Vector3 mMousePressPosition;
	protected Vector3 mMouseDownWindowPosition;
	protected Vector3 mCurMousePosition;
	// 为true表示DragView只能在父节点的区域内滑动，父节点区域小于DragView区域时不能滑动
	// false表示DragView只能在父节点的区域外滑动,父节点区域大于DragView区域时不能滑动
	protected bool mClampInner = true;
	protected bool mAutoMoveToEdge = false; // 是否自动停靠到最近的边
	protected float mMoveToEdgeSpeed = 5.0f;// 自动停靠到最近的边的速度
	public txNGUIDragView()
	{
		mDragDirection = DRAG_DIRECTION.DD_HORIZONTAL;
		mClampType = CLAMP_TYPE.CT_EDGE;
		mMinRelativePos = -Vector3.one;
		mMaxRelativePos = Vector3.one;
		mReceiveScreenMouse = true;
	}
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		if(mBoxCollider == null)
		{
			logError("DragView must have BoxCollider!");
		}
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
		if(mMouseDown && mDragingCallback != null)
		{
			mDragingCallback();
		}
		// 按照当前滑动速度移动
		if (mMoveSpeed > 0.0f)
		{
			// 只有鼠标未按下并且不自动停靠到最近的边时才衰减速度
			Vector3 curPosition = getPosition();
			if (!mMouseDown && !mAutoMoveToEdge)
			{
				mMoveSpeed = lerp(mMoveSpeed, 0.0f, elapsedTime * mAttenuateFactor, 0.001f);
				curPosition += mMoveNormal * mMoveSpeed * mMoveSpeedScale * elapsedTime;
			}
			// 鼠标为按下状态时,鼠标移动量就是窗口的移动量,此处未考虑父窗口的缩放不为1的情况
			else
			{
				curPosition = mMouseDownWindowPosition + mCurMousePosition - mMousePressPosition;
			}
			clampPosition(ref curPosition);
			setLocalPosition(curPosition);
			if(mPositionChangeCallback != null)
			{
				mPositionChangeCallback();
			}
		}
		// 自动停靠最近的边
		if (!mMouseDown && mAutoMoveToEdge)
		{
			Vector3[] minMaxPos = getLocalMinMaxPixelPos();
			Vector2 minPos = minMaxPos[0];
			Vector2 maxPos = minMaxPos[1];
			Vector3 curPosition = getPosition();
			Vector3 targetPosition = curPosition;
			// 获得当前最近的边
			if (mDragDirection == DRAG_DIRECTION.DD_HORIZONTAL)
			{
				if (Mathf.Abs(curPosition.x - minPos.x) < Mathf.Abs(curPosition.x - maxPos.x))
				{
					targetPosition.x = minPos.x;
				}
				else
				{
					targetPosition.x = maxPos.x;
				}
			}
			else if (mDragDirection == DRAG_DIRECTION.DD_VERTICAL)
			{
				if (Mathf.Abs(curPosition.y - minPos.y) < Mathf.Abs(curPosition.y - maxPos.y))
				{
					targetPosition.y = minPos.y;
				}
				else
				{
					targetPosition.y = maxPos.y;
				}
			}
			else
			{
				float[] disArray = new float[4];
				disArray[0] = Mathf.Abs(curPosition.x - minPos.x);
				disArray[1] = Mathf.Abs(curPosition.x - maxPos.x);
				disArray[2] = Mathf.Abs(curPosition.y - minPos.y);
				disArray[3] = Mathf.Abs(curPosition.y - maxPos.y);
				int minIndex = -1;
				float minDistance = 0.0f;
				for (int i = 0; i < 4; ++i)
				{
					if (minIndex < 0 || minDistance > disArray[i])
					{
						minIndex = i;
						minDistance = disArray[i];
					}
				}
				if (minIndex == 0)
				{
					targetPosition.x = minPos.x;
				}
				else if (minIndex == 1)
				{
					targetPosition.x = maxPos.x;
				}
				else if (minIndex == 2)
				{
					targetPosition.y = minPos.y;
				}
				else if (minIndex == 3)
				{
					targetPosition.y = maxPos.y;
				}
			}
			Vector3 newPos = lerp(curPosition, targetPosition, elapsedTime * mMoveToEdgeSpeed);
			setLocalPosition(newPos);
			if (mPositionChangeCallback != null)
			{
				mPositionChangeCallback();
			}
		}
	}
	public override void onMouseDown(Vector2 mousePos)
	{
		mMouseDown = true;
		mMoveSpeed = 0.0f;
		mMousePressPosition = mousePos;
		mMouseDownWindowPosition = getPosition();
	}
	// 鼠标在屏幕上抬起
	public override void onScreenMouseUp(Vector2 mousePos)
	{
		mMouseDown = false;
		if(mReleaseDragCallback != null)
		{
			mReleaseDragCallback();
		}
	}
	public override void onMouseMove(Vector2 mousePos, Vector2 moveDelta, float moveSpeed)
	{
		// 鼠标未按下时不允许改变移动速度
		if(!mMouseDown)
		{
			return;
		}
		mMoveSpeed = moveSpeed;
		if(mDragDirection == DRAG_DIRECTION.DD_HORIZONTAL)
		{
			moveDelta.y = 0.0f;
		}
		else if(mDragDirection == DRAG_DIRECTION.DD_VERTICAL)
		{
			moveDelta.x = 0.0f;
		}
		mMoveNormal = normalize(moveDelta);
		mCurMousePosition = mousePos;
		if (mDragDirection == DRAG_DIRECTION.DD_HORIZONTAL)
		{
			mCurMousePosition.y = mMousePressPosition.y;
		}
		else if (mDragDirection == DRAG_DIRECTION.DD_VERTICAL)
		{
			mCurMousePosition.x = mMousePressPosition.x;
		}
	}
	public override void onMouseStay(Vector2 mousePos)
	{
		// 鼠标在窗口内为按下状态,并且没有移动时,确保速度为0
		mMoveSpeed = 0.0f;
	}
	public void setClampInner(bool inner) { mClampInner = inner; }
	public void setDragDirection(DRAG_DIRECTION direction) { mDragDirection = direction; }
	public void setMaxRelativePos(Vector3 max){mMaxRelativePos = max;}
	public void setMinRelativePos(Vector3 min){ mMinRelativePos = min;}
	public void setMoveSpeedScale(float scale) { mMoveSpeedScale = scale; }
	public void setDragingCallback(OnDragCallback draging) { mDragingCallback = draging; }
	public void setReleaseDragCallback(OnDragCallback releaseDrag) { mReleaseDragCallback = releaseDrag; }
	public void setPositionChangeCallback(OnDragCallback positionChange) { mPositionChangeCallback = positionChange; }
	public void setClampType(CLAMP_TYPE clampType) { mClampType = clampType; }
	public bool isDraging() { return mMouseDown; }
	public DRAG_DIRECTION getDragDirection() { return mDragDirection; }
	public Vector3 getMaxRelativePos() { return mMaxRelativePos; }
	public Vector3 getMinRelativePos() { return mMinRelativePos; }
	public void setAutoMoveToEdge(bool autoMove) { mAutoMoveToEdge = autoMove; }
	//------------------------------------------------------------------------------------------------------------------------------------------
	public Vector3[] getLocalMinMaxPixelPos()
	{
		// 获得第一个带widget的父节点的rect
		UIRect rect = WidgetUtility.findParentRect(mObject);
		Vector2 parentWidgetSize = WidgetUtility.getRectSize(rect);
		// 计算父节点的世界缩放
		Vector3 worldScale = getMatrixScale(mTransform.parent.localToWorldMatrix);
		txUIObject root = mLayout.isNGUI() ? mLayoutManager.getNGUIRoot() : mLayoutManager.getUGUIRoot();
		Vector3 uiRootScale = root.getTransform().localScale;
		Vector2 parentScale = new Vector2(worldScale.x / uiRootScale.x, worldScale.y / uiRootScale.y);
		// 计算移动的位置范围
		Vector2 minPos = new Vector2(parentWidgetSize.x / 2.0f * mMinRelativePos.x / parentScale.x, parentWidgetSize.y / 2.0f * mMinRelativePos.y / parentScale.y);
		Vector2 maxPos = new Vector2(parentWidgetSize.x / 2.0f * mMaxRelativePos.x / parentScale.x, parentWidgetSize.y / 2.0f * mMaxRelativePos.y / parentScale.y);
		if (mClampType == CLAMP_TYPE.CT_EDGE)
		{
			Vector2 thisSize = getWindowSize(true);
			minPos += thisSize / 2.0f;
			maxPos -= thisSize / 2.0f;
			if (!mClampInner)
			{
				swap(ref minPos, ref maxPos);
			}
		}
		else if (mClampType == CLAMP_TYPE.CT_CENTER)
		{ }
		return new Vector3[2] { minPos, maxPos };
	}
	protected void clampPosition(ref Vector3 position)
	{
		// 计算移动的位置范围
		Vector3[] minMaxPos = getLocalMinMaxPixelPos();
		Vector2 minPos = minMaxPos[0];
		Vector2 maxPos = minMaxPos[1];
		if (mDragDirection == DRAG_DIRECTION.DD_HORIZONTAL || mDragDirection == DRAG_DIRECTION.DD_FREE)
		{
			// 滑动范围有效时需要限定在一定范围
			if (minPos.x <= maxPos.x)
			{
				clamp(ref position.x, minPos.x, maxPos.x);
			}
			// 滑动范围无效时,固定位置
			else
			{
				position.x = getPosition().x;
			}
		}
		if (mDragDirection == DRAG_DIRECTION.DD_VERTICAL || mDragDirection == DRAG_DIRECTION.DD_FREE)
		{
			// 滑动范围有效时需要限定在一定范围
			if (minPos.y <= maxPos.y)
			{
				clamp(ref position.y, minPos.y, maxPos.y);
			}
			// 滑动范围无效时,固定位置
			else
			{
				position.y = getPosition().y;
			}
		}
	}
}