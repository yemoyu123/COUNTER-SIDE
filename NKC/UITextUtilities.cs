using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class UITextUtilities : MonoBehaviour
{
	public static bool hasLinkText(string str)
	{
		string input = RemoveHtmlLikeTags(str);
		string pattern = "(^|[\\n ])(?<url>(http://www\\.|http://|https://)[^ ,\"\\s<]*)";
		Regex regex = new Regex("(^|[\\n ])(?<url>(www|ftp)\\.[^ ,\"\\s<]*)", RegexOptions.IgnoreCase);
		Regex regex2 = new Regex(pattern, RegexOptions.IgnoreCase);
		if (!regex.IsMatch(input))
		{
			return regex2.IsMatch(input);
		}
		return true;
	}

	public static string RemoveHtmlLikeTags(string str)
	{
		string pattern = "<[^>]+>| ";
		return Regex.Replace(str, pattern, "").Trim();
	}

	public static string FindIntersectingWord(Text textComp, Vector3 position, Camera camera)
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(textComp.GetComponent<RectTransform>(), position, camera, out var localPoint);
		int characterIndexFromPosition = GetCharacterIndexFromPosition(textComp, localPoint);
		if (!string.IsNullOrWhiteSpace(GetCharFromIndex(textComp, characterIndexFromPosition)))
		{
			return GetWordFromCharIndex(textComp.text, characterIndexFromPosition);
		}
		return "";
	}

	public static string FindIntersectingWordWrappedAroundSpace(TextMeshProUGUI textComp, Vector3 position, Camera camera, bool visibleOnly)
	{
		int num = TMP_TextUtilities.FindIntersectingCharacter(textComp, position, camera, visibleOnly);
		if (num == -1)
		{
			return "";
		}
		return GetWordFromCharIndex(textComp.text, num);
	}

	private static string GetCharFromIndex(Text textComp, int index)
	{
		char[] array = textComp.text.ToCharArray();
		if (index != -1 && index < array.Length)
		{
			return array[index].ToString() ?? "";
		}
		return "";
	}

	private static int GetCharacterIndexFromPosition(Text textComp, Vector2 pos)
	{
		TextGenerator cachedTextGenerator = textComp.cachedTextGenerator;
		if (cachedTextGenerator.lineCount == 0)
		{
			return 0;
		}
		int unclampedCharacterLineFromPosition = GetUnclampedCharacterLineFromPosition(textComp, pos, cachedTextGenerator);
		if (unclampedCharacterLineFromPosition < 0)
		{
			return 0;
		}
		if (unclampedCharacterLineFromPosition >= cachedTextGenerator.lineCount)
		{
			return cachedTextGenerator.characterCountVisible;
		}
		int startCharIdx = cachedTextGenerator.lines[unclampedCharacterLineFromPosition].startCharIdx;
		int lineEndPosition = GetLineEndPosition(cachedTextGenerator, unclampedCharacterLineFromPosition);
		for (int i = startCharIdx; i < lineEndPosition && i < cachedTextGenerator.characterCountVisible; i++)
		{
			UICharInfo uICharInfo = cachedTextGenerator.characters[i];
			Vector2 vector = uICharInfo.cursorPos / textComp.pixelsPerUnit;
			float num = pos.x - vector.x;
			float num2 = vector.x + uICharInfo.charWidth / textComp.pixelsPerUnit - pos.x;
			if (num < num2)
			{
				return i;
			}
		}
		return lineEndPosition;
	}

	private static int GetUnclampedCharacterLineFromPosition(Text textComp, Vector2 pos, TextGenerator generator)
	{
		float num = pos.y * textComp.pixelsPerUnit;
		float num2 = 0f;
		for (int i = 0; i < generator.lineCount; i++)
		{
			float topY = generator.lines[i].topY;
			float num3 = topY - (float)generator.lines[i].height;
			if (num > topY)
			{
				float num4 = topY - num2;
				if (num > topY - 0.5f * num4)
				{
					return i - 1;
				}
				return i;
			}
			if (num > num3)
			{
				return i;
			}
			num2 = num3;
		}
		return generator.lineCount;
	}

	private static int GetLineStartPosition(TextGenerator gen, int line)
	{
		line = Mathf.Clamp(line, 0, gen.lines.Count - 1);
		return gen.lines[line].startCharIdx;
	}

	private static int GetLineEndPosition(TextGenerator gen, int line)
	{
		line = Mathf.Max(line, 0);
		if (line + 1 < gen.lines.Count)
		{
			return gen.lines[line + 1].startCharIdx - 1;
		}
		return gen.characterCountVisible;
	}

	private static string GetWordFromCharIndex(string str, int characterIndex)
	{
		string text = str.Substring(0, characterIndex);
		string text2 = str.Substring(characterIndex);
		string text3 = text;
		int num = text.LastIndexOf(' ');
		if (num != -1)
		{
			text3 = text.Substring(num);
		}
		string text4 = text2;
		int num2 = text2.IndexOf(' ');
		if (num2 != -1)
		{
			text4 = text2.Substring(0, num2);
		}
		if (text3.IndexOf('\n') != -1)
		{
			text3 = text3.Substring(text3.IndexOf('\n'));
		}
		if (text4.IndexOf('\n') != -1)
		{
			text4 = text4.Substring(0, text4.IndexOf('\n'));
		}
		return text3.Replace("\n", "").Replace("\r", "") + text4.Replace("\n", "").Replace("\r", "");
	}
}
