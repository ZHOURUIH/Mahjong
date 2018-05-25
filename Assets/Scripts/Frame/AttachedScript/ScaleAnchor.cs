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

public class ScaleAnchor : MonoBehaviour
{
	// 只能更新一次
	public bool mDirty = true;
	public float mHorizontalScale = 1.0f;
	public float mVerticalScale = 1.0f;
	protected int mOriginWidth;
	protected int mOriginHeight;
	public Vector3 mOriginPos;
	public UIRoot mRoot;
	public bool mKeepAspect;    // 是否保持宽高比
	public ASPECT_BASE mAspectBase = ASPECT_BASE.AB_USE_HEIGHT_SCALE;
	public float mWidthScale;
	public float mHeightScale;
	public float mPosXScale;
	public float mPosYScale;
	public void Start()
	{
		if(GameBase.mLayoutManager != null)
		{
			mRoot = GameBase.mLayoutManager.getNGUIRootObject().GetComponent<UIRoot>();
			UIWidget widget = GetComponent<UIWidget>();
			if(widget != null)
			{
				widget.keepAspectRatio = UIWidget.AspectRatioSource.Free;
			}
		}
	}
	public void OnEnable()
	{
		
	}
	public void Update()
	{
		if (mDirty && mRoot != null)
		{
			mHorizontalScale = mRoot.manualWidth / (float)CommonDefine.STANDARD_WIDTH;
			mVerticalScale = mRoot.manualHeight / (float)CommonDefine.STANDARD_HEIGHT;
			UIWidget thisWidget = getGameObjectWidget(gameObject);
			if (thisWidget != null)
			{
				mOriginWidth = thisWidget.width;
				mOriginHeight = thisWidget.height;
			}
			mOriginPos = transform.localPosition;
			updateRect();
			mDirty = false;
		}
	}
	//------------------------------------------------------------------------------------------------------------------------------------------------
	protected void updateRect()
	{
		UIWidget thisWidget = getGameObjectWidget(gameObject);
		if(thisWidget != null)
		{
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
			thisWidget.width = (int)(mOriginWidth * mWidthScale + 0.5f);
			thisWidget.height = (int)(mOriginHeight * mHeightScale + 0.5f);
		}
		// 如果父节点保持了宽高比,则不需要缩放
		ScaleAnchor parentAnchor = transform.parent.gameObject.GetComponent<ScaleAnchor>();
		if (parentAnchor != null && parentAnchor.mKeepAspect)
		{
			mPosXScale = mWidthScale;
			mPosYScale = mHeightScale;
		}
		else
		{
			mPosXScale = mHorizontalScale;
			mPosYScale = mVerticalScale;
		}
		Vector3 pos = mOriginPos;
		pos.x *= mPosXScale;
		pos.y *= mPosYScale;
		gameObject.transform.localPosition = pos;
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
}