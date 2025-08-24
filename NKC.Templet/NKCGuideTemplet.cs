using System.Collections.Generic;
using System.Linq;
using NKM.Templet.Base;

namespace NKC.Templet;

public class NKCGuideTemplet : INKMTemplet
{
	public int ID;

	public string ID_STRING;

	public List<NKCGuideTempletImage> lstImages = new List<NKCGuideTempletImage>();

	private static bool Loaded;

	public int Key => ID;

	public NKCGuideTemplet(int id, IEnumerable<NKCGuideTempletImage> imageTemplets)
	{
		ID = id;
		lstImages.AddRange(imageTemplets);
		ID_STRING = lstImages[0].ID_STRING;
	}

	public void Join()
	{
		foreach (NKCGuideTempletImage lstImage in lstImages)
		{
			lstImage.Join();
		}
	}

	public void Validate()
	{
	}

	public static void LoadFromLua()
	{
		NKMTempletContainer<NKCGuideTemplet>.Load(from e in NKMTempletLoader<NKCGuideTempletImage>.LoadGroup("AB_SCRIPT", "LUA_GUIDE_TEMPLET", "m_GuideTemplet", NKCGuideTempletImage.LoadFromLUA)
			select new NKCGuideTemplet(e.Key, e.Value), (NKCGuideTemplet e) => e.ID_STRING);
		Loaded = true;
	}

	public static NKCGuideTemplet Find(int id)
	{
		if (!Loaded)
		{
			LoadFromLua();
		}
		return NKMTempletContainer<NKCGuideTemplet>.Find(id);
	}

	public static NKCGuideTemplet Find(string strID)
	{
		if (!Loaded)
		{
			LoadFromLua();
		}
		return NKMTempletContainer<NKCGuideTemplet>.Find(strID);
	}
}
