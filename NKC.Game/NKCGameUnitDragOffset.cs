using UnityEngine;

namespace NKC.Game;

public class NKCGameUnitDragOffset : MonoBehaviour
{
	public SpriteRenderer m_srSpawnOffset;

	public GameObject m_objTargetMarker;

	public NKCAssetInstanceData m_NKCAssetInstanceData;

	public static NKCGameUnitDragOffset GetInstance()
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_FX_UI_SHIP_SKILL_TARGET", "AB_FX_UI_UNIT_SPAWN_OFFSET");
		NKCGameUnitDragOffset nKCGameUnitDragOffset = nKCAssetInstanceData?.m_Instant?.GetComponent<NKCGameUnitDragOffset>();
		if (nKCGameUnitDragOffset == null)
		{
			Debug.LogError("NKCGameUnitDragOffset instance creation failed!");
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			return null;
		}
		nKCGameUnitDragOffset.m_NKCAssetInstanceData = nKCAssetInstanceData;
		return nKCGameUnitDragOffset;
	}

	public void Init()
	{
		if (NKCScenManager.GetScenManager().Get_SCEN_GAME() != null && NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
			.Get_GAME_BATTLE_MAP() != null)
		{
			base.transform.SetParent(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
				.Get_GAME_BATTLE_MAP()
				.transform);
			}
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		}

		public void SetOffset(float fX, float fZ, float targetOffsetPos)
		{
			if (!(m_srSpawnOffset == null) && base.gameObject.activeInHierarchy)
			{
				m_srSpawnOffset.flipX = targetOffsetPos < fX;
				float num = targetOffsetPos - fX;
				m_srSpawnOffset.size = new Vector2(num, m_srSpawnOffset.size.y);
				base.transform.localPosition = new Vector3((fX + targetOffsetPos) * 0.5f, fZ, base.transform.localPosition.z);
				if (m_objTargetMarker != null)
				{
					m_objTargetMarker.transform.localPosition = new Vector3(num * 0.5f, 0f, 0f);
				}
			}
		}

		private void OnDestroy()
		{
			NKCAssetResourceManager.CloseInstance(m_NKCAssetInstanceData);
		}
	}
