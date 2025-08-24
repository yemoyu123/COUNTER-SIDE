namespace NKM;

public class NKMColor
{
	public float r;

	public float g;

	public float b;

	public float a;

	public NKMColor(float _r, float _g, float _b, float _a = 1f)
	{
		r = _r;
		g = _g;
		b = _b;
		a = _a;
	}

	public NKMColor()
	{
		Init();
	}

	public void Init(float fr = 1f, float fg = 1f, float fb = 1f, float fa = 1f)
	{
		r = fr;
		g = fg;
		b = fb;
		a = fa;
	}

	public void DeepCopyFromSource(NKMColor source)
	{
		r = source.r;
		g = source.g;
		b = source.b;
		a = source.a;
	}

	public bool LoadFromLua(NKMLua cNKMLua, string pKey)
	{
		if (cNKMLua.OpenTable(pKey))
		{
			cNKMLua.GetData(1, ref r);
			cNKMLua.GetData(2, ref g);
			cNKMLua.GetData(3, ref b);
			cNKMLua.GetData(4, ref a);
			cNKMLua.CloseTable();
		}
		return true;
	}

	public bool LoadFromLua(NKMLua cNKMLua, int index)
	{
		if (cNKMLua.OpenTable(index))
		{
			cNKMLua.GetData(1, ref r);
			cNKMLua.GetData(2, ref g);
			cNKMLua.GetData(3, ref b);
			cNKMLua.GetData(4, ref a);
			cNKMLua.CloseTable();
		}
		return true;
	}
}
