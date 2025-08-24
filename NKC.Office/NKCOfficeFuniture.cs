using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NKM;
using NKM.Templet.Office;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.Office;

public class NKCOfficeFuniture : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, INKCAnimationActor
{
	public delegate void OnFunitureDragEvent(PointerEventData eventData);

	public delegate void OnClickFuniture(int id, long uid);

	public RectTransform m_rtFloor;

	public RectTransform m_rtFuniture;

	public RectTransform m_rtInverse;

	[Header("가구 기본")]
	protected Image m_imgFloor;

	protected Image m_imgFuniture;

	protected Image m_imgInverse;

	[Header("가구 추가 이미지들")]
	public Image[] m_aImgFurnitureSub;

	public InteriorTarget m_eFunitureType;

	[Header("타일 사이즈")]
	public int m_sizeX;

	public int m_sizeY;

	[Header("터치 사운드")]
	public string m_strTouchSound;

	public float m_fSoundDelay;

	[Header("상호작용 포지션")]
	public GameObject m_objInteractionPos;

	public GameObject m_objInteractionInvertPos;

	private int m_id;

	private long m_uid;

	private float m_fTileSize;

	private Mask m_mask;

	protected bool m_bInvert;

	private Vector3 m_originalScale = Vector3.one;

	private NKCAssetInstanceData m_instanceData;

	private NKMOfficeInteriorTemplet m_Templet;

	public OnFunitureDragEvent dOnBeginDragFuniture;

	public OnFunitureDragEvent dOnDragFuniture;

	public OnFunitureDragEvent dOnEndDragFuniture;

	private bool m_bDragged;

	public OnClickFuniture dOnClickFuniture;

	private int m_soundUID;

	private Coroutine soundCoroutine;

	private NKCAnimationInstance m_animEventInstance;

	public long UID => m_uid;

	public NKMOfficeInteriorTemplet Templet => m_Templet;

	public NKCOfficeCharacter InteractingCharacter { get; private set; }

	public bool IsInteractionOngoing => InteractingCharacter != null;

	Animator INKCAnimationActor.Animator => null;

	Transform INKCAnimationActor.SDParent => m_rtFloor;

	Transform INKCAnimationActor.Transform => base.transform;

	public static NKCOfficeFuniture GetInstance(long uid, NKMOfficeInteriorTemplet templet, float tileSize, bool bInvert, Transform parent = null, bool bShowTile = false)
	{
		NKCOfficeFuniture instance = GetInstance(uid, templet, tileSize, parent);
		if (instance == null)
		{
			return null;
		}
		instance.SetShowTile(bShowTile);
		instance.SetInvert(bInvert);
		instance.InvalidateWorldRect();
		return instance;
	}

	public static NKCOfficeFuniture GetInstance(long uid, NKMOfficeInteriorTemplet templet, float tileSize, Transform parent = null)
	{
		if (templet == null)
		{
			return null;
		}
		NKMAssetName nKMAssetName = ((!templet.IsTexture) ? NKMAssetName.ParseBundleName(templet.PrefabName, templet.PrefabName) : new NKMAssetName("ab_ui_office", "FNC_FLAT_BASE"));
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>(nKMAssetName, bAsync: false, parent);
		if (nKCAssetInstanceData?.m_Instant == null)
		{
			Debug.LogError($"NKCUIOfficeFuniture : {nKMAssetName} not found!");
			return null;
		}
		NKCOfficeFuniture component = nKCAssetInstanceData.m_Instant.GetComponent<NKCOfficeFuniture>();
		component.m_instanceData = nKCAssetInstanceData;
		component.Init();
		component.m_uid = uid;
		component.m_id = templet.m_ItemMiscID;
		component.SetData(templet, tileSize);
		return component;
	}

	public virtual void Init()
	{
		if (m_rtFuniture != null)
		{
			m_imgFuniture = m_rtFuniture.GetComponent<Image>();
			if (m_imgFuniture != null && m_imgFuniture.mainTexture != null && m_imgFuniture.mainTexture.isReadable)
			{
				m_imgFuniture.alphaHitTestMinimumThreshold = 0.5f;
			}
		}
		if (m_rtInverse != null)
		{
			m_imgInverse = m_rtInverse.GetComponent<Image>();
			if (m_imgInverse != null && m_imgInverse.mainTexture != null && m_imgInverse.mainTexture.isReadable)
			{
				m_imgInverse.alphaHitTestMinimumThreshold = 0.5f;
			}
		}
		if (m_aImgFurnitureSub != null)
		{
			Image[] aImgFurnitureSub = m_aImgFurnitureSub;
			foreach (Image image in aImgFurnitureSub)
			{
				if (image.mainTexture != null && image.mainTexture.isReadable)
				{
					image.alphaHitTestMinimumThreshold = 0.5f;
				}
			}
		}
		if (m_rtFloor != null)
		{
			m_imgFloor = m_rtFloor.GetComponent<Image>();
			if (m_imgFloor != null && m_imgFloor.mainTexture != null && m_imgFloor.mainTexture.isReadable)
			{
				m_imgFloor.alphaHitTestMinimumThreshold = 0.5f;
			}
		}
		m_originalScale = m_rtFuniture.localScale;
	}

	public void CleanUp()
	{
		if (m_mask != null)
		{
			Object.Destroy(m_mask);
			m_rtFuniture.localPosition = Vector3.zero;
		}
		if (InteractingCharacter != null)
		{
			InteractingCharacter.UnregisterInteraction();
		}
		SetColor(Color.white);
		dOnDragFuniture = null;
		NKCAssetResourceManager.CloseInstance(m_instanceData);
		m_instanceData = null;
	}

	public void SetShowTile(bool value)
	{
		if (m_rtFloor == null)
		{
			return;
		}
		Image component = m_rtFloor.GetComponent<Image>();
		if (component != null)
		{
			if (m_mask != null)
			{
				component.color = Color.white;
			}
			else
			{
				component.color = (value ? Color.white : new Color(1f, 1f, 1f, 0f));
			}
		}
	}

	public void SetFunitureBoxRaycast(bool value)
	{
		if (m_imgFuniture != null)
		{
			m_imgFuniture.raycastTarget = value;
		}
		if (m_imgInverse != null)
		{
			m_imgInverse.raycastTarget = value;
		}
		if (m_aImgFurnitureSub != null)
		{
			Image[] aImgFurnitureSub = m_aImgFurnitureSub;
			for (int i = 0; i < aImgFurnitureSub.Length; i++)
			{
				aImgFurnitureSub[i].raycastTarget = value;
			}
		}
	}

	public virtual void SetData(NKMOfficeInteriorTemplet templet, float tileSize)
	{
		m_Templet = templet;
		if (!templet.IsTexture && templet.InteriorCategory == InteriorCategory.FURNITURE && (m_sizeX != templet.CellX || m_sizeY != templet.CellY))
		{
			Debug.LogError(templet.DebugName + " / " + base.gameObject.name + " : Templet 상의 가구 사이즈와 실제 프리팹 사이즈가 다름!");
		}
		SetTileSize(templet.CellX, templet.CellY, tileSize);
		SetFunitureBoxRaycast(templet.Target == InteriorTarget.Floor);
	}

	public void SetTileSize(int x, int y, float tileSize)
	{
		m_fTileSize = tileSize;
		m_rtFloor.SetSize(new Vector2((float)x * tileSize, (float)y * tileSize));
	}

	public bool GetInvert()
	{
		return m_bInvert;
	}

	public virtual void SetInvert(bool bInvert, bool bEditMode = false)
	{
		m_bInvert = bInvert;
		if (m_rtInverse != null)
		{
			NKCUtil.SetGameobjectActive(m_rtInverse, bInvert);
			NKCUtil.SetGameobjectActive(m_rtFuniture, !bInvert);
			switch (m_eFunitureType)
			{
			case InteriorTarget.Floor:
			case InteriorTarget.Tile:
				if (bInvert)
				{
					if (bEditMode)
					{
						m_rtFloor.rotation = Quaternion.Euler(66.5f, 0f, -45f);
					}
					else
					{
						m_rtFloor.localRotation = Quaternion.Euler(0f, 0f, -90f);
					}
					m_rtInverse.rotation = Quaternion.identity;
					Vector3 eulerAngles2 = m_rtInverse.rotation.eulerAngles;
					m_rtInverse.rotation = Quaternion.Euler(eulerAngles2.x, 0f - eulerAngles2.y, 0f - eulerAngles2.z);
				}
				else
				{
					if (bEditMode)
					{
						m_rtFloor.rotation = Quaternion.Euler(66.5f, 0f, 45f);
						m_rtFuniture.localScale = new Vector3(m_rtFuniture.localScale.y, m_rtFuniture.localScale.y, m_rtFuniture.localScale.z);
					}
					else
					{
						m_rtFloor.localRotation = Quaternion.identity;
						m_rtFuniture.localScale = m_originalScale;
					}
					m_rtFuniture.rotation = Quaternion.identity;
				}
				break;
			case InteriorTarget.Wall:
				if (bInvert)
				{
					if (bEditMode)
					{
						m_rtFloor.rotation = Quaternion.Euler(-16.377f, 47.477f, -17.091f);
					}
					else
					{
						m_rtFloor.localRotation = Quaternion.identity;
					}
					m_rtInverse.rotation = Quaternion.identity;
					Vector3 eulerAngles = m_rtInverse.rotation.eulerAngles;
					m_rtInverse.rotation = Quaternion.Euler(eulerAngles.x, 0f - eulerAngles.y, eulerAngles.z);
				}
				else
				{
					if (bEditMode)
					{
						m_rtFloor.rotation = Quaternion.Euler(-16.377f, -47.477f, 17.091f);
						m_rtFuniture.localScale = new Vector3(m_rtFuniture.localScale.y, m_rtFuniture.localScale.y, m_rtFuniture.localScale.z);
					}
					else
					{
						m_rtFloor.localRotation = Quaternion.identity;
						m_rtFuniture.localScale = m_originalScale;
					}
					m_rtFuniture.rotation = Quaternion.identity;
				}
				break;
			}
			return;
		}
		switch (m_eFunitureType)
		{
		case InteriorTarget.Floor:
		case InteriorTarget.Tile:
			if (bInvert)
			{
				if (bEditMode)
				{
					m_rtFloor.rotation = Quaternion.Euler(66.5f, 0f, -45f);
					m_rtFuniture.localScale = new Vector3(0f - m_rtFuniture.localScale.y, m_rtFuniture.localScale.y, m_rtFuniture.localScale.z);
				}
				else
				{
					m_rtFloor.localRotation = Quaternion.Euler(0f, 0f, -90f);
					m_rtFuniture.localScale = new Vector3(0f - m_originalScale.x, m_originalScale.y, m_originalScale.z);
				}
				m_rtFuniture.rotation = Quaternion.identity;
				Vector3 eulerAngles4 = m_rtFuniture.rotation.eulerAngles;
				m_rtFuniture.rotation = Quaternion.Euler(eulerAngles4.x, 0f - eulerAngles4.y, 0f - eulerAngles4.z);
			}
			else
			{
				if (bEditMode)
				{
					m_rtFloor.rotation = Quaternion.Euler(66.5f, 0f, 45f);
					m_rtFuniture.localScale = new Vector3(m_rtFuniture.localScale.y, m_rtFuniture.localScale.y, m_rtFuniture.localScale.z);
				}
				else
				{
					m_rtFloor.localRotation = Quaternion.identity;
					m_rtFuniture.localScale = m_originalScale;
				}
				m_rtFuniture.rotation = Quaternion.identity;
			}
			break;
		case InteriorTarget.Wall:
			if (bInvert)
			{
				if (bEditMode)
				{
					m_rtFloor.rotation = Quaternion.Euler(-16.377f, 47.477f, -17.091f);
					m_rtFuniture.localScale = new Vector3(0f - m_rtFuniture.localScale.y, m_rtFuniture.localScale.y, m_rtFuniture.localScale.z);
				}
				else
				{
					m_rtFloor.localRotation = Quaternion.identity;
					m_rtFuniture.localScale = new Vector3(0f - m_originalScale.x, m_originalScale.y, m_originalScale.z);
				}
				m_rtFuniture.rotation = Quaternion.identity;
				Vector3 eulerAngles3 = m_rtFuniture.rotation.eulerAngles;
				m_rtFuniture.rotation = Quaternion.Euler(eulerAngles3.x, 0f - eulerAngles3.y, eulerAngles3.z);
			}
			else
			{
				if (bEditMode)
				{
					m_rtFloor.rotation = Quaternion.Euler(-16.377f, -47.477f, 17.091f);
					m_rtFuniture.localScale = new Vector3(m_rtFuniture.localScale.y, m_rtFuniture.localScale.y, m_rtFuniture.localScale.z);
				}
				else
				{
					m_rtFloor.localRotation = Quaternion.identity;
					m_rtFuniture.localScale = m_originalScale;
				}
				m_rtFuniture.rotation = Quaternion.identity;
			}
			break;
		}
	}

	public virtual void Resize(NKMOfficeInteriorTemplet templet, int overflowX, int overflowY)
	{
		if (overflowX == 0 && overflowY == 0)
		{
			if (m_mask != null)
			{
				Object.Destroy(m_mask);
				m_mask = null;
			}
		}
		else
		{
			if (m_mask == null)
			{
				m_mask = m_rtFloor.gameObject.AddComponent<Mask>();
				m_mask.showMaskGraphic = false;
			}
			if (m_imgFloor != null)
			{
				m_imgFloor.color = Color.white;
			}
		}
		m_rtFuniture.localPosition = new Vector3((float)overflowX * m_fTileSize, (float)overflowY * m_fTileSize, 0f);
	}

	public virtual void SetAlpha(float value)
	{
		if (m_imgFuniture != null)
		{
			m_imgFuniture.DOKill();
			m_imgFuniture.color = new Color(1f, 1f, 1f, value);
		}
		if (m_imgInverse != null)
		{
			m_imgInverse.DOKill();
			m_imgInverse.color = new Color(1f, 1f, 1f, value);
		}
		if (m_aImgFurnitureSub != null)
		{
			Image[] aImgFurnitureSub = m_aImgFurnitureSub;
			foreach (Image obj in aImgFurnitureSub)
			{
				obj.DOKill();
				obj.color = new Color(1f, 1f, 1f, value);
			}
		}
	}

	public virtual void SetColor(Color color)
	{
		if (m_imgFuniture != null)
		{
			m_imgFuniture.DOKill();
			m_imgFuniture.color = color;
		}
		if (m_imgInverse != null)
		{
			m_imgInverse.DOKill();
			m_imgInverse.color = color;
		}
		if (m_aImgFurnitureSub != null)
		{
			Image[] aImgFurnitureSub = m_aImgFurnitureSub;
			foreach (Image obj in aImgFurnitureSub)
			{
				obj.DOKill();
				obj.color = color;
			}
		}
	}

	public virtual void SetGlow(Color color, float time)
	{
		if (m_imgFuniture != null)
		{
			m_imgFuniture.DOKill();
			m_imgFuniture.color = Color.white;
			m_imgFuniture.DOColor(color, time).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
		}
		if (m_imgInverse != null)
		{
			m_imgInverse.DOKill();
			m_imgInverse.color = Color.white;
			m_imgInverse.DOColor(color, time).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
		}
		if (m_aImgFurnitureSub != null)
		{
			Image[] aImgFurnitureSub = m_aImgFurnitureSub;
			foreach (Image obj in aImgFurnitureSub)
			{
				obj.DOKill();
				obj.color = Color.white;
				obj.DOColor(color, time).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
			}
		}
	}

	public virtual void SetHighlight(bool value)
	{
		if (value)
		{
			SetGlow(Color.green, 1f);
		}
		else
		{
			SetColor(Color.white);
		}
	}

	public virtual void InvalidateWorldRect()
	{
	}

	public virtual Rect GetWorldRect(bool bFurnitureOnly)
	{
		Rect rect = GetFurnitureRect();
		if (!bFurnitureOnly)
		{
			Rect worldRect = m_rtFloor.GetWorldRect();
			rect = rect.Union(worldRect);
		}
		if (InteractingCharacter != null && InteractingCharacter.PlayingInteractionAnimation)
		{
			Rect worldRect2 = InteractingCharacter.GetWorldRect();
			rect = rect.Union(worldRect2);
		}
		return rect;
	}

	protected virtual Rect GetFurnitureRect()
	{
		if (m_bInvert && m_rtInverse != null)
		{
			return m_rtInverse.GetWorldRect();
		}
		return m_rtFuniture.GetWorldRect();
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		m_bDragged = true;
		dOnBeginDragFuniture?.Invoke(eventData);
	}

	public void OnDrag(PointerEventData eventData)
	{
		dOnDragFuniture?.Invoke(eventData);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		m_bDragged = false;
		dOnEndDragFuniture?.Invoke(eventData);
	}

	public virtual void OnPointerClick(PointerEventData eventData)
	{
		if (!m_bDragged)
		{
			dOnClickFuniture?.Invoke(m_id, m_uid);
		}
	}

	public virtual void OnTouchReact()
	{
		Debug.Log($"Futniture Touch : {base.gameObject.name}(id {m_id} / uid {m_uid})");
		if (!string.IsNullOrEmpty(m_Templet.TouchSound))
		{
			PlaySound();
		}
	}

	protected void PlaySound()
	{
		if (m_soundUID != 0)
		{
			NKCSoundManager.StopSound(m_soundUID);
		}
		if (soundCoroutine != null)
		{
			StopCoroutine(soundCoroutine);
		}
		soundCoroutine = StartCoroutine(SoundProcess());
	}

	private IEnumerator SoundProcess()
	{
		if (m_Templet != null)
		{
			if (m_Templet.SoundDelay > 0f)
			{
				yield return new WaitForSeconds(m_Templet.SoundDelay);
			}
			m_soundUID = NKCSoundManager.PlaySound(m_Templet.TouchSound, 1f, 0f, 0f);
			soundCoroutine = null;
		}
	}

	public bool HasInteractionTarget()
	{
		return InteractingCharacter != null;
	}

	public void RegisterInteractionCharacter(NKCOfficeCharacter character)
	{
		InteractingCharacter = character;
	}

	public void CleanupInteraction()
	{
		InteractingCharacter = null;
		CleanupAnimEvent();
		InvalidateWorldRect();
	}

	public void PlayAnimationEvent(string animEventName)
	{
		if (!string.IsNullOrEmpty(animEventName))
		{
			List<NKCAnimationEventTemplet> list = NKCAnimationEventManager.Find(animEventName);
			if (list != null && list.Count != 0)
			{
				m_animEventInstance = new NKCAnimationInstance(this, base.transform, list, base.transform.localPosition, base.transform.localPosition);
			}
		}
	}

	public GameObject GetInteractionPoint()
	{
		if (m_objInteractionInvertPos != null && m_bInvert)
		{
			return m_objInteractionInvertPos;
		}
		return m_objInteractionPos;
	}

	public virtual void CleanupAnimEvent()
	{
		if (m_animEventInstance != null)
		{
			m_animEventInstance.RemoveEffect();
			m_animEventInstance = null;
		}
	}

	public void InvokeTouchEvent()
	{
		dOnClickFuniture?.Invoke(m_id, m_uid);
	}

	public virtual RectTransform MakeHighlightRect()
	{
		if (m_rtFuniture.gameObject.activeInHierarchy)
		{
			return m_rtFuniture;
		}
		return m_rtInverse;
	}

	public (Vector3, Vector3) GetHorizonalLine(float extend = 0f)
	{
		Vector3[] array = new Vector3[4];
		m_rtFloor.GetWorldCorners(array);
		float num = Mathf.Abs(array[0].x - array[2].x);
		float num2 = Mathf.Abs(array[1].x - array[3].x);
		(Vector3, Vector3) tuple = ((num > num2) ? ((!(array[0].x < array[2].x)) ? (array[2], array[0]) : (array[0], array[2])) : ((!(array[1].x < array[3].x)) ? (array[3], array[1]) : (array[1], array[3])));
		Vector3 vector = (tuple.Item2 - tuple.Item1).normalized * extend;
		return (tuple.Item1 - vector, tuple.Item2 + vector);
	}

	public (float, float) GetZMinMax()
	{
		Vector3[] array = new Vector3[4];
		m_rtFloor.GetWorldCorners(array);
		float item = Mathf.Min(array[0].z, array[1].z, array[2].z, array[3].z);
		float item2 = Mathf.Max(array[0].z, array[1].z, array[2].z, array[3].z);
		return (item, item2);
	}

	public void GetWorldInfo(out float zMin, out float zMax, out Vector3 xMinPos, out Vector3 xMaxPos)
	{
		Vector3[] array = new Vector3[4];
		m_rtFloor.GetWorldCorners(array);
		zMin = Mathf.Min(array[0].z, array[1].z, array[2].z, array[3].z);
		zMax = Mathf.Max(array[0].z, array[1].z, array[2].z, array[3].z);
		float num = Mathf.Abs(array[0].x - array[2].x);
		float num2 = Mathf.Abs(array[1].x - array[3].x);
		if (num > num2)
		{
			if (array[0].x < array[2].x)
			{
				xMinPos = array[0];
				xMaxPos = array[2];
			}
			else
			{
				xMinPos = array[2];
				xMaxPos = array[0];
			}
		}
		else if (array[1].x < array[3].x)
		{
			xMinPos = array[1];
			xMaxPos = array[3];
		}
		else
		{
			xMinPos = array[3];
			xMaxPos = array[1];
		}
	}

	protected virtual void Update()
	{
		if (m_animEventInstance != null)
		{
			if (m_animEventInstance.IsFinished())
			{
				CleanupAnimEvent();
			}
			else
			{
				m_animEventInstance.Update(Time.deltaTime);
			}
		}
	}

	public virtual Vector3 GetBonePosition(string name)
	{
		return base.transform.position;
	}

	public void PlayEmotion(string animName, float speed)
	{
	}

	public virtual void PlaySpineAnimation(string name, bool loop, float timeScale)
	{
	}

	public virtual void PlaySpineAnimation(NKCASUIUnitIllust.eAnimation eAnim, bool loop, float timeScale, bool bDefaultAnim)
	{
	}

	public virtual bool IsSpineAnimationFinished(NKCASUIUnitIllust.eAnimation eAnim)
	{
		return true;
	}

	public virtual bool IsSpineAnimationFinished(string name)
	{
		return true;
	}

	public virtual bool CanPlaySpineAnimation(string name)
	{
		return false;
	}

	public virtual bool CanPlaySpineAnimation(NKCASUIUnitIllust.eAnimation eAnim)
	{
		return false;
	}

	public void UpdateInteractionPos(RectTransform floorRect)
	{
		ProjectPointToPlane(m_objInteractionPos, floorRect);
		ProjectPointToPlane(m_objInteractionInvertPos, floorRect);
	}

	private void ProjectPointToPlane(GameObject point, RectTransform plane)
	{
		if (!(point == null))
		{
			point.transform.position = plane.ProjectPointToPlaneWorldPos(point.transform.position);
		}
	}
}
