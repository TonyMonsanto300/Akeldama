public class ArmorSet {
    public string Name;
    public ArmorClass ArmorClass;
    public ArmorView View;
    public int Defense; //TODO: Add stats

    public ArmorSet(string name, ArmorClass armorClass, ArmorView view) {
        Name = name;
        ArmorClass = armorClass;
        View = view;
    }
}