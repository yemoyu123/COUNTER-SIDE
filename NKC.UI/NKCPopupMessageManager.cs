using System.Collections.Generic;
using NKC.PacketHandler;
using NKC.Publisher;
using NKC.UI.Contract;
using NKC.UI.Result;
using NKM;

namespace NKC.UI;

internal static class NKCPopupMessageManager
{
	private static LinkedList<PopupMessage> s_llMessage = new LinkedList<PopupMessage>();

	public static void AddPopupMessage(string message, NKCPopupMessage.eMessagePosition position = NKCPopupMessage.eMessagePosition.Top, bool bShowFX = false, bool bPreemptive = true, float delayTime = 0f, bool bWaitForGameEnd = false)
	{
		PopupMessage value = new PopupMessage(message, position, delayTime, bPreemptive, bShowFX, bWaitForGameEnd);
		s_llMessage.AddLast(value);
	}

	public static void AddPopupMessage(NKM_ERROR_CODE errorCode, NKCPopupMessage.eMessagePosition position = NKCPopupMessage.eMessagePosition.Top, bool bShowFX = false, bool bPreemptive = true, float delayTime = 0f, bool bWaitForGameEnd = false)
	{
		PopupMessage value = new PopupMessage(NKCPacketHandlers.GetErrorMessage(errorCode), position, delayTime, bPreemptive, bShowFX, bWaitForGameEnd);
		s_llMessage.AddLast(value);
	}

	public static void AddPopupMessage(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalError, NKCPopupMessage.eMessagePosition position = NKCPopupMessage.eMessagePosition.Top, bool bShowFX = false, bool bPreemptive = true, float delayTime = 0f, bool bWaitForGameEnd = false)
	{
		PopupMessage value = new PopupMessage(NKCPublisherModule.GetErrorMessage(resultCode, additionalError), position, delayTime, bPreemptive, bShowFX, bWaitForGameEnd);
		s_llMessage.AddLast(value);
	}

	public static void AddPopupMessage(PopupMessage msg)
	{
		s_llMessage.AddLast(msg);
	}

	public static void Update(float deltaTime)
	{
		if (NKCScenManager.GetScenManager().GetNowScenState() != NKC_SCEN_STATE.NSS_START || (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_CONTRACT && (NKCUIContractSequence.IsInstanceOpen || NKCUIGameResultGetUnit.IsInstanceOpen || NKCUIResult.IsInstanceOpen)))
		{
			return;
		}
		LinkedListNode<PopupMessage> linkedListNode = s_llMessage.First;
		while (linkedListNode != null)
		{
			PopupMessage value = linkedListNode.Value;
			if (value.m_delayTime <= 0f)
			{
				if (value.m_bWaitForGameEnd && NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
				{
					linkedListNode = linkedListNode.Next;
					continue;
				}
				NKCUIManager.NKCPopupMessage.Open(value);
				LinkedListNode<PopupMessage> node = linkedListNode;
				linkedListNode = linkedListNode.Next;
				s_llMessage.Remove(node);
			}
			else
			{
				value.m_delayTime -= deltaTime;
				linkedListNode = linkedListNode.Next;
			}
		}
	}
}
