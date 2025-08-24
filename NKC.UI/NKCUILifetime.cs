using System.Collections;
using System.Collections.Generic;
using AssetBundles;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUILifetime : NKCUIBase
{
	public const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_personnel";

	public const string UI_ASSET_NAME = "NKM_UI_PERSONNEL_LIFETIME_CONTRACT";

	private static NKCUILifetime m_Instance;

	public Image m_background;

	public NKCUICharacterView m_charView;

	public NKCUILifetimeContract m_contract;

	private NKMUnitData m_targetUnit;

	private bool m_bUpside;

	public GameObject m_FX_UI_CONTRACT_PAPER;

	public GameObject m_FX_UI_CONTRACT_STAMP;

	public GameObject m_FX_UI_CONTRACT_NAME;

	public GameObject m_FX_UI_CONTRACT_CONGRATUALTE;

	public GameObject m_FX_UI_CONTRACT_COMPLETE;

	private bool m_replay;

	private bool m_bLoading;

	public static NKCUILifetime Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUILifetime>("ab_ui_nkm_ui_personnel", "NKM_UI_PERSONNEL_LIFETIME_CONTRACT", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUILifetime>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => NKCUtilString.GET_STRING_LIFETIME;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode
	{
		get
		{
			if (!m_bUpside)
			{
				return NKCUIUpsideMenu.eMode.Disable;
			}
			return base.eUpsideMenuMode;
		}
	}

	public override bool IgnoreBackButtonWhenOpen => !m_bUpside;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public void Init()
	{
		m_charView.Init();
		m_contract.Init(OnEndDrag, EndAni);
	}

	public void Open(NKMUnitData targetUnit, bool replay)
	{
		if (targetUnit != null)
		{
			NKCUIVoiceManager.StopVoice();
			EnableAllLifeTimeSoundFX(bEnable: false);
			m_targetUnit = targetUnit;
			m_replay = replay;
			SetBackground(targetUnit);
			LoadCutScene(bStart: true);
			m_bUpside = true;
			m_contract.SetData(m_targetUnit);
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_contract, bValue: false);
			UIOpened();
			StartCoroutine(CheckScenLoading(bStart: true));
		}
	}

	public override void CloseInternal()
	{
		m_charView.CleanUp();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		NKCSoundManager.PlayScenMusic(NKM_SCEN_ID.NSI_BASE);
	}

	public override void UnHide()
	{
		base.UnHide();
		NKCUtil.SetGameobjectActive(m_contract, bValue: true);
	}

	private void OnEndDrag()
	{
		if (m_targetUnit != null)
		{
			m_bUpside = false;
			NKCUIManager.UpdateUpsideMenu();
			if (m_replay)
			{
				PlayAni();
			}
			else
			{
				NKCPacketSender.Send_NKMPacket_CONTRACT_PERMANENTLY_REQ(m_targetUnit.m_UnitUID);
			}
		}
	}

	public void PlayAni()
	{
		m_contract?.PlayAni();
		LoadCutScene(bStart: false);
	}

	private void EndAni()
	{
		EnableAllLifeTimeSoundFX(bEnable: false);
		StartCoroutine(CheckScenLoading(bStart: false));
	}

	private void LoadCutScene(bool bStart)
	{
		if (m_targetUnit != null)
		{
			Dictionary<string, int> dicSkin;
			string cutsceneName = GetCutsceneName(bStart, m_targetUnit, out dicSkin);
			if (!string.IsNullOrEmpty(cutsceneName))
			{
				LoadCutScene(cutsceneName, bStart, dicSkin);
			}
		}
	}

	private void LoadCutScene(string strID, bool bStart, Dictionary<string, int> dicSkin)
	{
		m_bLoading = true;
		NKCUICutScenPlayer.Instance.UnLoad();
		NKCUICutScenPlayer.Instance.Load(strID, bPreLoad: true, dicSkin);
	}

	private void PlayCutScene(bool bStart)
	{
		Dictionary<string, int> dicSkin;
		string cutsceneName = GetCutsceneName(bStart, m_targetUnit, out dicSkin);
		if (!string.IsNullOrEmpty(cutsceneName))
		{
			if (!bStart)
			{
				NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(m_targetUnit);
				if (skinTemplet == null || string.IsNullOrEmpty(skinTemplet.m_CutsceneLifetime_end) || AssetBundleManager.IsBundleExists(skinTemplet.m_VoiceBundleName))
				{
					NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_LIFETIME_COMPLETE, m_targetUnit);
				}
			}
			NKCUICutScenPlayer.Instance.Play(cutsceneName, 0, delegate
			{
				EndCutScene(bStart);
			});
		}
		else
		{
			EndCutScene(bStart);
		}
	}

	private void SetBackground(NKMUnitData unitData)
	{
		string text = string.Empty;
		if (unitData == null)
		{
			return;
		}
		NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(unitData.m_SkinID);
		if (skinTemplet != null && !string.IsNullOrEmpty(skinTemplet.m_CutsceneLifetime_BG))
		{
			text = skinTemplet.m_CutsceneLifetime_BG;
		}
		if (string.IsNullOrEmpty(text))
		{
			NKCCollectionUnitTemplet unitTemplet = NKCCollectionManager.GetUnitTemplet(unitData.m_UnitID);
			if (unitTemplet != null && !string.IsNullOrEmpty(unitTemplet.m_CutsceneLifetime_BG))
			{
				text = unitTemplet.m_CutsceneLifetime_BG;
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			NKCUtil.SetImageSprite(m_background, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_personnel_lifetime_sprite", "NKM_UI_PERSONNEL_LIFETIME_CONTRACT_BG"));
			return;
		}
		string[] array = text.Split('@');
		if (array.Length > 1)
		{
			NKCUtil.SetImageSprite(m_background, NKCResourceUtility.GetOrLoadAssetResource<Sprite>(array[0], array[1]));
		}
		else
		{
			NKCUtil.SetImageSprite(m_background, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_personnel_lifetime_sprite", text));
		}
	}

	private string GetCutsceneName(bool bStart, NKMUnitData unitData, out Dictionary<string, int> dicSkin)
	{
		string text = string.Empty;
		dicSkin = null;
		if (unitData == null)
		{
			return text;
		}
		NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(unitData.m_SkinID);
		if (skinTemplet != null)
		{
			text = ((!bStart) ? skinTemplet.m_CutsceneLifetime_end : skinTemplet.m_CutsceneLifetime_start);
		}
		if (string.IsNullOrEmpty(text))
		{
			NKCCollectionUnitTemplet unitTemplet = NKCCollectionManager.GetUnitTemplet(unitData.m_UnitID);
			if (unitTemplet != null)
			{
				text = ((!bStart) ? unitTemplet.m_CutsceneLifetime_End : unitTemplet.m_CutsceneLifetime_Start);
			}
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData);
			if (unitTempletBase != null)
			{
				dicSkin = new Dictionary<string, int>();
				dicSkin.Add(unitTempletBase.m_UnitStrID, unitData.m_SkinID);
				if (unitTempletBase.BaseUnit != null)
				{
					dicSkin[unitTempletBase.BaseUnit.m_UnitStrID] = unitData.m_SkinID;
				}
			}
		}
		return text;
	}

	private void EndCutScene(bool bStart)
	{
		if (bStart)
		{
			m_charView.SetCharacterIllust(m_targetUnit, bAsync: false, m_targetUnit.m_SkinID == 0);
			return;
		}
		Close();
		NKCSoundManager.StopAllSound(SOUND_TRACK.VOICE);
	}

	private IEnumerator CheckScenLoading(bool bStart)
	{
		while (!m_bLoading)
		{
			yield return null;
		}
		while (!NKCAssetResourceManager.IsLoadEnd())
		{
			yield return null;
		}
		NKCResourceUtility.SwapResource();
		m_bLoading = false;
		PlayCutScene(bStart);
		yield return null;
	}

	private void EnableAllLifeTimeSoundFX(bool bEnable)
	{
		NKCUtil.SetGameobjectActive(m_FX_UI_CONTRACT_PAPER, bEnable);
		NKCUtil.SetGameobjectActive(m_FX_UI_CONTRACT_STAMP, bEnable);
		NKCUtil.SetGameobjectActive(m_FX_UI_CONTRACT_NAME, bEnable);
		NKCUtil.SetGameobjectActive(m_FX_UI_CONTRACT_CONGRATUALTE, bEnable);
		NKCUtil.SetGameobjectActive(m_FX_UI_CONTRACT_COMPLETE, bEnable);
	}
}
