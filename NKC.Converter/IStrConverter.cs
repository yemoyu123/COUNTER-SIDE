namespace NKC.Converter;

public interface IStrConverter
{
	string Encryption(string str);

	string Decryption(string str);

	char ShiftChar(char ch, int range);
}
