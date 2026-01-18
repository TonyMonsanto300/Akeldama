#nullable enable

using System.Collections.Generic;

public enum WeaponType {
    AXE,
    BATTLEAXE,
    DAGGER,
    GREATSWORD,
    HAMMER,
    POLEARM,
    SCYTHE,
    STAFF,
    SWORD,
    TRIDENT,
    WARHAMMER
}

public class BaseClassEquipment {
    public ArmorSet ArmorSet;
    public Helmet? Helmet;
    public Weapon? Weapon;
    public BaseClassEquipment(ArmorSet armorSet, Helmet? helmet = null, Weapon? weapon = null) {
        ArmorSet = armorSet;
        Helmet = helmet;
        Weapon = weapon;
    }
}

public enum ClassEnum {
    BANDIT,
    FENCER,
    FIGHTER,
    GUARDIAN,
    ILLUSIONIST,
    JUDGE,
    MAGICIAN,
    MYSTIC,
    PRIEST,
    RAIDER,
    REAPER,
    WITCH
}

public static class CharacterClassStore {
    private static Dictionary<ClassEnum, CharacterClass> _characterClasses = new Dictionary<ClassEnum, CharacterClass>() {
        { CharacterClassConstants.BANDIT_CLASS.Enum, CharacterClassConstants.BANDIT_CLASS },
        { CharacterClassConstants.FENCER_CLASS.Enum, CharacterClassConstants.FENCER_CLASS },
        { CharacterClassConstants.FIGHTER_CLASS.Enum, CharacterClassConstants.FIGHTER_CLASS },
        { CharacterClassConstants.GUARDIAN_CLASS.Enum, CharacterClassConstants.GUARDIAN_CLASS },
        { CharacterClassConstants.ILLUSIONIST_CLASS.Enum, CharacterClassConstants.ILLUSIONIST_CLASS },
        { CharacterClassConstants.JUDGE_CLASS.Enum, CharacterClassConstants.JUDGE_CLASS },
        { CharacterClassConstants.MAGICIAN_CLASS.Enum, CharacterClassConstants.MAGICIAN_CLASS },
        { CharacterClassConstants.MYSTIC_CLASS.Enum, CharacterClassConstants.MYSTIC_CLASS },
        { CharacterClassConstants.PRIEST_CLASS.Enum, CharacterClassConstants.PRIEST_CLASS },
        { CharacterClassConstants.REAPER_CLASS.Enum, CharacterClassConstants.REAPER_CLASS },
        { CharacterClassConstants.RAIDER_CLASS.Enum, CharacterClassConstants.RAIDER_CLASS },
        { CharacterClassConstants.WITCH_CLASS.Enum, CharacterClassConstants.WITCH_CLASS }
    };

    public static CharacterClass GetCharacterClass(ClassEnum classEnum) {
        return _characterClasses[classEnum];
    }
}

public static class CharacterClassNames {
    public static string BANDIT_NAME = "Bandit";
    public static string FENCER_NAME = "Fencer";
    public static string FIGHTER_NAME = "Fighter";
    public static string GUARDIAN_NAME = "Guardian";
    public static string ILLUSIONIST_NAME = "Illusionist";
    public static string JUDGE_NAME = "Judge";
    public static string MAGICIAN_NAME = "Magician";
    public static string MYSTIC_NAME = "Mystic";
    public static string PRIEST_NAME = "Priest";
    public static string RAIDER_NAME = "Raider";
    public static string REAPER_NAME = "Reaper";
    public static string WITCH_NAME = "Witch";
}

public static class CharacterClassConstants {
    public static CharacterClass BANDIT_CLASS = new CharacterClass(
        ClassEnum.BANDIT,
        CharacterClassNames.BANDIT_NAME,
        100,
        20,
        40,
        new StatBlock(20, 40, 40, 10, 10, 20),
        new BaseClassEquipment(
            new ArmorSet("Bandit Armor", ArmorClass.LIGHT, new ArmorView("clothing_bandit")),
            new Helmet("Bandit Helmet", ArmorClass.CLOTH, "face_bandana_red", true)
        )
    );
    public static CharacterClass FENCER_CLASS = new CharacterClass(
        ClassEnum.FENCER,
        CharacterClassNames.FENCER_NAME,
        90,
        120,
        30,
        new StatBlock(20, 50, 30, 5, 5, 20),
        new BaseClassEquipment(
            new ArmorSet("Fencer Armor", ArmorClass.LIGHT, new ArmorView("clothing_fencer")),
            null
        )
    );
    public static CharacterClass FIGHTER_CLASS = new CharacterClass(
        ClassEnum.FIGHTER,
        CharacterClassNames.FIGHTER_NAME,
        130,
        80,
        30,
        new StatBlock(40, 40, 20, 10, 10, 20),
        new BaseClassEquipment(
            new ArmorSet("Fighter Armor", ArmorClass.HALF, new ArmorView("clothing_fighter"))
        )
    );
    public static CharacterClass GUARDIAN_CLASS = new CharacterClass(
        ClassEnum.GUARDIAN,
        CharacterClassNames.GUARDIAN_NAME,
        140,
        100,
        20,
        new StatBlock(40, 30, 30, 10, 10, 20),
        new BaseClassEquipment(
            new ArmorSet("Clanguard Armor", ArmorClass.HALF, new ArmorView("clothing_clanguard", "pauldrons_clanguard")),
            new Helmet("Clanguard Helmet", ArmorClass.HALF, "helmet_clanguard"),
            WeaponConsants.SWORD_CLANGUARD
        )
    );
    public static CharacterClass ILLUSIONIST_CLASS = new CharacterClass(
        ClassEnum.ILLUSIONIST,
        CharacterClassNames.ILLUSIONIST_NAME,
        80,
        90,
        110,
        new StatBlock(10, 20, 30, 30, 10, 40),
        new BaseClassEquipment(
            new ArmorSet("Illusionist Robes", ArmorClass.CLOTH, new ArmorView("clothing_illusionist", "pauldrons_illusionist")),
            new Helmet("Illusionist Hood", ArmorClass.CLOTH, "hood_illusionist")
        )
    );
    public static CharacterClass JUDGE_CLASS = new CharacterClass(
        ClassEnum.JUDGE,
        CharacterClassNames.JUDGE_NAME,
        120,
        80,
        40,
        new StatBlock(30, 40, 30, 10, 10, 20),
        new BaseClassEquipment(
            new ArmorSet("Judge Armor", ArmorClass.HALF, new ArmorView("clothing_judge", "pauldrons_judge")),
            new Helmet("Judge Helmet", ArmorClass.CLOTH, "helmet_judge")
        )
    );
    public static CharacterClass MAGICIAN_CLASS = new CharacterClass(
        ClassEnum.MAGICIAN,
        CharacterClassNames.MAGICIAN_NAME,
        70,
        60,
        140,
        new StatBlock(10, 10, 30, 40, 20, 30),
        new BaseClassEquipment(
            new ArmorSet("Magician Robes", ArmorClass.CLOTH, new ArmorView("clothing_magician", null, null)),
            new Helmet("Magcians Hat", ArmorClass.CLOTH, "hat_magician")
        )
    );
    public static CharacterClass MYSTIC_CLASS = new CharacterClass(
        ClassEnum.MYSTIC,
        CharacterClassNames.MYSTIC_NAME,
        90,
        80,
        100,
        new StatBlock(20, 20, 20, 30, 20, 30),
        new BaseClassEquipment(
            new ArmorSet("Mystic Robes", ArmorClass.LIGHT, new ArmorView("clothing_mystic")),
            new Helmet("Mystic Hat", ArmorClass.CLOTH, "hat_mystic")
        )
    );
    public static CharacterClass PRIEST_CLASS = new CharacterClass(
        ClassEnum.PRIEST,
        CharacterClassNames.PRIEST_NAME,
        80,
        80,
        100,
        new StatBlock(10, 10, 10, 20, 50, 30),
        new BaseClassEquipment(
            new ArmorSet("Priest Robes", ArmorClass.CLOTH, new ArmorView("clothing_priest", null, null)),
            new Helmet("Priest Hood", ArmorClass.CLOTH, "crown_priest")
        )
    );
    public static CharacterClass RAIDER_CLASS = new CharacterClass(
        ClassEnum.RAIDER,
        "Raider",
        120,
        100,
        20,
        new StatBlock(50, 20, 30, 10, 10, 20),
        new BaseClassEquipment(
            new ArmorSet("Raider Armor", ArmorClass.LIGHT, new ArmorView("clothing_raider", "pauldrons_raider")),
            new Helmet("Raider Helmet", ArmorClass.LIGHT, "helmet_raider")
        )
    );
    public static CharacterClass REAPER_CLASS = new CharacterClass(
        ClassEnum.REAPER,
        "Reaper",
        110,
        80,
        40,
        new StatBlock(30, 30, 40, 10, 10, 20),
        new BaseClassEquipment(
            new ArmorSet("Reaper Armor", ArmorClass.HALF, new ArmorView("clothing_reaper")),
            new Helmet("Reaper Hood", ArmorClass.CLOTH, "hood_reaper", true),
            WeaponConsants.SCYTHE_BASIC
        )
    );
    public static CharacterClass WITCH_CLASS = new CharacterClass(
        ClassEnum.WITCH,
        CharacterClassNames.WITCH_NAME,
        70,
        80,
        100,
        new StatBlock(10, 10, 30, 35, 35, 20),
        new BaseClassEquipment(
            new ArmorSet("Witch Robes", ArmorClass.LIGHT, new ArmorView("clothing_witch", null, "shawl_witch")),
            null,
            WeaponConsants.TRIDENT_BASIC
        ),
        new string[] { "eye_witch" }
    );
}