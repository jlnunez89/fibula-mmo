namespace OpenTibia.Data.Contracts
{
    public interface IHighscore
    {
        int AccountId { get; set; }
        string Charname { get; set; }
        string Vocation { get; set; }
        int Level { get; set; }
        byte Exp { get; set; }
        byte Mlvl { get; set; }
        byte SkillShield { get; set; }
        byte SkillDist { get; set; }
        byte SkillAxe { get; set; }
        byte SkillSword { get; set; }
        byte SkillClub { get; set; }
        byte SkillFist { get; set; }
        byte SkillFish { get; set; }
    }
}
