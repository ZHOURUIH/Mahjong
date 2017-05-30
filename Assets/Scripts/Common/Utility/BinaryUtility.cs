using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BinaryUtility : GameBase
{
	public void init() { }
	// 计算 16进制的c中1的个数
	public static int crc_check(byte c)
	{
		int count = 0;
		int bitCount = sizeof(char) * 8;
		for (int i = 0; i < bitCount; ++i)
		{
			if ((c & (0x01 << i)) > 0)
			{
				++count;
			}
		}
		return count;
	}
	public static byte readByte(byte[] buffer, ref int curIndex)
	{
		if (buffer.Length < 1)
		{
			return 0;
		}
		byte byte0 = (byte)(0xff & buffer[curIndex++]);
		return byte0;
	}
	public static char readChar(byte[] buffer, ref int curIndex)
	{
		if (buffer.Length < 1)
		{
			return '\0';
		}
		char byte0 = (char)(0xff & buffer[curIndex++]);
		return byte0;
	}
	public static short readShort(byte[] buffer, ref int curIndex, bool inverse = false)
	{
		if (buffer.Length < 2)
		{
			return 0;
		}
		int byte0 = (int)(0xff & buffer[curIndex++]);
		int byte1 = (int)(0xff & buffer[curIndex++]);
		if(inverse)
		{
			short finalValue = (short)((byte1 << (8 * 0)) | (byte0 << (8 * 1)));
			return finalValue;
		}
		else
		{
			short finalValue = (short)((byte1 << (8 * 1)) | (byte0 << (8 * 0)));
			return finalValue;
		}
	}
	public static int readInt(byte[] buffer, ref int curIndex, bool inverse = false)
	{
		if (buffer.Length < 4)
		{
			return 0;
		}
		int byte0 = (int)(0xff & buffer[curIndex++]);
		int byte1 = (int)(0xff & buffer[curIndex++]);
		int byte2 = (int)(0xff & buffer[curIndex++]);
		int byte3 = (int)(0xff & buffer[curIndex++]);
		if (inverse)
		{
			int finalInt = (int)((byte3 << (8 * 0)) | (byte2 << (8 * 1)) | (byte1 << (8 * 2)) | (byte0 << (8 * 3)));
			return finalInt;
		}
		else
		{
			int finalInt = (int)((byte3 << (8 * 3)) | (byte2 << (8 * 2)) | (byte1 << (8 * 1)) | (byte0 << (8 * 0)));
			return finalInt;
		}
	}
	public static float readFloat(byte[] buffer, ref int curIndex, bool inverse = false)
	{
		if (buffer.Length < 4)
		{
			return 0.0f;
		}
		byte[] floatBuffer = new byte[4];
		if(inverse)
		{
			floatBuffer[3] = buffer[curIndex++];
			floatBuffer[2] = buffer[curIndex++];
			floatBuffer[1] = buffer[curIndex++];
			floatBuffer[0] = buffer[curIndex++];
		}
		else
		{
			floatBuffer[0] = buffer[curIndex++];
			floatBuffer[1] = buffer[curIndex++];
			floatBuffer[2] = buffer[curIndex++];
			floatBuffer[3] = buffer[curIndex++];
		}
		return System.BitConverter.ToSingle(floatBuffer, 0);
	}
	public static bool readBytes(byte[] buffer, ref int index, int bufferSize, byte[] destBuffer, int destBufferSize, int readSize)
	{
		if (bufferSize == -1)
		{
			bufferSize = buffer.Length;
		}
		if (destBufferSize == -1)
		{
			destBufferSize = destBuffer.Length;
		}
		if (readSize == -1)
		{
			readSize = destBuffer.Length;
		}
		if (destBufferSize < readSize || readSize + index > bufferSize)
		{
			return false;
		}
		Array.Copy(buffer, index, destBuffer, 0, readSize);
		index += readSize;
		return true;
	}
	public static bool readChars(byte[] buffer, ref int index, int bufferSize, char[] destBuffer, int destBufferSize, int readSize)
	{
		if(bufferSize == -1)
		{
			bufferSize = buffer.Length;
		}
		if(destBufferSize == -1)
		{
			destBufferSize = destBuffer.Length;
		}
		if(readSize == -1)
		{
			readSize = destBuffer.Length;
		}
		if (destBufferSize < readSize || readSize + index > bufferSize)
		{
			return false;
		}
		for(int i = 0; i < readSize; ++i)
		{
			destBuffer[i] = (char)buffer[i + index];
		}
		index += readSize;
		return true;
	}
	public static bool writeByte(byte[] buffer, ref int index, byte value)
	{
		if (buffer.Length < 1)
		{
			return false;
		}
		buffer[index++] = value;
		return true;
	}
	public static bool writeChar(byte[] buffer, ref int index, char value)
	{
		if (buffer.Length < 1)
		{
			return false;
		}
		buffer[index++] = (byte)(value);
		return true;
	}
	public static bool writeShort(byte[] buffer, ref int index, short value)
	{
		if (buffer.Length < 2)
		{
			return false;
		}
		buffer[index++] = (byte)((0x00ff & value) >> 0);
		buffer[index++] = (byte)((0xff00 & value) >> 8);
		return true;
	}
	public static bool writeInt(byte[] buffer, ref int index, int value)
	{
		if (buffer.Length < 4)
		{
			return false;
		}
		buffer[index++] = (byte)((0x000000ff & value) >> 0);
		buffer[index++] = (byte)((0x0000ff00 & value) >> 8);
		buffer[index++] = (byte)((0x00ff0000 & value) >> 16);
		buffer[index++] = (byte)((0xff000000 & value) >> 24);
		return true;
	}

	public static bool writeFloat(byte[] buffer, ref int index, float value)
	{
		if (buffer.Length < 4)
		{
			return false;
		}
		byte[] valueByte = System.BitConverter.GetBytes(value);
		for (int i = 0; i < 4; ++i)
		{
			buffer[index++] = valueByte[i];
		}
		return true;
	}
	public static bool writeBytes(byte[] buffer, ref int index, int bufferSize, byte[] sourceBuffer, int sourceBufferSize, int writeSize)
	{
		if (bufferSize == -1)
		{
			bufferSize = buffer.Length;
		}
		if (sourceBufferSize == -1)
		{
			sourceBufferSize = sourceBuffer.Length;
		}
		if (writeSize == -1)
		{
			writeSize = sourceBuffer.Length;
		}
		if (writeSize > sourceBufferSize || writeSize + index > bufferSize)
		{
			return false;
		}
		Array.Copy(sourceBuffer, 0, buffer, index, writeSize);
		index += writeSize;
		return true;
	}
	public static bool writeChars(byte[] buffer, ref int index, int bufferSize, char[] sourceBuffer, int sourceBufferSize, int writeSize)
	{
		if(bufferSize == -1)
		{
			bufferSize = buffer.Length;
		}
		if(sourceBufferSize == -1)
		{
			sourceBufferSize = sourceBuffer.Length;
		}
		if(writeSize == -1)
		{
			writeSize = sourceBuffer.Length;
		}
		if (writeSize > sourceBufferSize || writeSize + index > bufferSize)
		{
			return false;
		}
		for(int i = 0; i < writeSize; ++i)
		{
			buffer[index + i] = (byte)sourceBuffer[i];
		}
		index += writeSize;
		return true;
	}
	public static string byteArrayToHEXString(byte[] byteList, bool flag)
	{
		string byteString = "";
		foreach (var curByte in byteList)
		{
			if (flag)
			{
				byteString += byteToHexString(curByte) + " ";
			}
			else
			{
				byteString += byteToHexString(curByte);
			}

		}
		byteString = byteString.Substring(0, byteString.Length - 1);
		return byteString;
	}
	public static string byteToHexString(byte value)
	{
		string hexString = "";
		char[] hexChar = { 'A', 'B', 'C', 'D', 'E', 'F' };
		int high = value / 16;
		int low = value % 16;
		if (high < 10)
		{
			hexString += high.ToString();
		}
		else
		{
			hexString += hexChar[high - 10];
		}
		if (low < 10)
		{
			hexString += low.ToString();
		}
		else
		{
			hexString += hexChar[low - 10];
		}
		return hexString;
	}
	public static void memcpy<T>(T[] dest, T[] src, int destOffset, int srcOffset, int count)
	{
		for (int i = 0; i < count; ++i)
		{
			dest[destOffset + i] = src[srcOffset + i];
		}
	}
	public static void memcpy(byte[] dest, char[] src, int destOffset, int srcOffset, int count)
	{
		for (int i = 0; i < count; ++i)
		{
			dest[destOffset + i] = (byte)src[srcOffset + i];
		}
	}
	public static void memcpy(char[] dest, byte[] src, int destOffset, int srcOffset, int count)
	{
		for (int i = 0; i < count; ++i)
		{
			dest[destOffset + i] = (char)src[srcOffset + i];
		}
	}
	public static void memmove(short[] data, int start0, int start1, int count)
	{
		if (start1 > start0 && (start0 + count > start1))
		{
			// 如果源地址与目标地址有重叠,并且源地址在前面,则从后面往前拷贝字节
			for (int i = 0; i < count; ++i)
			{
				data[count - i - 1 + start0] = data[count - i - 1 + start1];
			}
		}
		else
		{
			for (int i = 0; i < count; ++i)
			{
				data[i + start0] = data[i + start1];
			}
		}
	}
	public static void memset<T>(T[] p, T value, int length = -1)
	{
		if(length == -1)
		{
			length = p.Length;
		}
		for (int i = 0; i < length; ++i)
		{
			p[i] = value;
		}
	}
	public static byte[] toByteArray(char value)
	{
		return System.BitConverter.GetBytes(value);
	}
	public static byte[] toByteArray(byte value)
	{
		return System.BitConverter.GetBytes(value);
	}
	public static byte[] toByteArray(short value)
	{
		return System.BitConverter.GetBytes(value);
	}
	public static byte[] toByteArray(int value)
	{
		return System.BitConverter.GetBytes(value);
	}
	public static byte[] toByteArray(float value)
	{
		return System.BitConverter.GetBytes(value);
	}
	public static byte[] toByteArray(string value)
	{
		return Encoding.Default.GetBytes(value);
	}
	public static byte byteArrayToByte(byte[] array)
	{
		return array[0];
	}
	public static char byteArrayToChar(byte[] array)
	{
		return System.BitConverter.ToChar(array, 0);
	}
	public static short byteArrayToShort(byte[] array)
	{
		return System.BitConverter.ToInt16(array, 0);
	}
	public static int byteArrayToInt(byte[] array)
	{
		return System.BitConverter.ToInt32(array, 0);
	}
	public static float byteArrayToFloat(byte[] array)
	{
		return System.BitConverter.ToSingle(array, 0);
	}
	public static string byteArrayToString(byte[] array)
	{
		string str = "";
		int arrayLen = array.Length;
		for(int i = 0; i < arrayLen; ++i)
		{
			if(array[i] == 0)
			{
				break;
			}
			str += (char)array[i];
		}
		return str;
	}
}