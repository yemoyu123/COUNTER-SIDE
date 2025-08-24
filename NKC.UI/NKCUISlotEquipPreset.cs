using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

[RequireComponent(typeof(NKCUISlot))]
public class NKCUISlotEquipPreset : MonoBehaviour
{
	public Image m_imgEquipUnit;

	public NKCUISlot Slot { get; private set; }

	private void Awake()
	{
		Slot = GetComponent<NKCUISlot>();
	}

	public void Init()
	{
		Slot.Init();
		NKCUtil.SetGameobjectActive(m_imgEquipUnit.gameObject, bValue: false);
	}

	public void SetEquipUnitSprite(Sprite unitSprite)
	{
		if (unitSprite == null)
		{
			NKCUtil.SetGameobjectActive(m_imgEquipUnit.gameObject, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_imgEquipUnit.gameObject, bValue: true);
		NKCUtil.SetImageSprite(m_imgEquipUnit, unitSprite);
	}

	public void SetEmpty(NKCUISlot.OnClick dOnClick = null)
	{
		Slot.SetEmpty(dOnClick);
		NKCUtil.SetGameobjectActive(m_imgEquipUnit.gameObject, bValue: false);
	}
}
