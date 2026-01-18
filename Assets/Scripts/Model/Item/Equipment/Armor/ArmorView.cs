#nullable enable

public class ArmorView {
    public string Clothing; //TODO: Turn into class
    public string? Pauldrons;
    public string? Cloak;

    public ArmorView(string clothing, string? pauldrons = null, string? cloak = null) {
        Clothing = clothing;
        Pauldrons = pauldrons;
        Cloak = cloak;
    }
}