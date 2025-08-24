using UnityEngine;

namespace NKC.UI;

[RequireComponent(typeof(NKCUISlot))]
public class NKCUISlotCustomData : MonoBehaviour
{
	public delegate void OnClick(NKCUISlot.SlotData slotData, bool bLocked, int data);

	public int m_Data;

	private OnClick dOnClick;

	public NKCUISlot Slot { get; private set; }

	private void Awake()
	{
		Slot = GetComponent<NKCUISlot>();
	}

	public void Init()
	{
		Slot.Init();
	}

	public void SetData(int data, NKCUISlot.SlotData slotData, bool bEnableLayoutElement = true, OnClick onClick = null)
	{
		Slot.SetData(slotData, bEnableLayoutElement, OnSlotClick);
		m_Data = data;
		dOnClick = onClick;
	}

	public void SetData(int data, NKCUISlot.SlotData slotData, bool bShowName, bool bShowNumber, bool bEnableLayoutElement, OnClick onClick)
	{
		Slot.SetData(slotData, bShowName, bShowNumber, bEnableLayoutElement, OnSlotClick);
		m_Data = data;
		dOnClick = onClick;
	}

	public void SetOnClick(OnClick onClick)
	{
		Slot.SetOnClick(OnSlotClick);
		dOnClick = onClick;
	}

	private void OnSlotClick(NKCUISlot.SlotData slotData, bool bLocked)
	{
		dOnClick?.Invoke(slotData, bLocked, m_Data);
	}
}
