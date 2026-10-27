using System.Text.RegularExpressions;

namespace TfIdf;

public static class TextProcessor
{
    private static readonly Dictionary<string, string> StemDictionary = new()
    {
        { "компьютеров", "компьютер" },
        { "компьютеры", "компьютер" },
        { "компьютера", "компьютер" },
        { "ноутбука", "ноутбук" },
        { "ноутбуки", "ноутбук" },
        { "видеокарта", "видеокарта" },
        { "видеокарты", "видеокарта" },
        { "видеокарт", "видеокарта" }
    };
    
    private static readonly Dictionary<string, string> Synonyms = new()
    {
        { "пк", "компьютер" },
        { "лэптоп", "ноутбук" },
        { "gpu", "видеокарта" }
    };
    
    /// <summary>
    /// Ищет термин в словаре синонимов и склонений
    /// </summary>
    /// <param name="word">слово</param>
    /// <returns>термин</returns>
    public static string ProcessToken(string word)
    {
        string lower = word.ToLower();
        
        if (Synonyms.TryGetValue(lower, out var canonicalSynonym))
            lower = canonicalSynonym;

        return StemDictionary.GetValueOrDefault(lower, lower);
    }
    
    /// <summary>
    /// Токенизация и нормализация текста
    /// </summary>
    /// <param name="text">текст</param>
    /// <returns>токены</returns>
    public static List<string> TokenizeAndNormalize(string text)
    {
        var matches = Regex.Matches(text.ToLower(), @"\b\w+\b");
        var result = new List<string>();
        foreach (Match match in matches)
        {
            result.Add(ProcessToken(match.Value));
        }
        return result;
    }
}