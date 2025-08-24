using UnityEngine;

namespace NKC;

public class NKCShipSkillMark
{
	private NKCAssetInstanceData m_NKM_SHIP_SKILL_MARK;

	private NKCAssetInstanceData m_NKM_SHIP_SKILL_MARK_LAND;

	private Vector3 m_Vec3Temp;

	private SpriteRenderer m_SrLand;

	private Color m_DefaultColor = new Color(0.897f, 0.1846f, 0.1846f, 0.5098f);

	public void Init()
	{
		if (m_NKM_SHIP_SKILL_MARK == null && NKCScenManager.GetScenManager().Get_SCEN_GAME() != null && NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
			.Get_GAME_BATTLE_MAP() != null)
		{
			m_NKM_SHIP_SKILL_MARK = NKCAssetResourceManager.OpenInstance<GameObject>("AB_FX_UI_SHIP_SKILL_TARGET", "AB_FX_UI_SHIP_SKILL_TARGET_CENTER");
			m_NKM_SHIP_SKILL_MARK_LAND = NKCAssetResourceManager.OpenInstance<GameObject>("AB_FX_UI_SHIP_SKILL_TARGET", "AB_FX_UI_SHIP_SKILL_TARGET_LAND");
			m_NKM_SHIP_SKILL_MARK.m_Instant.transform.SetParent(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
				.Get_GAME_BATTLE_MAP()
				.transform);
				m_NKM_SHIP_SKILL_MARK_LAND.m_Instant.transform.SetParent(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
					.Get_GAME_BATTLE_MAP()
					.transform);
					m_SrLand = m_NKM_SHIP_SKILL_MARK_LAND.m_Instant.GetComponentInChildren<SpriteRenderer>();
					if (m_SrLand != null)
					{
						m_DefaultColor = m_SrLand.color;
					}
				}
				SetShow(bShow: false);
			}

			public void SetShow(bool bShow)
			{
				if (m_NKM_SHIP_SKILL_MARK == null || m_NKM_SHIP_SKILL_MARK_LAND == null)
				{
					return;
				}
				if (bShow)
				{
					if (!m_NKM_SHIP_SKILL_MARK.m_Instant.activeSelf)
					{
						m_NKM_SHIP_SKILL_MARK.m_Instant.SetActive(value: true);
					}
					if (!m_NKM_SHIP_SKILL_MARK_LAND.m_Instant.activeSelf)
					{
						m_NKM_SHIP_SKILL_MARK_LAND.m_Instant.SetActive(value: true);
					}
				}
				else
				{
					if (m_NKM_SHIP_SKILL_MARK.m_Instant.activeSelf)
					{
						m_NKM_SHIP_SKILL_MARK.m_Instant.SetActive(value: false);
					}
					if (m_NKM_SHIP_SKILL_MARK_LAND.m_Instant.activeSelf)
					{
						m_NKM_SHIP_SKILL_MARK_LAND.m_Instant.SetActive(value: false);
					}
				}
			}

			public void SetPos(float fX, float fZ)
			{
				m_Vec3Temp = m_NKM_SHIP_SKILL_MARK.m_Instant.transform.localPosition;
				m_Vec3Temp.x = fX;
				m_Vec3Temp.y = fZ;
				m_NKM_SHIP_SKILL_MARK.m_Instant.transform.localPosition = m_Vec3Temp;
				m_Vec3Temp = m_NKM_SHIP_SKILL_MARK_LAND.m_Instant.transform.localPosition;
				m_Vec3Temp.x = fX;
				m_Vec3Temp.y = fZ;
				m_NKM_SHIP_SKILL_MARK_LAND.m_Instant.transform.localPosition = m_Vec3Temp;
			}

			public void SetScale(float fX, float fY)
			{
				m_Vec3Temp.x = fX;
				m_Vec3Temp.y = fY;
				m_Vec3Temp.z = 1f;
				if (m_SrLand != null)
				{
					m_SrLand.size = new Vector2(fX, fY);
				}
				else
				{
					m_NKM_SHIP_SKILL_MARK_LAND.m_Instant.transform.localScale = m_Vec3Temp;
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

			public void SetColor(Color col)
			{
				if (m_SrLand != null)
				{
					m_SrLand.color = col;
				}
			}
		}
