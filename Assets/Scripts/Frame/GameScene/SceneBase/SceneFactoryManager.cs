using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SceneFactoryManager
{
	protected Dictionary<GAME_SCENE_TYPE, SceneFactory> mFactoryList = new Dictionary<GAME_SCENE_TYPE, SceneFactory>();
	public SceneFactory addFactory(Type classType, GAME_SCENE_TYPE type)
	{
		SceneFactory factory = SceneFactory.createFactory(type, classType);
		mFactoryList.Add(factory.getType(), factory);
		return factory;
	}
	public SceneFactory getFactory(GAME_SCENE_TYPE type)
	{
		if (mFactoryList.ContainsKey(type))
		{
			return mFactoryList[type];
		}
		return null;
	}
}