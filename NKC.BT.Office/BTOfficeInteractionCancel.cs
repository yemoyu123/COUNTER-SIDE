namespace NKC.BT.Office;

public class BTOfficeInteractionCancel : BTOfficeActionBase
{
	public override void OnStart()
	{
		bActionSuccessFlag = true;
		if (m_Character != null)
		{
			m_Character.UnregisterInteraction();
		}
	}
}
