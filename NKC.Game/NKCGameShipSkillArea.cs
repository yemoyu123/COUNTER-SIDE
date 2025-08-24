using UnityEngine;

namespace NKC.Game;

public class NKCGameShipSkillArea : MonoBehaviour
{
	public GameObject m_objCenterShip;

	public GameObject m_objCenterUnit;

	public SpriteRenderer m_SrLand;

	private NKCAssetInstanceData m_NKCAssetInstanceData;

	private Color m_DefaultColor = new Color(0.897f, 0.1846f, 0.1846f, 0.5098f);

	public static NKCGameShipSkillArea GetInstance()
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_FX_UI_SHIP_SKILL_TARGET", "AB_FX_UI_SHIP_SKILL_TARGET_AREA");
		NKCGameShipSkillArea nKCGameShipSkillArea = nKCAssetInstanceData?.m_Instant?.GetComponent<NKCGameShipSkillArea>();
		if (nKCGameShipSkillArea == null)
		{
			Debug.LogError("NKCGameShipSkillArea instance creation failed!");
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			return null;
		}
		nKCGameShipSkillArea.m_NKCAssetInstanceData = nKCAssetInstanceData;
		return nKCGameShipSkillArea;
	}

	public void Init()
	{
		if (NKCScenManager.GetScenManager().Get_SCEN_GAME() != null && NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
			.Get_GAME_BATTLE_MAP() != null)
		{
			base.transform.SetParent(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
				.Get_GAME_BATTLE_MAP()
				.transform);
				base.transform.localPosition = Vector3.zero;
				base.transform.localScale = Vector3.one;
				base.transform.localRotation = Quaternion.identity;
				if (m_SrLand != null)
				{
					m_DefaultColor = m_SrLand.color;
				}
			}
			SetShow(bShow: false);
		}

		public void SetCenterShip()
		{
			NKCUtil.SetGameobjectActive(m_objCenterUnit, bValue: false);
			NKCUtil.SetGameobjectActive(m_objCenterShip, bValue: true);
		}

		public void SetCenterUnit()
		{
			NKCUtil.SetGameobjectActive(m_objCenterUnit, bValue: true);
			NKCUtil.SetGameobjectActive(m_objCenterShip, bValue: false);
		}

		public void SetShow(bool bShow)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bShow);
		}

		public void SetPos(float fX, float fZ)
		{
			base.transform.localPosition = new Vector3(fX, fZ, 0f);
		}

		public void SetScale(float fX, float fY)
		{
			if (m_SrLand != null)
			{
				m_SrLand.size = new Vector2(fX, fY);
			}
		}

		public void SetDefaultColor()
		{
			SetColor(m_DefaultColor);
		}

		public void SetColor(float r, float g, float b)
		{
			Color color = new Color(r, g, b, m_DefaultColor.a);
			SetColor(color);
		}

		public void SetColor(float r, float g, float b, float a)
		{
			if (a < 0f)
			{
				SetColor(r, g, b);
				return;
			}
			Color color = new Color(r, g, b, a);
			SetColor(color);
		}

		public void SetColor(Color col)
		{
			if (m_SrLand != null)
			{
				m_SrLand.color = col;
			}
		}

		private void OnDestroy()
		{
			NKCAssetResourceManager.CloseInstance(m_NKCAssetInstanceData);
		}
	}
