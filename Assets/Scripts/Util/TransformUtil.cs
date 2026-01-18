using UnityEngine;

public static class TransformUtil {
    public static Transform FindRecursive(Transform root, string name) {
        if (root.name == name) return root;
        foreach (Transform child in root) {
            Transform found = FindRecursive(child, name);
            if (found != null) return found;
        }
        return null;
    }
}