using System.Collections.Generic;
using UnityEngine;

namespace NKC;

public class NKCMessage
{
	private static LinkedList<NKCMessageData> m_linklistNKMMessageData = new LinkedList<NKCMessageData>();

	public static void Init()
	{
	}

	public static void Update()
	{
		LinkedListNode<NKCMessageData> linkedListNode = m_linklistNKMMessageData.First;
		while (linkedListNode != null)
		{
			NKCMessageData value = linkedListNode.Value;
			if (value != null)
			{
				if (!(value.m_fLatency > 0f))
				{
					NKCScenManager.GetScenManager().MsgProc(value);
					LinkedListNode<NKCMessageData> next = linkedListNode.Next;
					m_linklistNKMMessageData.Remove(linkedListNode);
					linkedListNode = next;
					continue;
				}
				value.m_fLatency -= Time.deltaTime;
			}
			linkedListNode = linkedListNode.Next;
		}
	}

	public static void SendMessage(NKC_EVENT_MESSAGE eNKC_EVENT_MESSAGE, int msgID2 = 0, object param1 = null, object param2 = null, object param3 = null, bool bDirect = false, float fLatency = 0f)
	{
		NKCMessageData nKCMessageData = new NKCMessageData();
		nKCMessageData.m_NKC_EVENT_MESSAGE = eNKC_EVENT_MESSAGE;
		nKCMessageData.m_MsgID2 = msgID2;
		nKCMessageData.m_Param1 = param1;
		nKCMessageData.m_Param2 = param2;
		nKCMessageData.m_Param3 = param3;
		nKCMessageData.m_fLatency = fLatency;
		if (!bDirect)
		{
			m_linklistNKMMessageData.AddLast(nKCMessageData);
		}
		else
		{
			NKCScenManager.GetScenManager().MsgProc(nKCMessageData);
		}
	}
}
