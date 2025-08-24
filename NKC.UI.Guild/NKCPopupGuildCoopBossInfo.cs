using NKC.PacketHandler;
using NKC.UI.Guide;
using NKM;
using NKM.Guild;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCPopupGuildCoopBossInfo : NKCUIBase
{
	public delegate void OnStartBoss();

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_CONSORTIUM_COOP";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_CONSORTIUM_COOP_BOSS_INFO";

	private static NKCPopupGuildCoopBossInfo m_Instance;

	public NKCUIComStateButton m_btnClose;

	public NKCUIComStateButton m_btnDetail;

	public NKCUIComStateButton m_btnStart;

	public NKCUIComStateButton m_btnGuide;

	public NKCUIComStateButton m_btnEnemy;

	public Text m_lbRage;

	public Text m_lbName;

	public Text m_lbHP;

	public Image m_imgHP;

	public GameObject m_ObjGaugeRoot;

	public NKCUIComGuildArtifactContent m_Artifact;

	[Header("Extra Boss")]
	public Text m_lbExtraPoint;

	public GameObject m_ObjExtraBossTag;

	public GameObject m_ObjExtraBossScore;

	private GuildRaidTemplet m_GuildRaidTemplet;

	private OnStartBoss m_dOnStartBoss;

	public static NKCPopupGuildCoopBossInfo Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupGuildCoopBossInfo>("AB_UI_NKM_UI_CONSORTIUM_COOP", "NKM_UI_POPUP_CONSORTIUM_COOP_BOSS_INFO", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), CleanupInstance).GetInstance<NKCPopupGuildCoopBossInfo>();
				if (m_Instance != null)
				{
					m_Instance.InitUI();
				}
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

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "";

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public void InitUI()
	{
		if (m_btnClose != null)
		{
			m_btnClose.PointerClick.RemoveAllListeners();
			m_btnClose.PointerClick.AddListener(base.Close);
		}
		if (m_btnDetail != null)
		{
			m_btnDetail.PointerClick.RemoveAllListeners();
			m_btnDetail.PointerClick.AddListener(OnClickDetail);
		}
		if (m_btnStart != null)
		{
			m_btnStart.PointerClick.RemoveAllListeners();
			m_btnStart.PointerClick.AddListener(OnClickStart);
			m_btnStart.m_bGetCallbackWhileLocked = true;
		}
		if (m_Artifact != null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
			m_Artifact.Init();
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		}
		if (m_btnGuide != null)
		{
			m_btnGuide.PointerClick.RemoveAllListeners();
			m_btnGuide.PointerClick.AddListener(OnClickGuide);
		}
		NKCUtil.SetButtonClickDelegate(m_btnEnemy, OnClickEnemy);
	}

	public override void CloseInternal()
	{
		m_Artifact.Close();
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_COOP().OnCloseInfoPopup();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_dOnStartBoss = null;
	}

	public override void OnCloseInstance()
	{
		m_Instance = null;
		base.OnCloseInstance();
	}

	public void Open(GuildRaidTemplet templet, OnStartBoss onStartBoss, int lastBossStageIndex)
	{
		m_GuildRaidTemplet = templet;
		m_dOnStartBoss = onStartBoss;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_Artifact.SetData(NKCGuildCoopManager.GetMyArtifactDictionary());
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(templet.GetStageId());
		if (dungeonTempletBase != null)
		{
			NKCUtil.SetLabelText(m_lbName, dungeonTempletBase.GetDungeonName());
		}
		bool flag = m_GuildRaidTemplet.GetStageIndex() == lastBossStageIndex;
		NKCUtil.SetGameobjectActive(m_ObjGaugeRoot, !flag);
		NKCUtil.SetGameobjectActive(m_lbHP, !flag);
		NKCUtil.SetGameobjectActive(m_lbExtraPoint, flag);
		NKCUtil.SetGameobjectActive(m_ObjExtraBossTag, flag);
		NKCUtil.SetGameobjectActive(m_ObjExtraBossScore, flag);
		UpdateBossInfo();
		UpdateButtonState();
		UIOpened();
	}

	private void UpdateBossInfo()
	{
		NKCUtil.SetLabelText(m_lbRage, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_DUNGEON_RESULT_BOSS_LEVEL_INFO, NKCGuildCoopManager.m_cGuildRaidTemplet.GetStageIndex()));
		float num = NKCGuildCoopManager.m_BossData.remainHp / NKCGuildCoopManager.m_BossMaxHp;
		NKCUtil.SetLabelText(m_lbHP, string.Format("{0} ({1:0.##}%)", NKCGuildCoopManager.m_BossData.remainHp.ToString("N0"), num * 100f));
		NKCUtil.SetLabelText(m_lbExtraPoint, $"{NKCGuildCoopManager.m_BossData.extraPoint}");
		m_imgHP.fillAmount = num;
		NKCUtil.SetGameobjectActive(m_btnGuide, !string.IsNullOrEmpty(m_GuildRaidTemplet.GetGuideShortCut()));
	}

	private void UpdateButtonState()
	{
		m_btnStart.UnLock();
	}

	public void OnClickDetail()
	{
		NKCPopupGuildCoopBossInfoDetail.Instance.Open();
	}

	public void OnClickStart()
	{
		if (!m_btnStart.m_bLock)
		{
			m_dOnStartBoss?.Invoke();
		}
		else
		{
			NKCPacketHandlers.Check_NKM_ERROR_CODE(NKCGuildCoopManager.CanStartBoss());
		}
	}

	private void OnClickGuide()
	{
		if (!string.IsNullOrEmpty(m_GuildRaidTemplet.GetGuideShortCut()))
		{
			NKCUIPopupTutorialImagePanel.Instance.Open(m_GuildRaidTemplet.GetGuideShortCut(), null);
		}
	}

	private void OnClickEnemy()
	{
		if (m_GuildRaidTemplet != null)
		{
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(m_GuildRaidTemplet.GetStageId());
			NKCPopupEnemyList.Instance.Open(dungeonTempletBase);
		}
	}

	public void Refresh()
	{
		m_Artifact.RefreshUI();
		UpdateBossInfo();
		UpdateButtonState();
	}
}
