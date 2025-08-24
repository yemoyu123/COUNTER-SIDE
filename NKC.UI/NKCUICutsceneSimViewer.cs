using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUICutsceneSimViewer : NKCUIBase
{
	public Dropdown m_ddCutScenList;

	public Dropdown m_ddLanguageList;

	public Dictionary<string, NKM_NATIONAL_CODE> m_dicNationalCode = new Dictionary<string, NKM_NATIONAL_CODE>();

	public List<string> m_CurrentTempletList = new List<string>();

	public CanvasGroup m_CanvasGroup;

	public Toggle m_tglFilterMain;

	public Toggle m_tglFilterSide;

	public Toggle m_tglFilterCounterCase;

	public Toggle m_tglFilterEndLifeTimeContract;

	public Toggle m_tglFilterFreeContract;

	public Toggle m_tglFilterFirstCome;

	public Toggle m_tglFilterEvent;

	public Toggle m_tglFilterEtc;

	public InputField m_strIdFilter;

	public NKCUIComButton m_comBtnVaildateListString;

	public NKCUIComButton m_comBtnExportCutscenIndex;

	private NKCUICutScenPlayer m_NKCUICutScenPlayer;

	private NKM_NATIONAL_CODE m_CurrLanguage;

	public override string MenuName => "컷신 시뮬레이터";

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	public override void CloseInternal()
	{
		if ((bool)m_NKCUICutScenPlayer)
		{
			m_NKCUICutScenPlayer.UnLoad();
			m_NKCUICutScenPlayer.StopWithCallBack();
		}
	}

	public override void OnBackButton()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_LOGIN);
	}

	public void Open()
	{
		m_NKCUICutScenPlayer = NKCUICutScenPlayer.Instance;
		UIOpened();
		base.gameObject.SetActive(value: true);
	}

	private void SetCutScenList()
	{
	}

	private void SetStrIDFilterCallBack()
	{
		if (m_strIdFilter != null)
		{
			m_strIdFilter.onEndEdit.RemoveAllListeners();
			m_strIdFilter.onEndEdit.AddListener(delegate
			{
				SetCutScenList();
			});
		}
	}

	public static NKCUICutsceneSimViewer InitUI()
	{
		NKCUICutsceneSimViewer nKCUICutsceneSimViewer = NKCUIManager.OpenUI<NKCUICutsceneSimViewer>("NKM_CUTSCEN_SIM_Panel");
		if (nKCUICutsceneSimViewer != null)
		{
			nKCUICutsceneSimViewer.gameObject.SetActive(value: false);
			nKCUICutsceneSimViewer.m_tglFilterMain.onValueChanged.RemoveAllListeners();
			nKCUICutsceneSimViewer.m_tglFilterMain.onValueChanged.AddListener(nKCUICutsceneSimViewer.OnValueChangedFilter);
			nKCUICutsceneSimViewer.m_tglFilterSide.onValueChanged.RemoveAllListeners();
			nKCUICutsceneSimViewer.m_tglFilterSide.onValueChanged.AddListener(nKCUICutsceneSimViewer.OnValueChangedFilter);
			nKCUICutsceneSimViewer.m_tglFilterCounterCase.onValueChanged.RemoveAllListeners();
			nKCUICutsceneSimViewer.m_tglFilterCounterCase.onValueChanged.AddListener(nKCUICutsceneSimViewer.OnValueChangedFilter);
			nKCUICutsceneSimViewer.m_tglFilterEndLifeTimeContract.onValueChanged.RemoveAllListeners();
			nKCUICutsceneSimViewer.m_tglFilterEndLifeTimeContract.onValueChanged.AddListener(nKCUICutsceneSimViewer.OnValueChangedFilter);
			nKCUICutsceneSimViewer.m_tglFilterFreeContract.onValueChanged.RemoveAllListeners();
			nKCUICutsceneSimViewer.m_tglFilterFreeContract.onValueChanged.AddListener(nKCUICutsceneSimViewer.OnValueChangedFilter);
			nKCUICutsceneSimViewer.m_tglFilterFirstCome.onValueChanged.RemoveAllListeners();
			nKCUICutsceneSimViewer.m_tglFilterFirstCome.onValueChanged.AddListener(nKCUICutsceneSimViewer.OnValueChangedFilter);
			nKCUICutsceneSimViewer.m_tglFilterEvent.onValueChanged.RemoveAllListeners();
			nKCUICutsceneSimViewer.m_tglFilterEvent.onValueChanged.AddListener(nKCUICutsceneSimViewer.OnValueChangedFilter);
			nKCUICutsceneSimViewer.m_tglFilterEtc.onValueChanged.RemoveAllListeners();
			nKCUICutsceneSimViewer.m_tglFilterEtc.onValueChanged.AddListener(nKCUICutsceneSimViewer.OnValueChangedFilter);
			nKCUICutsceneSimViewer.m_comBtnVaildateListString.PointerClick.RemoveAllListeners();
			nKCUICutsceneSimViewer.m_comBtnVaildateListString.PointerClick.AddListener(nKCUICutsceneSimViewer.OnClickedValidateBtn);
			NKCUtil.SetButtonClickDelegate(nKCUICutsceneSimViewer.m_comBtnExportCutscenIndex, nKCUICutsceneSimViewer.ExportCutscenIndex);
			foreach (NKM_NATIONAL_CODE value in Enum.GetValues(typeof(NKM_NATIONAL_CODE)))
			{
				if (!string.IsNullOrEmpty(NKCStringTable.GetNationalPostfix(value)))
				{
					string nationalPostfix = NKCStringTable.GetNationalPostfix(value);
					switch (nationalPostfix)
					{
					case "_KOREA":
					case "_JPN":
					case "_ENG":
					case "_TWN":
					case "_THA":
					case "_VTN":
					case "_SCN":
						nKCUICutsceneSimViewer.m_dicNationalCode[nationalPostfix] = value;
						break;
					}
				}
			}
			List<string> list = nKCUICutsceneSimViewer.m_dicNationalCode.Keys.ToList();
			list.Insert(0, "_KOREA");
			nKCUICutsceneSimViewer.m_ddLanguageList.AddOptions(list);
			nKCUICutsceneSimViewer.SetStrIDFilterCallBack();
		}
		else
		{
			UnityEngine.Debug.LogError("NKM_CUTSCEN_SIM_Panel is null");
		}
		return nKCUICutsceneSimViewer;
	}

	public void OnClickedValidateBtn()
	{
		ValidateTempletList();
	}

	public void ExportCutscenIndex()
	{
		NKCCollectionManager.Init();
		string text = "CutscenIndex.txt";
		FileStream stream;
		if (!File.Exists(text))
		{
			stream = new FileStream(text, FileMode.Create, FileAccess.Write, FileShare.Read);
		}
		else
		{
			FileInfo fileInfo = new FileInfo(text);
			if (fileInfo.IsReadOnly)
			{
				fileInfo.Attributes = FileAttributes.Normal;
			}
			stream = File.Open(text, FileMode.Truncate, FileAccess.Write, FileShare.Read);
		}
		StreamWriter streamWriter = new StreamWriter(stream);
		foreach (KeyValuePair<int, storyUnlockData> storyDatum in NKCCollectionManager.GetStoryData())
		{
			storyUnlockData value = storyDatum.Value;
			NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(value.m_StageID);
			if (nKMStageTempletV != null)
			{
				string stageBeforeCutscenStrID = nKMStageTempletV.GetStageBeforeCutscenStrID();
				string stageAfterCutscenStrID = nKMStageTempletV.GetStageAfterCutscenStrID();
				int num = 1;
				if (!string.IsNullOrEmpty(stageBeforeCutscenStrID))
				{
					streamWriter.Write(stageBeforeCutscenStrID);
					streamWriter.Write('\t');
					streamWriter.Write(value.m_EpisodeCategory.ToDescription());
					streamWriter.Write('\t');
					streamWriter.Write(nKMStageTempletV.EpisodeTemplet.GetEpisodeName());
					streamWriter.Write('\t');
					streamWriter.Write(nKMStageTempletV.GetDungeonName());
					streamWriter.Write($" Part {num}\t");
					streamWriter.Write('\n');
					num++;
				}
				if (!string.IsNullOrEmpty(stageAfterCutscenStrID))
				{
					streamWriter.Write(stageAfterCutscenStrID);
					streamWriter.Write('\t');
					streamWriter.Write(value.m_EpisodeCategory.ToDescription());
					streamWriter.Write('\t');
					streamWriter.Write(nKMStageTempletV.EpisodeTemplet.GetEpisodeName());
					streamWriter.Write('\t');
					streamWriter.Write(nKMStageTempletV.GetDungeonName());
					streamWriter.Write($" Part {num}\t");
					streamWriter.Write('\n');
					num++;
				}
			}
		}
		streamWriter.Close();
		NKCPopupOKCancel.OpenOKBox("앗싸", "CutscenIndex.txt에 Export 완료");
	}

	public string GetStageBeforeCutscen(NKMStageTempletV2 stageTemplet)
	{
		if (stageTemplet == null)
		{
			return null;
		}
		switch (stageTemplet.m_STAGE_TYPE)
		{
		case STAGE_TYPE.ST_DUNGEON:
			if (stageTemplet.DungeonTempletBase != null)
			{
				return stageTemplet.DungeonTempletBase.m_CutScenStrIDBefore;
			}
			break;
		case STAGE_TYPE.ST_WARFARE:
			if (stageTemplet.WarfareTemplet != null)
			{
				return stageTemplet.WarfareTemplet.m_CutScenStrIDBefore;
			}
			break;
		case STAGE_TYPE.ST_PHASE:
			if (stageTemplet.PhaseTemplet != null)
			{
				return stageTemplet.PhaseTemplet.m_CutScenStrIDBefore;
			}
			break;
		}
		return null;
	}

	public string GetStageAfterCutscen(NKMStageTempletV2 stageTemplet)
	{
		if (stageTemplet == null)
		{
			return null;
		}
		switch (stageTemplet.m_STAGE_TYPE)
		{
		case STAGE_TYPE.ST_DUNGEON:
			if (stageTemplet.DungeonTempletBase != null)
			{
				return stageTemplet.DungeonTempletBase.m_CutScenStrIDAfter;
			}
			break;
		case STAGE_TYPE.ST_WARFARE:
			if (stageTemplet.WarfareTemplet != null)
			{
				return stageTemplet.WarfareTemplet.m_CutScenStrIDAfter;
			}
			break;
		case STAGE_TYPE.ST_PHASE:
			if (stageTemplet.PhaseTemplet != null)
			{
				return stageTemplet.PhaseTemplet.m_CutScenStrIDAfter;
			}
			break;
		}
		return null;
	}

	public void OnValueChangedFilter(bool bSet)
	{
	}

	public void OnValueChangedLanguage(int index)
	{
		string text = m_ddLanguageList.options[index].text;
		m_CurrLanguage = m_dicNationalCode[text];
		OnClickReload();
	}

	public void OnValueChanged(int index)
	{
		if (index == 0)
		{
			if ((bool)m_NKCUICutScenPlayer)
			{
				m_NKCUICutScenPlayer.StopWithCallBack();
			}
		}
		else
		{
			string text = m_ddCutScenList.options[index].text;
			PlayCutScen(text);
		}
	}

	public void OnClickPlay()
	{
	}

	public void OnClickReload()
	{
		if (m_NKCUICutScenPlayer.IsPlaying())
		{
			NKCPopupOKCancel.OpenOKBox("에러", "플레이 중에는 리로딩 불가능해요.");
			return;
		}
		if (NKCStringTable.GetNationalCode() != m_CurrLanguage)
		{
			NKCStringTable.SetNationalCode(m_CurrLanguage);
		}
		NKCCutScenManager.Init();
	}

	private void PlayCutScen(string strID)
	{
		if ((bool)m_NKCUICutScenPlayer)
		{
			m_NKCUICutScenPlayer.UnLoad();
			m_NKCUICutScenPlayer.Load(strID, bPreLoad: false);
			m_NKCUICutScenPlayer.StopWithCallBack();
			m_NKCUICutScenPlayer.Play(strID, 0, delegate
			{
				m_CanvasGroup.alpha = 1f;
				m_CanvasGroup.interactable = true;
			});
			m_CanvasGroup.alpha = 0f;
			m_CanvasGroup.interactable = false;
		}
	}

	private void ValidateTempletList()
	{
		m_NKCUICutScenPlayer.UnLoad();
		UnityEngine.Debug.Log("Validation Start");
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		try
		{
			for (int i = 1; i < m_CurrentTempletList.Count; i++)
			{
				ValidateTemplet(m_CurrentTempletList[i]);
			}
		}
		catch (Exception arg)
		{
			UnityEngine.Debug.Log($"Exception occurred. [{arg}]");
		}
		finally
		{
			m_CanvasGroup.alpha = 1f;
			m_CanvasGroup.interactable = true;
			stopwatch.Stop();
			UnityEngine.Debug.Log("Validation End");
			UnityEngine.Debug.Log($"Validated Templet Count : {m_CurrentTempletList.Count}");
			UnityEngine.Debug.Log($"Validation Time : {stopwatch.ElapsedMilliseconds} ms");
		}
	}

	private void ValidateTemplet(string strID)
	{
		m_NKCUICutScenPlayer.Load(strID, bPreLoad: false);
		m_NKCUICutScenPlayer.Play(strID, 0);
		m_NKCUICutScenPlayer.ValidateBySimulate();
	}
}
