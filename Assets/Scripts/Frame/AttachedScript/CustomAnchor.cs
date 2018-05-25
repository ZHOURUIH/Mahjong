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

public class SetPropertyAttribute : PropertyAttribute
{
	public string Name { get; private set; }
	public bool IsDirty { get; set; }

	public SetPropertyAttribute(string name)
	{
		this.Name = name;
	}
}

[ExecuteInEditMode]
public class CustomAnchor : MonoBehaviour
{
	protected ANCHOR_MODE mAnchorMode = ANCHOR_MODE.AM_NONE;
	protected int mNearSide = 0;    // 当mAnchorMode的值为AM_NEAR_SIDE时,要停靠的边界,0,1,2,3表示左,上,右,下
	// 左上右下的顺序
	public AnchorPoint[] mAnchorPoint = new AnchorPoint[4] { new AnchorPoint(), new AnchorPoint(), new AnchorPoint(), new AnchorPoint()};
	public ANCHOR_MODE _mAnchorMode
	{
		get
		{
			return mAnchorMode;
		}
		set
		{
			mAnchorMode = value;
			if (mAnchorMode == ANCHOR_MODE.AM_PADDING_PARENT_SIDE)
			{
				setToPaddingParentSide(mNearSide);
			}
			else if(mAnchorMode == ANCHOR_MODE.AM_NEAR_PARENT_SIDE)
			{
				setToNearParentSides();
			}
			else if(mAnchorMode == ANCHOR_MODE.AM_NEAR_PARENT_CENTER_FIXED_SIZE)
			{
				setToNearParentCenterFixedSize();
			}
			else if(mAnchorMode == ANCHOR_MODE.AM_NEAR_PARENT_CENTER_SCALE_SIZE)
			{
				setToNearParentCenterScaleSize();
			}
		}
	}
	
	public int _mNearSide
	{
		get
		{
			return mNearSide;
		}
		set
		{
			mNearSide = value;
			if(mAnchorMode == ANCHOR_MODE.AM_PADDING_PARENT_SIDE)
			{
				setToPaddingParentSide(mNearSide);
			}
		}
	}
	public void Start()
	{
		;
	}
	public void OnEnable()
	{
		updateRect();
	}
	// 将锚点设置到距离相对于父节点最近的边,并且各边界到父节点对应边界的距离固定不变
	public void setToNearParentSides()
	{
		UIRect parentRect = findRectParent(gameObject);
		if(parentRect == null)
		{
			Vector3[] sides = getSides(null);
			for (int i = 0; i < 4; ++i)
			{
				mAnchorPoint[i].setRelative(0.0f);
				if(i == 0 || i == 2)
				{
					mAnchorPoint[i].setAbsolute(sides[i].x);
				}
				else if(i == 1 || i == 3)
				{
					mAnchorPoint[i].setAbsolute(sides[i].y);
				}
			}
			return;
		}
		else
		{
			GameObject parent = parentRect.gameObject;
			Vector3[] sides = getSides(parent);
			Vector3[] parentSides = getParentSides(parent);
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
	public void setToNearParentCenterFixedSize()
	{
		Vector3[] sides = null;
		UIRect parentRect = findRectParent(gameObject);
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
	public void setToNearParentCenterScaleSize()
	{
		UIRect parentRect = findRectParent(gameObject);
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
			Vector3[] parentSides = getParentSides(parent);
			for (int i = 0; i < 4; ++i)
			{
				mAnchorPoint[i].setAbsolute(0.0f);
				if (i == 0 || i == 2)
				{
					mAnchorPoint[i].setRelative(sides[i].x / parentSides[i].x);
				}
				else
				{
					mAnchorPoint[i].setRelative(sides[i].y / parentSides[i].y);
				}
			}
		}
	}
	// 停靠父节点的指定边界,并且大小不改变,0,1,2,3表示左上右下
	public void setToPaddingParentSide(int side)
	{
		UIRect parentRect = findRectParent(gameObject);
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
			Vector3[] parentSides = getParentSides(parent);
			// 相对于左右边界
			if (side == 0 || side == 2)
			{
				for (int i = 0; i < 4; ++i)
				{
					if (i == 0 || i == 2)
					{
						mAnchorPoint[i].setRelative((side == 0) ? -1.0f : 1.0f);
						mAnchorPoint[i].setAbsolute(sides[i].x - parentSides[side].x);
					}
					else
					{
						mAnchorPoint[i].setRelative(0.0f);
						mAnchorPoint[i].setAbsolute(sides[i].y);
					}
				}
			}
			// 相对于上下边界
			else if(side == 1 || side == 3)
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
						mAnchorPoint[i].setRelative((side == 1) ? 1.0f : -1.0f);
						mAnchorPoint[i].setAbsolute(sides[i].y - parentSides[side].y);
					}
				}
			}
		}
	}
	//------------------------------------------------------------------------------------------------------------------------------------------------
	protected void updateRect()
	{
		// 如果还没有设置自适应方式,则查找父节点和设置默认的自适应方式
		if(_mAnchorMode == ANCHOR_MODE.AM_NONE)
		{
			_mAnchorMode = ANCHOR_MODE.AM_NEAR_PARENT_SIDE;
		}
		float width = 0.0f;
		float height = 0.0f;
		Vector3 pos = Vector3.zero;
		UIRect parentRect = findRectParent(gameObject);
		if (parentRect != null)
		{
			GameObject parent = parentRect.gameObject;
			Vector3[] parentSides = getParentSides(parent);
			float thisLeft = mAnchorPoint[0].mRelative * parentSides[2].x + mAnchorPoint[0].mAbsolute;
			float thisRight = mAnchorPoint[2].mRelative * parentSides[2].x + mAnchorPoint[2].mAbsolute;
			float thisTop = mAnchorPoint[1].mRelative * parentSides[1].y + mAnchorPoint[1].mAbsolute;
			float thisBottom = mAnchorPoint[3].mRelative * parentSides[1].y + mAnchorPoint[3].mAbsolute;
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
		UIWidget thisWidget = getGameObjectWidget(gameObject);
		thisWidget.width = (int)(width + 0.5f);
		thisWidget.height = (int)(height + 0.5f);
		Transform thisTrans = gameObject.transform;
		thisTrans.localPosition = pos;
	}
	// 本地坐标系下的的各条边
	protected Vector3[] getSides(GameObject parent)
	{
		// 如果自身带有旋转,则需要还原自身的变换
		Vector3[] sides = new Vector3[4] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
		UIWidget thisRect = getGameObjectWidget(gameObject);
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
	// 父节点在父节点坐标系下的各条边
	protected Vector3[] getParentSides(GameObject parent)
	{
		Vector3[] sides = new Vector3[4] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero};
		UIRect parentRect = getGameObjectRect(parent);
		if(parentRect == null)
		{
			return sides;
		}
		Vector3[] worldCorners = parentRect.worldCorners;
		for(int i = 0; i < 4; ++i)
		{
			sides[i] = (worldCorners[i] + worldCorners[(i + 1) % 4]) / 2;
			sides[i] = parent.transform.InverseTransformPoint(sides[i]);
		}
		return sides;
	}
	protected UIRect findRectParent(GameObject obj)
	{
		if (obj == null)
		{
			return null;
		}
		if(obj.transform.parent == null)
		{
			return null;
		}
		GameObject parent = obj.transform.parent.gameObject;
		if (parent != null)
		{
			// 自己有父节点,并且父节点有UIRect,则返回父节点的UIRect
			UIRect widget = getGameObjectRect(parent);
			if (widget != null)
			{
				return widget;
			}
			// 父节点没有UIRect,则继续往上找
			else
			{
				return findRectParent(parent);
			}
		}
		else
		{
			return null;
		}
	}
	public static UIWidget getGameObjectWidget(GameObject obj)
	{
		UILabel label = obj.GetComponent<UILabel>();
		if (label != null)
		{
			return label;
		}
		UITexture texture = obj.GetComponent<UITexture>();
		if (texture != null)
		{
			return texture;
		}
		UISprite sprite = obj.GetComponent<UISprite>();
		if (sprite != null)
		{
			return sprite;
		}
		UI2DSprite sprite2D = obj.GetComponent<UI2DSprite>();
		if (sprite2D != null)
		{
			return sprite2D;
		}
		return null;
	}
	public static UIRect getGameObjectRect(GameObject obj)
	{
		UIPanel panel = obj.GetComponent<UIPanel>();
		if (panel != null)
		{
			return panel;
		}
		UILabel label = obj.GetComponent<UILabel>();
		if (label != null)
		{
			return label;
		}
		UITexture texture = obj.GetComponent<UITexture>();
		if(texture != null)
		{
			return texture;
		}
		UISprite sprite = obj.GetComponent<UISprite>();
		if(sprite != null)
		{
			return sprite;
		}
		UI2DSprite sprite2D = obj.GetComponent<UI2DSprite>();
		if(sprite2D != null)
		{
			return sprite2D;
		}
		return null;
	}
}