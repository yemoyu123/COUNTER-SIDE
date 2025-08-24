using System.Text;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Lobby;

public class NKCUILobbyMenuContract : NKCUILobbyMenuButtonBase
{
	public NKCUIComStateButton m_csbtnMenu;

	public GameObject m_objAlert;

	public Text m_lbCompletedContracts;

	public Text m_lbOnProgressContracts;

	private StringBuilder m_StringBuilder = new StringBuilder();

	public GameObject m_CONTRACT_FREE_ON;

	public GameObject m_CONTRACT_FREE_OFF;

	public Text m_NKM_UI_LOBBY_RIGHT_MENU_1_CONTRACT_TEXT3_ON;

	public Text m_NKM_UI_LOBBY_RIGHT_MENU_1_CONTRACT_TEXT3_OFF;

	private float m_fUpdateTimer = 1f;

	private bool m_bNewFreeChanceContract;

	public void Init(ContentsType contentsType)
	{
		if (m_csbtnMenu != null)
		{
			m_csbtnMenu.PointerClick.RemoveAllListeners();
			m_csbtnMenu.PointerClick.AddListener(OnButton);
			m_ContentsType = contentsType;
			NKCUtil.SetGameobjectActive(m_objAlert, bValue: false);
		}
	}

	private void Update()
	{
		if (!m_bLocked)
		{
			m_fUpdateTimer -= Time.deltaTime;
			if (m_fUpdateTimer <= 0f)
			{
				m_fUpdateTimer = 1f;
				UpdateData(null);
			}
		}
	}

	protected override void ContentsUpdate(NKMUserData userData)
	{
		NKCContractDataMgr nKCContractDataMgr = NKCScenManager.GetScenManager().GetNKCContractDataMgr();
		if (nKCContractDataMgr != null)
		{
			if (!nKCContractDataMgr.PossibleFreeContract && !m_bNewFreeChanceContract)
			{
				NKCUtil.SetGameobjectActive(m_CONTRACT_FREE_ON, bValue: false);
				NKCUtil.SetGameobjectActive(m_CONTRACT_FREE_OFF, bValue: true);
				NKCUtil.SetLabelText(m_NKM_UI_LOBBY_RIGHT_MENU_1_CONTRACT_TEXT3_OFF, "--:--:--");
			}
			else if (m_bNewFreeChanceContract || nKCContractDataMgr.IsPossibleFreeChance())
			{
				NKCUtil.SetGameobjectActive(m_CONTRACT_FREE_ON, bValue: true);
				NKCUtil.SetGameobjectActive(m_CONTRACT_FREE_OFF, bValue: false);
				string timeLeftString = NKCSynchronizedTime.GetTimeLeftString(NKCScenManager.GetScenManager().GetNKCContractDataMgr().GetNextResetTime()
					.Ticks);
					NKCUtil.SetLabelText(m_NKM_UI_LOBBY_RIGHT_MENU_1_CONTRACT_TEXT3_ON, timeLeftString);
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_CONTRACT_FREE_ON, bValue: false);
					NKCUtil.SetGameobjectActive(m_CONTRACT_FREE_OFF, bValue: true);
					string timeLeftString2 = NKCSynchronizedTime.GetTimeLeftString(NKCScenManager.GetScenManager().GetNKCContractDataMgr().GetNextResetTime()
						.Ticks);
						NKCUtil.SetLabelText(m_NKM_UI_LOBBY_RIGHT_MENU_1_CONTRACT_TEXT3_OFF, timeLeftString2);
					}
				}
			}

			private void OnButton()
			{
				if (m_bLocked)
				{
					NKCUtil.SetGameobjectActive(m_CONTRACT_FREE_ON, bValue: false);
					NKCUtil.SetGameobjectActive(m_CONTRACT_FREE_OFF, bValue: true);
					NKCUtil.SetLabelText(m_NKM_UI_LOBBY_RIGHT_MENU_1_CONTRACT_TEXT3_OFF, "--:--:--");
					NKCContentManager.ShowLockedMessagePopup(ContentsType.CONTRACT);
				}
				else
				{
					NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_CONTRACT, bForce: false);
				}
			}

			private void OnEnable()
			{
				NKCContractDataMgr nKCContractDataMgr = NKCScenManager.GetScenManager().GetNKCContractDataMgr();
				if (nKCContractDataMgr != null)
				{
					m_bNewFreeChanceContract = nKCContractDataMgr.IsActiveNewFreeChance();
				}
			}
		}
