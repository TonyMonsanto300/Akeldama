using System.Collections.Generic;

public static class AvatarPaletteRepo {
    private static readonly Dictionary<NPCAvatar.Race, RacePalette> Map =
        new()
        {
            { NPCAvatar.Race.Human,     new RacePalette("7F3300","C87048","3C86B9","E9C390","B99972") },
            { NPCAvatar.Race.ElfM,      new RacePalette("C89C35","DEBC76","3C86B9","76C5ED","4EA4DD") },
            { NPCAvatar.Race.ElfD,      new RacePalette("7E87C1","485197","FF0000","BB9179","874D35") },
            { NPCAvatar.Race.ElfA,      new RacePalette("000000","808080","9200FF","E9C390","A16E3E") },
            { NPCAvatar.Race.Celt,      new RacePalette("DAE07C","C89C35","3C86B9","E9C390","B99972","A63849") },
            { NPCAvatar.Race.Dragonman, new RacePalette("EEE37A","C4BC66","5C9C30","FACEB2","D89676") },
            { NPCAvatar.Race.Dwarf,     new RacePalette("94AEC9","CDD5E2","3F569A","915B69","7C4E58") },
        };

    public static RacePalette GetPalette(NPCAvatar.Race race) => Map[race];
}