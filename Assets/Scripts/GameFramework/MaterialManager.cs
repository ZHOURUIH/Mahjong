using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaterialManager : GameBase
{
	protected Dictionary<string, Material> mMaterialList;   // 代码动态创建的材质列表
	public MaterialManager()
	{
		mMaterialList = new Dictionary<string, Material>();
	}
	public virtual void init()
	{
		// 加载所有材质
	}
	public void destroy()
	{
		mMaterialList.Clear();
	}
	public virtual void update(float elapsedTime)
	{}
	// path为Material下相对路径,不以/结尾,name为材质名,不带后缀
	public Material copyMaterial(string name, string newName)
	{
		if (mMaterialList.ContainsKey(newName))
		{
			return null;
		}
		// 材质已加载,则获得已有材质,然后复制一个材质
		Material sourceMaterial = tryGetMaterial(name);
		if(sourceMaterial == null)
		{
			return null;
		}
		Material newMat = new Material(sourceMaterial);
		newMat.name = newName;
		mMaterialList.Add(newName, newMat);
		return newMat;
	}
	// 尝试从资源管理器中获得材质
	public Material tryGetMaterial(string name)
	{
		// 如果材质还未加载,则直接返回空
		if (!mResourceManager.isResourceLoaded<Material>(name))
		{
			return null;
		}
		return mResourceManager.getResource<Material>(name, true);
	}
}