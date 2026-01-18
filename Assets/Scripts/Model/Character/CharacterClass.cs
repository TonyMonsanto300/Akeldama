public class CharacterClass {
    public ClassEnum Enum;
    public string Name;

    public int BaseHP;
    public int BaseEP;
    public int BaseMP;

    public StatBlock BaseStats;
    public BaseClassEquipment BaseEquipment;
    public string[] Mutations;

    public CharacterClass(
        ClassEnum classEnum,
        string name,
        int baseHP,
        int baseEP,
        int baseMP,
        StatBlock baseStats,
        BaseClassEquipment baseEquipment,
        string[] mutations = null
    ) {
        Enum = classEnum;
        Name = name;
        BaseHP = baseHP;
        BaseEP = baseEP;
        BaseMP = baseMP;
        BaseStats = baseStats;
        BaseEquipment = baseEquipment;
        Mutations = mutations ?? new string[0];
    }
}