using System.Collections;
using System.Collections.Generic;
using ClientPacket.Common;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC.UI.Result;

public class NKCUIPopupModuleResult : NKCUIBase
{
	private static NKCUIManager.LoadedUIData m_loadedUIData;

	public List<NKCUISlot> m_lstResult = new List<NKCUISlot>();

	public NKCUIComStateButton[] m_csbtnBack;

	public Animator m_Ani;

	[Header("Misc \ufffd\ufffd\ufffd\ufffd")]
	public GameObject m_objMiscReward;

	public Image m_imgMiscRewardIcon;

	public Text m_lbMiscRewardDesc;

	private UnityAction dClose;

	private const string TRIGGER_INTRO = "INTRO";

	private const string TRIGGER_OUTRO = "OUTRO";

	private const string TRIGGER_SKIP = "SKIP";

	public override string MenuName => "NKCUIPopupModuleResult \ufffd\ufffd\ufffdâ";

	public override eMenutype eUIType => eMenutype.Popup;

	public static void CheckInstanceAndClose()
	{
		if (m_loadedUIData != null)
		{
			m_loadedUIData.CloseInstance();
			m_loadedUIData = null;
		}
	}

	public static NKCUIPopupModuleResult MakeInstance(string bundleName, string assetName)
	{
		NKCUIPopupModuleResult instance = NKCUIManager.OpenNewInstance<NKCUIPopupModuleResult>(bundleName, assetName, NKCUIManager.eUIBaseRect.UIOverlay, null).GetInstance<NKCUIPopupModuleResult>();
		instance.Init();
		return instance;
	}

	public override void CloseInternal()
	{
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void Init()
	{
		foreach (NKCUISlot item in m_lstResult)
		{
			item.Init();
		}
		NKCUIComStateButton[] csbtnBack = m_csbtnBack;
		for (int i = 0; i < csbtnBack.Length; i++)
		{
			NKCUtil.SetBindFunction(csbtnBack[i], OnClose);
		}
	}

	public override void OnBackButton()
	{
		OnClose();
	}

	public void Open(NKMRewardData reward, UnityAction closeCallBack)
	{
		dClose = closeCallBack;
		NKCUtil.SetGameobjectActive(m_objMiscReward, bValue: false);
		if (reward.MiscItemDataList != null && reward.MiscItemDataList.Count > 0)
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(reward.MiscItemDataList[0].ItemID);
			if (itemMiscTempletByID != null)
			{
				string msg = string.Format(NKCUtilString.GET_STRING_MODULE_CONTRACT_MILEAGE_POINT_DESC_02, itemMiscTempletByID.GetItemName(), reward.MiscItemDataList[0].TotalCount);
				NKCUtil.SetLabelText(m_lbMiscRewardDesc, msg);
				Sprite orLoadMiscItemSmallIcon = NKCResourceUtility.GetOrLoadMiscItemSmallIcon(itemMiscTempletByID);
				NKCUtil.SetImageSprite(m_imgMiscRewardIcon, orLoadMiscItemSmallIcon);
				NKCUtil.SetGameobjectActive(m_objMiscReward, bValue: true);
			}
		}
		List<NKMUnitData> unitDataList = reward.UnitDataList;
		unitDataList.Sort(delegate(NKMUnitData x, NKMUnitData y)
		{
			if (x.GetUnitGrade() < y.GetUnitGrade())
			{
				return 1;
			}
			return (x.GetUnitGrade() > y.GetUnitGrade()) ? (-1) : 0;
		});
		for (int num = 0; num < m_lstResult.Count; num++)
		{
			if (m_lstResult[num] == null)
			{
				continue;
			}
			if (num >= unitDataList.Count)
			{
				NKCUtil.SetGameobjectActive(m_lstResult[num], bValue: false);
				continue;
			}
			NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeUnitData(unitDataList[num]);
			if (slotData != null)
			{
				m_lstResult[num].SetData(slotData);
			}
			NKCUtil.SetGameobjectActive(m_lstResult[num], bValue: true);
		}
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		OnPlayAni("INTRO");
		UIOpened();
	}

	public void Open(NKMRewardData rewardData, NKMAdditionalReward additionalReward, UnityAction closeCallBack)
	{
		dClose = closeCallBack;
		List<NKCUISlot.SlotData> list = new List<NKCUISlot.SlotData>();
		if (rewardData != null)
		{
			list.AddRange(NKCUISlot.MakeSlotDataListFromReward(rewardData));
		}
		if (additionalReward != null)
		{
			list.AddRange(NKCUISlot.MakeSlotDataListFromReward(additionalReward));
		}
		for (int i = 0; i < m_lstResult.Count; i++)
		{
			if (m_lstResult[i] == null)
			{
				continue;
			}
			if (i >= list.Count)
			{
				NKCUtil.SetGameobjectActive(m_lstResult[i], bValue: false);
				continue;
			}
			if (list[i] != null)
			{
				m_lstResult[i].SetData(list[i]);
			}
			NKCUtil.SetGameobjectActive(m_lstResult[i], bValue: true);
		}
		NKCUtil.SetGameobjectActive(m_objMiscReward, bValue: false);
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		OnPlayAni("INTRO");
		UIOpened();
	}

	private void OnClose()
	{
		OnPlayAni("OUTRO");
	}

	private void OnPlayAni(string trigger)
	{
		m_Ani.SetTrigger(trigger);
		if (string.Equals(trigger, "OUTRO"))
		{
			StartCoroutine(PlayCoroutine());
		}
	}

	private bool AnimatorIsPlaying()
	{
		return 1f > m_Ani.GetCurrentAnimatorStateInfo(0).normalizedTime;
	}

	private IEnumerator PlayCoroutine()
	{
		yield return new WaitForSeconds(0.3f);
		if (AnimatorIsPlaying())
		{
			yield return null;
		}
		OnExit();
	}

	private void OnExit()
	{
		dClose?.Invoke();
		Close();
		StopAllCoroutines();
	}

	public override void OnHotkeyHold(HotkeyEventType hotkey)
	{
		if (hotkey == HotkeyEventType.Skip)
		{
			if (!NKCUIManager.IsTopmostUI(this))
			{
				return;
			}
			m_Ani.SetTrigger("SKIP");
		}
		if (HotkeyEventType.Confirm == hotkey)
		{
			OnClose();
		}
	}
}
