using UnityEngine;

namespace NKC;

[RequireComponent(typeof(Canvas))]
public class NKCUIComCanvasSortingGroup : MonoBehaviour
{
	public int SortingLayer;

	public int SortingOrder;

	private void Awake()
	{
		Canvas component = GetComponent<Canvas>();
		if (component != null)
		{
			component.sortingLayerID = SortingLayer;
			component.sortingOrder = SortingOrder;
		}
	}
}
