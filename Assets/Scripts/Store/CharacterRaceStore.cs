using Mono.Cecil;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;

public enum RaceEnum {
    Random,
    Human,
    ElfM,   // regular elf
    ElfD,   // dark elf
    ElfA,   // ancient elf
    Celt,
    Dragonman,
    Dwarf   // uses elf ears prefab
}

public class CharacterRace {
    public RaceEnum Enum;
    public string Name;
    public RacePalette Palette;

    public CharacterRace(RaceEnum raceEnum, string name, RacePalette palette) {
        this.Enum = raceEnum;
        this.Name = name;
        this.Palette = palette;
    }
}

public static class CharacterRaceStore {
    private static string _humanName = "Human";
    private static string _elfMName = "Mystic Elf";
    private static string _elfDName = "Dark Elf";
    private static string _elfAName = "Ancient Elf";
    private static string _celtName = "Caelan";
    private static string _dragonmanName = "Dragonman";
    private static string _dwarfName = "Dwarf";

    private static RacePalette _humanPalette = new RacePalette("7F3300","C87048","3C86B9","E9C390","B99972");
    private static RacePalette _elfMPalette = new RacePalette("C89C35","DEBC76","3C86B9","76C5ED","4EA4DD");
    private static RacePalette _elfDPalette = new RacePalette("7E87C1","485197","FF0000","BB9179","874D35");
    private static RacePalette _elfAPalette = new RacePalette("000000","808080","9200FF","E9C390","A16E3E");
    private static RacePalette _celtPalette = new RacePalette("DAE07C","C89C35","3C86B9","E9C390","B99972","A63849");
    private static RacePalette _dragonManPalette = new RacePalette("EEE37A","C4BC66","5C9C30","FACEB2","D89676");
    private static RacePalette _dwarfPalette = new RacePalette("94AEC9","CDD5E2","3F569A","915B69","7C4E58");

    private static readonly Dictionary<RaceEnum, CharacterRace> Map =
        new() {
            { RaceEnum.Human, new CharacterRace(RaceEnum.Human, _humanName, _humanPalette)},
            { RaceEnum.ElfM, new CharacterRace(RaceEnum.ElfM, _elfMName, _elfMPalette)},
            { RaceEnum.ElfD, new CharacterRace(RaceEnum.ElfD, _elfDName, _elfDPalette)},
            { RaceEnum.ElfA, new CharacterRace(RaceEnum.ElfA, _elfAName, _elfAPalette)},
            { RaceEnum.Celt, new CharacterRace(RaceEnum.Celt, _celtName, _celtPalette) },
            { RaceEnum.Dragonman, new CharacterRace(RaceEnum.Dragonman, _dragonmanName, _dragonManPalette)},
            { RaceEnum.Dwarf, new CharacterRace(RaceEnum.Dwarf, _dwarfName, _dwarfPalette)},
        };

    public static CharacterRace GetRace(RaceEnum raceEnum) => Map[raceEnum];
    public static CharacterRace GetRandomRace() {
        var values = new List<CharacterRace>(Map.Values);
        int index = UnityEngine.Random.Range(0, values.Count);
        return values[index];
    }
}