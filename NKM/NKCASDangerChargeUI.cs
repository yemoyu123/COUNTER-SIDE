using NKC;
using UnityEngine;
using UnityEngine.UI;

namespace NKM;

public class NKCASDangerChargeUI : NKMObjectPoolData
{
	private NKCAssetInstanceData m_Instant;

	private Image m_DANGER_CHARGE_WAIT_TIME_Image;

	private Image m_DANGER_CHARGE_DAMAGE_Image;

	private bool m_bOpen;

	private float m_fTimeMax;

	private float m_fDamageMax;

	private float m_fHitCountMax;

	public NKCASDangerChargeUI(bool bAsync = false)
	{
		m_NKM_OBJECT_POOL_TYPE = NKM_OBJECT_POOL_TYPE.NOPT_NKCASDangerChargeUI;
		m_bUnloadable = true;
		Load(bAsync);
	}

	public override void Load(bool bAsync)
	{
		m_Instant = NKCAssetResourceManager.OpenInstance<GameObject>("AB_FX_UI_DANGER", "AB_FX_UI_DANGER", bAsync);
	}

	public override bool LoadComplete()
	{
		if (m_Instant == null || m_Instant.m_Instant == null)
		{
			return false;
		}
		m_Instant.m_Instant.transform.SetParent(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
			.Get_GAME_BATTLE_UNIT_VIEWER()
			.transform, worldPositionStays: false);
			m_DANGER_CHARGE_WAIT_TIME_Image = m_Instant.m_Instant.transform.Find("AB_FX_UI_DANGER/DANGER_GAUGE/DANGER_GAUGE_BACK/DANGER_CHARGE_WAIT_TIME").gameObject.GetComponent<Image>();
			m_DANGER_CHARGE_DAMAGE_Image = m_Instant.m_Instant.transform.Find("AB_FX_UI_DANGER/DANGER_GAUGE/DANGER_GAUGE_BACK/DANGER_CHARGE_DAMAGE").gameObject.GetComponent<Image>();
			CloseDangerCharge();
			return true;
		}

		public override void Open()
		{
			CloseDangerCharge();
		}

		public override void Close()
		{
			if (m_Instant.m_Instant.activeSelf)
			{
				m_Instant.m_Instant.SetActive(value: false);
			}
			CloseDangerCharge();
		}

		public override void Unload()
		{
			NKCAssetResourceManager.CloseInstance(m_Instant);
			m_Instant = null;
			m_DANGER_CHARGE_WAIT_TIME_Image = null;
			m_DANGER_CHARGE_DAMAGE_Image = null;
		}

		public void OpenDangerCharge(float fTimeMax, float fDamageMax, float fHitCountMax)
		{
			m_bOpen = true;
			m_fTimeMax = fTimeMax;
			m_fDamageMax = fDamageMax;
			m_fHitCountMax = fHitCountMax;
			m_DANGER_CHARGE_WAIT_TIME_Image.fillAmount = 0f;
			m_DANGER_CHARGE_DAMAGE_Image.fillAmount = 0f;
			if (!m_Instant.m_Instant.activeSelf)
			{
				m_Instant.m_Instant.SetActive(value: true);
			}
		}

		public void CloseDangerCharge()
		{
			m_bOpen = false;
			if (m_Instant.m_Instant.activeSelf)
			{
				m_Instant.m_Instant.SetActive(value: false);
			}
		}

		public void SetPos(ref Vector3 m_Pos)
		{
			if (m_bOpen)
			{
				m_Instant.m_Instant.transform.localPosition = m_Pos;
			}
		}

		public void Update(float fRemainTime, float fDamage, float fHitCount)
		{
			if (m_bOpen)
			{
				m_DANGER_CHARGE_WAIT_TIME_Image.fillAmount = (m_fTimeMax - fRemainTime) / m_fTimeMax;
				if (m_fDamageMax > 0f)
				{
					m_DANGER_CHARGE_DAMAGE_Image.fillAmount = fDamage / m_fDamageMax;
				}
				else if (m_fHitCountMax > 0f)
				{
					m_DANGER_CHARGE_DAMAGE_Image.fillAmount = fHitCount / m_fHitCountMax;
				}
			}
		}
	}
