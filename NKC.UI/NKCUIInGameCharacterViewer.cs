using NKM;
using NKM.Templet;
using Spine.Unity;
using UnityEngine;

namespace NKC.UI;

public class NKCUIInGameCharacterViewer : MonoBehaviour
{
	[Header("스킨 인게임유닛 미리보기")]
	public NKCUIComModelTextureRenderer m_TextureRenderer;

	public GameObject m_objLoading;

	private NKCASUnitSpineSprite m_UnitPreview;

	private int m_UnitPreviewOrigLayer;

	private bool bWaitingForLoading;

	public void Prepare(Material mat = null)
	{
		m_TextureRenderer.PrepareTexture(mat);
	}

	public void CleanUp()
	{
		CloseCurrentPreviewModel();
		m_TextureRenderer.CleanUp();
	}

	private void CloseCurrentPreviewModel()
	{
		if (m_UnitPreview != null && m_UnitPreview.m_UnitSpineSpriteInstant != null && m_UnitPreview.m_UnitSpineSpriteInstant.m_Instant != null)
		{
			m_UnitPreview.m_UnitSpineSpriteInstant.m_Instant.transform.localScale = Vector3.one;
			NKCUtil.SetLayer(m_UnitPreview.m_UnitSpineSpriteInstant.m_Instant.transform, m_UnitPreviewOrigLayer);
		}
		NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_UnitPreview);
		m_UnitPreview = null;
	}

	public void SetPreviewBattleUnit(int unitID, int skinID)
	{
		CloseCurrentPreviewModel();
		if (skinID == 0)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitID);
			m_UnitPreview = NKCUnitViewer.OpenUnitViewerSpineSprite(unitTempletBase, bSub: false, bAsync: true);
			bWaitingForLoading = true;
		}
		else
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(skinID);
			m_UnitPreview = NKCUnitViewer.OpenUnitViewerSpineSprite(skinTemplet, bSub: false, bAsync: true);
			bWaitingForLoading = true;
		}
		NKCUtil.SetGameobjectActive(m_TextureRenderer, bValue: false);
		NKCUtil.SetGameobjectActive(m_objLoading, bWaitingForLoading);
	}

	private void Update()
	{
		if (bWaitingForLoading && m_UnitPreview != null && m_UnitPreview.m_bIsLoaded)
		{
			AfterUnitLoadComplete();
		}
	}

	private void AfterUnitLoadComplete()
	{
		bWaitingForLoading = false;
		if (m_UnitPreview != null && m_UnitPreview.m_UnitSpineSpriteInstant != null && m_UnitPreview.m_cSkeletonAnimation != null)
		{
			m_UnitPreviewOrigLayer = m_UnitPreview.m_UnitSpineSpriteInstant.m_Instant.layer;
			NKCUtil.SetLayer(m_UnitPreview.m_UnitSpineSpriteInstant.m_Instant.transform, 31);
			m_UnitPreview.m_UnitSpineSpriteInstant.m_Instant.transform.SetParent(m_TextureRenderer.transform, worldPositionStays: false);
			m_UnitPreview.m_UnitSpineSpriteInstant.m_Instant.transform.localPosition = new Vector3(0f, 30f, 0f);
			float num = m_TextureRenderer.m_rtImage.GetHeight() / 600f;
			m_UnitPreview.m_UnitSpineSpriteInstant.m_Instant.transform.localScale = Vector3.one * num;
			Transform transform = m_UnitPreview.m_UnitSpineSpriteInstant.m_Instant.transform.Find("SPINE_SkeletonAnimation");
			if (transform != null)
			{
				SkeletonAnimation component = transform.GetComponent<SkeletonAnimation>();
				if (component != null)
				{
					component.AnimationState.SetAnimation(0, "ASTAND", loop: true);
				}
				NKCUtil.SetGameobjectActive(m_TextureRenderer, bValue: true);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_TextureRenderer, bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_objLoading, bValue: false);
	}
}
