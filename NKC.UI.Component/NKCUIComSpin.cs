using DG.Tweening;
using NKM;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NKC.UI.Component;

public class NKCUIComSpin : MonoBehaviour, IDragHandler, IEventSystemHandler, IEndDragHandler, IBeginDragHandler, IScrollHandler
{
	public delegate void OnSelectItem(int selectedIndex);

	public OnSelectItem dOnSelectItem;

	[Header("메뉴 몇 개?")]
	public int m_ItemCount = 3;

	[Header("메뉴 하나 돌아가는데 걸리는 시간")]
	public float m_fMenuSpinTime = 1f;

	[Header("드래그시 최고 속도")]
	public float MAX_ROT_SPEED = 2f;

	[Header("메뉴 돌아갈때 사용할 Ease Function")]
	public Ease m_eEase = Ease.OutBack;

	[Header("입력 대비 회전 속도")]
	public float m_fRate = -0.2f;

	[Header("회전시 좌우로 메뉴 몇개분이 더 돌아가는가")]
	public float m_fRotateLimitRate = 1.25f;

	[Header("몇 도 이상 회전해야 다음 메뉴로 넘어가는가?")]
	public float ROT_THRESHOLD = 6f;

	private float MENU_ANGLE = 120f;

	private float MAX_ROTATION = 150f;

	private int m_SelectedItemIndex;

	private float RotOffset;

	private NKCUIBase m_uiBase;

	private bool m_bDrag;

	public void Init(int itemCount, NKCUIBase uiBase)
	{
		m_uiBase = uiBase;
		m_ItemCount = itemCount;
		MENU_ANGLE = 360f / (float)itemCount;
		MAX_ROTATION = MENU_ANGLE * m_fRotateLimitRate;
		m_SelectedItemIndex = 0;
		base.transform.localRotation = Quaternion.identity;
	}

	private float GetMenuRotAngle(int menuindex)
	{
		return (float)menuindex * MENU_ANGLE;
	}

	private void SetItem(int menuIndex)
	{
		RotateToIndex(menuIndex);
		if (dOnSelectItem != null)
		{
			dOnSelectItem(menuIndex);
		}
	}

	public void RotateToIndex(int menuIndex, bool bAnimate = true)
	{
		m_SelectedItemIndex = menuIndex;
		base.transform.DOKill();
		if (!bAnimate)
		{
			base.transform.localRotation = Quaternion.Euler(0f, GetMenuRotAngle(menuIndex), 0f);
			return;
		}
		float num = NormalizeAngle(base.transform.localRotation.eulerAngles.y, (0f - MENU_ANGLE) * 0.5f);
		base.transform.localRotation = Quaternion.Euler(0f, num, 0f);
		float menuRotAngle = GetMenuRotAngle(menuIndex);
		float num2 = Mathf.Abs(menuRotAngle - num);
		ShortcutExtensions.DOLocalRotate(duration: (!(num2 >= MENU_ANGLE)) ? (num2 / MENU_ANGLE * m_fMenuSpinTime) : m_fMenuSpinTime, target: base.transform, endValue: new Vector3(0f, menuRotAngle, 0f)).SetEase(m_eEase);
	}

	private float NormalizeAngle(float value)
	{
		while (value >= 360f)
		{
			value -= 360f;
		}
		while (value < 0f)
		{
			value += 360f;
		}
		return value;
	}

	private float NormalizeAngle(float Value, float Offset)
	{
		return NormalizeAngle(Value - Offset) + Offset;
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (m_bDrag)
		{
			float num = MENU_ANGLE * Time.deltaTime * MAX_ROT_SPEED;
			float num2 = eventData.delta.x * m_fRate;
			if (num2 > num)
			{
				num2 = num;
			}
			else if (num2 < 0f - num)
			{
				num2 = 0f - num;
			}
			RotOffset += num2;
			num2 = ((RotOffset >= MAX_ROTATION) ? MAX_ROTATION : ((RotOffset <= 0f - MAX_ROTATION) ? (0f - MAX_ROTATION) : ((RotOffset > 0f) ? NKCUtil.TrackValue(TRACKING_DATA_TYPE.TDT_SLOWER, 0f, MAX_ROTATION, RotOffset, MAX_ROTATION) : ((!(RotOffset < 0f)) ? 0f : NKCUtil.TrackValue(TRACKING_DATA_TYPE.TDT_SLOWER, 0f, 0f - MAX_ROTATION, RotOffset, 0f - MAX_ROTATION)))));
			base.transform.localRotation = Quaternion.Euler(0f, GetMenuRotAngle(m_SelectedItemIndex) + num2, 0f);
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		m_bDrag = false;
		ChangeMenuByRotOffset(RotOffset);
		RotOffset = 0f;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		m_bDrag = true;
		RotOffset = 0f;
		base.transform.DOComplete();
	}

	public void OnScroll(PointerEventData eventData)
	{
		if (eventData.scrollDelta.y < 0f)
		{
			ChangeMenuByRotOffset(ROT_THRESHOLD);
		}
		else if (eventData.scrollDelta.y > 0f)
		{
			ChangeMenuByRotOffset(0f - ROT_THRESHOLD);
		}
	}

	private void ChangeMenuByRotOffset(float RotateOffset)
	{
		m_bDrag = false;
		int num;
		if (RotateOffset >= ROT_THRESHOLD)
		{
			num = m_SelectedItemIndex + 1;
			if (num >= m_ItemCount)
			{
				num = 0;
			}
		}
		else if (RotateOffset <= 0f - ROT_THRESHOLD)
		{
			num = m_SelectedItemIndex - 1;
			if (num < 0)
			{
				num = m_ItemCount - 1;
			}
		}
		else
		{
			num = m_SelectedItemIndex;
		}
		SetItem(num);
	}

	public float GetDeltaIndex(int index)
	{
		return NormalizeAngle(base.transform.localRotation.eulerAngles.y - GetMenuRotAngle(index), -180f) / MENU_ANGLE;
	}

	private void OnDisable()
	{
		m_bDrag = false;
		RotateToIndex(m_SelectedItemIndex, bAnimate: false);
	}

	private void Update()
	{
		if (NKCInputManager.CheckHotKeyEvent(HotkeyEventType.Left) && m_uiBase != null && NKCUIManager.CanUIProcessHotkey(m_uiBase))
		{
			ChangeMenuByRotOffset(0f - ROT_THRESHOLD);
			NKCInputManager.ConsumeHotKeyEvent(HotkeyEventType.Left);
		}
		if (NKCInputManager.CheckHotKeyEvent(HotkeyEventType.Right) && m_uiBase != null && NKCUIManager.CanUIProcessHotkey(m_uiBase))
		{
			ChangeMenuByRotOffset(ROT_THRESHOLD);
			NKCInputManager.ConsumeHotKeyEvent(HotkeyEventType.Right);
		}
		if (NKCInputManager.CheckHotKeyEvent(HotkeyEventType.ShowHotkey) && m_uiBase != null && NKCUIManager.CanUIProcessHotkey(m_uiBase))
		{
			NKCUIComHotkeyDisplay.OpenInstance(base.transform, HotkeyEventType.Left, HotkeyEventType.Right);
		}
	}
}
