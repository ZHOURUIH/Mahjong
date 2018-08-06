using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// 用于生成二进制文件的
public class Serializer
{
	protected int mIndex;
	protected int mBufferSize;
	protected bool mWriteFlag;	// 如果为真,则表示是只写的,为假则表示是只读的
	protected byte[] mBuffer;
	public Serializer()
	{
		mWriteFlag = true;
		mIndex = 0;
		mBufferSize = 0;
		mBuffer = null;
	}
	public Serializer(byte[] buffer)
	{
		mWriteFlag = false;
		mIndex = 0;
		mBuffer = buffer;
		mBufferSize = mBuffer.Length;
	}
	public void write(byte value)
	{
		int writeLen = sizeof(byte);
		if (!writeCheck(writeLen))
		{
			return;
		}
		BinaryUtility.memcpy(mBuffer, BinaryUtility.toBytes(value), mIndex, 0, writeLen);
		mIndex += writeLen;
	}
	public void write(short value)
	{
		int writeLen = sizeof(short);
		if (!writeCheck(writeLen))
		{
			return;
		}
		BinaryUtility.memcpy(mBuffer, BinaryUtility.toBytes(value), mIndex, 0, writeLen);
		mIndex += writeLen;
	}
	public void write(int value)
	{
		int writeLen = sizeof(int);
		if (!writeCheck(writeLen))
		{
			return;
		}
		BinaryUtility.memcpy(mBuffer, BinaryUtility.toBytes(value), mIndex, 0, writeLen);
		mIndex += writeLen;
	}
	public void write(float value)
	{
		int writeLen = sizeof(float);
		if (!writeCheck(writeLen))
		{
			return;
		}
		BinaryUtility.memcpy(mBuffer, BinaryUtility.toBytes(value), mIndex, 0, writeLen);
		mIndex += writeLen;
	}
	public void read(ref byte value)
	{
		int readLen = sizeof(byte);
		if (!readCheck(readLen))
		{
			return;
		}
		byte[] dest = BinaryUtility.toBytes(value);
		BinaryUtility.memcpy(dest, mBuffer, 0, mIndex, readLen);
		value = BinaryUtility.bytesToByte(dest);
		mIndex += readLen;
	}
	public void read(ref short value)
	{
		int readLen = sizeof(short);
		if (!readCheck(readLen))
		{
			return;
		}
		byte[] dest = BinaryUtility.toBytes(value);
		BinaryUtility.memcpy(dest, mBuffer, 0, mIndex, readLen);
		value = BinaryUtility.bytesToShort(dest);
		mIndex += readLen;
	}
	public void read(ref int value)
	{
		int readLen = sizeof(int);
		if (!readCheck(readLen))
		{
			return;
		}
		byte[] dest = BinaryUtility.toBytes(value);
		BinaryUtility.memcpy(dest, mBuffer, 0, mIndex, readLen);
		value = BinaryUtility.bytesToInt(dest);
		mIndex += readLen;
	}
	public void read(ref float value)
	{
		int readLen = sizeof(float);
		if (!readCheck(readLen))
		{
			return;
		}
		byte[] dest = BinaryUtility.toBytes(value);
		BinaryUtility.memcpy(dest, mBuffer, 0, mIndex, readLen);
		value = BinaryUtility.bytesToFloat(dest);
		mIndex += readLen;
	}
	public void writeBuffer(byte[] buffer, int bufferSize)
	{
		if (!writeCheck(bufferSize))
		{
			return;
		}
		BinaryUtility.memcpy(mBuffer, buffer, mIndex, 0, bufferSize);
		mIndex += bufferSize;
	}
	public void readBuffer(byte[] buffer, int bufferSize, int readLen)
	{
		if (!readCheck(readLen))
		{
			return;
		}
		// 如果存放数据的空间大小不足以放入当前要读取的数据,则只拷贝能容纳的长度,但是下标应该正常跳转
		if (bufferSize <= readLen)
		{
			BinaryUtility.memcpy(buffer, mBuffer, 0, mIndex, bufferSize);
			mIndex += readLen;
		}
		else
		{
			BinaryUtility.memcpy(buffer, mBuffer, 0, mIndex, readLen);
			mIndex += readLen;
		}
	}
	public void writeString(string str)
	{
		int strLen = str.Length;
		if (!writeCheck(strLen + sizeof(int)))
		{
			return;
		}
		// 先写入字符串长度
		write(strLen);
		BinaryUtility.memcpy(mBuffer, BinaryUtility.stringToBytes(str), mIndex, 0, strLen);
		mIndex += strLen;
	}
	public void readString(byte[] str, int strBufferSize)
	{
		if (!readCheck(sizeof(int)))
		{
			return;
		}
		// 先读入字符串长度
		int readLen = 0;
		read(ref readLen);
		if (!readCheck(readLen))
		{
			return;
		}
		// 如果存放字符串的空间大小不足以放入当前要读取的字符串,则只拷贝能容纳的长度,但是下标应该正常跳转
		if (strBufferSize <= readLen)
		{
			BinaryUtility.memcpy(str, mBuffer, 0, mIndex, strBufferSize - 1);
			mIndex += readLen;
			// 加上结束符
			str[strBufferSize - 1] = 0;
		}
		else
		{
			BinaryUtility.memcpy(str, mBuffer, 0, mIndex, readLen);
			mIndex += readLen;
			// 加上结束符
			str[readLen] = 0;
		}
	}
	public byte[] getBuffer() { return mBuffer; }
	public int getBufferSize() { return mBufferSize; }
	public int getDataSize() { return mIndex; }
	//-------------------------------------------------------------------------------------------------------------------------------------------
	protected bool writeCheck(int writeLen)
	{
		// 如果是只读的,则不能写入
		if (!mWriteFlag)
		{
			UnityUtility.logError("error : the buffer is read only, can not write!");
			return false;
		}
		// 如果缓冲区为空,则创建缓冲区
		if (mBuffer == null)
		{
			createBuffer(writeLen);
		}
		// 如果缓冲区已经不够用了,则重新扩展缓冲区
		else if (writeLen + mIndex > mBufferSize)
		{
			resizeBuffer(writeLen + mIndex);
		}
		return true;
	}
	protected bool readCheck(int readLen)
	{
		// 如果是只写的,则不能读取
		if (mWriteFlag)
		{
			UnityUtility.logError("error : the buffer is write only, can not read!");
			return false;
		}
		if (mBuffer == null)
		{
			UnityUtility.logError("error : buffer is NULL! can not read");
			return false;
		}
		if (mIndex + readLen > mBufferSize)
		{
			UnityUtility.logError("error : read buffer out of range! cur index : " + mIndex + ", buffer size : " + mBufferSize + ", read length : " + readLen);
			return false;
		}
		return true;
	}
	protected void resizeBuffer(int maxSize)
	{
		int newSize = maxSize > mBufferSize * 2 ? maxSize : mBufferSize * 2;
		byte[] newBuffer = new byte[newSize];
		for (int i = 0; i < mBufferSize; ++i)
		{
			newBuffer[i] = mBuffer[i];
		}
		mBuffer = newBuffer;
		mBufferSize = newSize;
	}
	protected void createBuffer(int bufferSize)
	{
		if (mBuffer == null)
		{
			mBufferSize = bufferSize;
			mBuffer = new byte[mBufferSize];
		}
	}
};