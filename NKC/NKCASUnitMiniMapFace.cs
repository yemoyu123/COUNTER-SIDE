using NKC.UI;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCASUnitMiniMapFace : NKMObjectPoolData
{
	public NKCAssetInstanceData m_MiniMapFaceInstant;

	public RectTransform m_RectTransform;

	public Image m_MiniMapFaceImage;

	public NKCAssetResourceData m_MiniMapFaceSprite;

	public GameObject m_MarkerGreen;

	public GameObject m_MarkerRed;

	public GameObject m_UNIT_MINI_MAP_FACE_FX;

	public Animator m_UNIT_MINI_MAP_FACE_FX_Animator;

	public NKCASUnitMiniMapFace(string bundleName, string name, bool bAsync = false)
	{
		m_NKM_OBJECT_POOL_TYPE = NKM_OBJECT_POOL_TYPE.NOPT_NKCASUnitMiniMapFace;
		m_ObjectPoolBundleName = bundleName;
		m_ObjectPoolName = name;
		m_bUnloadable = true;
		Load(bAsync);
	}

	public override void Load(bool bAsync)
	{
		m_MiniMapFaceInstant = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UNIT_MINI_MAP_FACE", "AB_UNIT_MINI_MAP_FACE", bAsync);
		m_MiniMapFaceSprite = NKCAssetResourceManager.OpenResource<Sprite>(m_ObjectPoolBundleName, m_ObjectPoolName, bAsync);
	}

	public override bool LoadComplete()
	{
		if (m_MiniMapFaceInstant == null || m_MiniMapFaceInstant.m_Instant == null)
		{
			Debug.LogError("NKCASUnitMiniMapFace:LoadComplete null " + m_ObjectPoolBundleName + " " + m_ObjectPoolName);
			return false;
		}
		ObjectParentRestore();
		m_RectTransform = m_MiniMapFaceInstant.m_Instant.GetComponentInChildren<RectTransform>();
		m_MiniMapFaceImage = m_MiniMapFaceInstant.m_Instant.GetComponentInChildren<Image>();
		m_MiniMapFaceImage.sprite = m_MiniMapFaceSprite.GetAsset<Sprite>();
		m_MarkerGreen = m_MiniMapFaceInstant.m_Instant.transform.Find("MINI_MAP_FACE_MARKER_GREEN_Panel").gameObject;
		m_MarkerRed = m_MiniMapFaceInstant.m_Instant.transform.Find("MINI_MAP_FACE_MARKER_RED_Panel").gameObject;
		m_UNIT_MINI_MAP_FACE_FX = m_MiniMapFaceInstant.m_Instant.transform.Find("UNIT_MINI_MAP_FACE_FX").gameObject;
		if (m_UNIT_MINI_MAP_FACE_FX != null)
		{
			m_UNIT_MINI_MAP_FACE_FX_Animator = m_UNIT_MINI_MAP_FACE_FX.GetComponentInChildren<Animator>();
		}
		return true;
	}

	public override void Open()
	{
		ObjectParentRestore();
		if (!m_MiniMapFaceInstant.m_Instant.activeSelf)
		{
			m_MiniMapFaceInstant.m_Instant.SetActive(value: true);
		}
	}

	public override void Close()
	{
		ObjectParentWait();
	}

	public override void Unload()
	{
		NKCAssetResourceManager.CloseInstance(m_MiniMapFaceInstant);
		NKCAssetResourceManager.CloseResource(m_MiniMapFaceSprite);
		m_MiniMapFaceInstant = null;
		m_RectTransform = null;
		m_MiniMapFaceImage = null;
		m_MiniMapFaceSprite = null;
		m_MarkerGreen = null;
		m_MarkerRed = null;
		m_UNIT_MINI_MAP_FACE_FX = null;
		m_UNIT_MINI_MAP_FACE_FX_Animator = null;
	}

	public void ObjectParentWait()
	{
		if (m_MiniMapFaceInstant != null && m_MiniMapFaceInstant.m_Instant != null && m_MiniMapFaceInstant.m_Instant.transform.parent != NKCUIManager.m_TR_NKM_WAIT_INSTANT)
		{
			m_MiniMapFaceInstant.m_Instant.transform.SetParent(NKCUIManager.m_TR_NKM_WAIT_INSTANT, worldPositionStays: false);
		}
	}

	public void ObjectParentRestore()
	{
		if (NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA() != null && !(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
			.Get_NUF_GAME_HUD_MINI_MAP() == null) && m_MiniMapFaceInstant != null && !(m_MiniMapFaceInstant.m_Instant == null) && m_MiniMapFaceInstant.m_Instant.transform.parent != NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
			.Get_NUF_GAME_HUD_MINI_MAP()
			.transform)
		{
			m_MiniMapFaceInstant.m_Instant.transform.SetParent(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
				.Get_NUF_GAME_HUD_MINI_MAP()
				.transform, worldPositionStays: false);
			}
		}

		public void SetColor(float fR, float fG, float fB)
		{
			if (!(m_MiniMapFaceImage == null))
			{
				Color color = m_MiniMapFaceImage.color;
				color.r = fR;
				color.g = fG;
				color.b = fB;
				color.a = 1f;
				m_MiniMapFaceImage.color = color;
			}
		}

		public void Warnning()
		{
			if (!(m_UNIT_MINI_MAP_FACE_FX == null) && !(m_UNIT_MINI_MAP_FACE_FX_Animator == null) && m_UNIT_MINI_MAP_FACE_FX.activeInHierarchy)
			{
				m_UNIT_MINI_MAP_FACE_FX_Animator.Play("IDLE", -1);
			}
		}
	}
