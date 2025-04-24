using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponPickUp))]
public class StringDropdownEditor : Editor
{
    /*public override void OnInspectorGUI()
    {
        var script = (WeaponPickUp)target;

        // Find the index of the currently selected string
        int index = System.Array.IndexOf(WeaponPickUp.options, script.selectedWeapon);
        if (index < 0) index = 0;

        // Show dropdown
        int selectedIndex = EditorGUILayout.Popup("Selected Option", index, WeaponPickUp.options);

        // Update the string if the selection changed
        if (selectedIndex != index)
        {
            script.selectedWeapon = WeaponPickUp.options[selectedIndex];
            EditorUtility.SetDirty(script);
        }
    }*/
}
