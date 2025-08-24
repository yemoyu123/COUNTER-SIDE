using System;
using UnityEngine;

namespace NKM;

public struct NKMVector2
{
	public float x;

	public float y;

	public NKMVector2(float fx = 0f, float fy = 0f)
	{
		x = fx;
		y = fy;
	}

	public void Init(float fx = 0f, float fy = 0f)
	{
		x = fx;
		y = fy;
	}

	public void DeepCopyFromSource(NKMVector2 source)
	{
		x = source.x;
		y = source.y;
	}

	public bool LoadFromLua(NKMLua cNKMLua, string pKey)
	{
		if (cNKMLua.OpenTable(pKey))
		{
			cNKMLua.GetData(1, ref x);
			cNKMLua.GetData(2, ref y);
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
			cNKMLua.CloseTable();
		}
		return true;
	}

	public static NKMVector2 operator -(NKMVector2 a, NKMVector2 b)
	{
		return new NKMVector2(a.x - b.x, a.y - b.y);
	}

	public static NKMVector2 operator -(NKMVector2 a)
	{
		return new NKMVector2(0f - a.x, 0f - a.y);
	}

	public static NKMVector2 operator +(NKMVector2 a, NKMVector2 b)
	{
		return new NKMVector2(a.x + b.x, a.y + b.y);
	}

	public static NKMVector2 operator /(NKMVector2 a, NKMVector2 b)
	{
		return new NKMVector2(a.x / b.x, a.y / b.y);
	}

	public static NKMVector2 operator *(NKMVector2 a, NKMVector2 b)
	{
		return new NKMVector2(a.x * b.x, a.y * b.y);
	}

	public static NKMVector2 operator *(NKMVector2 a, float b)
	{
		return new NKMVector2(a.x * b, a.y * b);
	}

	public static NKMVector2 operator /(NKMVector2 a, float b)
	{
		return new NKMVector2(a.x / b, a.y / b);
	}

	public void Normalize()
	{
		float num = x * x + y * y;
		if (num > 0f)
		{
			num = (float)Math.Sqrt(num);
			x /= num;
			y /= num;
		}
	}

	public static NKMVector2 Vec2Normalize(out NKMVector2 pOut, NKMVector2 pV)
	{
		float num = pV.x * pV.x + pV.y * pV.y;
		if (num > 0f)
		{
			num = (float)Math.Sqrt(num);
			pOut.x = pV.x / num;
			pOut.y = pV.y / num;
			return pOut;
		}
		pOut = pV;
		return pOut;
	}

	public static float Vec2Dot(NKMVector2 a, NKMVector2 b)
	{
		return a.x * b.x + a.y * b.y;
	}

	public static implicit operator Vector2(NKMVector2 nv)
	{
		return new Vector2(nv.x, nv.y);
	}

	public static implicit operator NKMVector2(Vector2 uv)
	{
		return new NKMVector2(uv.x, uv.y);
	}
}
