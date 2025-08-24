using System.Collections;
using System.Collections.Generic;
using ClientPacket.Common;
using DG.Tweening;
using NKC.UI.Result;
using NKM;
using NKM.Guild;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI;

public class NKCPopupMessageToastSimple : NKCUIBase
{
	public struct MessageData
	{
		public int enchantModuleId;

		public Sprite sprite;

		public string name;

		public long count;
	}

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_MESSAGE_TOAST";

	private static NKCPopupMessageToastSimple m_Instance;

	public int maxCount;

	public float spacing;

	public float slotMovingUpDuration;

	public RectTransform contentRect;

	public Transform group;

	private List<NKCUIMessageToastSimpleSlot> m_lstMsgSlot = new List<NKCUIMessageToastSimpleSlot>();

	private List<MessageData> m_queueList = new List<MessageData>();

	private Coroutine m_coroutine;

	private float slotHeight;

	private float[] slotYPosition;

	private bool m_rewardFromBattle;

	public static NKCPopupMessageToastSimple Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupMessageToastSimple>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX", "NKM_UI_POPUP_MESSAGE_TOAST", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCPopupMessageToastSimple>();
				m_Instance?.Init();
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

	public override eMenutype eUIType => eMenutype.Overlay;

	public override string MenuName => "Toast Message Simple";

	public override NKCUIManager.eUIUnloadFlag UnloadFlag => NKCUIManager.eUIUnloadFlag.ONLY_MEMORY_SHORTAGE;

	public bool RewardFromBattle
	{
		get
		{
			return m_rewardFromBattle;
		}
		set
		{
			m_rewardFromBattle = value;
		}
	}

	private static void CleanupInstance()
	{
		m_Instance.Release();
		m_Instance = null;
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.RewardFromBattle)
		{
			m_Instance.RewardFromBattle = false;
		}
		else if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private void Init()
	{
		if (group.childCount <= 0)
		{
			GameObject orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<GameObject>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX", "NKM_UI_POPUP_MESSAGE_TOAST_SLOT");
			if (!(orLoadAssetResource != null))
			{
				Debug.LogWarning("NKM_UI_POPUP_MESSAGE_TOAST_SLOT is not created");
				return;
			}
			RectTransform component = Object.Instantiate(orLoadAssetResource, group).GetComponent<RectTransform>();
			AddSlotObjectToGroup(component);
		}
		int childCount = group.childCount;
		for (int i = 0; i < childCount; i++)
		{
			group.GetChild(i).gameObject.SetActive(value: false);
			m_lstMsgSlot.Add(group.GetChild(i).GetComponent<NKCUIMessageToastSimpleSlot>());
		}
		int num = maxCount - group.childCount + 1;
		for (int j = 0; j < num; j++)
		{
			RectTransform component2 = Object.Instantiate(group.GetChild(0), group).GetComponent<RectTransform>();
			if (!(component2 == null))
			{
				AddSlotObjectToGroup(component2);
				component2.gameObject.SetActive(value: false);
				m_lstMsgSlot.Add(component2.GetComponent<NKCUIMessageToastSimpleSlot>());
			}
		}
		if (group.childCount > 0)
		{
			slotHeight = group.GetChild(0).GetComponent<RectTransform>().GetHeight();
		}
		if (contentRect != null)
		{
			contentRect.SetHeight(slotHeight + (slotHeight + spacing) * (float)(maxCount - 1));
		}
		childCount = m_lstMsgSlot.Count;
		slotYPosition = new float[childCount];
		for (int k = 0; k < childCount; k++)
		{
			m_lstMsgSlot[k].ResetSlot();
			slotYPosition[k] = m_lstMsgSlot[k].transform.position.y + slotHeight;
		}
	}

	public override void CloseInternal()
	{
		if (m_coroutine != null)
		{
			StopCoroutine(m_coroutine);
			m_coroutine = null;
		}
		m_queueList.Clear();
		int count = m_lstMsgSlot.Count;
		for (int i = 0; i < count; i++)
		{
			m_lstMsgSlot[i].transform.DOKill();
			m_lstMsgSlot[i].gameObject.SetActive(value: false);
			m_lstMsgSlot[i].ResetSlot();
			Vector3 position = m_lstMsgSlot[i].transform.position;
			position.y = slotYPosition[i] - slotHeight;
			m_lstMsgSlot[i].transform.position = position;
		}
		m_rewardFromBattle = false;
		base.gameObject.SetActive(value: false);
	}

	public void Open(NKMRewardData rewardData, NKMAdditionalReward additionalReward, NKCUIResult.OnClose onOpen = null)
	{
		List<NKCUISlot.SlotData> slotDataList = NKCUISlot.MakeSlotDataListFromReward(rewardData);
		if (additionalReward != null)
		{
			NKCUISlot.MakeSlotDataListFromReward(additionalReward).ForEach(delegate(NKCUISlot.SlotData e)
			{
				slotDataList.Add(e);
			});
		}
		int count = slotDataList.Count;
		for (int num = 0; num < count; num++)
		{
			switch (slotDataList[num].eType)
			{
			case NKCUISlot.eSlotMode.Unit:
			case NKCUISlot.eSlotMode.UnitCount:
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(slotDataList[num].ID);
				MessageData item6 = default(MessageData);
				if (unitTempletBase != null)
				{
					item6.sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, unitTempletBase);
				}
				item6.name = NKCUISlot.GetName(slotDataList[num]);
				item6.count = 1L;
				m_queueList.Add(item6);
				break;
			}
			case NKCUISlot.eSlotMode.ItemMisc:
			{
				NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(slotDataList[num].ID);
				MessageData item9 = default(MessageData);
				if (itemMiscTempletByID != null)
				{
					item9.sprite = NKCResourceUtility.GetOrLoadMiscItemSmallIcon(itemMiscTempletByID);
				}
				item9.name = NKCUISlot.GetName(slotDataList[num]);
				item9.count = slotDataList[num].Count;
				m_queueList.Add(item9);
				break;
			}
			case NKCUISlot.eSlotMode.Equip:
			case NKCUISlot.eSlotMode.EquipCount:
			{
				NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(slotDataList[num].ID);
				MessageData item7 = default(MessageData);
				if (equipTemplet != null)
				{
					item7.sprite = NKCResourceUtility.GetOrLoadEquipIcon(equipTemplet);
					if (equipTemplet.m_EquipUnitStyleType == NKM_UNIT_STYLE_TYPE.NUST_ENCHANT)
					{
						int num2 = m_queueList.FindIndex((MessageData e) => e.enchantModuleId == equipTemplet.m_ItemEquipID);
						if (num2 >= 0 && num2 < m_queueList.Count)
						{
							item7 = m_queueList[num2];
							item7.count++;
							m_queueList[num2] = item7;
							break;
						}
						item7.enchantModuleId = equipTemplet.m_ItemEquipID;
					}
				}
				item7.name = NKCUISlot.GetName(slotDataList[num]);
				item7.count = 1L;
				m_queueList.Add(item7);
				break;
			}
			case NKCUISlot.eSlotMode.Mold:
			{
				NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(slotDataList[num].ID);
				MessageData item4 = default(MessageData);
				if (itemMoldTempletByID != null)
				{
					item4.sprite = NKCResourceUtility.GetOrLoadMoldIcon(itemMoldTempletByID);
				}
				item4.name = NKCUISlot.GetName(slotDataList[num]);
				item4.count = slotDataList[num].Count;
				m_queueList.Add(item4);
				break;
			}
			case NKCUISlot.eSlotMode.DiveArtifact:
			{
				NKMDiveArtifactTemplet nKMDiveArtifactTemplet = NKMDiveArtifactTemplet.Find(slotDataList[num].ID);
				MessageData item3 = default(MessageData);
				if (nKMDiveArtifactTemplet != null)
				{
					item3.sprite = NKCResourceUtility.GetOrLoadDiveArtifactIcon(nKMDiveArtifactTemplet);
				}
				item3.name = NKCUISlot.GetName(slotDataList[num]);
				item3.count = slotDataList[num].Count;
				m_queueList.Add(item3);
				break;
			}
			case NKCUISlot.eSlotMode.Buff:
			{
				NKMCompanyBuffTemplet companyBuffTemplet = NKMCompanyBuffManager.GetCompanyBuffTemplet(slotDataList[num].ID);
				MessageData item8 = default(MessageData);
				if (companyBuffTemplet != null)
				{
					item8.sprite = NKCResourceUtility.GetOrLoadBuffIconForItemPopup(companyBuffTemplet);
				}
				item8.name = NKCUISlot.GetName(slotDataList[num]);
				item8.count = 1L;
				m_queueList.Add(item8);
				break;
			}
			case NKCUISlot.eSlotMode.Emoticon:
			{
				NKMEmoticonTemplet nKMEmoticonTemplet = NKMEmoticonTemplet.Find(slotDataList[num].ID);
				MessageData item5 = default(MessageData);
				if (nKMEmoticonTemplet != null)
				{
					item5.sprite = NKCResourceUtility.GetOrLoadEmoticonIcon(nKMEmoticonTemplet);
				}
				item5.name = NKCUISlot.GetName(slotDataList[num]);
				item5.count = slotDataList[num].Count;
				m_queueList.Add(item5);
				break;
			}
			case NKCUISlot.eSlotMode.GuildArtifact:
			{
				GuildDungeonArtifactTemplet artifactTemplet = GuildDungeonTempletManager.GetArtifactTemplet(slotDataList[num].ID);
				MessageData item2 = default(MessageData);
				if (artifactTemplet != null)
				{
					item2.sprite = NKCResourceUtility.GetOrLoadGuildArtifactIcon(artifactTemplet);
				}
				item2.name = NKCUISlot.GetName(slotDataList[num]);
				item2.count = slotDataList[num].Count;
				m_queueList.Add(item2);
				break;
			}
			default:
			{
				MessageData item = new MessageData
				{
					name = "Unidentified Data"
				};
				m_queueList.Add(item);
				break;
			}
			}
		}
		if (m_queueList.Count > 0)
		{
			if (m_coroutine != null)
			{
				SetMessageToSlot(addedSlot: true);
			}
			base.gameObject.SetActive(value: true);
			if (m_coroutine == null)
			{
				m_coroutine = StartCoroutine(IUpdateMessageSlot());
			}
		}
		onOpen?.Invoke();
		if (!base.IsOpen)
		{
			UIOpened();
		}
	}

	private void AddSlotObjectToGroup(RectTransform slotRect)
	{
		if (!(slotRect == null))
		{
			slotRect.SetParent(group);
			Vector3 localPosition = slotRect.localPosition;
			float height = slotRect.GetHeight();
			localPosition.y = (0f - height) * (1f - slotRect.pivot.y) - (height + spacing) * (float)(group.childCount - 1);
			slotRect.localPosition = localPosition;
		}
	}

	private void SetFirstSlotToLastSibling()
	{
		Vector3 position = group.GetChild(0).position;
		position.y = slotYPosition[slotYPosition.Length - 1] - slotHeight;
		group.GetChild(0).position = position;
		group.GetChild(0).SetAsLastSibling();
	}

	private void SetMessageToSlot(bool addedSlot)
	{
		int a = Mathf.Min(maxCount + 1, group.childCount);
		a = Mathf.Min(a, m_lstMsgSlot.Count);
		for (int i = 0; i < a; i++)
		{
			if (!m_lstMsgSlot[i].gameObject.activeSelf)
			{
				if (m_queueList.Count <= 0)
				{
					break;
				}
				MessageData data = m_queueList[0];
				m_queueList.RemoveAt(0);
				m_lstMsgSlot[i].SetData(data);
				m_lstMsgSlot[i].gameObject.SetActive(value: true);
				if (addedSlot && i >= maxCount)
				{
					m_lstMsgSlot[i].PlayIdleAni();
				}
			}
		}
	}

	private IEnumerator IUpdateMessageSlot()
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			m_rewardFromBattle = true;
		}
		while (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			yield return null;
		}
		SetMessageToSlot(addedSlot: false);
		if (m_lstMsgSlot.Count > 0)
		{
			while (m_lstMsgSlot[0].gameObject.activeSelf)
			{
				if (m_lstMsgSlot[0].m_animator.GetCurrentAnimatorStateInfo(0).IsName("NKM_UI_POPUP_MESSAGE_TOAST_OUTRO") && m_lstMsgSlot[0].m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
				{
					int moveComplete = 0;
					int slotCount = m_lstMsgSlot.Count;
					for (int i = 0; i < slotCount; i++)
					{
						m_lstMsgSlot[i].transform.DOMoveY(slotYPosition[i], slotMovingUpDuration).OnComplete(delegate
						{
							int num2 = moveComplete + 1;
							moveComplete = num2;
						});
					}
					while (moveComplete < slotCount)
					{
						yield return null;
					}
					m_lstMsgSlot[0].gameObject.SetActive(value: false);
					m_lstMsgSlot[0].ResetSlot();
					SetFirstSlotToLastSibling();
					NKCUIMessageToastSimpleSlot value = m_lstMsgSlot[0];
					for (int num = 0; num < slotCount - 1; num++)
					{
						m_lstMsgSlot[num] = m_lstMsgSlot[num + 1];
					}
					m_lstMsgSlot[slotCount - 1] = value;
					if (m_queueList.Count > 0)
					{
						SetMessageToSlot(addedSlot: true);
					}
				}
				yield return null;
			}
		}
		Close();
		m_coroutine = null;
	}

	private void Release()
	{
		if (m_coroutine != null)
		{
			StopCoroutine(m_coroutine);
			m_coroutine = null;
		}
		m_queueList?.Clear();
		m_queueList = null;
		m_lstMsgSlot?.Clear();
		m_lstMsgSlot = null;
		slotYPosition = null;
	}
}
