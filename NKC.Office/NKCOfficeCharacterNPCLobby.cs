using System.Collections.Generic;
using NKC.UI.NPC;
using NKC.UI.Office;
using NKM;
using UnityEngine;

namespace NKC.Office;

public class NKCOfficeCharacterNPCLobby : NKCOfficeCharacterNPC
{
	public GameObject m_objBizcardRoot;

	public GameObject m_objBizcardEnable;

	public GameObject m_objBizcardDisable;

	private void SetBizcardEnable(bool value)
	{
		NKCUtil.SetGameobjectActive(m_objBizcardEnable, value);
		NKCUtil.SetGameobjectActive(m_objBizcardDisable, !value);
	}

	public override void Init(NKCOfficeBuildingBase officeBuilding)
	{
		base.Init(officeBuilding);
		if (NKCUINPCBase.GetNPCTempletDic(m_eNPCType).Count == 0)
		{
			LoadFromLua();
		}
	}

	protected void LoadFromLua()
	{
		Dictionary<NPC_ACTION_TYPE, List<NKCNPCTemplet>> nPCTempletDic = NKCUINPCBase.GetNPCTempletDic(m_eNPCType);
		NKMLua nKMLua = new NKMLua();
		if (!nKMLua.LoadCommonPath("AB_SCRIPT_NPC", "LUA_NPC_MANAGER_KIMHANA_TEMPLET"))
		{
			return;
		}
		if (nKMLua.OpenTable("m_dicNPCTemplet"))
		{
			int num = 1;
			while (nKMLua.OpenTable(num))
			{
				NKCNPCTemplet nKCNPCTemplet = new NKCNPCTemplet();
				if (nKCNPCTemplet.LoadLUA(nKMLua))
				{
					if (!nPCTempletDic.ContainsKey(nKCNPCTemplet.m_ActionType))
					{
						nPCTempletDic.Add(nKCNPCTemplet.m_ActionType, new List<NKCNPCTemplet>());
					}
					nPCTempletDic[nKCNPCTemplet.m_ActionType].Add(nKCNPCTemplet);
				}
				num++;
				nKMLua.CloseTable();
			}
			nKMLua.CloseTable();
		}
		nKMLua.LuaClose();
	}

	protected override bool OnTouchAction()
	{
		if (NKCScenManager.CurrentUserData().OfficeData.BizcardCount > 0)
		{
			NKCUIPopupOfficeInteract.Instance.Open();
			return true;
		}
		return false;
	}

	protected override void Update()
	{
		base.Update();
		SetBizcardEnable(NKCScenManager.CurrentUserData().OfficeData.BizcardCount > 0);
		if (m_objBizcardRoot.transform.lossyScale.x < 0f)
		{
			m_objBizcardRoot.transform.localScale = new Vector3(0f - m_objBizcardRoot.transform.localScale.x, m_objBizcardRoot.transform.localScale.y, m_objBizcardRoot.transform.localScale.z);
		}
	}
}
