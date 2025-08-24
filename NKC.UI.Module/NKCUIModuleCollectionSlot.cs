using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Module;

public class NKCUIModuleCollectionSlot : MonoBehaviour
{
	public delegate void OnClickSlot(NKCUISlot.SlotData slotData, bool bLocked);

	public Image m_imgBG;

	public Image m_imgRank;

	public Image m_imgUnit;

	public GameObject m_objNotOwned;

	public GameObject m_objAwaken;

	[Header("\ufffd\ufffd\ufffd\ued75 \ufffd\u0339\ufffd\ufffd\ufffd")]
	public Sprite m_spRarityN;

	public Sprite m_spRarityR;

	public Sprite m_spRaritySR;

	public Sprite m_spRaritySSR;

	private NKCAssetInstanceData m_NKCAssetInstanceData;

	[Header("\ufffd\ufffdư")]
	public NKCUIComStateButton m_csbtnOK;

	private OnClickSlot m_dOnClick;

	private NKCUISlot.SlotData m_slotData;

	public void Init()
	{
		NKCUtil.SetBindFunction(m_csbtnOK, OnClick);
	}

	public void SetData(NKCUISlot.SlotData slotData, bool awaken, OnClickSlot onClick = null)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(slotData.ID);
		NKM_UNIT_GRADE unitBackground = NKM_UNIT_GRADE.NUG_N;
		if (unitTempletBase != null)
		{
			unitBackground = unitTempletBase.m_NKM_UNIT_GRADE;
		}
		SetUnitBackground(unitBackground);
		NKCUtil.SetGameobjectActive(m_objAwaken, awaken);
		if (m_imgUnit != null)
		{
			m_imgUnit.sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, unitTempletBase);
		}
		m_slotData = slotData;
		m_dOnClick = onClick;
	}

	public void SetOwnState(bool owned)
	{
		NKCUtil.SetGameobjectActive(m_objNotOwned, !owned);
	}

	private void SetUnitBackground(NKM_UNIT_GRADE grade)
	{
		if (!(m_imgRank == null))
		{
			switch (grade)
			{
			case NKM_UNIT_GRADE.NUG_N:
				m_imgRank.sprite = m_spRarityN;
				break;
			case NKM_UNIT_GRADE.NUG_R:
				m_imgRank.sprite = m_spRarityR;
				break;
			case NKM_UNIT_GRADE.NUG_SR:
				m_imgRank.sprite = m_spRaritySR;
				break;
			case NKM_UNIT_GRADE.NUG_SSR:
				m_imgRank.sprite = m_spRaritySSR;
				break;
			default:
				Debug.LogError("Unit BG undefined");
				m_imgRank.sprite = m_spRarityN;
				break;
			}
		}
	}

	private void OnDestroy()
	{
		NKCAssetResourceManager.CloseInstance(m_NKCAssetInstanceData);
	}

	private void OnClick()
	{
		m_dOnClick?.Invoke(m_slotData, bLocked: false);
	}
}
