using BenchmarkDotNet.Attributes;

namespace NicknameGenerator.Benchmark;

[MemoryDiagnoser]
public class NicknameBench 
{
    [Benchmark]
    public void GenerateName()
    {
        NameGeneratorService.GenerateName();
    }
        
    [Benchmark]
    public void GenerateSimpleName()
    {
        NameGeneratorService.GenerateSimpleName();
    }
    
    [Benchmark]
    public void GenerateSyllable()
    {
        Span<char> buffer = stackalloc char[NameGeneratorService.SyllabeMaxLength];

        NameGeneratorService.GenerateSyllabe(buffer);
    }
    
    [Benchmark]
    public void GenerateFirstPart()
    {     
        Span<char> buffer = stackalloc char[NameGeneratorService.SyllabeMaxLength * 2];

        NameGeneratorService.GenerateFirstPart(buffer);
    }
    
    [Benchmark]
    public void GenerateMiddlePart()
    {
        Span<char> buffer = stackalloc char[NameGeneratorService.SyllabeMaxLength * 3];

        NameGeneratorService.GenerateMiddlePart(buffer);
    }
    
    [Benchmark]
    public void GenerateLastPart()
    {
        Span<char> buffer = stackalloc char[NameGeneratorService.SyllabeMaxLength * 2];

        NameGeneratorService.GenerateLastPart(buffer);
    }

}