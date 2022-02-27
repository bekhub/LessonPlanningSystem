
int GeneratorCount = 0;
int IsOddCount = 0;
int IsPrimeCount = 0;

IEnumerable<int> Generator()
{
    for (int i = 0; i < 10; i++) {
        GeneratorCount++;
        Console.WriteLine($"Executing in {nameof(Generator)}: {i}");
        yield return i;
    }
}

var odds = Generator().ToList().Where(IsOdd);
var primeOdds = odds.Where(number => {
    IsPrimeCount++;
    var res = $"Executing in {nameof(IsPrime)}: {number} - ";
    if (number < 2) {
        Console.WriteLine(res + "False");
        return false;
    }
    if (number % 2 == 0) {
        Console.WriteLine(res + (number == 2));
        return number == 2;
    }
    int root = (int)Math.Sqrt(number);
    for (int i = 3; i <= root; i += 2) {
        if (number % i != 0) continue;

        Console.WriteLine(res + "False");
        return false;
    }
    Console.WriteLine(res + "True");
    return true;
});
var f = primeOdds.Where(x => x == 100).Take(1);
foreach (var r in f) {
    Console.WriteLine(r);
}
Console.WriteLine(nameof(GeneratorCount) + ": " + GeneratorCount);
Console.WriteLine(nameof(IsOddCount) + ": " + IsOddCount);
Console.WriteLine(nameof(IsPrimeCount) + ": " + IsPrimeCount);

bool IsOdd(int number)
{
    IsOddCount++;
    var res = number % 2 == 1;
    Console.WriteLine($"Executing in {nameof(IsOdd)}: {number} - {res}");
    return res;
}

bool IsPrime(int number)
{
    IsPrimeCount++;
    var res = $"Executing in {nameof(IsPrime)}: {number} - ";
    if (number < 2) {
        Console.WriteLine(res + "False");
        return false;
    }
    if (number % 2 == 0) {
        Console.WriteLine(res + (number == 2));
        return number == 2;
    }
    int root = (int)Math.Sqrt(number);
    for (int i = 3; i <= root; i += 2) {
        if (number % i != 0) continue;

        Console.WriteLine(res + "False");
        return false;
    }
    Console.WriteLine(res + "True");
    return true;
}
