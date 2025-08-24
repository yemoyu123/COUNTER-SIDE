using System.Collections.Generic;
using ClientPacket.Game;
using NKC.UI;
using NKM;
using Spine.Unity;
using UnityEngine;

namespace NKC;

public class NKCGameHudEmoticon : MonoBehaviour
{
	public NKCUIComStateButton m_csbtnEmoticonSelectPopupOpen;

	public GameObject m_objEmoticonBlockOff;

	public GameObject m_objEmoticonBlockOn;

	public NKCGameHudEmoticonComment m_NKCGameHudEmoticonCommentLeft;

	public NKCGameHudEmoticonComment m_NKCGameHudEmoticonCommentRight;

	public GameObject m_obj_EMOTICON_LEFT;

	private Dictionary<string, SkeletonGraphic> m_dic_sg_EMOTICON_LEFT = new Dictionary<string, SkeletonGraphic>();

	public GameObject m_obj_EMOTICON_RIGHT;

	private Dictionary<string, SkeletonGraphic> m_dic_sg_EMOTICON_RIGHT = new Dictionary<string, SkeletonGraphic>();

	private int m_PrevLeftSoundID;

	private int m_PrevRightSoundID;

	private NKCAssetInstanceData m_cNKCAssetInstanceDataPopup;

	private List<NKCAssetInstanceData> m_lstNKCAssetInstanceDataAni = new List<NKCAssetInstanceData>();

	private NKCPopupInGameEmoticon m_NKCPopupInGameEmoticon;

	public NKCPopupInGameEmoticon NKCPopupInGameEmoticon
	{
		get
		{
			if (m_NKCPopupInGameEmoticon == null)
			{
				m_cNKCAssetInstanceDataPopup = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_EMOTICON", "NKM_UI_EMOTICON");
				if (m_cNKCAssetInstanceDataPopup != null)
				{
					m_NKCPopupInGameEmoticon = m_cNKCAssetInstanceDataPopup.m_Instant.GetComponent<NKCPopupInGameEmoticon>();
					if (m_NKCPopupInGameEmoticon == null)
					{
						Debug.LogError("NKCPopupInGameEmoticon load fail!");
						return null;
					}
					m_NKCPopupInGameEmoticon.transform.SetParent(NKCUIManager.rectFrontCanvas, worldPositionStays: false);
					m_NKCPopupInGameEmoticon.Init();
				}
			}
			return m_NKCPopupInGameEmoticon;
		}
		private set
		{
		}
	}

	public bool IsNKCPopupInGameEmoticonOpen
	{
		get
		{
			if (m_NKCPopupInGameEmoticon != null)
			{
				return m_NKCPopupInGameEmoticon.IsOpen;
			}
			return false;
		}
	}

	public void Awake()
	{
		m_csbtnEmoticonSelectPopupOpen.PointerClick.RemoveAllListeners();
		m_csbtnEmoticonSelectPopupOpen.PointerClick.AddListener(OnClickOpenEmoticonSelectPopup);
	}

	public void Start()
	{
		m_NKCGameHudEmoticonCommentLeft.SetEnableBtn(bSet: false);
		m_NKCGameHudEmoticonCommentRight.SetEnableBtn(bSet: false);
	}

	private void PreLoadEmoticonAni(int emoticonID, bool bLeft)
	{
		NKMEmoticonTemplet nKMEmoticonTemplet = NKMEmoticonTemplet.Find(emoticonID);
		if (nKMEmoticonTemplet == null || nKMEmoticonTemplet.m_EmoticonType != NKM_EMOTICON_TYPE.NET_ANI)
		{
			return;
		}
		if (bLeft)
		{
			if (m_dic_sg_EMOTICON_LEFT.ContainsKey(nKMEmoticonTemplet.m_EmoticonAssetName))
			{
				return;
			}
		}
		else if (m_dic_sg_EMOTICON_RIGHT.ContainsKey(nKMEmoticonTemplet.m_EmoticonAssetName))
		{
			return;
		}
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_" + nKMEmoticonTemplet.m_EmoticonAssetName, nKMEmoticonTemplet.m_EmoticonAssetName);
		SkeletonGraphic componentInChildren = nKCAssetInstanceData.m_Instant.GetComponentInChildren<SkeletonGraphic>();
		if (componentInChildren == null || componentInChildren.AnimationState == null)
		{
			Debug.LogError("emoticon prefab Can't find Skeleton graphic, AssetName : " + nKMEmoticonTemplet.m_EmoticonAssetName);
			return;
		}
		componentInChildren.AnimationState.SetAnimation(0, "BASE_END", loop: false);
		if (bLeft)
		{
			m_dic_sg_EMOTICON_LEFT.Add(nKMEmoticonTemplet.m_EmoticonAssetName, componentInChildren);
			nKCAssetInstanceData.m_Instant.transform.SetParent(m_obj_EMOTICON_LEFT.transform, worldPositionStays: false);
		}
		else
		{
			m_dic_sg_EMOTICON_RIGHT.Add(nKMEmoticonTemplet.m_EmoticonAssetName, componentInChildren);
			nKCAssetInstanceData.m_Instant.transform.SetParent(m_obj_EMOTICON_RIGHT.transform, worldPositionStays: false);
		}
		nKCAssetInstanceData.m_Instant.transform.localPosition = new Vector3(nKCAssetInstanceData.m_Instant.transform.localPosition.x, nKCAssetInstanceData.m_Instant.transform.localPosition.y, 0f);
		m_lstNKCAssetInstanceDataAni.Add(nKCAssetInstanceData);
	}

	private void PreLoadEmoticonAni(List<int> lstEmoticonMy, List<int> lstEmoticonEnemy)
	{
		if (lstEmoticonMy != null)
		{
			for (int i = 0; i < lstEmoticonMy.Count; i++)
			{
				PreLoadEmoticonAni(lstEmoticonMy[i], bLeft: true);
			}
		}
		if (lstEmoticonEnemy != null)
		{
			for (int j = 0; j < lstEmoticonEnemy.Count; j++)
			{
				PreLoadEmoticonAni(lstEmoticonEnemy[j], bLeft: false);
			}
		}
	}

	public void Clear()
	{
		if (m_cNKCAssetInstanceDataPopup != null)
		{
			NKCAssetResourceManager.CloseInstance(m_cNKCAssetInstanceDataPopup);
		}
		m_cNKCAssetInstanceDataPopup = null;
		for (int i = 0; i < m_lstNKCAssetInstanceDataAni.Count; i++)
		{
			NKCAssetResourceManager.CloseInstance(m_lstNKCAssetInstanceDataAni[i]);
		}
		m_lstNKCAssetInstanceDataAni.Clear();
	}

	public void SetEnableUI(bool value)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, value);
		NKCUtil.SetGameobjectActive(m_csbtnEmoticonSelectPopupOpen, value);
	}

	public void SetUI(NKMGameData cNKMGameData)
	{
		if (cNKMGameData == null || !cNKMGameData.IsPVP())
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnEmoticonSelectPopupOpen, bValue: false);
			return;
		}
		SetBlockUI();
		if (NKCScenManager.CurrentUserData() == null)
		{
			return;
		}
		NKMGameTeamData teamData = cNKMGameData.GetTeamData(NKCScenManager.CurrentUserData().m_UserUID);
		if (teamData == null)
		{
			if (cNKMGameData.GetGameType() != NKM_GAME_TYPE.NGT_PVP_PRIVATE)
			{
				return;
			}
			teamData = cNKMGameData.GetTeamData(NKM_TEAM_TYPE.NTT_A1);
		}
		NKMGameTeamData enemyTeamData = cNKMGameData.GetEnemyTeamData(teamData.m_eNKM_TEAM_TYPE);
		if (enemyTeamData != null)
		{
			PreLoadEmoticonAni(teamData.m_emoticonPreset.animationList, enemyTeamData.m_emoticonPreset.animationList);
		}
	}

	public void SetBlockUI()
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			SetBlockUI(gameOptionData.UseEmoticonBlock);
		}
	}

	private void OnClickOpenEmoticonSelectPopup()
	{
		if (NKCReplayMgr.IsPlayingReplay())
		{
			return;
		}
		NKCGameClient gameClient = NKCScenManager.GetScenManager().GetGameClient();
		if (gameClient == null)
		{
			return;
		}
		NKMGameData gameData = gameClient.GetGameData();
		if (gameData == null)
		{
			return;
		}
		NKMGameTeamData teamData = gameData.GetTeamData(NKCScenManager.CurrentUserData().m_UserUID);
		if (teamData == null)
		{
			if (gameData.GetGameType() == NKM_GAME_TYPE.NGT_PVP_PRIVATE)
			{
				ReverseBlockState();
			}
		}
		else if (teamData.m_emoticonPreset != null)
		{
			List<int> list = new List<int>();
			list = teamData.m_emoticonPreset.animationList;
			List<int> list2 = new List<int>();
			list2 = teamData.m_emoticonPreset.textList;
			NKCPopupInGameEmoticon.Open(list, list2);
		}
	}

	private void ReverseBlockState()
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			gameOptionData.UseEmoticonBlock = !gameOptionData.UseEmoticonBlock;
			gameOptionData.Save();
			SetBlockUI();
		}
	}

	private void SetBlockUI(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_objEmoticonBlockOn, bSet);
		NKCUtil.SetGameobjectActive(m_objEmoticonBlockOff, !bSet);
	}

	private void TurnOffEmoticoAni(bool bLeft)
	{
		Dictionary<string, SkeletonGraphic> dictionary = null;
		dictionary = ((!bLeft) ? m_dic_sg_EMOTICON_RIGHT : m_dic_sg_EMOTICON_LEFT);
		foreach (KeyValuePair<string, SkeletonGraphic> item in dictionary)
		{
			if (item.Value != null)
			{
				item.Value.AnimationState.SetAnimation(0, "BASE_END", loop: false);
			}
		}
	}

	private void TurnOnEmoticoAni(bool bLeft, NKMEmoticonTemplet cNKMEmoticonTemplet)
	{
		if (cNKMEmoticonTemplet == null || cNKMEmoticonTemplet.m_EmoticonType != NKM_EMOTICON_TYPE.NET_ANI)
		{
			return;
		}
		Dictionary<string, SkeletonGraphic> dictionary = null;
		dictionary = ((!bLeft) ? m_dic_sg_EMOTICON_RIGHT : m_dic_sg_EMOTICON_LEFT);
		if (!dictionary.ContainsKey(cNKMEmoticonTemplet.m_EmoticonAssetName))
		{
			return;
		}
		foreach (KeyValuePair<string, SkeletonGraphic> item in dictionary)
		{
			if (item.Key == cNKMEmoticonTemplet.m_EmoticonAssetName)
			{
				item.Value.AnimationState.SetAnimation(0, "BASE", loop: false);
				item.Value.AnimationState.AddAnimation(0, "BASE_END", loop: false, 0f);
			}
			else
			{
				item.Value.AnimationState.SetAnimation(0, "BASE_END", loop: false);
			}
		}
	}

	public void OnRecv(NKMPacket_GAME_EMOTICON_NOT cNKMPacket_GAME_EMOTICON_NOT)
	{
		NKMGameData gameData = NKCScenManager.GetScenManager().GetGameClient().GetGameData();
		if (gameData == null)
		{
			return;
		}
		NKMGameTeamData teamData = gameData.GetTeamData(cNKMPacket_GAME_EMOTICON_NOT.senderUserUID);
		if (teamData == null || teamData.m_emoticonPreset == null)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < teamData.m_emoticonPreset.textList.Count; i++)
		{
			if (teamData.m_emoticonPreset.textList[i] == cNKMPacket_GAME_EMOTICON_NOT.emoticonID)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			for (int j = 0; j < teamData.m_emoticonPreset.animationList.Count; j++)
			{
				if (teamData.m_emoticonPreset.animationList[j] == cNKMPacket_GAME_EMOTICON_NOT.emoticonID)
				{
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			return;
		}
		bool flag2 = false;
		if (NKCScenManager.CurrentUserData().m_UserUID == cNKMPacket_GAME_EMOTICON_NOT.senderUserUID)
		{
			flag2 = true;
		}
		else if (gameData.GetTeamData(NKCScenManager.CurrentUserData().m_UserUID) == null)
		{
			flag2 = teamData.m_eNKM_TEAM_TYPE == NKM_TEAM_TYPE.NTT_A1;
		}
		NKMEmoticonTemplet nKMEmoticonTemplet = NKMEmoticonTemplet.Find(cNKMPacket_GAME_EMOTICON_NOT.emoticonID);
		if (nKMEmoticonTemplet == null)
		{
			return;
		}
		if (flag2)
		{
			if (m_PrevLeftSoundID != 0)
			{
				NKCSoundManager.StopSound(m_PrevLeftSoundID);
				m_PrevLeftSoundID = 0;
			}
		}
		else if (m_PrevRightSoundID != 0)
		{
			NKCSoundManager.StopSound(m_PrevRightSoundID);
			m_PrevRightSoundID = 0;
		}
		if (!string.IsNullOrWhiteSpace(nKMEmoticonTemplet.m_EmoticonSound))
		{
			int num = NKCSoundManager.PlaySound("AB_FX_UI_EMOTICON_" + nKMEmoticonTemplet.m_EmoticonSound, 1f, 0f, 0f);
			if (flag2)
			{
				m_PrevLeftSoundID = num;
			}
			else
			{
				m_PrevRightSoundID = num;
			}
		}
		if (nKMEmoticonTemplet.m_EmoticonType == NKM_EMOTICON_TYPE.NET_TEXT)
		{
			if (flag2)
			{
				m_NKCGameHudEmoticonCommentLeft.Play(nKMEmoticonTemplet.m_EmoticonID);
				TurnOffEmoticoAni(bLeft: true);
			}
			else
			{
				m_NKCGameHudEmoticonCommentRight.Play(nKMEmoticonTemplet.m_EmoticonID);
				TurnOffEmoticoAni(bLeft: false);
			}
		}
		else if (nKMEmoticonTemplet.m_EmoticonType == NKM_EMOTICON_TYPE.NET_ANI)
		{
			if (flag2)
			{
				m_NKCGameHudEmoticonCommentLeft.Stop();
				TurnOnEmoticoAni(bLeft: true, nKMEmoticonTemplet);
			}
			else
			{
				m_NKCGameHudEmoticonCommentRight.Stop();
				TurnOnEmoticoAni(bLeft: false, nKMEmoticonTemplet);
			}
		}
	}

	private void Update()
	{
		if (NKCScenManager.GetScenManager().GetNKCPowerSaveMode().GetEnable())
		{
			return;
		}
		if (NKCInputManager.CheckHotKeyEvent(InGamehotkeyEventType.Emoticon))
		{
			if (IsNKCPopupInGameEmoticonOpen)
			{
				NKCPopupInGameEmoticon.Close();
			}
			else
			{
				OnClickOpenEmoticonSelectPopup();
			}
			NKCInputManager.ConsumeHotKeyEvent(InGamehotkeyEventType.Emoticon);
		}
		if (NKCInputManager.CheckHotKeyEvent(HotkeyEventType.ShowHotkey))
		{
			NKCUIComHotkeyDisplay.OpenInstance(m_csbtnEmoticonSelectPopupOpen?.transform, InGamehotkeyEventType.Emoticon);
		}
	}
}
