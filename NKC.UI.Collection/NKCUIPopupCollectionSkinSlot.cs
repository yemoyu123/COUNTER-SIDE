using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Collection;

[RequireComponent(typeof(NKCUISkinSlot))]
public class NKCUIPopupCollectionSkinSlot : MonoBehaviour
{
	public GameObject m_specialCutscen;

	public GameObject m_compRoot;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\u033d\ufffd")]
	public GameObject m_objVoice;

	public GameObject m_objVoiceDot;

	public GameObject m_objSkillCutin;

	public GameObject m_objSkillCutinDot;

	[Header("\ufffd\ufffdų \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd")]
	public Image m_imgSkillCutIn;

	public Image m_bgSkillCutIn;

	public Sprite m_spBG_CutinNormal;

	public Sprite m_spBG_CutinSpecial;

	public GameObject m_objCutInSpecial;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffdƮ \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public GameObject m_ellipse03;

	public GameObject m_NKM_UI_SHOP_SKIN_SLOT_COMPONENT_ICON_SKIN_EFFECT;

	[Header("\ufffdƽ\ufffd \ufffd\ufffd\ufffd丮")]
	public GameObject m_ellipse04;

	public GameObject m_NKM_UI_SHOP_SKIN_SLOT_COMPONENT_ICON_SKIN_STORY;

	[Header("\ufffdα\ufffd\ufffd\ufffd \ufffd\u05b4\ufffd")]
	public GameObject m_dotLoginAnim;

	public GameObject m_objComponentLoginAnim;

	[Header("\ufffd\u07f0\ufffd ǥ\ufffd\ufffd")]
	public GameObject m_dotLobbyFace;

	public GameObject m_objComponentLobbyFace;

	[Header("\ufffd\ufffdŲ \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd")]
	public GameObject m_dotConversion;

	public GameObject m_objComponentConversion;

	[Header("\ufffdݶ\ufffd")]
	public GameObject m_dotCollab;

	public GameObject m_objComponentCollab;

	[Header("-\ufffd\ufffd-")]
	public GameObject m_dotGauntlet;

	public GameObject m_objComponentGauntlet;

	private NKCUISkinSlot m_skinSlotComp;

	public int SkinID
	{
		get
		{
			if (!(m_skinSlotComp != null))
			{
				return 0;
			}
			return m_skinSlotComp.SkinID;
		}
	}

	public void Init(NKCUISkinSlot.OnClick dOnClick)
	{
		TryGetComponent<NKCUISkinSlot>(out m_skinSlotComp);
		m_skinSlotComp?.Init(dOnClick);
	}

	public void SetData(NKMUnitTempletBase unitTemplet, bool bEquipped = false)
	{
		m_skinSlotComp?.SetData(unitTemplet, bEquipped);
		NKCUtil.SetGameobjectActive(m_specialCutscen, bValue: false);
		NKCUtil.SetGameobjectActive(m_compRoot, bValue: false);
	}

	public void SetData(NKMSkinTemplet skinTemplet, bool bOwned = false, bool bEquipped = false, bool bBlack = false)
	{
		m_skinSlotComp?.SetData(skinTemplet, bOwned, bEquipped, bBlack);
		bool bValue = !string.IsNullOrEmpty(skinTemplet.m_CutsceneLifetime_start) || !string.IsNullOrEmpty(skinTemplet.m_CutsceneLifetime_end) || !string.IsNullOrEmpty(skinTemplet.m_CutsceneLifetime_BG);
		NKCUtil.SetGameobjectActive(m_specialCutscen, bValue);
		NKCUtil.SetGameobjectActive(m_compRoot, bValue: true);
		SetComponent(skinTemplet);
	}

	public void SetSelected(bool selected)
	{
		m_skinSlotComp?.SetSelected(selected);
	}

	private void SetComponent(NKMSkinTemplet skinTemplet)
	{
		if (skinTemplet == null)
		{
			NKCUtil.SetGameobjectActive(m_objVoice, bValue: false);
			NKCUtil.SetGameobjectActive(m_objSkillCutin, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_SHOP_SKIN_SLOT_COMPONENT_ICON_SKIN_EFFECT, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_SHOP_SKIN_SLOT_COMPONENT_ICON_SKIN_STORY, bValue: false);
			NKCUtil.SetGameobjectActive(m_objComponentLoginAnim, bValue: false);
			NKCUtil.SetGameobjectActive(m_objComponentLobbyFace, bValue: false);
			NKCUtil.SetGameobjectActive(m_objComponentConversion, bValue: false);
			NKCUtil.SetGameobjectActive(m_objComponentCollab, bValue: false);
			NKCUtil.SetGameobjectActive(m_objComponentGauntlet, bValue: false);
			return;
		}
		bool bActivatedBeforeThis = false;
		ActivateIcon(skinTemplet.ChangesVoice(), m_objVoice, m_objVoiceDot);
		ActivateIcon(skinTemplet.ChangesHyperCutin(), m_objSkillCutin, m_objSkillCutinDot);
		if (skinTemplet.ChangesHyperCutin())
		{
			NKCUtil.SetGameobjectActive(m_objCutInSpecial, skinTemplet.m_SkinSkillCutIn == NKMSkinTemplet.SKIN_CUTIN.CUTIN_PRIVATE);
			if (skinTemplet.m_SkinSkillCutIn == NKMSkinTemplet.SKIN_CUTIN.CUTIN_PRIVATE)
			{
				NKCUtil.SetImageSprite(m_bgSkillCutIn, m_spBG_CutinSpecial);
				NKCUtil.SetImageSprite(m_imgSkillCutIn, NKCUtil.GetShopSprite("NKM_UI_SHOP_SKIN_ICON_CUTIN_SPECIAL"));
			}
			else
			{
				NKCUtil.SetImageSprite(m_bgSkillCutIn, m_spBG_CutinNormal);
				NKCUtil.SetImageSprite(m_imgSkillCutIn, NKCUtil.GetShopSprite("NKM_UI_SHOP_SKIN_ICON_CUTIN"));
			}
		}
		ActivateIcon(skinTemplet.m_bEffect, m_NKM_UI_SHOP_SKIN_SLOT_COMPONENT_ICON_SKIN_EFFECT, m_ellipse03);
		ActivateIcon(!string.IsNullOrEmpty(skinTemplet.m_CutscenePurchase), m_NKM_UI_SHOP_SKIN_SLOT_COMPONENT_ICON_SKIN_STORY, m_ellipse04);
		ActivateIcon(skinTemplet.HasLoginCutin, m_objComponentLoginAnim, m_dotLoginAnim);
		ActivateIcon(skinTemplet.m_LobbyFace, m_objComponentLobbyFace, m_dotLobbyFace);
		ActivateIcon(skinTemplet.m_Conversion, m_objComponentConversion, m_dotConversion);
		ActivateIcon(skinTemplet.m_Collabo, m_objComponentCollab, m_dotCollab);
		ActivateIcon(skinTemplet.m_Gauntlet, m_objComponentGauntlet, m_dotGauntlet);
		void ActivateIcon(bool value, GameObject mainObj, GameObject beforeDot)
		{
			NKCUtil.SetGameobjectActive(beforeDot, value && bActivatedBeforeThis);
			NKCUtil.SetGameobjectActive(mainObj, value);
			bActivatedBeforeThis |= value;
		}
	}
}
