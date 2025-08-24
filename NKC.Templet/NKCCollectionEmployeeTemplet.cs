using System.Collections.Generic;
using Cs.Logging;
using NKC.Templet.Base;
using NKM;

namespace NKC.Templet;

public class NKCCollectionEmployeeTemplet : INKCTemplet
{
	public int UnitID;

	public string OpenTag;

	public int SortIndex;

	public bool bExclude;

	public string CharacterType;

	public string NameType;

	public string NameValue;

	public string TeamConceptStrID;

	public string TeamUpStrID;

	public string TeamUpMarkStrID;

	public string GenderType;

	public string GenderValueStrID;

	public string BirthType;

	public string BirthValueStrID;

	public string HeightType;

	public string HeightValueStrID;

	public string SpecialityType;

	public string SpecialityValueStrID;

	public string LikeType;

	public string LikeValueStrID;

	public string DisLikeType;

	public string DisLikeValueStrID;

	public string CombatLevelType;

	public string CombatLevelValue;

	public string CommandLevelType;

	public string CommandLevelValue;

	public string CRFType;

	public List<string> CRFSubType = new List<string>();

	public List<int> CRFSubAmount = new List<int>();

	public List<string> ProfileType = new List<string>();

	public List<string> ProfileValue = new List<string>();

	public string OprProfileType01;

	public string OprProfileType02;

	public string OprProfileType03;

	public const int MaxProfileSlotCount = 10;

	public int Key => UnitID;

	public static NKCCollectionEmployeeTemplet LoadFromLUA(NKMLua lua)
	{
		NKCCollectionEmployeeTemplet nKCCollectionEmployeeTemplet = new NKCCollectionEmployeeTemplet();
		bool flag = true;
		flag &= lua.GetData("UnitID", ref nKCCollectionEmployeeTemplet.UnitID);
		flag &= lua.GetData("OpenTag", ref nKCCollectionEmployeeTemplet.OpenTag);
		lua.GetData("SortIndex", ref nKCCollectionEmployeeTemplet.SortIndex);
		lua.GetData("bExclude", ref nKCCollectionEmployeeTemplet.bExclude);
		lua.GetData("CharacterType", ref nKCCollectionEmployeeTemplet.CharacterType);
		lua.GetData("NameType", ref nKCCollectionEmployeeTemplet.NameType);
		lua.GetData("NameValue", ref nKCCollectionEmployeeTemplet.NameValue);
		lua.GetData("TeamConceptStrID", ref nKCCollectionEmployeeTemplet.TeamConceptStrID);
		lua.GetData("TeamUpStrID", ref nKCCollectionEmployeeTemplet.TeamUpStrID);
		lua.GetData("TeamUpMarkStrID", ref nKCCollectionEmployeeTemplet.TeamUpMarkStrID);
		lua.GetData("GenderType", ref nKCCollectionEmployeeTemplet.GenderType);
		lua.GetData("GenderValueStrID", ref nKCCollectionEmployeeTemplet.GenderValueStrID);
		lua.GetData("BirthType", ref nKCCollectionEmployeeTemplet.BirthType);
		lua.GetData("BirthValueStrID", ref nKCCollectionEmployeeTemplet.BirthValueStrID);
		lua.GetData("HeightType", ref nKCCollectionEmployeeTemplet.HeightType);
		lua.GetData("HeightValueStrID", ref nKCCollectionEmployeeTemplet.HeightValueStrID);
		lua.GetData("SpecialityType", ref nKCCollectionEmployeeTemplet.SpecialityType);
		lua.GetData("SpecialityValueStrID", ref nKCCollectionEmployeeTemplet.SpecialityValueStrID);
		lua.GetData("LikeType", ref nKCCollectionEmployeeTemplet.LikeType);
		lua.GetData("LikeValueStrID", ref nKCCollectionEmployeeTemplet.LikeValueStrID);
		lua.GetData("DisLikeType", ref nKCCollectionEmployeeTemplet.DisLikeType);
		lua.GetData("DisLikeValueStrID", ref nKCCollectionEmployeeTemplet.DisLikeValueStrID);
		lua.GetData("CombatLevelType", ref nKCCollectionEmployeeTemplet.CombatLevelType);
		lua.GetData("CombatLevelValue", ref nKCCollectionEmployeeTemplet.CombatLevelValue);
		lua.GetData("CommandLevelType", ref nKCCollectionEmployeeTemplet.CommandLevelType);
		lua.GetData("CommandLevelValue", ref nKCCollectionEmployeeTemplet.CommandLevelValue);
		lua.GetData("CRFType", ref nKCCollectionEmployeeTemplet.CRFType);
		lua.GetData("OPR_Profile_01", ref nKCCollectionEmployeeTemplet.OprProfileType01);
		lua.GetData("OPR_Profile_02", ref nKCCollectionEmployeeTemplet.OprProfileType02);
		lua.GetData("OPR_Profile_03", ref nKCCollectionEmployeeTemplet.OprProfileType03);
		nKCCollectionEmployeeTemplet.CRFSubType.Clear();
		string rValue;
		for (int i = 1; lua.GetData($"CRFSubType_{i}", out rValue, ""); i++)
		{
			nKCCollectionEmployeeTemplet.CRFSubType.Add(rValue);
		}
		nKCCollectionEmployeeTemplet.CRFSubAmount.Clear();
		int rValue2;
		for (int i = 1; lua.GetData($"CRFSubAmount_{i}", out rValue2, 0); i++)
		{
			nKCCollectionEmployeeTemplet.CRFSubAmount.Add(rValue2);
		}
		nKCCollectionEmployeeTemplet.ProfileType.Clear();
		int num = 0;
		for (int j = 1; j <= 10; j++)
		{
			lua.GetData($"ProfileType_{j}", out var rValue3, "");
			if (string.IsNullOrEmpty(rValue3))
			{
				num++;
				continue;
			}
			for (int k = 0; k < num; k++)
			{
				nKCCollectionEmployeeTemplet.ProfileType.Add("");
			}
			nKCCollectionEmployeeTemplet.ProfileType.Add(rValue3);
			num = 0;
		}
		nKCCollectionEmployeeTemplet.ProfileValue.Clear();
		num = 0;
		for (int l = 1; l <= 10; l++)
		{
			lua.GetData($"ProfileValue_{l}", out var rValue4, "");
			if (string.IsNullOrEmpty(rValue4))
			{
				num++;
				continue;
			}
			for (int m = 0; m < num; m++)
			{
				nKCCollectionEmployeeTemplet.ProfileValue.Add("");
			}
			nKCCollectionEmployeeTemplet.ProfileValue.Add(rValue4);
			num = 0;
		}
		if (!flag)
		{
			Log.Error("NKCCollectionEmployeeTemplet data is not valid", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCCollectionEmployeeTemplet.cs", 151);
			return null;
		}
		return nKCCollectionEmployeeTemplet;
	}

	public string GetTeamConcept()
	{
		return NKCStringTable.GetString(TeamConceptStrID);
	}

	public string GetTeamName()
	{
		return NKCStringTable.GetString(TeamUpStrID);
	}
}
