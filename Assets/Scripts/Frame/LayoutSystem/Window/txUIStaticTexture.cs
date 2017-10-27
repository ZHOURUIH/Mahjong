using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class txUIStaticTexture : txUIObject
{
	protected UITexture mTexture;
	protected string    mNormalShaderName;
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
		string materialName = getMaterialName();
		if(materialName != "")
		{
			setMaterial(getMaterialName(), true);
		}
	}
	public override void destroy()
	{
		// 卸载创建出的材质
		GameObject.Destroy(mTexture.material);
		base.destroy();
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
	public int getDepth()
	{
		if(mTexture == null)
		{
			return 0;
		}
		return mTexture.depth;
	}
	public void setDepth(int depth)
	{
		if(mTexture == null)
		{
			return;
		}
		mTexture.depth = depth;
	}
	//---------------------------------------------------------------------------------------------------
	protected void onWidgetRender(Material mat)
	{
		applyShader(mat);
	}
	protected virtual void applyShader(Material mat)
	{
		;
	}
}