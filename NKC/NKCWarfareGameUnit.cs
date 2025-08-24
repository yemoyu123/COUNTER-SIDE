using System.Collections;
using ClientPacket.Community;
using ClientPacket.Warfare;
using DG.Tweening;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC;

public class NKCWarfareGameUnit : MonoBehaviour
{
	public enum NKC_WARFARE_GAME_UNIT_STATE
	{
		NWGUS_IDLE,
		NWGUS_MOVING,
		NWGUS_DYING
	}

	public enum NKC_WARFARE_GAME_UNIT_ANIMATION
	{
		NWGUA_IDLE,
		NWGUA_RUNAWAY,
		NWGUA_ENTER
	}

	public delegate void onClickUnit(int gameUID);

	public delegate void OnTimeEndCallback(NKCWarfareGameUnit unit);

	public GameObject m_goNUM_WARFARE_USER_UNIT_IMG;

	public Image m_ImgUnit;

	public NKCUIComButton m_NKCUIComButton;

	public NKCWarfareUnitMover m_NKCWarfareUnitMover;

	public GameObject m_NUM_WARFARE_FX_SPECIAL_POWER_UP;

	public GameObject m_NUM_WARFARE_FX_TILE_REPAIR;

	public GameObject m_NUM_WARFARE_FX_TILE_SUPPLY;

	public GameObject m_NUM_WARFARE_FX_TILE_THUNDER;

	public DOTweenVisualManager m_NUM_WARFARE_USER_UNIT_SHADOW;

	public DOTweenVisualManager m_NUM_WARFARE_USER_UNIT_IMG;

	public GameObject m_NUM_WARFARE_USER_UNIT_IMG_TURN_ON;

	public GameObject m_NUM_WARFARE_USER_UNIT_END;

	public CanvasGroup m_NUM_WARFARE_USER_UNIT_END_CG;

	public GameObject m_NUM_WARFARE_USER_UNIT_ATTACK;

	public GameObject m_NUM_WARFARE_USER_UNIT_ATTACK_GRAY;

	public GameObject m_NUM_WARFARE_USER_UNIT_CHANGE;

	private WarfareUnitData m_NKMWarfareUnitData;

	private NKCAssetInstanceData m_Instance;

	private onClickUnit m_OnClickUnit;

	private Sequence m_JumpSeq;

	private NKC_WARFARE_GAME_UNIT_STATE m_NKC_WARFARE_GAME_UNIT_STATE;

	private Vector3 m_goNUM_WARFARE_USER_UNIT_IMG_orgPos = new Vector3(0f, 0f, 0f);

	private float m_fTimer;

	private OnTimeEndCallback dOnTimeEndCallback;

	private Animator m_animator;

	private bool m_bPause;

	public int TileIndex => m_NKMWarfareUnitData.tileIndex;

	public bool IsSupporter => m_NKMWarfareUnitData.friendCode != 0;

	public static NKCWarfareGameUnit GetNewInstance(Transform parent, onClickUnit _OnClickUnit)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_WARFARE", "NUM_WARFARE_UNIT");
		NKCWarfareGameUnit component = nKCAssetInstanceData.m_Instant.GetComponent<NKCWarfareGameUnit>();
		if (component == null)
		{
			Debug.LogError("NKCWarfareGameUnit Prefab null!");
			return null;
		}
		component.m_Instance = nKCAssetInstanceData;
		component.m_OnClickUnit = _OnClickUnit;
		component.m_goNUM_WARFARE_USER_UNIT_IMG_orgPos = component.m_goNUM_WARFARE_USER_UNIT_IMG.transform.localPosition;
		component.m_NUM_WARFARE_USER_UNIT_IMG.enabled = false;
		component.m_NUM_WARFARE_USER_UNIT_SHADOW.enabled = false;
		component.m_NKCUIComButton.PointerClick.RemoveAllListeners();
		component.m_NKCUIComButton.PointerClick.AddListener(component.OnClickUnit);
		if (parent != null)
		{
			component.transform.SetParent(parent);
			component.transform.localScale = new Vector3(1f, 1f, 1f);
		}
		component.m_animator = component.GetComponent<Animator>();
		return component;
	}

	public bool IsMoving()
	{
		if (m_NKCWarfareUnitMover == null)
		{
			return false;
		}
		return m_NKCWarfareUnitMover.IsRunning();
	}

	public void ShowUserUnitTileFX(WarfareUnitSyncData cNKMWarfareUnitSyncData)
	{
		if (m_NKMWarfareUnitData.unitType == WarfareUnitData.Type.User)
		{
			if (m_NKMWarfareUnitData.hp > cNKMWarfareUnitSyncData.hp)
			{
				TriggerThunderFX();
			}
			else if (m_NKMWarfareUnitData.supply < cNKMWarfareUnitSyncData.supply)
			{
				TriggerSupplyFX();
			}
		}
	}

	public void TriggerRepairFX()
	{
		if (m_NUM_WARFARE_FX_TILE_REPAIR != null)
		{
			m_NUM_WARFARE_FX_TILE_REPAIR.SetActive(value: false);
			m_NUM_WARFARE_FX_TILE_REPAIR.SetActive(value: true);
		}
	}

	public void TriggerSupplyFX()
	{
		if (m_NUM_WARFARE_FX_TILE_SUPPLY != null)
		{
			m_NUM_WARFARE_FX_TILE_SUPPLY.SetActive(value: false);
			m_NUM_WARFARE_FX_TILE_SUPPLY.SetActive(value: true);
		}
	}

	public void TriggerThunderFX()
	{
		if (m_NUM_WARFARE_FX_TILE_THUNDER != null)
		{
			m_NUM_WARFARE_FX_TILE_THUNDER.SetActive(value: false);
			m_NUM_WARFARE_FX_TILE_THUNDER.SetActive(value: true);
		}
	}

	public void HideFX()
	{
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_TILE_REPAIR, bValue: false);
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_TILE_SUPPLY, bValue: false);
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_FX_TILE_THUNDER, bValue: false);
	}

	public void SetPause(bool bSet)
	{
		m_bPause = bSet;
		if (m_NKCWarfareUnitMover != null)
		{
			m_NKCWarfareUnitMover.SetPause(bSet);
		}
	}

	public void SetState(NKC_WARFARE_GAME_UNIT_STATE _NKC_WARFARE_GAME_UNIT_STATE)
	{
		m_NKC_WARFARE_GAME_UNIT_STATE = _NKC_WARFARE_GAME_UNIT_STATE;
		if (m_NKC_WARFARE_GAME_UNIT_STATE != NKC_WARFARE_GAME_UNIT_STATE.NWGUS_IDLE && m_NKC_WARFARE_GAME_UNIT_STATE == NKC_WARFARE_GAME_UNIT_STATE.NWGUS_MOVING && CheckPossibleBreathing())
		{
			m_goNUM_WARFARE_USER_UNIT_IMG.transform.DOKill();
			m_goNUM_WARFARE_USER_UNIT_IMG.transform.localPosition = m_goNUM_WARFARE_USER_UNIT_IMG_orgPos;
			m_goNUM_WARFARE_USER_UNIT_IMG.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		}
	}

	public NKC_WARFARE_GAME_UNIT_STATE GetState()
	{
		return m_NKC_WARFARE_GAME_UNIT_STATE;
	}

	public void Move(Vector3 _EndPos, float _fTrackingTime, NKCWarfareUnitMover.OnCompleteMove _OnCompleteMove = null, bool bOnlyJump = false)
	{
		if (CheckShipType() && !bOnlyJump)
		{
			m_NKCWarfareUnitMover.Move(_EndPos, _fTrackingTime, _OnCompleteMove);
		}
		else
		{
			if (m_JumpSeq != null && m_JumpSeq.IsActive())
			{
				m_JumpSeq.Kill();
			}
			m_JumpSeq = DOTween.Sequence();
			m_goNUM_WARFARE_USER_UNIT_IMG.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			float num = _fTrackingTime - 0.5f;
			m_JumpSeq.Append(m_goNUM_WARFARE_USER_UNIT_IMG.transform.DORotate(new Vector3(0f, 0f, -5f), num / 3f));
			m_JumpSeq.Append(m_goNUM_WARFARE_USER_UNIT_IMG.transform.DORotate(new Vector3(0f, 0f, 0f), num * 2f / 3f));
			m_NKCWarfareUnitMover.Jump(_EndPos, _fTrackingTime, _OnCompleteMove);
		}
		NKCSoundManager.PlaySound("FX_UI_WARFARE_SHIP_MOVE", 1f, 0f, 0f);
	}

	public void OnClickUnit()
	{
		if (m_OnClickUnit != null)
		{
			m_OnClickUnit(m_NKMWarfareUnitData.warfareGameUnitUID);
		}
	}

	private void ClearSeqs()
	{
		if (m_JumpSeq != null && m_JumpSeq.IsActive())
		{
			m_JumpSeq.Pause();
			m_JumpSeq.Kill();
		}
		m_JumpSeq = null;
	}

	public void Close()
	{
		ClearSeqs();
		m_bPause = false;
		if (m_NKCWarfareUnitMover != null)
		{
			m_NKCWarfareUnitMover.Stop();
		}
		base.gameObject.transform.DOKill();
		if (GetComponent<CanvasGroup>() != null)
		{
			GetComponent<CanvasGroup>().DOKill();
		}
		if (m_ImgUnit != null)
		{
			m_ImgUnit.DOKill();
		}
		if (m_NUM_WARFARE_USER_UNIT_END_CG != null)
		{
			m_NUM_WARFARE_USER_UNIT_END_CG.DOKill();
		}
		if (m_goNUM_WARFARE_USER_UNIT_IMG != null)
		{
			m_goNUM_WARFARE_USER_UNIT_IMG.transform.DOKill();
		}
		if (m_Instance != null)
		{
			NKCAssetResourceManager.CloseInstance(m_Instance);
		}
		m_Instance = null;
	}

	public WarfareUnitData GetNKMWarfareUnitData()
	{
		return m_NKMWarfareUnitData;
	}

	private void Update()
	{
		if (m_NKC_WARFARE_GAME_UNIT_STATE == NKC_WARFARE_GAME_UNIT_STATE.NWGUS_IDLE && m_fTimer > 0f)
		{
			m_fTimer -= Time.deltaTime;
			if (m_fTimer <= 0f && dOnTimeEndCallback != null)
			{
				dOnTimeEndCallback(this);
				dOnTimeEndCallback = null;
			}
		}
	}

	public void SetNKMWarfareUnitData(WarfareUnitData cNKMWarfareUnitData)
	{
		m_NKMWarfareUnitData = cNKMWarfareUnitData;
	}

	public void SetDungeonStrID(string dungeonStrID)
	{
		if (m_NKMWarfareUnitData != null)
		{
			m_NKMWarfareUnitData.dungeonID = NKMDungeonManager.GetDungeonID(dungeonStrID);
		}
	}

	public void SetDeckIndex(NKMDeckIndex sNKMDeckIndex)
	{
		if (m_NKMWarfareUnitData != null)
		{
			m_NKMWarfareUnitData.deckIndex = sNKMDeckIndex;
		}
	}

	public void SetWarfareGameUnitUID(int uid)
	{
		if (m_NKMWarfareUnitData != null)
		{
			m_NKMWarfareUnitData.warfareGameUnitUID = uid;
		}
	}

	private void ResetWinBuff()
	{
		_ = m_NUM_WARFARE_FX_SPECIAL_POWER_UP != null;
	}

	private bool CheckShipTypeEvenIfDungeon(int dungeonID)
	{
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(m_NKMWarfareUnitData.dungeonID);
		if (dungeonTempletBase != null && dungeonTempletBase.m_DungeonIcon == "")
		{
			NKMDungeonTemplet dungeonTemplet = NKMDungeonManager.GetDungeonTemplet(dungeonTempletBase.m_DungeonID);
			if (dungeonTemplet != null)
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(dungeonTemplet.m_BossUnitStrID);
				if (unitTempletBase != null && unitTempletBase.m_UnitID > 50000 && unitTempletBase.m_UnitID < 60000)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void PlayClickAni()
	{
		if (m_NKMWarfareUnitData != null && m_NKMWarfareUnitData.unitType == WarfareUnitData.Type.Dungeon && m_goNUM_WARFARE_USER_UNIT_IMG != null && !CheckShipTypeEvenIfDungeon(m_NKMWarfareUnitData.dungeonID))
		{
			ClearSeqs();
			m_goNUM_WARFARE_USER_UNIT_IMG.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			m_goNUM_WARFARE_USER_UNIT_IMG.transform.DOShakeScale(0.8f, new Vector3(0f, 0.07f, 0f), 5, 20f);
		}
	}

	public void PlayEnemySpawnAni()
	{
		if (m_NKMWarfareUnitData != null && m_NKMWarfareUnitData.unitType == WarfareUnitData.Type.Dungeon && m_goNUM_WARFARE_USER_UNIT_IMG != null)
		{
			m_goNUM_WARFARE_USER_UNIT_IMG.transform.localScale = new Vector3(1f, 0f, 1f);
			m_goNUM_WARFARE_USER_UNIT_IMG.transform.DOScaleY(1f, 0.4f).SetEase(Ease.OutBack);
		}
	}

	public bool CheckShipType()
	{
		if (m_NKMWarfareUnitData != null)
		{
			if (m_NKMWarfareUnitData.unitType == WarfareUnitData.Type.User)
			{
				return true;
			}
			if (CheckShipTypeEvenIfDungeon(m_NKMWarfareUnitData.dungeonID))
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public void PlayDieAni()
	{
		SetState(NKC_WARFARE_GAME_UNIT_STATE.NWGUS_DYING);
		if (m_goNUM_WARFARE_USER_UNIT_IMG != null && m_NKMWarfareUnitData != null)
		{
			if (m_NKMWarfareUnitData.unitType == WarfareUnitData.Type.User)
			{
				m_goNUM_WARFARE_USER_UNIT_IMG.transform.DORotate(new Vector3(0f, 0f, 20f), 2f);
			}
			else if (CheckShipTypeEvenIfDungeon(m_NKMWarfareUnitData.dungeonID))
			{
				m_goNUM_WARFARE_USER_UNIT_IMG.transform.DORotate(new Vector3(0f, 0f, -20f), 2f);
			}
			else
			{
				m_goNUM_WARFARE_USER_UNIT_IMG.transform.DOShakeRotation(2f);
			}
		}
	}

	private void EnableAni()
	{
		m_NUM_WARFARE_USER_UNIT_IMG.enabled = true;
		m_NUM_WARFARE_USER_UNIT_SHADOW.enabled = true;
	}

	public void PlayAni(NKC_WARFARE_GAME_UNIT_ANIMATION aniType, UnityAction completeAction = null)
	{
		string text = string.Empty;
		switch (aniType)
		{
		case NKC_WARFARE_GAME_UNIT_ANIMATION.NWGUA_RUNAWAY:
			text = "NUM_WARFARE_UNIT_RUNAWAY";
			break;
		case NKC_WARFARE_GAME_UNIT_ANIMATION.NWGUA_ENTER:
			text = "NUM_WARFARE_UNIT_ENTER";
			break;
		}
		if (text != string.Empty)
		{
			m_animator.Play(text);
			if (completeAction != null)
			{
				StartCoroutine(OnCompleteAni(completeAction, text));
			}
		}
	}

	private IEnumerator OnCompleteAni(UnityAction completeCallback, string aniName)
	{
		if (!m_animator.GetCurrentAnimatorStateInfo(0).IsName(aniName))
		{
			yield return null;
		}
		while (m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
		{
			yield return null;
		}
		while (m_bPause)
		{
			yield return null;
		}
		completeCallback?.Invoke();
	}

	public void UpdateTurnUI()
	{
		if (m_NKMWarfareUnitData == null)
		{
			return;
		}
		if (m_NKMWarfareUnitData.unitType == WarfareUnitData.Type.User)
		{
			ResetWinBuff();
		}
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		if (warfareGameData == null)
		{
			return;
		}
		bool flag = warfareGameData.CheckTeamA_By_GameUnitUID(m_NKMWarfareUnitData.warfareGameUnitUID);
		if (warfareGameData.isTurnA == flag && m_NKMWarfareUnitData.hp > 0f)
		{
			NKCUtil.SetGameobjectActive(m_NUM_WARFARE_USER_UNIT_END, m_NKMWarfareUnitData.isTurnEnd);
			NKCUtil.SetGameobjectActive(m_NUM_WARFARE_USER_UNIT_IMG_TURN_ON, flag && !m_NKMWarfareUnitData.isTurnEnd);
			if (m_NKMWarfareUnitData.isTurnEnd)
			{
				if (m_ImgUnit != null)
				{
					m_ImgUnit.DOColor(new Color(42f / 85f, 42f / 85f, 42f / 85f, 1f), 0.9f).SetEase(Ease.InCubic);
				}
				if (m_NUM_WARFARE_USER_UNIT_END_CG != null)
				{
					m_NUM_WARFARE_USER_UNIT_END_CG.DOFade(1f, 0.9f).SetEase(Ease.InCubic);
				}
			}
			else
			{
				if (m_ImgUnit != null)
				{
					m_ImgUnit.DOColor(new Color(1f, 1f, 1f, 1f), 0.9f).SetEase(Ease.OutCubic);
				}
				if (m_NUM_WARFARE_USER_UNIT_END_CG != null)
				{
					m_NUM_WARFARE_USER_UNIT_END_CG.alpha = 0f;
				}
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_NUM_WARFARE_USER_UNIT_IMG_TURN_ON, bValue: false);
			NKCUtil.SetGameobjectActive(m_NUM_WARFARE_USER_UNIT_END, bValue: false);
			if (m_NUM_WARFARE_USER_UNIT_END_CG != null)
			{
				m_NUM_WARFARE_USER_UNIT_END_CG.alpha = 0f;
			}
			if (m_ImgUnit != null)
			{
				m_ImgUnit.DOColor(new Color(1f, 1f, 1f, 1f), 0.9f).SetEase(Ease.OutCubic);
			}
		}
	}

	public void SetTurnEndTimer(OnTimeEndCallback onTimeEndCallback)
	{
		dOnTimeEndCallback = onTimeEndCallback;
		m_fTimer = 0.9f;
	}

	public bool CheckPossibleBreathing()
	{
		if (m_NKMWarfareUnitData != null && m_NKMWarfareUnitData.unitType == WarfareUnitData.Type.Dungeon)
		{
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(m_NKMWarfareUnitData.dungeonID);
			if (dungeonTempletBase != null && !CheckShipTypeEvenIfDungeon(dungeonTempletBase.m_DungeonID))
			{
				return true;
			}
		}
		return false;
	}

	public void SetBreathingMotion()
	{
		m_goNUM_WARFARE_USER_UNIT_IMG.transform.DOScale(new Vector3(1.04f, 1.04f, 1.04f), 1.5f + Random.value).SetLoops(-1, LoopType.Yoyo);
	}

	public void OneTimeSetUnitUI()
	{
		if (m_NKMWarfareUnitData == null)
		{
			return;
		}
		if (m_NKMWarfareUnitData.unitType == WarfareUnitData.Type.User)
		{
			base.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			m_NKCUIComButton.UpdateOrgSize();
			NKMUnitTempletBase nKMUnitTempletBase = null;
			if (!IsSupporter)
			{
				NKMDeckIndex deckIndex = m_NKMWarfareUnitData.deckIndex;
				NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
				NKMDeckData deckData = myUserData.m_ArmyData.GetDeckData(deckIndex);
				if (deckData != null)
				{
					NKMUnitData shipFromUID = myUserData.m_ArmyData.GetShipFromUID(deckData.m_ShipUID);
					if (shipFromUID != null)
					{
						nKMUnitTempletBase = NKMUnitManager.GetUnitTempletBase(shipFromUID.m_UnitID);
					}
				}
			}
			else
			{
				WarfareSupporterListData supportUnitData = NKCScenManager.GetScenManager().WarfareGameData.supportUnitData;
				if (supportUnitData != null && supportUnitData.commonProfile.friendCode == m_NKMWarfareUnitData.friendCode)
				{
					nKMUnitTempletBase = NKMUnitManager.GetUnitTempletBase(supportUnitData.deckData.GetShipUnitId());
				}
			}
			if (nKMUnitTempletBase != null)
			{
				Sprite orLoadMinimapFaceIcon = NKCResourceUtility.GetOrLoadMinimapFaceIcon(nKMUnitTempletBase.m_MiniMapFaceName);
				if (orLoadMinimapFaceIcon == null)
				{
					NKCAssetResourceData assetResourceUnitInvenIconEmpty = NKCResourceUtility.GetAssetResourceUnitInvenIconEmpty();
					if (assetResourceUnitInvenIconEmpty != null)
					{
						m_ImgUnit.sprite = assetResourceUnitInvenIconEmpty.GetAsset<Sprite>();
					}
					else
					{
						m_ImgUnit.sprite = null;
					}
				}
				else
				{
					m_ImgUnit.sprite = orLoadMinimapFaceIcon;
				}
			}
			EnableAni();
		}
		else if (m_NKMWarfareUnitData.unitType == WarfareUnitData.Type.Dungeon)
		{
			bool flag = false;
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(m_NKMWarfareUnitData.dungeonID);
			if (dungeonTempletBase != null)
			{
				if (CheckShipTypeEvenIfDungeon(dungeonTempletBase.m_DungeonID))
				{
					EnableAni();
				}
				if (dungeonTempletBase.m_DungeonIcon == "")
				{
					NKMDungeonTemplet dungeonTemplet = NKMDungeonManager.GetDungeonTemplet(dungeonTempletBase.m_DungeonID);
					if (dungeonTemplet != null)
					{
						NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(dungeonTemplet.m_BossUnitStrID);
						if (unitTempletBase != null)
						{
							Sprite orLoadMinimapFaceIcon2 = NKCResourceUtility.GetOrLoadMinimapFaceIcon(unitTempletBase.m_MiniMapFaceName);
							if (orLoadMinimapFaceIcon2 != null)
							{
								m_ImgUnit.sprite = orLoadMinimapFaceIcon2;
								flag = true;
							}
						}
					}
				}
				else
				{
					Sprite orLoadMinimapFaceIcon3 = NKCResourceUtility.GetOrLoadMinimapFaceIcon(dungeonTempletBase.m_DungeonIcon);
					if (orLoadMinimapFaceIcon3 != null)
					{
						m_ImgUnit.sprite = orLoadMinimapFaceIcon3;
						flag = true;
					}
				}
			}
			if (!flag)
			{
				NKCAssetResourceData assetResourceUnitInvenIconEmpty2 = NKCResourceUtility.GetAssetResourceUnitInvenIconEmpty();
				if (assetResourceUnitInvenIconEmpty2 != null)
				{
					m_ImgUnit.sprite = assetResourceUnitInvenIconEmpty2.GetAsset<Sprite>();
				}
				else
				{
					m_ImgUnit.sprite = null;
				}
			}
			if (m_NUM_WARFARE_USER_UNIT_END != null)
			{
				m_NUM_WARFARE_USER_UNIT_END.transform.localScale = new Vector3(-1f, 1f, 1f);
			}
			base.gameObject.transform.localScale = new Vector3(-1f, 1f, 1f);
			m_NKCUIComButton.UpdateOrgSize();
		}
		UpdateTurnUI();
	}

	public void SetAttackIcon(bool bActive, bool bGray = false)
	{
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_USER_UNIT_ATTACK, bActive && !bGray);
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_USER_UNIT_ATTACK_GRAY, bActive && bGray);
	}

	public void SetChangeIcon(bool bActive)
	{
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_USER_UNIT_CHANGE, bActive);
	}
}
