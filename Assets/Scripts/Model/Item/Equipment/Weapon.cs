public class Weapon {
    public string Name { get; set; }
    public string Description { get; set; }
    public WeaponType Type { get; set; }
    public string View { get; set; }
    public int Damage { get; set; }


    public Weapon(string name, string description, WeaponType type, string view, int damage) {
        Name = name;
        Description = description;
        Type = type;
        View = view;
        Damage = damage;
    }
}

public static class WeaponConsants {
    public static Weapon SWORD_CLANGUARD = new Weapon("Clanguard Sword", "A sturdy sword favored by the Clanguard.", WeaponType.SWORD, "sword_clanguard", 10);
    public static Weapon SCYTHE_BASIC = new Weapon("Basic Scythe", "A basic scythe favored by reapers.", WeaponType.SCYTHE, "scythe_basic", 12);
    public static Weapon TRIDENT_BASIC = new Weapon("Basic Trident", "A basic trident favored by witches.", WeaponType.STAFF, "trident_basic", 8);
}