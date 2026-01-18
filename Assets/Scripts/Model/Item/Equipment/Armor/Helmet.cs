using System;

public class Helmet {
    public string Name;
    public ArmorClass ArmorClass;
    public string Model;
    public bool HideHair;

    public Helmet(string name, ArmorClass armorClass, string model, bool hideHair = false) {
        Name = name;
        ArmorClass = armorClass;
        Model = model;
        HideHair = hideHair;
    }
}