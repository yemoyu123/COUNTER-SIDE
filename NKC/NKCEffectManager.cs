using System.Collections.Generic;
using NKM;
using UnityEngine;

namespace NKC;

public class NKCEffectManager
{
	private int m_EffectUIDIndex = 1;

	private Dictionary<string, NKCEffectTemplet> m_dicEffectTemplet = new Dictionary<string, NKCEffectTemplet>();

	private Dictionary<int, NKCASEffect> m_dicEffect = new Dictionary<int, NKCASEffect>();

	private List<int> m_listEffectUIDDelete = new List<int>();

	private LinkedList<NKCEffectReserveData> m_linklistEffectReserveData = new LinkedList<NKCEffectReserveData>();

	private float m_fDeltaTime;

	public void Init()
	{
		foreach (NKCEffectReserveData linklistEffectReserveDatum in m_linklistEffectReserveData)
		{
			NKCScenManager.GetScenManager().GetObjectPool().CloseObj(linklistEffectReserveDatum);
		}
		m_linklistEffectReserveData.Clear();
		Dictionary<int, NKCASEffect>.Enumerator enumerator2 = m_dicEffect.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			NKCASEffect value = enumerator2.Current.Value;
			if (value != null)
			{
				NKCScenManager.GetScenManager().GetObjectPool().CloseObj(value);
			}
		}
		m_dicEffect.Clear();
	}

	private int GetEffectUID()
	{
		return m_EffectUIDIndex++;
	}

	public bool LoadFromLUA(string fileName)
	{
		NKMLua nKMLua = new NKMLua();
		if (nKMLua.LoadCommonPath("AB_SCRIPT_EFFECT", fileName) && nKMLua.OpenTable("m_dicEffectTemplet"))
		{
			int num = 1;
			while (nKMLua.OpenTable(num))
			{
				NKCEffectTemplet nKCEffectTemplet = new NKCEffectTemplet();
				nKCEffectTemplet.LoadFromLUA(nKMLua);
				if (!m_dicEffectTemplet.ContainsKey(nKCEffectTemplet.m_Name))
				{
					m_dicEffectTemplet.Add(nKCEffectTemplet.m_Name, nKCEffectTemplet);
				}
				num++;
				nKMLua.CloseTable();
			}
			nKMLua.CloseTable();
		}
		nKMLua.LuaClose();
		return true;
	}

	public void ObjectParentWait()
	{
		Dictionary<int, NKCASEffect>.Enumerator enumerator = m_dicEffect.GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.Value?.ObjectParentWait();
		}
	}

	public void ObjectParentRestore()
	{
		Dictionary<int, NKCASEffect>.Enumerator enumerator = m_dicEffect.GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.Value?.ObjectParentRestore();
		}
	}

	public void Update(float fDeltaTime)
	{
		m_fDeltaTime = fDeltaTime;
		LinkedListNode<NKCEffectReserveData> linkedListNode = m_linklistEffectReserveData.First;
		while (linkedListNode != null)
		{
			NKCEffectReserveData value = linkedListNode.Value;
			if (value != null)
			{
				value.m_fReserveTime -= m_fDeltaTime;
				if (value.m_fReserveTime <= 0f)
				{
					EffectStart(value.m_NKCASEffect, value.m_PosX, value.m_PosY, value.m_PosZ, value.m_bNotStart);
					NKCScenManager.GetScenManager().GetObjectPool().CloseObj(value);
					LinkedListNode<NKCEffectReserveData> next = linkedListNode.Next;
					m_linklistEffectReserveData.Remove(linkedListNode);
					linkedListNode = next;
					continue;
				}
			}
			linkedListNode = linkedListNode.Next;
		}
		m_listEffectUIDDelete.Clear();
		Dictionary<int, NKCASEffect>.Enumerator enumerator = m_dicEffect.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKCASEffect value2 = enumerator.Current.Value;
			if (value2 == null)
			{
				continue;
			}
			bool flag = false;
			bool flag2 = false;
			if (NKCScenManager.GetScenManager().GetGameClient() != null && NKCScenManager.GetScenManager().GetGameClient().GetWorldStopTime() > 0f)
			{
				flag = true;
				if (value2.GetMasterUnit() != null)
				{
					flag2 = value2.GetMasterUnit().IsStopTime();
				}
			}
			if (!value2.m_bDEEffect)
			{
				flag = ((value2.ApplyStopTime && flag2) ? true : false);
			}
			else if (value2.CanIgnoreStopTime && !flag2)
			{
				flag = false;
			}
			if (!flag)
			{
				value2.Update(m_fDeltaTime);
			}
			if (value2.m_bAutoDie && value2.IsEnd())
			{
				m_listEffectUIDDelete.Add(value2.m_EffectUID);
				NKCScenManager.GetScenManager().GetObjectPool().CloseObj(value2);
			}
		}
		for (int i = 0; i < m_listEffectUIDDelete.Count; i++)
		{
			m_dicEffect.Remove(m_listEffectUIDDelete[i]);
		}
	}

	public void StopEffectAnim()
	{
		Dictionary<int, NKCASEffect>.Enumerator enumerator = m_dicEffect.GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.Value?.StopEffectAnim();
		}
	}

	public bool IsLiveEffect(int effectUID)
	{
		return m_dicEffect.ContainsKey(effectUID);
	}

	public void DeleteAllEffect()
	{
		foreach (int item in new List<int>(m_dicEffect.Keys))
		{
			DeleteEffect(item);
		}
	}

	public void DeleteEffect(NKCASEffect cNKCASEffect)
	{
		if (cNKCASEffect != null)
		{
			DeleteEffect(cNKCASEffect.m_EffectUID);
		}
	}

	public void DeleteEffect(int effectUID)
	{
		if (m_dicEffect.ContainsKey(effectUID))
		{
			NKCASEffect closeObj = m_dicEffect[effectUID];
			NKCScenManager.GetScenManager().GetObjectPool().CloseObj(closeObj);
			m_dicEffect.Remove(effectUID);
		}
	}

	public NKCASEffect UseEffect(short masterUnitGameUID, NKMAssetName name, NKM_EFFECT_PARENT_TYPE eNKM_EFFECT_PARENT_TYPE, float posX, float posY, float posZ, bool bRight = true, float fScaleFactor = 1f, float offsetX = 0f, float offsetY = 0f, float offsetZ = 0f, bool m_bUseZtoY = false, float fAddRotate = 0f, bool bUseZScale = false, string boneName = "", bool bUseBoneRotate = false, bool bAutoDie = true, string animName = "", float fAnimSpeed = 1f, bool bNotStart = false, bool bCutIn = false, float reserveTime = 0f, float reserveDieTime = -1f, bool bDEEffect = false)
	{
		return UseEffect(masterUnitGameUID, name.m_BundleName, name.m_AssetName, eNKM_EFFECT_PARENT_TYPE, posX, posY, posZ, bRight, fScaleFactor, offsetX, offsetY, offsetZ, m_bUseZtoY, fAddRotate, bUseZScale, boneName, bUseBoneRotate, bAutoDie, animName, fAnimSpeed, bNotStart, bCutIn, reserveTime, reserveDieTime, bDEEffect);
	}

	public NKCASEffect UseEffect(short masterUnitGameUID, string bundleName, string name, NKM_EFFECT_PARENT_TYPE eNKM_EFFECT_PARENT_TYPE, float posX, float posY, float posZ, bool bRight = true, float fScaleFactor = 1f, float offsetX = 0f, float offsetY = 0f, float offsetZ = 0f, bool m_bUseZtoY = false, float fAddRotate = 0f, bool bUseZScale = false, string boneName = "", bool bUseBoneRotate = false, bool bAutoDie = true, string animName = "", float fAnimSpeed = 1f, bool bNotStart = false, bool bCutIn = false, float reserveTime = 0f, float reserveDieTime = -1f, bool bDEEffect = false)
	{
		if (!bDEEffect && NKCScenManager.GetScenManager().GetNKCPowerSaveMode().GetEnable())
		{
			return null;
		}
		NKCASEffect nKCASEffect = (NKCASEffect)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASEffect, bundleName, name);
		if (nKCASEffect == null)
		{
			return null;
		}
		if (nKCASEffect.GetLoadFail())
		{
			DeleteEffect(nKCASEffect.m_EffectUID);
			return null;
		}
		nKCASEffect.m_EffectUID = GetEffectUID();
		nKCASEffect.m_NKM_EFFECT_PARENT_TYPE = eNKM_EFFECT_PARENT_TYPE;
		nKCASEffect.m_OffsetX = offsetX;
		if (!m_bUseZtoY)
		{
			nKCASEffect.m_OffsetY = offsetY;
		}
		else
		{
			nKCASEffect.m_OffsetY = offsetY + offsetZ;
		}
		nKCASEffect.m_OffsetZ = offsetZ;
		nKCASEffect.m_fAddRotate = fAddRotate;
		nKCASEffect.m_bRight = bRight;
		nKCASEffect.m_MasterUnitGameUID = masterUnitGameUID;
		nKCASEffect.m_BoneName = boneName;
		nKCASEffect.m_bUseBoneRotate = bUseBoneRotate;
		nKCASEffect.m_bAutoDie = bAutoDie;
		nKCASEffect.m_AnimName = animName;
		nKCASEffect.m_fAnimSpeed = fAnimSpeed;
		nKCASEffect.m_bCutIn = bCutIn;
		nKCASEffect.m_bUseZScale = bUseZScale;
		nKCASEffect.m_bDEEffect = bDEEffect;
		nKCASEffect.SetReserveDieTime(reserveDieTime);
		nKCASEffect.SetScaleFactor(fScaleFactor, fScaleFactor, fScaleFactor);
		if (nKCASEffect.m_EffectInstant != null && nKCASEffect.m_EffectInstant.m_Instant != null)
		{
			bool flag = false;
			if (masterUnitGameUID != 0 && NKCScenManager.GetScenManager() != null && NKCScenManager.GetScenManager().GetGameClient() != null)
			{
				NKMUnit unit = NKCScenManager.GetScenManager().GetGameClient().GetUnit(masterUnitGameUID, bChain: true, bPool: true);
				if (unit != null)
				{
					flag = NKCScenManager.GetScenManager().GetGameClient().m_MyTeam == unit.GetTeam();
				}
			}
			Renderer[] componentsInChildren = nKCASEffect.m_EffectInstant.m_Instant.GetComponentsInChildren<Renderer>(includeInactive: true);
			foreach (Renderer renderer in componentsInChildren)
			{
				if (renderer.material != null)
				{
					renderer.material.SetFloat("_IsMine", flag ? 1 : 0);
				}
			}
		}
		if (reserveTime > 0f)
		{
			if (nKCASEffect.m_EffectInstant != null && nKCASEffect.m_EffectInstant.m_Instant != null && nKCASEffect.m_EffectInstant.m_Instant.activeSelf)
			{
				nKCASEffect.m_EffectInstant.m_Instant.SetActive(value: false);
			}
			NKCEffectReserveData nKCEffectReserveData = (NKCEffectReserveData)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCEffectReserveData);
			nKCEffectReserveData.m_NKCASEffect = nKCASEffect;
			nKCEffectReserveData.m_PosX = posX;
			nKCEffectReserveData.m_PosY = posY;
			nKCEffectReserveData.m_PosZ = posZ;
			nKCEffectReserveData.m_bNotStart = bNotStart;
			nKCEffectReserveData.m_fReserveTime = reserveTime;
			m_linklistEffectReserveData.AddLast(nKCEffectReserveData);
			return nKCEffectReserveData.m_NKCASEffect;
		}
		return EffectStart(nKCASEffect, posX, posY, posZ, bNotStart);
	}

	public NKCASEffect EffectStart(NKCASEffect cNKCASEffect, float posX, float posY, float posZ, bool bNotStart)
	{
		if (!bNotStart)
		{
			if (cNKCASEffect.m_EffectInstant != null && cNKCASEffect.m_EffectInstant.m_Instant != null && !cNKCASEffect.m_EffectInstant.m_Instant.activeSelf)
			{
				cNKCASEffect.m_EffectInstant.m_Instant.SetActive(value: true);
			}
			cNKCASEffect.m_bPlayed = true;
		}
		else if (cNKCASEffect.m_EffectInstant != null && cNKCASEffect.m_EffectInstant.m_Instant != null && cNKCASEffect.m_EffectInstant.m_Instant.activeSelf)
		{
			cNKCASEffect.m_EffectInstant.m_Instant.SetActive(value: false);
		}
		cNKCASEffect.SetRight(cNKCASEffect.m_bRight);
		cNKCASEffect.ObjectParentRestore();
		if (!cNKCASEffect.m_bCutIn)
		{
			if (posX != -1f || posY != -1f || posZ != -1f)
			{
				cNKCASEffect.SetPos(posX, posY, posZ);
			}
			if (cNKCASEffect.m_fAddRotate != -1f)
			{
				cNKCASEffect.SetRotate(cNKCASEffect.m_fAddRotate);
			}
		}
		else
		{
			cNKCASEffect.m_bUseZScale = false;
		}
		if (!bNotStart && cNKCASEffect.m_AnimName.Length > 1)
		{
			float fAnimSpeed = cNKCASEffect.m_fAnimSpeed;
			cNKCASEffect.PlayAnim(cNKCASEffect.m_AnimName, bLoop: false, fAnimSpeed);
		}
		if (cNKCASEffect.m_EffectInstant != null && cNKCASEffect.m_EffectInstant.m_Instant != null)
		{
			cNKCASEffect.m_EffectInstant.m_Instant.transform.SetAsLastSibling();
		}
		m_dicEffect.Add(cNKCASEffect.m_EffectUID, cNKCASEffect);
		return cNKCASEffect;
	}

	public void StopCutInEffect()
	{
		m_listEffectUIDDelete.Clear();
		Dictionary<int, NKCASEffect>.Enumerator enumerator = m_dicEffect.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKCASEffect value = enumerator.Current.Value;
			if (value != null && value.m_bCutIn && (value.m_bAutoDie || value.m_fReserveDieTime != -1f))
			{
				m_listEffectUIDDelete.Add(value.m_EffectUID);
				NKCScenManager.GetScenManager().GetObjectPool().CloseObj(value);
			}
		}
		for (int i = 0; i < m_listEffectUIDDelete.Count; i++)
		{
			m_dicEffect.Remove(m_listEffectUIDDelete[i]);
		}
	}
}
