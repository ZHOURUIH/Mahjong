using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class txNGUIStaticTexture : txUIObject
{
	protected UITexture mTexture;
	protected WindowShader mWindowShader;
	protected Vector3 mOriginalPosition;
	public txNGUIStaticTexture()
	{
		mType = UI_TYPE.UT_NGUI_STATIC_TEXTURE;
	}
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		mTexture = mObject.GetComponent<UITexture>();
		if (mTexture == null)
		{
			mTexture = mObject.AddComponent<UITexture>();
		}
		mTexture.onRender = onWidgetRender;
		string materialName = getMaterialName();
		if(materialName != "")
		{
			setMaterial(getMaterialName(), true);
		}
		mOriginalPosition = mTransform.localPosition;
	}
	public void setWindowShader<T>() where T : WindowShader, new()
	{
		mWindowShader = new T();
	}
	public T getWindowShader<T>() where T : WindowShader
	{
		return mWindowShader as T;
	}
	public override void destroy()
	{
		// 卸载创建出的材质
		UnityUtility.destroyGameObject(mTexture.material);
		base.destroy();
	}
	public void setTexture(Texture tex)
	{
		setTexture(tex, Vector2.zero, Vector2.zero);
	}
	public void setTexture(Texture tex, Vector2 windowSize, Vector2 windowPosOffset)
	{
		if (mTexture == null)
		{
			return;
		}
		mTexture.mainTexture = tex;
		if (!MathUtility.isVectorZero(windowSize))
		{
			setWindowSize(windowSize);
		}
		if (!MathUtility.isVectorZero(windowPosOffset))
		{
			mTransform.localPosition = mOriginalPosition + new Vector3(windowPosOffset.x, windowPosOffset.y, 0.0f);
		}
	}
	public Texture getTexture()
	{
		if (mTexture == null)
		{
			return null;
		}
		return mTexture.mainTexture;
	}
	public void setMaterial(string materialName, bool newMaterial)
	{
		if (mTexture == null)
		{
			return;
		}
		Material mat = null;
		if (newMaterial)
		{
			mat = new Material(mResourceManager.loadResource<Material>(CommonDefine.R_MATERIAL_PATH + materialName, true));
			mat.name = materialName + mID;
		}
		else
		{
			mat = mResourceManager.loadResource<Material>(CommonDefine.R_MATERIAL_PATH + materialName, true);
		}
		mTexture.material = mat;
	}
	public void setShader(Shader shader, bool force)
	{
		if (mTexture == null)
		{
			return;
		}
		if(force)
		{
			mTexture.shader = null;
			mTexture.shader = shader;
		}
	}
	public void setTextureName(string name)
	{
		if (name != "")
		{
			Texture tex = mResourceManager.loadResource<Texture>(name, true);
			setTexture(tex);
		}
		else
		{
			setTexture(null);
		}
	}
	public string getTextureName()
	{
		if(mTexture == null || mTexture.mainTexture == null)
		{
			return "";
		}
		return mTexture.mainTexture.name;
	}
	public string getMaterialName()
	{
		if (mTexture == null || mTexture.material == null)
		{
			return "";
		}
		return mTexture.material.name;
	}
	public string getShaderName()
	{
		if (mTexture == null || mTexture.material == null || mTexture.material.shader == null)
		{
			return "";
		}
		return mTexture.material.shader.name;
	}
	public override void setAlpha(float alpha)
	{
		if (mTexture == null)
		{
			return;
		}
		mTexture.alpha = alpha;
	}
	public override float getAlpha()
	{
		if (mTexture == null)
		{
			return 0.0f;
		}
		return mTexture.alpha;
	}
	public override void setFillPercent(float percent)
	{
		if (mTexture == null)
		{
			return;
		}
		mTexture.fillAmount = percent;
	}
	public override float getFillPercent()
	{
		if (mTexture == null)
		{
			return 0.0f;
		}
		return mTexture.fillAmount;
	}
	public Vector2 getWindowSize()
	{
		if (mTexture == null)
		{
			return Vector2.zero;
		}
		return new Vector2(mTexture.width, mTexture.height);
	}
	public void setWindowSize(Vector2 size)
	{
		mTexture.width = (int)size.x;
		mTexture.height = (int)size.y;
	}
	public override int getDepth()
	{
		if(mTexture == null)
		{
			return 0;
		}
		return mTexture.depth;
	}
	public override void setDepth(int depth)
	{
		if(mTexture == null)
		{
			return;
		}
		mTexture.depth = depth;
		base.setDepth(depth);
	}
	public override Vector3 getPosition(){return mOriginalPosition;}
	public override Vector3 getWorldPosition() { return mTransform.parent.localToWorldMatrix.MultiplyPoint(mOriginalPosition); }
	public override void setLocalPosition(Vector3 pos)
	{
		base.setLocalPosition(pos);
		mOriginalPosition = pos;
	}
	public override void setWorldPosition(Vector3 pos)
	{
		base.setWorldPosition(pos);
		mOriginalPosition = mTransform.parent.worldToLocalMatrix.MultiplyPoint(mTransform.localPosition);
	}
	//---------------------------------------------------------------------------------------------------
	protected void onWidgetRender(Material mat)
	{
		if(mWindowShader != null)
		{
			mWindowShader.applyShader(mat);
		}
	}
}