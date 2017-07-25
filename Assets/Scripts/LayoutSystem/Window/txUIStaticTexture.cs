using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class txUIStaticTexture : txUIObject
{
	protected UITexture mTexture;
	protected Vector3   mHSLOffset;	// 当前HSL偏移,只有当shader为HSLOffet或者HSLOffsetLinearDodge时才有效
	protected string    mNormalShaderName;
	protected Texture	mMask;
	protected Vector2	mMaskSize;
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
		mNormalShaderName = mTexture.shader.name;
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
			mat = new Material(mResourceManager.loadResource<Material>(CommonDefine.R_MATERIAL_PATH + materialName, true));
			mat.name = materialName + mID;
		}
		else
		{
			mat = mResourceManager.loadResource<Material>(CommonDefine.R_MATERIAL_PATH + materialName, true);
		}
		mTexture.material = mat;
		mNormalShaderName = mTexture.material.shader.name;
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
			mNormalShaderName = shader.name;
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
	public void setMaskTexture(Texture mask){mMask = mask;}
	public void setMaskSize(Vector2 size){mMaskSize = size;}
	public void setHSLOffset(Vector3 offset){mHSLOffset = offset;}
	public Vector3 getHSLOff() {return mHSLOffset;}
	public void setGray(bool gray)
	{
		if (mTexture == null)
		{
			return;
		}
		// 设置为灰色shader时,需要记录当前shader名,以便取消灰色时恢复之前的shader
		if (gray)
		{
			mTexture.shader = mShaderManager.getShader("Gray");
		}
		else
		{
			mTexture.shader = mShaderManager.getShader(mNormalShaderName);
		}
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
	//---------------------------------------------------------------------------------------------------
	protected void onWidgetRender(Material mat)
	{
		if (mat != null && mat.shader != null)
		{
			string shaderName = mat.shader.name;
			if (shaderName == "HSLOffset" || shaderName == "HSLOffsetLinearDodge")
			{
				mat.SetColor("_HSLOffset", new Color(mHSLOffset.x, mHSLOffset.y, mHSLOffset.z));
			}
			else if(shaderName == "MaskCut")
			{
				mat.SetTexture("_MaskTex", mMask);
				mat.SetFloat("_SizeX", mMaskSize.x);
				mat.SetFloat("_SizeY", mMaskSize.y);
			}
		}
	}
}