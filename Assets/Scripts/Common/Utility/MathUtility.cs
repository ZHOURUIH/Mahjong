using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MathUtility : GameBase
{
	protected static float[] sin_tb = null;
	protected static float[] cos_tb = null;
	protected static int mMaxFFTCount = 1024 * 8;
	protected static Complex[] mComplexList;
	public void init()
	{
		sin_tb = new float[]
		{  // 精度(PI PI/2 PI/4 PI/8 PI/16 ... PI/(2^k))
			0.000000f, 1.000000f, 0.707107f, 0.382683f, 0.195090f, 0.098017f,
			0.049068f, 0.024541f, 0.012272f, 0.006136f, 0.003068f, 0.001534f,
			0.000767f, 0.000383f, 0.000192f, 0.000096f, 0.000048f, 0.000024f,
			0.000012f, 0.000006f, 0.000003f, 0.000003f, 0.000003f, 0.000003f,
			0.000003f
		};
		cos_tb = new float[]
		{  // 精度(PI PI/2 PI/4 PI/8 PI/16 ... PI/(2^k))
			-1.000000f, 0.000000f, 0.707107f, 0.923880f, 0.980785f, 0.995185f,
			0.998795f, 0.999699f, 0.999925f, 0.999981f, 0.999995f, 0.999999f,
			1.000000f, 1.000000f, 1.000000f, 1.000000f, 1.000000f, 1.000000f,
			1.000000f, 1.000000f, 1.000000f, 1.000000f, 1.000000f, 1.000000f,
			1.000000f
		};
		mComplexList = new Complex[mMaxFFTCount];
		for (int i = 0; i < mMaxFFTCount; ++i)
		{
			mComplexList[i] = new Complex();
		}
	}
	public static float KMHtoMS(float kmh) { return kmh / 3.6f; }		// km/h转m/s
	public static float MStoKMH(float ms) { return ms * 3.6f; }
	public static float calculateFloat(string str)
	{
		// 判断字符串是否含有非法字符,也就是除数字,小数点,运算符以外的字符
		string newString = "";
		int oldStrLen = str.Length;
		for (int i = 0; i < oldStrLen; ++i)
		{
			if ((str[i] < '0' || str[i] > '9') && str[i] != '.' && str[i] != '+'
				&& str[i] != '-' && str[i] != '*' && str[i] != '/' && str[i] != '(' && str[i] != ')')
			{
				;
			}
			else
			{
				newString += str[i];
			}
		}
		str = newString;
		// 判断左右括号数量是否相等
		int leftBracketCount = 0;
		int rightBracketCount = 0;
		int newStrLen = str.Length;
		for (int i = 0; i < newStrLen; ++i)
		{
			if (str[i] == '(')
			{
				++leftBracketCount;
			}
			else if (str[i] == ')')
			{
				++rightBracketCount;
			}
		}
		if (leftBracketCount != rightBracketCount)
		{
			// 计算错误,左右括号数量不对应
			return 0;
		}

		// 循环判断传入的字符串有没有括号
		while (true)
		{
			// 先判断有没有括号，如果有括号就先算括号里的,如果没有就退出while循环
			if (str.IndexOf("(") != -1 || str.IndexOf(")") != -1)
			{
				int curpos = str.LastIndexOf("(");
				string strInBracket = str.Substring(curpos + 1, str.Length - curpos - 1);
				strInBracket = strInBracket.Substring(0, strInBracket.IndexOf(")"));
				float ret = calculateFloat(strInBracket);
				// 如果括号中的计算结果是负数,则标记为负数
				bool isMinus = false;
				if (ret < 0)
				{
					ret = -ret;
					isMinus = true;
				}
				// 将括号中的计算结果替换原来的表达式,包括括号也一起替换
				string floatStr = (Math.Round(ret, 4)).ToString();
				str = StringUtility.strReplace(str, curpos, curpos + strInBracket.Length + 2, floatStr);
				char[] strchar = str.ToCharArray();
				if (isMinus)
				{
					// 如果括号中计算出来是负数,则将负号提取出来,将左边的第一个加减号改为相反的符号
					bool changeMark = false;
					for (int i = curpos - 1; i >= 0; --i)
					{
						// 找到第一个+号,则直接改为减号,然后退出遍历
						if (strchar[i] == '+')
						{
							strchar[i] = '-';
							str = StringUtility.charArrayToString(strchar);
							changeMark = true;
							break;
						}
						// 找到第一个减号,如果减号的左边有数字,则直接改为+号
						// 如果减号的左边不是数字,则该减号是负号,将减号去掉,
						else if (strchar[i] == '-')
						{
							if (strchar[i - 1] >= '0' && strchar[i - 1] <= '9')
							{
								strchar[i] = '+';
								str = StringUtility.charArrayToString(strchar);
							}
							else
							{
								str = StringUtility.strReplace(str, i, i + 1, "");
							}
							changeMark = true;
							break;
						}
					}
					// 如果遍历完了还没有找到可以替换的符号,则在表达式最前面加一个负号
					if (!changeMark)
					{
						str = "-" + str;
					}
				}
			}
			else
			{
				break;
			}
		}
		List<float> numbers = new List<float>();
		List<char> factors = new List<char>();
		// 表示上一个运算符的下标+1
		int beginpos = 0;
		for (int i = 0; i < str.Length; ++i)
		{
			// 遍历到了最后一个字符,则直接把最后一个数字放入列表,然后退出循环
			if (i == str.Length - 1)
			{
				string num = str.Substring(beginpos, str.Length - beginpos);
				float fNum = float.Parse(num);
				numbers.Add(fNum);
				break;
			}
			// 找到第一个运算符
			if ((str[i] < '0' || str[i] > '9') && str[i] != '.')
			{
				if (i != 0)
				{
					string num = str.Substring(beginpos, i - beginpos);
					float fNum = float.Parse(num);
					numbers.Add(fNum);
				}
				// 如果在表达式的开始就发现了运算符,则表示第一个数是负数,那就处理为0减去这个数的绝对值
				else
				{
					numbers.Add(0);
				}
				factors.Add(str[i]);
				beginpos = i + 1;
			}
		}
		if (factors.Count + 1 != numbers.Count)
		{
			// 计算错误,运算符与数字数量不符
			return 0;
		}
		// 现在开始计算表达式,按照运算优先级,先计算乘除和取余
		while (true)
		{
			// 表示是否还有乘除表达式
			bool hasMS = false;
			for (int i = 0; i < (int)factors.Count; ++i)
			{
				// 先遍历到哪个就先计算哪个
				if (factors[i] == '*' || factors[i] == '/')
				{
					// 第一个运算数的下标与运算符的下标是相同的
					float num1 = numbers[i];
					float num2 = numbers[i + 1];
					float num3 = 0.0f;
					if (factors[i] == '*')
					{
						num3 = num1 * num2;
					}
					else if (factors[i] == '/')
					{
						num3 = num1 / num2;
					}
					// 删除第i + 1个数,然后将第i个数替换为计算结果
					numbers.RemoveAt(i + 1);
					if (numbers.Count == 0)
					{
						// 计算错误
						return 0;
					}
					numbers[i] = num3;
					// 删除第i个运算符
					factors.RemoveAt(i);
					hasMS = true;
					break;
				}
			}
			if (!hasMS)
			{
				break;
			}
		}
		// 再计算加减法
		while (true)
		{
			if (factors.Count == 0)
			{
				break;
			}
			if (factors[0] == '+' || factors[0] == '-')
			{
				// 第一个运算数的下标与运算符的下标是相同的
				float num1 = numbers[0];
				float num2 = numbers[1];
				float num3 = 0.0f;
				if (factors[0] == '+')
				{
					num3 = num1 + num2;
				}
				else if (factors[0] == '-')
				{
					num3 = num1 - num2;
				}
				// 删除第1个数,然后将第0个数替换为计算结果
				numbers.RemoveAt(1);
				if (numbers.Count == 0)
				{
					// 计算错误
					return 0;
				}
				numbers[0] = num3;
				// 删除第0个运算符
				factors.RemoveAt(0);
			}
		}
		if (numbers.Count != 1)
		{
			// 计算错误
			return 0;
		}
		else
		{
			return numbers[0];
		}
	}
	public static void checkInt(ref float value, float precision = 0.0001f)
	{
		// 先判断是否为0
		if (isFloatZero(value, precision))
		{
			value = 0.0f;
			return;
		}
		int intValue = (int)value;
		// 大于0
		if (value > 0.0f)
		{
			// 如果原值减去整数值小于0.5f,则表示原值可能接近于整数值
			if (value - (float)intValue < 0.5f)
			{
				if (isFloatZero(value - intValue, precision))
				{
					value = (float)intValue;
				}
			}
			// 如果原值减去整数值大于0.5f, 则表示原值可能接近于整数值+1
			else
			{
				if (isFloatZero(value - (intValue + 1), precision))
				{
					value = (float)(intValue + 1);
				}
			}
		}
		// 小于0
		else if (value < 0.0f)
		{
			// 如果原值减去整数值的结果的绝对值小于0.5f,则表示原值可能接近于整数值
			if (Math.Abs(value - (float)intValue) < 0.5f)
			{
				if (isFloatZero(value - intValue, precision))
				{
					value = (float)intValue;
				}
			}
			else
			{
				// 如果原值减去整数值的结果的绝对值大于0.5f, 则表示原值可能接近于整数值-1
				if (isFloatZero(value - (intValue - 1), precision))
				{
					value = (float)(intValue - 1);
				}
			}
		}
	}
	public static float randomFloat(float min, float max)
	{
		return UnityEngine.Random.Range(min, max);
	}
	public static int randomInt(int min, int max)
	{
		if(min == max)
		{
			return min;
		}
		else
		{
			max += 1;
		}
		return UnityEngine.Random.Range(min, max);
	}
	public static float getLength(Vector3 vec)
	{
		return Mathf.Sqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z);
	}
	public static Matrix4x4 identityMatrix4(Matrix4x4 rot)
	{
		Vector3 vec0 = new Vector3(rot.m00, rot.m01, rot.m02);
		Vector3 vec1 = new Vector3(rot.m10, rot.m11, rot.m12);
		Vector3 vec2 = new Vector3(rot.m20, rot.m21, rot.m22);
		vec0.Normalize();
		vec1.Normalize();
		vec2.Normalize();
		Matrix4x4 temp = new Matrix4x4();
		temp.m00 = vec0.x;
		temp.m01 = vec0.y;
		temp.m02 = vec0.z;
		temp.m10 = vec1.x;
		temp.m11 = vec1.y;
		temp.m12 = vec1.z;
		temp.m20 = vec2.x;
		temp.m21 = vec2.y;
		temp.m22 = vec2.z;
		return temp;
	}
	public static Vector3 matrix3ToEulerAngle(Matrix4x4 rot)
	{
		Matrix4x4 tempMat4 = identityMatrix4(rot);
		// 计算滚动角
		// 首先求出矩阵中X-Y平面与世界坐标系水平面的交线
		// 交线为X = -rot[2][2] / rot[2][0] * Z,然后随意构造出一个向量
		Vector3 intersectLineVector;
		float angleRoll = 0.0f;
		if (!isFloatZero(tempMat4.m20) || !isFloatZero(tempMat4.m22))
		{
			// 矩阵中Z轴的x分量为0,则交线在世界坐标系的X轴上,取X轴正方向上的一个点
			if (isFloatZero(tempMat4.m20) && !isFloatZero(tempMat4.m22))
			{
				intersectLineVector = new Vector3(1.0f, 0.0f, 0.0f);
			}
			// 矩阵中Z轴的z分量为0,则交线在世界坐标系的Z轴上,
			else if (!isFloatZero(tempMat4.m20) && isFloatZero(tempMat4.m22))
			{
				// Z轴朝向世界坐标系的X轴正方向,即Z轴的x分量大于0,应该计算X轴与世界坐标系的Z轴负方向的夹角
				if (tempMat4.m20 > 0.0f)
				{
					intersectLineVector = new Vector3(0.0f, 0.0f, -1.0f);
				}
				// Z轴朝向世界坐标系的X轴负方向,应该计算X轴与世界坐标系的Z轴正方向的夹角
				else
				{
					intersectLineVector = new Vector3(0.0f, 0.0f, 1.0f);
				}
			}
			// 矩阵中Z轴的x和z分量都不为0,取X轴正方向上的一个点
			else
			{
				intersectLineVector = new Vector3(1.0f, 0.0f, -tempMat4.m20 / tempMat4.m22);
			}
			// 然后求出矩阵中X轴与交线的夹角
			angleRoll = getAngleBetweenVector(intersectLineVector, new Vector3(tempMat4.m00, tempMat4.m01, tempMat4.m02));
			// 如果X轴的y分量大于0,则滚动角为负
			if (tempMat4.m01 > 0.0f)
			{
				angleRoll = -angleRoll;
			}
		}
		// 如果Z轴的x和z分量都为0,则俯仰角为90°或者-90°,此处不计算
		else
		{
			// 此时X-Y平面与水平面相平行,计算X轴与世界坐标系中X轴的夹角,X轴的z分量小于0时,滚动角为负
			angleRoll = getAngleBetweenVector(new Vector3(tempMat4.m00, tempMat4.m01, tempMat4.m02), new Vector3(1.0f, 0.0f, 0.0f));
			if (tempMat4.m02 < 0.0f)
			{
				angleRoll = -angleRoll;
			}
		}

		// 计算出滚动角后,将矩阵中的滚动角归0
		Matrix4x4 nonRollMat = rot;
		if (!isFloatZero(angleRoll))
		{
			nonRollMat *= getRollMatrix3(-angleRoll);
		}

		// 然后计算俯仰角
		// Z轴与Z轴在水平面上的投影的夹角
		Vector3 zAxisInMatrix = new Vector3(nonRollMat.m20, nonRollMat.m21, nonRollMat.m22);
		float anglePitch = 0.0f;
		if (!isFloatZero(zAxisInMatrix.x) || !isFloatZero(zAxisInMatrix.z))
		{
			anglePitch = getAngleBetweenVector(zAxisInMatrix, new Vector3(zAxisInMatrix.x, 0.0f, zAxisInMatrix.z));
			// Z轴的y分量小于0,则俯仰角为负
			if (nonRollMat.m21 < 0.0f)
			{
				anglePitch = -anglePitch;
				// 如果Y轴的y分量小于0,则说明俯仰角的绝对值已经大于90°了
				if (nonRollMat.m11 < 0.0f)
				{
					anglePitch = -Mathf.PI - anglePitch;
				}
			}
			else
			{
				if (nonRollMat.m11 < 0.0f)
				{
					anglePitch = Mathf.PI - anglePitch;
				}
			}
		}
		// 如果在水平面上的投影为0,俯仰角为90°或-90°
		else
		{
			if (nonRollMat.m21 > 0.0f)
			{
				anglePitch = Mathf.PI * 0.5f;
			}
			else
			{
				anglePitch = -Mathf.PI * 0.5f;
			}
		}

		// 然后计算航向角
		// X轴与世界坐标系中的X轴的夹角
		float angleYaw = getAngleBetweenVector(new Vector3(nonRollMat.m00, nonRollMat.m01, nonRollMat.m02), Vector3.right);
		// X轴的z分量小于0,则航向角为负
		if (nonRollMat.m02 < 0.0f)
		{
			angleYaw = -angleYaw;
		}
		return new Vector3(angleYaw, anglePitch, angleRoll);
	}
	public static float getAngleBetweenVector(Vector3 vec1, Vector3 vec2)
	{
		Vector3 curVec1 = normalize(vec1);
		Vector3 curVec2 = normalize(vec2);
		float dotValue = Vector3.Dot(curVec1, curVec2);
		clamp(ref dotValue, -1.0f, 1.0f);
		return Mathf.Acos(dotValue);
	}
	public static Vector3 normalize(Vector3 vec3)
	{
		float length = getLength(vec3);
		if (isFloatZero(length))
		{
			return Vector3.zero;
		}
		float inverseLen = 1.0f / length;
		return new Vector3(vec3.x * inverseLen, vec3.y * inverseLen, vec3.z * inverseLen);
	}

	public static Matrix4x4 eulerAngleToMatrix3(Vector3 angle)
	{
		// 分别计算三个分量的旋转矩阵,然后相乘得出最后的结果
		return getYawMatrix3(angle.x) * getPitchMatrix3(angle.y) * getRollMatrix3(angle.z);
	}
	public static Matrix4x4 getYawMatrix3(float angle)
	{
		float cosY = Mathf.Cos(angle);
		float sinY = Mathf.Sin(angle);
		Matrix4x4 rot = new Matrix4x4();
		rot.m00 = cosY;
		rot.m01 = 0.0f;
		rot.m02 = sinY;
		rot.m10 = 0.0f;
		rot.m11 = 1.0f;
		rot.m12 = 0.0f;
		rot.m20 = -sinY;
		rot.m21 = 0.0f;
		rot.m22 = cosY;
		return rot;
	}
	public static Matrix4x4 getPitchMatrix3(float angle)
	{
		float cosZ = Mathf.Cos(angle);
		float sinZ = Mathf.Sin(angle);
		Matrix4x4 rot = new Matrix4x4();
		rot.m00 = 1.0f;
		rot.m01 = 0.0f;
		rot.m02 = 0.0f;
		rot.m10 = 0.0f;
		rot.m11 = cosZ;
		rot.m12 = -sinZ;
		rot.m20 = 0.0f;
		rot.m21 = sinZ;
		rot.m22 = cosZ;
		return rot;
	}
	public static Matrix4x4 getRollMatrix3(float angle)
	{
		float cosX = Mathf.Cos(angle);
		float sinX = Mathf.Sin(angle);
		Matrix4x4 rot = new Matrix4x4();
		rot.m00 = cosX;
		rot.m01 = -sinX;
		rot.m02 = 0.0f;
		rot.m10 = sinX;
		rot.m11 = cosX;
		rot.m12 = 0.0f;
		rot.m20 = 0.0f;
		rot.m21 = 0.0f;
		rot.m22 = 1.0f;
		return rot;
	}
	public static float degreeToRadian(float degreeAngle)
	{
		return degreeAngle * Mathf.Deg2Rad;
	}
	public static float radianToDegree(float radianAngle)
	{
		return radianAngle * Mathf.Rad2Deg;
	}
	public static float getQuaternionYaw(Quaternion q)
	{
		return q.eulerAngles.y;
	}
	public static float getQuaternionPitch(Quaternion q)
	{
		return q.eulerAngles.z;
	}
	public static float getQuaternionRoll(Quaternion q)
	{
		return q.eulerAngles.x;
	}
	public static int getMin(int a, int b)
	{
		return a < b ? a : b;
	}
	public static int getMax(int a, int b)
	{
		return a > b ? a : b;
	}
	public static float getMin(float a, float b)
	{
		return a < b ? a : b;
	}
	public static float getMax(float a, float b)
	{
		return a > b ? a : b;
	}
	public static void clamp(ref float value, float min, float max)
	{
		if (min >= max)
		{
			return;
		}
		if (value < min)
		{
			value = min;
		}
		else if (value > max)
		{
			value = max;
		}
	}
	public static void clamp(ref int value, int min, int max)
	{
		if(min > max)
		{
			return;
		}
		if (min == max)
		{
			value = min;
			return;
		}
		if (value < min)
		{
			value = min;
		}
		else if (value > max)
		{
			value = max;
		}
	}
	public static bool isFloatZero(float value, float precision = 0.0001f)
	{
		return value >= -precision && value <= precision;
	}
	public static bool isFloatEqual(float value1, float value2, float precision = 0.0001f)
	{
		return isFloatZero(value1 - value2);
	}
	public static float getAngleFromVectorToVector(Vector2 from, Vector2 to)
	{
		float dotValue = Vector2.Dot(from, to);
		float fromLength = from.magnitude;
		float toLength = to.magnitude;
		float angleValue = dotValue / (fromLength * toLength);
		if (angleValue > 1.0f)
		{
			angleValue = 1.0f;
		}
		else if (angleValue < -1.0f)
		{
			angleValue = -1.0f;
		}
		float angle = Mathf.Acos(angleValue);

		Vector3 crossVec = Vector3.Cross(new Vector3(from.x, 0.0f, from.y), new Vector3(to.x, 0.0f, to.y));
		if (crossVec.y > 0.0f)
		{
			angle = -angle;
		}
		return angle;
	}
	public static void adjustAngle180(ref float radianAngle, bool radian)
	{
		float pi = radian ? Mathf.PI : 180.0f;
		// 如果小于-PI,则循环加上PI * 2
		while (radianAngle < -pi)
		{
			radianAngle += pi * 2.0f;
		}
		// 如果大于PI,则循环减去PI * 2
		while (radianAngle > pi)
		{
			radianAngle -= pi * 2.0f;
		}
	}
	public static void adjustAngle180(ref Vector3 radianAngle, bool radian)
	{
		adjustAngle180(ref radianAngle.x, radian);
		adjustAngle180(ref radianAngle.y, radian);
		adjustAngle180(ref radianAngle.z, radian);
	}
	public static void adjustAngle360(ref float radianAngle, bool radian)
	{
		float pi = radian ? Mathf.PI : 180.0f;

		// 如果小于0,则循环加上PI * 2
		while (radianAngle < 0.0f)
		{
			radianAngle += pi * 2.0f;
		}
		// 如果大于PI * 2,则循环减去PI * 2
		while (radianAngle > pi * 2.0f)
		{
			radianAngle -= pi * 2.0f;
		}
	}
	public static void adjustAngle360(ref Vector3 radianAngle, bool radian)
	{
		adjustAngle360(ref radianAngle.x, radian);
		adjustAngle360(ref radianAngle.y, radian);
		adjustAngle360(ref radianAngle.z, radian);
	}
	// 求从z轴到指定向量的水平方向上的顺时针角度,角度范围是-MATH_PI 到 MATH_PI
	public static float getAngleFromVector(Vector3 vec)
	{
		vec.y = 0.0f;
		vec = Vector3.Normalize(vec);
		float angle = Mathf.Acos(vec.z);

		if (vec.x > 0.0f)
		{
			angle = -angle;
		}
		adjustAngle180(ref angle, true);

		return angle;
	}
	public static Vector3 rotateVector3(Vector3 vec, Matrix4x4 transMat3)
	{
		return transMat3 * vec;
	}
	// 使用一个四元数去旋转一个三维向量
	public static Vector3 rotateVector3(Vector3 vec, Quaternion transQuat)
	{
		return transQuat * vec;
	}
	// 求向量水平顺时针旋转一定角度后的向量,角度范围是-MATH_PI 到 MATH_PI
	public static Vector3 rotateVector3(Vector3 vec, float angle)
	{
		Vector3 temp = vec;
		temp.y = 0.0f;
		float tempLength = getLength(temp);
		float questAngle = getAngleFromVector(temp);
		questAngle += angle;
		adjustAngle180(ref questAngle, true);
		temp = getVectorFromAngle(questAngle) * tempLength;
		temp.y = vec.y;
		return temp;
	}
	// 求Z轴顺时针旋转一定角度后的向量,角度范围是-MATH_PI 到 MATH_PI
	public static Vector3 getVectorFromAngle(float angle)
	{
		Vector3 temp = new Vector3();
		temp.x = -Mathf.Sin(angle);
		temp.y = 0.0f;
		temp.z = Mathf.Cos(angle);
		return temp;
	}
	public static int pcm_db_count(short[] ptr, int size)
	{
		long v = 0;
		for (int i = 0; i < size; ++i)
		{
			v += Mathf.Abs(ptr[i]);
		}
		v /= size;

		int ndb = 0;
		if (v != 0)
		{
			ndb = (int)(20.0f * Mathf.Log10((float)v / 0xFFFF));
		}
		else
		{
			ndb = -96;
		}
		return ndb;
	}
	public static void getFrequencyZone(short[] pcmData, int dataCount, short[] frequencyList)
	{
		if (dataCount > mMaxFFTCount)
		{
			UnityUtility.logError("pcm data count is too many, data count : " + dataCount + ", max count : " + mMaxFFTCount);
			return;
		}
		for (int i = 0; i < dataCount; ++i)
		{
			mComplexList[i].mReal = pcmData[i];
			mComplexList[i].mImg = 0.0f;
		}
		fft(mComplexList, dataCount);
		for (int i = 0; i < dataCount; ++i)
		{
			frequencyList[i] = (short)Mathf.Sqrt(mComplexList[i].mReal * mComplexList[i].mReal + mComplexList[i].mImg * mComplexList[i].mImg);
		}
	}
	/*
	* FFT Algorithm
	* === Inputs ===
	* x : complex numbers
	* N : nodes of FFT. @N should be power of 2, that is 2^(*)
	* === Output ===
	* the @x contains the result of FFT algorithm, so the original data
	* in @x is destroyed, please store them before using FFT.
	*/
	protected static void fft(Complex[] x, int count)
	{
		int i, j, k;
		float sR, sI, uR, uI;
		Complex tempComplex = new Complex();

		/*
		* bit reversal sorting
		*/
		int l = count >> 1;
		j = l;
		int forCount = count - 2;
		for (i = 1; i <= forCount; ++i)
		{
			if (i < j)
			{
				tempComplex = x[j];
				x[j] = x[i];
				x[i] = tempComplex;
			}
			k = l;
			while (k <= j)
			{
				j -= k;
				k >>= 1;
			}
			j += k;
		}

		/*
		* For Loops
		*/
		int dftForCount = count - 1;
		int le = 1;
		int halfLe = 0;
		int M = (int)(Mathf.Log(count) / Mathf.Log(2));
		int ip = 0;
		for (l = 0; l < M; ++l)
		{
			// 在le左移1位之前保存值,也就是左移以后的值的一半
			halfLe = le;
			le <<= 1;
			uR = 1;
			uI = 0;
			sR = cos_tb[l];
			sI = -sin_tb[l];
			for (j = 0; j < halfLe; ++j)
			{
				/* loop for each sub DFT */
				for (i = j; i <= dftForCount; i += le)
				{
					/* loop for each butterfly */
					ip = i + halfLe;
					tempComplex.mReal = x[ip].mReal * uR - x[ip].mImg * uI;
					tempComplex.mImg = x[ip].mReal * uI + x[ip].mImg * uR;
					x[ip] = x[i] - tempComplex;
					x[i] += tempComplex;
				}
				tempComplex.mReal = uR;
				uR = tempComplex.mReal * sR - uI * sI;
				uI = tempComplex.mReal * sI + uI * sR;
			}
		}
	}

	/*
	* Inverse FFT Algorithm
	* === Inputs ===
	* x : complex numbers
	* N : nodes of FFT. @N should be power of 2, that is 2^(*)
	* === Output ===
	* the @x contains the result of FFT algorithm, so the original data
	* in @x is destroyed, please store them before using FFT.
	*/
	protected static void ifft(Complex[] x, int count)
	{
		int k = 0;

		for (k = 0; k <= count - 1; k++)
		{
			x[k].mImg = -x[k].mImg;
		}

		fft(x, count);

		float inverseCount = 1.0f / count;
		for (k = 0; k <= count - 1; k++)
		{
			x[k].mReal = x[k].mReal * inverseCount;
			x[k].mImg = -x[k].mImg * inverseCount;
		}
	}
	public static void secondsToMinutesSeconds(int seconds, ref int outMin, ref int outSec)
	{
		outMin = seconds / 60;
		outSec = seconds - outMin * 60;
	}
	public static void secondsToHoursMinutesSeconds(int seconds, ref int outHour, ref int outMin, ref int outSec)
	{
		outHour = seconds / 3600;
		outMin = (seconds - outHour * 3600) / 60;
		outSec = seconds - outHour * 3600 - outMin * 60;
	}
}