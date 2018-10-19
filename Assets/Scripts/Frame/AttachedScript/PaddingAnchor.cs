using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum ANCHOR_MODE
{
	AM_NONE,							// 无效值
	AM_PADDING_PARENT_SIDE,             // 停靠父节点的指定边界,并且大小不改变,0,1,2,3表示左上右下
	AM_NEAR_PARENT_SIDE,				// 将锚点设置到距离相对于父节点最近的边,并且各边界到父节点对应边界的距离固定不变
	AM_NEAR_PARENT_CENTER_FIXED_SIZE,	// 将锚点设置到相对于父节点的中心,并且大小不改变
	AM_NEAR_PARENT_CENTER_SCALE_SIZE,	// 将锚点设置到相对于父节点的中心,并且各边界距离父节点对应边界占的比例固定,但是如果父节点为空,则只能固定大小
}

// 当mAnchorMode的值为AM_NEAR_SIDE时,要停靠的边界
public enum NEAR_SIDE
{
	NS_LEFT,
	NS_TOP,
	NS_RIGHT,
	NS_BOTTOM,
}

[Serializable]
public class AnchorPoint
{
	// relative为0表示相对于中心,为1则表示相对于各条边,符号表示方向
	// 左右边界,1表示相对于父节点右边界,-1表示相对于父节点左边界
	// 上下边界,1表示相对于父节点上边界,-1表示相对于父节点下边界
	public float mRelative;
	public int mAbsolute;
	public void setRelative(float relative)
	{
		mRelative = relative;
	}
	public void setAbsolute(float absolute)
	{
		mAbsolute = (int)(absolute + 0.5f);
	}
}

// 该组件所在的物体不能有旋转,否则会计算错误
public class PaddingAnchor : MonoBehaviour
{
	protected bool mDirty = true;
	public ANCHOR_MODE mAnchorMode;
	protected NEAR_SIDE mNearSide;
	public Vector3[] mParentSides;
	// 左上右下的顺序
	public AnchorPoint[] mAnchorPoint;
	public ANCHOR_MODE _mAnchorMode
	{
		get
		{
			return mAnchorMode;
		}
		set
		{
			mAnchorMode = value;
			setAnchorMode(mAnchorMode);
		}
	}
	public NEAR_SIDE _mNearSide
	{
		get
		{
			return mNearSide;
		}
		set
		{
			mNearSide = value;
			setAnchorMode(mAnchorMode);
		}
	}
	public bool _mDirty
	{
		get
		{
			return mDirty;
		}
		set
		{
			mDirty = value;
		}
	}
	public static void forceUpdateChildren(GameObject obj)
	{
		// 先更新自己
		if (obj.GetComponent<PaddingAnchor>() != null)
		{
			obj.GetComponent<PaddingAnchor>().updateRect(true);
		}
		// 然后更新所有子节点
		Transform curTrans = obj.transform;
		int childCount = curTrans.childCount;
		for (int i = 0; i < childCount; ++i)
		{
			forceUpdateChildren(curTrans.GetChild(i).gameObject);
		}
	}

	public void setAnchorMode(ANCHOR_MODE mode)
	{
		mAnchorMode = mode;
		if (mAnchorPoint == null)
		{
			mAnchorPoint = new AnchorPoint[4] { new AnchorPoint(), new AnchorPoint(), new AnchorPoint(), new AnchorPoint() };
		}
		if (mAnchorMode == ANCHOR_MODE.AM_PADDING_PARENT_SIDE)
		{
			setToPaddingParentSide(mNearSide);
		}
		else if (mAnchorMode == ANCHOR_MODE.AM_NEAR_PARENT_SIDE)
		{
			setToNearParentSides();
		}
		else if (mAnchorMode == ANCHOR_MODE.AM_NEAR_PARENT_CENTER_FIXED_SIZE)
		{
			setToNearParentCenterFixedSize();
		}
		else if (mAnchorMode == ANCHOR_MODE.AM_NEAR_PARENT_CENTER_SCALE_SIZE)
		{
			setToNearParentCenterScaleSize();
		}
	}
	//------------------------------------------------------------------------------------------------------------------------------------------------
	// 将锚点设置到距离相对于父节点最近的边,并且各边界到父节点对应边界的距离固定不变
	protected void setToNearParentSides()
	{
		UIRect parentRect = WidgetUtility.findParentRect(gameObject);
		if(parentRect == null)
		{
			Vector3[] sides = getSides(null);
			for (int i = 0; i < 4; ++i)
			{
				mAnchorPoint[i].setRelative(0.0f);
				if(i == 0 || i == 2)
				{
					mAnchorPoint[i].setAbsolute(MathUtility.getLength(sides[i]));
				}
				else if(i == 1 || i == 3)
				{
					mAnchorPoint[i].setAbsolute(MathUtility.getLength(sides[i]));
				}
			}
			return;
		}
		else
		{
			GameObject parent = parentRect.gameObject;
			Vector3[] sides = getSides(parent);
			Vector3[] parentSides = WidgetUtility.getParentSides(parent);
			for(int i = 0; i < 4; ++i)
			{
				if (i == 0 || i== 2)
				{
					float relativeLeft = sides[i].x - parentSides[0].x;
					float relativeCenter = sides[i].x;
					float relativeRight = sides[i].x - parentSides[2].x;
					float disToLeft = Mathf.Abs(relativeLeft);
					float disToCenter = Mathf.Abs(relativeCenter);
					float disToRight = Mathf.Abs(relativeRight);
					// 靠近左边
					if (disToLeft < disToCenter && disToLeft < disToRight)
					{
						mAnchorPoint[i].setRelative(-1.0f);
						mAnchorPoint[i].setAbsolute(relativeLeft);
					}
					// 靠近右边
					else if (disToRight < disToLeft && disToRight < disToCenter)
					{
						mAnchorPoint[i].setRelative(1.0f);
						mAnchorPoint[i].setAbsolute(relativeRight);
					}
					// 靠近中心
					else
					{
						mAnchorPoint[i].setRelative(0.0f);
						mAnchorPoint[i].setAbsolute(relativeCenter);
					}
				}
				else if (i == 1 || i == 3)
				{
					float relativeTop = sides[i].y - parentSides[1].y;
					float relativeCenter = sides[i].y;
					float relativeBottom = sides[i].y - parentSides[3].y;
					float disToTop = Mathf.Abs(relativeTop);
					float disToCenter = Mathf.Abs(relativeCenter);
					float disToBottom = Mathf.Abs(relativeBottom);
					// 靠近顶部
					if (disToTop < disToCenter && disToTop < disToBottom)
					{
						mAnchorPoint[i].setRelative(1.0f);
						mAnchorPoint[i].setAbsolute(relativeTop);
					}
					// 靠近底部
					else if (disToBottom < disToTop && disToBottom < disToCenter)
					{
						mAnchorPoint[i].setRelative(-1.0f);
						mAnchorPoint[i].setAbsolute(relativeBottom);
					}
					// 靠近中心
					else
					{
						mAnchorPoint[i].setRelative(0.0f);
						mAnchorPoint[i].setAbsolute(relativeCenter);
					}
				}
			}
		}
	}
	// 将锚点设置到相对于父节点的中心,并且大小不改变
	protected void setToNearParentCenterFixedSize()
	{
		Vector3[] sides = null;
		UIRect parentRect = WidgetUtility.findParentRect(gameObject);
		if (parentRect == null)
		{
			sides = getSides(null);
		}
		else
		{
			sides = getSides(parentRect.gameObject);
		}
		for (int i = 0; i < 4; ++i)
		{
			mAnchorPoint[i].setRelative(0.0f);
			if (i == 0 || i == 2)
			{
				mAnchorPoint[i].setAbsolute(sides[i].x);
			}
			else
			{
				mAnchorPoint[i].setAbsolute(sides[i].y);
			}
		}
	}
	// 将锚点设置到相对于父节点的中心,并且各边界距离父节点对应边界占的比例固定,但是如果父节点为空,则只能固定大小
	protected void setToNearParentCenterScaleSize()
	{
		UIRect parentRect = WidgetUtility.findParentRect(gameObject);
		if (parentRect == null)
		{
			Vector3[] sides = getSides(null);
			for (int i = 0; i < 4; ++i)
			{
				mAnchorPoint[i].setRelative(0.0f);
				if (i == 0 || i == 2)
				{
					mAnchorPoint[i].setAbsolute(sides[i].x);
				}
				else
				{
					mAnchorPoint[i].setAbsolute(sides[i].y);
				}
			}
		}
		else
		{
			GameObject parent = parentRect.gameObject;
			Vector3[] sides = getSides(parent);
			Vector3[] parentSides = WidgetUtility.getParentSides(parent);
			for (int i = 0; i < 4; ++i)
			{
				mAnchorPoint[i].setAbsolute(0.0f);
				if (i == 0 || i == 2)
				{
					mAnchorPoint[i].setRelative(sides[i].x / parentSides[2].x);
				}
				else
				{
					mAnchorPoint[i].setRelative(sides[i].x / parentSides[1].y);
				}
			}
		}
	}
	// 停靠父节点的指定边界,并且大小不改变,0,1,2,3表示左上右下
	protected void setToPaddingParentSide(NEAR_SIDE side)
	{
		UIRect parentRect = WidgetUtility.findParentRect(gameObject);
		if(parentRect == null)
		{
			Vector3[] sides = getSides(null);
			for (int i = 0; i < 4; ++i)
			{
				mAnchorPoint[i].setRelative(0.0f);
				if (i == 0 || i == 2)
				{
					mAnchorPoint[i].setAbsolute(sides[i].x);
				}
				else
				{
					mAnchorPoint[i].setAbsolute(sides[i].y);
				}
			}
		}
		else
		{
			GameObject parent = parentRect.gameObject;
			Vector3[] sides = getSides(parent);
			Vector3[] parentSides = WidgetUtility.getParentSides(parent);
			// 相对于左右边界
			if (side == NEAR_SIDE.NS_LEFT || side == NEAR_SIDE.NS_RIGHT)
			{
				for (int i = 0; i < 4; ++i)
				{
					if (i == 0 || i == 2)
					{
						mAnchorPoint[i].setRelative((side == NEAR_SIDE.NS_LEFT) ? -1.0f : 1.0f);
						mAnchorPoint[i].setAbsolute(sides[i].x - parentSides[(int)side].x);
					}
					else
					{
						mAnchorPoint[i].setRelative(0.0f);
						mAnchorPoint[i].setAbsolute(sides[i].y);
					}
				}
			}
			// 相对于上下边界
			else if(side == NEAR_SIDE.NS_TOP || side == NEAR_SIDE.NS_BOTTOM)
			{
				for (int i = 0; i < 4; ++i)
				{
					if (i == 0 || i == 2)
					{
						mAnchorPoint[i].setRelative(0.0f);
						mAnchorPoint[i].setAbsolute(sides[i].x);
					}
					else
					{
						mAnchorPoint[i].setRelative((side == NEAR_SIDE.NS_TOP) ? 1.0f : -1.0f);
						mAnchorPoint[i].setAbsolute(sides[i].y - parentSides[(int)side].y);
					}
				}
			}
		}
	}
	protected void updateRect(bool force = false)
	{
		if (!force && !mDirty)
		{
			return;
		}
		mDirty = false;
		float width = 0.0f;
		float height = 0.0f;
		Vector3 pos = Vector3.zero;
		UIRect parentRect = WidgetUtility.findParentRect(gameObject);
		if (parentRect != null)
		{
			GameObject parent = parentRect.gameObject;
			mParentSides = WidgetUtility.getParentSides(parent);
			float thisLeft = mAnchorPoint[0].mRelative * mParentSides[2].x + mAnchorPoint[0].mAbsolute;
			float thisRight = mAnchorPoint[2].mRelative * mParentSides[2].x + mAnchorPoint[2].mAbsolute;
			float thisTop = mAnchorPoint[1].mRelative * mParentSides[1].y + mAnchorPoint[1].mAbsolute;
			float thisBottom = mAnchorPoint[3].mRelative * mParentSides[1].y + mAnchorPoint[3].mAbsolute;
			width = thisRight - thisLeft;
			height = thisTop - thisBottom;
			pos.x = (thisRight + thisLeft) / 2.0f;
			pos.y = (thisTop + thisBottom) / 2.0f;
		}
		else
		{
			width = mAnchorPoint[2].mAbsolute - mAnchorPoint[0].mAbsolute;
			height = mAnchorPoint[1].mAbsolute - mAnchorPoint[3].mAbsolute;	
		}
		if (width < 0)
		{
			UnityUtility.logError("width error in anchor!");
		}
		if (height < 0)
		{
			UnityUtility.logError("height error in anchor!");
		}
		UIWidget thisWidget = WidgetUtility.getGameObjectWidget(gameObject);
		// 没有widget则是panel,panel是没有宽高的
		if (thisWidget != null)
		{
			thisWidget.width = (int)(width + 0.5f);
			thisWidget.height = (int)(height + 0.5f);
		}
		Transform thisTrans = gameObject.transform;
		thisTrans.localPosition = pos;
	}
	// 本地坐标系下的的各条边
	protected Vector3[] getSides(GameObject parent)
	{
		// 如果自身带有旋转,则需要还原自身的变换
		Vector3[] sides = new Vector3[4] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
		UIRect thisRect = WidgetUtility.getGameObjectRect(gameObject);
		Vector3[] worldCorners = thisRect.worldCorners;
		Vector3[] localCorners = new Vector3[4];
		for (int i = 0; i < 4; ++i)
		{
			if (parent != null)
			{
				localCorners[i] = parent.transform.InverseTransformPoint(worldCorners[i]);
			}
			else
			{
				localCorners[i] = worldCorners[i];
			}
		}
		for (int i = 0; i < 4; ++i)
		{
			sides[i] = (localCorners[i] + localCorners[(i + 1) % 4]) / 2;
		}
		return sides;
	}
	
}