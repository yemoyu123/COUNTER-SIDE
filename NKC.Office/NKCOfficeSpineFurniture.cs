using System.Collections.Generic;
using DG.Tweening;
using NKC.UI;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace NKC.Office;

public class NKCOfficeSpineFurniture : NKCOfficeFuniture
{
	private const string ANIM_BASE = "BASE";

	private const string ANIM_TOUCH = "TOUCH";

	public RectTransform m_rtTouchArea;

	protected SkeletonGraphic[] m_aSpineFurniture;

	protected SkeletonGraphic[] m_aSpineInvert;

	private bool m_bRectCalculated;

	private Rect m_rectWorld;

	private bool bUseInvert
	{
		get
		{
			if (m_bInvert)
			{
				return m_aSpineInvert != null;
			}
			return false;
		}
	}

	private SkeletonGraphic[] GetCurrentSpineSet()
	{
		if (!bUseInvert)
		{
			return m_aSpineFurniture;
		}
		return m_aSpineInvert;
	}

	public override void Init()
	{
		base.Init();
		if (m_rtFuniture != null && m_aSpineFurniture == null)
		{
			m_aSpineFurniture = m_rtFuniture.GetComponentsInChildren<SkeletonGraphic>();
		}
		if (m_aSpineFurniture != null)
		{
			SkeletonGraphic[] aSpineFurniture = m_aSpineFurniture;
			foreach (SkeletonGraphic skeletonGraphic in aSpineFurniture)
			{
				skeletonGraphic.raycastTarget = true;
				SetAnimation(skeletonGraphic, "BASE", loop: true);
			}
		}
		if (m_rtInverse != null && m_aSpineInvert == null)
		{
			m_aSpineInvert = m_rtInverse.GetComponentsInChildren<SkeletonGraphic>();
		}
		if (m_aSpineInvert != null)
		{
			SkeletonGraphic[] aSpineFurniture = m_aSpineInvert;
			foreach (SkeletonGraphic skeletonGraphic2 in aSpineFurniture)
			{
				skeletonGraphic2.raycastTarget = true;
				SetAnimation(skeletonGraphic2, "BASE", loop: true);
			}
		}
	}

	public override void OnTouchReact()
	{
		base.OnTouchReact();
		List<string> list = new List<string>();
		if (HasAnimation(m_aSpineFurniture, "TOUCH"))
		{
			list.Add("TOUCH");
		}
		int num = 1;
		while (HasAnimation(m_aSpineFurniture, "TOUCH" + num))
		{
			list.Add("TOUCH" + num);
			num++;
		}
		if (list.Count > 0)
		{
			string animName = list[Random.Range(0, list.Count)];
			SkeletonGraphic[] currentSpineSet = GetCurrentSpineSet();
			SetAnimation(currentSpineSet, animName, loop: false);
			AddAnimation(currentSpineSet, "BASE", loop: true);
		}
	}

	public override void InvalidateWorldRect()
	{
		base.InvalidateWorldRect();
		m_bRectCalculated = false;
	}

	protected override Rect GetFurnitureRect()
	{
		if (!m_bRectCalculated)
		{
			m_rectWorld = CalculateWorldRect();
			m_bRectCalculated = true;
		}
		return m_rectWorld;
	}

	private Rect CalculateWorldRect()
	{
		Rect rect;
		if (m_aSpineFurniture == null || m_aSpineFurniture.Length == 0)
		{
			rect = ((!(m_rtFuniture != null)) ? new Rect(base.transform.position, Vector2.zero) : m_rtFuniture.GetWorldRect());
		}
		else
		{
			GameObject gameObject = new GameObject("temp", typeof(RectTransform));
			RectTransform component = gameObject.GetComponent<RectTransform>();
			rect = new Rect(base.transform.position, Vector2.zero);
			for (int i = 0; i < m_aSpineFurniture.Length; i++)
			{
				SkeletonGraphic skeletonGraphic = m_aSpineFurniture[i];
				component.SetParent(skeletonGraphic.rectTransform.parent);
				component.localPosition = skeletonGraphic.rectTransform.localPosition;
				component.localRotation = skeletonGraphic.rectTransform.localRotation;
				component.localScale = skeletonGraphic.rectTransform.localScale;
				Bounds bounds = skeletonGraphic.GetLastMesh().bounds;
				Vector3 size = bounds.size;
				Vector3 center = bounds.center;
				Vector2 pivot = new Vector2(0.5f - center.x / size.x, 0.5f - center.y / size.y);
				component.sizeDelta = size;
				component.pivot = pivot;
				Rect worldRect = component.GetWorldRect();
				rect = rect.Union(worldRect);
			}
			Object.Destroy(gameObject);
		}
		if (m_rtTouchArea != null)
		{
			Rect worldRect2 = m_rtTouchArea.GetWorldRect();
			rect = rect.Union(worldRect2);
		}
		return rect;
	}

	public override void SetInvert(bool bInvert, bool bEditMode = false)
	{
		base.SetInvert(bInvert, bEditMode);
		if (m_rtTouchArea != null)
		{
			m_rtTouchArea.rotation = Quaternion.identity;
		}
	}

	protected bool HasAnimation(SkeletonGraphic target, string AnimName)
	{
		if (target == null || target.SkeletonData == null)
		{
			return false;
		}
		return target.SkeletonData.FindAnimation(AnimName) != null;
	}

	protected bool HasAnimation(SkeletonGraphic[] aTarget, string AnimName)
	{
		if (aTarget == null)
		{
			return false;
		}
		foreach (SkeletonGraphic skeletonGraphic in aTarget)
		{
			if (skeletonGraphic.SkeletonData != null && skeletonGraphic.SkeletonData.FindAnimation(AnimName) != null)
			{
				return true;
			}
		}
		return false;
	}

	protected void SetAnimation(SkeletonGraphic target, string AnimName, bool loop, float timeScale = 1f)
	{
		if (HasAnimation(target, AnimName))
		{
			target.Skeleton?.SetToSetupPose();
			target.AnimationState.SetAnimation(0, AnimName, loop).TimeScale = timeScale;
		}
	}

	protected void SetAnimation(SkeletonGraphic[] aTarget, string AnimName, bool loop, float timeScale = 1f)
	{
		if (aTarget == null)
		{
			return;
		}
		foreach (SkeletonGraphic skeletonGraphic in aTarget)
		{
			if (HasAnimation(skeletonGraphic, AnimName))
			{
				skeletonGraphic.Skeleton?.SetToSetupPose();
				skeletonGraphic.AnimationState.SetAnimation(0, AnimName, loop).TimeScale = timeScale;
			}
		}
	}

	protected void AddAnimation(SkeletonGraphic target, string AnimName, bool loop)
	{
		if (HasAnimation(target, AnimName))
		{
			target.AnimationState.AddAnimation(0, AnimName, loop, 0f);
		}
	}

	protected void AddAnimation(SkeletonGraphic[] aTarget, string AnimName, bool loop)
	{
		if (aTarget == null)
		{
			return;
		}
		foreach (SkeletonGraphic skeletonGraphic in aTarget)
		{
			if (HasAnimation(skeletonGraphic, AnimName))
			{
				skeletonGraphic.AnimationState.AddAnimation(0, AnimName, loop, 0f);
			}
		}
	}

	public override RectTransform MakeHighlightRect()
	{
		if (m_rtTouchArea != null)
		{
			return m_rtTouchArea;
		}
		return base.MakeHighlightRect();
	}

	public override void SetColor(Color color)
	{
		base.SetColor(color);
		if (m_aSpineFurniture != null)
		{
			SkeletonGraphic[] aSpineFurniture = m_aSpineFurniture;
			foreach (SkeletonGraphic obj in aSpineFurniture)
			{
				obj.DOKill();
				obj.color = color;
			}
		}
		if (m_aSpineInvert != null)
		{
			SkeletonGraphic[] aSpineFurniture = m_aSpineInvert;
			foreach (SkeletonGraphic obj2 in aSpineFurniture)
			{
				obj2.DOKill();
				obj2.color = color;
			}
		}
	}

	public override void SetAlpha(float value)
	{
		base.SetAlpha(value);
		if (m_aSpineFurniture != null)
		{
			SkeletonGraphic[] aSpineFurniture = m_aSpineFurniture;
			foreach (SkeletonGraphic obj in aSpineFurniture)
			{
				obj.DOKill();
				obj.color = new Color(1f, 1f, 1f, value);
			}
		}
		if (m_aSpineInvert != null)
		{
			SkeletonGraphic[] aSpineFurniture = m_aSpineInvert;
			foreach (SkeletonGraphic obj2 in aSpineFurniture)
			{
				obj2.DOKill();
				obj2.color = new Color(1f, 1f, 1f, value);
			}
		}
	}

	public override void SetGlow(Color color, float time)
	{
		base.SetGlow(color, time);
		if (m_aSpineFurniture != null)
		{
			SkeletonGraphic[] aSpineFurniture = m_aSpineFurniture;
			foreach (SkeletonGraphic obj in aSpineFurniture)
			{
				obj.DOKill();
				obj.color = Color.white;
				obj.DOColor(color, time).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
			}
		}
		if (m_aSpineInvert != null)
		{
			SkeletonGraphic[] aSpineFurniture = m_aSpineInvert;
			foreach (SkeletonGraphic obj2 in aSpineFurniture)
			{
				obj2.DOKill();
				obj2.color = Color.white;
				obj2.DOColor(color, time).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
			}
		}
	}

	public override void CleanupAnimEvent()
	{
		base.CleanupAnimEvent();
		SetAnimation(m_aSpineFurniture, "BASE", loop: true);
		SetAnimation(m_aSpineInvert, "BASE", loop: true);
	}

	public override Vector3 GetBonePosition(string name)
	{
		SkeletonGraphic[] currentSpineSet = GetCurrentSpineSet();
		foreach (SkeletonGraphic skeletonGraphic in currentSpineSet)
		{
			Bone bone = skeletonGraphic.Skeleton?.FindBone(name);
			if (bone != null)
			{
				float referencePixelsPerUnit = NKCUIManager.FrontCanvas.referencePixelsPerUnit;
				return skeletonGraphic.transform.TransformPoint(bone.WorldX * referencePixelsPerUnit, bone.WorldY * referencePixelsPerUnit, 0f);
			}
		}
		Debug.LogError("Bone " + name + " not found!");
		return Vector3.zero;
	}

	public override void PlaySpineAnimation(string name, bool loop, float timeScale)
	{
		SetAnimation(GetCurrentSpineSet(), name, loop, timeScale);
	}

	public override void PlaySpineAnimation(NKCASUIUnitIllust.eAnimation eAnim, bool loop, float timeScale, bool bDefaultAnim)
	{
		Debug.LogError("가구에는 ANIMATION_NAME_SPINE만 써 주세요");
	}

	public override bool IsSpineAnimationFinished(NKCASUIUnitIllust.eAnimation eAnim)
	{
		string animationName = NKCASUIUnitIllust.GetAnimationName(eAnim);
		return IsSpineAnimationFinished(animationName);
	}

	public override bool IsSpineAnimationFinished(string name)
	{
		SkeletonGraphic[] currentSpineSet = GetCurrentSpineSet();
		foreach (SkeletonGraphic skeletonGraphic in currentSpineSet)
		{
			if (skeletonGraphic != null && skeletonGraphic.AnimationState != null)
			{
				TrackEntry current = skeletonGraphic.AnimationState.GetCurrent(0);
				if (current != null && current.Animation != null && !(current.Animation.Name != name) && current.AnimationTime <= current.AnimationEnd)
				{
					return false;
				}
			}
		}
		return true;
	}

	public override bool CanPlaySpineAnimation(NKCASUIUnitIllust.eAnimation eAnim)
	{
		return CanPlaySpineAnimation(NKCASUIUnitIllust.GetAnimationName(eAnim));
	}

	public override bool CanPlaySpineAnimation(string animName)
	{
		SkeletonGraphic[] currentSpineSet = GetCurrentSpineSet();
		foreach (SkeletonGraphic skeletonGraphic in currentSpineSet)
		{
			if (!(skeletonGraphic == null) && skeletonGraphic.SkeletonData != null && skeletonGraphic.SkeletonData.FindAnimation(animName) != null)
			{
				return true;
			}
		}
		return false;
	}
}
