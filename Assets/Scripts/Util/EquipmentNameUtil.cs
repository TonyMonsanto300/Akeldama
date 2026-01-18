public static class EquipmentNameUtil {
    public static string CleanName(string n) {
        int inst = n.IndexOf(" (Instance)");
        return inst >= 0 ? n.Substring(0, inst) : n;
    }
}