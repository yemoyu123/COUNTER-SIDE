using NKC.UI;
using NKM;

namespace NKC;

public class NKC_SCEN_INVENTORY : NKC_SCEN_BASIC
{
	private NKCUIInventory.NKC_INVENTORY_TAB m_reservedOpenType = NKCUIInventory.NKC_INVENTORY_TAB.NIT_NONE;

	public NKC_SCEN_INVENTORY()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_INVENTORY;
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
	}

	public override void ScenLoadComplete()
	{
		base.ScenLoadComplete();
	}

	public override void ScenStart()
	{
		base.ScenStart();
		Open();
		NKCCamera.EnableBloom(bEnable: false);
	}

	public override void ScenEnd()
	{
		NKCUIForge.CheckInstanceAndClose();
		base.ScenEnd();
		Close();
	}

	public void Open()
	{
		NKCUIInventory.EquipSelectListOptions options = new NKCUIInventory.EquipSelectListOptions(NKC_INVENTORY_OPEN_TYPE.NIOT_NORMAL, _bMultipleSelect: false);
		options.strEmptyMessage = NKCUtilString.GET_STRING_INVEN_MISC_NO_EXIST;
		NKCUIInventory.Instance.Open(options, null, 0L, m_reservedOpenType);
		m_reservedOpenType = NKCUIInventory.NKC_INVENTORY_TAB.NIT_NONE;
	}

	public void Close()
	{
		NKCUIInventory.Instance.Close();
	}

	public void OnRemoveEquipItemAck()
	{
		if (NKCUIInventory.IsInstanceOpen)
		{
			NKCUIInventory.Instance.OnRemoveMode(bValue: false);
		}
	}

	public override void ScenUpdate()
	{
		base.ScenUpdate();
	}

	public override bool ScenMsgProc(NKCMessageData cNKCMessageData)
	{
		return false;
	}

	public void SetReservedOpenTyp(NKCUIInventory.NKC_INVENTORY_TAB openType)
	{
		m_reservedOpenType = openType;
	}
}
