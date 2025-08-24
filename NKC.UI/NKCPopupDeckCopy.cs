using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Cs.Engine.Network.Buffer.Detail;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupDeckCopy : NKCUIBase
{
	private enum StateType
	{
		ST_INPUT,
		ST_CONFIRM
	}

	private struct strOpenData
	{
		public NKMDeckIndex m_DeckIndex;

		public long m_ShipUID;

		public long m_OperUID;

		public List<long> m_lstUnitUIDs;

		public int leaderIndex;

		public bool IsDeckIndexData => m_DeckIndex != NKMDeckIndex.None;

		public bool IsRaidDeck
		{
			get
			{
				if (m_DeckIndex != NKMDeckIndex.None)
				{
					return m_DeckIndex.m_eDeckType == NKM_DECK_TYPE.NDT_RAID;
				}
				return false;
			}
		}

		public void SetDeckIndexData(NKMDeckIndex deckIndex)
		{
			m_DeckIndex = deckIndex;
		}

		public void SetDeckUnitData(long shipUID, long operUID, List<long> lstUnitUIDs, int leaderIdx)
		{
			m_ShipUID = shipUID;
			m_OperUID = operUID;
			m_lstUnitUIDs = lstUnitUIDs;
			leaderIndex = leaderIdx;
			m_DeckIndex = NKMDeckIndex.None;
		}
	}

	private static readonly ulong[] MaskList = new ulong[4] { 16440842866624161704uL, 16475857767454201900uL, 14260620119722636881uL, 14123698925096760729uL };

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_COPY_SQUAD";

	private static NKCPopupDeckCopy m_Instance;

	public EventTrigger m_etBG;

	public GameObject m_objCodeInput;

	public InputField m_IFDeckCopyCode;

	public NKCUIComStateButton m_csbtnOK;

	public NKCUIComStateButton m_csbtnCancel;

	[Header("\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd")]
	public GameObject m_objDeckSquadConfirm;

	public Image m_imgBeforeDeckShip;

	public NKCUIOperatorDeckSlot m_BeforeDeckOperDeckSlot;

	public List<NKCDeckViewUnitSlot> m_lstBeforeDeckUnitslots;

	public Image m_imgAfterDeckShip;

	public NKCUIOperatorDeckSlot m_AfterDeckOperDeckSlot;

	public List<NKCDeckViewUnitSlot> m_lstAfterDeckUnitslots;

	[Header("\ufffd\ufffd\ufffd\u0335\ufffd \ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd")]
	public GameObject m_objRaidSquadConfirm;

	public Image m_imgBeforeRaidDeckShip;

	public NKCUIOperatorDeckSlot m_BeforeRaidDeckOperDeckSlot;

	public List<NKCDeckViewUnitSlot> m_lstBeforeRaidDeckUnitslots;

	public Image m_imgAfterRaidDeckShip;

	public NKCUIOperatorDeckSlot m_AfterDeckOperRaidDeckSlot;

	public List<NKCDeckViewUnitSlot> m_lstAfterRaidDeckUnitslots;

	public GameObject m_objMissMatchMakeDeck;

	private StateType m_curStateType;

	private NKMDeckIndex m_curDeckIndex;

	private strOpenData m_OpenData;

	private List<int> m_lstCopyDeckDatas = new List<int>();

	private List<long> m_lstApplyCopyDackData = new List<long>();

	private const int DECK_DATA_SHIP = 0;

	private const int DECK_DATA_OPER = 1;

	private const int DECK_DATA_MAX_COUNT = 11;

	private const int DECK_DATA_MAX_COUNT_RAID = 19;

	public static NKCPopupDeckCopy Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupDeckCopy>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX", "NKM_UI_POPUP_COPY_SQUAD", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupDeckCopy>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "Deck Copy";

	public static void MakeDeckCopyCode(ref NKMDeckIndex deckIndex)
	{
		NKMArmyData nKMArmyData = NKCScenManager.CurrentUserData()?.m_ArmyData;
		if (nKMArmyData != null)
		{
			List<long> unitList = new List<long>();
			nKMArmyData.GetDeckList(deckIndex.m_eDeckType, deckIndex.m_iIndex, ref unitList);
			nKMArmyData.GetDeckOperator(deckIndex);
		}
	}

	public static void MakeDeckCopyCode(int shipID, int OperID, List<int> lstUnitIDs, int leaderIndex)
	{
		if (shipID == 0 && OperID == 0 && leaderIndex == -1 && lstUnitIDs.Count((int f) => f == 0) == lstUnitIDs.Count)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_PF_COPY_SQUAD_EMPTY"));
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(shipID);
		if (unitTempletBase == null)
		{
			Debug.Log($"[MakeDeckCopyCode] \ufffdԼ\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\u0378\ufffd ã\ufffd\ufffd \ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffdϴ\ufffd. {shipID}");
		}
		stringBuilder.Append(shipID);
		stringBuilder.Append(',');
		NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(OperID);
		if (unitTempletBase2 == null)
		{
			Debug.Log($"[MakeDeckCopyCode] \ufffd\ufffd\ufffd۷\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\u0378\ufffd ã\ufffd\ufffd \ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffdϴ\ufffd. {OperID}");
		}
		stringBuilder.Append(OperID);
		stringBuilder.Append(',');
		for (int num = 0; num < lstUnitIDs.Count; num++)
		{
			NKMUnitTempletBase unitTempletBase3 = NKMUnitManager.GetUnitTempletBase(lstUnitIDs[num]);
			if (unitTempletBase3 == null)
			{
				Debug.Log($"[MakeDeckCopyCode] \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\u0378\ufffd ã\ufffd\ufffd \ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffdϴ\ufffd. {lstUnitIDs[num]}[iCnt]");
			}
			stringBuilder.Append(lstUnitIDs[num]);
			stringBuilder.Append(',');
		}
		stringBuilder.Append(leaderIndex);
		byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
		Crypto.Encrypt(bytes, bytes.Length, MaskList);
		string text = Convert.ToBase64String(bytes);
		Debug.Log("<color=red>MakeDeckCopyCode Result</color>");
		Debug.Log($"<color=red>original data {stringBuilder}</color>");
		Debug.Log("<color=red>encode data " + text + "</color>");
		GUIUtility.systemCopyBuffer = text;
		NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_PF_COPY_SQUAD_COMPLETE"));
	}

	public static List<int> PasteDeckCode(string strDeckData)
	{
		List<int> list = new List<int>();
		byte[] array = Convert.FromBase64String(strDeckData);
		Crypto.Encrypt(array, array.Length, MaskList);
		string text = Encoding.UTF8.GetString(array);
		Debug.Log("<color=red>PasteDeckCode Result</color>");
		Debug.Log("<color=red>encode data " + strDeckData + "</color>");
		Debug.Log("<color=red>decode data " + text + "</color>");
		string[] array2 = text.Split(',');
		foreach (string s in array2)
		{
			list.Add(int.Parse(s));
		}
		return list;
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void InitUI()
	{
		NKCUtil.SetBindFunction(m_csbtnOK, OnClickOK);
		NKCUtil.SetBindFunction(m_csbtnCancel, OnClickCancel);
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener(delegate
		{
			Close();
		});
		m_etBG.triggers.Add(entry);
		m_IFDeckCopyCode.onEndEdit.RemoveAllListeners();
		m_IFDeckCopyCode.onEndEdit.AddListener(OnEditDeckCode);
		m_BeforeDeckOperDeckSlot.Init();
		for (int num = 0; num < m_lstBeforeDeckUnitslots.Count; num++)
		{
			m_lstBeforeDeckUnitslots[num].Init(num, bEnableDrag: false);
		}
		m_AfterDeckOperDeckSlot.Init();
		for (int num2 = 0; num2 < m_lstAfterDeckUnitslots.Count; num2++)
		{
			m_lstAfterDeckUnitslots[num2].Init(num2, bEnableDrag: false);
		}
		m_BeforeRaidDeckOperDeckSlot.Init();
		for (int num3 = 0; num3 < m_lstBeforeRaidDeckUnitslots.Count; num3++)
		{
			m_lstBeforeRaidDeckUnitslots[num3].Init(num3, bEnableDrag: false);
		}
		m_AfterDeckOperRaidDeckSlot.Init();
		for (int num4 = 0; num4 < m_lstAfterRaidDeckUnitslots.Count; num4++)
		{
			m_lstAfterRaidDeckUnitslots[num4].Init(num4, bEnableDrag: false);
		}
	}

	public void Open(NKMDeckIndex targetDeckIndex)
	{
		m_OpenData.SetDeckIndexData(targetDeckIndex);
		m_curDeckIndex = targetDeckIndex;
		m_curStateType = StateType.ST_INPUT;
		m_IFDeckCopyCode.text = "";
		m_lstCopyDeckDatas.Clear();
		UpdateUI();
		UIOpened();
	}

	public void Open(long lShipUID, long lOperUID, List<long> lstUnitUIDs, int iLeaderIdx)
	{
		m_OpenData.SetDeckUnitData(lShipUID, lOperUID, lstUnitUIDs, iLeaderIdx);
		m_curStateType = StateType.ST_INPUT;
		m_IFDeckCopyCode.text = "";
		m_lstCopyDeckDatas.Clear();
		UpdateUI();
		UIOpened();
	}

	private void UpdateUI()
	{
		switch (m_curStateType)
		{
		case StateType.ST_INPUT:
		{
			string systemCopyBuffer = GUIUtility.systemCopyBuffer;
			m_IFDeckCopyCode.text = systemCopyBuffer;
			NKCUtil.SetGameobjectActive(m_objMissMatchMakeDeck, bValue: false);
			break;
		}
		case StateType.ST_CONFIRM:
			UpdateBeforeDeckUI();
			UpdateAfterDeckUI();
			break;
		}
		NKCUtil.SetGameobjectActive(m_objCodeInput, m_curStateType == StateType.ST_INPUT);
		NKCUtil.SetGameobjectActive(m_objDeckSquadConfirm, m_curStateType == StateType.ST_CONFIRM && !m_OpenData.IsRaidDeck);
		NKCUtil.SetGameobjectActive(m_objRaidSquadConfirm, m_curStateType == StateType.ST_CONFIRM && m_OpenData.IsRaidDeck);
	}

	private void UpdateBeforeDeckUI()
	{
		NKMArmyData nKMArmyData = NKCScenManager.CurrentUserData()?.m_ArmyData;
		if (m_OpenData.IsDeckIndexData)
		{
			NKMDeckData deckData = nKMArmyData.GetDeckData(m_curDeckIndex.m_eDeckType, m_curDeckIndex.m_iIndex);
			if (deckData == null)
			{
				return;
			}
			List<long> listDeckUnitUID = deckData.m_listDeckUnitUID;
			NKMUnitTempletBase nKMUnitTempletBase = nKMArmyData.GetDeckShip(m_curDeckIndex)?.GetUnitTempletBase();
			Sprite shipSprite = ((nKMUnitTempletBase != null) ? NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, nKMUnitTempletBase) : NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_DECK_VIEW_TEXTURE", "NKM_DECK_VIEW_SHIP_UNKNOWN"));
			UpdateBeforeShip(shipSprite);
			NKMOperator deckOperator = nKMArmyData.GetDeckOperator(m_curDeckIndex);
			UpdateBeforeOperator(deckOperator);
			for (int i = 0; i < listDeckUnitUID.Count; i++)
			{
				NKMUnitData unitFromUID = nKMArmyData.GetUnitFromUID(listDeckUnitUID[i]);
				if (unitFromUID == null)
				{
					UpdateBeforeUnit(null, i);
				}
				else
				{
					UpdateBeforeUnit(unitFromUID, i);
				}
			}
			return;
		}
		NKMUnitTempletBase nKMUnitTempletBase2 = nKMArmyData.GetShipFromUID(m_OpenData.m_ShipUID)?.GetUnitTempletBase();
		Sprite shipSprite2 = ((nKMUnitTempletBase2 != null) ? NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, nKMUnitTempletBase2) : NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_DECK_VIEW_TEXTURE", "NKM_DECK_VIEW_SHIP_UNKNOWN"));
		UpdateBeforeShip(shipSprite2);
		NKMOperator operatorFromUId = nKMArmyData.GetOperatorFromUId(m_OpenData.m_OperUID);
		UpdateBeforeOperator(operatorFromUId);
		for (int j = 0; j < m_OpenData.m_lstUnitUIDs.Count; j++)
		{
			NKMUnitData unitFromUID2 = nKMArmyData.GetUnitFromUID(m_OpenData.m_lstUnitUIDs[j]);
			if (unitFromUID2 == null)
			{
				UpdateBeforeUnit(null, j);
			}
			else
			{
				UpdateBeforeUnit(unitFromUID2, j);
			}
		}
	}

	private void UpdateBeforeShip(Sprite shipSprite)
	{
		if (m_OpenData.IsRaidDeck)
		{
			m_imgBeforeRaidDeckShip.sprite = shipSprite;
		}
		else
		{
			m_imgBeforeDeckShip.sprite = shipSprite;
		}
	}

	private void UpdateBeforeOperator(NKMOperator oper)
	{
		if (m_OpenData.IsRaidDeck)
		{
			m_BeforeRaidDeckOperDeckSlot.SetData(oper);
		}
		else
		{
			m_BeforeDeckOperDeckSlot.SetData(oper);
		}
	}

	private void UpdateBeforeUnit(NKMUnitData unitData, int slotCnt)
	{
		if (m_OpenData.IsRaidDeck)
		{
			m_lstBeforeRaidDeckUnitslots[slotCnt].SetData(unitData, bEnableButton: false);
		}
		else
		{
			m_lstBeforeDeckUnitslots[slotCnt].SetData(unitData, bEnableButton: false);
		}
	}

	public static bool IsBase64String(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return false;
		}
		s = s.Trim();
		if (s.Length % 4 != 0)
		{
			return false;
		}
		return Regex.IsMatch(s, "^[a-zA-Z0-9\\+/]*={0,2}$", RegexOptions.None);
	}

	private void UpdateAfterDeckUI()
	{
		if (!IsBase64String(m_IFDeckCopyCode.text))
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_PF_COPY_SQUAD_ERROR"));
			return;
		}
		bool flag = false;
		m_lstApplyCopyDackData.Clear();
		m_lstCopyDeckDatas = PasteDeckCode(m_IFDeckCopyCode.text);
		NKMArmyData nKMArmyData = NKCScenManager.CurrentArmyData();
		NKMInventoryData inventoryData = NKCScenManager.CurrentUserData().m_InventoryData;
		for (int i = 0; i < 2; i++)
		{
			if (i == 0)
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_lstCopyDeckDatas[i]);
				long num = 0L;
				if (unitTempletBase != null)
				{
					int num2 = 0;
					foreach (KeyValuePair<long, NKMUnitData> item in nKMArmyData.m_dicMyShip)
					{
						if (NKMShipManager.IsSameKindShip(item.Value.m_UnitID, unitTempletBase.m_UnitID))
						{
							int num3 = item.Value.CalculateOperationPower(inventoryData);
							if (num2 < num3)
							{
								num2 = num3;
								num = item.Value.m_UnitUID;
							}
							else if (num2 == num3 && item.Value.m_UnitUID < num)
							{
								num = item.Value.m_UnitUID;
							}
						}
					}
				}
				m_lstApplyCopyDackData.Add(num);
				NKMUnitData shipFromUID = nKMArmyData.GetShipFromUID(num);
				Sprite shipSprite = ((shipFromUID != null) ? NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, shipFromUID.GetUnitTempletBase()) : NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_DECK_VIEW_TEXTURE", "NKM_DECK_VIEW_SHIP_UNKNOWN"));
				UpdateAfterShip(shipSprite);
				if (!flag)
				{
					flag = ((num == 0L && unitTempletBase != null) ? true : false);
				}
			}
			if (i != 1)
			{
				continue;
			}
			long num4 = 0L;
			int num5 = 0;
			NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(m_lstCopyDeckDatas[i]);
			if (unitTempletBase2 != null)
			{
				foreach (KeyValuePair<long, NKMOperator> item2 in nKMArmyData.m_dicMyOperator)
				{
					if (item2.Value.id == unitTempletBase2.m_UnitID)
					{
						int num6 = item2.Value.CalculateOperatorOperationPower();
						if (num5 < num6)
						{
							num5 = num6;
							num4 = item2.Value.uid;
						}
						else if (num5 == num6 && item2.Value.uid < num4)
						{
							num4 = item2.Value.uid;
						}
					}
				}
			}
			NKMOperator operatorFromUId = nKMArmyData.GetOperatorFromUId(num4);
			UpdateAfterOperator(operatorFromUId);
			m_lstApplyCopyDackData.Add(num4);
			if (!flag)
			{
				flag = ((num4 == 0L && unitTempletBase2 != null) ? true : false);
			}
		}
		int num7 = m_lstCopyDeckDatas.Count - 1;
		int num8 = m_lstCopyDeckDatas[num7];
		int num9 = 2;
		for (int j = num9; j < num7; j++)
		{
			long num10 = 0L;
			int num11 = j - num9;
			NKMUnitTempletBase unitTempletBase3 = NKMUnitManager.GetUnitTempletBase(m_lstCopyDeckDatas[j]);
			if (unitTempletBase3 != null)
			{
				int num12 = 0;
				foreach (KeyValuePair<long, NKMUnitData> item3 in nKMArmyData.m_dicMyUnit)
				{
					if (item3.Value.m_UnitID == unitTempletBase3.m_UnitID)
					{
						int num13 = item3.Value.CalculateOperationPower(inventoryData);
						if (num12 < num13)
						{
							num12 = num13;
							num10 = item3.Value.m_UnitUID;
						}
						else if (num12 == num13 && item3.Value.m_UnitUID < num10)
						{
							num10 = item3.Value.m_UnitUID;
						}
					}
				}
			}
			m_lstApplyCopyDackData.Add(num10);
			if (!flag)
			{
				flag = ((num10 == 0L && unitTempletBase3 != null) ? true : false);
			}
			NKMUnitData unitFromUID = nKMArmyData.GetUnitFromUID(num10);
			UpdateAfterUnit(unitFromUID, num11);
			if (num11 == num8)
			{
				m_lstAfterDeckUnitslots[num11].SetLeader(bLeader: true, bEffect: false);
			}
		}
		m_lstApplyCopyDackData.Add(num8);
		NKCUtil.SetGameobjectActive(m_objMissMatchMakeDeck, flag);
	}

	private void UpdateAfterShip(Sprite shipSprite)
	{
		if (m_OpenData.IsRaidDeck)
		{
			m_imgAfterRaidDeckShip.sprite = shipSprite;
		}
		else
		{
			m_imgAfterDeckShip.sprite = shipSprite;
		}
	}

	private void UpdateAfterOperator(NKMOperator oper)
	{
		if (m_OpenData.IsRaidDeck)
		{
			m_AfterDeckOperRaidDeckSlot.SetData(oper);
		}
		else
		{
			m_AfterDeckOperDeckSlot.SetData(oper);
		}
	}

	private void UpdateAfterUnit(NKMUnitData unitData, int iSlotCnt)
	{
		if (m_OpenData.IsRaidDeck)
		{
			m_lstAfterRaidDeckUnitslots[iSlotCnt].SetData(unitData);
		}
		else
		{
			m_lstAfterDeckUnitslots[iSlotCnt].SetData(unitData);
		}
	}

	public void OnClickOK()
	{
		switch (m_curStateType)
		{
		case StateType.ST_INPUT:
		{
			if (!IsBase64String(m_IFDeckCopyCode.text))
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_PF_COPY_SQUAD_ERROR"));
				break;
			}
			List<int> list = PasteDeckCode(m_IFDeckCopyCode.text);
			if (list.Count < 11)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_PF_COPY_SQUAD_ERROR"));
				break;
			}
			if ((m_OpenData.IsRaidDeck && list.Count == 11) || (!m_OpenData.IsRaidDeck && list.Count == 19))
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_PF_COPY_SQUAD_SQUAD_TYPE_ERROR"));
				break;
			}
			m_curStateType = StateType.ST_CONFIRM;
			UpdateUI();
			break;
		}
		case StateType.ST_CONFIRM:
			ApplyDeck();
			break;
		}
	}

	public void OnClickCancel()
	{
		switch (m_curStateType)
		{
		case StateType.ST_INPUT:
			Close();
			break;
		case StateType.ST_CONFIRM:
			m_curStateType = StateType.ST_INPUT;
			UpdateUI();
			break;
		}
	}

	private void OnEditDeckCode(string input)
	{
		if (NKCInputManager.IsChatSubmitEnter())
		{
			if (!m_csbtnOK.m_bLock)
			{
				OnClickOK();
			}
			EventSystem.current.SetSelectedGameObject(null);
		}
	}

	private void ApplyDeck()
	{
		if (m_OpenData.IsDeckIndexData)
		{
			if (NKCUIDeckViewer.IsInstanceOpen)
			{
				if (m_curDeckIndex.m_eDeckType == NKM_DECK_TYPE.NDT_DAILY)
				{
					NKCPacketSender.Send_Packet_DECK_UNIT_AUTO_SET_REQ(m_curDeckIndex, new List<long> { 0L, 0L, 0L, 0L, 0L, 0L, 0L, 0L }, 0L, 0L);
				}
				if (m_lstApplyCopyDackData[0] != 0L)
				{
					NKCUIDeckViewer.Instance.Send_NKMPacket_DECK_SHIP_SET_REQ(m_curDeckIndex, m_lstApplyCopyDackData[0]);
				}
				if (m_lstApplyCopyDackData[1] != 0L)
				{
					NKCUIDeckViewer.Instance.Send_NKMPacket_DECK_OPERATOR_SET_REQ(m_curDeckIndex, m_lstApplyCopyDackData[1]);
				}
				int num = 0;
				for (int i = 2; i < m_lstApplyCopyDackData.Count - 1; i++)
				{
					NKCUIDeckViewer.Instance.Send_NKMPacket_DECK_UNIT_SET_REQ(m_curDeckIndex, num, m_lstApplyCopyDackData[i]);
					num++;
				}
				sbyte b = (sbyte)m_lstApplyCopyDackData[m_lstApplyCopyDackData.Count - 1];
				if (!m_OpenData.IsRaidDeck && b > -1)
				{
					NKCUIDeckViewer.Instance.Send_Packet_DECK_UNIT_SET_LEADER_REQ(m_curDeckIndex, b);
				}
			}
			else
			{
				if (m_lstApplyCopyDackData[0] != 0L)
				{
					NKCPacketSender.Send_NKMPacket_DECK_SHIP_SET_REQ(m_curDeckIndex, m_lstApplyCopyDackData[0]);
				}
				if (m_lstApplyCopyDackData[1] != 0L)
				{
					NKCPacketSender.Send_NKMPacket_DECK_OPERATOR_SET_REQ(m_curDeckIndex, m_lstApplyCopyDackData[1]);
				}
				for (int j = 2; j < m_lstApplyCopyDackData.Count; j++)
				{
					int slotIndex = j - 1 + 1;
					NKCPacketSender.Send_NKMPacket_DECK_UNIT_SET_REQ(m_curDeckIndex, slotIndex, m_lstApplyCopyDackData[j]);
				}
				sbyte b2 = (sbyte)m_lstApplyCopyDackData[m_lstApplyCopyDackData.Count - 1];
				if (b2 > -1)
				{
					NKCPacketSender.Send_Packet_DECK_UNIT_SET_LEADER_REQ(m_curDeckIndex, b2);
				}
			}
			Close();
			return;
		}
		if (NKCUIPrepareEventDeck.IsInstanceOpen)
		{
			NKCUIPrepareEventDeck.Instance.OnShipSelected(m_lstApplyCopyDackData[0]);
			NKCUIPrepareEventDeck.Instance.OnOperatorSelected(m_lstApplyCopyDackData[1]);
			int num2 = 0;
			for (int k = 2; k < m_lstApplyCopyDackData.Count - 1; k++)
			{
				int index = k - 1 + 1;
				NKCUIPrepareEventDeck.Instance.OnUnitSelected(num2, m_lstApplyCopyDackData[index]);
				num2++;
			}
			NKCUIPrepareEventDeck.Instance.SetAsLeader((int)m_lstApplyCopyDackData[m_lstApplyCopyDackData.Count - 1]);
		}
		Close();
	}
}
