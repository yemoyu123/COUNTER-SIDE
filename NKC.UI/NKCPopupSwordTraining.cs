using System;
using System.Collections;
using System.Collections.Generic;
using NKC.UI.Component;
using NKM;
using UnityEngine;
using UnityEngine.Events;

namespace NKC.UI;

public class NKCPopupSwordTraining : MonoBehaviour
{
	private enum GAME_STATE
	{
		NONE,
		IDLE,
		INIT,
		READY,
		START,
		PLAY,
		GAMEOVER,
		RESULT,
		END
	}

	[Header("\ufffd\ufffd\ufffdΰ\ufffd ĳ\ufffd\ufffd\ufffd\ufffd SD")]
	public string m_strUnitAssetBundleName = "AB_UNIT_EVENT_SD_ACADEMY_C_TWINTAIL";

	public string m_strUnitAssetName = "SPINE_NKM_EVENT_SD_ACADEMY_C_TWINTAIL";

	[Space]
	public string m_strAttackFXAssetBundleName = "ab_fx_ui_slash";

	public string m_strAttackFXAssetName = "AB_FX_UI_SLASH_02";

	public string m_strAttackHitFXAssetName = "AB_FX_UI_SLASH_03";

	private float m_fAttackRadius = 45f;

	[Header("ĳ\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public RectTransform m_rtLeftAttackSocket;

	public RectTransform m_rtRightAttackSocket;

	private float m_fMonsterJumpPosX = 600f;

	private float m_fMonsterJumpPosY = 1000f;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public RectTransform m_rtCenter;

	public RectTransform m_rtSpawnPointLeft;

	public RectTransform m_rtSpawnPointRight;

	public RectTransform m_rtSpawnParent;

	private float m_fIncreaseSpawnTimePercent = 0.5f;

	private int m_iSpawnMonsterPerStage = 24;

	private float m_fDefaultSpawnTime = 2000f;

	private float m_fMinimumSpawnTime = 1000f;

	private float m_fKnockBackValue = 300f;

	private int m_iBossHp = 3;

	private int Score;

	private int m_iAttackDmg = 1;

	private int Score1;

	private GAME_STATE m_eGameState;

	private float m_fMonsterSpeedSlow = 0.1f;

	private float m_fMonsterSpeedNormal = 0.2f;

	private float m_fMonsterSpeedFast = 0.3f;

	public float m_fIncreaseMonsterSpeedPerStage = 0.5f;

	[Header("UI")]
	public NKCComTMPUIText m_lbScore;

	public NKCUIComStateButton m_csbtnTouchL;

	public NKCUIComStateButton m_csbtnTouchR;

	private UnityAction m_dOnGameEnd;

	private int m_iMiniGameKey;

	private const string ANI_NAME_START = "START";

	private bool m_bJoinEffect;

	private NKCSwordTrainingWeapon m_Weapon;

	private NKCAssetInstanceData m_AttackObjectAsssetInstanceData;

	private NKCAssetInstanceData m_AttackFxAsssetInstanceData;

	private float m_fAttackDurationTime = 0.1f;

	private Transform m_trAttackHitFX;

	private int m_iCurrentStageLevel;

	private List<double> m_lstMonsterSpawnTimes = new List<double>();

	private int m_iMonsterID;

	private int m_iNormalMonsterKillCount;

	private int m_iBossMonsterKillCount;

	private int m_iBestGameScore;

	[Header("\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public RectTransform m_rtBackgroundMonsterParent;

	public RectTransform m_rtBackgroundMonsterSpawnPositionLeft;

	public RectTransform m_rtBackgroundMonsterSpawnPositionRight;

	private int m_iCreateBackGroundMonsterMaxCount = 3;

	private float m_fCreateBackGroundMonsterSpwanDelay = 20f;

	[Header("\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd Ÿ\ufffd\ufffd(\ufffdش\ufffd \ufffdð\ufffd\ufffd\ufffd\ufffd\ufffd 1\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffdݴ\ufffd\ufffd \ufffd\ufffd)")]
	public int m_iBackGroundMonsterTurnTime = 10;

	private List<NKCSwordTraningBackgroundMonster> m_lstBackMonsters = new List<NKCSwordTraningBackgroundMonster>();

	private int m_iCurBackGroundMonsterCount;

	private NKCASUIUnitIllust m_spineSD;

	private RectTransform m_rtSDTransfrom;

	private List<NKCSwordTrainingMonster> m_lstMonsters = new List<NKCSwordTrainingMonster>();

	private int Score3 => TotalKillCount;

	private int TotalKillCount => m_iNormalMonsterKillCount + m_iBossMonsterKillCount;

	private bool IsPlayableState => m_eGameState == GAME_STATE.PLAY;

	public void InitUI(UnityAction onGameEnd)
	{
		m_eGameState = GAME_STATE.IDLE;
		m_dOnGameEnd = onGameEnd;
		m_csbtnTouchL.SetHotkey(HotkeyEventType.Left);
		m_csbtnTouchR.SetHotkey(HotkeyEventType.Right);
		NKCUtil.SetBindFunction(m_csbtnTouchL, delegate
		{
			OnAttack(bAttackLeft: true);
		});
		NKCUtil.SetBindFunction(m_csbtnTouchR, delegate
		{
			OnAttack(bAttackLeft: false);
		});
	}

	public void CleanUp()
	{
		StopAllCoroutines();
		if (m_AttackObjectAsssetInstanceData != null)
		{
			NKCAssetResourceManager.CloseInstance(m_AttackObjectAsssetInstanceData);
			m_AttackObjectAsssetInstanceData = null;
		}
		if (m_AttackFxAsssetInstanceData != null)
		{
			NKCAssetResourceManager.CloseInstance(m_AttackFxAsssetInstanceData);
			m_AttackFxAsssetInstanceData = null;
		}
		if (m_spineSD != null)
		{
			NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_spineSD);
			m_spineSD = null;
		}
	}

	public void Open(int miniGameKey)
	{
		m_iMiniGameKey = miniGameKey;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		UpdateState(GAME_STATE.INIT);
	}

	private void Update()
	{
		CheckInput();
		UpdateMonster();
	}

	private void CheckInput()
	{
		if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
		{
			OnAttack(bAttackLeft: true);
		}
		if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
		{
			OnAttack(bAttackLeft: false);
		}
	}

	private void UpdateState(GAME_STATE newState)
	{
		if (m_eGameState == newState)
		{
			return;
		}
		m_eGameState = newState;
		switch (newState)
		{
		case GAME_STATE.INIT:
			InitGame();
			break;
		case GAME_STATE.READY:
			ReadyGame();
			UpdateState(GAME_STATE.START);
			if (!m_bJoinEffect)
			{
				NKCSoundManager.PlaySound("FX_CUTSCEN_SYS_START1", 1f, 0f, 0f);
				m_spineSD?.SetAnimation("START", loop: false);
				m_bJoinEffect = true;
			}
			break;
		case GAME_STATE.START:
			StartCoroutine(CreateBGMonster());
			StartCoroutine(TurnRandomBGMonster());
			PrepareMonsterSpawnTime();
			UpdateState(GAME_STATE.PLAY);
			break;
		case GAME_STATE.PLAY:
			StartCoroutine(SpawnMonter());
			break;
		case GAME_STATE.RESULT:
			StopAllCoroutines();
			UpdateGameResult();
			ClearData();
			ClearMonster();
			break;
		case GAME_STATE.GAMEOVER:
			break;
		}
	}

	private void InitGame()
	{
		if (m_spineSD == null)
		{
			if (!OpenSpineIllust(m_strUnitAssetBundleName, m_strUnitAssetName, ref m_spineSD, m_rtCenter))
			{
				return;
			}
			Transform transform = m_spineSD.GetRectTransform().Find("SPINE_SkeletonGraphic");
			if (null != transform)
			{
				m_rtSDTransfrom = transform.GetComponent<RectTransform>();
			}
			BoxCollider2D boxCollider2D = m_spineSD.GetRectTransform().gameObject.AddComponent<BoxCollider2D>();
			if (null != boxCollider2D)
			{
				boxCollider2D.size = new Vector2(100f, 150f);
				boxCollider2D.isTrigger = true;
			}
			NKCSwordTrainingCollision nKCSwordTrainingCollision = m_spineSD.GetRectTransform().gameObject.AddComponent<NKCSwordTrainingCollision>();
			if (null != nKCSwordTrainingCollision)
			{
				nKCSwordTrainingCollision.Init(OnCrashMonster);
			}
		}
		if (m_AttackObjectAsssetInstanceData == null)
		{
			m_AttackObjectAsssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>(m_strAttackFXAssetBundleName, m_strAttackFXAssetName);
			if (m_AttackObjectAsssetInstanceData != null && null != m_AttackObjectAsssetInstanceData.m_Instant)
			{
				Transform transform2 = m_AttackObjectAsssetInstanceData.m_Instant.transform;
				if (null != transform2)
				{
					transform2.SetParent(m_rtLeftAttackSocket, worldPositionStays: false);
					m_Weapon = transform2.GetComponent<NKCSwordTrainingWeapon>();
					if (null == m_Weapon)
					{
						m_Weapon = transform2.gameObject.AddComponent<NKCSwordTrainingWeapon>();
					}
					m_Weapon.Init(transform2, OnKillMonster, AttackFail, m_fAttackRadius, m_rtLeftAttackSocket, m_rtRightAttackSocket);
					m_Weapon.SetData(m_iAttackDmg, m_fKnockBackValue);
				}
			}
		}
		if (m_AttackFxAsssetInstanceData == null)
		{
			m_AttackFxAsssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>(m_strAttackFXAssetBundleName, m_strAttackHitFXAssetName);
			if (m_AttackFxAsssetInstanceData != null && null != m_AttackFxAsssetInstanceData.m_Instant)
			{
				m_trAttackHitFX = m_AttackFxAsssetInstanceData.m_Instant.transform;
				NKCUtil.SetGameobjectActive(m_trAttackHitFX.gameObject, bValue: false);
				m_Weapon.SetHitFX(m_trAttackHitFX);
			}
		}
		m_bJoinEffect = false;
		ClearData();
		ClearMonster();
		UpdateState(GAME_STATE.READY);
	}

	private void ReadyGame()
	{
		NKCSoundManager.PlayMusic("THEMA_CA_TWINTAIL", bLoop: true);
		ClearMonster(bClearBGMonster: false);
		UpdateScoreUI();
		m_Weapon.m_IsAttacking = false;
	}

	private void ClearMonster(bool bClearBGMonster = true)
	{
		m_iMonsterID = 0;
		for (int i = 0; i < m_lstMonsters.Count; i++)
		{
			m_lstMonsters[i].rtPosition = null;
			m_lstMonsters[i].Clear();
			m_lstMonsters[i] = null;
		}
		m_lstMonsters.Clear();
		if (bClearBGMonster)
		{
			m_iCurBackGroundMonsterCount = 0;
			for (int j = 0; j < m_lstBackMonsters.Count; j++)
			{
				m_lstBackMonsters[j].Clear();
				m_lstBackMonsters[j] = null;
			}
			m_lstBackMonsters.Clear();
		}
	}

	private void PrepareMonsterSpawnTime()
	{
		m_lstMonsterSpawnTimes.Clear();
		System.Random random = new System.Random();
		double num = m_fDefaultSpawnTime - m_fDefaultSpawnTime * ((float)m_iCurrentStageLevel * m_fIncreaseSpawnTimePercent);
		num = (((double)m_fMinimumSpawnTime >= num) ? ((double)m_fMinimumSpawnTime) : num);
		for (int i = 0; i < m_iSpawnMonsterPerStage && m_iSpawnMonsterPerStage > i; i++)
		{
			double item = Math.Round(random.NextDouble() * num, 2);
			if (m_lstMonsterSpawnTimes.Contains(item))
			{
				i--;
			}
			else
			{
				m_lstMonsterSpawnTimes.Add(Math.Round(random.NextDouble() * num, 2) * 0.009999999776482582);
			}
		}
		m_lstMonsterSpawnTimes.Sort();
	}

	private IEnumerator SpawnMonter()
	{
		int iCnt = 0;
		float fPrevSpawnTime = 0f;
		yield return new WaitForSeconds(0.5f);
		while (iCnt < m_iSpawnMonsterPerStage)
		{
			float fSpawnTime = (float)m_lstMonsterSpawnTimes[iCnt];
			yield return new WaitForSeconds(fSpawnTime - fPrevSpawnTime);
			if (!IsPlayableState)
			{
				break;
			}
			fPrevSpawnTime = fSpawnTime;
			int num = iCnt + 1;
			iCnt = num;
			CreateMonster();
		}
		yield return null;
	}

	private void CreateMonster(bool bMidBoss = false)
	{
		if (!IsPlayableState)
		{
			return;
		}
		bool flag = UnityEngine.Random.Range(1, 11) >= 5;
		float num = 0f;
		int increaseLevel = m_iCurrentStageLevel + 1;
		string assetBundleName;
		string assetName;
		if (bMidBoss)
		{
			assetBundleName = "AB_UNIT_EVENT_MONSTER_BOSS";
			assetName = "SPINE_NKM_EVENT_MONSTER_BOSS";
			num = GetMonsterSpeed(bFast: true, increaseLevel);
		}
		else
		{
			bool flag2 = UnityEngine.Random.Range(1, 11) >= 5;
			assetBundleName = (flag2 ? "AB_UNIT_EVENT_MONSTER_NORMAL" : "AB_UNIT_EVENT_MONSTER_YELLOW");
			assetName = (flag2 ? "SPINE_NKM_EVENT_MONSTER_NORMAL" : "SPINE_NKM_EVENT_MONSTER_YELLOW");
			num = GetMonsterSpeed(flag2, increaseLevel);
		}
		NKCASUIUnitIllust monster = GetMonster(assetBundleName, assetName, m_rtSpawnParent);
		if (monster == null)
		{
			return;
		}
		RectTransform rectTransform = monster.GetRectTransform();
		if (!(null == rectTransform))
		{
			if (!flag)
			{
				rectTransform.SetPositionAndRotation(m_rtSpawnParent.position, new Quaternion(0f, 180f, 0f, 0f));
			}
			rectTransform.position = (flag ? m_rtSpawnPointLeft.position : m_rtSpawnPointRight.position);
			int hp = 1;
			rectTransform.localScale = Vector3.one;
			if (bMidBoss)
			{
				hp = m_iBossHp + m_iCurrentStageLevel;
			}
			NKCSwordTrainingMonster nKCSwordTrainingMonster = rectTransform.gameObject.GetComponent<NKCSwordTrainingMonster>();
			if (null == nKCSwordTrainingMonster)
			{
				nKCSwordTrainingMonster = rectTransform.gameObject.AddComponent<NKCSwordTrainingMonster>();
			}
			nKCSwordTrainingMonster.Init(bMidBoss);
			nKCSwordTrainingMonster.SetData(m_iMonsterID, monster, rectTransform, flag, num, hp, m_fMonsterJumpPosX, m_fMonsterJumpPosY);
			m_iMonsterID++;
			m_lstMonsters.Add(nKCSwordTrainingMonster);
		}
	}

	private NKCASUIUnitIllust GetMonster(string assetBundleName, string assetName, RectTransform SpawnParent)
	{
		NKCASUIUnitIllust SpineIllust = null;
		if (!OpenSpineIllust(assetBundleName, assetName, ref SpineIllust, SpawnParent))
		{
			return null;
		}
		return SpineIllust;
	}

	private float GetMonsterSpeed(bool bFast, int increaseLevel)
	{
		if (bFast)
		{
			return m_fMonsterSpeedFast + m_fMonsterSpeedFast * (m_fIncreaseMonsterSpeedPerStage * (float)increaseLevel);
		}
		return m_fMonsterSpeedNormal + m_fMonsterSpeedNormal * (m_fIncreaseMonsterSpeedPerStage * (float)increaseLevel);
	}

	private void UpdateMonster()
	{
		if (IsPlayableState)
		{
			_ = m_lstMonsters.Count;
		}
	}

	private void OnAttack(bool bAttackLeft)
	{
		if (IsPlayableState && !m_Weapon.m_IsAttacking)
		{
			Quaternion rotation = (bAttackLeft ? new Quaternion(0f, 180f, 0f, 0f) : new Quaternion(0f, 0f, 0f, 0f));
			m_rtSDTransfrom.SetPositionAndRotation(m_rtCenter.position, rotation);
			m_spineSD.SetAnimation(NKCASUIUnitIllust.eAnimation.SD_ATTACK, loop: false);
			NKCSoundManager.PlaySound("FX_COMBAT_ALL_SWORD_SLASH_SMALL_14", 1f, 0f, 0f);
			StopCoroutine(OnWeaponAttack());
			StartCoroutine(OnWeaponAttack(bAttackLeft));
		}
	}

	private IEnumerator OnWeaponAttack(bool bLeft = false)
	{
		m_Weapon.OnAttack(bActive: true, bLeft);
		yield return new WaitForSeconds(m_fAttackDurationTime);
		m_Weapon.OnAttack(bActive: false, bLeft);
		yield return null;
	}

	private void AttackFail()
	{
		if (IsPlayableState)
		{
			StopAllCoroutines();
			StartCoroutine(OnGameOver(0.1f));
		}
	}

	private void OnCrashMonster(int iID, bool bLeftAttacked)
	{
		if (IsPlayableState)
		{
			StopAllCoroutines();
			StartCoroutine(OnGameOver(0.15f, iID));
		}
	}

	private IEnumerator OnGameOver(float fWaitTime, int attackMonsterID = -1)
	{
		bool _bMonsterAttacked = false;
		bool _bLeftAttack = false;
		foreach (NKCSwordTrainingMonster lstMonster in m_lstMonsters)
		{
			if (lstMonster.iId == attackMonsterID)
			{
				_bLeftAttack = lstMonster.bSpawnLeft;
				_bMonsterAttacked = true;
				lstMonster.m_illust.SetAnimation(NKCASUIUnitIllust.eAnimation.SD_ATTACK, loop: true);
			}
			else
			{
				lstMonster.m_illust.SetAnimSpeed(0f);
			}
			if (!(null == lstMonster.rig2D))
			{
				lstMonster.rig2D.isKinematic = true;
				lstMonster.rig2D.velocity = Vector2.zero;
				lstMonster.rig2D.gravityScale = 0f;
			}
		}
		foreach (NKCSwordTraningBackgroundMonster lstBackMonster in m_lstBackMonsters)
		{
			if (!(null == lstBackMonster))
			{
				lstBackMonster.m_illust.SetAnimSpeed(0f);
				lstBackMonster.rig2D.isKinematic = true;
				lstBackMonster.rig2D.velocity = Vector2.zero;
				lstBackMonster.rig2D.gravityScale = 0f;
				lstBackMonster.SetDead();
			}
		}
		m_Weapon?.Deactive();
		UpdateState(GAME_STATE.GAMEOVER);
		yield return new WaitForSeconds(fWaitTime);
		if (_bMonsterAttacked)
		{
			Quaternion rotation = (_bLeftAttack ? new Quaternion(0f, 180f, 0f, 0f) : new Quaternion(0f, 0f, 0f, 0f));
			m_rtSDTransfrom.SetPositionAndRotation(m_rtCenter.position, rotation);
		}
		NKCSoundManager.PlaySound(_bMonsterAttacked ? "VOICE_UNIT_ACADEMY_C_TWINTAIL_BATTLE_DAMAGE_1" : "VOICE_UNIT_ACADEMY_C_TWINTAIL_BATTLE_DAMAGE_2", 1f, 0f, 0f);
		string text = (_bMonsterAttacked ? "DEATH2" : "DEATH1");
		m_spineSD?.SetAnimation(text, loop: false);
		float animationTime = m_spineSD.GetAnimationTime(text);
		yield return new WaitForSeconds(animationTime);
		UpdateState(GAME_STATE.RESULT);
		yield return null;
	}

	private void OnKillMonster(bool bBoss)
	{
		if (bBoss)
		{
			m_iBossMonsterKillCount++;
		}
		else
		{
			m_iNormalMonsterKillCount++;
		}
		UpdateScoreUI();
		if (!bBoss && m_iSpawnMonsterPerStage * (m_iCurrentStageLevel + 1) - m_iNormalMonsterKillCount == 0)
		{
			CreateMonster(bMidBoss: true);
		}
		else if (bBoss)
		{
			StartCoroutine(ClearStage());
		}
	}

	private IEnumerator ClearStage()
	{
		yield return new WaitForSeconds(0.8f);
		m_iCurrentStageLevel++;
		UpdateState(GAME_STATE.READY);
	}

	private void UpdateScoreUI()
	{
		NKCUtil.SetLabelText(m_lbScore, TotalKillCount.ToString());
	}

	private void UpdateGameResult()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		m_iBestGameScore = 0;
		if (nKMUserData != null)
		{
			NKMMiniGameData miniGameData = nKMUserData.GetMiniGameData(NKM_MINI_GAME_TYPE.SWORD_TRAINING, m_iMiniGameKey);
			if (miniGameData != null)
			{
				m_iBestGameScore = (int)miniGameData.score;
				if (m_iBestGameScore < Score3)
				{
					NKCPacketSender.Send_NKMPacket_MINI_GAME_RESULT_REQ(NKMMiniGameData.Create(m_iMiniGameKey, Score3, miniGameData.gameInfo));
					NKCPacketSender.Send_NKMPacket_MINI_GAME_INFO_REQ(NKM_MINI_GAME_TYPE.SWORD_TRAINING, m_iMiniGameKey);
				}
			}
			else if (Score3 > 0)
			{
				NKCPacketSender.Send_NKMPacket_MINI_GAME_RESULT_REQ(NKMMiniGameData.Create(m_iMiniGameKey, Score3));
			}
		}
		NKCSoundManager.PlayMusic("UI_WARFARE_RESULT_WIN", bLoop: true);
		Score1 = m_iNormalMonsterKillCount + m_iBossMonsterKillCount;
		NKCPopupSwordTrainingResult.Instance.Open(m_iBestGameScore, Score3, ReStart, ExitGame);
	}

	private void ReStart()
	{
		m_bJoinEffect = false;
		ClearData();
		UpdateState(GAME_STATE.READY);
	}

	private void ExitGame()
	{
		m_dOnGameEnd?.Invoke();
	}

	private void ClearData()
	{
		m_iCurrentStageLevel = 0;
		m_iNormalMonsterKillCount = 0;
		m_iBossMonsterKillCount = 0;
		m_iCurrentStageLevel = 0;
	}

	private IEnumerator CreateBGMonster()
	{
		while (m_iCurBackGroundMonsterCount < m_iCreateBackGroundMonsterMaxCount)
		{
			yield return new WaitForSeconds(m_fCreateBackGroundMonsterSpwanDelay);
			CreateBackMonster();
			m_iCurBackGroundMonsterCount++;
		}
		yield return null;
	}

	private IEnumerator TurnRandomBGMonster()
	{
		while (true)
		{
			yield return new WaitForSeconds(m_iBackGroundMonsterTurnTime);
			if (m_lstBackMonsters.Count > 0 && IsPlayableState)
			{
				int index = UnityEngine.Random.Range(0, m_lstBackMonsters.Count - 1);
				if (null != m_lstBackMonsters[index])
				{
					m_lstBackMonsters[index].MoveTurn();
				}
			}
		}
	}

	private void CreateBackMonster()
	{
		if (!IsPlayableState)
		{
			return;
		}
		bool flag = UnityEngine.Random.Range(1, 11) >= 5;
		bool flag2 = UnityEngine.Random.Range(1, 11) >= 5;
		string assetBundleName = (flag2 ? "AB_UNIT_EVENT_MONSTER_NORMAL" : "AB_UNIT_EVENT_MONSTER_YELLOW");
		string assetName = (flag2 ? "SPINE_NKM_EVENT_MONSTER_NORMAL" : "SPINE_NKM_EVENT_MONSTER_YELLOW");
		NKCASUIUnitIllust monster = GetMonster(assetBundleName, assetName, m_rtBackgroundMonsterParent);
		if (monster == null)
		{
			return;
		}
		RectTransform rectTransform = monster.GetRectTransform();
		if (!(null == rectTransform))
		{
			new System.Random().Next(0, 2);
			int increaseLevel = m_iCurrentStageLevel + 1;
			float monsterSpeed = GetMonsterSpeed(flag2, increaseLevel);
			NKCSwordTraningBackgroundMonster nKCSwordTraningBackgroundMonster = rectTransform.gameObject.GetComponent<NKCSwordTraningBackgroundMonster>();
			if (null == nKCSwordTraningBackgroundMonster)
			{
				nKCSwordTraningBackgroundMonster = rectTransform.gameObject.AddComponent<NKCSwordTraningBackgroundMonster>();
			}
			nKCSwordTraningBackgroundMonster.Init();
			nKCSwordTraningBackgroundMonster.SetData(monster, rectTransform, !flag, monsterSpeed, m_rtBackgroundMonsterSpawnPositionLeft, m_rtBackgroundMonsterSpawnPositionRight);
			m_lstBackMonsters.Add(nKCSwordTraningBackgroundMonster);
		}
	}

	private bool OpenSpineIllust(string assetbundleName, string assetName, ref NKCASUIUnitIllust SpineIllust, RectTransform parent)
	{
		SpineIllust = (NKCASUISpineIllust)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASUISpineIllust, assetbundleName, assetName);
		if (SpineIllust != null)
		{
			SpineIllust.SetParent(parent, worldPositionStays: false);
			RectTransform rectTransform = SpineIllust.GetRectTransform();
			if (rectTransform != null)
			{
				rectTransform.localPosition = Vector2.zero;
				rectTransform.localScale = Vector2.one;
				rectTransform.localRotation = Quaternion.identity;
			}
			return true;
		}
		Debug.LogError("spine data not found from : " + assetName);
		return false;
	}
}
