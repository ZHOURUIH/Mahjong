using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BinaryUtility
{
	protected static Encoding ENCODING_GB2312;
	/** CRC table for the CRC-16. The poly is 0x8005 (x^16 + x^15 + x^2 + 1) */
	protected static ushort[] crc16_table = new ushort[256]
	{
		0x0000, 0xC0C1, 0xC181, 0x0140, 0xC301, 0x03C0, 0x0280, 0xC241,
		0xC601, 0x06C0, 0x0780, 0xC741, 0x0500, 0xC5C1, 0xC481, 0x0440,
		0xCC01, 0x0CC0, 0x0D80, 0xCD41, 0x0F00, 0xCFC1, 0xCE81, 0x0E40,
		0x0A00, 0xCAC1, 0xCB81, 0x0B40, 0xC901, 0x09C0, 0x0880, 0xC841,
		0xD801, 0x18C0, 0x1980, 0xD941, 0x1B00, 0xDBC1, 0xDA81, 0x1A40,
		0x1E00, 0xDEC1, 0xDF81, 0x1F40, 0xDD01, 0x1DC0, 0x1C80, 0xDC41,
		0x1400, 0xD4C1, 0xD581, 0x1540, 0xD701, 0x17C0, 0x1680, 0xD641,
		0xD201, 0x12C0, 0x1380, 0xD341, 0x1100, 0xD1C1, 0xD081, 0x1040,
		0xF001, 0x30C0, 0x3180, 0xF141, 0x3300, 0xF3C1, 0xF281, 0x3240,
		0x3600, 0xF6C1, 0xF781, 0x3740, 0xF501, 0x35C0, 0x3480, 0xF441,
		0x3C00, 0xFCC1, 0xFD81, 0x3D40, 0xFF01, 0x3FC0, 0x3E80, 0xFE41,
		0xFA01, 0x3AC0, 0x3B80, 0xFB41, 0x3900, 0xF9C1, 0xF881, 0x3840,
		0x2800, 0xE8C1, 0xE981, 0x2940, 0xEB01, 0x2BC0, 0x2A80, 0xEA41,
		0xEE01, 0x2EC0, 0x2F80, 0xEF41, 0x2D00, 0xEDC1, 0xEC81, 0x2C40,
		0xE401, 0x24C0, 0x2580, 0xE541, 0x2700, 0xE7C1, 0xE681, 0x2640,
		0x2200, 0xE2C1, 0xE381, 0x2340, 0xE101, 0x21C0, 0x2080, 0xE041,
		0xA001, 0x60C0, 0x6180, 0xA141, 0x6300, 0xA3C1, 0xA281, 0x6240,
		0x6600, 0xA6C1, 0xA781, 0x6740, 0xA501, 0x65C0, 0x6480, 0xA441,
		0x6C00, 0xACC1, 0xAD81, 0x6D40, 0xAF01, 0x6FC0, 0x6E80, 0xAE41,
		0xAA01, 0x6AC0, 0x6B80, 0xAB41, 0x6900, 0xA9C1, 0xA881, 0x6840,
		0x7800, 0xB8C1, 0xB981, 0x7940, 0xBB01, 0x7BC0, 0x7A80, 0xBA41,
		0xBE01, 0x7EC0, 0x7F80, 0xBF41, 0x7D00, 0xBDC1, 0xBC81, 0x7C40,
		0xB401, 0x74C0, 0x7580, 0xB541, 0x7700, 0xB7C1, 0xB681, 0x7640,
		0x7200, 0xB2C1, 0xB381, 0x7340, 0xB101, 0x71C0, 0x7080, 0xB041,
		0x5000, 0x90C1, 0x9181, 0x5140, 0x9301, 0x53C0, 0x5280, 0x9241,
		0x9601, 0x56C0, 0x5780, 0x9741, 0x5500, 0x95C1, 0x9481, 0x5440,
		0x9C01, 0x5CC0, 0x5D80, 0x9D41, 0x5F00, 0x9FC1, 0x9E81, 0x5E40,
		0x5A00, 0x9AC1, 0x9B81, 0x5B40, 0x9901, 0x59C0, 0x5880, 0x9841,
		0x8801, 0x48C0, 0x4980, 0x8941, 0x4B00, 0x8BC1, 0x8A81, 0x4A40,
		0x4E00, 0x8EC1, 0x8F81, 0x4F40, 0x8D01, 0x4DC0, 0x4C80, 0x8C41,
		0x4400, 0x84C1, 0x8581, 0x4540, 0x8701, 0x47C0, 0x4680, 0x8641,
		0x8201, 0x42C0, 0x4380, 0x8341, 0x4100, 0x81C1, 0x8081, 0x4040
	};
	public static Encoding getGB2312()
	{
		if(ENCODING_GB2312 == null)
		{
			ENCODING_GB2312 = Encoding.GetEncoding("gb2312");
		}
		return ENCODING_GB2312;
	}
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
	public static ushort crc16(ushort crc, byte[] buffer, int len, int bufferOffset = 0)
	{
		for(int i = 0; i < len; ++i)
		{
			crc = crc16_byte(crc, buffer[bufferOffset + i]);
		}
		return crc;
	}
	public static ushort crc16_byte(ushort crc, byte data)
	{
		return (ushort)((crc >> 8) ^ crc16_table[(crc ^ data) & 0xFF]);
	}
	public static bool readBool(byte[] buffer, ref int curIndex)
	{
		if (buffer.Length < 1)
		{
			return false;
		}
		bool value = (0xff & buffer[curIndex++]) != 0;
		return value;
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
		return bytesToFloat(floatBuffer);
	}
	public static void readBools(byte[] buffer, ref int index, bool[] destBuffer)
	{
		int shortCount = destBuffer.Length;
		for (int i = 0; i < shortCount; ++i)
		{
			destBuffer[i] = readBool(buffer, ref index);
		}
	}
	public static bool readBytes(byte[] buffer, ref int index, byte[] destBuffer, int bufferSize = -1, int destBufferSize = -1, int readSize = -1)
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
		memcpy(destBuffer, buffer, 0, index, readSize);
		index += readSize;
		return true;
	}
	public static void readShorts(byte[] buffer, ref int index, short[] destBuffer)
	{
		int shortCount = destBuffer.Length;
		for(int i = 0; i < shortCount; ++i)
		{
			destBuffer[i] = readShort(buffer, ref index);
		}
	}
	public static void readInts(byte[] buffer, ref int index, int[] destBuffer)
	{
		int shortCount = destBuffer.Length;
		for (int i = 0; i < shortCount; ++i)
		{
			destBuffer[i] = readInt(buffer, ref index);
		}
	}
	public static void readFloats(byte[] buffer, ref int index, float[] destBuffer)
	{
		int shortCount = destBuffer.Length;
		for (int i = 0; i < shortCount; ++i)
		{
			destBuffer[i] = readFloat(buffer, ref index);
		}
	}
	public static bool writeBool(byte[] buffer, ref int index, bool value)
	{
		if (buffer.Length < 1)
		{
			return false;
		}
		buffer[index++] = (byte)(value ? 1 : 0);
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
	public static bool writeShort(byte[] buffer, ref int index, short value, bool inverse = false)
	{
		if (buffer.Length < 2)
		{
			return false;
		}
		if(inverse)
		{
			buffer[index++] = (byte)((0xff00 & value) >> 8);
			buffer[index++] = (byte)((0x00ff & value) >> 0);	
		}
		else
		{
			buffer[index++] = (byte)((0x00ff & value) >> 0);
			buffer[index++] = (byte)((0xff00 & value) >> 8);
		}
		return true;
	}
	public static bool writeInt(byte[] buffer, ref int index, int value, bool inverse = false)
	{
		if (buffer.Length < 4)
		{
			return false;
		}
		if(inverse)
		{
			buffer[index++] = (byte)((0xff000000 & value) >> 24);
			buffer[index++] = (byte)((0x00ff0000 & value) >> 16);
			buffer[index++] = (byte)((0x0000ff00 & value) >> 8);
			buffer[index++] = (byte)((0x000000ff & value) >> 0);
		}
		else
		{
			buffer[index++] = (byte)((0x000000ff & value) >> 0);
			buffer[index++] = (byte)((0x0000ff00 & value) >> 8);
			buffer[index++] = (byte)((0x00ff0000 & value) >> 16);
			buffer[index++] = (byte)((0xff000000 & value) >> 24);
		}
		
		return true;
	}

	public static bool writeFloat(byte[] buffer, ref int index, float value)
	{
		if (buffer.Length < 4)
		{
			return false;
		}
		byte[] valueByte = toBytes(value);
		for (int i = 0; i < 4; ++i)
		{
			buffer[index++] = valueByte[i];
		}
		return true;
	}
	public static bool writeBools(byte[] buffer, ref int index, bool[] sourceBuffer)
	{
		bool ret = true;
		int floatCount = sourceBuffer.Length;
		for (int i = 0; i < floatCount; ++i)
		{
			ret = writeBool(buffer, ref index, sourceBuffer[i]) && ret;
		}
		return ret;
	}
	public static bool writeBytes(byte[] buffer, ref int index, byte[] sourceBuffer, int bufferSize = -1, int sourceBufferSize = -1, int writeSize = -1)
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
		memcpy(buffer, sourceBuffer, index, 0, writeSize);
		index += writeSize;
		return true;
	}
	public static bool writeShorts(byte[] buffer, ref int index, short[] sourceBuffer)
	{
		bool ret = true;
		int floatCount = sourceBuffer.Length;
		for (int i = 0; i < floatCount; ++i)
		{
			ret = writeShort(buffer, ref index, sourceBuffer[i]) && ret;
		}
		return ret;
	}
	public static bool writeInts(byte[] buffer, ref int index, int[] sourceBuffer)
	{
		bool ret = true;
		int floatCount = sourceBuffer.Length;
		for (int i = 0; i < floatCount; ++i)
		{
			ret = writeInt(buffer, ref index, sourceBuffer[i]) && ret;
		}
		return ret;
	}
	public static bool writeFloats(byte[] buffer, ref int index, float[] sourceBuffer)
	{
		bool ret = true;
		int floatCount = sourceBuffer.Length;
		for(int i = 0; i < floatCount; ++i)
		{
			ret = writeFloat(buffer, ref index, sourceBuffer[i]) && ret;
		}
		return ret;
	}
	public static string bytesToHEXString(byte[] byteList, bool addSpace = true, bool upperOrLower = true)
	{
		string byteString = "";
		int byteCount = byteList.Length;
		for (int i = 0; i < byteCount; ++i)
		{
			if (addSpace)
			{
				byteString += byteToHEXString(byteList[i], upperOrLower) + " ";
			}
			else
			{
				byteString += byteToHEXString(byteList[i], upperOrLower);
			}
		}
		if (addSpace)
		{
			byteString = byteString.Substring(0, byteString.Length - 1);
		}
		return byteString;
	}
	public static string byteToHEXString(byte value, bool upperOrLower = true)
	{
		string hexString = "";
		char[] hexChar = null;
		if (upperOrLower)
		{
			hexChar = new char[] { 'A', 'B', 'C', 'D', 'E', 'F' };
		}
		else
		{
			hexChar = new char[] { 'a', 'b', 'c', 'd', 'e', 'f' };
		}
		int high = value / 16;
		int low = value % 16;
		if (high < 10)
		{
			hexString += (char)('0' + high);
		}
		else
		{
			hexString += hexChar[high - 10];
		}
		if (low < 10)
		{
			hexString += (char)('0' + low);
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
	public static void memcpy<T>(T[] dest, T[] src, int destOffset)
	{
		memcpy(dest, src, destOffset, 0, src.Length);
	}
	public static void memmove<T>(T[] data, int start0, int start1, int count)
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
	public static byte[] toBytes(byte value)
	{
		return BitConverter.GetBytes(value);
	}
	public static byte[] toBytes(short value)
	{
		return BitConverter.GetBytes(value);
	}
	public static byte[] toBytes(int value)
	{
		return BitConverter.GetBytes(value);
	}
	public static byte[] toBytes(float value)
	{
		return BitConverter.GetBytes(value);
	}
	public static byte bytesToByte(byte[] array)
	{
		return array[0];
	}
	public static short bytesToShort(byte[] array)
	{
		return BitConverter.ToInt16(array, 0);
	}
	public static int bytesToInt(byte[] array)
	{
		return BitConverter.ToInt32(array, 0);
	}
	public static float bytesToFloat(byte[] array)
	{
		return BitConverter.ToSingle(array, 0);
	}
	public static byte[] stringToBytes(string str)
	{
		return stringToBytes(str, getGB2312());
	}
	public static byte[] stringToBytes(string str, Encoding encoding)
	{
		return encoding.GetBytes(str);
	}
	public static string bytesToString(byte[] bytes)
	{
		return bytesToString(bytes, Encoding.Default);
	}
	public static string bytesToString(byte[] bytes, Encoding encoding)
	{
		return removeLastZero(encoding.GetString(bytes));
	}
	public static string convertStringFormat(string str, Encoding source, Encoding target)
	{
		return bytesToString(stringToBytes(str, source), target);
	}
	public static string UTF8ToUnicode(string str)
	{
		return convertStringFormat(str, Encoding.UTF8, Encoding.Unicode);
	}
	public static string UTF8ToGB2312(string str)
	{
		return convertStringFormat(str, Encoding.UTF8, getGB2312());
	}
	public static string UnicodeToUTF8(string str)
	{
		return convertStringFormat(str, Encoding.Unicode, Encoding.UTF8);
	}
	public static string UnicodeToGB2312(string str)
	{
		return convertStringFormat(str, Encoding.Unicode, getGB2312());
	}
	public static string GB2312ToUTF8(string str)
	{
		return convertStringFormat(str, getGB2312(), Encoding.UTF8);
	}
	public static string GB2312ToUnicode(string str)
	{
		return convertStringFormat(str, getGB2312(), Encoding.Unicode);
	}
	// 字节数组转换为字符串时,末尾可能会带有数字0,此时在字符串比较时会出现错误,所以需要移除字符串末尾的0
	public static string removeLastZero(string str)
	{
		int strLen = str.Length;
		int newLen = strLen;
		for(int i = 0; i < strLen; ++i)
		{
			if(str[i] == 0)
			{
				newLen = i;
				break;
			}
		}
		str = str.Substring(0, newLen);
		return str;
	}
}