using ClientPacket.Community;
using ClientPacket.Guild;
using ClientPacket.Warfare;
using DG.Tweening;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCWarfareGameUnitInfo : MonoBehaviour
{
	public GameObject m_NUM_WARFARE_UNIT_FLAG;

	public GameObject m_NUM_WARFARE_UNIT_TARGET;

	public GameObject m_NUM_WARFARE_UNIT_INCR;

	public Text m_NUM_WARFARE_UNIT_LV_TEXT;

	public GameObject m_NUM_WARFARE_UNIT_HP;

	public GameObject m_NUM_WARFARE_UNIT_HP_1;

	public Image m_NUM_WARFARE_UNIT_HP_1_Img;

	public GameObject m_NUM_WARFARE_UNIT_HP_2;

	public Image m_NUM_WARFARE_UNIT_HP_2_Img;

	public GameObject m_NUM_WARFARE_UNIT_HP_3;

	public Image m_NUM_WARFARE_UNIT_HP_3_Img;

	public GameObject m_NUM_WARFARE_UNIT_BATTLE_POINT;

	public GameObject m_NUM_WARFARE_UNIT_BATTLE_POINT_ACTIVE_1;

	public GameObject m_NUM_WARFARE_UNIT_BATTLE_POINT_ACTIVE_2;

	public GameObject m_NUM_WARFARE_UNIT_ENEMY_WAVE;

	public Text m_NUM_WARFARE_UNIT_ENEMY_WAVE_TEXT;

	public GameObject m_NUM_WARFARE_UNIT_ENEMY_MOVE;

	public Text m_NUM_WARFARE_UNIT_ENEMY_MOVE2;

	public GameObject m_NUM_WARFARE_UNIT_ENEMY_MOVE_TOLEADER;

	public GameObject m_NUM_WARFARE_UNIT_DECKNUMBER;

	public Text m_NUM_WARFARE_UNIT_DECKNUMBER_COUNT;

	public GameObject m_NUM_WARFARE_UNIT_DECKNUMBER_SUPPORT;

	public GameObject m_NUM_WARFARE_UNIT_DECKNUMBER_FRIEND_GUEST;

	public GameObject m_NUM_WARFARE_UNIT_DECKNUMBER_GUILD_GUEST;

	public GameObject m_NUM_WARFARE_UNIT_SPECIALTILE_REPAIR;

	public GameObject m_NUM_WARFARE_UNIT_BATTLEASSIST;

	private Transform m_UnitTransform;

	private WarfareUnitData m_NKMWarfareUnitData;

	private bool m_bFlag;

	private bool m_bTarget;

	private NKCAssetInstanceData m_Instance;

	private Animator m_animator;

	private NKMWarfareMapTemplet m_NKMWarfareMapTemplet;

	private Sequence m_flagSequence;

	public int TileIndex => m_NKMWarfareUnitData.tileIndex;

	public bool IsSupporter => m_NKMWarfareUnitData.friendCode != 0;

	public long FriendCode => m_NKMWarfareUnitData.friendCode;

	public void SetFlag(bool bSet)
	{
		m_bFlag = bSet;
	}

	public bool GetFlag()
	{
		return m_bFlag;
	}

	public void SetTartget(bool bSet)
	{
		m_bTarget = bSet;
	}

	public bool GetTarget()
	{
		return m_bTarget;
	}

	public static NKCWarfareGameUnitInfo GetNewInstance(Transform parent, Transform unitTrans)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_WARFARE", "NUM_WARFARE_UNIT_INFO");
		NKCWarfareGameUnitInfo component = nKCAssetInstanceData.m_Instant.GetComponent<NKCWarfareGameUnitInfo>();
		if (component == null)
		{
			Debug.LogError("NKCWarfareGameUnitInfo Prefab null!");
			return null;
		}
		component.m_Instance = nKCAssetInstanceData;
		if (parent != null)
		{
			component.transform.SetParent(parent);
			component.transform.localScale = new Vector3(1f, 1f, 1f);
		}
		component.m_UnitTransform = unitTrans;
		component.m_animator = component.GetComponent<Animator>();
		return component;
	}

	private NKMWarfareMapTemplet GetNKMWarfareMapTemplet()
	{
		if (m_NKMWarfareMapTemplet == null)
		{
			WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
			if (warfareGameData != null)
			{
				NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(warfareGameData.warfareTempletID);
				if (nKMWarfareTemplet != null)
				{
					m_NKMWarfareMapTemplet = nKMWarfareTemplet.MapTemplet;
				}
			}
		}
		return m_NKMWarfareMapTemplet;
	}

	public void Close()
	{
		if (m_flagSequence != null)
		{
			m_flagSequence.Kill();
			m_flagSequence = null;
		}
		if (m_NUM_WARFARE_UNIT_FLAG != null)
		{
			m_NUM_WARFARE_UNIT_FLAG.transform.DOKill();
		}
		if (m_Instance != null)
		{
			NKCAssetResourceManager.CloseInstance(m_Instance);
		}
		m_Instance = null;
	}

	public void PlayFlagAni()
	{
		if (m_NUM_WARFARE_UNIT_FLAG != null && m_NUM_WARFARE_UNIT_FLAG.activeSelf)
		{
			m_NUM_WARFARE_UNIT_FLAG.transform.localScale = new Vector3(1f, 1f, 1f);
			if (m_flagSequence != null)
			{
				m_flagSequence.Kill();
				m_flagSequence = null;
			}
			m_flagSequence = DOTween.Sequence();
			m_flagSequence.Append(m_NUM_WARFARE_UNIT_FLAG.transform.DOScale(new Vector3(2.5f, 2.5f, 2.5f), 0.7f).From().SetEase(Ease.OutQuad));
			m_flagSequence.Join(m_NUM_WARFARE_UNIT_FLAG.transform.DOMoveY(20f, 0.7f).From(isRelative: true).SetEase(Ease.OutQuad));
		}
	}

	public void PlayAni(NKCWarfareGameUnit.NKC_WARFARE_GAME_UNIT_ANIMATION aniType)
	{
		string text = string.Empty;
		switch (aniType)
		{
		case NKCWarfareGameUnit.NKC_WARFARE_GAME_UNIT_ANIMATION.NWGUA_RUNAWAY:
			text = "NUM_WARFARE_UNIT_INFO_RUNAWAY";
			break;
		case NKCWarfareGameUnit.NKC_WARFARE_GAME_UNIT_ANIMATION.NWGUA_ENTER:
			text = "NUM_WARFARE_UNIT_INFO_ENTER";
			break;
		}
		if (text != string.Empty)
		{
			m_animator.Play(text);
		}
	}

	public void SetUnitTransform(Transform trans)
	{
		m_UnitTransform = trans;
	}

	public void SetNKMWarfareUnitData(WarfareUnitData cNKMWarfareUnitData)
	{
		m_NKMWarfareUnitData = cNKMWarfareUnitData;
	}

	public WarfareUnitData GetNKMWarfareUnitData()
	{
		return m_NKMWarfareUnitData;
	}

	private void SetSuuplyCountUI(int count)
	{
		if (count <= 0)
		{
			NKCUtil.SetGameobjectActive(m_NUM_WARFARE_UNIT_BATTLE_POINT_ACTIVE_1, bValue: false);
			NKCUtil.SetGameobjectActive(m_NUM_WARFARE_UNIT_BATTLE_POINT_ACTIVE_2, bValue: false);
		}
		if (count == 1)
		{
			NKCUtil.SetGameobjectActive(m_NUM_WARFARE_UNIT_BATTLE_POINT_ACTIVE_1, bValue: true);
			NKCUtil.SetGameobjectActive(m_NUM_WARFARE_UNIT_BATTLE_POINT_ACTIVE_2, bValue: false);
		}
		if (count >= 2)
		{
			NKCUtil.SetGameobjectActive(m_NUM_WARFARE_UNIT_BATTLE_POINT_ACTIVE_1, bValue: true);
			NKCUtil.SetGameobjectActive(m_NUM_WARFARE_UNIT_BATTLE_POINT_ACTIVE_2, bValue: true);
		}
	}

	public void SetUnitInfoUI()
	{
		if (m_NKMWarfareUnitData == null)
		{
			return;
		}
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_UNIT_FLAG, m_bFlag);
		if (m_NUM_WARFARE_UNIT_FLAG != null)
		{
			m_NUM_WARFARE_UNIT_FLAG.transform.localScale = new Vector3(1f, 1f, 1f);
		}
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_UNIT_TARGET, m_bTarget);
		if (m_NKMWarfareUnitData.unitType == WarfareUnitData.Type.User)
		{
			NKCUtil.SetGameobjectActive(m_NUM_WARFARE_UNIT_BATTLE_POINT, bValue: true);
			NKCUtil.SetGameobjectActive(m_NUM_WARFARE_UNIT_INCR, bValue: false);
			SetMovableIcon(active: false);
			SetActionTypeIcon(active: false);
			bool flag = NKCGuildManager.HasGuild() && NKCGuildManager.MyGuildData.members.Find((NKMGuildMemberData x) => x.commonProfile.friendCode == m_NKMWarfareUnitData.friendCode) != null;
			bool flag2 = NKCWarfareManager.IsGeustSupporter(m_NKMWarfareUnitData.friendCode);
			NKCUtil.SetGameobjectActive(m_NUM_WARFARE_UNIT_DECKNUMBER, !IsSupporter);
			NKCUtil.SetGameobjectActive(m_NUM_WARFARE_UNIT_DECKNUMBER_SUPPORT, IsSupporter && !flag && !flag2);
			NKCUtil.SetGameobjectActive(m_NUM_WARFARE_UNIT_DECKNUMBER_FRIEND_GUEST, IsSupporter && !flag && flag2);
			NKCUtil.SetGameobjectActive(m_NUM_WARFARE_UNIT_DECKNUMBER_GUILD_GUEST, IsSupporter && flag);
			bool flag3 = NKCWarfareManager.CheckOnTileType(GetNKMWarfareMapTemplet(), TileIndex, NKM_WARFARE_MAP_TILE_TYPE.NWMTT_REPAIR);
			bool flag4 = NKCWarfareManager.CheckOnTileType(GetNKMWarfareMapTemplet(), TileIndex, NKM_WARFARE_MAP_TILE_TYPE.NWMTT_RESUPPLY);
			bool flag5 = NKCWarfareManager.CheckOnTileType(GetNKMWarfareMapTemplet(), TileIndex, NKM_WARFARE_MAP_TILE_TYPE.NWNTT_SERVICE);
			NKCUtil.SetGameobjectActive(m_NUM_WARFARE_UNIT_SPECIALTILE_REPAIR, flag3 || flag4 || flag5);
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			string hexRGB;
			if (!IsSupporter)
			{
				hexRGB = "#5AFB55";
				NKMDeckIndex deckIndex = m_NKMWarfareUnitData.deckIndex;
				m_NUM_WARFARE_UNIT_DECKNUMBER_COUNT.text = (deckIndex.m_iIndex + 1).ToString();
				NKMUnitData deckLeaderUnitData = myUserData.m_ArmyData.GetDeckLeaderUnitData(deckIndex);
				if (deckLeaderUnitData != null)
				{
					m_NUM_WARFARE_UNIT_LV_TEXT.text = deckLeaderUnitData.m_UnitLevel.ToString();
				}
			}
			else
			{
				hexRGB = "#FFC501";
				WarfareSupporterListData supportUnitData = NKCScenManager.GetScenManager().WarfareGameData.supportUnitData;
				if (supportUnitData != null && supportUnitData.commonProfile.friendCode == m_NKMWarfareUnitData.friendCode)
				{
					NKMDummyUnitData nKMDummyUnitData = supportUnitData.deckData.List[supportUnitData.deckData.LeaderIndex];
					m_NUM_WARFARE_UNIT_LV_TEXT.text = nKMDummyUnitData.UnitLevel.ToString();
				}
			}
			SetSuuplyCountUI(m_NKMWarfareUnitData.supply);
			m_NUM_WARFARE_UNIT_HP_1_Img.color = NKCUtil.GetColor(hexRGB);
			m_NUM_WARFARE_UNIT_HP_2_Img.color = m_NUM_WARFARE_UNIT_HP_1_Img.color;
			m_NUM_WARFARE_UNIT_HP_3_Img.color = m_NUM_WARFARE_UNIT_HP_1_Img.color;
			UpdateHPBarUI();
		}
		else
		{
			if (m_NKMWarfareUnitData.unitType != WarfareUnitData.Type.Dungeon)
			{
				return;
			}
			NKCUtil.SetGameobjectActive(m_NUM_WARFARE_UNIT_BATTLE_POINT, bValue: false);
			NKCUtil.SetGameobjectActive(m_NUM_WARFARE_UNIT_INCR, m_NKMWarfareUnitData.isSummonee);
			SetMovableIcon(IsMovableActionType());
			SetActionTypeIcon(active: true, m_NKMWarfareUnitData.warfareEnemyActionType);
			NKCUtil.SetGameobjectActive(m_NUM_WARFARE_UNIT_DECKNUMBER, bValue: false);
			NKCUtil.SetGameobjectActive(m_NUM_WARFARE_UNIT_SPECIALTILE_REPAIR, bValue: false);
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(m_NKMWarfareUnitData.dungeonID);
			if (dungeonTempletBase != null)
			{
				NKCUtil.SetGameobjectActive(m_NUM_WARFARE_UNIT_HP, dungeonTempletBase.m_DungeonType != NKM_DUNGEON_TYPE.NDT_WAVE);
				NKCUtil.SetGameobjectActive(m_NUM_WARFARE_UNIT_ENEMY_WAVE, dungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_WAVE);
				m_NUM_WARFARE_UNIT_LV_TEXT.text = dungeonTempletBase.m_DungeonLevel.ToString();
				if (dungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_WAVE)
				{
					NKMDungeonTemplet dungeonTemplet = NKMDungeonManager.GetDungeonTemplet(dungeonTempletBase.m_DungeonID);
					if (dungeonTemplet != null)
					{
						m_NUM_WARFARE_UNIT_ENEMY_WAVE_TEXT.text = string.Format(NKCUtilString.GET_STRING_WARFARE_WAVE_ONE_PARAM, dungeonTemplet.m_listDungeonWave.Count);
					}
				}
			}
			m_NUM_WARFARE_UNIT_HP_1_Img.color = new Color(1f, 0f, 26f / 85f);
			m_NUM_WARFARE_UNIT_HP_2_Img.color = m_NUM_WARFARE_UNIT_HP_1_Img.color;
			m_NUM_WARFARE_UNIT_HP_3_Img.color = m_NUM_WARFARE_UNIT_HP_1_Img.color;
			UpdateHPBarUI();
		}
	}

	private float GetProperRatioValue(float fRatio)
	{
		if (fRatio < 0f)
		{
			fRatio = 0f;
		}
		if (fRatio > 1f)
		{
			fRatio = 1f;
		}
		return fRatio;
	}

	public void OnPlayServiceSound(NKM_WARFARE_SERVICE_TYPE serviceType)
	{
		if (m_NKMWarfareUnitData.unitType == WarfareUnitData.Type.User && serviceType == NKM_WARFARE_SERVICE_TYPE.NWST_RESUPPLY)
		{
			NKCOperatorUtil.PlayVoice(m_NKMWarfareUnitData.deckIndex, VOICE_TYPE.VT_BULLET_FILL);
		}
	}

	private void UpdateHPBarUI()
	{
		if (m_NKMWarfareUnitData == null)
		{
			return;
		}
		if (m_NKMWarfareUnitData.hpMax == 0f)
		{
			m_NUM_WARFARE_UNIT_HP_1_Img.transform.localScale = new Vector3(1f, 1f, 1f);
			m_NUM_WARFARE_UNIT_HP_2_Img.transform.localScale = new Vector3(1f, 1f, 1f);
			m_NUM_WARFARE_UNIT_HP_3_Img.transform.localScale = new Vector3(1f, 1f, 1f);
			return;
		}
		float fRatio = m_NKMWarfareUnitData.hp / m_NKMWarfareUnitData.hpMax;
		fRatio = GetProperRatioValue(fRatio);
		if (fRatio > 0.6f)
		{
			m_NUM_WARFARE_UNIT_HP_1_Img.transform.localScale = new Vector3(1f, 1f, 1f);
			m_NUM_WARFARE_UNIT_HP_2_Img.transform.localScale = new Vector3(1f, 1f, 1f);
			m_NUM_WARFARE_UNIT_HP_3_Img.transform.localScale = new Vector3(GetProperRatioValue((fRatio - 0.6f) / 0.4f), 1f, 1f);
		}
		else if (fRatio > 0.3f)
		{
			m_NUM_WARFARE_UNIT_HP_1_Img.transform.localScale = new Vector3(1f, 1f, 1f);
			m_NUM_WARFARE_UNIT_HP_2_Img.transform.localScale = new Vector3(GetProperRatioValue((fRatio - 0.3f) / 0.3f), 1f, 1f);
			m_NUM_WARFARE_UNIT_HP_3_Img.transform.localScale = new Vector3(0f, 1f, 1f);
		}
		else
		{
			m_NUM_WARFARE_UNIT_HP_1_Img.transform.localScale = new Vector3(GetProperRatioValue(fRatio / 0.3f), 1f, 1f);
			m_NUM_WARFARE_UNIT_HP_2_Img.transform.localScale = new Vector3(0f, 1f, 1f);
			m_NUM_WARFARE_UNIT_HP_3_Img.transform.localScale = new Vector3(0f, 1f, 1f);
		}
	}

	private void SetMovableIcon(bool active, int movable = 1)
	{
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_UNIT_ENEMY_MOVE, active);
		if (active)
		{
			m_NUM_WARFARE_UNIT_ENEMY_MOVE2.text = movable.ToString();
		}
	}

	private void SetActionTypeIcon(bool active, NKM_WARFARE_ENEMY_ACTION_TYPE actionType = NKM_WARFARE_ENEMY_ACTION_TYPE.NWEAT_NONE)
	{
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_UNIT_ENEMY_MOVE_TOLEADER, active && actionType == NKM_WARFARE_ENEMY_ACTION_TYPE.NWEAT_ONLY_FLAG_SHIP_ATK);
	}

	private void Update()
	{
		if (m_UnitTransform != null)
		{
			base.gameObject.transform.localPosition = m_UnitTransform.localPosition;
		}
	}

	public bool IsMovableActionType()
	{
		if (m_NKMWarfareUnitData == null)
		{
			return false;
		}
		if (m_NKMWarfareUnitData.warfareEnemyActionType != NKM_WARFARE_ENEMY_ACTION_TYPE.NWEAT_NEAREST_ATK && m_NKMWarfareUnitData.warfareEnemyActionType != NKM_WARFARE_ENEMY_ACTION_TYPE.NWEAT_ONLY_FLAG_SHIP_ATK && m_NKMWarfareUnitData.warfareEnemyActionType != NKM_WARFARE_ENEMY_ACTION_TYPE.NWEAT_FIND_WIN_TILE)
		{
			return m_NKMWarfareUnitData.warfareEnemyActionType == NKM_WARFARE_ENEMY_ACTION_TYPE.NWEAT_FIND_LOSE_TILE;
		}
		return true;
	}

	public void SetBattleAssistIcon(bool bActive)
	{
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_UNIT_BATTLEASSIST, bActive);
	}
}
