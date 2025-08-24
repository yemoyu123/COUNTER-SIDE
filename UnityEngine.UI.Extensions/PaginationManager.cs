using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.UI.Extensions;

[AddComponentMenu("UI/Extensions/Pagination Manager")]
public class PaginationManager : ToggleGroup
{
	private List<Toggle> m_PaginationChildren;

	[SerializeField]
	private ScrollSnapBase scrollSnap;

	private bool isAClick;

	public int CurrentPage => scrollSnap.CurrentPage;

	protected PaginationManager()
	{
	}

	protected override void Start()
	{
		base.Start();
		if (scrollSnap == null)
		{
			Debug.LogError("A ScrollSnap script must be attached");
			return;
		}
		if ((bool)scrollSnap.Pagination)
		{
			scrollSnap.Pagination = null;
		}
		scrollSnap.OnSelectionPageChangedEvent.AddListener(SetToggleGraphics);
		scrollSnap.OnSelectionChangeEndEvent.AddListener(OnPageChangeEnd);
		ResetPaginationChildren();
	}

	public void ResetPaginationChildren()
	{
		m_PaginationChildren = GetComponentsInChildren<Toggle>().ToList();
		for (int i = 0; i < m_PaginationChildren.Count; i++)
		{
			m_PaginationChildren[i].onValueChanged.AddListener(ToggleClick);
			m_PaginationChildren[i].group = this;
			m_PaginationChildren[i].isOn = false;
		}
		SetToggleGraphics(CurrentPage);
		if (m_PaginationChildren.Count != scrollSnap._scroll_rect.content.childCount)
		{
			Debug.LogWarning("Uneven pagination icon to page count");
		}
	}

	public void GoToScreen(int pageNo)
	{
		scrollSnap.GoToScreen(pageNo, pagination: true);
	}

	private void ToggleClick(Toggle target)
	{
		if (!target.isOn)
		{
			isAClick = true;
			GoToScreen(m_PaginationChildren.IndexOf(target));
		}
	}

	private void ToggleClick(bool toggle)
	{
		if (!toggle)
		{
			return;
		}
		for (int i = 0; i < m_PaginationChildren.Count; i++)
		{
			if (m_PaginationChildren[i].isOn && !scrollSnap._suspendEvents)
			{
				GoToScreen(i);
				break;
			}
		}
	}

	private void ToggleClick(int target)
	{
		isAClick = true;
		GoToScreen(target);
	}

	private void SetToggleGraphics(int pageNo)
	{
		if (!isAClick)
		{
			m_PaginationChildren[pageNo].isOn = true;
		}
	}

	private void OnPageChangeEnd(int pageNo)
	{
		isAClick = false;
	}
}
