using System.Globalization;
using System.Text.RegularExpressions;

namespace UnityEngine.UI.Extensions.ColorPicker;

[RequireComponent(typeof(InputField))]
public class HexColorField : MonoBehaviour
{
	public ColorPickerControl ColorPicker;

	public bool displayAlpha;

	private InputField hexInputField;

	private const string hexRegex = "^#?(?:[0-9a-fA-F]{3,4}){1,2}$";

	private void Awake()
	{
		hexInputField = GetComponent<InputField>();
		hexInputField.onEndEdit.AddListener(UpdateColor);
		ColorPicker.onValueChanged.AddListener(UpdateHex);
	}

	private void OnDestroy()
	{
		hexInputField.onValueChanged.RemoveListener(UpdateColor);
		ColorPicker.onValueChanged.RemoveListener(UpdateHex);
	}

	private void UpdateHex(Color newColor)
	{
		hexInputField.text = ColorToHex(newColor);
	}

	private void UpdateColor(string newHex)
	{
		if (HexToColor(newHex, out var color))
		{
			ColorPicker.CurrentColor = color;
		}
		else
		{
			Debug.Log("hex value is in the wrong format, valid formats are: #RGB, #RGBA, #RRGGBB and #RRGGBBAA (# is optional)");
		}
	}

	private string ColorToHex(Color32 color)
	{
		if (displayAlpha)
		{
			return $"#{color.r:X2}{color.g:X2}{color.b:X2}{color.a:X2}";
		}
		return $"#{color.r:X2}{color.g:X2}{color.b:X2}";
	}

	public static bool HexToColor(string hex, out Color32 color)
	{
		if (Regex.IsMatch(hex, "^#?(?:[0-9a-fA-F]{3,4}){1,2}$"))
		{
			int num = (hex.StartsWith("#") ? 1 : 0);
			if (hex.Length == num + 8)
			{
				color = new Color32(byte.Parse(hex.Substring(num, 2), NumberStyles.AllowHexSpecifier), byte.Parse(hex.Substring(num + 2, 2), NumberStyles.AllowHexSpecifier), byte.Parse(hex.Substring(num + 4, 2), NumberStyles.AllowHexSpecifier), byte.Parse(hex.Substring(num + 6, 2), NumberStyles.AllowHexSpecifier));
			}
			else if (hex.Length == num + 6)
			{
				color = new Color32(byte.Parse(hex.Substring(num, 2), NumberStyles.AllowHexSpecifier), byte.Parse(hex.Substring(num + 2, 2), NumberStyles.AllowHexSpecifier), byte.Parse(hex.Substring(num + 4, 2), NumberStyles.AllowHexSpecifier), byte.MaxValue);
			}
			else if (hex.Length == num + 4)
			{
				color = new Color32(byte.Parse(hex[num].ToString() + hex[num], NumberStyles.AllowHexSpecifier), byte.Parse(hex[num + 1].ToString() + hex[num + 1], NumberStyles.AllowHexSpecifier), byte.Parse(hex[num + 2].ToString() + hex[num + 2], NumberStyles.AllowHexSpecifier), byte.Parse(hex[num + 3].ToString() + hex[num + 3], NumberStyles.AllowHexSpecifier));
			}
			else
			{
				color = new Color32(byte.Parse(hex[num].ToString() + hex[num], NumberStyles.AllowHexSpecifier), byte.Parse(hex[num + 1].ToString() + hex[num + 1], NumberStyles.AllowHexSpecifier), byte.Parse(hex[num + 2].ToString() + hex[num + 2], NumberStyles.AllowHexSpecifier), byte.MaxValue);
			}
			return true;
		}
		color = default(Color32);
		return false;
	}
}
