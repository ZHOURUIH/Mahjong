using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// 复数
public class Complex
{
	public float mReal;
	public float mImg;
	public Complex()
	{
		mReal = 0.0f;
		mImg = 0.0f;
	}
	public Complex(float realPart, float imgPart)
	{
		mReal = realPart;
		mImg = imgPart;
	}
	public static Complex operator+(Complex c0, Complex c1)
	{
		Complex temp = new Complex(c1.mReal + c0.mReal, c1.mImg + c0.mImg);
		return temp;
	}
	public static Complex operator -(Complex c0, Complex c1)
	{
		Complex temp = new Complex(c1.mReal - c0.mReal, c1.mImg - c0.mImg);
		return temp;
	}
};