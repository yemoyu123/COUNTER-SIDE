using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUITournamentScrollSnap : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IEndDragHandler, IScrollHandler
{
	public ScrollRect scrollRect;

	public RectTransform[] heightObjectFromBottom;

	private float[] fixVerticalNormanlizedPosition;

	private int index;

	private bool scrolling;

	public void OnBeginDrag(PointerEventData eventData)
	{
		ScrollBegin();
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		scrollRect.OnEndDrag(eventData);
		ScrollEnd(eventData.delta.y);
	}

	public void OnScroll(PointerEventData eventData)
	{
		if (!scrolling)
		{
			scrolling = true;
			ScrollBegin();
			ScrollEnd(0f - eventData.scrollDelta.y);
		}
	}

	private void ScrollBegin()
	{
		index = -1;
		if (heightObjectFromBottom == null)
		{
			return;
		}
		List<float> list = new List<float>();
		float num = 0f;
		float num2 = 0f;
		int num3 = heightObjectFromBottom.Length;
		int num4 = 0;
		for (int i = 0; i < num3; i++)
		{
			if (heightObjectFromBottom[i].gameObject.activeSelf)
			{
				float num5 = heightObjectFromBottom[i].sizeDelta.y * 0.5f;
				if (num4 == 0)
				{
					num = num5;
					num4++;
					continue;
				}
				float num6 = num + num5;
				list.Add(num6);
				num2 += num6;
				num = num5;
				num4++;
			}
		}
		int count = list.Count;
		float num7 = 0f;
		fixVerticalNormanlizedPosition = new float[count + 1];
		fixVerticalNormanlizedPosition[0] = 0f;
		for (int j = 0; j < count; j++)
		{
			num7 += list[j];
			fixVerticalNormanlizedPosition[j + 1] = num7 / num2;
		}
		int num8 = fixVerticalNormanlizedPosition.Length;
		float num9 = float.MaxValue;
		for (int k = 0; k < num8; k++)
		{
			float num10 = Mathf.Abs(fixVerticalNormanlizedPosition[k] - scrollRect.verticalNormalizedPosition);
			if (num9 > num10)
			{
				num9 = num10;
				index = k;
			}
		}
		scrollRect.content.DOKill();
	}

	private void ScrollEnd(float delta)
	{
		if (heightObjectFromBottom == null)
		{
			return;
		}
		if (delta > 0f)
		{
			if (index <= 0)
			{
				scrolling = false;
				return;
			}
			index = Mathf.Max(0, --index);
		}
		else if (delta < 0f)
		{
			if (index >= fixVerticalNormanlizedPosition.Length - 1)
			{
				scrolling = false;
				return;
			}
			index = Mathf.Min(fixVerticalNormanlizedPosition.Length - 1, ++index);
		}
		float endValue = scrollRect.content.sizeDelta.y * scrollRect.content.pivot.y - fixVerticalNormanlizedPosition[index] * scrollRect.content.sizeDelta.y;
		scrollRect.velocity = Vector2.zero;
		scrollRect.content.DOAnchorPosY(endValue, 0.5f).OnComplete(delegate
		{
			scrolling = false;
		});
	}
}
