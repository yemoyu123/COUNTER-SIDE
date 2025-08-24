using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCDiveGameSector : MonoBehaviour
{
	public delegate void OnClickSector(NKCDiveGameSector cNKMDiveSlot, bool bByAuto);

	private OnClickSector m_dOnClickSector;

	public Animator m_NKM_UI_DIVE_SECTOR_SLOT_Animator;

	public CanvasGroup m_CanvasGroup;

	public NKCUIComStateButton m_NKM_UI_DIVE_SECTOR_SLOT;

	public GameObject m_NKM_UI_DIVE_SECTOR_SLOT_START;

	[Header("건틀릿")]
	public GameObject m_NKM_UI_DIVE_SECTOR_SLOT_GAUNTLET;

	public GameObject m_NKM_UI_DIVE_SECTOR_SLOT_ICON_GAUNTLET;

	public GameObject m_NKM_UI_DIVE_SECTOR_SLOT_ICON_GAUNTLET_BIG;

	public Image m_NKM_UI_DIVE_SECTOR_SLOT_GAUNTLET_Img;

	public Text m_NKM_UI_DIVE_SECTOR_SLOT_ICON_GAUNTLET_TEXT;

	public GameObject m_NKM_UI_DIVE_SECTOR_SLOT_GAUNTLET_SELECTED;

	[Header("리만")]
	public GameObject m_NKM_UI_DIVE_SECTOR_SLOT_REIMANN;

	public GameObject m_NKM_UI_DIVE_SECTOR_SLOT_ICON_REIMANN;

	public GameObject m_NKM_UI_DIVE_SECTOR_SLOT_ICON_REIMANN_BIG;

	public Image m_NKM_UI_DIVE_SECTOR_SLOT_REIMANN_Img;

	public Text m_NKM_UI_DIVE_SECTOR_SLOT_ICON_REIMANN_TEXT;

	public GameObject m_NKM_UI_DIVE_SECTOR_SLOT_REIMANN_SELECTED;

	[Header("푸엥")]
	public GameObject m_NKM_UI_DIVE_SECTOR_SLOT_POINCARE;

	public GameObject m_NKM_UI_DIVE_SECTOR_SLOT_ICON_POINCARE;

	public GameObject m_NKM_UI_DIVE_SECTOR_SLOT_ICON_POINCARE_BIG;

	public Image m_NKM_UI_DIVE_SECTOR_SLOT_POINCARE_Img;

	public Text m_NKM_UI_DIVE_SECTOR_SLOT_ICON_POINCARE_TEXT;

	public GameObject m_NKM_UI_DIVE_SECTOR_SLOT_POINCARE_SELECTED;

	[Header("유클리드")]
	public GameObject m_NKM_UI_DIVE_SECTOR_SLOT_EUCLID;

	public GameObject m_NKM_UI_DIVE_SECTOR_SLOT_ICON_EUCLID;

	public GameObject m_NKM_UI_DIVE_SECTOR_SLOT_ICON_EUCLID_BIG;

	public Image m_NKM_UI_DIVE_SECTOR_SLOT_EUCLID_Img;

	public Text m_NKM_UI_DIVE_SECTOR_SLOT_ICON_EUCLID_TEXT;

	public GameObject m_NKM_UI_DIVE_SECTOR_SLOT_EUCLID_SELECTED;

	[Header("유클리드 섹터 타입")]
	public GameObject m_EUCLID_TYPE_NORMAL;

	public GameObject m_EUCLID_TYPE_ARTIFACT;

	public GameObject m_EUCLID_TYPE_REPAIR;

	[Header("보스")]
	public GameObject m_NKM_UI_DIVE_SECTOR_SLOT_BOSS;

	public GameObject m_NKM_UI_DIVE_SECTOR_SLOT_ICON_BOSS;

	public GameObject m_NKM_UI_DIVE_SECTOR_SLOT_ICON_BOSS_BIG;

	public GameObject m_NKM_UI_DIVE_SECTOR_SLOT_BOSS_SELECTED;

	[Header("이벤트")]
	public GameObject m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_RANDOM;

	public Image m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_RANDOM_Img;

	public GameObject m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_RESCUE_SIGNAL;

	public Image m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_RESCUE_SIGNAL_Img;

	public GameObject m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_LOST_CONTAINER;

	public Image m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_LOST_CONTAINER_Img;

	public GameObject m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_LOST_SHIP;

	public Image m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_LOST_SHIP_Img;

	public GameObject m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_SAFETY;

	public GameObject m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_REPAIR_KIT;

	public Image m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_REPAIR_KIT_Img;

	private NKMDiveSlot m_NKMDiveSlot;

	private NKCDiveGameSectorSet m_NKCDiveGameSectorSet;

	private int m_SlotIndex = -1;

	private int m_UISlotIndex = -1;

	private bool m_bGrey;

	public NKMDiveSlot GetNKMDiveSlot()
	{
		return m_NKMDiveSlot;
	}

	public bool GetGrey()
	{
		return m_bGrey;
	}

	public void SetSlotIndex(int index)
	{
		m_SlotIndex = index;
	}

	public int GetSlotIndex()
	{
		return m_SlotIndex;
	}

	public void SetUISlotIndex(int index)
	{
		m_UISlotIndex = index;
	}

	public int GetUISlotIndex()
	{
		return m_UISlotIndex;
	}

	public void Init(NKCDiveGameSectorSet cNKCDiveGameSectorSet, OnClickSector dOnClickSector = null)
	{
		m_NKCDiveGameSectorSet = cNKCDiveGameSectorSet;
		m_dOnClickSector = dOnClickSector;
		m_NKM_UI_DIVE_SECTOR_SLOT.PointerClick.RemoveAllListeners();
		m_NKM_UI_DIVE_SECTOR_SLOT.PointerClick.AddListener(OnClick);
	}

	public int GetRealSetSize()
	{
		if (m_NKCDiveGameSectorSet != null)
		{
			return m_NKCDiveGameSectorSet.GetRealSetSize();
		}
		return 0;
	}

	public void InvaldGrey()
	{
		NKCUtil.SetImageMaterial(m_NKM_UI_DIVE_SECTOR_SLOT_GAUNTLET_Img, null);
		NKCUtil.SetImageMaterial(m_NKM_UI_DIVE_SECTOR_SLOT_REIMANN_Img, null);
		NKCUtil.SetImageMaterial(m_NKM_UI_DIVE_SECTOR_SLOT_POINCARE_Img, null);
		NKCUtil.SetImageMaterial(m_NKM_UI_DIVE_SECTOR_SLOT_EUCLID_Img, null);
		NKCUtil.SetImageMaterial(m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_RANDOM_Img, null);
		NKCUtil.SetImageMaterial(m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_RESCUE_SIGNAL_Img, null);
		NKCUtil.SetImageMaterial(m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_LOST_CONTAINER_Img, null);
		NKCUtil.SetImageMaterial(m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_LOST_SHIP_Img, null);
		NKCUtil.SetImageMaterial(m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_REPAIR_KIT_Img, null);
		NKCUtil.SetLabelTextColor(m_NKM_UI_DIVE_SECTOR_SLOT_ICON_GAUNTLET_TEXT, new Color(1f, 0.9254902f, 27f / 85f, 1f));
		NKCUtil.SetLabelTextColor(m_NKM_UI_DIVE_SECTOR_SLOT_ICON_REIMANN_TEXT, new Color(0.7372549f, 0.56078434f, 1f, 1f));
		NKCUtil.SetLabelTextColor(m_NKM_UI_DIVE_SECTOR_SLOT_ICON_POINCARE_TEXT, new Color(1f, 0.19215687f, 0.20784314f, 1f));
		m_bGrey = false;
		SetAlphaByReachable();
	}

	public void SetGrey()
	{
		InvaldGrey();
		Material orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Material>("AB_UI_NKM_UI_OPERATION_EP_THUMBNAIL", "EP_THUMBNAIL_BLACK_AND_WHITE");
		if (orLoadAssetResource != null)
		{
			if (m_NKM_UI_DIVE_SECTOR_SLOT_GAUNTLET != null && m_NKM_UI_DIVE_SECTOR_SLOT_GAUNTLET.activeSelf)
			{
				NKCUtil.SetImageMaterial(m_NKM_UI_DIVE_SECTOR_SLOT_GAUNTLET_Img, orLoadAssetResource);
				m_bGrey = true;
				NKCUtil.SetLabelTextColor(m_NKM_UI_DIVE_SECTOR_SLOT_ICON_GAUNTLET_TEXT, new Color(1f, 1f, 1f, 1f));
			}
			else if (m_NKM_UI_DIVE_SECTOR_SLOT_REIMANN != null && m_NKM_UI_DIVE_SECTOR_SLOT_REIMANN.activeSelf)
			{
				NKCUtil.SetImageMaterial(m_NKM_UI_DIVE_SECTOR_SLOT_REIMANN_Img, orLoadAssetResource);
				m_bGrey = true;
				NKCUtil.SetLabelTextColor(m_NKM_UI_DIVE_SECTOR_SLOT_ICON_REIMANN_TEXT, new Color(1f, 1f, 1f, 1f));
			}
			else if (m_NKM_UI_DIVE_SECTOR_SLOT_POINCARE != null && m_NKM_UI_DIVE_SECTOR_SLOT_POINCARE.activeSelf)
			{
				NKCUtil.SetImageMaterial(m_NKM_UI_DIVE_SECTOR_SLOT_POINCARE_Img, orLoadAssetResource);
				m_bGrey = true;
				NKCUtil.SetLabelTextColor(m_NKM_UI_DIVE_SECTOR_SLOT_ICON_POINCARE_TEXT, new Color(1f, 1f, 1f, 1f));
			}
			else if (m_NKM_UI_DIVE_SECTOR_SLOT_EUCLID != null && m_NKM_UI_DIVE_SECTOR_SLOT_EUCLID.activeSelf)
			{
				NKCUtil.SetImageMaterial(m_NKM_UI_DIVE_SECTOR_SLOT_EUCLID_Img, orLoadAssetResource);
				m_bGrey = true;
			}
			if (m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_RANDOM != null && m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_RANDOM.activeSelf)
			{
				NKCUtil.SetImageMaterial(m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_RANDOM_Img, orLoadAssetResource);
			}
			else if (m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_RESCUE_SIGNAL != null && m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_RESCUE_SIGNAL.activeSelf)
			{
				NKCUtil.SetImageMaterial(m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_RESCUE_SIGNAL_Img, orLoadAssetResource);
			}
			else if (m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_LOST_CONTAINER != null && m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_LOST_CONTAINER.activeSelf)
			{
				NKCUtil.SetImageMaterial(m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_LOST_CONTAINER_Img, orLoadAssetResource);
			}
			else if (m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_LOST_SHIP != null && m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_LOST_SHIP.activeSelf)
			{
				NKCUtil.SetImageMaterial(m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_LOST_SHIP_Img, orLoadAssetResource);
			}
			else if (m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_REPAIR_KIT != null && m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_REPAIR_KIT.activeSelf)
			{
				NKCUtil.SetImageMaterial(m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_REPAIR_KIT_Img, orLoadAssetResource);
			}
			if (m_bGrey)
			{
				m_CanvasGroup.alpha = 0.25f;
			}
		}
	}

	public bool CheckSelectable()
	{
		if (m_CanvasGroup.alpha < 1f || m_bGrey)
		{
			return false;
		}
		return true;
	}

	public void PlayOpenAni(bool playSound)
	{
		if (m_NKM_UI_DIVE_SECTOR_SLOT_Animator != null)
		{
			m_NKM_UI_DIVE_SECTOR_SLOT_Animator.Play("NKM_UI_DIVE_SECTOR_SLOT_OPEN");
			if (playSound)
			{
				NKCSoundManager.PlaySound("FX_UI_DIVE_SLOT_OPEN", 1f, 0f, 0f);
			}
		}
	}

	public void SetSelected(bool bSet)
	{
		if (bSet)
		{
			if (m_NKMDiveSlot != null)
			{
				if (NKCDiveManager.IsGauntletSectorType(m_NKMDiveSlot.SectorType))
				{
					NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_GAUNTLET_SELECTED, bValue: true);
				}
				else if (NKCDiveManager.IsReimannSectorType(m_NKMDiveSlot.SectorType))
				{
					NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_REIMANN_SELECTED, bValue: true);
				}
				else if (NKCDiveManager.IsPoincareSectorType(m_NKMDiveSlot.SectorType))
				{
					NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_POINCARE_SELECTED, bValue: true);
				}
				else if (NKCDiveManager.IsEuclidSectorType(m_NKMDiveSlot.SectorType))
				{
					NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_EUCLID_SELECTED, bValue: true);
				}
				else if (NKCDiveManager.IsBossSectorType(m_NKMDiveSlot.SectorType))
				{
					NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_BOSS_SELECTED, bValue: true);
				}
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_GAUNTLET_SELECTED, bSet);
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_REIMANN_SELECTED, bSet);
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_POINCARE_SELECTED, bSet);
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_EUCLID_SELECTED, bSet);
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_BOSS_SELECTED, bSet);
		}
	}

	public int GetDistance()
	{
		if ((bool)m_NKCDiveGameSectorSet)
		{
			return m_NKCDiveGameSectorSet.GetDistance();
		}
		return 0;
	}

	public Vector3 GetFinalPos()
	{
		if (m_NKCDiveGameSectorSet != null)
		{
			return m_NKCDiveGameSectorSet.transform.localPosition + base.transform.localPosition;
		}
		return base.transform.localPosition;
	}

	private void SetAlphaByReachable()
	{
		bool flag = false;
		if (NKCScenManager.GetScenManager().GetMyUserData() != null)
		{
			NKMDiveGameData diveGameData = NKCScenManager.GetScenManager().GetMyUserData().m_DiveGameData;
			if (diveGameData != null)
			{
				flag = diveGameData.Player.PlayerBase.Distance + 1 == GetDistance();
			}
		}
		if (flag)
		{
			m_CanvasGroup.alpha = 1f;
		}
		else
		{
			m_CanvasGroup.alpha = 0.5f;
		}
	}

	public void SetUI(NKMDiveSlot cNKMDiveSlot)
	{
		if (cNKMDiveSlot != null)
		{
			m_NKMDiveSlot = cNKMDiveSlot;
			SetAlphaByReachable();
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_START, cNKMDiveSlot.SectorType == NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_START || cNKMDiveSlot.SectorType == NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_NONE);
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_GAUNTLET, NKCDiveManager.IsGauntletSectorType(cNKMDiveSlot.SectorType));
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_ICON_GAUNTLET, cNKMDiveSlot.SectorType == NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_GAUNTLET);
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_ICON_GAUNTLET_BIG, cNKMDiveSlot.SectorType == NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_GAUNTLET_HARD);
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_REIMANN, NKCDiveManager.IsReimannSectorType(cNKMDiveSlot.SectorType));
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_ICON_REIMANN, cNKMDiveSlot.SectorType == NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_REIMANN);
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_ICON_REIMANN_BIG, cNKMDiveSlot.SectorType == NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_REIMANN_HARD);
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_POINCARE, NKCDiveManager.IsPoincareSectorType(cNKMDiveSlot.SectorType));
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_ICON_POINCARE, cNKMDiveSlot.SectorType == NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_POINCARE);
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_ICON_POINCARE_BIG, cNKMDiveSlot.SectorType == NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_POINCARE_HARD);
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_EUCLID, NKCDiveManager.IsEuclidSectorType(cNKMDiveSlot.SectorType));
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_ICON_EUCLID, cNKMDiveSlot.SectorType == NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_EUCLID);
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_ICON_EUCLID_BIG, cNKMDiveSlot.SectorType == NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_EUCLID_HARD);
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_BOSS, NKCDiveManager.IsBossSectorType(cNKMDiveSlot.SectorType));
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_ICON_BOSS, cNKMDiveSlot.SectorType == NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_BOSS);
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_ICON_BOSS_BIG, cNKMDiveSlot.SectorType == NKM_DIVE_SECTOR_TYPE.NDST_SECTOR_BOSS_HARD);
			if (NKCDiveManager.IsEuclidSectorType(cNKMDiveSlot.SectorType))
			{
				NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_RANDOM, NKCDiveManager.IsRandomEventType(cNKMDiveSlot.EventType));
				NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_SAFETY, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_LOST_CONTAINER, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_RESCUE_SIGNAL, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_LOST_SHIP, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SECTOR_SLOT_EVENT_REPAIR_KIT, bValue: false);
				NKCUtil.SetGameobjectActive(m_EUCLID_TYPE_NORMAL, cNKMDiveSlot.EventType != NKM_DIVE_EVENT_TYPE.NDET_ARTIFACT && cNKMDiveSlot.EventType != NKM_DIVE_EVENT_TYPE.NDET_REPAIR);
				NKCUtil.SetGameobjectActive(m_EUCLID_TYPE_ARTIFACT, cNKMDiveSlot.EventType == NKM_DIVE_EVENT_TYPE.NDET_ARTIFACT);
				NKCUtil.SetGameobjectActive(m_EUCLID_TYPE_REPAIR, cNKMDiveSlot.EventType == NKM_DIVE_EVENT_TYPE.NDET_REPAIR);
			}
		}
	}

	private void OnClick()
	{
		if (m_dOnClickSector != null)
		{
			m_dOnClickSector(this, bByAuto: false);
		}
	}
}
