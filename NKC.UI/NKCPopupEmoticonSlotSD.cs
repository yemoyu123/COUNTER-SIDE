using NKM;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupEmoticonSlotSD : MonoBehaviour
{
	public delegate void dOnClickChange(int emoticonID);

	public const string PLAY_ANI_NAME = "BASE";

	public const string STOP_ANI_NAME = "BASE_END";

	public NKCUISlot m_NKCUISlot;

	public GameObject m_objSelected;

	public GameObject m_objSelectedForChange;

	public NKCUIComStateButton m_csbtnChange;

	public GameObject m_objSDRoot;

	public Animator m_amtorChangeEffect;

	public GameObject m_objFavorite;

	private Canvas m_Canvas;

	private GraphicRaycaster m_GraphicRaycaster;

	private NKCUISlot.OnClick m_dOnClick;

	private NKCAssetInstanceData m_cNKCAssetInstanceDataEmoticonSD;

	private SkeletonGraphic m_SkeletonGraphicEmoticon;

	private NKCAssetInstanceData m_cNKCAssetInstanceData;

	private dOnClickChange m_dOnClickChange;

	private int m_PrevSoundID = -1;

	private int m_emoticonId;

	public static NKCPopupEmoticonSlotSD GetNewInstance(Transform parent)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_EMOTICON", "NKM_UI_EMOTICON_SLOT_SD");
		NKCPopupEmoticonSlotSD component = nKCAssetInstanceData.m_Instant.GetComponent<NKCPopupEmoticonSlotSD>();
		if (component == null)
		{
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			Debug.LogError("NKCPopupEmoticonSlotSD Prefab null!");
			return null;
		}
		component.m_cNKCAssetInstanceData = nKCAssetInstanceData;
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.transform.localPosition = new Vector3(component.transform.localPosition.x, component.transform.localPosition.y, 0f);
		component.gameObject.SetActive(value: false);
		return component;
	}

	public void MakeCanvas()
	{
		if (m_Canvas == null)
		{
			m_Canvas = m_objSDRoot.AddComponent<Canvas>();
			m_Canvas.pixelPerfect = false;
			m_Canvas.overrideSorting = true;
			m_Canvas.sortingLayerName = "GAME_UI_FRONT";
		}
		if (m_GraphicRaycaster == null)
		{
			m_GraphicRaycaster = m_objSDRoot.AddComponent<GraphicRaycaster>();
		}
	}

	public void RemoveCanvas()
	{
		Object.Destroy(m_GraphicRaycaster);
		m_GraphicRaycaster = null;
		Object.Destroy(m_Canvas);
		m_Canvas = null;
	}

	public void ResetCanvasLayer(int layer = 100)
	{
		if (m_Canvas != null)
		{
			m_Canvas.sortingOrder = layer;
		}
	}

	private void ClearReusableData()
	{
		if (m_cNKCAssetInstanceDataEmoticonSD != null)
		{
			NKCAssetResourceManager.CloseInstance(m_cNKCAssetInstanceDataEmoticonSD);
			m_cNKCAssetInstanceDataEmoticonSD = null;
		}
		m_SkeletonGraphicEmoticon = null;
		if (m_PrevSoundID > 0)
		{
			NKCSoundManager.StopSound(m_PrevSoundID);
		}
		m_PrevSoundID = -1;
	}

	public void PlayChangeEffect()
	{
		m_amtorChangeEffect.Play("NKM_UI_EMOTICON_CHANGE_BASE", -1, 0f);
	}

	private void OnDestroy()
	{
		if (m_cNKCAssetInstanceData != null)
		{
			NKCAssetResourceManager.CloseInstance(m_cNKCAssetInstanceData);
			m_cNKCAssetInstanceData = null;
		}
		ClearReusableData();
	}

	public void Reset_SD_Scale(float fScale = 0.6f)
	{
		m_objSDRoot.transform.localScale = new Vector3(fScale, fScale, 1f);
	}

	private void Start()
	{
		m_NKCUISlot.Init();
		m_NKCUISlot.m_cbtnButton.dOnPointerHolding = OnClickFavorite;
		m_csbtnChange.PointerClick.RemoveAllListeners();
		m_csbtnChange.PointerClick.AddListener(OnClickChange);
	}

	private void OnClickChange()
	{
		if (m_dOnClickChange != null)
		{
			m_dOnClickChange(GetEmoticonID());
		}
	}

	public void SetClickEvent(NKCUISlot.OnClick _dOnClick)
	{
		m_dOnClick = _dOnClick;
	}

	public void SetClickEventForChange(dOnClickChange _dOnClickChange)
	{
		m_dOnClickChange = _dOnClickChange;
	}

	public void SetUI(int emoticonID)
	{
		m_emoticonId = emoticonID;
		if (m_NKCUISlot.GetSlotData() != null && m_NKCUISlot.GetSlotData().ID != emoticonID)
		{
			ClearReusableData();
		}
		m_NKCUISlot.SetData(NKCUISlot.SlotData.MakeEmoticonData(emoticonID), bEnableLayoutElement: true, m_dOnClick);
		m_NKCUISlot.SetBGVisible(bSet: false);
		NKCUtil.SetGameobjectActive(m_objFavorite, NKCEmoticonManager.IsFavorite(emoticonID));
	}

	public int GetEmoticonID()
	{
		if (m_NKCUISlot.GetSlotData() != null)
		{
			return m_NKCUISlot.GetSlotData().ID;
		}
		return 0;
	}

	public void SetSelected(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_objSelected, bSet);
	}

	public bool GetSelected()
	{
		return m_objSelected.activeSelf;
	}

	public void SetSelectedWithChangeButton(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_objSelectedForChange, bSet);
		NKCUtil.SetGameobjectActive(m_csbtnChange, bSet);
	}

	public void StopSDAni()
	{
		if (m_SkeletonGraphicEmoticon != null)
		{
			m_SkeletonGraphicEmoticon.AnimationState.SetAnimation(0, "BASE_END", loop: false);
		}
		if (m_PrevSoundID > 0)
		{
			NKCSoundManager.StopSound(m_PrevSoundID);
		}
		m_PrevSoundID = -1;
	}

	public void PlaySDAni()
	{
		int emoticonID = GetEmoticonID();
		if (emoticonID <= 0)
		{
			return;
		}
		NKMEmoticonTemplet nKMEmoticonTemplet = NKMEmoticonTemplet.Find(emoticonID);
		if (nKMEmoticonTemplet == null)
		{
			return;
		}
		if (m_cNKCAssetInstanceDataEmoticonSD == null)
		{
			m_cNKCAssetInstanceDataEmoticonSD = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_" + nKMEmoticonTemplet.m_EmoticonAssetName, nKMEmoticonTemplet.m_EmoticonAssetName);
			m_SkeletonGraphicEmoticon = m_cNKCAssetInstanceDataEmoticonSD.m_Instant.GetComponentInChildren<SkeletonGraphic>();
			if (m_SkeletonGraphicEmoticon == null)
			{
				Debug.LogError("PopupEmoticonSlotSD Can't find Skeleton graphic, AssetName : " + nKMEmoticonTemplet.m_EmoticonAssetName);
				return;
			}
			m_cNKCAssetInstanceDataEmoticonSD.m_Instant.transform.SetParent(m_objSDRoot.transform, worldPositionStays: false);
		}
		if (m_SkeletonGraphicEmoticon != null)
		{
			m_SkeletonGraphicEmoticon.AnimationState.SetAnimation(0, "BASE", loop: false);
			m_SkeletonGraphicEmoticon.AnimationState.AddAnimation(0, "BASE_END", loop: false, 0f);
			if (m_PrevSoundID > 0)
			{
				NKCSoundManager.StopSound(m_PrevSoundID);
			}
			m_PrevSoundID = -1;
			if (!string.IsNullOrWhiteSpace(nKMEmoticonTemplet.m_EmoticonSound))
			{
				m_PrevSoundID = NKCSoundManager.PlaySound("AB_FX_UI_EMOTICON_" + nKMEmoticonTemplet.m_EmoticonSound, 1f, 0f, 0f);
			}
		}
	}

	private void OnClickFavorite()
	{
		if (!(m_objFavorite == null))
		{
			NKCPacketSender.Send_NKMPacket_EMOTICON_FAVORITES_SET_REQ(m_emoticonId, !m_objFavorite.activeSelf);
		}
	}
}
