namespace NicknameGenerator;

public static class NameGeneratorService
{
    public const int SyllabeMaxLength = 3;
    
    private const int DefaultFirstPartIsVowelProbability = 20;
    private const int DefaultAddSyllableInMiddlePartProbability = 45;
    private const int DefaultUseTerminalLetterProbability = 30;
    private const int DefaultComposedNamesProbability = 8;

    public static readonly char[] Vowels;
    public static readonly char[] Consonants;
    public static readonly Dictionary<char, char[]> Bigrams;

    static NameGeneratorService()
    {
        Bigrams = new Dictionary<char, char[]>();

        char[] primaryVowels = { 'a', 'e', 'i', 'o', 'u', 'y' };
        char[] primaryConsonants =
            { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'z' };
        short[] vowelsProbabilities     = { 9, 15, 8, 6, 6, 1 };
        short[] consonantsProbabilities = { 2, 2, 3, 2, 2, 2, 1, 1, 5, 3, 6, 2, 1, 6, 6, 6, 2, 1, 1, 1 };

        AddBigram('c', new[] { '_', 'h', 'l', 'r' }, new short[] { 8, 3, 2, 2 });
        AddBigram('g', new[] { '_', 'l', 'n', 'r' }, new short[] { 10, 2, 1, 2 });
        AddBigram('l', new[] { '_', 'l' }, new short[] { 8, 1 });
        AddBigram('m', new[] { '_', 'm' }, new short[] { 8, 1 });
        AddBigram('n', new[] { '_', 'n' }, new short[] { 6, 1 });
        AddBigram('p', new[] { '_', 'h', 'l', 'p', 'r' }, new short[] { 8, 2, 1, 3, 1 });
        AddBigram('q', new[] { '_', 'u' }, new short[] { 0, 1 });
        AddBigram('s', new[] { '_', 'h', 'k', 'l', 'n', 'p', 'q', 'r', 's', 't' },
            new short[] { 15, 1, 1, 1, 2, 5, 1, 2, 10, 5 });
        AddBigram('t', new[] { '_', 'h', 'r', 't' }, new short[] { 8, 3, 5, 1 });
        AddBigram('a', new[] { 't' }, new short[] { 1 });
        AddBigram('e', new[] { 't', 'd' }, new short[] { 2, 1 });
        AddBigram('i', Array.Empty<char>(), Array.Empty<short>());
        AddBigram('o', new[] { 't' }, new short[] { 1 });
        AddBigram('u', new[] { 's', 't' }, new short[] { 2, 1 });
        AddBigram('y', Array.Empty<char>(), Array.Empty<short>());

        Vowels     = GenerateProbabilisticList(primaryVowels, vowelsProbabilities);
        Consonants = GenerateProbabilisticList(primaryConsonants, consonantsProbabilities);
    }

    private static char[] GenerateProbabilisticList(char[] primaries, short[] probabilities)
    {
        var nb = probabilities.Aggregate(0, (current, prob) => current + prob);

        var index = new List<int>();
        for (var j = 0; j < nb; ++j)
        {
            index.Add(j);
        }

        var result = new char[nb];
        for (var i = 0; i < primaries.Length; ++i)
        {
            for (var k = 0; k < probabilities[i]; ++k)
            {
                var l           = Random.Shared.Next(index.Count);
                var randomIndex = index[l];
                result[randomIndex] = primaries[i];
                index.RemoveAt(l);
            }
        }

        return result;
    }

    private static void AddBigram(char c, char[] possibles, short[] probabilities)
    {
        var nb = 0;
        foreach (var prob in probabilities)
        {
            nb += prob;
        }

        var index = new List<int>();
        for (var j = 0; j < nb; ++j)
        {
            index.Add(j);
        }

        var result = new char[nb];
        for (var k = 0; k < possibles.Length; ++k)
        {
            for (var l = 0; l < probabilities[k]; ++l)
            {
                var m           = Random.Shared.Next(index.Count);
                var randomIndex = index[m];
                result[randomIndex] = possibles[k];
                index.RemoveAt(m);
            }
        }

        Bigrams[c] = result;
    }

    private static bool CheckRandom(short probability)
    {
        return Random.Shared.Next(100) < probability;
    }

    private static char GetRandomVowel()
    {
        return Vowels[Random.Shared.Next(Vowels.Length)];
    }

    private static char GetRandomConsonant()
    {
        return Consonants[Random.Shared.Next(Consonants.Length)];
    }

    public static int GenerateSyllabe(Span<char> buffer)
    {
        if (buffer.Length < 3) // Ensure there's enough space in the buffer
        {
            throw new ArgumentException("Buffer too small for a syllable.", nameof(buffer));
        }

        var length = 0;

        var c = GetRandomConsonant();
        buffer[length++] = c;

        if (Bigrams.TryGetValue(c, out var tab) && tab.Length > 0)
            buffer[length++] = tab[Random.Shared.Next(tab.Length)];

        buffer[length++] = GetRandomVowel();

        for (var i = 0; i < length; i++)
        {
            if (buffer[i] != '_')
            {
                continue;
            }

            // Shift the rest of the array one position to the left
            for (var j = i; j < length - 1; j++)
            {
                buffer[j] = buffer[j + 1];
            }

            length--; // Decrease the length of the result
            i--;      // Recheck the current position
        }

        return length;
    }

    /// <summary>
    /// A First part is usually composed of 1 syllable or 1 Vowel.
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static int GenerateFirstPart(Span<char> buffer)
    {
        if (CheckRandom(DefaultFirstPartIsVowelProbability))
        {
            // First part is a vowel
            buffer[0] = char.ToUpperInvariant(GetRandomVowel());

            return 1;
        }

        // First part is a consonant and possibly a syllable
        Span<char> syllableBuffer = stackalloc char[SyllabeMaxLength]; // Local buffer for the syllable

        var syllableLength = GenerateSyllabe(syllableBuffer);
        syllableBuffer[..syllableLength].CopyTo(buffer);

        var length = syllableLength;

        // Handle duplicate consonants at the beginning
        if (length > 1 && buffer[0] == buffer[1])
        {
            for (var i = 0; i < length - 1; i++)
            {
                buffer[i] = buffer[i + 1];
            }

            length--;
        }

        buffer[0] = char.ToUpperInvariant(buffer[0]); // Capitalize the first character
        return length;                                // Return the actual length of the first part
    }

    /// <summary>
    /// The middle part is usually composed of 0 to 3 syllables.
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static int GenerateMiddlePart(Span<char> buffer)
    {
        if (buffer.Length == 0)
        {
            return 0;
        }

        var length = 0;

        Span<char> syllableBuffer = stackalloc char[SyllabeMaxLength]; // Local buffer for syllables

        while (length < buffer.Length && CheckRandom(DefaultAddSyllableInMiddlePartProbability) && length < 3)
        {
            var syllableLength = GenerateSyllabe(syllableBuffer);
            if (length + syllableLength > buffer.Length)
            {
                break; // Stop if the buffer can't accommodate more syllables
            }

            syllableBuffer[..syllableLength].CopyTo(buffer[length..]);
            length += syllableLength;
            
            syllableBuffer.Clear();
        }

        return length; // Return the length of the middle part
    }

    /// <summary>
    /// The last part is usually composed of 1 to 2 syllables.
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static int GenerateLastPart(Span<char> buffer)
    {
        if (buffer.Length < 3) // Ensure buffer is large enough for a typical last part
        {
            return 0;
        }

        Span<char> syllableBuffer = stackalloc char[SyllabeMaxLength]; // Buffer for the syllable

        var syllableLength = GenerateSyllabe(syllableBuffer);

        if (syllableLength > buffer.Length)
        {
            // If the buffer can't fit the syllable, you can choose to either truncate or skip
            syllableLength = buffer.Length;
        }

        syllableBuffer[..syllableLength].CopyTo(buffer);
        var length = syllableLength;

        // Check for the addition of a terminal letter based on probability
        if (length <= 0 || Random.Shared.Next(100) >= DefaultUseTerminalLetterProbability)
        {
            return length; // Return the actual length of the last part
        }

        var lastChar = buffer[length - 1];

        if (!Bigrams.TryGetValue(lastChar, out var possibleEndings) || possibleEndings.Length <= 0)
        {
            return length; // Return the actual length of the last part
        }

        var terminalChar = possibleEndings[Random.Shared.Next(possibleEndings.Length)];

        // Ensure there's space in the buffer for the terminal character
        if (length < buffer.Length)
        {
            buffer[length++] = terminalChar;
        }

        return length; // Return the actual length of the last part
    }

    public static string GenerateSimpleName()
    {
        Span<char> nameBuffer = stackalloc char[7];

        var length = 0;

        length += GenerateFirstPart(nameBuffer[length..]);
        length += GenerateMiddlePart(nameBuffer[length..]);
        length += GenerateLastPart(nameBuffer[length..]);

        return new string(nameBuffer[..length]);
    }

    public static int GenerateSimpleName(Span<char> nameBuffer)
    {
        var length = 0;

        length += GenerateFirstPart(nameBuffer[length..]);
        length += GenerateMiddlePart(nameBuffer[length..]);
        length += GenerateLastPart(nameBuffer[length..]);

        return length;
    }

    public static string GenerateName()
    {
        // Estimate a buffer size that can typically hold either a simple or composed name
        Span<char> buffer = stackalloc char[16];

        var length = 0;

        // Generate the first simple name directly into the buffer
        length += GenerateSimpleName(buffer[length..]);

        // Check for composed names
        if (CheckRandom(DefaultComposedNamesProbability))
        {
            // Add a hyphen if there's space
            if (length < buffer.Length)
            {
                buffer[length++] = '-';
            }

            // Generate the second simple name, if there's space
            if (length < buffer.Length)
            {
                length += GenerateSimpleName(buffer[length..]);
            }
        }

        return new string(buffer[..length]);
    }
}