using System.Security.Cryptography;

namespace HaushaltsOS.Api.Common.Households;

/// <summary>
/// Erzeugt kurze, kryptografisch zufällige Einladungscodes
/// </summary>
public static class InviteCodeGenerator
{
    private const string Alphabet = "ABCDEFGHJKMNPQRSTUVWXYZ23456789";

    /// <summary>
    /// Erzeugt einen achtstelligen Einladungscode
    /// </summary>
    public static string Generate()
    {
        char[] chars = new char[8];

        for(int i = 0; i < chars.Length; i++)
        {
            chars[i] = Alphabet[RandomNumberGenerator.GetInt32(Alphabet.Length)];
        }

        return new string(chars);
    }
}