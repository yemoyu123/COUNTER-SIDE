using System;
using Cs.Math;

namespace NKM;

public struct NKMMatrix
{
	public float _11;

	public float _12;

	public float _13;

	public float _14;

	public float _21;

	public float _22;

	public float _23;

	public float _24;

	public float _31;

	public float _32;

	public float _33;

	public float _34;

	public float _41;

	public float _42;

	public float _43;

	public float _44;

	public static float ToRadian(float degree)
	{
		return degree * ((float)Math.PI / 180f);
	}

	public static float ToDegree(float radian)
	{
		return radian * (180f / (float)Math.PI);
	}

	public static NKMMatrix operator -(NKMMatrix a)
	{
		return new NKMMatrix
		{
			_11 = 0f - a._11,
			_12 = 0f - a._12,
			_13 = 0f - a._13,
			_14 = 0f - a._14,
			_21 = 0f - a._21,
			_22 = 0f - a._22,
			_23 = 0f - a._23,
			_24 = 0f - a._24,
			_31 = 0f - a._31,
			_32 = 0f - a._32,
			_33 = 0f - a._33,
			_34 = 0f - a._34,
			_41 = 0f - a._41,
			_42 = 0f - a._42,
			_43 = 0f - a._43,
			_44 = 0f - a._44
		};
	}

	public static NKMMatrix operator +(NKMMatrix a, NKMMatrix b)
	{
		return new NKMMatrix
		{
			_11 = a._11 + b._11,
			_12 = a._12 + b._12,
			_13 = a._13 + b._13,
			_14 = a._14 + b._14,
			_21 = a._21 + b._21,
			_22 = a._22 + b._22,
			_23 = a._23 + b._23,
			_24 = a._24 + b._24,
			_31 = a._31 + b._31,
			_32 = a._32 + b._32,
			_33 = a._33 + b._33,
			_34 = a._34 + b._34,
			_41 = a._41 + b._41,
			_42 = a._42 + b._42,
			_43 = a._43 + b._43,
			_44 = a._44 + b._44
		};
	}

	public static NKMMatrix operator -(NKMMatrix a, NKMMatrix b)
	{
		return new NKMMatrix
		{
			_11 = a._11 - b._11,
			_12 = a._12 - b._12,
			_13 = a._13 - b._13,
			_14 = a._14 - b._14,
			_21 = a._21 - b._21,
			_22 = a._22 - b._22,
			_23 = a._23 - b._23,
			_24 = a._24 - b._24,
			_31 = a._31 - b._31,
			_32 = a._32 - b._32,
			_33 = a._33 - b._33,
			_34 = a._34 - b._34,
			_41 = a._41 - b._41,
			_42 = a._42 - b._42,
			_43 = a._43 - b._43,
			_44 = a._44 - b._44
		};
	}

	public static NKMMatrix operator *(NKMMatrix a, float v)
	{
		return new NKMMatrix
		{
			_11 = a._11 * v,
			_12 = a._12 * v,
			_13 = a._13 * v,
			_14 = a._14 * v,
			_21 = a._21 * v,
			_22 = a._22 * v,
			_23 = a._23 * v,
			_24 = a._24 * v,
			_31 = a._31 * v,
			_32 = a._32 * v,
			_33 = a._33 * v,
			_34 = a._34 * v,
			_41 = a._41 * v,
			_42 = a._42 * v,
			_43 = a._43 * v,
			_44 = a._44 * v
		};
	}

	public static NKMMatrix operator /(NKMMatrix a, float v)
	{
		return new NKMMatrix
		{
			_11 = a._11 / v,
			_12 = a._12 / v,
			_13 = a._13 / v,
			_14 = a._14 / v,
			_21 = a._21 / v,
			_22 = a._22 / v,
			_23 = a._23 / v,
			_24 = a._24 / v,
			_31 = a._31 / v,
			_32 = a._32 / v,
			_33 = a._33 / v,
			_34 = a._34 / v,
			_41 = a._41 / v,
			_42 = a._42 / v,
			_43 = a._43 / v,
			_44 = a._44 / v
		};
	}

	public static NKMMatrix MatrixRotationYawPitchRoll(out NKMMatrix pout, float yaw, float pitch, float roll)
	{
		MatrixIdentity(out var pOut);
		MatrixRotationZ(out var pOut2, roll);
		MatrixMultiply(out var pOut3, pOut, pOut2);
		MatrixRotationX(out pOut2, pitch);
		MatrixMultiply(out var pOut4, pOut3, pOut2);
		MatrixRotationY(out pOut2, yaw);
		MatrixMultiply(out pout, pOut4, pOut2);
		return pout;
	}

	public static NKMMatrix MatrixIdentity(out NKMMatrix pOut)
	{
		pOut._11 = 1f;
		pOut._12 = 0f;
		pOut._13 = 0f;
		pOut._14 = 0f;
		pOut._21 = 0f;
		pOut._22 = 1f;
		pOut._23 = 0f;
		pOut._24 = 0f;
		pOut._31 = 0f;
		pOut._32 = 0f;
		pOut._33 = 1f;
		pOut._34 = 0f;
		pOut._41 = 0f;
		pOut._42 = 0f;
		pOut._43 = 0f;
		pOut._44 = 1f;
		return pOut;
	}

	public static NKMMatrix MatrixRotationX(out NKMMatrix pOut, float Angle)
	{
		MatrixIdentity(out pOut);
		float num = (float)Math.Cos(Angle);
		float num2 = (float)Math.Sin(Angle);
		pOut._22 = num;
		pOut._33 = num;
		pOut._23 = num2;
		pOut._32 = 0f - num2;
		return pOut;
	}

	public static NKMMatrix MatrixRotationY(out NKMMatrix pOut, float Angle)
	{
		MatrixIdentity(out pOut);
		float num = (float)Math.Cos(Angle);
		float num2 = (float)Math.Sin(Angle);
		pOut._11 = num;
		pOut._33 = num;
		pOut._13 = 0f - num2;
		pOut._31 = num2;
		return pOut;
	}

	public static NKMMatrix MatrixRotationZ(out NKMMatrix pOut, float Angle)
	{
		MatrixIdentity(out pOut);
		float num = (float)Math.Cos(Angle);
		float num2 = (float)Math.Sin(Angle);
		pOut._11 = num;
		pOut._22 = num;
		pOut._12 = num2;
		pOut._21 = 0f - num2;
		return pOut;
	}

	public static NKMMatrix MatrixMultiply(out NKMMatrix pOut, NKMMatrix pM1, NKMMatrix pM2)
	{
		MatrixIdentity(out pOut);
		pOut._11 = pM1._11 * pM2._11 + pM1._12 * pM2._21 + pM1._13 * pM2._31 + pM1._14 * pM2._41;
		pOut._12 = pM1._11 * pM2._12 + pM1._12 * pM2._22 + pM1._13 * pM2._32 + pM1._14 * pM2._42;
		pOut._13 = pM1._11 * pM2._13 + pM1._12 * pM2._23 + pM1._13 * pM2._33 + pM1._14 * pM2._43;
		pOut._14 = pM1._11 * pM2._14 + pM1._12 * pM2._24 + pM1._13 * pM2._34 + pM1._14 * pM2._44;
		pOut._21 = pM1._21 * pM2._11 + pM1._22 * pM2._21 + pM1._23 * pM2._31 + pM1._24 * pM2._41;
		pOut._22 = pM1._21 * pM2._12 + pM1._22 * pM2._22 + pM1._23 * pM2._32 + pM1._24 * pM2._42;
		pOut._23 = pM1._21 * pM2._13 + pM1._22 * pM2._23 + pM1._23 * pM2._33 + pM1._24 * pM2._43;
		pOut._24 = pM1._21 * pM2._14 + pM1._22 * pM2._24 + pM1._23 * pM2._34 + pM1._24 * pM2._44;
		pOut._31 = pM1._31 * pM2._11 + pM1._32 * pM2._21 + pM1._33 * pM2._31 + pM1._34 * pM2._41;
		pOut._32 = pM1._31 * pM2._12 + pM1._32 * pM2._22 + pM1._33 * pM2._32 + pM1._34 * pM2._42;
		pOut._33 = pM1._31 * pM2._13 + pM1._32 * pM2._23 + pM1._33 * pM2._33 + pM1._34 * pM2._43;
		pOut._34 = pM1._31 * pM2._14 + pM1._32 * pM2._24 + pM1._33 * pM2._34 + pM1._34 * pM2._44;
		pOut._41 = pM1._41 * pM2._11 + pM1._42 * pM2._21 + pM1._43 * pM2._31 + pM1._44 * pM2._41;
		pOut._42 = pM1._41 * pM2._12 + pM1._42 * pM2._22 + pM1._43 * pM2._32 + pM1._44 * pM2._42;
		pOut._43 = pM1._41 * pM2._13 + pM1._42 * pM2._23 + pM1._43 * pM2._33 + pM1._44 * pM2._43;
		pOut._44 = pM1._41 * pM2._14 + pM1._42 * pM2._24 + pM1._43 * pM2._34 + pM1._44 * pM2._44;
		return pOut;
	}

	public static NKMMatrix MatrixLookAtLH(out NKMMatrix pOut, NKMVector3 pEye, NKMVector3 pAt, NKMVector3 pUp)
	{
		NKMVector3 nKMVector = default(NKMVector3);
		nKMVector.x = pAt.x - pEye.x;
		nKMVector.y = pAt.y - pEye.y;
		nKMVector.z = pAt.z - pEye.z;
		nKMVector.Normalize();
		NKMVector3.Vec3Cross(out var pOut2, pUp, nKMVector);
		pOut2.Normalize();
		NKMVector3.Vec3Cross(out var pOut3, nKMVector, pOut2);
		pOut._11 = pOut2.x;
		pOut._12 = pOut3.x;
		pOut._13 = nKMVector.x;
		pOut._14 = 0f;
		pOut._21 = pOut2.y;
		pOut._22 = pOut3.y;
		pOut._23 = nKMVector.y;
		pOut._24 = 0f;
		pOut._31 = pOut2.z;
		pOut._32 = pOut3.z;
		pOut._33 = nKMVector.z;
		pOut._34 = 0f;
		pOut._41 = 0f - NKMVector3.Vec3Dot(pOut2, pEye);
		pOut._42 = 0f - NKMVector3.Vec3Dot(pOut3, pEye);
		pOut._43 = 0f - NKMVector3.Vec3Dot(nKMVector, pEye);
		pOut._44 = 1f;
		return pOut;
	}

	public static NKMMatrix MatrixInverse(out NKMMatrix pOut, float pDeterminant, NKMMatrix pM)
	{
		return MatrixInverseTrace(out pOut, pDeterminant, pM);
	}

	public static NKMMatrix MatrixInverseTrace(out NKMMatrix pOut, float pDeterminant, NKMMatrix pM)
	{
		float num = MatrixDeterminant(pM);
		MatrixMultiply(out var pOut2, pM, pM);
		MatrixMultiply(out var pOut3, pOut2, pM);
		float num2 = MatrixTrace(pM);
		float num3 = MatrixTrace(pOut2);
		float num4 = MatrixTrace(pOut3);
		float num5 = (num2 * num2 * num2 - 3f * num2 * num3 + 2f * num4) / 6f;
		float num6 = (0f - (num2 * num2 - num3)) / 2f;
		MatrixIdentity(out var pOut4);
		pOut = (pOut4 * num5 + pM * num6 + pOut2 * num2 - pOut3) / num;
		if (!pDeterminant.IsNearlyZero())
		{
			pDeterminant = num;
		}
		return pOut;
	}

	public static float MatrixDeterminant(NKMMatrix pM)
	{
		return pM._11 * pM._22 * pM._33 * pM._44 + pM._11 * pM._23 * pM._34 * pM._42 + pM._11 * pM._24 * pM._32 * pM._43 + pM._12 * pM._21 * pM._34 * pM._43 + pM._12 * pM._23 * pM._31 * pM._44 + pM._12 * pM._24 * pM._33 * pM._41 + pM._13 * pM._21 * pM._32 * pM._44 + pM._13 * pM._22 * pM._34 * pM._41 + pM._13 * pM._24 * pM._31 * pM._42 + pM._14 * pM._21 * pM._33 * pM._42 + pM._14 * pM._22 * pM._31 * pM._43 + pM._14 * pM._23 * pM._32 * pM._41 - (pM._11 * pM._22 * pM._34 * pM._44 + pM._11 * pM._23 * pM._32 * pM._44 + pM._11 * pM._24 * pM._33 * pM._42 + pM._12 * pM._21 * pM._33 * pM._44 + pM._12 * pM._23 * pM._34 * pM._41 + pM._12 * pM._24 * pM._32 * pM._41 + pM._13 * pM._21 * pM._34 * pM._42 + pM._13 * pM._22 * pM._31 * pM._44 + pM._13 * pM._24 * pM._32 * pM._41 + pM._14 * pM._21 * pM._32 * pM._43 + pM._14 * pM._22 * pM._33 * pM._41 + pM._14 * pM._23 * pM._31 * pM._42);
	}

	public static float MatrixTrace(NKMMatrix pM)
	{
		return pM._11 + pM._22 + pM._33 + pM._44;
	}

	public static NKMVector4 Vec3Transform(out NKMVector4 pOut, NKMVector3 pV, NKMMatrix pM)
	{
		pOut.x = pV.x * pM._11 + pV.y * pM._21 + pV.z * pM._31 + pM._41;
		pOut.y = pV.x * pM._12 + pV.y * pM._22 + pV.z * pM._32 + pM._42;
		pOut.z = pV.x * pM._13 + pV.y * pM._23 + pV.z * pM._33 + pM._43;
		pOut.w = pV.x * pM._14 + pV.y * pM._24 + pV.z * pM._34 + pM._44;
		return pOut;
	}
}
