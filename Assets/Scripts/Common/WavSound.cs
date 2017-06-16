using UnityEngine;
using System.Collections;
using System.IO;

public class WavSound
{
	protected string mFileName;
	protected int mRiffMark;			// riff标记
	protected int mFileSize;			// 音频文件大小 - 8,也就是从文件大小字节后到文件结尾的长度
	protected int mWaveMark;			// wave标记
	protected int mFmtMark;				// fmt 标记
	protected int mFmtChunkSize;		// fmt块大小
	protected short mFormatType;			// 编码格式,为1是PCM编码
	protected short mSoundChannels;		// 声道数
	protected int mSamplesPerSec;		// 采样频率
	protected int mAvgBytesPerSec;		// 波形数据传输速率（每秒平均字节数）
	protected short mBlockAlign;			// DATA数据块长度
	protected short mBitsPerSample;		// 单个采样数据大小,如果双声道16位,则是4个字节,也叫PCM位宽
	protected short mOtherSize;			// 附加信息（可选，由上方过滤字节确定）
	protected char[] mDataMark = new char[4];			// data标记
	protected int mDataSize;
	protected byte[] mDataBuffer;
	protected short[] mMixPCMData;
	protected Serializer mWaveDataSerializer;

	public WavSound()
	{
		init();
	}

	public WavSound(string file)
	{
		init();
		readFile(file);
	}

	public void init()
	{
		mFileName = "";
		mRiffMark = 0;
		mFileSize = 0;
		mWaveMark = 0;
		mFmtMark = 0;
		mFmtChunkSize = 0;
		mFormatType = 0;
		mSoundChannels = 0;
		mSamplesPerSec = 0;
		mAvgBytesPerSec = 0;
		mBlockAlign = 0;
		mBitsPerSample = 0;
		mOtherSize = 0;
		BinaryUtility.memset(mDataMark, (char)0, 4);
		mDataSize = 0;
		mDataBuffer = null;
		mMixPCMData = null;
		mWaveDataSerializer = null;
	}
	public byte[] getPCMBuffer(){ return mDataBuffer; }
	public short[] getMixPCMData() { return mMixPCMData; }
	public int getPCMBufferSize() { return mDataSize; }
	public short getSoundChannels() { return mSoundChannels; }
	public int getPCMShortDataCount() { return mDataSize / sizeof(short); }
	public int getMixPCMDataCount() { return mDataSize / sizeof(short) / mSoundChannels; }
	public bool readFile(string file)
	{
		int fileSize = 0;
		byte[] fileData = null;
		FileUtility.openFile(file, ref fileData, ref fileSize);
		mFileName = file;
		Serializer serializer = new Serializer(fileData, fileSize);
		serializer.read(ref mRiffMark);
		serializer.read(ref mFileSize);
		serializer.read(ref mWaveMark);
		serializer.read(ref mFmtMark);
		serializer.read(ref mFmtChunkSize);
		serializer.read(ref mFormatType);
		serializer.read(ref mSoundChannels);
		serializer.read(ref mSamplesPerSec);
		serializer.read(ref mAvgBytesPerSec);
		serializer.read(ref mBlockAlign);
		serializer.read(ref mBitsPerSample);
		if (mFmtChunkSize == 18)
		{
			serializer.read(ref mOtherSize);
		}
		// 如果不是data块,则跳过,重新读取
		do
		{
			mDataBuffer = null;
			serializer.readBuffer(mDataMark, 4, 4);
			serializer.read(ref mDataSize);
			mDataBuffer = new byte[mDataSize];
			serializer.readBuffer(mDataBuffer, mDataSize, mDataSize);
		} while (StringUtility.charArrayToString(mDataMark) != "data");
		refreshFileSize();

		int mixDataCount = getMixPCMDataCount();
		mMixPCMData = new short[mixDataCount];
		generateMixPCMData(mMixPCMData, mixDataCount, mSoundChannels, mDataBuffer, mDataSize);
		return true;
	}
	public static void generateMixPCMData(short[] mixPCMData, int mixDataCount, short channelCount, byte[] dataBuffer, int bufferSize)
	{
		// 如果单声道,则直接将mDataBuffer的数据拷贝到mMixPCMData中
		if (channelCount == 1)
		{
			for (int i = 0; i < mixDataCount; ++i)
			{
				byte[] byteData0 = new byte[2];
				byteData0[0] = (byte)dataBuffer[2 * i + 0];
				byteData0[1] = (byte)dataBuffer[2 * i + 1];
				mixPCMData[i] = BinaryUtility.bytesToShort(byteData0);
			}
		}
		// 如果有两个声道,则将左右两个声道的平均值赋值到mMixPCMData中
		else if (channelCount == 2)
		{
			for (int i = 0; i < mixDataCount; ++i)
			{
				byte[] byteData0 = new byte[2];
				byteData0[0] = (byte)dataBuffer[4 * i + 0];
				byteData0[1] = (byte)dataBuffer[4 * i + 1];
				short shortData0 = BinaryUtility.bytesToShort(byteData0);
				byte[] byteData1 = new byte[2];
				byteData1[0] = (byte)dataBuffer[4 * i + 2];
				byteData1[1] = (byte)dataBuffer[4 * i + 3];
				short shortData1 = BinaryUtility.bytesToShort(byteData1);
				mixPCMData[i] = (short)((shortData0 + shortData1) * 0.5f);
			}
		}
	}
	public static void generateMixPCMData(short[] mixPCMData, int mixDataCount, short channelCount, short[] dataBuffer, int bufferSize)
	{
		// 如果单声道,则直接将mDataBuffer的数据拷贝到mMixPCMData中
		if (channelCount == 1)
		{
			BinaryUtility.memcpy(mixPCMData, dataBuffer, 0, 0, MathUtility.getMin(bufferSize, mixDataCount));
		}
		// 如果有两个声道,则将左右两个声道的平均值赋值到mMixPCMData中
		else if (channelCount == 2)
		{
			for (int i = 0; i < mixDataCount; ++i)
			{
				mixPCMData[i] = (short)((dataBuffer[2 * i + 0] + dataBuffer[2 * i + 1]) * 0.5f);
			}
		}
	}
	public void refreshFileSize()
	{
		// 由于舍弃了fact块,所以需要重新计算文件大小,20是fmt块数据区的起始偏移,8是data块的头的大小
		mFileSize = 20 - 8 + mFmtChunkSize + 8 + mDataSize;
	}

	public void startWaveStream(WaveFormatEx waveHeader)
	{
		mWaveDataSerializer = new Serializer();
		byte[] riffByte = new byte[4] { (byte)'R', (byte)'I', (byte)'F', (byte)'F' };
		mRiffMark = BinaryUtility.bytesToInt(riffByte);
		mFileSize = 0;
		byte[] waveByte = new byte[4] { (byte)'W', (byte)'A', (byte)'V', (byte)'E' };
		mWaveMark = BinaryUtility.bytesToInt(waveByte);
		byte[] fmtByte = new byte[4] { (byte)'f', (byte)'m', (byte)'t', (byte)' ' };
		mFmtMark = BinaryUtility.bytesToInt(fmtByte);
		mFmtChunkSize = 16;
		mFormatType = waveHeader.wFormatTag;
		mSoundChannels = waveHeader.nChannels;
		mSamplesPerSec = waveHeader.nSamplesPerSec;
		mAvgBytesPerSec = waveHeader.nAvgBytesPerSec;
		mBlockAlign = waveHeader.nBlockAlign;
		mBitsPerSample = waveHeader.wBitsPerSample;
		mOtherSize = waveHeader.cbSize;
		mDataMark = new char[4] { 'd', 'a', 't', 'a' };
	}
	public void pushWaveStream(byte[] data, int dataSize)
	{
		mWaveDataSerializer.writeBuffer(data, dataSize);
	}

	public void endWaveStream()
	{
		mDataSize = mWaveDataSerializer.getDataSize();
		mDataBuffer = new byte[mDataSize];
		BinaryUtility.memcpy(mDataBuffer, mWaveDataSerializer.getBuffer(), 0, 0, mDataSize);
		mWaveDataSerializer = null;
		int mixDataCount = getMixPCMDataCount();
		mMixPCMData = new short[mixDataCount];
		generateMixPCMData(mMixPCMData, mixDataCount, mSoundChannels, mDataBuffer, mDataSize);
		refreshFileSize();
	}
}