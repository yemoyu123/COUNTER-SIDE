using System.Collections.Generic;
using ClientPacket.Contract;
using NKM;
using NKM.Contract2;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Contract;

public class NKCUIContractSelection : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_CONTRACT_V2";

	private const string UI_ASSET_NAME = "NKM_UI_CONTRACT_V2_SELECTION_BEGINNER_RESULT";

	private static NKCUIContractSelection m_Instance;

	[Header("채용 타이틀")]
	public NKCComText TOP_TEXT;

	[Header("채용 유닛 슬롯")]
	public NKCUIUnitSelectListSlot[] NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT;

	[Header("버튼")]
	public NKCUIComStateButton SELECTION_BEGINNER_RESULT_Button01;

	public Image img_SELECTION_BEGINNER_RESULT_Button01;

	public Text SELECTION_BEGINNER_RESULT_Button01_TEXT;

	public NKCComText PROCEEDING_COUNT_TEXT;

	[Space]
	public NKCUIComResourceButton SELECTION_BEGINNER_RESULT_Button02;

	public NKCUIComStateButton UPSIDE_BACK_BUTTON;

	private Color m_ChangeConfirmTextOriginalColor;

	private SelectableContractTemplet m_SelectableContractTemplet;

	private List<NKMUnitData> m_lstUnit = new List<NKMUnitData>();

	private bool m_bWaitForGameObjectActive;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => "채용 후보 리스트";

	public static NKCUIContractSelection Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIContractSelection>("AB_UI_NKM_UI_CONTRACT_V2", "NKM_UI_CONTRACT_V2_SELECTION_BEGINNER_RESULT", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCUIContractSelection>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	private void Init()
	{
		if (NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT != null)
		{
			if (NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT.Length < 10)
			{
				Debug.LogError($"선택슬롯 갯수 확인 필요 - 현재 갯수 : {NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT?.Length}");
			}
			NKCUIUnitSelectListSlot[] nKM_UI_UNIT_SELECT_LIST_UNIT_SLOT = NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT;
			for (int i = 0; i < nKM_UI_UNIT_SELECT_LIST_UNIT_SLOT.Length; i++)
			{
				nKM_UI_UNIT_SELECT_LIST_UNIT_SLOT[i].gameObject.SetActive(value: false);
			}
			NKCUtil.SetBindFunction(UPSIDE_BACK_BUTTON, delegate
			{
				Close();
			});
			if (SELECTION_BEGINNER_RESULT_Button01_TEXT != null)
			{
				m_ChangeConfirmTextOriginalColor = SELECTION_BEGINNER_RESULT_Button01_TEXT.color;
			}
		}
	}

	public override void CloseInternal()
	{
		Object.Destroy(m_Instance.gameObject);
		m_Instance = null;
		if (NKCGameEventManager.IsWaiting())
		{
			NKCGameEventManager.WaitFinished();
		}
	}

	public void Open(NKMSelectableContractState state)
	{
		if (state != null)
		{
			Open(state.contractId, state.unitIdList, state.unitPoolChangeCount);
		}
	}

	public void Open(int contractID, List<int> unitIdList, int poolChangeCount = 0)
	{
		if (unitIdList.Count > 10)
		{
			return;
		}
		m_lstUnit.Clear();
		for (int i = 0; i < unitIdList.Count; i++)
		{
			NKMUnitData nKMUnitData = NKCUtil.MakeDummyUnit(unitIdList[i], 1, 0);
			if (nKMUnitData != null)
			{
				m_lstUnit.Add(nKMUnitData);
			}
		}
		SELECTION_BEGINNER_RESULT_Button02.OnShow(bShow: false);
		m_bWaitForGameObjectActive = true;
		SelectableContractTemplet selectableContractTemplet = SelectableContractTemplet.Find(contractID);
		if (selectableContractTemplet != null)
		{
			m_SelectableContractTemplet = selectableContractTemplet;
			if (m_SelectableContractTemplet.m_RequireItem != null && m_SelectableContractTemplet.m_RequireItem.Count != 0L)
			{
				SELECTION_BEGINNER_RESULT_Button02.OnShow(bShow: true);
				SELECTION_BEGINNER_RESULT_Button02.SetData(m_SelectableContractTemplet.m_RequireItem.ItemId, m_SelectableContractTemplet.m_RequireItem.Count32);
			}
			UpdateUI(poolChangeCount);
		}
		UIOpened();
		CheckTutorial();
	}

	private void Update()
	{
		if (m_bWaitForGameObjectActive && base.gameObject.activeSelf)
		{
			m_bWaitForGameObjectActive = false;
			UpdateUnitSlot(m_lstUnit);
		}
	}

	private void UpdateUnitSlot(List<NKMUnitData> lstUnit)
	{
		if (lstUnit.Count != NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT.Length)
		{
			return;
		}
		for (int i = 0; i < lstUnit.Count; i++)
		{
			if (lstUnit[i] == null)
			{
				Debug.LogError($"NKCUIContractSelection::UpdateUnitSlot unit data is null : {i}");
				continue;
			}
			if (NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT[i] == null)
			{
				Debug.LogError($"NKCUIContractSelection::UpdateUnitSlot slot data is null : {i}");
				continue;
			}
			NKCUtil.SetGameobjectActive(NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT[i].gameObject, bValue: true);
			NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT[i].SetDataForContractSelection(lstUnit[i], bHave: false);
			Animator component = NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT[i].GetComponent<Animator>();
			if (component == null)
			{
				Debug.LogError($"NKCUIContractSelection::UpdateUnitSlot Animator data is null : {i}");
				continue;
			}
			switch (lstUnit[i].GetUnitGrade())
			{
			case NKM_UNIT_GRADE.NUG_SSR:
				component.SetTrigger("SSR");
				break;
			case NKM_UNIT_GRADE.NUG_SR:
				component.SetTrigger("SR");
				break;
			default:
				component.SetTrigger("NONE");
				break;
			}
		}
	}

	private void UpdateUI(int CurCnt = 0)
	{
		NKCUtil.SetLabelText(TOP_TEXT, string.Format(NKCUtilString.GET_STRING_CONTRACT_SELECTION_TITLE, m_SelectableContractTemplet.GetContractName()));
		int unitPoolChangeCount = m_SelectableContractTemplet.m_UnitPoolChangeCount;
		if (PROCEEDING_COUNT_TEXT != null)
		{
			PROCEEDING_COUNT_TEXT.text = $"{CurCnt}/{unitPoolChangeCount}";
		}
		bool flag = true;
		if (m_SelectableContractTemplet.m_RequireItem != null && m_SelectableContractTemplet.m_RequireItem.ItemId != 0)
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null && nKMUserData.m_InventoryData.GetCountMiscItem(m_SelectableContractTemplet.m_RequireItem.ItemId) < m_SelectableContractTemplet.m_RequireItem.Count)
			{
				flag = false;
			}
			if (flag)
			{
				if (m_SelectableContractTemplet.m_RequireItem.Count > 0)
				{
					NKCUtil.SetBindFunction(SELECTION_BEGINNER_RESULT_Button02, SelectableContractConfirmUseItem);
				}
				else
				{
					NKCUtil.SetBindFunction(SELECTION_BEGINNER_RESULT_Button02, SelectableContractConfirm);
				}
			}
			else
			{
				NKCUtil.SetBindFunction(SELECTION_BEGINNER_RESULT_Button02, delegate
				{
					NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(m_SelectableContractTemplet.m_RequireItem.ItemId);
					NKCPopupResourceConfirmBox.Instance.Open(NKCUtilString.GET_STRING_NOTICE, string.Format(NKCUtilString.GET_STRING_SELECTABLE_CONTRACT_NOT_ENOUGH, itemMiscTempletByID.GetItemName()), m_SelectableContractTemplet.m_RequireItem.ItemId, m_SelectableContractTemplet.m_RequireItem.Count32, null);
				});
			}
		}
		else
		{
			NKCUtil.SetBindFunction(SELECTION_BEGINNER_RESULT_Button02, SelectableContractConfirm);
		}
		if (unitPoolChangeCount <= CurCnt)
		{
			SELECTION_BEGINNER_RESULT_Button01?.PointerClick.RemoveAllListeners();
			NKCUtil.SetImageSprite(img_SELECTION_BEGINNER_RESULT_Button01, NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY));
			NKCUtil.SetLabelTextColor(SELECTION_BEGINNER_RESULT_Button01_TEXT, NKCUtil.GetColor("#212122"));
		}
		else
		{
			NKCUtil.SetBindFunction(SELECTION_BEGINNER_RESULT_Button01, ChangeUnitPool);
			NKCUtil.SetImageSprite(img_SELECTION_BEGINNER_RESULT_Button01, NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_YELLOW));
			NKCUtil.SetLabelTextColor(SELECTION_BEGINNER_RESULT_Button01_TEXT, m_ChangeConfirmTextOriginalColor);
		}
	}

	private void ChangeUnitPool()
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_SELECTABLE_CONTRACT_UNIT_POOL_CHANGE_CONFIRM, delegate
		{
			NKCPacketSender.Send_NKMPacket_SELECTABLE_CONTRACT_CHANGE_POOL_REQ(m_SelectableContractTemplet.Key);
		});
	}

	private void SelectableContractConfirmUseItem()
	{
		NKCPopupResourceConfirmBox.Instance.Open(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_SELECTABLE_CONTRACT_USE_ITEM, m_SelectableContractTemplet.m_RequireItem.ItemId, m_SelectableContractTemplet.m_RequireItem.Count32, delegate
		{
			NKCPacketSender.Send_NKMPacket_SELECTABLE_CONTRACT_CONFIRM_REQ(m_SelectableContractTemplet.Key);
		});
	}

	private void SelectableContractConfirm()
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_SELECTABLE_CONTRACT_CONFIRM, delegate
		{
			NKCPacketSender.Send_NKMPacket_SELECTABLE_CONTRACT_CONFIRM_REQ(m_SelectableContractTemplet.Key);
		});
	}

	private void CheckTutorial()
	{
		if (NKCGameEventManager.IsWaiting())
		{
			NKCGameEventManager.WaitFinished();
		}
		else
		{
			NKCTutorialManager.TutorialRequired(TutorialPoint.ContractSelection);
		}
	}
}
