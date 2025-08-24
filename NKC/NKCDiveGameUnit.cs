using DG.Tweening;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCDiveGameUnit : MonoBehaviour
{
	public delegate void OnAniComplete();

	public Image m_NKM_UI_DIVE_PROCESS_UNIT_IMG;

	public Animator m_NKM_UI_DIVE_PROCESS_UNIT_SEARCH_FX;

	public NKCDiveGameUnitMover m_NKCDiveGameUnitMover;

	public Animator m_NKM_UI_DIVE_PROCESS_UNIT;

	private Sequence m_UnitDieSequence;

	private OnAniComplete m_AfterUnitDie;

	private OnAniComplete m_OnSpawnComplete;

	private NKCAssetInstanceData m_InstanceData;

	private const float UNIT_SHAKE_TIME = 2f;

	private const float UNIT_FADE_OUT_TIME = 0.6f;

	public static NKCDiveGameUnit GetNewInstance(Transform parent)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_WORLD_MAP_DIVE", "NKM_UI_DIVE_PROCESS_UNIT");
		NKCDiveGameUnit component = nKCAssetInstanceData.m_Instant.GetComponent<NKCDiveGameUnit>();
		if (component == null)
		{
			Debug.LogError("NKM_UI_DIVE_PROCESS_UNIT Prefab null!");
			return null;
		}
		component.m_InstanceData = nKCAssetInstanceData;
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.gameObject.SetActive(value: false);
		return component;
	}

	private void OnDestroy()
	{
		if (m_InstanceData != null)
		{
			NKCAssetResourceManager.CloseInstance(m_InstanceData);
		}
		m_InstanceData = null;
	}

	public void PlaySpawnAni(OnAniComplete _OnSpawnComplete = null)
	{
		m_OnSpawnComplete = _OnSpawnComplete;
		Color color = m_NKM_UI_DIVE_PROCESS_UNIT_IMG.color;
		m_NKM_UI_DIVE_PROCESS_UNIT_IMG.color = new Color(color.r, color.g, color.b, 1f);
		base.gameObject.transform.DOMove(new Vector3(-500f, 1000f, -1000f), 1.5f).SetEase(Ease.OutCubic).From(isRelative: true)
			.OnComplete(OnSpawnComplete);
	}

	public void ResetRotation()
	{
		m_NKM_UI_DIVE_PROCESS_UNIT_IMG.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
	}

	public void PlayDieAniExplosion(OnAniComplete _AfterUnitDie = null)
	{
		m_AfterUnitDie = _AfterUnitDie;
		m_NKM_UI_DIVE_PROCESS_UNIT_IMG.transform.DORotate(new Vector3(0f, 0f, 20f), 2f);
		if (m_UnitDieSequence != null)
		{
			m_UnitDieSequence.Kill();
		}
		m_UnitDieSequence = DOTween.Sequence();
		m_UnitDieSequence.Append(base.gameObject.transform.DOShakePosition(2f, 30f, 30));
		Color color = m_NKM_UI_DIVE_PROCESS_UNIT_IMG.color;
		m_UnitDieSequence.Append(m_NKM_UI_DIVE_PROCESS_UNIT_IMG.DOColor(new Color(color.r, color.g, color.b, 0f), 0.6f));
		m_UnitDieSequence.AppendCallback(AfterUnitDieImpl);
	}

	public void PlayDieAniWarp(OnAniComplete _AfterUnitDie = null)
	{
		m_AfterUnitDie = _AfterUnitDie;
		if (m_UnitDieSequence != null)
		{
			m_UnitDieSequence.Kill();
		}
		m_UnitDieSequence = DOTween.Sequence();
		m_UnitDieSequence.AppendInterval(2f);
		Color color = m_NKM_UI_DIVE_PROCESS_UNIT_IMG.color;
		m_UnitDieSequence.Append(m_NKM_UI_DIVE_PROCESS_UNIT_IMG.DOColor(new Color(color.r, color.g, color.b, 0f), 0.6f));
		m_UnitDieSequence.AppendCallback(AfterUnitDieImpl);
	}

	private void AfterUnitDieImpl()
	{
		if (m_AfterUnitDie != null)
		{
			m_AfterUnitDie();
			m_AfterUnitDie = null;
		}
	}

	private void OnSpawnComplete()
	{
		if (m_OnSpawnComplete != null)
		{
			m_OnSpawnComplete();
			m_OnSpawnComplete = null;
		}
	}

	public void Move(Vector3 _EndPos, float _fTrackingTime, NKCDiveGameUnitMover.OnCompleteMove _OnCompleteMove = null)
	{
		m_NKCDiveGameUnitMover.Move(_EndPos, _fTrackingTime, _OnCompleteMove);
	}

	public bool IsMoving()
	{
		if (m_NKCDiveGameUnitMover == null)
		{
			return false;
		}
		return m_NKCDiveGameUnitMover.IsRunning();
	}

	public void SetPause(bool bSet)
	{
		if (m_NKCDiveGameUnitMover != null)
		{
			m_NKCDiveGameUnitMover.SetPause(bSet);
		}
	}

	public void Clear()
	{
		if (m_NKCDiveGameUnitMover != null)
		{
			m_NKCDiveGameUnitMover.Stop();
		}
		base.gameObject.transform.DOKill();
		if (m_UnitDieSequence != null)
		{
			m_UnitDieSequence.Kill();
			m_UnitDieSequence = null;
		}
		m_AfterUnitDie = null;
		m_OnSpawnComplete = null;
	}

	public void PlaySearch()
	{
		if (m_NKM_UI_DIVE_PROCESS_UNIT_SEARCH_FX != null)
		{
			m_NKM_UI_DIVE_PROCESS_UNIT_SEARCH_FX.Play("NKM_UI_DIVE_PROCESS_UNIT_SEARCH_FX_BASE");
		}
	}

	public void SetUI(int unitID)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitID);
		if (unitTempletBase == null)
		{
			return;
		}
		Sprite orLoadMinimapFaceIcon = NKCResourceUtility.GetOrLoadMinimapFaceIcon(unitTempletBase.m_MiniMapFaceName);
		if (orLoadMinimapFaceIcon == null)
		{
			NKCAssetResourceData assetResourceUnitInvenIconEmpty = NKCResourceUtility.GetAssetResourceUnitInvenIconEmpty();
			if (assetResourceUnitInvenIconEmpty != null)
			{
				m_NKM_UI_DIVE_PROCESS_UNIT_IMG.sprite = assetResourceUnitInvenIconEmpty.GetAsset<Sprite>();
			}
			else
			{
				m_NKM_UI_DIVE_PROCESS_UNIT_IMG.sprite = null;
			}
		}
		else
		{
			m_NKM_UI_DIVE_PROCESS_UNIT_IMG.sprite = orLoadMinimapFaceIcon;
		}
		m_NKM_UI_DIVE_PROCESS_UNIT.Play("NKM_UI_DIVE_PROCESS_UNIT_CRUISER");
	}
}
