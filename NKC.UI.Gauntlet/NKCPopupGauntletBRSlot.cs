using System;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCPopupGauntletBRSlot : MonoBehaviour
{
	public delegate void OnClick(int index);

	private OnClick dOnClick;

	public Text m_lbGameResult;

	public Image m_imgGameResult;

	public Image m_imgModeBG;

	public Text m_lbMode;

	public Text m_lbLevel;

	public Text m_lbUserNickName;

	public Text m_lbDate;

	public Text m_lbAddScore;

	public NKCUIComToggle m_ctglBRSlot;

	private int m_Index;

	private NKCAssetInstanceData m_InstanceData;

	public int GetIndex()
	{
		return m_Index;
	}

	public static NKCPopupGauntletBRSlot GetNewInstance(Transform parent, OnClick _onClick)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_POPUP_RECORD_SLOT");
		NKCPopupGauntletBRSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCPopupGauntletBRSlot>();
		if (component == null)
		{
			Debug.LogError("NKCPopupGauntletBRSlot Prefab null!");
			return null;
		}
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.m_InstanceData = nKCAssetInstanceData;
		component.transform.localPosition = new Vector3(component.transform.localPosition.x, component.transform.localPosition.y, 0f);
		component.dOnClick = _onClick;
		component.gameObject.SetActive(value: false);
		component.m_ctglBRSlot.OnValueChanged.RemoveAllListeners();
		component.m_ctglBRSlot.OnValueChanged.AddListener(component.OnClicked);
		return component;
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnClicked(bool bSet)
	{
		if (bSet && dOnClick != null)
		{
			dOnClick(m_Index);
		}
	}

	public void SetUI(PvpSingleHistory cPvpSingleHistory, int index)
	{
		if (cPvpSingleHistory != null)
		{
			m_Index = index;
			if (cPvpSingleHistory.Result == PVP_RESULT.WIN)
			{
				m_lbGameResult.text = NKCUtilString.GET_STRING_WIN;
				m_lbGameResult.color = NKCUtil.GetColor("#FFDF5D");
				m_imgGameResult.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_GAUNTLET_SPRITE", "AB_UI_NKM_UI_GAUNTLET_RESULTSMALL_WIN");
			}
			else if (cPvpSingleHistory.Result == PVP_RESULT.LOSE)
			{
				m_lbGameResult.text = NKCUtilString.GET_STRING_LOSE;
				m_lbGameResult.color = NKCUtil.GetColor("#FF2626");
				m_imgGameResult.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_GAUNTLET_SPRITE", "AB_UI_NKM_UI_GAUNTLET_RESULTSMALL_LOSE");
			}
			else
			{
				m_lbGameResult.text = NKCUtilString.GET_STRING_DRAW;
				m_lbGameResult.color = NKCUtil.GetColor("#D4D4D4");
				m_imgGameResult.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_GAUNTLET_SPRITE", "AB_UI_NKM_UI_GAUNTLET_RESULTSMALL_DRAW");
			}
			string assetName = "";
			string msg = "";
			bool bValue = false;
			switch (cPvpSingleHistory.GameType)
			{
			case NKM_GAME_TYPE.NGT_PVP_RANK:
				assetName = "AB_UI_NKM_UI_GAUNTLET_ELLIPSE_RANK";
				msg = NKCUtilString.GET_STRING_GAUNTLET_RANK_GAME;
				bValue = true;
				break;
			case NKM_GAME_TYPE.NGT_ASYNC_PVP:
			case NKM_GAME_TYPE.NGT_PVP_STRATEGY:
			case NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE:
			case NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC:
				assetName = "AB_UI_NKM_UI_GAUNTLET_ELLIPSE_ASYNCMAYCH";
				msg = NKCUtilString.GET_STRING_GAUNTLET_ASYNC_GAME;
				bValue = true;
				break;
			case NKM_GAME_TYPE.NGT_PVP_LEAGUE:
				assetName = "AB_UI_NKM_UI_GAUNTLET_ELLIPSE_LEAGUE";
				msg = NKCUtilString.GET_STRING_GAUNTLET_LEAGUE_TITLE;
				bValue = true;
				break;
			case NKM_GAME_TYPE.NGT_PVP_UNLIMITED:
				assetName = "AB_UI_NKM_UI_GAUNTLET_ELLIPSE_LEAGUE";
				msg = NKCUtilString.GET_STRING_GAUNTLET_UNLIMITED_TITLE;
				bValue = true;
				break;
			case NKM_GAME_TYPE.NGT_PVP_PRIVATE:
				assetName = "AB_UI_NKM_UI_GAUNTLET_ELLIPSE_NORMAL";
				msg = NKCUtilString.GET_STRING_PRIVATE_PVP;
				bValue = false;
				break;
			case NKM_GAME_TYPE.NGT_PVP_EVENT:
				assetName = "AB_UI_NKM_UI_GAUNTLET_ELLIPSE_NORMAL";
				msg = NKCUtilString.GET_STRING_GAUNTLET_EVENT_GAME;
				bValue = false;
				break;
			default:
				Debug.LogError("NKCPopupGauntletBRSlot.SetUI - Not Set Game Type " + cPvpSingleHistory.GameType);
				break;
			}
			NKCUtil.SetImageSprite(m_imgModeBG, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_GAUNTLET_SPRITE", assetName), bDisableIfSpriteNull: true);
			NKCUtil.SetLabelTextColor(m_lbMode, NKCUtil.GetColor("#FFFFFFFF"));
			NKCUtil.SetLabelText(m_lbMode, msg);
			NKCUtil.SetGameobjectActive(m_lbAddScore, bValue);
			m_lbLevel.text = string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, cPvpSingleHistory.TargetUserLevel.ToString());
			m_lbUserNickName.text = NKCUtilString.GetUserNickname(cPvpSingleHistory.TargetNickName, bOpponent: true);
			DateTime dateTime = new DateTime(cPvpSingleHistory.RegdateTick).ToLocalTime();
			m_lbDate.text = string.Format(NKCUtilString.GET_STRING_DATE_FOUR_PARAM, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute.ToString("#00"));
			if (cPvpSingleHistory.GainScore > 0)
			{
				m_lbAddScore.text = "+" + cPvpSingleHistory.GainScore;
				m_lbAddScore.color = NKCUtil.GetColor("#FFDF5D");
			}
			else if (cPvpSingleHistory.GainScore < 0)
			{
				m_lbAddScore.text = cPvpSingleHistory.GainScore.ToString();
				m_lbAddScore.color = NKCUtil.GetColor("#FF4747");
			}
			else
			{
				m_lbAddScore.text = cPvpSingleHistory.GainScore.ToString();
				m_lbAddScore.color = NKCUtil.GetColor("#C1C1C1");
			}
		}
	}
}
