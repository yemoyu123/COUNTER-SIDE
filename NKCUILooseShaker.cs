using DG.Tweening;
using UnityEngine;

public class NKCUILooseShaker : MonoBehaviour
{
	[SerializeField]
	private float min = -30f;

	[SerializeField]
	private float max = 30f;

	[SerializeField]
	private float durationTime = 2f;

	[SerializeField]
	private Ease[] EasingType = new Ease[5];

	private Vector2 originAnchorPos;

	private RectTransform _myRectTransform;

	private void Awake()
	{
		_myRectTransform = GetComponent<RectTransform>();
		if (_myRectTransform != null)
		{
			originAnchorPos = _myRectTransform.anchoredPosition;
		}
		if (EasingType.Length == 5)
		{
			EasingType[0] = Ease.InBounce;
			EasingType[1] = Ease.OutElastic;
			EasingType[2] = Ease.InOutBounce;
			EasingType[3] = Ease.InOutBack;
			EasingType[4] = Ease.OutCirc;
		}
	}

	private void OnDisable()
	{
		CleanUp();
	}

	private float GetRandValue()
	{
		return Random.Range(min, max);
	}

	public void StartShake()
	{
		if (!(_myRectTransform == null))
		{
			float randValue = GetRandValue();
			float randValue2 = GetRandValue();
			StopShake();
			_myRectTransform.DOAnchorPos(new Vector2(originAnchorPos.x + randValue, originAnchorPos.y + randValue2), durationTime).SetEase(GetRandEaseType()).OnComplete(StartShake);
		}
	}

	public void StopShake()
	{
		if (!(_myRectTransform == null) && DOTween.IsTweening(_myRectTransform))
		{
			_myRectTransform.DOKill();
		}
	}

	public void CleanUp()
	{
		if (!(_myRectTransform == null))
		{
			StopShake();
			_myRectTransform.anchoredPosition = originAnchorPos;
		}
	}

	private Ease GetRandEaseType()
	{
		return Random.Range(0, EasingType.Length) switch
		{
			0 => EasingType[0], 
			1 => EasingType[1], 
			2 => EasingType[2], 
			3 => EasingType[3], 
			4 => EasingType[4], 
			_ => Ease.InBounce, 
		};
	}
}
