using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace UnityEngine.UI.Extensions;

[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("UI/Extensions/ComboBox")]
public class ComboBox : MonoBehaviour
{
	[Serializable]
	public class SelectionChangedEvent : UnityEvent<string>
	{
	}

	public Color disabledTextColor;

	public List<string> AvailableOptions;

	[SerializeField]
	private float _scrollBarWidth = 20f;

	[SerializeField]
	private int _itemsToDisplay;

	[SerializeField]
	private bool _displayPanelAbove;

	public SelectionChangedEvent OnSelectionChanged;

	private bool _isPanelActive;

	private bool _hasDrawnOnce;

	private InputField _mainInput;

	private RectTransform _inputRT;

	private RectTransform _rectTransform;

	private RectTransform _overlayRT;

	private RectTransform _scrollPanelRT;

	private RectTransform _scrollBarRT;

	private RectTransform _slidingAreaRT;

	private RectTransform _scrollHandleRT;

	private RectTransform _itemsPanelRT;

	private Canvas _canvas;

	private RectTransform _canvasRT;

	private ScrollRect _scrollRect;

	private List<string> _panelItems;

	private Dictionary<string, GameObject> panelObjects;

	private GameObject itemTemplate;

	public DropDownListItem SelectedItem { get; private set; }

	public string Text { get; private set; }

	public float ScrollBarWidth
	{
		get
		{
			return _scrollBarWidth;
		}
		set
		{
			_scrollBarWidth = value;
			RedrawPanel();
		}
	}

	public int ItemsToDisplay
	{
		get
		{
			return _itemsToDisplay;
		}
		set
		{
			_itemsToDisplay = value;
			RedrawPanel();
		}
	}

	public void Awake()
	{
		Initialize();
	}

	public void Start()
	{
		RedrawPanel();
	}

	private bool Initialize()
	{
		bool result = true;
		try
		{
			_rectTransform = GetComponent<RectTransform>();
			_inputRT = _rectTransform.Find("InputField").GetComponent<RectTransform>();
			_mainInput = _inputRT.GetComponent<InputField>();
			_overlayRT = _rectTransform.Find("Overlay").GetComponent<RectTransform>();
			_overlayRT.gameObject.SetActive(value: false);
			_scrollPanelRT = _overlayRT.Find("ScrollPanel").GetComponent<RectTransform>();
			_scrollBarRT = _scrollPanelRT.Find("Scrollbar").GetComponent<RectTransform>();
			_slidingAreaRT = _scrollBarRT.Find("SlidingArea").GetComponent<RectTransform>();
			_scrollHandleRT = _slidingAreaRT.Find("Handle").GetComponent<RectTransform>();
			_itemsPanelRT = _scrollPanelRT.Find("Items").GetComponent<RectTransform>();
			_canvas = GetComponentInParent<Canvas>();
			_canvasRT = _canvas.GetComponent<RectTransform>();
			_scrollRect = _scrollPanelRT.GetComponent<ScrollRect>();
			_scrollRect.scrollSensitivity = _rectTransform.sizeDelta.y / 2f;
			_scrollRect.movementType = ScrollRect.MovementType.Clamped;
			_scrollRect.content = _itemsPanelRT;
			itemTemplate = _rectTransform.Find("ItemTemplate").gameObject;
			itemTemplate.SetActive(value: false);
		}
		catch (NullReferenceException exception)
		{
			Debug.LogException(exception);
			Debug.LogError("Something is setup incorrectly with the dropdownlist component causing a Null Reference Exception");
			result = false;
		}
		panelObjects = new Dictionary<string, GameObject>();
		_panelItems = AvailableOptions.ToList();
		RebuildPanel();
		return result;
	}

	public void AddItem(string item)
	{
		AvailableOptions.Add(item);
		RebuildPanel();
	}

	public void RemoveItem(string item)
	{
		AvailableOptions.Remove(item);
		RebuildPanel();
	}

	public void SetAvailableOptions(List<string> newOptions)
	{
		AvailableOptions.Clear();
		AvailableOptions = newOptions;
		RebuildPanel();
	}

	public void SetAvailableOptions(string[] newOptions)
	{
		AvailableOptions.Clear();
		for (int i = 0; i < newOptions.Length; i++)
		{
			AvailableOptions.Add(newOptions[i]);
		}
		RebuildPanel();
	}

	public void ResetItems()
	{
		AvailableOptions.Clear();
		RebuildPanel();
	}

	private void RebuildPanel()
	{
		_panelItems.Clear();
		foreach (string availableOption in AvailableOptions)
		{
			_panelItems.Add(availableOption.ToLower());
		}
		List<GameObject> list = new List<GameObject>(panelObjects.Values);
		panelObjects.Clear();
		int num = 0;
		while (list.Count < AvailableOptions.Count)
		{
			GameObject gameObject = Object.Instantiate(itemTemplate);
			gameObject.name = "Item " + num;
			gameObject.transform.SetParent(_itemsPanelRT, worldPositionStays: false);
			list.Add(gameObject);
			num++;
		}
		for (int i = 0; i < list.Count; i++)
		{
			list[i].SetActive(i <= AvailableOptions.Count);
			if (i < AvailableOptions.Count)
			{
				list[i].name = "Item " + i + " " + _panelItems[i];
				list[i].transform.Find("Text").GetComponent<Text>().text = AvailableOptions[i];
				Button component = list[i].GetComponent<Button>();
				component.onClick.RemoveAllListeners();
				string textOfItem = _panelItems[i];
				component.onClick.AddListener(delegate
				{
					OnItemClicked(textOfItem);
				});
				panelObjects[_panelItems[i]] = list[i];
			}
		}
	}

	private void OnItemClicked(string item)
	{
		Text = item;
		_mainInput.text = Text;
		ToggleDropdownPanel(directClick: true);
	}

	private void RedrawPanel()
	{
		float num = ((_panelItems.Count > ItemsToDisplay) ? _scrollBarWidth : 0f);
		_scrollBarRT.gameObject.SetActive(_panelItems.Count > ItemsToDisplay);
		if (!_hasDrawnOnce || _rectTransform.sizeDelta != _inputRT.sizeDelta)
		{
			_hasDrawnOnce = true;
			_inputRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _rectTransform.sizeDelta.x);
			_inputRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _rectTransform.sizeDelta.y);
			_scrollPanelRT.SetParent(base.transform, worldPositionStays: true);
			_scrollPanelRT.anchoredPosition = (_displayPanelAbove ? new Vector2(0f, _rectTransform.sizeDelta.y * (float)ItemsToDisplay - 1f) : new Vector2(0f, 0f - _rectTransform.sizeDelta.y));
			_overlayRT.SetParent(_canvas.transform, worldPositionStays: false);
			_overlayRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _canvasRT.sizeDelta.x);
			_overlayRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _canvasRT.sizeDelta.y);
			_overlayRT.SetParent(base.transform, worldPositionStays: true);
			_scrollPanelRT.SetParent(_overlayRT, worldPositionStays: true);
		}
		if (_panelItems.Count >= 1)
		{
			float num2 = _rectTransform.sizeDelta.y * (float)Mathf.Min(_itemsToDisplay, _panelItems.Count);
			_scrollPanelRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num2);
			_scrollPanelRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _rectTransform.sizeDelta.x);
			_itemsPanelRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _scrollPanelRT.sizeDelta.x - num - 5f);
			_itemsPanelRT.anchoredPosition = new Vector2(5f, 0f);
			_scrollBarRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num);
			_scrollBarRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num2);
			if (num == 0f)
			{
				_scrollHandleRT.gameObject.SetActive(value: false);
			}
			else
			{
				_scrollHandleRT.gameObject.SetActive(value: true);
			}
			_slidingAreaRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
			_slidingAreaRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num2 - _scrollBarRT.sizeDelta.x);
		}
	}

	public void OnValueChanged(string currText)
	{
		Text = currText;
		RedrawPanel();
		if (_panelItems.Count == 0)
		{
			_isPanelActive = true;
			ToggleDropdownPanel(directClick: false);
		}
		else if (!_isPanelActive)
		{
			ToggleDropdownPanel(directClick: false);
		}
		OnSelectionChanged.Invoke(Text);
	}

	public void ToggleDropdownPanel(bool directClick)
	{
		_isPanelActive = !_isPanelActive;
		_overlayRT.gameObject.SetActive(_isPanelActive);
		if (_isPanelActive)
		{
			base.transform.SetAsLastSibling();
		}
	}
}
