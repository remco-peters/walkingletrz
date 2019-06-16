

public class Achievement
{
    public string Name;
    public int Amount;
    public int Credits;
    public int Level;
    public string NameInPlayfab;

    public Achievement(string name, int amount, int credits, int level, string nameInPlayfab)
    {
        Name = name;
        Amount = amount;
        Credits = credits;
        Level = level;
        NameInPlayfab = nameInPlayfab;
    }
}
