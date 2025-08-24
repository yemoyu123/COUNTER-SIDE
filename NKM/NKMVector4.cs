using UnityEngine;

namespace NKM;

public struct NKMVector4
{
	public float x;

	public float y;

	public float z;

	public float w;

	public NKMVector4(float fx = 0f, float fy = 0f, float fz = 0f, float fw = 0f)
	{
		x = fx;
		y = fy;
		z = fz;
		w = fw;
	}

	public void Set(float fx = 0f, float fy = 0f, float fz = 0f, float fw = 0f)
	{
		x = fx;
		y = fy;
		z = fz;
		w = fw;
	}

	public bool LoadFromLua(NKMLua cNKMLua, string pKey)
	{
		if (cNKMLua.OpenTable(pKey))
		{
			cNKMLua.GetData(1, ref x);
			cNKMLua.GetData(2, ref y);
			cNKMLua.GetData(3, ref z);
			cNKMLua.GetData(4, ref w);
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
			cNKMLua.GetData(4, ref w);
			cNKMLua.CloseTable();
		}
		return true;
	}

	public static implicit operator Vector4(NKMVector4 nv)
	{
		return new Vector4(nv.x, nv.y, nv.z, nv.w);
	}

	public static implicit operator NKMVector4(Vector4 uv)
	{
		return new NKMVector4(uv.x, uv.y, uv.z, uv.w);
	}
}
