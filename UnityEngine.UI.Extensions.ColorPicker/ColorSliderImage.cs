using System;

namespace UnityEngine.UI.Extensions.ColorPicker;

[RequireComponent(typeof(RawImage))]
[ExecuteInEditMode]
public class ColorSliderImage : MonoBehaviour
{
	public ColorPickerControl picker;

	public ColorValues type;

	public Slider.Direction direction;

	private RawImage image;

	private RectTransform RectTransform => base.transform as RectTransform;

	private void Awake()
	{
		image = GetComponent<RawImage>();
		if ((bool)image)
		{
			RegenerateTexture();
		}
		else
		{
			Debug.LogWarning("Missing RawImage on object [" + base.name + "]");
		}
	}

	private void OnEnable()
	{
		if (picker != null && Application.isPlaying)
		{
			picker.onValueChanged.AddListener(ColorChanged);
			picker.onHSVChanged.AddListener(ColorChanged);
		}
	}

	private void OnDisable()
	{
		if (picker != null)
		{
			picker.onValueChanged.RemoveListener(ColorChanged);
			picker.onHSVChanged.RemoveListener(ColorChanged);
		}
	}

	private void OnDestroy()
	{
		if (image.texture != null)
		{
			Object.DestroyImmediate(image.texture);
		}
	}

	private void ColorChanged(Color newColor)
	{
		switch (type)
		{
		case ColorValues.R:
		case ColorValues.G:
		case ColorValues.B:
		case ColorValues.Saturation:
		case ColorValues.Value:
			RegenerateTexture();
			break;
		case ColorValues.A:
		case ColorValues.Hue:
			break;
		}
	}

	private void ColorChanged(float hue, float saturation, float value)
	{
		switch (type)
		{
		case ColorValues.R:
		case ColorValues.G:
		case ColorValues.B:
		case ColorValues.Saturation:
		case ColorValues.Value:
			RegenerateTexture();
			break;
		case ColorValues.A:
		case ColorValues.Hue:
			break;
		}
	}

	private void RegenerateTexture()
	{
		if (!picker)
		{
			Debug.LogWarning("Missing Picker on object [" + base.name + "]");
		}
		Color32 color = ((picker != null) ? picker.CurrentColor : Color.black);
		float num = ((picker != null) ? picker.H : 0f);
		float num2 = ((picker != null) ? picker.S : 0f);
		float num3 = ((picker != null) ? picker.V : 0f);
		bool flag = direction == Slider.Direction.BottomToTop || direction == Slider.Direction.TopToBottom;
		bool flag2 = direction == Slider.Direction.TopToBottom || direction == Slider.Direction.RightToLeft;
		int num4;
		switch (type)
		{
		case ColorValues.R:
		case ColorValues.G:
		case ColorValues.B:
		case ColorValues.A:
			num4 = 255;
			break;
		case ColorValues.Hue:
			num4 = 360;
			break;
		case ColorValues.Saturation:
		case ColorValues.Value:
			num4 = 100;
			break;
		default:
			throw new NotImplementedException("");
		}
		Texture2D texture2D = ((!flag) ? new Texture2D(num4, 1) : new Texture2D(1, num4));
		texture2D.hideFlags = HideFlags.DontSave;
		Color32[] array = new Color32[num4];
		switch (type)
		{
		case ColorValues.R:
		{
			for (byte b3 = 0; b3 < num4; b3++)
			{
				array[flag2 ? (num4 - 1 - b3) : b3] = new Color32(b3, color.g, color.b, byte.MaxValue);
			}
			break;
		}
		case ColorValues.G:
		{
			for (byte b = 0; b < num4; b++)
			{
				array[flag2 ? (num4 - 1 - b) : b] = new Color32(color.r, b, color.b, byte.MaxValue);
			}
			break;
		}
		case ColorValues.B:
		{
			for (byte b2 = 0; b2 < num4; b2++)
			{
				array[flag2 ? (num4 - 1 - b2) : b2] = new Color32(color.r, color.g, b2, byte.MaxValue);
			}
			break;
		}
		case ColorValues.A:
		{
			for (byte b4 = 0; b4 < num4; b4++)
			{
				array[flag2 ? (num4 - 1 - b4) : b4] = new Color32(b4, b4, b4, byte.MaxValue);
			}
			break;
		}
		case ColorValues.Hue:
		{
			for (int k = 0; k < num4; k++)
			{
				array[flag2 ? (num4 - 1 - k) : k] = HSVUtil.ConvertHsvToRgb(k, 1.0, 1.0, 1f);
			}
			break;
		}
		case ColorValues.Saturation:
		{
			for (int j = 0; j < num4; j++)
			{
				array[flag2 ? (num4 - 1 - j) : j] = HSVUtil.ConvertHsvToRgb(num * 360f, (float)j / (float)num4, num3, 1f);
			}
			break;
		}
		case ColorValues.Value:
		{
			for (int i = 0; i < num4; i++)
			{
				array[flag2 ? (num4 - 1 - i) : i] = HSVUtil.ConvertHsvToRgb(num * 360f, num2, (float)i / (float)num4, 1f);
			}
			break;
		}
		default:
			throw new NotImplementedException("");
		}
		texture2D.SetPixels32(array);
		texture2D.Apply();
		if (image.texture != null)
		{
			Object.DestroyImmediate(image.texture);
		}
		image.texture = texture2D;
		switch (direction)
		{
		case Slider.Direction.BottomToTop:
		case Slider.Direction.TopToBottom:
			image.uvRect = new Rect(0f, 0f, 2f, 1f);
			break;
		case Slider.Direction.LeftToRight:
		case Slider.Direction.RightToLeft:
			image.uvRect = new Rect(0f, 0f, 1f, 2f);
			break;
		}
	}
}
