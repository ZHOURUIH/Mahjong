using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum ASPECT_BASE
{
	AB_USE_WIDTH_SCALE,		// 使用宽的缩放值来缩放控件
	AB_USE_HEIGHT_SCALE,    // 使用高的缩放值来缩放控件
}

public enum PADDING_STYLE
{
	PS_NONE,
	PS_CENTER,
	PS_LEFT,
	PS_RIGHT,
	PS_TOP,
	PS_BOTTOM,
	PS_LEFT_BOTTOM,
	PS_LEFT_TOP,
	PS_RIGHT_BOTTOM,
	PS_RIGHT_TOP,
	PS_CUSTOM_VALUE,
}
public class ScaleAnchor : MonoBehaviour
{
	protected bool mDirty = true;
	protected UIRoot mRoot;
	public float mHorizontalScale = 1.0f;
	public float mVerticalScale = 1.0f;
	protected int mOriginWidth;
	protected int mOriginHeight;
	public Vector3 mOriginPos;
	public bool mKeepAspect;    // 是否保持宽高比
	public ASPECT_BASE mAspectBase = ASPECT_BASE.AB_USE_HEIGHT_SCALE;
	public float mWidthScale;
	public float mHeightScale;
	public float mPosXScale;
	public float mPosYScale;
	// 窗口的停靠方式,只改变位置,如果停靠方式不为PS_NONE,则会重新计算窗口位置
	public PADDING_STYLE mPadding = PADDING_STYLE.PS_NONE;
	public float mHorizontalRelativePos = 0.0f;
	public float mVerticalRelativePos = 0.0f;
	public void Awake()
	{
		if (GameBase.mLayoutManager != null && mRoot == null)
		{
			mRoot = GameBase.mLayoutManager.getNGUIRootObject().GetComponent<UIRoot>();
			UIWidget widget = CustomAnchor.getGameObjectWidget(gameObject);
			if (widget != null)
			{
				widget.keepAspectRatio = UIWidget.AspectRatioSource.Free;
			}
			mHorizontalScale = mRoot.activeHeight * GameBase.mCameraManager.getUICamera().getCamera().aspect / (float)CommonDefine.STANDARD_WIDTH;
			mVerticalScale = mRoot.activeHeight / (float)CommonDefine.STANDARD_HEIGHT;
		}
		// 由于初始位置是在Awake中记录的,所以在动态实例化预设后挂接到父节点下时,坐标必须为0,也就是与预设初始状态保持一致
		mOriginPos = transform.localPosition;
		UIWidget thisWidget = CustomAnchor.getGameObjectWidget(gameObject);
		if (thisWidget != null)
		{
			mOriginWidth = thisWidget.width;
			mOriginHeight = thisWidget.height;
		}
	}
	public void forceUpdateChildren()
	{
		// 先更新自己
		updateRect(true);
		// 然后更新所有子节点
		int childCount = transform.childCount;
		for (int i = 0; i < childCount; ++i)
		{
			ScaleAnchor anchor = transform.GetChild(i).GetComponent<ScaleAnchor>();
			if (anchor != null)
			{
				anchor.forceUpdateChildren();
			}
		}
	}
	//------------------------------------------------------------------------------------------------------------------------------------------------
	protected void updateRect(bool force = false)
	{
		if (mRoot == null || !force && !mDirty)
		{
			return;
		}
		mDirty = false;
		mWidthScale = mHorizontalScale;
		mHeightScale = mVerticalScale;
		if (mKeepAspect)
		{
			if (mAspectBase == ASPECT_BASE.AB_USE_HEIGHT_SCALE)
			{
				mWidthScale = mVerticalScale;
			}
			else if (mAspectBase == ASPECT_BASE.AB_USE_WIDTH_SCALE)
			{
				mHeightScale = mHorizontalScale;
			}
		}
		float thisWidth = 0.0f;
		float thisHeight = 0.0f;
		UIWidget thisWidget = CustomAnchor.getGameObjectWidget(gameObject);
		if (thisWidget != null)
		{
			thisWidth = mOriginWidth * mWidthScale;
			thisHeight = mOriginHeight * mHeightScale;
			thisWidget.width = (int)(thisWidth + 0.5f);
			thisWidget.height = (int)(thisHeight + 0.5f);
		}
		if (mPadding == PADDING_STYLE.PS_NONE)
		{
			mPosXScale = mWidthScale;
			mPosYScale = mHeightScale;
			gameObject.transform.localPosition = new Vector3(mOriginPos.x * mPosXScale, mOriginPos.y * mPosYScale, mOriginPos.z);
		}
		else
		{
			// 只有在刷新时才能确定父节点,所以父节点需要实时获取
			UIRect parentRect = CustomAnchor.findParentRect(gameObject);
			Vector2 parentSize = CustomAnchor.getRectSize(parentRect);
			// hori为-1表示窗口坐标在父窗口的左侧边界上,为1表示在右侧边界上
			if (mPadding == PADDING_STYLE.PS_LEFT || mPadding == PADDING_STYLE.PS_LEFT_BOTTOM || mPadding == PADDING_STYLE.PS_LEFT_TOP)
			{
				mHorizontalRelativePos = -(parentSize.x - thisWidth) / (parentSize.x);
			}
			else if (mPadding == PADDING_STYLE.PS_RIGHT || mPadding == PADDING_STYLE.PS_RIGHT_BOTTOM || mPadding == PADDING_STYLE.PS_RIGHT_TOP)
			{
				mHorizontalRelativePos = (parentSize.x - thisWidth) / (parentSize.x);
			}
			else if (mPadding != PADDING_STYLE.PS_CUSTOM_VALUE)
			{
				mHorizontalRelativePos = mOriginPos.x / (parentSize.x / 2.0f);
			}
			if (mPadding == PADDING_STYLE.PS_TOP || mPadding == PADDING_STYLE.PS_LEFT_TOP || mPadding == PADDING_STYLE.PS_RIGHT_TOP)
			{
				mVerticalRelativePos = (parentSize.y - thisHeight) / (parentSize.y);
			}
			else if (mPadding == PADDING_STYLE.PS_BOTTOM || mPadding == PADDING_STYLE.PS_LEFT_BOTTOM || mPadding == PADDING_STYLE.PS_RIGHT_BOTTOM)
			{
				mVerticalRelativePos = -(parentSize.y - thisHeight) / (parentSize.y);
			}
			else if (mPadding != PADDING_STYLE.PS_CUSTOM_VALUE)
			{
				mVerticalRelativePos = mOriginPos.y / (parentSize.y / 2.0f);
			}
			Vector3 pos = mOriginPos;
			pos.x = mHorizontalRelativePos * (parentSize.x / 2.0f);
			pos.y = mVerticalRelativePos * (parentSize.y / 2.0f);
			gameObject.transform.localPosition = pos;
		}
		// 如果有UIGrid组件,则需要调整UIGrid中的排列间隔
		UIGrid grid = gameObject.GetComponent<UIGrid>();
		if (gameObject.GetComponent<UIGrid>() != null)
		{
			grid.cellHeight *= mHeightScale;
			grid.cellWidth *= mWidthScale;
		}
	}
}