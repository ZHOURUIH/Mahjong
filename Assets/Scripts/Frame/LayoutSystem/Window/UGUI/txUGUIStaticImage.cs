using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// UGUI的静态图片不支持递归变化透明度
public class txUGUIStaticImage : txUIObject
{
	public Image mImage;
	protected RectTransform mRectTransform;
	protected WindowShader mWindowShader;
	public txUGUIStaticImage()
	{
		mType = UI_TYPE.UT_UGUI_STATIC_IMAGE;
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
		// 因为添加Image组件后,原来的Transform会被替换为RectTransform,所以需要重新设置Transform组件
		mTransform = mRectTransform;
		string materialName = getMaterialName();
		// 将默认材质替换为自定义的默认材质
		if (materialName == "Default UI Material")
		{
			setMaterial("UGUIDefault", true);
		}
		else if (materialName != "")
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
	public void setSprite(Sprite tex)
	{
		if (mImage == null)
		{
			return;
		}
		mImage.sprite = tex;
	}
	public void setTexture(Texture2D tex)
	{
		if(mImage == null)
		{
			return;
		}
		mImage.sprite = Sprite.Create(tex, new Rect(Vector2.zero, tex.texelSize), new Vector2(0.5f, 0.5f));
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
			setTexture(mResourceManager.loadResource<Texture2D>(name, true));
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
	public void setWindowSize(Vector2 size)
	{
		mRectTransform.sizeDelta = size;
	}
}