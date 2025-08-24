using UnityEngine;

namespace NKC;

public static class RectTransformExtensions
{
	public enum FitMode
	{
		FitToRect,
		FitOutside,
		FitInside,
		FitToWidth,
		FitToHeight
	}

	public enum ScaleMode
	{
		Scale,
		RectSize
	}

	public static void SetDefaultScale(this RectTransform trans)
	{
		trans.localScale = new Vector3(1f, 1f, 1f);
	}

	public static void SetPivotAndAnchors(this RectTransform trans, Vector2 aVec)
	{
		trans.pivot = aVec;
		trans.anchorMin = aVec;
		trans.anchorMax = aVec;
	}

	public static Vector2 GetSize(this RectTransform trans)
	{
		return trans.rect.size;
	}

	public static float GetWidth(this RectTransform trans)
	{
		return trans.rect.width;
	}

	public static float GetHeight(this RectTransform trans)
	{
		return trans.rect.height;
	}

	public static void SetPositionOfPivot(this RectTransform trans, Vector2 newPos)
	{
		trans.localPosition = new Vector3(newPos.x, newPos.y, trans.localPosition.z);
	}

	public static void SetLeftBottomPosition(this RectTransform trans, Vector2 newPos)
	{
		trans.localPosition = new Vector3(newPos.x + trans.pivot.x * trans.rect.width, newPos.y + trans.pivot.y * trans.rect.height, trans.localPosition.z);
	}

	public static void SetLeftTopPosition(this RectTransform trans, Vector2 newPos)
	{
		trans.localPosition = new Vector3(newPos.x + trans.pivot.x * trans.rect.width, newPos.y - (1f - trans.pivot.y) * trans.rect.height, trans.localPosition.z);
	}

	public static void SetRightBottomPosition(this RectTransform trans, Vector2 newPos)
	{
		trans.localPosition = new Vector3(newPos.x - (1f - trans.pivot.x) * trans.rect.width, newPos.y + trans.pivot.y * trans.rect.height, trans.localPosition.z);
	}

	public static void SetRightTopPosition(this RectTransform trans, Vector2 newPos)
	{
		trans.localPosition = new Vector3(newPos.x - (1f - trans.pivot.x) * trans.rect.width, newPos.y - (1f - trans.pivot.y) * trans.rect.height, trans.localPosition.z);
	}

	public static void SetSize(this RectTransform trans, Vector2 newSize)
	{
		Vector2 size = trans.rect.size;
		Vector2 vector = newSize - size;
		trans.offsetMin -= new Vector2(vector.x * trans.pivot.x, vector.y * trans.pivot.y);
		trans.offsetMax += new Vector2(vector.x * (1f - trans.pivot.x), vector.y * (1f - trans.pivot.y));
	}

	public static void SetWidth(this RectTransform trans, float newSize)
	{
		trans.SetSize(new Vector2(newSize, trans.rect.size.y));
	}

	public static void SetHeight(this RectTransform trans, float newSize)
	{
		trans.SetSize(new Vector2(trans.rect.size.x, newSize));
	}

	public static Vector3 GetCenterWorldPos(this RectTransform rt)
	{
		Vector3[] array = new Vector3[4];
		rt.GetWorldCorners(array);
		return (array[0] + array[2]) / 2f;
	}

	public static Rect GetWorldRect(this RectTransform rect)
	{
		Vector3[] array = new Vector3[4];
		rect.GetWorldCorners(array);
		return Rect.MinMaxRect(Mathf.Min(array[0].x, array[1].x, array[2].x, array[3].x), Mathf.Min(array[0].y, array[1].y, array[2].y, array[3].y), Mathf.Max(array[0].x, array[1].x, array[2].x, array[3].x), Mathf.Max(array[0].y, array[1].y, array[2].y, array[3].y));
	}

	public static Vector3 ClampLocalPos(this RectTransform rectTransfom, Vector3 localPos)
	{
		Vector3 result = default(Vector3);
		result.x = Mathf.Clamp(localPos.x, rectTransfom.rect.xMin, rectTransfom.rect.xMax);
		result.y = Mathf.Clamp(localPos.y, rectTransfom.rect.yMin, rectTransfom.rect.yMax);
		result.z = localPos.z;
		return result;
	}

	public static void FitRectTransformToRect(this RectTransform rect, Rect TargetSizeRect, FitMode fitMode = FitMode.FitOutside, ScaleMode scaleMode = ScaleMode.Scale)
	{
		Vector2 newSize = default(Vector2);
		switch (fitMode)
		{
		case FitMode.FitToRect:
			newSize.x = TargetSizeRect.width;
			newSize.y = TargetSizeRect.height;
			break;
		case FitMode.FitToWidth:
		{
			float num4 = rect.GetWidth() / rect.GetHeight();
			newSize.x = TargetSizeRect.width;
			newSize.y = newSize.x / num4;
			break;
		}
		case FitMode.FitToHeight:
		{
			float num2 = rect.GetWidth() / rect.GetHeight();
			newSize.y = TargetSizeRect.height;
			newSize.x = newSize.y * num2;
			break;
		}
		default:
		{
			float num3 = rect.GetWidth() / rect.GetHeight();
			if (rect.GetWidth() / TargetSizeRect.width > rect.GetHeight() / TargetSizeRect.height)
			{
				newSize.y = TargetSizeRect.height;
				newSize.x = newSize.y * num3;
			}
			else
			{
				newSize.x = TargetSizeRect.width;
				newSize.y = newSize.x / num3;
			}
			break;
		}
		case FitMode.FitInside:
		{
			float num = rect.GetWidth() / rect.GetHeight();
			if (rect.GetWidth() / TargetSizeRect.width > rect.GetHeight() / TargetSizeRect.height)
			{
				newSize.x = TargetSizeRect.width;
				newSize.y = newSize.x / num;
			}
			else
			{
				newSize.y = TargetSizeRect.height;
				newSize.x = newSize.y * num;
			}
			break;
		}
		}
		switch (scaleMode)
		{
		case ScaleMode.Scale:
			rect.localScale = new Vector3
			{
				x = newSize.x / rect.GetWidth(),
				y = newSize.y / rect.GetHeight(),
				z = rect.localScale.z
			};
			break;
		case ScaleMode.RectSize:
			rect.SetSize(newSize);
			break;
		}
	}

	public static Vector3 ProjectPointToPlane(this RectTransform plane, Vector3 targetPos)
	{
		Camera subUICamera = NKCCamera.GetSubUICamera();
		Vector2 screenPoint = subUICamera.WorldToScreenPoint(targetPos);
		RectTransformUtility.ScreenPointToLocalPointInRectangle(plane, screenPoint, subUICamera, out var localPoint);
		return localPoint;
	}

	public static Vector3 ProjectPointToPlaneWorldPos(this RectTransform plane, Vector3 targetPos)
	{
		Vector3 position = plane.ProjectPointToPlane(targetPos);
		return plane.TransformPoint(position);
	}
}
