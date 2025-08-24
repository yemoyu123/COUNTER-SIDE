using System.Collections.Generic;
using Cs.Logging;

namespace NKM;

public class NKMDamageEffectManager
{
	private short m_DEUIDIndex;

	private float m_fDeltaTime;

	protected NKMGame m_NKMGame;

	private Dictionary<short, NKMDamageEffect> m_dicNKMDamageEffect = new Dictionary<short, NKMDamageEffect>();

	private List<NKMDamageEffect> m_listNKMDamageEffectNextFrame = new List<NKMDamageEffect>();

	private List<short> m_listDEUIDDelete = new List<short>();

	public void Init(NKMGame cNKMGame)
	{
		m_DEUIDIndex = 1;
		m_fDeltaTime = 0f;
		m_NKMGame = cNKMGame;
		Dictionary<short, NKMDamageEffect>.Enumerator enumerator = m_dicNKMDamageEffect.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKMDamageEffect value = enumerator.Current.Value;
			CloseDamageEffect(value);
		}
		foreach (NKMDamageEffect item in m_listNKMDamageEffectNextFrame)
		{
			CloseDamageEffect(item);
		}
		m_dicNKMDamageEffect.Clear();
		m_listNKMDamageEffectNextFrame.Clear();
		m_listDEUIDDelete.Clear();
	}

	private short GetDEUID()
	{
		return m_DEUIDIndex++;
	}

	public void Update(float fDeltaTime)
	{
		NKMProfiler.BeginSample("NKMDamageEffectManager.Update");
		m_fDeltaTime = fDeltaTime;
		m_listDEUIDDelete.Clear();
		Dictionary<short, NKMDamageEffect>.Enumerator enumerator = m_dicNKMDamageEffect.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKMDamageEffect value = enumerator.Current.Value;
			if (value != null && (!(m_NKMGame.GetWorldStopTime() > 0f) || (value.GetTemplet() != null && value.GetTemplet().m_CanIgnoreStopTime && value.GetMasterUnit() != null && !value.GetMasterUnit().IsStopTime())))
			{
				value.Update(m_fDeltaTime);
				if (value.IsEnd())
				{
					m_listDEUIDDelete.Add(value.GetDEUID());
					CloseDamageEffect(value);
				}
			}
		}
		for (int i = 0; i < m_listDEUIDDelete.Count; i++)
		{
			m_dicNKMDamageEffect.Remove(m_listDEUIDDelete[i]);
		}
		foreach (NKMDamageEffect item in m_listNKMDamageEffectNextFrame)
		{
			if (!m_dicNKMDamageEffect.ContainsKey(item.GetDEUID()))
			{
				m_dicNKMDamageEffect.Add(item.GetDEUID(), item);
			}
			else
			{
				Log.Error($"NKMDamageEffectManager Update Duplicate DEUID: {item.GetDEUID()} ", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDamageEffectManager.cs", 99);
			}
		}
		m_listNKMDamageEffectNextFrame.Clear();
		NKMProfiler.EndSample();
	}

	public bool IsLiveEffect(short effectUID)
	{
		return m_dicNKMDamageEffect.ContainsKey(effectUID);
	}

	public NKMDamageEffect GetDamageEffect(short DEUID)
	{
		if (m_dicNKMDamageEffect.ContainsKey(DEUID))
		{
			return m_dicNKMDamageEffect[DEUID];
		}
		return null;
	}

	public NKMDamageEffect UseDamageEffect(string templetID, short masterGameUID, short targetGameUID, NKMUnitSkillTemplet cUnitSkillTemplet, int masterUnitPhase, float posX, float posY, float posZ, NKMEventPosData.MoveOffset moveOffset, float fPosMapRate, bool bRight = true, float offsetX = 0f, float offsetY = 0f, float offsetZ = 0f, float fAddRotate = 0f, bool bUseZScale = true, float fSpeedFactorX = 0f, float fSpeedFactorY = 0f, float reserveTime = 0f, bool bNextFrame = false)
	{
		NKMDamageEffect nKMDamageEffect = CreateDamageEffect();
		if (!nKMDamageEffect.SetDamageEffect(m_NKMGame, this, cUnitSkillTemplet, masterUnitPhase, GetDEUID(), templetID, masterGameUID, targetGameUID, posX, posY, posZ, bRight, moveOffset, fPosMapRate, offsetX, offsetY, offsetZ, fAddRotate, bUseZScale, fSpeedFactorX, fSpeedFactorY))
		{
			CloseDamageEffect(nKMDamageEffect);
			return null;
		}
		nKMDamageEffect.DoStateEndStart();
		if (!bNextFrame)
		{
			if (!m_dicNKMDamageEffect.ContainsKey(nKMDamageEffect.GetDEUID()))
			{
				m_dicNKMDamageEffect.Add(nKMDamageEffect.GetDEUID(), nKMDamageEffect);
			}
			else
			{
				Log.Error($"UseDamageEffect Duplicate DEUID: {nKMDamageEffect.GetDEUID()}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDamageEffectManager.cs", 149);
			}
		}
		else
		{
			m_listNKMDamageEffectNextFrame.Add(nKMDamageEffect);
		}
		return nKMDamageEffect;
	}

	protected virtual NKMDamageEffect CreateDamageEffect()
	{
		return (NKMDamageEffect)m_NKMGame.GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKMDamageEffect);
	}

	public void DeleteDE(short DEUID)
	{
		if (m_dicNKMDamageEffect.ContainsKey(DEUID))
		{
			NKMDamageEffect nKMDamageEffect = m_dicNKMDamageEffect[DEUID];
			if (nKMDamageEffect != null)
			{
				nKMDamageEffect.SetDie();
				CloseDamageEffect(nKMDamageEffect);
			}
			m_dicNKMDamageEffect.Remove(DEUID);
		}
	}

	protected virtual void CloseDamageEffect(NKMDamageEffect cNKMDamageEffect)
	{
		m_NKMGame.GetObjectPool().CloseObj(cNKMDamageEffect);
	}
}
