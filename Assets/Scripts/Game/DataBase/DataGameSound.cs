using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DataGameSound : Data
{
	public int mSoundID;
	public byte[] mSoundOwner = new byte[32];
	public int mSoundType;
	public byte[] mSoundFileName = new byte[64];
	public byte[] mDescribe = new byte[64];
	public float mVolumeScale;
	public DataGameSound(DATA_TYPE type)
		:
		base(type)
	{ }
	public override void read(byte[] data, int dataCount)
	{
		Serializer seri = new Serializer(data);
		seri.read(ref mSoundID);
		seri.readBuffer(mSoundOwner, 32, 32);
		seri.read(ref mSoundType);
		seri.readBuffer(mSoundFileName, 64, 64);
		seri.readBuffer(mDescribe, 64, 64);
		seri.read(ref mVolumeScale);
	}
	public override int getDataSize()
	{
		return sizeof(int) + 32 + sizeof(int) + 64 + 64 + sizeof(float);
	}
};