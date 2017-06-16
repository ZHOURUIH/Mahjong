using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// 基础数据类型再包装基类
public abstract class OBJECT
{
	public Type mType;
	public int mSize;
	public abstract void zero();
	public abstract void readFromBuffer(byte[] buffer, ref int index);
	public abstract void writeToBuffer(byte[] buffer, ref int index);
}