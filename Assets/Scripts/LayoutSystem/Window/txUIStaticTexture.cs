using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class txUIStaticTexture : txUIObject
{
	public UITexture mTexture;
	public Vector3 mHSLOffset;	// 当前HSL偏移,只有当shader为HSLOffet或者HSLOffsetLinearDodge时才有效
	public string mLastShaderName;
	public txUIStaticTexture()
	{
		mType = UI_OBJECT_TYPE.UBT_STATIC_TEXTURE;
	}
	public override void init(GameLayout layout, GameObject go)
	{
		base.init(layout, go);
		mTexture = mObject.GetComponent<UITexture>();
		if (mTexture == null)
		{
			mTexture = mObject.AddComponent<UITexture>();
		}
		mTexture.onRender = onWidgetRender;
	}
	public void setTexture(Texture tex)
	{
		if(mTexture == null)
		{
			return;
		}
		mTexture.mainTexture = tex;
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
			mat = mMaterialManager.copyMaterial(materialName, materialName + mID);
		}
		else
		{
			mat = mMaterialManager.tryGetMaterial(materialName);
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
	public void setHSLOffset(Vector3 offset)
	{
		mHSLOffset = offset;
	}
	public Vector3 getHSLOff() 
	{
		return mHSLOffset;
	}
	public void setFillPercent(float percent)
	{
		if (mTexture == null)
		{
			return;
		}
		mTexture.fillAmount = percent;
	}
	public void setGray(bool gray)
	{
		if (mTexture == null)
		{
			return;
		}
		// 设置为灰色shader时,需要记录当前shader名,以便取消灰色时恢复之前的shader
		if(gray)
		{
			mLastShaderName = mTexture.shader.name;
			mTexture.shader = mShaderManager.getShader("Gray");
		}
		else
		{
			mTexture.shader = mShaderManager.getShader(mLastShaderName);
			mLastShaderName = "";
		}
	}
	public float getFillPercent()
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
	//---------------------------------------------------------------------------------------------------
	protected void onWidgetRender(Material mat)
	{
		if (mat != null && mat.shader != null)
		{
			if (mat.shader.name == "HSLOffset" || mat.shader.name == "HSLOffsetLinearDodge")
			{
				mat.SetColor("_HSLOffset", new Color(mHSLOffset.x, mHSLOffset.y, mHSLOffset.z));
			}
		}
	}
}