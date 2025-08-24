using NKM;
using NKM.Shop;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Shop;

public class NKCUIShopSlotSkin : NKCUIShopSlotCard
{
	public GameObject m_objVoice;

	public GameObject m_objVoiceDot;

	public GameObject m_objSkillCutin;

	public GameObject m_objSkillCutinDot;

	public Text m_lbUnitName;

	[Header("스킬 컷인 아이콘")]
	public Image m_imgSkillCutIn;

	public Image m_bgSkillCutIn;

	public Sprite m_spBG_CutinNormal;

	public Sprite m_spBG_CutinSpecial;

	public GameObject m_objCutInSpecial;

	[Header("전용 이펙트 보유 여부")]
	public GameObject m_ellipse03;

	public GameObject m_NKM_UI_SHOP_SKIN_SLOT_COMPONENT_ICON_SKIN_EFFECT;

	[Header("컷신 스토리")]
	public GameObject m_ellipse04;

	public GameObject m_NKM_UI_SHOP_SKIN_SLOT_COMPONENT_ICON_SKIN_STORY;

	[Header("로그인 애니")]
	public GameObject m_dotLoginAnim;

	public GameObject m_objComponentLoginAnim;

	[Header("추가 표정")]
	public GameObject m_dotLobbyFace;

	public GameObject m_objComponentLobbyFace;

	[Header("스킨 컨버전")]
	public GameObject m_dotConversion;

	public GameObject m_objComponentConversion;

	[Header("콜라보")]
	public GameObject m_dotCollab;

	public GameObject m_objComponentCollab;

	[Header("-건-")]
	public GameObject m_dotGauntlet;

	public GameObject m_objComponentGauntlet;

	[Header("배경")]
	public Image m_imgBackground;

	public Sprite m_spBG_Variation;

	public Sprite m_spBG_Normal;

	public Sprite m_spBG_Rare;

	public Sprite m_spBG_Premium;

	public GameObject m_objBG_Special;

	[Header("등급 라인")]
	public Image m_imgGradeLine;

	public Sprite m_spLine_Variation;

	public Sprite m_spLine_Normal;

	public Sprite m_spLine_Rare;

	public Sprite m_spLine_Premium;

	protected override void SetGoodsImage(ShopItemTemplet shopTemplet, bool bFirstBuy)
	{
		NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(shopTemplet.m_ItemID);
		if (skinTemplet == null)
		{
			Debug.LogError($"Skintemplet {shopTemplet.m_ItemID} not found!");
			return;
		}
		Sprite sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, skinTemplet);
		if (sprite == null)
		{
			Debug.LogError($"Skin templet {shopTemplet.m_CardImage}(from productID {shopTemplet.m_ItemID}) null");
		}
		m_imgItem.sprite = sprite;
		SetGoodImageFromSkinData(skinTemplet);
	}

	public void SetGoodImageFromSkinData(NKMSkinTemplet skinTemplet)
	{
		if (skinTemplet == null)
		{
			Debug.LogError($"SkinTemplet not found");
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
		NKCUtil.SetImageSprite(m_imgBackground, GetBGSprite(skinTemplet.m_SkinGrade));
		NKCUtil.SetImageSprite(m_imgGradeLine, GetLineSprite(skinTemplet.m_SkinGrade));
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(skinTemplet.m_SkinEquipUnitID);
		string msg = "";
		if (unitTempletBase != null)
		{
			msg = ((!unitTempletBase.m_bAwaken) ? unitTempletBase.GetUnitName() : NKCStringTable.GetString("SI_PF_SHOP_SKIN_AWAKEN", unitTempletBase.GetUnitName()));
		}
		NKCUtil.SetLabelText(m_lbUnitName, msg);
		ActivateIcon(skinTemplet.m_bEffect, m_NKM_UI_SHOP_SKIN_SLOT_COMPONENT_ICON_SKIN_EFFECT, m_ellipse03);
		ActivateIcon(!string.IsNullOrEmpty(skinTemplet.m_CutscenePurchase), m_NKM_UI_SHOP_SKIN_SLOT_COMPONENT_ICON_SKIN_STORY, m_ellipse04);
		ActivateIcon(skinTemplet.HasLoginCutin, m_objComponentLoginAnim, m_dotLoginAnim);
		ActivateIcon(skinTemplet.m_LobbyFace, m_objComponentLobbyFace, m_dotLobbyFace);
		ActivateIcon(skinTemplet.m_Conversion, m_objComponentConversion, m_dotConversion);
		ActivateIcon(skinTemplet.m_Collabo, m_objComponentCollab, m_dotCollab);
		ActivateIcon(skinTemplet.m_Gauntlet, m_objComponentGauntlet, m_dotGauntlet);
		NKCUtil.SetGameobjectActive(m_objBG_Special, skinTemplet.m_SkinGrade == NKMSkinTemplet.SKIN_GRADE.SG_SPECIAL);
		void ActivateIcon(bool value, GameObject mainObj, GameObject beforeDot)
		{
			NKCUtil.SetGameobjectActive(beforeDot, value && bActivatedBeforeThis);
			NKCUtil.SetGameobjectActive(mainObj, value);
			bActivatedBeforeThis |= value;
		}
	}

	private Sprite GetBGSprite(NKMSkinTemplet.SKIN_GRADE grade)
	{
		switch (grade)
		{
		default:
			return m_spBG_Variation;
		case NKMSkinTemplet.SKIN_GRADE.SG_NORMAL:
			return m_spBG_Normal;
		case NKMSkinTemplet.SKIN_GRADE.SG_RARE:
			return m_spBG_Rare;
		case NKMSkinTemplet.SKIN_GRADE.SG_PREMIUM:
		case NKMSkinTemplet.SKIN_GRADE.SG_SPECIAL:
			return m_spBG_Premium;
		}
	}

	private Sprite GetLineSprite(NKMSkinTemplet.SKIN_GRADE grade)
	{
		switch (grade)
		{
		default:
			return m_spLine_Variation;
		case NKMSkinTemplet.SKIN_GRADE.SG_NORMAL:
			return m_spLine_Normal;
		case NKMSkinTemplet.SKIN_GRADE.SG_RARE:
			return m_spLine_Rare;
		case NKMSkinTemplet.SKIN_GRADE.SG_PREMIUM:
		case NKMSkinTemplet.SKIN_GRADE.SG_SPECIAL:
			return m_spLine_Premium;
		}
	}
}
