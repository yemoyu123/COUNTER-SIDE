using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Result;

public class NKCUIWRRewardSlot : MonoBehaviour
{
	public NKCUISlot m_AB_ICON_SLOT;

	public Text m_AB_ICON_SLOT_RESULT_TEXT;

	public Animator m_Animator;

	public GameObject m_AB_ICON_SLOT_WF_RESULT_CONTAINER;

	public GameObject m_AB_ICON_SLOT_WF_RESULT_FIRSTCLEAR;

	public Text m_lbFirstClear;

	public GameObject m_AB_ICON_SLOT_WF_RESULT_ARTIFACT;

	public GameObject m_AB_ICON_SLOT_WF_RESULT_DIVE_STORM;

	public GameObject m_AB_ICON_SLOT_WF_RESULT_MULTIPLY_REWARD;

	public GameObject m_AB_ICON_SLOT_WF_RESULT_KILL_REWARD;

	public const float WHITE_IN_ANI_TIME = 5f / 6f;

	private float m_fElapsedTime;

	private int m_seqToAni;

	private bool m_bReservedAni;

	private NKCAssetInstanceData m_instance;

	public static NKCUIWRRewardSlot GetNewInstance(Transform parent)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_RESULT", "AB_ICON_SLOT_WF_RESULT");
		NKCUIWRRewardSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIWRRewardSlot>();
		if (component == null)
		{
			Debug.LogError("NKCUIWRRewardSlot Prefab null!");
			return null;
		}
		component.m_instance = nKCAssetInstanceData;
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
		component.m_AB_ICON_SLOT.Init();
		component.gameObject.SetActive(value: false);
		return component;
	}

	public void ManualUpdate()
	{
		if (m_bReservedAni)
		{
			m_fElapsedTime += Time.deltaTime;
			if (m_fElapsedTime > (float)m_seqToAni * (2f / 15f))
			{
				NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
				m_Animator.Play("AB_ICON_SLOT_RESULT_INTRO");
				m_bReservedAni = false;
			}
		}
	}

	public void InvalidAni()
	{
		m_bReservedAni = false;
	}

	public void SetUI(NKCUISlot.SlotData slotData, int seqToAni)
	{
		m_seqToAni = seqToAni;
		m_bReservedAni = true;
		m_fElapsedTime = 0f;
		bool bShowNumber = slotData.eType == NKCUISlot.eSlotMode.ItemMisc || slotData.eType == NKCUISlot.eSlotMode.Mold || slotData.eType == NKCUISlot.eSlotMode.EquipCount;
		m_AB_ICON_SLOT.SetData(slotData, bShowName: false, bShowNumber, bEnableLayoutElement: true, null);
		m_AB_ICON_SLOT.SetBonusRate(slotData.BonusRate);
		m_AB_ICON_SLOT.SetOnClickAction(default(NKCUISlot.SlotClickType));
		m_AB_ICON_SLOT_RESULT_TEXT.text = NKCUISlot.GetName(slotData.eType, slotData.ID);
	}

	public void SetFirstMark(bool bFirst)
	{
		NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT_WF_RESULT_FIRSTCLEAR, bFirst);
		NKCUtil.SetLabelText(m_lbFirstClear, NKCUtilString.GET_STRING_REWARD_FIRST_CLEAR);
	}

	public void SetFirstAllClearMark(bool bClear)
	{
		NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT_WF_RESULT_FIRSTCLEAR, bClear);
		NKCUtil.SetLabelText(m_lbFirstClear, NKCUtilString.GET_STRING_WARFARE_FIRST_ALL_CLEAR);
	}

	public void SetStarAllClearMark(bool bAllStarClear)
	{
		NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT_WF_RESULT_FIRSTCLEAR, bAllStarClear);
		NKCUtil.SetLabelText(m_lbFirstClear, NKCUtilString.GET_STRING_REWARD_FIRST_STAR_ALL_CLEAR);
	}

	public void SetContainerMark(bool bContainer)
	{
		NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT_WF_RESULT_CONTAINER, bContainer);
	}

	public void SetArtifactMark(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT_WF_RESULT_ARTIFACT, bSet);
	}

	public void SetDiveStormMark(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT_WF_RESULT_DIVE_STORM, bSet);
	}

	public void SetChanceUpMark(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT_WF_RESULT_FIRSTCLEAR, bSet);
		NKCUtil.SetLabelText(m_lbFirstClear, NKCUtilString.GET_STRING_REWARD_CHANCE_UP);
	}

	public void SetMultiplyMark(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT_WF_RESULT_MULTIPLY_REWARD, bSet);
	}

	public void SetKillRewardMark(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT_WF_RESULT_KILL_REWARD, bSet);
	}

	public void Close()
	{
		NKCAssetResourceManager.CloseInstance(m_instance);
		m_instance = null;
	}
}
