using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCDeckViewGuide : MonoBehaviour
{
	public delegate void OnWarning(bool bWarning);

	[Header("Class")]
	public Text m_txtStriker;

	public Text m_txtRanger;

	public Text m_txtSniper;

	public Text m_txtDefender;

	public Text m_txtSupporter;

	public Text m_txtTower;

	public Text m_txtSiege;

	[Header("ATK Type")]
	public Text m_txtGround;

	public Text m_txtAir;

	public Text m_txtAll;

	private const string TEXT_COLOR_NORMAL = "#FFFFFF";

	private const string TEXT_COLOR_WARNING = "#FF2626";

	private const string TEXT_COLOR_WARNING2 = "#FF7979";

	private OnWarning dOnWarning;

	public bool IsOpen => base.gameObject.activeSelf;

	public void Init(OnWarning onWarning)
	{
		dOnWarning = onWarning;
	}

	public void Active(bool bSet)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bSet);
	}

	public void SetData(NKMArmyData armyData, NKMDeckIndex deckIndex)
	{
		NKMDeckData deckData = armyData.GetDeckData(deckIndex);
		if (deckData == null)
		{
			return;
		}
		Dictionary<NKM_UNIT_ROLE_TYPE, int> dictionary = new Dictionary<NKM_UNIT_ROLE_TYPE, int>();
		Dictionary<NKM_FIND_TARGET_TYPE, int> dictionary2 = new Dictionary<NKM_FIND_TARGET_TYPE, int>();
		for (int i = 0; i < deckData.m_listDeckUnitUID.Count; i++)
		{
			long unitUid = deckData.m_listDeckUnitUID[i];
			NKMUnitData unitFromUID = armyData.GetUnitFromUID(unitUid);
			if (unitFromUID == null)
			{
				continue;
			}
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitFromUID);
			if (unitTempletBase != null)
			{
				if (dictionary.ContainsKey(unitTempletBase.m_NKM_UNIT_ROLE_TYPE))
				{
					dictionary[unitTempletBase.m_NKM_UNIT_ROLE_TYPE]++;
				}
				else
				{
					dictionary.Add(unitTempletBase.m_NKM_UNIT_ROLE_TYPE, 1);
				}
				NKM_FIND_TARGET_TYPE key = unitTempletBase.m_NKM_FIND_TARGET_TYPE;
				if (unitTempletBase.m_NKM_FIND_TARGET_TYPE_Desc != NKM_FIND_TARGET_TYPE.NFTT_INVALID)
				{
					key = unitTempletBase.m_NKM_FIND_TARGET_TYPE_Desc;
				}
				if (dictionary2.ContainsKey(key))
				{
					dictionary2[key]++;
				}
				else
				{
					dictionary2.Add(key, 1);
				}
			}
		}
		bool flag = false;
		flag |= SetRole(dictionary);
		flag |= SetAtk(dictionary2);
		dOnWarning?.Invoke(flag);
	}

	private bool SetRole(Dictionary<NKM_UNIT_ROLE_TYPE, int> dicRole)
	{
		return (byte)(0u | (SetRoleText(dicRole, NKM_UNIT_ROLE_TYPE.NURT_STRIKER, m_txtStriker) ? 1u : 0u) | (SetRoleText(dicRole, NKM_UNIT_ROLE_TYPE.NURT_RANGER, m_txtRanger) ? 1u : 0u) | (SetRoleText(dicRole, NKM_UNIT_ROLE_TYPE.NURT_SNIPER, m_txtSniper) ? 1u : 0u) | (SetRoleText(dicRole, NKM_UNIT_ROLE_TYPE.NURT_DEFENDER, m_txtDefender) ? 1u : 0u) | (SetRoleText(dicRole, NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER, m_txtSupporter) ? 1u : 0u) | (SetRoleText(dicRole, NKM_UNIT_ROLE_TYPE.NURT_TOWER, m_txtTower) ? 1u : 0u) | (SetRoleText(dicRole, NKM_UNIT_ROLE_TYPE.NURT_SIEGE, m_txtSiege) ? 1u : 0u)) != 0;
	}

	private bool SetRoleText(Dictionary<NKM_UNIT_ROLE_TYPE, int> dicRole, NKM_UNIT_ROLE_TYPE role, Text textUI)
	{
		bool result = false;
		int num = 0;
		string hexRGB = "#FFFFFF";
		if (dicRole.ContainsKey(role))
		{
			num = dicRole[role];
		}
		switch (role)
		{
		case NKM_UNIT_ROLE_TYPE.NURT_STRIKER:
		case NKM_UNIT_ROLE_TYPE.NURT_RANGER:
		case NKM_UNIT_ROLE_TYPE.NURT_DEFENDER:
		case NKM_UNIT_ROLE_TYPE.NURT_SNIPER:
			switch (num)
			{
			case 0:
				hexRGB = "#FF2626";
				break;
			case 1:
				hexRGB = "#FF7979";
				break;
			}
			result = num < 2;
			break;
		}
		textUI.text = num.ToString();
		textUI.color = NKCUtil.GetColor(hexRGB);
		return result;
	}

	private bool SetAtk(Dictionary<NKM_FIND_TARGET_TYPE, int> dicAtk)
	{
		bool result = false;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		foreach (KeyValuePair<NKM_FIND_TARGET_TYPE, int> item in dicAtk)
		{
			switch (item.Key)
			{
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY:
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_AIR_FIRST:
			case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY:
			case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_BOSS_LAST:
				num3 += item.Value;
				break;
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_LAND:
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_LAND_RANGER_SUPPORTER_SNIPER_FIRST:
			case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_LAND:
			case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_LAND_BOSS_LAST:
				num += item.Value;
				break;
			case NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY_AIR:
			case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_AIR:
			case NKM_FIND_TARGET_TYPE.NFTT_FAR_ENEMY_AIR_BOSS_LAST:
				num2 += item.Value;
				break;
			}
		}
		m_txtGround.text = num.ToString();
		m_txtAir.text = num2.ToString();
		m_txtAll.text = num3.ToString();
		m_txtGround.color = NKCUtil.GetColor(GetAtkColor(num + num3));
		m_txtAir.color = NKCUtil.GetColor(GetAtkColor(num2 + num3));
		m_txtAll.color = NKCUtil.GetColor(GetAtkColor(Mathf.Min(num + num3, num2 + num3)));
		return result;
	}

	private string GetAtkColor(int count)
	{
		if (count == 0)
		{
			return "#FF2626";
		}
		if (count <= 2)
		{
			return "#FF7979";
		}
		return "#FFFFFF";
	}
}
