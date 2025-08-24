using System.Collections;
using NKC.UI.NPC;
using NKC.UI.Tooltip;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCUIEventBarPhaseCreate : MonoBehaviour
{
	public NKCUIEventBarCreateMenu m_eventBarCreateMenu;

	public Sprite m_spriteCocktailNone;

	public Image m_imgCocktailResult;

	public NKCUIComStateButton m_csbtnCockTailResult;

	[Header("이펙트")]
	public GameObject m_objCocktailFx;

	public Animator m_aniCocktailFx;

	public GameObject m_objCreateFx;

	public NKCComSoundPlayer m_CreateFxSoundPlayer;

	public GameObject m_objMenuSelectOnFx;

	public GameObject m_objMenuSelectOffFx;

	[Header("캐릭터")]
	public RectTransform m_spineRoot;

	public NKCUINPCSpineIllust m_npcSpineIllust;

	public int m_bartenderID;

	[Header("인트로 음성")]
	public NKCUIComRandomVoicePlayer m_introVoice;

	[Header("오늘의 칵테일 음성")]
	public NKCUIComRandomVoicePlayer m_todayCocktailVoice;

	[Header("칵테일 제조 음성")]
	public NKCUIComRandomVoicePlayer m_createVoice;

	[Header("대사 창")]
	public Animator m_aniScript;

	public CanvasGroup m_scriptCanvasGroup;

	public GameObject m_objScriptRoot;

	public Text m_lbUnitName;

	public Text m_lbScriptMsg;

	public float m_showScriptTime;

	public NKCUIComStateButton m_csbtnScriptPanel;

	[Header("대사")]
	public string m_BartenderHello;

	public string m_BartenderReject;

	private NKCUITypeWriter m_typeWriter = new NKCUITypeWriter();

	private Coroutine m_cocktailFxCoroutine;

	private int m_iEventID;

	private int m_iResultCocktailID;

	private float m_fScriptTimer;

	private Coroutine m_scriptCoroutine;

	private bool m_showScript;

	public void Init()
	{
		m_eventBarCreateMenu?.Init();
		if (m_csbtnCockTailResult != null)
		{
			m_csbtnCockTailResult.PointerDown.RemoveAllListeners();
			m_csbtnCockTailResult.PointerDown.AddListener(OnPressCocktailResult);
		}
		NKCUtil.SetButtonClickDelegate(m_csbtnScriptPanel, OnClickScriptPanel);
	}

	public void SetData(int eventID)
	{
		m_iEventID = eventID;
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_bartenderID);
		NKCUtil.SetLabelText(m_lbUnitName, (unitTempletBase != null) ? unitTempletBase.GetUnitName() : "");
		m_eventBarCreateMenu.Open(eventID, m_bartenderID, OnSelectMenu, OnChangeAnimation, OnCreateCocktail, OnCreateRefuse);
		NKCUtil.SetGameobjectActive(m_objCocktailFx, bValue: false);
		NKCUtil.SetGameobjectActive(m_objCreateFx, bValue: false);
		NKCUtil.SetGameobjectActive(m_objMenuSelectOnFx, bValue: false);
		NKCUtil.SetGameobjectActive(m_objMenuSelectOffFx, bValue: false);
		NKCUtil.SetImageSprite(m_imgCocktailResult, m_spriteCocktailNone);
		m_iResultCocktailID = 0;
		if (m_cocktailFxCoroutine != null)
		{
			StopCoroutine(m_cocktailFxCoroutine);
			m_cocktailFxCoroutine = null;
		}
		if (m_scriptCoroutine != null)
		{
			StopCoroutine(m_scriptCoroutine);
			m_scriptCoroutine = null;
		}
		HideScript();
		m_showScript = true;
	}

	public void Close()
	{
		if (m_cocktailFxCoroutine != null)
		{
			StopCoroutine(m_cocktailFxCoroutine);
			m_cocktailFxCoroutine = null;
		}
		m_eventBarCreateMenu.Close();
		if (m_scriptCoroutine != null)
		{
			StopCoroutine(m_scriptCoroutine);
			m_scriptCoroutine = null;
		}
	}

	public void Refresh()
	{
		switch (m_eventBarCreateMenu.CurrentStep)
		{
		case NKCUIEventBarCreateMenu.Step.Step1:
			SetData(m_iEventID);
			break;
		case NKCUIEventBarCreateMenu.Step.Step2:
			m_eventBarCreateMenu.Refresh();
			break;
		}
	}

	public void ActivateCreateFx()
	{
		NKCUtil.SetGameobjectActive(m_objCreateFx, bValue: false);
		NKCUtil.SetGameobjectActive(m_objCreateFx, bValue: true);
		m_CreateFxSoundPlayer?.Play();
	}

	public void OnLeavePhase()
	{
		if (m_objMenuSelectOnFx != null && m_objMenuSelectOnFx.gameObject.activeSelf)
		{
			NKCUtil.SetGameobjectActive(m_objMenuSelectOffFx, bValue: true);
		}
	}

	private void Update()
	{
		if (m_showScript && base.gameObject.activeSelf)
		{
			ShowScript(NKCStringTable.GetString(m_BartenderHello));
			m_fScriptTimer = m_showScriptTime;
			if (m_scriptCoroutine == null)
			{
				m_scriptCoroutine = StartCoroutine(IOnShowRequestScript());
			}
			m_introVoice?.PlayRandomVoice();
			m_npcSpineIllust?.SetDefaultAnimation(NKCASUIUnitIllust.GetAnimationName(NKCASUIUnitIllust.eAnimation.UNIT_IDLE));
			m_showScript = false;
		}
		m_typeWriter.Update();
	}

	private void ShowScript(string message)
	{
		NKCUtil.SetGameobjectActive(m_objScriptRoot, bValue: true);
		m_typeWriter.Start(m_lbScriptMsg, message, 0f, _bTalkAppend: false);
	}

	private void HideScript()
	{
		NKCUtil.SetGameobjectActive(m_objScriptRoot, bValue: false);
		NKCUtil.SetLabelText(m_lbScriptMsg, "");
	}

	private IEnumerator IOnShowRequestScript()
	{
		while (m_fScriptTimer > 0f)
		{
			m_fScriptTimer -= Time.deltaTime;
			yield return null;
		}
		yield return new WaitWhile(() => m_typeWriter.IsTyping());
		m_aniScript?.SetTrigger("OUTRO");
		yield return new WaitWhile(() => m_scriptCanvasGroup.alpha > 0f);
		HideScript();
		m_scriptCoroutine = null;
	}

	private void OnCreateCocktail()
	{
		m_createVoice?.PlayRandomVoice();
	}

	private void OnCreateRefuse()
	{
		ShowScript(NKCStringTable.GetString(m_BartenderReject));
		m_fScriptTimer = m_showScriptTime;
		if (m_scriptCoroutine == null)
		{
			m_scriptCoroutine = StartCoroutine(IOnShowRequestScript());
		}
	}

	private void OnSelectMenu(int cocktailID)
	{
		if (m_imgCocktailResult == null || m_iResultCocktailID == cocktailID)
		{
			return;
		}
		if (cocktailID == 0)
		{
			if (m_cocktailFxCoroutine != null)
			{
				StopCoroutine(m_cocktailFxCoroutine);
				m_cocktailFxCoroutine = null;
			}
			NKCUtil.SetGameobjectActive(m_objMenuSelectOffFx, bValue: true);
			NKCUtil.SetGameobjectActive(m_objCocktailFx, bValue: false);
			m_cocktailFxCoroutine = StartCoroutine(IChangeCocktailImage(m_spriteCocktailNone));
		}
		else if (cocktailID > 0)
		{
			Sprite orLoadMiscItemIcon = NKCResourceUtility.GetOrLoadMiscItemIcon(cocktailID);
			if (m_cocktailFxCoroutine != null)
			{
				StopCoroutine(m_cocktailFxCoroutine);
				m_cocktailFxCoroutine = null;
			}
			NKCUtil.SetGameobjectActive(m_objMenuSelectOnFx, bValue: false);
			NKCUtil.SetGameobjectActive(m_objMenuSelectOnFx, bValue: true);
			NKCUtil.SetGameobjectActive(m_objMenuSelectOffFx, bValue: false);
			NKCUtil.SetGameobjectActive(m_objCocktailFx, bValue: false);
			m_cocktailFxCoroutine = StartCoroutine(IChangeCocktailImage(orLoadMiscItemIcon));
			if (cocktailID == NKCEventBarManager.DailyCocktailItemID)
			{
				m_todayCocktailVoice?.PlayRandomVoice();
			}
		}
		m_iResultCocktailID = cocktailID;
	}

	private void OnChangeAnimation(NKCASUIUnitIllust.eAnimation animation)
	{
		m_npcSpineIllust?.SetAnimation(NKCASUIUnitIllust.GetAnimationName(animation));
	}

	private IEnumerator IChangeCocktailImage(Sprite cocktailImage)
	{
		NKCUtil.SetGameobjectActive(m_objCocktailFx, bValue: true);
		while (m_aniCocktailFx != null && m_aniCocktailFx.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
		{
			yield return null;
		}
		NKCUtil.SetImageSprite(m_imgCocktailResult, cocktailImage);
		while (m_aniCocktailFx != null && m_aniCocktailFx.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
		{
			yield return null;
		}
		NKCUtil.SetGameobjectActive(m_objCocktailFx, bValue: false);
	}

	private void OnPressCocktailResult(PointerEventData eventData)
	{
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(m_iResultCocktailID);
		if (itemMiscTempletByID != null)
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			long count = 0L;
			if (nKMUserData != null)
			{
				count = nKMUserData.m_InventoryData.GetCountMiscItem(itemMiscTempletByID.m_ItemMiscID);
			}
			NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeMiscItemData(m_iResultCocktailID, count);
			NKCUITooltip.Instance.Open(slotData, eventData.position);
		}
	}

	private void OnClickScriptPanel()
	{
		if (m_typeWriter.IsTyping())
		{
			m_typeWriter.Finish();
			return;
		}
		if (m_scriptCoroutine != null)
		{
			StopCoroutine(m_scriptCoroutine);
			m_scriptCoroutine = null;
		}
		HideScript();
	}

	private void OnDestroy()
	{
		if (m_cocktailFxCoroutine != null)
		{
			StopCoroutine(m_cocktailFxCoroutine);
			m_cocktailFxCoroutine = null;
		}
		if (m_scriptCoroutine != null)
		{
			StopCoroutine(m_scriptCoroutine);
			m_scriptCoroutine = null;
		}
		m_typeWriter = null;
	}
}
