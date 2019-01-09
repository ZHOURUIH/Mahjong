using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WidgetUtility : GameBase
{
	protected static Vector2 mRootSize = Vector2.zero;
	public static Vector2 getRootSize()
	{
		if(isVectorZero(mRootSize))
		{
			// 实际运行时根节点大小需要实际获取
			UIRoot mRoot = UnityUtility.getGameObject(null, "NGUIRoot").GetComponent<UIRoot>();
			Camera camera = UnityUtility.getGameObject(mRoot.gameObject, "UICamera").GetComponent<Camera>();
			mRootSize = new Vector2(mRoot.activeHeight * camera.aspect, mRoot.activeHeight);
		}
		return mRootSize;
	}
	// 父节点在父节点坐标系下的各条边
	public static Vector3[] getParentSides(GameObject parent)
	{
		UIRect parentRect = getGameObjectRect(parent);
		if (parentRect == null)
		{
			return new Vector3[4] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
		}
		return getRectLocalSide(parentRect);
	}
	public static Vector2 getRectSize(UIRect rect)
	{
		Vector3[] sides = getRectLocalSide(rect);
		float width = getLength(sides[0] - sides[2]);
		float height = getLength(sides[1] - sides[3]);
		return new Vector2(width, height);
	}
	public static Vector3[] getRectLocalSide(UIRect rect)
	{
		Vector3[] sides = new Vector3[4] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
		Vector3[] localCorners = null;
		if (rect.transform.parent != null)
		{
			localCorners = rect.localCorners;
		}
		// rect是UIRoot节点的
		else
		{
			Vector2 rootSize = getRootSize();
			localCorners = new Vector3[4]
			{
				new Vector3(-rootSize.x / 2.0f, -rootSize.y / 2.0f),
				new Vector3(-rootSize.x / 2.0f, rootSize.y / 2.0f),
				new Vector3(rootSize.x / 2.0f, rootSize.y / 2.0f),
				new Vector3(rootSize.x / 2.0f, -rootSize.y / 2.0f),
			};
		}
		for (int i = 0; i < 4; ++i)
		{
			sides[i] = (localCorners[i] + localCorners[(i + 1) % 4]) / 2;
		}
		return sides;
	}
	public static UIRect findParentRect(GameObject obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj.transform.parent == null)
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
				return findParentRect(parent);
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