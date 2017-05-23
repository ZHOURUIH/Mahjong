using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CharacterFactoryManager
{
	protected Dictionary<CHARACTER_TYPE, CharacterFactory> mFactoryList = null;
	public CharacterFactoryManager()
	{
		mFactoryList = new Dictionary<CHARACTER_TYPE, CharacterFactory>();
	}
	public CharacterFactory addFactory<T>(CHARACTER_TYPE type) where T : Character
	{
		CharacterFactory factory = CharacterFactory.createFactory(typeof(T), type);
		mFactoryList.Add(factory.getType(), factory);
		return factory;
	}
	public CharacterFactory getFactory(CHARACTER_TYPE type)
	{
		if (mFactoryList.ContainsKey(type))
		{
			return mFactoryList[type];
		}
		return null;
	}
	public int getFactoryCount()
	{
		return mFactoryList.Count;
	}
}