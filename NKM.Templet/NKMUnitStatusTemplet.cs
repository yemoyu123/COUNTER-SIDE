using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMUnitStatusTemplet : INKMTemplet
{
	public enum EffectPosition
	{
		OverGuageMark,
		BodyCenter,
		Head,
		Ground
	}

	public NKM_UNIT_STATUS_EFFECT m_StatusType;

	public bool m_bDebuff;

	public string m_StatusStrID = "";

	public string m_StatusEffectName = "";

	public EffectPosition m_StatusEffectPosition;

	public bool m_bShowFixedStatus;

	public bool m_bAllowBoss = true;

	public bool m_bAllowShip = true;

	public bool m_bCrowdControl;

	public bool m_bDispel = true;

	public string m_DescStrID;

	public int m_sortIndex;

	public bool m_bShowGuide = true;

	public int Key => (int)m_StatusType;

	public static bool IsDebuff(NKM_UNIT_STATUS_EFFECT type)
	{
		return Find(type)?.m_bDebuff ?? false;
	}

	public static bool IsCrowdControlStatus(NKM_UNIT_STATUS_EFFECT type)
	{
		return Find(type)?.m_bCrowdControl ?? false;
	}

	public static NKMUnitStatusTemplet Find(int key)
	{
		return NKMTempletContainer<NKMUnitStatusTemplet>.Find(key);
	}

	public static NKMUnitStatusTemplet Find(NKM_UNIT_STATUS_EFFECT type)
	{
		return NKMTempletContainer<NKMUnitStatusTemplet>.Find((int)type);
	}

	public static NKMUnitStatusTemplet LoadFromLua(NKMLua cNKMLua)
	{
		NKMUnitStatusTemplet nKMUnitStatusTemplet = new NKMUnitStatusTemplet();
		int num = 1 & (cNKMLua.GetDataEnum<NKM_UNIT_STATUS_EFFECT>("m_StatusType", out nKMUnitStatusTemplet.m_StatusType) ? 1 : 0);
		cNKMLua.GetData("m_bDebuff", ref nKMUnitStatusTemplet.m_bDebuff);
		cNKMLua.GetData("m_StatusStrID", ref nKMUnitStatusTemplet.m_StatusStrID);
		cNKMLua.GetData("m_StatusEffectName", ref nKMUnitStatusTemplet.m_StatusEffectName);
		cNKMLua.GetData("m_StatusEffectPosition", ref nKMUnitStatusTemplet.m_StatusEffectPosition);
		cNKMLua.GetData("m_bShowFixedStatus", ref nKMUnitStatusTemplet.m_bShowFixedStatus);
		cNKMLua.GetData("m_bAllowBoss", ref nKMUnitStatusTemplet.m_bAllowBoss);
		cNKMLua.GetData("m_bAllowShip", ref nKMUnitStatusTemplet.m_bAllowShip);
		cNKMLua.GetData("m_bCrowdControl", ref nKMUnitStatusTemplet.m_bCrowdControl);
		cNKMLua.GetData("m_bDispel", ref nKMUnitStatusTemplet.m_bDispel);
		cNKMLua.GetData("m_StatusDesc", ref nKMUnitStatusTemplet.m_DescStrID);
		cNKMLua.GetData("Category_Order_List", ref nKMUnitStatusTemplet.m_sortIndex);
		cNKMLua.GetData("m_bShowGuide", ref nKMUnitStatusTemplet.m_bShowGuide);
		if (num == 0)
		{
			return null;
		}
		return nKMUnitStatusTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
