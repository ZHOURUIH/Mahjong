using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class txUGUIStaticImage : txUIObject
{
	protected Image mImage;
	protected RectTransform mRectTransform;
	protected WindowShader mWindowShader;
	public txUGUIStaticImage()
	{
		mType = UI_OBJECT_TYPE.UBT_STATIC_TEXTURE;
	}
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		mImage = mObject.GetComponent<Image>();
		if (mImage == null)
		{
			mImage = mObject.AddComponent<Image>();
		}
		mRectTransform = mObject.GetComponent<RectTransform>();
		string materialName = getMaterialName();
		if (materialName != "")
		{
			setMaterial(getMaterialName(), true);
		}
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
		base.destroy();
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
		if(mImage != null && mImage.material != null && mWindowShader != null)
		{
			mWindowShader.applyShader(mImage.material);
		}
	}
	public void setTexture(Sprite tex)
	{
		if (mImage == null)
		{
			return;
		}
		mImage.sprite = tex;
	}
	public Sprite getTexture()
	{
		if (mImage == null)
		{
			return null;
		}
		return mImage.sprite;
	}
	public void setMaterial(string materialName, bool newMaterial)
	{
		if (mImage == null)
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
		mImage.material = mat;
	}
	public void setShader(Shader shader, bool force)
	{
		if (mImage == null)
		{
			return;
		}
		if (force)
		{
			mImage.material.shader = null;
			mImage.material.shader = shader;
		}
	}
	public void setTextureName(string name)
	{
		if (name != "")
		{
			setTexture(mResourceManager.loadResource<Sprite>(name, true));
		}
		else
		{
			setTexture(null);
		}
	}
	public string getTextureName()
	{
		if (mImage == null || mImage.mainTexture == null)
		{
			return "";
		}
		return mImage.mainTexture.name;
	}
	public string getMaterialName()
	{
		if (mImage == null || mImage.material == null)
		{
			return "";
		}
		return mImage.material.name;
	}
	public string getShaderName()
	{
		if (mImage == null || mImage.material == null || mImage.material.shader == null)
		{
			return "";
		}
		return mImage.material.shader.name;
	}
	public override void setAlpha(float alpha)
	{
		if (mImage == null)
		{
			return;
		}
		Color color = mImage.color;
		color.a = alpha;
		mImage.color = color;
	}
	public override float getAlpha()
	{
		if (mImage == null)
		{
			return 0.0f;
		}
		return mImage.color.a;
	}
	public override void setFillPercent(float percent)
	{
		if (mImage == null)
		{
			return;
		}
		mImage.fillAmount = percent;
	}
	public override float getFillPercent()
	{
		if (mImage == null)
		{
			return 0.0f;
		}
		return mImage.fillAmount;
	}
	public Vector2 getWindowSize()
	{
		return mRectTransform.rect.size;
	}
}