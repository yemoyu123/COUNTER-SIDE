using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NKC.UI.Component;

public class NKCUIComDragSelectablePanel : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler, IScrollHandler
{
	public delegate RectTransform OnGetObject();

	public delegate void OnReturnObject(RectTransform rect);

	public delegate void OnProvideData(RectTransform rect, int idx);

	public delegate void OnFocus(RectTransform rect, bool bIsFocus);

	public delegate void OnFocusColor(RectTransform rect, Color setColor);

	public delegate void OnIndexChangeListener(int index);

	[Header("Content Rect. 각 아이템은 이 Rect의 사이즈와 같다고 가정")]
	public RectTransform m_rtContentRect;

	[Header("이전/다음 아이템 버튼. Nullable")]
	public NKCUIComStateButton m_csbtnPrev;

	public NKCUIComStateButton m_csbtnNext;

	[Header("Dot Scroll Indicator. Nullable")]
	public NKCUIComDotList m_dotList;

	[Header("스크롤 방향")]
	public Vector2 m_vScrollDirection = new Vector2(1f, 0f);

	[Header("Spacing")]
	public Vector2 Spacing;

	[Header("다음 칸으로 이동하는 시간")]
	public float m_fMenuMoveTime = 0.4f;

	[Header("자동으로 다음 칸으로 넘어가는지 여부")]
	public bool m_bAutoMove;

	[Header("자동으로 다음 칸으로 넘어갈때까지의 시간")]
	public float m_fAutoMoveTime;

	[Header("메뉴 돌아갈때 사용할 Ease Function")]
	public Ease m_eEase = Ease.OutQuad;

	[Header("드래그 거리 대비 이동 속도")]
	public float m_fMoveRate = 1f;

	[Header("끝까지 드래그시 메뉴 몇개분만큼 이동하는지. 스크롤 방향을 변경했는데 드래그시 심하게 튄다면 이 값의 +-를 반대로 할 것")]
	public float m_fDragLimitRate = 0.4f;

	[Header("얼마 이상 움직였어야 다음 메뉴로 넘어가는가?")]
	public float m_fMoveThreshold = 10f;

	[Header("배너를 반복하는가?")]
	public bool m_bBannerRotation;

	[Header("Focus에 따른 색상 변화를 적용하는가?")]
	public Color m_FocusColor = Color.white;

	public Color m_UnFocusColor = Color.gray;

	private float m_fOffset;

	private bool m_bPauseUpdate;

	private float m_fAutomoveDeltaTime;

	private int _totalCount;

	private int m_currentIndex;

	private RectTransform currentItem;

	private RectTransform prevItem;

	private RectTransform nextItem;

	public Vector2 ScrollDirection => m_vScrollDirection.normalized;

	public int TotalCount
	{
		get
		{
			return _totalCount;
		}
		set
		{
			_totalCount = value;
			if (m_dotList != null)
			{
				m_dotList.SetMaxCount(value);
			}
		}
	}

	public int CurrentIndex
	{
		get
		{
			return m_currentIndex;
		}
		set
		{
			m_currentIndex = value;
		}
	}

	private float OneMenuOffsetSize => Vector2.Dot(m_rtContentRect.GetSize(), ScrollDirection);

	public event OnGetObject dOnGetObject;

	public event OnReturnObject dOnReturnObject;

	public event OnProvideData dOnProvideData;

	public event OnFocus dOnFocus;

	public event OnFocusColor dOnFocusColor;

	public event OnIndexChangeListener dOnIndexChangeListener;

	public float GetDragOffset()
	{
		return m_fOffset;
	}

	public RectTransform GetCurrentItem()
	{
		return currentItem;
	}

	private RectTransform GetObject()
	{
		RectTransform rectTransform = this.dOnGetObject?.Invoke();
		if (rectTransform != null)
		{
			rectTransform.SetParent(m_rtContentRect, worldPositionStays: false);
		}
		return rectTransform;
	}

	private void ReturnObject(RectTransform rt)
	{
		if (rt != null)
		{
			this.dOnReturnObject?.Invoke(rt);
		}
	}

	private void SetFocus(RectTransform rt, bool bIsFocus)
	{
		if (rt == null || !m_bBannerRotation)
		{
			return;
		}
		if (this.dOnFocus != null)
		{
			this.dOnFocus(rt, bIsFocus);
		}
		if (this.dOnFocusColor != null)
		{
			if (bIsFocus)
			{
				this.dOnFocusColor(rt, m_FocusColor);
			}
			else
			{
				this.dOnFocusColor(rt, m_UnFocusColor);
			}
		}
	}

	private void ProvideData(RectTransform rt, int index, int changeVal = 0)
	{
		if (rt == null)
		{
			return;
		}
		if (m_bBannerRotation)
		{
			if (index < 0)
			{
				index = TotalCount - 1;
			}
			if (index > TotalCount - 1)
			{
				index = 0;
			}
		}
		else
		{
			if (index < 0)
			{
				NKCUtil.SetGameobjectActive(rt, bValue: false);
				return;
			}
			if (index >= TotalCount)
			{
				NKCUtil.SetGameobjectActive(rt, bValue: false);
				return;
			}
		}
		NKCUtil.SetGameobjectActive(rt, bValue: true);
		this.dOnProvideData?.Invoke(rt, index);
	}

	public void Init(bool rotation = false, bool bUseShortcut = true)
	{
		if (m_rtContentRect == null)
		{
			Debug.LogError(base.gameObject.name + " : Content Rect Null!");
			return;
		}
		if (m_csbtnPrev != null)
		{
			m_csbtnPrev.PointerClick.RemoveAllListeners();
			m_csbtnPrev.PointerClick.AddListener(ToPrevItem);
			if (bUseShortcut && m_csbtnPrev.m_HotkeyEventType == HotkeyEventType.None)
			{
				NKCUtil.SetHotkey(m_csbtnPrev, NKCInputManager.GetDirection(-m_vScrollDirection));
			}
		}
		if (m_csbtnNext != null)
		{
			m_csbtnNext.PointerClick.RemoveAllListeners();
			m_csbtnNext.PointerClick.AddListener(ToNextItem);
			if (bUseShortcut && m_csbtnNext.m_HotkeyEventType == HotkeyEventType.None)
			{
				NKCUtil.SetHotkey(m_csbtnNext, NKCInputManager.GetDirection(m_vScrollDirection));
			}
		}
		m_bBannerRotation = rotation;
	}

	public void Prepare()
	{
		ReturnObject(currentItem);
		ReturnObject(nextItem);
		ReturnObject(prevItem);
		currentItem = GetObject();
		nextItem = GetObject();
		prevItem = GetObject();
	}

	public void SetIndex(int index)
	{
		if (currentItem == null)
		{
			Prepare();
		}
		m_currentIndex = index;
		if (m_bBannerRotation)
		{
			if (index + 1 >= TotalCount)
			{
				ProvideData(prevItem, index - 1);
				ProvideData(currentItem, index);
				ProvideData(nextItem, 0);
			}
			else if (index == 0)
			{
				ProvideData(prevItem, TotalCount - 1);
				ProvideData(currentItem, index);
				ProvideData(nextItem, index + 1);
			}
			else
			{
				ProvideData(prevItem, index - 1);
				ProvideData(currentItem, index);
				ProvideData(nextItem, index + 1);
			}
		}
		else
		{
			ProvideData(prevItem, index - 1);
			SetFocus(prevItem, bIsFocus: false);
			ProvideData(currentItem, index);
			SetFocus(currentItem, bIsFocus: true);
			ProvideData(nextItem, index + 1);
			SetFocus(nextItem, bIsFocus: false);
		}
		SetLocationToPrev(prevItem, Vector2.zero);
		SetLocationToCenter(currentItem, Vector2.zero);
		SetLocationToNext(nextItem, Vector2.zero);
		SetSubComponents();
	}

	private void ChangeMenuByOffset(float offset)
	{
		if (offset >= m_fMoveThreshold)
		{
			ToPrevItem();
		}
		else if (offset <= 0f - m_fMoveThreshold)
		{
			ToNextItem();
		}
		else
		{
			Reposition(bAnim: true);
		}
	}

	private void ToPrevItem()
	{
		if (m_bBannerRotation)
		{
			if (m_currentIndex == 0)
			{
				m_currentIndex = TotalCount - 1;
			}
			else
			{
				m_currentIndex--;
			}
		}
		else
		{
			if (m_currentIndex == 0)
			{
				Reposition(bAnim: true);
				return;
			}
			m_currentIndex--;
		}
		RectTransform rectTransform = nextItem;
		nextItem = currentItem;
		currentItem = prevItem;
		prevItem = rectTransform;
		ProvideData(prevItem, m_currentIndex - 1, -1);
		SetLocationToPrev(prevItem, GetPrevPos() + Rubber(m_fOffset, OneMenuOffsetSize * m_fDragLimitRate) * ScrollDirection);
		Reposition(bAnim: true);
		if (!m_bBannerRotation)
		{
			SetFocus(currentItem, bIsFocus: true);
			SetFocus(nextItem, bIsFocus: false);
		}
		m_fAutomoveDeltaTime = 0f;
		SetSubComponents();
	}

	private void ToNextItem()
	{
		if (m_bBannerRotation)
		{
			if (m_currentIndex + 1 >= TotalCount)
			{
				m_currentIndex = 0;
			}
			else
			{
				m_currentIndex++;
			}
		}
		else
		{
			if (m_currentIndex + 1 >= TotalCount)
			{
				Reposition(bAnim: true);
				return;
			}
			m_currentIndex++;
		}
		RectTransform rectTransform = prevItem;
		prevItem = currentItem;
		currentItem = nextItem;
		nextItem = rectTransform;
		ProvideData(nextItem, m_currentIndex + 1, 1);
		SetLocationToNext(nextItem, GetNextPos() + Rubber(m_fOffset, OneMenuOffsetSize * m_fDragLimitRate) * ScrollDirection);
		Reposition(bAnim: true);
		if (!m_bBannerRotation)
		{
			SetFocus(currentItem, bIsFocus: true);
			SetFocus(prevItem, bIsFocus: false);
		}
		m_fAutomoveDeltaTime = 0f;
		SetSubComponents();
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (!m_bBannerRotation || TotalCount > 1)
		{
			ChangeMenuByOffset(m_fOffset);
		}
		m_fOffset = 0f;
		if (m_bBannerRotation && TotalCount == 1)
		{
			ChangeMenuByOffset(m_fOffset);
		}
		m_bPauseUpdate = false;
		m_fAutomoveDeltaTime = 0f;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		m_fOffset = 0f;
		if (!m_bBannerRotation || TotalCount > 1)
		{
			prevItem.DOComplete();
			currentItem.DOComplete();
			nextItem.DOComplete();
		}
		m_bPauseUpdate = true;
		m_fAutomoveDeltaTime = 0f;
	}

	public void OnDrag(PointerEventData eventData)
	{
		Vector2 lhs = eventData.delta * m_fMoveRate;
		m_fOffset += Vector2.Dot(lhs, ScrollDirection);
		Vector2 offset = Rubber(m_fOffset, OneMenuOffsetSize * m_fDragLimitRate) * ScrollDirection;
		SetLocationToPrev(prevItem, offset);
		SetLocationToCenter(currentItem, offset);
		SetLocationToNext(nextItem, offset);
	}

	private float Rubber(float currentValue, float Limit)
	{
		if (currentValue == 0f)
		{
			return 0f;
		}
		float num = Mathf.Abs(currentValue);
		return Limit * num / (Limit + num) * Mathf.Sign(currentValue);
	}

	public void OnScroll(PointerEventData eventData)
	{
		m_fAutomoveDeltaTime = 0f;
		Vector2 scrollDelta = eventData.scrollDelta;
		if (scrollDelta.y > 0f)
		{
			ToPrevItem();
		}
		else if (scrollDelta.y < 0f)
		{
			ToNextItem();
		}
	}

	public void CleanUp()
	{
		ReturnObject(prevItem);
		ReturnObject(currentItem);
		ReturnObject(nextItem);
		prevItem = null;
		currentItem = null;
		nextItem = null;
	}

	private void SetSubComponents()
	{
		SetDot();
		NotifyIndexChange();
		SetArrow();
		ChangeFocusColor();
	}

	public void SetArrow(bool forceHide = false)
	{
		if (forceHide)
		{
			NKCUtil.SetGameobjectActive(m_csbtnPrev, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnNext, bValue: false);
		}
		else if (m_bBannerRotation)
		{
			NKCUtil.SetGameobjectActive(m_csbtnPrev, TotalCount > 1);
			NKCUtil.SetGameobjectActive(m_csbtnNext, TotalCount > 1);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_csbtnPrev, m_currentIndex != 0);
			NKCUtil.SetGameobjectActive(m_csbtnNext, m_currentIndex < TotalCount - 1);
		}
	}

	private void ChangeFocusColor()
	{
		SetFocus(prevItem, bIsFocus: false);
		SetFocus(currentItem, bIsFocus: true);
		SetFocus(nextItem, bIsFocus: false);
	}

	private void SetDot()
	{
		if (!(m_dotList == null))
		{
			m_dotList.SetIndex(m_currentIndex);
		}
	}

	private void NotifyIndexChange()
	{
		if ((!m_bBannerRotation || TotalCount > 0) && this.dOnIndexChangeListener != null)
		{
			this.dOnIndexChangeListener(m_currentIndex);
		}
	}

	private void Reposition(bool bAnim)
	{
		if (bAnim)
		{
			MoveLocationToPrev(prevItem);
			MoveLocationToCenter(currentItem);
			MoveLocationToNext(nextItem);
		}
		else
		{
			SetLocationToPrev(prevItem, Vector2.zero);
			SetLocationToCenter(currentItem, Vector2.zero);
			SetLocationToNext(nextItem, Vector2.zero);
		}
	}

	private Vector2 GetPrevPos()
	{
		return -m_rtContentRect.GetSize() * ScrollDirection - Spacing;
	}

	private Vector2 GetCurrentPos()
	{
		return Vector2.zero;
	}

	private Vector2 GetNextPos()
	{
		return m_rtContentRect.GetSize() * ScrollDirection + Spacing;
	}

	private void SetLocationToPrev(RectTransform target, Vector2 offset)
	{
		if (target != null)
		{
			target.DOKill();
			target.anchoredPosition = GetPrevPos() + offset;
		}
	}

	private void MoveLocationToPrev(RectTransform target)
	{
		if (target != null)
		{
			target.DOKill();
			target.DOAnchorPos(GetPrevPos(), m_fMenuMoveTime).SetEase(m_eEase);
		}
	}

	private void SetLocationToCenter(RectTransform target, Vector2 offset)
	{
		if (target != null)
		{
			target.DOKill();
			target.anchoredPosition = GetCurrentPos() + offset;
			target.transform.SetAsLastSibling();
		}
	}

	private void MoveLocationToCenter(RectTransform target)
	{
		if (target != null)
		{
			target.DOKill();
			target.DOAnchorPos(GetCurrentPos(), m_fMenuMoveTime).SetEase(m_eEase);
			target.transform.SetAsLastSibling();
		}
	}

	private void SetLocationToNext(RectTransform target, Vector2 offset)
	{
		if (target != null)
		{
			target.DOKill();
			target.anchoredPosition = GetNextPos() + offset;
		}
	}

	private void MoveLocationToNext(RectTransform target)
	{
		if (target != null)
		{
			target.DOKill();
			target.DOAnchorPos(GetNextPos(), m_fMenuMoveTime).SetEase(m_eEase);
		}
	}

	public void Update()
	{
		if (!m_bPauseUpdate && m_bAutoMove && TotalCount > 1 && m_fAutoMoveTime > 0f)
		{
			m_fAutomoveDeltaTime += Time.deltaTime;
			if (m_fAutomoveDeltaTime > m_fAutoMoveTime)
			{
				m_fAutomoveDeltaTime = 0f;
				ToNextItem();
			}
		}
	}
}
