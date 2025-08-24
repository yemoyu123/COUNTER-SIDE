using ClientPacket.Office;
using NKM.Templet.Office;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Component.Office;

public class NKCUIComOfficeEnvScore : MonoBehaviour
{
	public Text m_lbEnvScore;

	public Text m_lbEnvInformation;

	public Image[] m_GradeStep;

	public string m_offIconName;

	public string m_onIconName;

	private const string m_strSpriteBundleName = "ab_ui_office_sprite";

	public void UpdateEnvScore(NKMOfficeRoom room)
	{
		if (room == null)
		{
			NKCUtil.SetLabelText(m_lbEnvScore, "-");
			NKCUtil.SetLabelText(m_lbEnvInformation, "");
			return;
		}
		NKCUtil.SetLabelText(m_lbEnvScore, room.interiorScore.ToString());
		NKMOfficeGradeTemplet nKMOfficeGradeTemplet = NKMOfficeGradeTemplet.Find(room.grade);
		if (nKMOfficeGradeTemplet != null)
		{
			string msg = NKCStringTable.GetString("SI_DP_OFFICE_LOYALTY_SPEED", nKMOfficeGradeTemplet.ChargingTimeHour);
			NKCUtil.SetLabelText(m_lbEnvInformation, msg);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbEnvInformation, "");
		}
		if (m_GradeStep != null)
		{
			int grade = (int)room.grade;
			int num = m_GradeStep.Length;
			for (int i = 0; i < num; i++)
			{
				Sprite sprite = null;
				NKCUtil.SetImageSprite(sp: (i > grade) ? NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_office_sprite", m_offIconName) : NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_office_sprite", m_onIconName), image: m_GradeStep[i], bDisableIfSpriteNull: true);
			}
		}
	}
}
