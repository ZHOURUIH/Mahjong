using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class MathUtility : StringUtility
{
	protected static float[] sin_tb = null;
	protected static float[] cos_tb = null;
	protected static int mMaxFFTCount = 1024 * 8;
	protected static Complex[] mComplexList;
	protected static void initParam()
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
	public static float KMHtoMS(float kmh) { return kmh / 3.6f; }       // km/h转m/s
	public static float MStoKMH(float ms) { return ms * 3.6f; }
	public static float MtoKM(float m) { return m / 1000.0f; }
	public static int sign(float value)
	{
		if (value < 0.0f)
		{
			return -1;
		}
		else if (value > 0.0f)
		{
			return 1;
		}
		else
		{
			return 0;
		}
	}
	public static float calculateFloat(string str)
	{
		// 判断字符串是否含有非法字符,也就是除数字,小数点,运算符以外的字符
		str = checkFloatString(str, "+-*/()");
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
				str = strReplace(str, curpos, curpos + strInBracket.Length + 2, floatStr);
				byte[] strchar = stringToBytes(str, Encoding.ASCII);
				if (isMinus)
				{
					// 如果括号中计算出来是负数,则将负号提取出来,将左边的第一个加减号改为相反的符号
					bool changeMark = false;
					for (int i = curpos - 1; i >= 0; --i)
					{
						// 找到第一个+号,则直接改为减号,然后退出遍历
						if (strchar[i] == '+')
						{
							strchar[i] = (byte)'-';
							str = bytesToString(strchar, Encoding.ASCII);
							changeMark = true;
							break;
						}
						// 找到第一个减号,如果减号的左边有数字,则直接改为+号
						// 如果减号的左边不是数字,则该减号是负号,将减号去掉,
						else if (strchar[i] == '-')
						{
							if (strchar[i - 1] >= '0' && strchar[i - 1] <= '9')
							{
								strchar[i] = (byte)'+';
								str = bytesToString(strchar, Encoding.ASCII);
							}
							else
							{
								str = strReplace(str, i, i + 1, "");
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
	// 得到大于等于value的第一个整数,只能是0或者整数
	public static int getForwardInt(float value)
	{
		if (value >= 0.0f)
		{
			int intValue = (int)(value);
			if (value - intValue > 0.0f)
			{
				return intValue + 1;
			}
			else
			{
				return (int)value;
			}
		}
		else
		{
			return (int)value;
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
		if (min == max)
		{
			return min;
		}
		else
		{
			max += 1;
		}
		return UnityEngine.Random.Range(min, max);
	}
	// 计算点p在平面on上的投影点,o为平面上一点,n为平面法线
	public static Vector3 getProjectionOnPlane(Vector3 o, Vector3 n, Vector3 p)
	{
		// o在np上的投影点即为交点
		return getProjectPoint(o, p, p + n);
	}
	// 该算法是网上找的,计算结果与上面的函数一致
	//public static Vector3 getProjectionOnPlane(Vector3 o, Vector3 n, Vector3 p)
	//{
	//	Vector3 projection = Vector3.zero;
	//	projection.x = (n.x * n.y * o.y + n.y * n.y * p.x - n.x * n.y * p.y + n.x * n.z * o.z + n.z * n.z * p.x - n.x * n.z* p.z + n.x * n.x * o.x) / (n.x * n.x + n.y * n.y + n.z * n.z);
	//	projection.y = (n.y * n.z * o.z + n.z * n.z * p.y - n.y * n.z * p.z + n.y * n.x * o.x + n.x * n.x * p.y - n.x * n.y* p.x + n.y * n.y * o.y) / (n.x * n.x + n.y * n.y + n.z * n.z);
	//	projection.z = (n.x * o.x * n.z + n.x * n.x * p.z - n.x * p.x * n.z + n.y * o.y * n.z + n.y * n.y * p.z - n.y * p.y* n.z + n.z * n.z * o.z) / (n.x * n.x + n.y * n.y + n.z * n.z);
	//	return projection;
	//}
	public static Vector3 getDirectionFromDegreeYawPitch(float yaw, float pitch)
	{
		yaw *= Mathf.Deg2Rad;
		pitch *= Mathf.Deg2Rad;
		return getDirectionFromRadianYawPitch(yaw, pitch);
	}
	public static Vector3 getDirectionFromRadianYawPitch(float yaw, float pitch)
	{
		// 如果pitch为90°或者-90°,则直接返回向量,此时无论航向角为多少,向量都是竖直向上或者竖直向下
		if (isFloatZero(pitch - Mathf.PI / 2.0f))
		{
			return Vector3.down;
		}
		else if (isFloatZero(pitch + Mathf.PI / 2.0f))
		{
			return Vector3.up;
		}
		else
		{
			// 在unity的坐标系中航向角需要取反
			yaw = -yaw;
			Vector3 dir = new Vector3();
			dir.z = Mathf.Cos(yaw);
			dir.x = -Mathf.Sin(yaw);
			dir.y = -Mathf.Tan(pitch);
			dir = normalize(dir);
			return dir;
		}
	}
	public static float getVectorYaw(Vector3 vec)
	{
		vec = normalize(vec);
		float fYaw = 0.0f;
		// 计算航向角,航向角是向量与在X-Z平面上的投影与Z轴正方向的夹角,从上往下看是顺时针为正,逆时针为负
		Vector3 projectionXZ = new Vector3(vec.x, 0.0f, vec.z);
		float len = getLength(projectionXZ);
		// 如果投影的长度为0,则表示俯仰角为90°或者-90°,航向角为0
		if (isFloatZero(len))
		{
			fYaw = 0.0f;
		}
		else
		{
			projectionXZ = normalize(projectionXZ);
			fYaw = Mathf.Acos(Vector3.Dot(projectionXZ, Vector3.forward));
			// 判断航向角的正负,如果x为正,则航向角为负,如果x为,则航向角为正
			if (vec.x > 0.0f)
			{
				fYaw = -fYaw;
			}
		}
		// 在unity的坐标系中航向角需要取反
		fYaw = -fYaw;
		return fYaw;
	}
	// 计算向量的俯仰角,朝上时俯仰角小于0,朝下时俯仰角大于0
	public static float getVectorPitch(Vector3 vec)
	{
		vec = normalize(vec);
		return -Mathf.Asin(vec.y);
	}
	// 顺时针旋转为正,逆时针为负
	public static float getAngleFromVectorToVector(Vector2 from, Vector2 to)
	{
		Vector3 from3 = normalize(new Vector3(from.x, 0.0f, from.y));
		Vector3 to3 = normalize(new Vector3(to.x, 0.0f, to.y));
		float angle = getAngleBetweenVector(from3, to3);
		Vector3 crossVec = Vector3.Cross(from3, to3);
		if (crossVec.y < 0.0f)
		{
			angle = -angle;
		}
		return angle;
	}
	// baseY为true表示将点当成X-Z平面上的点,忽略Y值,false表示将点当成X-Y平面的点
	public static float getAngleFromVectorToVector(Vector3 from, Vector3 to, bool baseY)
	{
		if(baseY)
		{
			from.y = 0.0f;
			to.y = 0.0f;
		}
		float angle = getAngleBetweenVector(from, to);
		Vector3 crossVec = Vector3.Cross(from, to);
		if(baseY)
		{
			if (crossVec.y < 0.0f)
			{
				angle = -angle;
			}
		}
		else
		{
			if (crossVec.z > 0.0f)
			{
				angle = -angle;
			}
		}
		return angle;
	}
	public static Vector3 getDegreeEulerFromDirection(Vector3 dir)
	{
		float yaw = 0.0f;
		float pitch = 0.0f;
		getDegreeYawPitchFromDirection(dir, ref yaw, ref pitch);
		return new Vector3(pitch, yaw, 0.0f);
	}
	public static void getDegreeYawPitchFromDirection(Vector3 dir, ref float fYaw, ref float fPitch)
	{
		getRadianYawPitchFromDirection(dir, ref fYaw, ref fPitch);
		fYaw *= Mathf.Rad2Deg;
		fPitch *= Mathf.Rad2Deg;
	}
	// fYaw是-PI到PI之间
	public static void getRadianYawPitchFromDirection(Vector3 dir, ref float fYaw, ref float fPitch)
	{
		dir = normalize(dir);
		// 首先计算俯仰角,俯仰角是向量与X-Z平面的夹角,在上面为负,在下面为正
		fPitch = getVectorPitch(dir);
		fYaw = getVectorYaw(dir);
	}
	// 给定一段圆弧,以及圆弧圆心角的百分比,计算对应的圆弧上的一个点以及该点的切线方向
	public static void getPosOnArc(Vector3 circleCenter, Vector3 startArcPos, Vector3 endArcPos, float anglePercent, ref Vector3 pos, ref Vector3 tangencyDir)
	{
		float radius = getLength(startArcPos - circleCenter);
		Vector3 relativeStart = startArcPos - circleCenter;
		Vector3 relativeEnd = endArcPos - circleCenter;
		clamp(ref anglePercent, 0.0f, 1.0f);
		// 首先判断从起始半径线段到终止半径线段的角度的正负
		float angleBetween = getAngleFromVectorToVector(new Vector2(relativeStart.x, relativeStart.z), new Vector2(relativeEnd.x, relativeEnd.z));
		if (isFloatZero(angleBetween))
		{
			pos = normalize(relativeStart) * radius;
			tangencyDir = normalize(rotateVector3(-pos, Mathf.PI / 2.0f));
		}
		// 根据夹角的正负,判断应该顺时针还是逆时针旋转起始半径线段
		else
		{
			pos = normalize(rotateVector3(relativeStart, anglePercent * angleBetween)) * radius;
			// 计算切线,如果顺时针计算出的切线与从起始点到终止点所成的角度大于90度,则使切线反向
			tangencyDir = normalize(rotateVector3(-pos, Mathf.PI / 2.0f));
			Vector3 posToEnd = relativeEnd - pos;
			if (Mathf.Abs(getAngleFromVectorToVector(new Vector2(tangencyDir.x, tangencyDir.z), new Vector2(posToEnd.x, posToEnd.z))) > Mathf.PI / 2.0f)
			{
				tangencyDir = -tangencyDir;
			}
		}
		pos += circleCenter;
	}
	// 根据入射角和法线得到反射角
	public static Vector3 getReflection(Vector3 inRay, Vector3 normal)
	{
		inRay = normalize(inRay);
		normal = normalize(normal);
		Vector3 outRay = inRay - 2 * (Vector3.Dot(inRay, normal)) * normal;
		return outRay;
	}
	public static bool isVectorEqual(Vector2 vec0, Vector2 vec1)
	{
		return isVectorZero(vec0 - vec1);
	}
	public static bool isVectorEqual(Vector3 vec0, Vector3 vec1)
	{
		return isVectorZero(vec0 - vec1);
	}
	public static bool isVectorZero(Vector2 vec)
	{
		return isFloatZero(vec.x) && isFloatZero(vec.y);
	}
	public static bool isVectorZero(Vector3 vec)
	{
		return isFloatZero(vec.x) && isFloatZero(vec.y) && isFloatZero(vec.z);
	}
	public static float getLength(Vector4 vec)
	{
		return Mathf.Sqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z + vec.w * vec.w);
	}
	public static float getSquaredLength(Vector4 vec)
	{
		return vec.x * vec.x + vec.y * vec.y + vec.z * vec.z + vec.w * vec.w;
	}
	public static float getLength(Vector3 vec)
	{
		return Mathf.Sqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z);
	}
	public static float getSquaredLength(Vector3 vec)
	{
		return vec.x * vec.x + vec.y * vec.y + vec.z * vec.z;
	}
	public static float getLength(Vector2 vec)
	{
		return Mathf.Sqrt(vec.x * vec.x + vec.y * vec.y);
	}
	public static float getSquaredLength(Vector2 vec)
	{
		return vec.x * vec.x + vec.y * vec.y;
	}
	public static Vector3 getMatrixScale(Matrix4x4 mat)
	{
		Vector3 vec0 = new Vector3(mat.m00, mat.m01, mat.m02);
		Vector3 vec1 = new Vector3(mat.m10, mat.m11, mat.m12);
		Vector3 vec2 = new Vector3(mat.m20, mat.m21, mat.m22);
		return new Vector3(getLength(vec0), getLength(vec1), getLength(vec2));
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
	// 计算点到线的距离
	public static float getDistanceToLine(Vector3 point, Vector3 start, Vector3 end)
	{
		return getLength(point - getProjectPoint(point, start, end));
	}
	// 计算点在线上的投影
	public static Vector3 getProjectPoint(Vector3 point, Vector3 start, Vector3 end)
	{
		// 计算出点到线一段的向量在线上的投影
		Vector3 projectOnLine = getProjection(point - start, end - start);
		// 点到线垂线的交点
		return start + projectOnLine;
	}
	// 点在线上的投影是否在线段范围内
	public static bool isPointProjectOnLine(Vector3 point, Vector3 start, Vector3 end)
	{
		Vector3 point0 = start;
		Vector3 point1 = end;
		if (getSquaredLength(point - start) > getSquaredLength(point - end))
		{
			point0 = end;
			point1 = start;
		}
		return getAngleBetweenVector(point - point0, point1 - point0) <= Mathf.PI / 2.0f;
	}
	// 计算一个向量在另一个向量上的投影
	public static Vector3 getProjection(Vector3 v1, Vector3 v2)
	{
		return normalize(v2) * getLength(v1) * Mathf.Cos(getAngleBetweenVector(v1, v2));
	}
	public static Vector3 normalize(Vector3 vec3)
	{
		float length = getLength(vec3);
		if (isFloatZero(length))
		{
			return Vector3.zero;
		}
		if (isFloatEqual(length, 1.0f))
		{
			return vec3;
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
	public static float inverseLerp(float a, float b, float value)
	{
		return (value - a) / (b - a);
	}
	public static float inverseLerp(Vector2 a, Vector2 b, Vector2 value)
	{
		return getLength(value - a) / getLength(b - a);
	}
	public static float inverseLerp(Vector3 a, Vector3 b, Vector3 value)
	{
		return getLength(value - a) / getLength(b - a);
	}
	public static float lerpSimple(float start, float end, float t)
	{
		return start + (end - start) * t;
	}
	public static float lerp(float start, float end, float t, float minAbsDelta = 0.0f)
	{
		clamp(ref t, 0.0f, 1.0f);
		float value = start + (end - start) * t;
		// 如果值已经在end的一定范围内了,则直接设置为end
		if (Mathf.Abs(value - end) <= minAbsDelta)
		{
			value = end;
		}
		return value;
	}
	public static Vector3 lerp(Vector3 start, Vector3 end, float t, float minAbsDelta = 0.0f)
	{
		clamp(ref t, 0.0f, 1.0f);
		Vector3 value = start + (end - start) * t;
		// 如果值已经在end的一定范围内了,则直接设置为end
		if (Mathf.Abs(getSquaredLength(value - end)) <= minAbsDelta * minAbsDelta)
		{
			value = end;
		}
		return value;
	}
	public static Color lerp(Color start, Color end, float t, float minAbsDelta = 0.0f)
	{
		clamp(ref t, 0.0f, 1.0f);
		Color value = start + (end - start) * t;
		// 如果值已经在end的一定范围内了,则直接设置为end
		Color curDelta = value - end;
		if (Mathf.Abs(getSquaredLength(new Vector4(curDelta.r, curDelta.g, curDelta.b, curDelta.a))) <= minAbsDelta * minAbsDelta)
		{
			value = end;
		}
		return value;
	}
	public static void perfectRotationDelta(ref float start, ref float target)
	{
		// 先都调整到-180~180的范围
		adjustAngle180(ref start);
		adjustAngle180(ref target);
		float dirDelta = target - start;
		// 如果目标方向与当前方向的差值超过180,则转换到0~360再计算
		if (Mathf.Abs(dirDelta) > 180.0f)
		{
			adjustAngle360(ref start);
			adjustAngle360(ref target);
		}
	}
	public static void perfectRotationDelta(ref Vector3 start, ref Vector3 target)
	{
		perfectRotationDelta(ref start.x, ref target.x);
		perfectRotationDelta(ref start.y, ref target.y);
		perfectRotationDelta(ref start.z, ref target.z);
	}
	public static void clamp(ref float value, float min, float max)
	{
		if (min > max || isFloatEqual(min, max))
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
	public static void clamp(ref int value, int min, int max)
	{
		if (min > max)
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
	public static void clampMin(ref int value, int min)
	{
		if (value < min)
		{
			value = min;
		}
	}
	public static void clampMax(ref int value, int max)
	{
		if (value > max)
		{
			value = max;
		}
	}
	public static void clampMin(ref float value, float min)
	{
		if (value < min)
		{
			value = min;
		}
	}
	public static void clampMax(ref float value, float max)
	{
		if (value > max)
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
		return isFloatZero(value1 - value2, precision);
	}
	public static void clampValue(ref int value, int min, int max, int cycle)
	{
		while (value < min)
		{
			value += cycle;
		}
		while (value > max)
		{
			value -= cycle;
		}
	}
	public static void clampValue(ref float value, float min, float max, float cycle)
	{
		while (value < min)
		{
			value += cycle;
		}
		while (value > max)
		{
			value -= cycle;
		}
	}
	// fixedRangeOrder表示是否范围是从range0到range1,如果range0大于range1,则返回false
	public static bool isInRange(float value, float range0, float range1, bool fixedRangeOrder = false)
	{
		if (fixedRangeOrder)
		{
			return value >= range0 && value <= range1;
		}
		else
		{
			return value >= getMin(range0, range1) && value <= getMax(range0, range1);
		}
	}
	public static bool isInRange(Vector3 value, Vector3 point0, Vector3 point1, bool ignoreY = true)
	{
		return isInRange(value.x, point0.x, point1.x) && isInRange(value.z, point0.z, point1.z) &&
			(ignoreY || isInRange(value.y, point0.y, point1.y));
	}
	public static void swap<T>(ref T value0, ref T value1)
	{
		T temp = value0;
		value0 = value1;
		value1 = temp;
	}
	public static void adjustRadian180(ref float radianAngle)
	{
		clampValue(ref radianAngle, -Mathf.PI, Mathf.PI, Mathf.PI * 2.0f);
	}
	public static void adjustAngle180(ref float radianAngle)
	{
		float degreePI = Mathf.PI * Mathf.Rad2Deg;
		clampValue(ref radianAngle, -degreePI, degreePI, degreePI * 2.0f);
	}
	public static void adjustRadian180(ref Vector3 radianAngle)
	{
		adjustRadian180(ref radianAngle.x);
		adjustRadian180(ref radianAngle.y);
		adjustRadian180(ref radianAngle.z);
	}
	public static void adjustAngle180(ref Vector3 radianAngle)
	{
		adjustAngle180(ref radianAngle.x);
		adjustAngle180(ref radianAngle.y);
		adjustAngle180(ref radianAngle.z);
	}
	public static void adjustRadian360(ref float radianAngle)
	{
		clampValue(ref radianAngle, 0.0f, Mathf.PI * 2.0f, Mathf.PI * 2.0f);
	}
	public static void adjustAngle360(ref float radianAngle)
	{
		float degreePI = Mathf.PI * Mathf.Rad2Deg;
		clampValue(ref radianAngle, 0.0f, degreePI * 2.0f, degreePI * 2.0f);
	}
	public static void adjustAngle360(ref Vector3 radianAngle)
	{
		adjustAngle360(ref radianAngle.x);
		adjustAngle360(ref radianAngle.y);
		adjustAngle360(ref radianAngle.z);
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
		adjustRadian180(ref angle);
		// 在unity的坐标系中航向角需要取反
		angle = -angle;
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
		adjustRadian180(ref questAngle);
		temp = getVectorFromAngle(questAngle) * tempLength;
		temp.y = vec.y;
		return temp;
	}
	// 求Z轴顺时针旋转一定角度后的向量,角度范围是-MATH_PI 到 MATH_PI
	public static Vector3 getVectorFromAngle(float angle)
	{
		adjustRadian180(ref angle);
		Vector3 temp = new Vector3();
		temp.x = -Mathf.Sin(angle);
		temp.y = 0.0f;
		temp.z = Mathf.Cos(angle);
		// 在unity坐标系中x轴需要取反
		temp.x = -temp.x;
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
		if (mComplexList == null)
		{
			initParam();
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
		if (sin_tb == null)
		{
			initParam();
		}
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
	//将时间转化成时间戳
	public static long convertDateTimeUnixTime(System.DateTime dateTime)
	{
		System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
		long timeStamp = (long)(dateTime - startTime).TotalSeconds; // 相差秒数
		return timeStamp;
	}
	//将时间戳转化成时间
	public static DateTime ConvertUnixTimeDateTime(long unixTimeStamp)
	{
		System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
		DateTime dt = startTime.AddSeconds(unixTimeStamp);
		return dt;
	}
	// 递归计算贝塞尔曲线的点
	public static Vector3 getBezier(List<Vector3> points, bool loop, float t)
	{
		int pointCount = points.Count;
		if (pointCount == 2)
		{
			return lerp(points[0], points[1], t);
		}
		int tempCount = loop ? pointCount : pointCount - 1;
		Vector3[] temp = new Vector3[tempCount];
		for (int i = 0; i < tempCount; ++i)
		{
			temp[i] = lerp(points[i], points[(i + 1) % pointCount], t);
		}
		return getBezier(temp, loop, t);
	}
	// 递归计算贝塞尔曲线的点
	public static Vector3 getBezier(Vector3[] points, bool loop, float t)
	{
		int pointCount = points.Length;
		if (pointCount == 2)
		{
			return lerp(points[0], points[1], t);
		}
		int tempCount = loop ? pointCount : pointCount - 1;
		Vector3[] temp = new Vector3[tempCount];
		for (int i = 0; i < tempCount; ++i)
		{
			temp[i] = lerp(points[i], points[(i + 1) % pointCount], t);
		}
		return getBezier(temp, loop, t);
	}
	public static List<Vector3> getBezierPoints(List<Vector3> points, bool loop, int bezierDetail = 20)
	{
		if (points.Count == 1)
		{
			return new List<Vector3>(points);
		}
		List<Vector3> bezierPoints = new List<Vector3>();
		for (int i = 0; i < bezierDetail; ++i)
		{
			bezierPoints.Add(getBezier(points, loop, i / (float)(bezierDetail - 1)));
		}
		return bezierPoints;
	}
	public static Vector3[] getBezierPoints(Vector3[] points, bool loop, int bezierDetail = 20)
	{
		if(points.Length == 1)
		{
			return new Vector3[1] { points[0] };
		}
		Vector3[] bezierPoints = new Vector3[bezierDetail];
		for (int i = 0; i < bezierDetail; ++i)
		{
			bezierPoints[i] = getBezier(points, loop, i / (float)(bezierDetail - 1));
		}
		return bezierPoints;
	}
	// 得到经过所有点的平滑曲线的点列表,detail是曲线平滑度,越大越平滑,scale是曲线接近折线的程度,越小越接近于折线
	public static Vector3[] getCurvePoints(Vector3[] originPoint, bool loop, int detail = 10, float scale = 0.6f)
	{
		if(originPoint.Length == 1)
		{
			return new Vector3[1] { originPoint[0]};
		}
		int originCount = originPoint.Length;
		int middleCount = loop ? originCount : originCount - 1;
		Vector3[] midpoints = new Vector3[middleCount];
		// 生成中点       
		for (int i = 0; i < middleCount; ++i)
		{
			midpoints[i] = (originPoint[i] + originPoint[(i + 1) % originCount]) / 2.0f;
		}

		// 平移中点,计算每个顶点的两个控制点
		Vector3[] extrapoints = new Vector3[2 * originCount];
		for (int i = 0; i < originCount; ++i)
		{
			if (!loop)
			{
				if (i == 0)
				{
					extrapoints[0] = originPoint[0];
					extrapoints[1] = originPoint[0];
				}
				else if (i == originCount - 1)
				{
					extrapoints[i * 2 + 0] = originPoint[originCount - 1];
					extrapoints[i * 2 + 1] = originPoint[originCount - 1];
				}
				else
				{

					int nexti = i + 1;
					int backi = i - 1;
					Vector3 midinmid = (midpoints[i] + midpoints[backi]) / 2.0f;
					Vector3 offset = originPoint[i] - midinmid;
					//朝 originPoint[i]方向收缩
					extrapoints[2 * i + 0] = originPoint[i] + (midpoints[backi] + offset - originPoint[i]) * scale;
					//朝 originPoint[i]方向收缩
					extrapoints[2 * i + 1] = originPoint[i] + (midpoints[i] + offset - originPoint[i]) * scale;

				}
			}
			else
			{
				int nexti = (i + 1) % originCount;
				int backi = (i + originCount - 1) % originCount;
				Vector3 midinmid = (midpoints[i] + midpoints[backi]) / 2.0f;
				Vector3 offset = originPoint[i] - midinmid;
				//朝 originPoint[i]方向收缩
				extrapoints[2 * i + 0] = originPoint[i] + (midpoints[backi] + offset - originPoint[i]) * scale;
				//朝 originPoint[i]方向收缩
				extrapoints[2 * i + 1] = originPoint[i] + (midpoints[i] + offset - originPoint[i]) * scale;
			}
		}

		int bezierCount = loop ? originCount : originCount - 1;
		Vector3[] curvePoint = new Vector3[bezierCount * detail];
		Vector3[] controlPoint = new Vector3[4];
		float step = 1 / (float)(detail - 1);
		// 生成4控制点，产生贝塞尔曲线
		for (int i = 0; i < bezierCount; ++i)
		{
			controlPoint[0] = originPoint[i];
			controlPoint[1] = extrapoints[2 * i + 1];
			controlPoint[2] = extrapoints[2 * (i + 1)];
			controlPoint[3] = originPoint[(i + 1) % originCount];
			for (int j = 0; j < detail; ++j)
			{
				curvePoint[i * detail + j] = getBezier(controlPoint, false, j * step);
			}
		}
		return curvePoint;
	}
	// 得到经过所有点的平滑曲线的点列表,detail是曲线平滑度,越大越平滑,scale是曲线接近折线的程度,越小越接近于折线
	public static List<Vector3> getCurvePoints(List<Vector3> originPoint, bool loop, int detail = 10, float scale = 0.6f)
	{
		if(originPoint.Count == 1)
		{
			return new List<Vector3>(originPoint);
		}
		int originCount = originPoint.Count;
		int middleCount = loop ? originCount : originCount - 1;
		Vector3[] midpoints = new Vector3[middleCount];
		// 生成中点       
		for (int i = 0; i < middleCount; ++i)
		{
			midpoints[i] = (originPoint[i] + originPoint[(i + 1) % originCount]) / 2.0f;
		}

		// 平移中点,计算每个顶点的两个控制点
		Vector3[] extrapoints = new Vector3[2 * originCount];
		for (int i = 0; i < originCount; ++i)
		{
			if (!loop)
			{
				if (i == 0)
				{
					extrapoints[0] = originPoint[0];
					extrapoints[1] = originPoint[0];
				}
				else if (i == originCount - 1)
				{
					extrapoints[i * 2 + 0] = originPoint[originCount - 1];
					extrapoints[i * 2 + 1] = originPoint[originCount - 1];
				}
				else
				{

					int nexti = i + 1;
					int backi = i - 1;
					Vector3 midinmid = (midpoints[i] + midpoints[backi]) / 2.0f;
					Vector3 offset = originPoint[i] - midinmid;
					//朝 originPoint[i]方向收缩
					extrapoints[2 * i + 0] = originPoint[i] + (midpoints[backi] + offset - originPoint[i]) * scale;
					//朝 originPoint[i]方向收缩
					extrapoints[2 * i + 1] = originPoint[i] + (midpoints[i] + offset - originPoint[i]) * scale;

				}
			}
			else
			{
				int nexti = (i + 1) % originCount;
				int backi = (i + originCount - 1) % originCount;
				Vector3 midinmid = (midpoints[i] + midpoints[backi]) / 2.0f;
				Vector3 offset = originPoint[i] - midinmid;
				//朝 originPoint[i]方向收缩
				extrapoints[2 * i + 0] = originPoint[i] + (midpoints[backi] + offset - originPoint[i]) * scale;
				//朝 originPoint[i]方向收缩
				extrapoints[2 * i + 1] = originPoint[i] + (midpoints[i] + offset - originPoint[i]) * scale;
			}
		}

		int bezierCount = loop ? originCount : originCount - 1;
		List<Vector3> curvePoint = new List<Vector3>();
		Vector3[] controlPoint = new Vector3[4];
		float step = 1 / (float)(detail - 1);
		// 生成4控制点，产生贝塞尔曲线
		for (int i = 0; i < bezierCount; ++i)
		{
			controlPoint[0] = originPoint[i];
			controlPoint[1] = extrapoints[2 * i + 1];
			controlPoint[2] = extrapoints[2 * (i + 1)];
			controlPoint[3] = originPoint[(i + 1) % originCount];
			for (int j = 0; j < detail; ++j)
			{
				curvePoint.Add(getBezier(controlPoint, false, j * step));
			}
		}
		return curvePoint;
	}

	public static uint generateGUID()
	{
		// 获得当前时间
		TimeSpan timeForm19700101 = DateTime.Now - DateTime.Parse("1970-1-1");
		ulong ulongMS = (ulong)timeForm19700101.TotalMilliseconds;
		uint halfIntMS = (uint)(ulongMS % 0x7FFFFFFF);
		// 获取当前系统信息生成的随机数
		byte[] randomNumber = new byte[4]; 
		RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
		rng.GetBytes(randomNumber);
		uint uintRand = bytesToUInt(randomNumber);
		uint halfIntRand = uintRand % 0x7FFFFFFF;
		return halfIntMS + halfIntRand;
	}
	protected static float HueToRGB(float v1, float v2, float vH)
	{
		if (vH < 0.0f)
		{
			vH += 1.0f;
		}
		if (vH > 1.0f)
		{
			vH -= 1.0f;
		}
		if (6.0f * vH < 1.0f)
		{
			return v1 + (v2 - v1) * 6.0f * vH;
		}
		else if (2.0f * vH < 1.0f)
		{
			return v2;
		}
		else if (3.0f * vH < 2.0f)
		{
			return v1 + (v2 - v1) * (0.667f - vH) * 6.0f;
		}
		else
		{
			return v1;
		}
	}

	// rgb转换为色相(H),饱和度(S),亮度(L)
	// HSL和RGB的范围都是0-1
	public static Vector3 RGBtoHSL(Vector3 rgb)
	{
		float minRGB = getMin(getMin(rgb.x, rgb.y), rgb.z);
		float maxRGB = getMax(getMax(rgb.x, rgb.y), rgb.z);
		float delta = maxRGB - minRGB;

		float H = 0.0f;
		float S = 0.0f;
		float L = (maxRGB + minRGB) * 0.5f;
		// 如果三个分量的最大和最小相等,则说明该颜色是灰色的,灰色的色相和饱和度都为0
		if (delta > 0.0f)                                //Chromatic data...
		{
			if (L < 0.5f)
			{
				S = delta / (maxRGB + minRGB);
			}
			else
			{
				S = delta / (2.0f - maxRGB - minRGB);
			}

			float inverseDelta = 1.0f / delta;
			float halfDelta = delta * 0.5f;
			float delR = ((maxRGB - rgb.x) * 0.167f + halfDelta) * inverseDelta;
			float delG = ((maxRGB - rgb.y) * 0.167f + halfDelta) * inverseDelta;
			float delB = ((maxRGB - rgb.z) * 0.167f + halfDelta) * inverseDelta;

			if (rgb.x == maxRGB)
			{
				H = delB - delG;
			}
			else if (rgb.y == maxRGB)
			{
				H = 0.33f + delR - delB;
			}
			else if (rgb.z == maxRGB)
			{
				H = 0.667f + delG - delR;
			}

			if (H < 0.0f)
			{
				H += 1.0f;
			}
			else if (H > 1.0f)
			{
				H -= 1.0f;
			}
		}
		return new Vector3(H, S, L);
	}

	// 色相(H),饱和度(S),亮度(L),转换为rgb
	// HSL和RGB的范围都是0-1
	public static Vector3 HSLtoRGB(Vector3 hsl)
	{
		Vector3 rgb = Vector3.zero;
		float H = hsl.x;
		float S = hsl.y;
		float L = hsl.z;
		if (S == 0.0)                       //HSL from 0 to 1
		{
			rgb.x = L;              //RGB results from 0 to 255
			rgb.y = L;
			rgb.z = L;
		}
		else
		{
			float var2;
			if (L < 0.5f)
			{
				var2 = L * (1.0f + S);
			}
			else
			{
				var2 = L + S - (S * L);
			}

			float var1 = 2.0f * L - var2;
			rgb.x = HueToRGB(var1, var2, H + 0.33f);
			rgb.y = HueToRGB(var1, var2, H);
			rgb.z = HueToRGB(var1, var2, H - 0.33f);
		}
		return rgb;
	}
}