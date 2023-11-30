using NicknameGenerator;

for(var i = 0; i < 1_000_000; ++i)
{
    var simpleName = NameGeneratorService.GenerateName();
    
    Console.WriteLine(simpleName);
}