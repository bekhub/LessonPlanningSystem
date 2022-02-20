var test = new Test {
    Some2 = 2,
    Some1 = 1,
};

Console.WriteLine(test);

record Test
{
    private int _some1;
    public int Some1 {
        get {
            return _some1;
        }
        init {
            if (value == 1 && Some2 is not 0 and < 3) {
                _some1 = 0;
                Some2 += value;
            } else {
                _some1 = value;
            }
        }
    }
    public int Some2 { get; init; }
}
