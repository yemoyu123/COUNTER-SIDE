using ClientPacket.Guild;
using NKM;
using NKM.Guild;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCUIGuildCoopBoss : MonoBehaviour
{
	public NKCUIComStateButton m_btn;

	public Text m_lbBossName;

	[Header("보스 공격 상태")]
	public GameObject m_objNone;

	public GameObject m_objAttackStop;

	public GameObject m_objAttackStart;

	[Header("보스 체력 게이지")]
	public Transform m_rtSDRoot;

	public Image m_imgBossHP;

	public Text m_lbBossHP;

	public GameObject m_objBossGauge;

	[Header("ExtraBoss")]
	public GameObject m_ObjExtraBossBubble;

	public GameObject m_ObjExtraBossTag;

	public Text m_lbExtraBossPoint;

	[Header("다른 유저가 전투중")]
	public GameObject m_objFight;

	public Text m_lbFightUserName;

	private NKCUIGuildCoopBack.OnClickBoss m_dOnClickBoss;

	private int m_BossStageId;

	private GuildRaidTemplet m_cGuildRaidTemplet;

	private NKCASUISpineIllust m_spineSD;

	private bool m_bIsExtraBoss;

	public void InitUI(NKCUIGuildCoopBack.OnClickBoss onClickBoss = null)
	{
		m_dOnClickBoss = onClickBoss;
		if (m_btn != null)
		{
			m_btn.PointerClick.RemoveAllListeners();
			m_btn.PointerClick.AddListener(OnClickBoss);
		}
	}

	public void OnDestroy()
	{
		CleanUpSpineSD();
	}

	public void SetData(GuildRaidTemplet templet, int lastStageIndex)
	{
		if (templet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		m_cGuildRaidTemplet = templet;
		m_BossStageId = templet.GetStageId();
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(m_BossStageId);
		if (dungeonTempletBase == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetLabelText(m_lbBossName, dungeonTempletBase.GetDungeonName());
		m_bIsExtraBoss = m_cGuildRaidTemplet.GetStageIndex() == lastStageIndex;
		NKCUtil.SetGameobjectActive(m_lbBossHP, !m_bIsExtraBoss);
		NKCUtil.SetGameobjectActive(m_lbExtraBossPoint, m_bIsExtraBoss);
		NKCUtil.SetGameobjectActive(m_ObjExtraBossBubble, m_bIsExtraBoss);
		NKCUtil.SetGameobjectActive(m_ObjExtraBossTag, m_bIsExtraBoss);
		NKCUtil.SetGameobjectActive(m_objBossGauge, !m_bIsExtraBoss);
		UpdateBossState();
		UpdateBossHP();
		UpdateBossPlayUser();
	}

	public void OnClickBoss()
	{
		if (m_BossStageId != 0)
		{
			m_dOnClickBoss?.Invoke(m_BossStageId);
		}
	}

	public void PlaySDAnim(NKCASUIUnitIllust.eAnimation eAnim, bool bLoop = false)
	{
		if (m_spineSD == null)
		{
			OpenSDIllust(eAnim, bLoop);
		}
		else if (m_spineSD != null)
		{
			m_spineSD.SetAnimation(eAnim, bLoop);
		}
	}

	public void CleanUpSpineSD()
	{
		if (m_spineSD != null)
		{
			NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_spineSD);
			m_spineSD = null;
		}
	}

	private bool OpenSDIllust(NKCASUIUnitIllust.eAnimation eStartAnim = NKCASUIUnitIllust.eAnimation.NONE, bool bLoopStartAnim = false)
	{
		NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_spineSD);
		m_spineSD = NKCResourceUtility.OpenSpineSD(m_cGuildRaidTemplet.GetRaidBossSDName());
		if (m_spineSD != null && (m_spineSD.m_SpineIllustInstant == null || m_spineSD.m_SpineIllustInstant_SkeletonGraphic == null))
		{
			m_spineSD = null;
		}
		if (m_spineSD != null)
		{
			if (eStartAnim == NKCASUIUnitIllust.eAnimation.NONE)
			{
				m_spineSD.SetDefaultAnimation(NKCASUIUnitIllust.eAnimation.SD_IDLE);
			}
			else
			{
				m_spineSD.SetDefaultAnimation(NKCASUIUnitIllust.eAnimation.SD_IDLE, bPlay: false);
				m_spineSD.SetAnimation(eStartAnim, bLoopStartAnim);
			}
			NKCUtil.SetGameobjectActive(m_rtSDRoot, bValue: true);
			m_spineSD.SetParent(m_rtSDRoot, worldPositionStays: false);
			RectTransform rectTransform = m_spineSD.GetRectTransform();
			if (rectTransform != null)
			{
				rectTransform.localPosition = Vector2.zero;
				rectTransform.localScale = Vector3.one;
				rectTransform.localRotation = Quaternion.identity;
			}
			return true;
		}
		Debug.Log("spine SD data not found from m_cGuildRaidTemplet.GetRaidBossSDName() : " + m_cGuildRaidTemplet.GetRaidBossSDName());
		NKCUtil.SetGameobjectActive(m_rtSDRoot, bValue: false);
		return false;
	}

	private void UpdateBossHP()
	{
		float num = NKCGuildCoopManager.m_BossData.remainHp / NKCGuildCoopManager.m_BossMaxHp;
		NKCUtil.SetLabelText(m_lbBossHP, string.Format("{0} ({1:0.##}%)", NKCGuildCoopManager.m_BossData.remainHp.ToString("N0"), num * 100f));
		NKCUtil.SetLabelText(m_lbExtraBossPoint, $"{NKCGuildCoopManager.m_BossData.extraPoint}");
		m_imgBossHP.fillAmount = num;
	}

	private void UpdateBossPlayUser()
	{
		if (NKCGuildCoopManager.m_BossData.playUserUid == 0L)
		{
			NKCUtil.SetGameobjectActive(m_objFight, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_objFight, bValue: true);
		GuildDungeonMemberInfo guildDungeonMemberInfo = NKCGuildCoopManager.GetGuildDungeonMemberInfo().Find((GuildDungeonMemberInfo x) => x.profile.userUid == NKCGuildCoopManager.m_BossData.playUserUid);
		NKCUtil.SetLabelText(m_lbFightUserName, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_SESSION_ATTACK_ING_USER_INFO, guildDungeonMemberInfo.profile.nickname));
	}

	public void UpdateBossState()
	{
		if (NKCGuildCoopManager.m_BossData.remainHp <= 0f)
		{
			PlaySDAnim(NKCASUIUnitIllust.eAnimation.SD_DOWN, bLoop: true);
		}
		else
		{
			PlaySDAnim(NKCASUIUnitIllust.eAnimation.SD_IDLE, bLoop: true);
		}
		NKCUtil.SetGameobjectActive(m_objNone, !m_bIsExtraBoss && NKCGuildCoopManager.BossOrderIndex == NKCGuildCoopManager.BOSS_ORDER_TYPE.NONE);
		NKCUtil.SetGameobjectActive(m_objAttackStop, !m_bIsExtraBoss && NKCGuildCoopManager.BossOrderIndex == NKCGuildCoopManager.BOSS_ORDER_TYPE.STOP);
		NKCUtil.SetGameobjectActive(m_objAttackStart, !m_bIsExtraBoss && NKCGuildCoopManager.BossOrderIndex == NKCGuildCoopManager.BOSS_ORDER_TYPE.START);
	}

	public void Refresh()
	{
		UpdateBossState();
		UpdateBossHP();
		UpdateBossPlayUser();
	}
}
