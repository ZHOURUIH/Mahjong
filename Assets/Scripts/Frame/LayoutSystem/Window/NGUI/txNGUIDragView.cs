using UnityEngine;
using System.Collections;

public enum DRAG_DIRECTION
{
	DD_HORIZONTAL,
	DD_VERTICAL,
	DD_FREE,
}

public delegate void OnDraging();

public class txNGUIDragView : txNGUIStaticTexture
{
	protected float mMoveSpeed;
	protected Vector3 mMoveNormal;
	protected DRAG_DIRECTION mDragDirection;
	protected Vector3 mMinPos;                 // 右边界和上边界最小的坐标,-1到1的相对值,相对于父窗口的宽高
	protected Vector3 mMaxPos;                 // 左边界和下边界最大的坐标,-1到1的相对值,相对于父窗口的宽高
	protected bool mMouseDown = false;         // 鼠标是否在窗口内按下,鼠标抬起或者离开窗口都会设置为false,鼠标按下时,跟随鼠标移动,鼠标放开时,按惯性移动
	protected float mMoveSpeedScale = 1.0f;
	protected float mAttenuateFactor = 5.0f;   // 移动速度衰减系数,鼠标在放开时移动速度会逐渐降低,衰减系数越大.速度降低越快
	protected OnDraging mDragingCallback;
	public txNGUIDragView()
	{
		mType = UI_TYPE.UI_NGUI_DRAG_VIEW;
		mDragDirection = DRAG_DIRECTION.DD_HORIZONTAL;
		mMinPos = Vector3.zero;
		mMaxPos = Vector3.zero;
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
		if(mMoveSpeed > 0.0f)
		{
			// 只有鼠标未按下时才衰减速度
			if(!mMouseDown)
			{
				mMoveSpeed = MathUtility.lerp(mMoveSpeed, 0.0f, elapsedTime * 5.0f, 0.01f);
			}
			Vector3 curPosition = getPosition();
			curPosition += mMoveNormal * mMoveSpeed * mMoveSpeedScale * elapsedTime;
			// 获得第一个带widget的父节点的rect
			UIRect rect = CustomAnchor.findParentRect(mObject);
			Vector2 parentWidgetSize = CustomAnchor.getRectSize(rect);
			// 计算父节点的世界缩放
			Vector3 worldScale = MathUtility.getMatrixScale(mTransform.parent.localToWorldMatrix);
			txUIObject root = mLayout.isNGUI() ? mLayoutManager.getNGUIRoot() : mLayoutManager.getUGUIRoot();
			Vector3 uiRootScale = root.getTransform().localScale;
			Vector2 parentScale = new Vector2(worldScale.x / uiRootScale.x, worldScale.y / uiRootScale.y);
			// 计算移动的位置范围
			Vector2 minPos = new Vector2(parentWidgetSize.x / 2.0f * mMinPos.x, parentWidgetSize.y / 2.0f * mMinPos.y);
			Vector2 maxPos = new Vector2(parentWidgetSize.x / 2.0f * mMaxPos.x, parentWidgetSize.y / 2.0f * mMaxPos.y);
			Vector2 thisSize = getWindowSize(true);
			float minX = (minPos.x - thisSize.x / 2.0f) / parentScale.x;
			float maxX = (maxPos.x + thisSize.x / 2.0f) / parentScale.x;
			float minY = (minPos.y - thisSize.y / 2.0f) / parentScale.y;
			float maxY = (maxPos.y + thisSize.y / 2.0f) / parentScale.y;
			if (mDragDirection == DRAG_DIRECTION.DD_HORIZONTAL || mDragDirection == DRAG_DIRECTION.DD_FREE)
			{
				// 有可以滑动范围时需要限定在一定范围
				if(minX <= maxX)
				{
					MathUtility.clamp(ref curPosition.x, minX, maxX);
				}
				// 不能滑动时,固定位置
				else
				{
					curPosition.x = getPosition().x;
				}
			}
			if(mDragDirection == DRAG_DIRECTION.DD_VERTICAL || mDragDirection == DRAG_DIRECTION.DD_FREE)
			{
				// 有可以滑动范围时需要限定在一定范围
				if (minY <= maxY)
				{
					MathUtility.clamp(ref curPosition.y, minY, maxY);
				}
				// 不能滑动时,固定位置
				else
				{
					curPosition.y = getPosition().y;
				}
			}
			setLocalPosition(curPosition);
		}
	}
	public override void onMouseLeave()
	{
		base.onMouseLeave();
		mMouseDown = false;
	}
	public override void onMouseDown(Vector2 mousePos)
	{
		mMouseDown = true;
		mMoveSpeed = 0.0f;
	}
	public override void onMouseUp(Vector2 mousePos)
	{
		mMouseDown = false;
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
		mMoveNormal = MathUtility.normalize(moveDelta);
	}
	public override void onMouseStay(Vector2 mousePos)
	{
		// 鼠标在窗口内为按下状态,并且没有移动时,确保速度为0
		mMoveSpeed = 0.0f;
	}
	public void setDragDirection(DRAG_DIRECTION direction) { mDragDirection = direction; }
	public void setMaxPos(Vector3 max){mMaxPos = max;}
	public void setMinPos(Vector3 min){mMinPos = min;}
	public void setMoveSpeedScale(float scale) { mMoveSpeedScale = scale; }
	public void setDragingCallback(OnDraging draging) { mDragingCallback = draging; }
}