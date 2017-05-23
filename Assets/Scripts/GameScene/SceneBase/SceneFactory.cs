using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SceneFactory
{
	protected Type mClassType;
	protected GAME_SCENE_TYPE mType;
	public SceneFactory(GAME_SCENE_TYPE type, Type classType)
	{
		mType = type;
		mClassType = classType; 
	}
	public virtual GameScene createScene(string name)
	{
		object[] param = new object[] { mType, name };  //构造器参数
		GameScene script = (GameScene)Activator.CreateInstance(mClassType, param);
		return script;
	}
	static public SceneFactory createFactory(GAME_SCENE_TYPE type, Type classType)
	{
		object[] param = new object[] { type, classType };  //构造器参数
		SceneFactory factory = Activator.CreateInstance(typeof(SceneFactory), param) as SceneFactory;
		return factory;
	}
	public GAME_SCENE_TYPE getType() { return mType; }
	public Type getClassType() { return mClassType; }
};