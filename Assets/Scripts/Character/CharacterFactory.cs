using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CharacterFactory
{
	protected Type mClassType;
	protected CHARACTER_TYPE mType;
	public CharacterFactory(CHARACTER_TYPE type, Type classType)
	{
		mType = type;
		mClassType = classType; 
	}
	public virtual Character createCharacter(string name)
	{
		object[] param = new object[] { mType, name };  //构造器参数
		Character character = Activator.CreateInstance(mClassType, param) as Character;
		return character;
	}
	static public CharacterFactory createFactory(Type classType, CHARACTER_TYPE type)
	{
		object[] param = new object[] { type, classType };  //构造器参数
		CharacterFactory factory = Activator.CreateInstance(typeof(CharacterFactory), param) as CharacterFactory;
		return factory;
	}
	public CHARACTER_TYPE getType() { return mType; }
};