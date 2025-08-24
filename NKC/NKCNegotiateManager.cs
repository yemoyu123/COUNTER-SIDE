using System;
using System.Collections.Generic;
using System.Linq;
using AssetBundles;
using ClientPacket.Common;
using ClientPacket.Negotiation;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC;

public static class NKCNegotiateManager
{
	public enum SpeechType
	{
		Ready,
		GreatSuccess,
		Success
	}

	[Serializable]
	public class NKMNegotiateSpeechTemplet
	{
		public int m_UnitID;

		public int m_SkinID;

		public string m_NegoStanby;

		public string m_NegoGreatSuccess;

		public string m_NegoSuccess;

		public static NKMNegotiateSpeechTemplet LoadFromLUA(NKMLua cNKMLua)
		{
			NKMNegotiateSpeechTemplet nKMNegotiateSpeechTemplet = new NKMNegotiateSpeechTemplet();
			int num = 1 & (cNKMLua.GetData("m_UnitID", ref nKMNegotiateSpeechTemplet.m_UnitID) ? 1 : 0);
			cNKMLua.GetData("m_SkinID", ref nKMNegotiateSpeechTemplet.m_SkinID);
			if (((uint)num & (cNKMLua.GetData("m_NegoStanby", ref nKMNegotiateSpeechTemplet.m_NegoStanby) ? 1u : 0u) & (cNKMLua.GetData("m_NegoGreatSuccess", ref nKMNegotiateSpeechTemplet.m_NegoGreatSuccess) ? 1u : 0u) & (cNKMLua.GetData("m_NegoSuccess", ref nKMNegotiateSpeechTemplet.m_NegoSuccess) ? 1u : 0u)) == 0)
			{
				Debug.LogError("NKMNegotiateSpeechTemplet LoadFromLUA fail");
				return null;
			}
			return nKMNegotiateSpeechTemplet;
		}

		public void Join()
		{
		}

		public void Validate()
		{
		}
	}

	public class NegotiateResultUIData
	{
		public NEGOTIATE_RESULT NegotiateResult;

		public long TargetUnitUID;

		public int UnitID;

		public int CreditUsed;

		public int UserLevelBefore;

		public int UserLevelAfter;

		public int UserExpBefore;

		public int UserExpAfter;

		public int UnitLevelBefore;

		public int UnitLevelAfter;

		public int UnitExpBefore;

		public int UnitExpAfter;

		public int LoyaltyBefore;

		public int LoyaltyAfter;
	}

	public const int NEGOTIATE_ITEM_REQUIRE_COUNT = 1;

	private static Dictionary<int, Dictionary<int, NKMNegotiateSpeechTemplet>> m_dicNegoSpeech;

	public static void LoadFromLua()
	{
		if (m_dicNegoSpeech != null)
		{
			return;
		}
		NKMLua nKMLua = new NKMLua();
		m_dicNegoSpeech = new Dictionary<int, Dictionary<int, NKMNegotiateSpeechTemplet>>();
		if (nKMLua.LoadCommonPath("AB_SCRIPT", "LUA_NEGOTIATE_SPEECH") && nKMLua.OpenTable("m_NegoSpeech"))
		{
			int num = 1;
			while (nKMLua.OpenTable(num))
			{
				NKMNegotiateSpeechTemplet nKMNegotiateSpeechTemplet = NKMNegotiateSpeechTemplet.LoadFromLUA(nKMLua);
				if (nKMNegotiateSpeechTemplet != null)
				{
					if (!m_dicNegoSpeech.ContainsKey(nKMNegotiateSpeechTemplet.m_UnitID))
					{
						m_dicNegoSpeech.Add(nKMNegotiateSpeechTemplet.m_UnitID, new Dictionary<int, NKMNegotiateSpeechTemplet>());
					}
					if (!m_dicNegoSpeech[nKMNegotiateSpeechTemplet.m_UnitID].ContainsKey(nKMNegotiateSpeechTemplet.m_SkinID))
					{
						m_dicNegoSpeech[nKMNegotiateSpeechTemplet.m_UnitID].Add(nKMNegotiateSpeechTemplet.m_SkinID, nKMNegotiateSpeechTemplet);
					}
					else
					{
						Debug.LogError($"m_dicNegoSpeech Duplicate unitID:{nKMNegotiateSpeechTemplet.m_UnitID}, skinID:{nKMNegotiateSpeechTemplet.m_SkinID}");
					}
				}
				num++;
				nKMLua.CloseTable();
			}
			nKMLua.CloseTable();
		}
		nKMLua.LuaClose();
	}

	public static NKMNegotiateSpeechTemplet GetSpeechTemplet(int unitID, int skinID, bool bCheckVoiceBundle = false)
	{
		if (m_dicNegoSpeech == null)
		{
			LoadFromLua();
		}
		NKMNegotiateSpeechTemplet value;
		if (m_dicNegoSpeech.ContainsKey(unitID))
		{
			bool flag = true;
			if (bCheckVoiceBundle)
			{
				flag = AssetBundleManager.IsBundleExists(NKMSkinManager.GetSkinTemplet(skinID)?.m_VoiceBundleName);
			}
			if (flag && m_dicNegoSpeech[unitID].TryGetValue(skinID, out value))
			{
				return value;
			}
			flag = true;
			if (bCheckVoiceBundle)
			{
				NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(unitID);
				flag = AssetBundleManager.IsBundleExists($"AB_UI_UNIT_VOICE_{nKMUnitTempletBase.m_UnitStrID}");
			}
			if (flag && m_dicNegoSpeech[unitID].TryGetValue(0, out value))
			{
				return value;
			}
		}
		else
		{
			NKMUnitTempletBase nKMUnitTempletBase2 = NKMUnitTempletBase.Find(unitID);
			if (nKMUnitTempletBase2.m_BaseUnitID != 0 && nKMUnitTempletBase2.m_BaseUnitID != unitID)
			{
				bool flag2 = true;
				if (bCheckVoiceBundle)
				{
					flag2 = AssetBundleManager.IsBundleExists(NKMSkinManager.GetSkinTemplet(skinID)?.m_VoiceBundleName);
				}
				if (flag2 && m_dicNegoSpeech[nKMUnitTempletBase2.m_BaseUnitID].TryGetValue(skinID, out value))
				{
					return value;
				}
				flag2 = true;
				if (bCheckVoiceBundle)
				{
					flag2 = AssetBundleManager.IsBundleExists($"AB_UI_UNIT_VOICE_{nKMUnitTempletBase2.BaseUnit.m_UnitStrID}");
				}
				if (flag2 && m_dicNegoSpeech[nKMUnitTempletBase2.m_BaseUnitID].TryGetValue(0, out value))
				{
					return value;
				}
			}
		}
		if (bCheckVoiceBundle)
		{
			return GetSpeechTemplet(unitID, skinID);
		}
		return null;
	}

	public static NKM_ERROR_CODE CanTargetNegotiate(NKMUserData userData, NKMUnitData targetUnit)
	{
		if (targetUnit == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_NOT_EXIST;
		}
		if (NKCExpManager.GetUnitExpTemplet(targetUnit) == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_GET_UNIT_EXP_TEMPLET_NULL;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(targetUnit.m_UnitID);
		if (unitTempletBase == null || unitTempletBase.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_TRAINER)
		{
			return NKM_ERROR_CODE.NEC_FAIL_GET_UNIT_BASE_TEMPLET_NULL;
		}
		if (targetUnit.loyalty == 10000 && targetUnit.m_UnitLevel == NKCExpManager.GetUnitMaxLevel(targetUnit))
		{
			return NKM_ERROR_CODE.NEC_FAIL_NEGOTIATION_EXP_LOYALTY_FULL;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static NKM_ERROR_CODE CanStartNegotiate(NKMUserData userData, NKMUnitData targetUnit, NEGOTIATE_BOSS_SELECTION bossSelection, List<MiscItemData> lstMaterials)
	{
		if (targetUnit == null || userData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_NOT_EXIST;
		}
		if (NKCExpManager.GetUnitExpTemplet(targetUnit) == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_GET_UNIT_EXP_TEMPLET_NULL;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(targetUnit.m_UnitID);
		if (unitTempletBase == null || unitTempletBase.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_TRAINER)
		{
			return NKM_ERROR_CODE.NEC_FAIL_GET_UNIT_BASE_TEMPLET_NULL;
		}
		if (lstMaterials == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_ITEM;
		}
		foreach (MiscItemData lstMaterial in lstMaterials)
		{
			if (userData.m_InventoryData.GetCountMiscItem(lstMaterial.itemId) < lstMaterial.count)
			{
				return NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_ITEM;
			}
		}
		if (lstMaterials.Sum((MiscItemData e) => e.count) > NKMCommonConst.Negotiation.MaxMaterialUsageLimit)
		{
			return NKM_ERROR_CODE.NEC_FAIL_NEGOTIATION_INVALID_MATERIAL_COUNT;
		}
		long negotiateSalary = GetNegotiateSalary(lstMaterials, bossSelection);
		if (userData.m_InventoryData.GetCountMiscItem(1) < negotiateSalary)
		{
			return NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_CREDIT;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static long GetNegotiateSalary(List<MiscItemData> lstMaterials, NEGOTIATE_BOSS_SELECTION selection = NEGOTIATE_BOSS_SELECTION.OK)
	{
		long credit = 0L;
		for (int i = 0; i < lstMaterials.Count; i++)
		{
			if (lstMaterials[i] != null)
			{
				NKMCommonConst.Negotiation.TryGetMaterial(lstMaterials[i].itemId, out var materialTemplet);
				credit += lstMaterials[i].count * materialTemplet.Credit;
			}
		}
		NKCCompanyBuff.SetDiscountOfCreditInNegotiation(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref credit);
		switch (selection)
		{
		case NEGOTIATE_BOSS_SELECTION.RAISE:
			credit += credit * NKMCommonConst.Negotiation.Bonus_CreditIncreasePercent / 100;
			break;
		case NEGOTIATE_BOSS_SELECTION.PASSION:
			credit -= credit * NKMCommonConst.Negotiation.Passion_CreditDecreasePercent / 100;
			break;
		}
		return credit;
	}

	public static int GetNegotiateExp(List<MiscItemData> lstMaterials, bool bPermanentContract)
	{
		int num = 0;
		for (int i = 0; i < lstMaterials.Count; i++)
		{
			if (lstMaterials[i] != null)
			{
				NKMCommonConst.Negotiation.TryGetMaterial(lstMaterials[i].itemId, out var materialTemplet);
				num += lstMaterials[i].count * materialTemplet.Exp;
			}
		}
		if (bPermanentContract)
		{
			num = num * 120 / 100;
		}
		return num;
	}

	public static int GetNegotiateLoyalty(List<MiscItemData> lstMaterials, NEGOTIATE_BOSS_SELECTION selection = NEGOTIATE_BOSS_SELECTION.OK)
	{
		int num = 0;
		for (int i = 0; i < lstMaterials.Count; i++)
		{
			if (lstMaterials[i] != null)
			{
				NKMCommonConst.Negotiation.TryGetMaterial(lstMaterials[i].itemId, out var materialTemplet);
				num += lstMaterials[i].count * materialTemplet.Loyalty;
			}
		}
		switch (selection)
		{
		case NEGOTIATE_BOSS_SELECTION.RAISE:
			num = num * 110 / 100;
			break;
		case NEGOTIATE_BOSS_SELECTION.PASSION:
			return 0;
		}
		return num;
	}

	public static SpeechType GetSpeechType(NEGOTIATE_RESULT resultType)
	{
		return resultType switch
		{
			NEGOTIATE_RESULT.SUCCESS => SpeechType.GreatSuccess, 
			_ => SpeechType.Success, 
		};
	}

	public static string GetSpeech(NKMUnitData unitData, SpeechType type, bool bCheckVoiceBundle = false)
	{
		if (unitData == null)
		{
			return "";
		}
		NKMNegotiateSpeechTemplet speechTemplet = GetSpeechTemplet(unitData.m_UnitID, unitData.m_SkinID, bCheckVoiceBundle);
		if (speechTemplet == null)
		{
			return "";
		}
		string text = string.Empty;
		switch (type)
		{
		case SpeechType.Ready:
			text = speechTemplet.m_NegoStanby;
			break;
		case SpeechType.GreatSuccess:
			text = speechTemplet.m_NegoGreatSuccess;
			break;
		case SpeechType.Success:
			text = speechTemplet.m_NegoSuccess;
			break;
		}
		if (!string.IsNullOrEmpty(text))
		{
			return NKCStringTable.GetString(text);
		}
		return "";
	}

	public static NegotiateResultUIData MakeResultUIData(NKMUserData userDataBefore, NKMPacket_NEGOTIATE_ACK sPacket)
	{
		NegotiateResultUIData negotiateResultUIData = new NegotiateResultUIData();
		NKMUnitData unitFromUID = userDataBefore.m_ArmyData.GetUnitFromUID(sPacket.targetUnitUid);
		negotiateResultUIData.NegotiateResult = sPacket.negotiateResult;
		negotiateResultUIData.TargetUnitUID = sPacket.targetUnitUid;
		negotiateResultUIData.UnitID = unitFromUID.m_UnitID;
		negotiateResultUIData.CreditUsed = sPacket.finalSalary;
		negotiateResultUIData.UnitLevelBefore = unitFromUID.m_UnitLevel;
		negotiateResultUIData.UnitLevelAfter = sPacket.targetUnitLevel;
		negotiateResultUIData.UnitExpBefore = unitFromUID.m_iUnitLevelEXP;
		negotiateResultUIData.UnitExpAfter = sPacket.targetUnitExp;
		negotiateResultUIData.LoyaltyBefore = unitFromUID.loyalty;
		negotiateResultUIData.LoyaltyAfter = sPacket.targetUnitLoyalty;
		return negotiateResultUIData;
	}
}
