using DG.Tweening;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Component.Office;

public class NKCUIComOfficeLoyalty : MonoBehaviour
{
	public Image m_imgGuage;

	public GameObject m_objFxFull;

	public GameObject m_objFxTake;

	public CanvasGroup m_CanvasGroup;

	public float m_fAlphaChangeTime = 1f;

	private NKMUnitData m_unitData;

	private const float UPDATE_GAP = 300f;

	private float m_fTimeToUpdate;

	private void Awake()
	{
		NKCUtil.SetGameobjectActive(m_objFxTake, bValue: false);
	}

	private void OnDisable()
	{
		NKCUtil.SetGameobjectActive(m_objFxTake, bValue: false);
	}

	public void SetData(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		m_unitData = unitData;
		UpdateData(m_unitData);
	}

	public void PlayTakeHeartEffect()
	{
		NKCUtil.SetGameobjectActive(m_objFxTake, bValue: true);
	}

	private void UpdateData(NKMUnitData unitData)
	{
		if (unitData.IsPermanentContract || unitData.loyalty >= 10000)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData);
		if (unitTempletBase != null && unitTempletBase.IsTrophy)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		if (unitData.CheckOfficeRoomHeartFull())
		{
			NKCUtil.SetImageFillAmount(m_imgGuage, 1f);
			NKCUtil.SetGameobjectActive(m_objFxFull, bValue: true);
			if (m_CanvasGroup != null)
			{
				m_CanvasGroup.DOKill();
				m_CanvasGroup.DOFade(1f, m_fAlphaChangeTime);
			}
		}
		else
		{
			float officeRoomHeartGauge = unitData.GetOfficeRoomHeartGauge();
			if (officeRoomHeartGauge >= 0.667f)
			{
				NKCUtil.SetImageFillAmount(m_imgGuage, 0.667f);
			}
			else if (officeRoomHeartGauge >= 0.333f)
			{
				NKCUtil.SetImageFillAmount(m_imgGuage, 0.333f);
			}
			else
			{
				NKCUtil.SetImageFillAmount(m_imgGuage, 0f);
			}
			NKCUtil.SetGameobjectActive(m_objFxFull, bValue: false);
			if (m_CanvasGroup != null)
			{
				m_CanvasGroup.DOKill();
				m_CanvasGroup.DOFade(0f, m_fAlphaChangeTime).SetDelay(4f);
			}
		}
		m_fTimeToUpdate = 300f;
	}

	private void Update()
	{
		m_fTimeToUpdate -= Time.unscaledDeltaTime;
		if (m_fTimeToUpdate < 0f)
		{
			UpdateData(m_unitData);
		}
		if (base.transform.lossyScale.x < 0f)
		{
			base.transform.localScale = new Vector3(0f - base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z);
		}
	}
}
