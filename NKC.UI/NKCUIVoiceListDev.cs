using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIVoiceListDev : MonoBehaviour
{
	public struct VoiceItem
	{
		public int Index;

		public Text FileName;

		public NKCUIComStateButton Button;
	}

	[SerializeField]
	private Transform trUnitContent;

	[SerializeField]
	private NKCDeckViewUnitSelectListSlot slotPrefab;

	[SerializeField]
	private Transform trVoiceContent;

	[SerializeField]
	private GameObject itemPrefab;

	private Dictionary<int, NKCDeckViewUnitSelectListSlot> dicUnit = new Dictionary<int, NKCDeckViewUnitSelectListSlot>();

	private List<VoiceItem> listVoiceItem = new List<VoiceItem>();

	private string currentUnitStrID = string.Empty;

	public static NKCUIVoiceListDev Init(GameObject voiceListUI)
	{
		NKCUIVoiceListDev component = voiceListUI.GetComponent<NKCUIVoiceListDev>();
		component.slotPrefab.gameObject.SetActive(value: false);
		component.itemPrefab.SetActive(value: false);
		component.InitItem();
		component.InitUnitList();
		return component;
	}

	public void Open()
	{
		NKCSoundManager.StopMusic();
	}

	public void Close()
	{
	}

	private void InitUnitList()
	{
		List<NKMUnitTempletBase> listNKMUnitTempletBaseForUnit = NKMUnitTempletBase.Get_listNKMUnitTempletBaseForUnit();
		RemoveNoNeedUnitTypeForSingleMode(listNKMUnitTempletBaseForUnit);
		int count = listNKMUnitTempletBaseForUnit.Count;
		for (int i = 0; i < count; i++)
		{
			NKCDeckViewUnitSelectListSlot nKCDeckViewUnitSelectListSlot = Object.Instantiate(slotPrefab);
			nKCDeckViewUnitSelectListSlot.Init();
			nKCDeckViewUnitSelectListSlot.gameObject.SetActive(value: true);
			nKCDeckViewUnitSelectListSlot.transform.localScale = Vector3.one;
			nKCDeckViewUnitSelectListSlot.transform.SetParent(trUnitContent);
			nKCDeckViewUnitSelectListSlot.SetData(listNKMUnitTempletBaseForUnit[i], 1, 0, bEnableLayoutElement: false, OnSelectThisSlot);
			dicUnit.Add(listNKMUnitTempletBaseForUnit[i].m_UnitID, nKCDeckViewUnitSelectListSlot);
		}
	}

	private void InitItem()
	{
		List<NKCVoiceTemplet> templets = NKCUIVoiceManager.GetTemplets();
		int count = templets.Count;
		for (int i = 0; i < count; i++)
		{
			GameObject obj = Object.Instantiate(itemPrefab);
			obj.SetActive(value: true);
			obj.transform.SetParent(trVoiceContent);
			obj.transform.Find("Type/text").GetComponent<Text>().text = templets[i].Type.ToString().Substring(3);
			Text component = obj.transform.Find("FileName/text").GetComponent<Text>();
			component.text = "";
			NKCUIComStateButton componentInChildren = obj.GetComponentInChildren<NKCUIComStateButton>();
			VoiceItem item = new VoiceItem
			{
				Index = i,
				FileName = component,
				Button = componentInChildren
			};
			componentInChildren.PointerClick.AddListener(delegate
			{
				OnClickPlay(item);
			});
			componentInChildren.gameObject.SetActive(value: false);
			listVoiceItem.Add(item);
		}
	}

	private void OnSelectThisSlot(NKMUnitData unitData, NKMUnitTempletBase unitTempletBase, NKMDeckIndex deckIndex, NKCUnitSortSystem.eUnitState slotState, NKCUIUnitSelectList.eUnitSlotSelectState unitSlotSelectState)
	{
		Debug.Log("unit : " + unitTempletBase.m_UnitStrID);
		currentUnitStrID = unitTempletBase.m_UnitStrID;
		SelectUnit(unitTempletBase.m_UnitID);
		SetVoiceList(currentUnitStrID);
	}

	private void SetVoiceList(string unitStrID)
	{
		List<NKCVoiceTemplet> templets = NKCUIVoiceManager.GetTemplets();
		for (int i = 0; i < listVoiceItem.Count; i++)
		{
			VoiceItem voiceItem = listVoiceItem[i];
			NKCVoiceTemplet nKCVoiceTemplet = templets[voiceItem.Index];
			if (NKCUIVoiceManager.CheckAsset(unitStrID, 0, nKCVoiceTemplet.FileName, VOICE_BUNDLE.UNIT))
			{
				voiceItem.FileName.text = nKCVoiceTemplet.FileName;
				voiceItem.Button.gameObject.SetActive(value: true);
			}
			else
			{
				voiceItem.FileName.text = "없음";
				voiceItem.Button.gameObject.SetActive(value: false);
			}
		}
	}

	private void OnClickPlay(VoiceItem item)
	{
		if (!string.IsNullOrEmpty(currentUnitStrID))
		{
			NKCVoiceTemplet nKCVoiceTemplet = NKCUIVoiceManager.GetTemplets()[item.Index];
			NKCUIVoiceManager.PlayOnUI(currentUnitStrID, 0, nKCVoiceTemplet.FileName, nKCVoiceTemplet.Volume, VOICE_BUNDLE.UNIT);
		}
	}

	private void SelectUnit(int unitID)
	{
		foreach (KeyValuePair<int, NKCDeckViewUnitSelectListSlot> item in dicUnit)
		{
			if (item.Key != unitID)
			{
				item.Value.SetSlotSelectState(NKCUIUnitSelectList.eUnitSlotSelectState.NONE);
			}
			else
			{
				item.Value.SetSlotSelectState(NKCUIUnitSelectList.eUnitSlotSelectState.SELECTED);
			}
		}
	}

	private void RemoveNoNeedUnitTypeForSingleMode(List<NKMUnitTempletBase> listNKMUnitTempletBase)
	{
		if (listNKMUnitTempletBase == null)
		{
			return;
		}
		int num = 0;
		for (num = 0; num < listNKMUnitTempletBase.Count; num++)
		{
			if (listNKMUnitTempletBase[num].m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_TRAINER)
			{
				listNKMUnitTempletBase.RemoveAt(num);
				num--;
			}
		}
	}
}
