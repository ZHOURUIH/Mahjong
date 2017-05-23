using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ScriptFactory
{
	protected Type mClassType;
	protected LAYOUT_TYPE mType;
	public ScriptFactory(LAYOUT_TYPE type, Type classType)
	{
		mType = type;
		mClassType = classType; 
	}
	public virtual LayoutScript createScript(GameLayout layout, string name)
	{
		object[] param = new object[] { mType, name, layout };
		return UnityUtility.createInstance<LayoutScript>(mClassType, param);
	}
	public LAYOUT_TYPE getType() { return mType; }
};