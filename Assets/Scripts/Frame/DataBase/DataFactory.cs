using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DataFactory
{
	protected Type mClassType;
	protected DATA_TYPE mType;
	public DataFactory(DATA_TYPE type, Type classType)
	{
		mType = type;
		mClassType = classType; 
	}
	public virtual Data createData()
	{
		object[] param = new object[] { mType };  //构造器参数
		Data data = UnityUtility.createInstance<Data>(mClassType, param);
		return data;
	}
	static public DataFactory createFactory(Type classType, DATA_TYPE type)
	{
		object[] param = new object[] { type, classType };  //构造器参数
		DataFactory factory = UnityUtility.createInstance<DataFactory>(typeof(DataFactory), param);
		return factory;
	}
	public DATA_TYPE getType() { return mType; }
};