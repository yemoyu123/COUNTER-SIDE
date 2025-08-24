using System;

namespace NKM;

public abstract class NKMObjectPoolData : IComparable<NKMObjectPoolData>
{
	public long m_ObjUID = NKMObjectPool.GetObjUID();

	public NKM_OBJECT_POOL_TYPE m_NKM_OBJECT_POOL_TYPE;

	public string m_ObjectPoolBundleName = "";

	public string m_ObjectPoolName = "";

	public bool m_bIsLoaded;

	public bool m_bUnloadable;

	public virtual void Load(bool bAsync)
	{
	}

	public virtual bool LoadComplete()
	{
		return true;
	}

	public virtual void Open()
	{
	}

	public virtual void Close()
	{
	}

	public virtual void Unload()
	{
	}

	public int CompareTo(NKMObjectPoolData other)
	{
		if (m_NKM_OBJECT_POOL_TYPE > other.m_NKM_OBJECT_POOL_TYPE)
		{
			return -1;
		}
		if (other.m_NKM_OBJECT_POOL_TYPE > m_NKM_OBJECT_POOL_TYPE)
		{
			return 1;
		}
		return 0;
	}
}
