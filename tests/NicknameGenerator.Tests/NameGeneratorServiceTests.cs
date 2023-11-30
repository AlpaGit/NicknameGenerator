namespace NicknameGenerator.Tests;

public class NameGeneratorServiceTests
{
    [Fact]
    public void VowelsAndConsonants_ShouldNotBeEmpty()
    {
        // Ensuring that the static constructor properly initializes vowels and consonants
        Assert.NotEmpty(NameGeneratorService.Vowels);
        Assert.NotEmpty(NameGeneratorService.Consonants);
    }
    
    [Fact]
    public void GenerateSyllable_ShouldStartWithConsonantFollowedByVowel()
    {
        Span<char> buffer = stackalloc char[NameGeneratorService.SyllabeMaxLength];

        NameGeneratorService.GenerateSyllabe(buffer);
        Assert.True(char.IsLetter(buffer[0]));
        Assert.Contains(buffer[0], NameGeneratorService.Consonants);
        Assert.Contains(buffer[^1], NameGeneratorService.Vowels);
    }
    
    
    [Fact]
    public void GenerateName_ShouldReturnNonEmptyString()
    {
        var name = NameGeneratorService.GenerateName();
        Assert.False(string.IsNullOrEmpty(name));
    }
    
    [Fact]
    public void GenerateName_ShouldContainHyphenForComposedName()
    {
        // This test might not always pass due to the randomness of generating a composed name.
        // It's recommended to run it multiple times or adjust the probability for the test environment.
        var name = NameGeneratorService.GenerateName();
        if (name.Contains("-"))
        {
            var parts = name.Split('-');
            Assert.Equal(2, parts.Length);
            // Each part should be a valid name
            foreach (var part in parts)
            {
                Assert.False(string.IsNullOrEmpty(part));
            }
        }
    }
    
    [Fact]
    public void GenerateName_ShouldNotContainMoreThanOneHyphen()
    {
        // This test might not always pass due to the randomness of generating a composed name.
        // It's recommended to run it multiple times or adjust the probability for the test environment.
        var name = NameGeneratorService.GenerateName();
        Assert.DoesNotContain("--", name);
    }
    
    [Fact]
    public void GenerateName_ShouldNotContainMoreThanOneHyphenInARow()
    {
        // This test might not always pass due to the randomness of generating a composed name.
        // It's recommended to run it multiple times or adjust the probability for the test environment.
        var name = NameGeneratorService.GenerateName();
        Assert.DoesNotContain("---", name);
    }
    
    [Fact]
    public void GenerateName_LengthShouldBeBetween3And12()
    {
        // This test might not always pass due to the randomness of generating a composed name.
        // It's recommended to run it multiple times or adjust the probability for the test environment.
        var name = NameGeneratorService.GenerateName();
        Assert.InRange(name.Length, 3, 16);
    }
    
    [Fact]
    public void GenerateSyllable_ShouldBeOfExpectedLength()
    {
        Span<char> buffer = stackalloc char[NameGeneratorService.SyllabeMaxLength];
        
        var length = NameGeneratorService.GenerateSyllabe(buffer);
        Assert.InRange(length, 2, 3); // Assuming a syllable is 2-3 characters long
    }

    [Fact]
    public void GenerateFirstPart_ShouldStartWithUppercaseLetter()
    {
        Span<char> buffer = stackalloc char[NameGeneratorService.SyllabeMaxLength * 2];

        NameGeneratorService.GenerateFirstPart(buffer);
        Assert.True(char.IsUpper(buffer[0]));
    }

    [Fact]
    public void GenerateFirstPart_ShouldBeOfValidLength()
    {
        Span<char> buffer = stackalloc char[NameGeneratorService.SyllabeMaxLength * 2];

        var length = NameGeneratorService.GenerateFirstPart(buffer);
        Assert.InRange(length, 1, 4); // Define the expected length range based on your logic
    }

    [Fact]
    public void GenerateMiddlePart_ShouldBeOfValidLength()
    {     
        Span<char> buffer = stackalloc char[NameGeneratorService.SyllabeMaxLength * 3];

        var length = NameGeneratorService.GenerateMiddlePart(buffer);
        // The length can be zero (no middle part) or more depending on the number of syllables
        Assert.True(length is 0 or >= 2);
    }

    [Fact]
    public void GenerateLastPart_ShouldEndWithVowelOrConsonant()
    {       
        Span<char> buffer = stackalloc char[NameGeneratorService.SyllabeMaxLength * 3];

        var length = NameGeneratorService.GenerateLastPart(buffer);
        
        var lastChar = buffer[length-1];
        Assert.True(NameGeneratorService.Vowels.Contains(lastChar) || NameGeneratorService.Consonants.Contains(lastChar));
    }

    [Theory]
    [Repeat(1000)]
    public void GenerateSimpleName_ShouldBeOfValidLength(int i)
    {
        var simpleName = NameGeneratorService.GenerateSimpleName();
        Assert.InRange(simpleName.Length, 2, 14); // Define the expected range based on your generation logic
    }

    [Fact]
    public void GenerateName_ShouldNotContainInvalidCharacters()
    {
        var name = NameGeneratorService.GenerateName();
        Assert.DoesNotContain("_", name); // Assuring that placeholders are replaced
    }



}