using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace UnityEngine.UI.Extensions;

[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("UI/Extensions/AutoComplete ComboBox")]
public class AutoCompleteComboBox : MonoBehaviour
{
	[Serializable]
	public class SelectionChangedEvent : UnityEvent<string, bool>
	{
	}

	[Serializable]
	public class SelectionTextChangedEvent : UnityEvent<string>
	{
	}

	[Serializable]
	public class SelectionValidityChangedEvent : UnityEvent<bool>
	{
	}

	public Color disabledTextColor;

	public List<string> AvailableOptions;

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

	private List<string> _prunedPanelItems;

	private Dictionary<string, GameObject> panelObjects;

	private GameObject itemTemplate;

	[SerializeField]
	private float _scrollBarWidth = 20f;

	[SerializeField]
	private int _itemsToDisplay;

	public bool SelectFirstItemOnStart;

	[SerializeField]
	[Tooltip("Change input text color based on matching items")]
	private bool _ChangeInputTextColorBasedOnMatchingItems;

	public float DropdownOffset = 10f;

	public Color ValidSelectionTextColor = Color.green;

	public Color MatchingItemsRemainingTextColor = Color.black;

	public Color NoItemsRemainingTextColor = Color.red;

	public AutoCompleteSearchType autocompleteSearchType = AutoCompleteSearchType.Linq;

	[SerializeField]
	private bool _displayPanelAbove;

	private bool _selectionIsValid;

	public SelectionTextChangedEvent OnSelectionTextChanged;

	public SelectionValidityChangedEvent OnSelectionValidityChanged;

	public SelectionChangedEvent OnSelectionChanged;

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

	public bool InputColorMatching
	{
		get
		{
			return _ChangeInputTextColorBasedOnMatchingItems;
		}
		set
		{
			_ChangeInputTextColorBasedOnMatchingItems = value;
			if (_ChangeInputTextColorBasedOnMatchingItems)
			{
				SetInputTextColor();
			}
		}
	}

	public void Awake()
	{
		Initialize();
	}

	public void Start()
	{
		if (SelectFirstItemOnStart && AvailableOptions.Count > 0)
		{
			ToggleDropdownPanel();
			OnItemClicked(AvailableOptions[0]);
		}
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
		_prunedPanelItems = new List<string>();
		_panelItems = new List<string>();
		RebuildPanel();
		return result;
	}

	public void AddItem(string item)
	{
		if (!AvailableOptions.Contains(item))
		{
			AvailableOptions.Add(item);
			RebuildPanel();
		}
		else
		{
			Debug.LogWarning("AutoCompleteComboBox.AddItem: items may only exists once. '" + item + "' can not be added.");
		}
	}

	public void RemoveItem(string item)
	{
		if (AvailableOptions.Contains(item))
		{
			AvailableOptions.Remove(item);
			RebuildPanel();
		}
	}

	public void SetAvailableOptions(List<string> newOptions)
	{
		List<string> list = newOptions.Distinct().ToList();
		if (newOptions.Count != list.Count)
		{
			Debug.LogWarning(string.Format("{0}.{1}: items may only exists once. {2} duplicates.", "AutoCompleteComboBox", "SetAvailableOptions", newOptions.Count - list.Count));
		}
		AvailableOptions.Clear();
		AvailableOptions = list;
		RebuildPanel();
	}

	public void SetAvailableOptions(string[] newOptions)
	{
		List<string> list = newOptions.Distinct().ToList();
		if (newOptions.Length != list.Count)
		{
			Debug.LogWarning(string.Format("{0}.{1}: items may only exists once. {2} duplicates.", "AutoCompleteComboBox", "SetAvailableOptions", newOptions.Length - list.Count));
		}
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
		if (_isPanelActive)
		{
			ToggleDropdownPanel();
		}
		_panelItems.Clear();
		_prunedPanelItems.Clear();
		panelObjects.Clear();
		foreach (Transform item in _itemsPanelRT.transform)
		{
			Object.Destroy(item.gameObject);
		}
		foreach (string availableOption in AvailableOptions)
		{
			_panelItems.Add(availableOption.ToLower());
		}
		List<GameObject> list = new List<GameObject>(panelObjects.Values);
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
		SetInputTextColor();
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
			_scrollPanelRT.anchoredPosition = (_displayPanelAbove ? new Vector2(0f, DropdownOffset + _rectTransform.sizeDelta.y * (float)_panelItems.Count - 1f) : new Vector2(0f, 0f - _rectTransform.sizeDelta.y));
			_overlayRT.SetParent(_canvas.transform, worldPositionStays: false);
			_overlayRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _canvasRT.sizeDelta.x);
			_overlayRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _canvasRT.sizeDelta.y);
			_overlayRT.SetParent(base.transform, worldPositionStays: true);
			_scrollPanelRT.SetParent(_overlayRT, worldPositionStays: true);
		}
		if (_panelItems.Count >= 1)
		{
			float num2 = _rectTransform.sizeDelta.y * (float)Mathf.Min(_itemsToDisplay, _panelItems.Count) + DropdownOffset;
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
		PruneItems(currText);
		RedrawPanel();
		if (_panelItems.Count == 0)
		{
			_isPanelActive = true;
			ToggleDropdownPanel();
		}
		else if (!_isPanelActive)
		{
			ToggleDropdownPanel();
		}
		bool num = _panelItems.Contains(Text) != _selectionIsValid;
		_selectionIsValid = _panelItems.Contains(Text);
		OnSelectionChanged.Invoke(Text, _selectionIsValid);
		OnSelectionTextChanged.Invoke(Text);
		if (num)
		{
			OnSelectionValidityChanged.Invoke(_selectionIsValid);
		}
		SetInputTextColor();
	}

	private void SetInputTextColor()
	{
		if (InputColorMatching)
		{
			if (_selectionIsValid)
			{
				_mainInput.textComponent.color = ValidSelectionTextColor;
			}
			else if (_panelItems.Count > 0)
			{
				_mainInput.textComponent.color = MatchingItemsRemainingTextColor;
			}
			else
			{
				_mainInput.textComponent.color = NoItemsRemainingTextColor;
			}
		}
	}

	public void ToggleDropdownPanel(bool directClick = false)
	{
		_isPanelActive = !_isPanelActive;
		_overlayRT.gameObject.SetActive(_isPanelActive);
		if (_isPanelActive)
		{
			base.transform.SetAsLastSibling();
		}
	}

	private void PruneItems(string currText)
	{
		if (autocompleteSearchType == AutoCompleteSearchType.Linq)
		{
			PruneItemsLinq(currText);
		}
		else
		{
			PruneItemsArray(currText);
		}
	}

	private void PruneItemsLinq(string currText)
	{
		currText = currText.ToLower();
		string[] array = _panelItems.Where((string x) => !x.Contains(currText)).ToArray();
		foreach (string text in array)
		{
			panelObjects[text].SetActive(value: false);
			_panelItems.Remove(text);
			_prunedPanelItems.Add(text);
		}
		array = _prunedPanelItems.Where((string x) => x.Contains(currText)).ToArray();
		foreach (string text2 in array)
		{
			panelObjects[text2].SetActive(value: true);
			_panelItems.Add(text2);
			_prunedPanelItems.Remove(text2);
		}
	}

	private void PruneItemsArray(string currText)
	{
		string value = currText.ToLower();
		for (int num = _panelItems.Count - 1; num >= 0; num--)
		{
			string text = _panelItems[num];
			if (!text.Contains(value))
			{
				panelObjects[_panelItems[num]].SetActive(value: false);
				_panelItems.RemoveAt(num);
				_prunedPanelItems.Add(text);
			}
		}
		for (int num2 = _prunedPanelItems.Count - 1; num2 >= 0; num2--)
		{
			string text2 = _prunedPanelItems[num2];
			if (text2.Contains(value))
			{
				panelObjects[_prunedPanelItems[num2]].SetActive(value: true);
				_prunedPanelItems.RemoveAt(num2);
				_panelItems.Add(text2);
			}
		}
	}
}
