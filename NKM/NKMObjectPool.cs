using System.Collections.Generic;
using System.Threading;
using Cs.Logging;

namespace NKM;

public class NKMObjectPool
{
	private static long m_ObjUidIndex = 1L;

	protected Dictionary<NKM_OBJECT_POOL_TYPE, NKMObjectPoolList> m_dicObjectPoolList = new Dictionary<NKM_OBJECT_POOL_TYPE, NKMObjectPoolList>();

	protected Queue<NKMObjectPoolData> m_qAsyncLoadObject = new Queue<NKMObjectPoolData>();

	protected List<NKMObjectPoolData> m_listUnloadObjectTemp = new List<NKMObjectPoolData>();

	protected int m_ObjectCount;

	private int m_LoadCountMax;

	public static long GetObjUID()
	{
		return Interlocked.Increment(ref m_ObjUidIndex);
	}

	public virtual void Init()
	{
		List<NKMGameUnitRespawnData> list = new List<NKMGameUnitRespawnData>();
		for (int i = 0; i < 100; i++)
		{
			NKMGameUnitRespawnData item = (NKMGameUnitRespawnData)OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKMGameUnitRespawnData);
			list.Add(item);
		}
		for (int j = 0; j < list.Count; j++)
		{
			CloseObj(list[j]);
		}
		List<NKMStateCoolTime> list2 = new List<NKMStateCoolTime>();
		for (int k = 0; k < 100; k++)
		{
			NKMStateCoolTime item2 = (NKMStateCoolTime)OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKMStateCoolTime);
			list2.Add(item2);
		}
		for (int l = 0; l < list2.Count; l++)
		{
			CloseObj(list2[l]);
		}
		List<NKMTimeStamp> list3 = new List<NKMTimeStamp>();
		for (int m = 0; m < 100; m++)
		{
			NKMTimeStamp item3 = (NKMTimeStamp)OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKMTimeStamp);
			list3.Add(item3);
		}
		for (int n = 0; n < list3.Count; n++)
		{
			CloseObj(list3[n]);
		}
		List<NKMDamageInst> list4 = new List<NKMDamageInst>();
		for (int num = 0; num < 100; num++)
		{
			NKMDamageInst item4 = (NKMDamageInst)OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKMDamageInst);
			list4.Add(item4);
		}
		for (int num2 = 0; num2 < list4.Count; num2++)
		{
			CloseObj(list4[num2]);
		}
		List<NKMBuffData> list5 = new List<NKMBuffData>();
		for (int num3 = 0; num3 < 100; num3++)
		{
			NKMBuffData item5 = (NKMBuffData)OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKMBuffData);
			list5.Add(item5);
		}
		for (int num4 = 0; num4 < list5.Count; num4++)
		{
			CloseObj(list5[num4]);
		}
	}

	public virtual void Unload()
	{
		Dictionary<NKM_OBJECT_POOL_TYPE, NKMObjectPoolList>.Enumerator enumerator = m_dicObjectPoolList.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Dictionary<string, NKMObjectPoolListBundle>.Enumerator enumerator2 = enumerator.Current.Value.m_dicNKCObjectPoolDataBundle.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				Dictionary<string, NKMObjectPoolListName>.Enumerator enumerator3 = enumerator2.Current.Value.m_dicNKCObjectPoolDataName.GetEnumerator();
				while (enumerator3.MoveNext())
				{
					m_listUnloadObjectTemp.Clear();
					NKMObjectPoolListName value = enumerator3.Current.Value;
					Dictionary<long, NKMObjectPoolData>.Enumerator enumerator4 = value.m_dicNKCObjectPoolData.GetEnumerator();
					while (enumerator4.MoveNext())
					{
						NKMObjectPoolData value2 = enumerator4.Current.Value;
						if (value2.m_bUnloadable)
						{
							m_listUnloadObjectTemp.Add(value2);
						}
					}
					for (int i = 0; i < m_listUnloadObjectTemp.Count; i++)
					{
						NKMObjectPoolData nKMObjectPoolData = m_listUnloadObjectTemp[i];
						if (value.m_dicNKCObjectPoolData.ContainsKey(nKMObjectPoolData.m_ObjUID))
						{
							nKMObjectPoolData.Unload();
							value.m_dicNKCObjectPoolData.Remove(nKMObjectPoolData.m_ObjUID);
							m_ObjectCount--;
						}
					}
					m_listUnloadObjectTemp.Clear();
				}
			}
		}
	}

	public virtual void Update()
	{
	}

	public bool IsLoadComplete()
	{
		if (m_qAsyncLoadObject.Count > 0)
		{
			return false;
		}
		m_LoadCountMax = 0;
		return true;
	}

	public float GetLoadProgress()
	{
		float num = 0f;
		if (m_LoadCountMax == 0)
		{
			return num + 1f;
		}
		return num + (1f - (float)m_qAsyncLoadObject.Count / (float)m_LoadCountMax);
	}

	protected virtual NKMObjectPoolData CreateNewObj(NKM_OBJECT_POOL_TYPE typeT, string bundleName = "", string name = "", bool bAsync = false)
	{
		NKMObjectPoolData result = null;
		switch (typeT)
		{
		case NKM_OBJECT_POOL_TYPE.NOPT_NKMGameUnitRespawnData:
			result = new NKMGameUnitRespawnData();
			break;
		case NKM_OBJECT_POOL_TYPE.NOPT_NKMStateCoolTime:
			result = new NKMStateCoolTime();
			break;
		case NKM_OBJECT_POOL_TYPE.NOPT_NKMTimeStamp:
			result = new NKMTimeStamp();
			break;
		case NKM_OBJECT_POOL_TYPE.NOPT_NKMDamageInst:
			result = new NKMDamageInst();
			break;
		case NKM_OBJECT_POOL_TYPE.NOPT_NKMDamageEffect:
			result = new NKMDamageEffect();
			break;
		case NKM_OBJECT_POOL_TYPE.NOPT_NKMBuffData:
			result = new NKMBuffData();
			break;
		}
		return result;
	}

	public virtual NKMObjectPoolData OpenObj(NKM_OBJECT_POOL_TYPE typeT, NKMAssetName cNKMAssetName, bool bAsync = false)
	{
		return OpenObj(typeT, cNKMAssetName.m_BundleName, cNKMAssetName.m_AssetName, bAsync);
	}

	public virtual NKMObjectPoolData OpenObj(NKM_OBJECT_POOL_TYPE typeT, string bundleName = "", string name = "", bool bAsync = false)
	{
		NKMObjectPoolData nKMObjectPoolData = null;
		NKMObjectPoolList nKMObjectPoolList = null;
		NKMObjectPoolListName nKMObjectPoolListName = null;
		if (m_dicObjectPoolList.ContainsKey(typeT))
		{
			nKMObjectPoolList = m_dicObjectPoolList[typeT];
			if (nKMObjectPoolList.m_dicNKCObjectPoolDataBundle.ContainsKey(bundleName))
			{
				NKMObjectPoolListBundle nKMObjectPoolListBundle = nKMObjectPoolList.m_dicNKCObjectPoolDataBundle[bundleName];
				if (nKMObjectPoolListBundle.m_dicNKCObjectPoolDataName.ContainsKey(name))
				{
					nKMObjectPoolListName = nKMObjectPoolListBundle.m_dicNKCObjectPoolDataName[name];
					if (nKMObjectPoolListName.m_dicNKCObjectPoolData.Count > 0)
					{
						Dictionary<long, NKMObjectPoolData>.Enumerator enumerator = nKMObjectPoolListName.m_dicNKCObjectPoolData.GetEnumerator();
						enumerator.MoveNext();
						nKMObjectPoolData = enumerator.Current.Value;
						nKMObjectPoolListName.m_dicNKCObjectPoolData.Remove(enumerator.Current.Key);
						m_ObjectCount--;
					}
				}
			}
		}
		if (nKMObjectPoolData == null)
		{
			nKMObjectPoolData = CreateNewObj(typeT, bundleName, name, bAsync);
			if (nKMObjectPoolData == null)
			{
				return null;
			}
			if (!bAsync)
			{
				if (!nKMObjectPoolData.LoadComplete())
				{
					nKMObjectPoolData = CreateNewObj(typeT, bundleName, name);
					if (nKMObjectPoolData == null)
					{
						return null;
					}
					if (!nKMObjectPoolData.LoadComplete())
					{
						Log.ErrorAndExit("LoadComplete 실패 [m_ObjectPoolBundleName :" + nKMObjectPoolData.m_ObjectPoolBundleName + "] m_ObjectPoolName:" + nKMObjectPoolData.m_ObjectPoolName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMObjectPool.cs", 331);
						return null;
					}
				}
				nKMObjectPoolData.m_bIsLoaded = true;
				nKMObjectPoolData.Open();
			}
			else
			{
				m_qAsyncLoadObject.Enqueue(nKMObjectPoolData);
				m_LoadCountMax = m_qAsyncLoadObject.Count;
			}
		}
		else
		{
			nKMObjectPoolData.Open();
		}
		return nKMObjectPoolData;
	}

	public virtual void CloseObj(NKMObjectPoolData closeObj)
	{
		if (closeObj != null)
		{
			NKM_OBJECT_POOL_TYPE nKM_OBJECT_POOL_TYPE = closeObj.m_NKM_OBJECT_POOL_TYPE;
			NKMObjectPoolList nKMObjectPoolList = null;
			NKMObjectPoolListBundle nKMObjectPoolListBundle = null;
			NKMObjectPoolListName nKMObjectPoolListName = null;
			if (m_dicObjectPoolList.ContainsKey(nKM_OBJECT_POOL_TYPE))
			{
				nKMObjectPoolList = m_dicObjectPoolList[nKM_OBJECT_POOL_TYPE];
			}
			else
			{
				nKMObjectPoolList = new NKMObjectPoolList();
				m_dicObjectPoolList.Add(nKM_OBJECT_POOL_TYPE, nKMObjectPoolList);
			}
			if (nKMObjectPoolList.m_dicNKCObjectPoolDataBundle.ContainsKey(closeObj.m_ObjectPoolBundleName))
			{
				nKMObjectPoolListBundle = nKMObjectPoolList.m_dicNKCObjectPoolDataBundle[closeObj.m_ObjectPoolBundleName];
			}
			else
			{
				nKMObjectPoolListBundle = new NKMObjectPoolListBundle();
				nKMObjectPoolList.m_dicNKCObjectPoolDataBundle.Add(closeObj.m_ObjectPoolBundleName, nKMObjectPoolListBundle);
			}
			if (nKMObjectPoolListBundle.m_dicNKCObjectPoolDataName.ContainsKey(closeObj.m_ObjectPoolName))
			{
				nKMObjectPoolListName = nKMObjectPoolListBundle.m_dicNKCObjectPoolDataName[closeObj.m_ObjectPoolName];
			}
			else
			{
				nKMObjectPoolListName = new NKMObjectPoolListName();
				nKMObjectPoolListBundle.m_dicNKCObjectPoolDataName.Add(closeObj.m_ObjectPoolName, nKMObjectPoolListName);
			}
			if (!nKMObjectPoolListName.m_dicNKCObjectPoolData.ContainsKey(closeObj.m_ObjUID))
			{
				nKMObjectPoolListName.m_dicNKCObjectPoolData.Add(closeObj.m_ObjUID, closeObj);
				m_ObjectCount++;
			}
			closeObj.Close();
		}
	}
}
