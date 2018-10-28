using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class txNGUITexture : txUIObject
{
	protected UITexture mTexture;
	protected WindowShader mWindowShader;
    protected string mOriginTextureName;    // 初始图片的名字,用于外部根据初始名字设置其他效果的图片
	public txNGUITexture()
	{
		mType = UI_TYPE.UT_NGUI_TEXTURE;
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
			bool newMaterial = mShaderManager.isSingleShader(materialName);
			setMaterial(getMaterialName(), !newMaterial);
		}
        mOriginTextureName = getTextureName();
    }
	public virtual void setWindowShader<T>() where T : WindowShader, new()
	{
		mWindowShader = new T();
	}
	public virtual T getWindowShader<T>() where T : WindowShader
	{
		return mWindowShader as T;
	}
	public override void destroy()
	{
		// 卸载创建出的材质
		UnityUtility.destroyGameObject(mTexture.material);
		base.destroy();
	}
	public virtual void setTexture(Texture tex, bool useTextureSize = false)
	{
		if (mTexture == null)
		{
			return;
		}
		mTexture.mainTexture = tex;
		if (useTextureSize && tex != null)
		{
			setWindowSize(getTextureSize());
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
	public void setTextureName(string name, bool useTextureSize = false)
	{
		if (name != "")
		{
			Texture tex = mResourceManager.loadResource<Texture>(name, true);
			setTexture(tex, useTextureSize);
		}
		else
		{
			setTexture(null, useTextureSize);
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
	public Vector2 getTextureSize()
	{
		if (mTexture.mainTexture == null)
		{
			return Vector2.zero;
		}
		return new Vector2(mTexture.mainTexture.width, mTexture.mainTexture.height);
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
	public Vector2 getWindowSize(bool transformed = false)
	{
		if (mTexture == null)
		{
			return Vector2.zero;
		}
		Vector2 textureSize = new Vector2(mTexture.width, mTexture.height);
		if(transformed)
		{
			Vector2 scale = getWorldScale();
			textureSize.x *= scale.x;
			textureSize.y *= scale.y;
		}
		return textureSize;
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
	public void setColor(Color color)
	{
		mTexture.color = color;
	}
	public UITexture getUITexture()
	{
		return mTexture;
	}
	public string getOriginTextureName() { return mOriginTextureName; }
    public void setOriginTextureName(string textureName) { mOriginTextureName = textureName; }
	// 自动计算图片的原始名称,也就是不带后缀的名称,后缀默认以_分隔
	public void generateOriginTextureName(string key = "_")
	{
		int pos = mOriginTextureName.LastIndexOf(key);
		if (pos >= 0)
		{
			mOriginTextureName = mOriginTextureName.Substring(0, mOriginTextureName.LastIndexOf(key) + 1);
		}
		else
		{
			logError("texture name is not valid!can not generate origin texture name, texture name : " + mOriginTextureName);
		}
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