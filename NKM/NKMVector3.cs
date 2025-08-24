using System;
using UnityEngine;

namespace NKM;

public struct NKMVector3
{
	public float x;

	public float y;

	public float z;

	public float squareMagniture => x * x + y * y + z * z;

	public float magnitude => (float)Math.Sqrt(x * x + y * y + z * z);

	public NKMVector3(float fx = 0f, float fy = 0f, float fz = 0f)
	{
		x = fx;
		y = fy;
		z = fz;
	}

	public void Set(float fx = 0f, float fy = 0f, float fz = 0f)
	{
		x = fx;
		y = fy;
		z = fz;
	}

	public bool LoadFromLua(NKMLua cNKMLua, string pKey)
	{
		if (cNKMLua.OpenTable(pKey))
		{
			cNKMLua.GetData(1, ref x);
			cNKMLua.GetData(2, ref y);
			cNKMLua.GetData(3, ref z);
			cNKMLua.CloseTable();
		}
		return true;
	}

	public bool LoadFromLua(NKMLua cNKMLua, int index)
	{
		if (cNKMLua.OpenTable(index))
		{
			cNKMLua.GetData(1, ref x);
			cNKMLua.GetData(2, ref y);
			cNKMLua.GetData(3, ref z);
			cNKMLua.CloseTable();
		}
		return true;
	}

	public void Normalize()
	{
		float num = x * x + z * z + y * y;
		if (num > 0f)
		{
			num = (float)Math.Sqrt(num);
			x /= num;
			y /= num;
			z /= num;
		}
	}

	public static NKMVector3 Vec3Normalize(out NKMVector3 pOut, NKMVector3 pV)
	{
		float num = pV.x * pV.x + pV.z * pV.z + pV.y * pV.y;
		if (num > 0f)
		{
			num = (float)Math.Sqrt(num);
			pOut.x = pV.x / num;
			pOut.y = pV.y / num;
			pOut.z = pV.z / num;
			return pOut;
		}
		pOut = pV;
		return pOut;
	}

	public static NKMVector3 Vec3Cross(out NKMVector3 pOut, NKMVector3 A, NKMVector3 B)
	{
		pOut.x = A.y * B.z - B.y * A.z;
		pOut.y = A.z * B.x - B.z * A.x;
		pOut.z = A.x * B.y - B.x * A.y;
		return pOut;
	}

	public static float Vec3Dot(NKMVector3 a, NKMVector3 b)
	{
		return a.x * b.x + a.y * b.y + a.z * b.z;
	}

	public static NKMVector3 operator -(NKMVector3 a, NKMVector3 b)
	{
		return new NKMVector3(a.x - b.x, a.y - b.y, a.z - b.z);
	}

	public static NKMVector3 operator -(NKMVector3 a)
	{
		return new NKMVector3(0f - a.x, 0f - a.y, 0f - a.z);
	}

	public static NKMVector3 operator +(NKMVector3 a, NKMVector3 b)
	{
		return new NKMVector3(a.x + b.x, a.y + b.y, a.z + b.z);
	}

	public static NKMVector3 operator /(NKMVector3 a, NKMVector3 b)
	{
		return new NKMVector3(a.x / b.x, a.y / b.y, a.z / b.z);
	}

	public static NKMVector3 operator *(NKMVector3 a, NKMVector3 b)
	{
		return new NKMVector3(a.x * b.x, a.y * b.y, a.z * b.z);
	}

	public static NKMVector3 operator *(float a, NKMVector3 b)
	{
		return new NKMVector3(a * b.x, a * b.y, a * b.z);
	}

	public static NKMVector3 operator *(NKMVector3 a, float b)
	{
		return new NKMVector3(a.x * b, a.y * b, a.z * b);
	}

	public static NKMVector3 operator /(NKMVector3 a, float b)
	{
		return new NKMVector3(a.x / b, a.y / b, a.z / b);
	}

	public static implicit operator Vector3(NKMVector3 nv)
	{
		return new Vector3(nv.x, nv.y, nv.z);
	}

	public static implicit operator NKMVector3(Vector3 uv)
	{
		return new NKMVector3(uv.x, uv.y, uv.z);
	}
}
