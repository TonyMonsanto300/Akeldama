using UnityEngine;

[System.Serializable]
public class RacePalette {
    public Color hair1;
    public Color hair2;
    public Color eyes;
    public Color skin;   // skin1
    public Color skin2;
    public Color paint;  // defaults to skin

    public RacePalette(string hair1Hex, string hair2Hex, string eyesHex, string skinHex, string skin2Hex, string paintHex = null) {
        hair1 = Parse(hair1Hex);
        hair2 = Parse(hair2Hex);
        eyes = Parse(eyesHex);
        skin = Parse(skinHex);
        skin2 = Parse(skin2Hex);
        paint = string.IsNullOrEmpty(paintHex) ? skin : Parse(paintHex);
    }

    static Color Parse(string hex) {
        ColorUtility.TryParseHtmlString("#" + hex, out var c);
        return c;
    }
}