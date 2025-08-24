using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NKC;

public class Korean
{
	public class Josa
	{
		private class JosaPair
		{
			public string josa1 { get; private set; }

			public string josa2 { get; private set; }

			public JosaPair(string josa1, string josa2)
			{
				this.josa1 = josa1;
				this.josa2 = josa2;
			}
		}

		private Regex _josaRegex = new Regex("\\(이\\)가|\\(와\\)과|\\(을\\)를|\\(은\\)는|\\(아\\)야|\\(이\\)여|\\(으\\)로|\\(이\\)라");

		private Regex _josaRegex_Rieul = new Regex("\\(으\\)로");

		private Dictionary<string, JosaPair> _josaPatternPaird = new Dictionary<string, JosaPair>
		{
			{
				"(이)가",
				new JosaPair("이", "가")
			},
			{
				"(와)과",
				new JosaPair("과", "와")
			},
			{
				"(을)를",
				new JosaPair("을", "를")
			},
			{
				"(은)는",
				new JosaPair("은", "는")
			},
			{
				"(아)야",
				new JosaPair("아", "야")
			},
			{
				"(이)여",
				new JosaPair("이여", "여")
			},
			{
				"(으)로",
				new JosaPair("으로", "로")
			},
			{
				"(이)라",
				new JosaPair("이라", "라")
			}
		};

		public string Replace(string src)
		{
			StringBuilder stringBuilder = new StringBuilder(src.Length);
			MatchCollection matchCollection = _josaRegex.Matches(src);
			int num = 0;
			foreach (Match item in matchCollection)
			{
				JosaPair josaPair = _josaPatternPaird[item.Value];
				stringBuilder.Append(src, num, item.Index - num);
				if (item.Index > 0)
				{
					char inChar = src[item.Index - 1];
					for (int num2 = item.Index - 1; num2 >= 0; num2--)
					{
						char c = src[num2];
						if (IsKorean(c))
						{
							inChar = c;
							break;
						}
					}
					if ((HasJong(inChar) && !_josaRegex_Rieul.IsMatch(item.Value)) || (HasJongExceptRieul(inChar) && _josaRegex_Rieul.IsMatch(item.Value)))
					{
						stringBuilder.Append(josaPair.josa1);
					}
					else
					{
						stringBuilder.Append(josaPair.josa2);
					}
				}
				else
				{
					stringBuilder.Append(josaPair.josa1);
				}
				num = item.Index + item.Length;
			}
			stringBuilder.Append(src, num, src.Length - num);
			return stringBuilder.ToString();
		}

		private static bool IsKorean(char inChar)
		{
			if (inChar >= '가')
			{
				return inChar <= '힣';
			}
			return false;
		}

		private static bool HasJong(char inChar)
		{
			if (inChar >= '가' && inChar <= '힣')
			{
				if ((inChar - 44032) % 28 > 0)
				{
					return true;
				}
				return false;
			}
			return false;
		}

		private static bool HasJongExceptRieul(char inChar)
		{
			if (inChar >= '가' && inChar <= '힣')
			{
				int num = (inChar - 44032) % 28;
				if (num == 8 || num == 0)
				{
					return false;
				}
				return true;
			}
			return false;
		}
	}

	private static Josa josa = new Josa();

	public static string ReplaceJosa(string src)
	{
		return josa.Replace(src);
	}
}
