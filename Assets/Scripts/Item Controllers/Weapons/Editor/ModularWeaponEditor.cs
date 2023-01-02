using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(ModularWeapon))]
public class ModularWeaponEditor : Editor
{
    private readonly GUIContent _addAimLabel = new GUIContent("Add Aim Component");
    private readonly GUIContent _addAttackerLabel = new GUIContent("Add Attacker Component");
    private readonly GUIContent _addReloadLabel = new GUIContent("Add Reload Component");
    private readonly GUIContent _addRecoilLabel = new GUIContent("Add Recoil Component");

    private ModularWeapon _modularGun;

    private SerializedProperty  _equipAnim;
    private SerializedProperty  _hideAnim;


    private void OnEnable()
    {
        _equipAnim = serializedObject.FindProperty("equipAnimationData"); 
        _hideAnim = serializedObject.FindProperty("hideAnimationData");
    }

    public override void OnInspectorGUI()
    {
        _modularGun = (ModularWeapon) target;
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Weapon Default Animation Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_equipAnim, true);
        EditorGUILayout.PropertyField(_hideAnim, true);
        serializedObject.ApplyModifiedProperties();
        
        EditorGUILayout.Space(30);
        EditorGUILayout.LabelField("Item Settings", EditorStyles.boldLabel);

        _modularGun.item = (Item)EditorGUILayout.ObjectField("Item", _modularGun.item, typeof(Item), true);
        
        EditorGUILayout.Space(30);
        EditorGUILayout.LabelField("Weapon Default Components", EditorStyles.boldLabel);
        
        
        EditorGUILayout.Space();
        ShowModuleDetails(typeof(IAimer), _addAimLabel);
        EditorGUILayout.Space();
        ShowModuleDetails(typeof(IAttacker), _addAttackerLabel);
        EditorGUILayout.Space();
        ShowModuleDetails(typeof(IRecoil), _addRecoilLabel);
        EditorGUILayout.Space();
        ShowModuleDetails(typeof(IReloader), _addReloadLabel);
    }
    
    void AddModule(object o)
    {
        Undo.AddComponent(_modularGun.gameObject, (Type)o);
    }
    
     void ShowModuleDetails(Type baseType, GUIContent buttonLabel)
     {
         if (EditorGUILayout.DropdownButton(buttonLabel, FocusType.Passive))
         { 
             GenericMenu menu = new GenericMenu(); 
             List<Type> validTypes = GetScriptTypes(baseType); 
             foreach (var t in validTypes) 
                 menu.AddItem(new GUIContent(t.ToString()), false, AddModule, t); 
             menu.ShowAsContext();
         }

            var modules = _modularGun.GetComponents(baseType);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Existing: ", EditorStyles.boldLabel, GUILayout.Width(60));
            EditorGUILayout.BeginVertical();
            
            if (modules.Length > 0)
            {
                for (int i = 0; i < modules.Length; ++i)
                {
                    // Check if module is on root object
                    bool isRoot = (modules[i].transform == _modularGun.transform);

                    // Label string
                    string labelString = isRoot ?
                        modules[i].GetType().Name :
                        string.Format("{0} ({1})", modules[i].GetType().Name, modules[i].gameObject.name);

                    // Get label rect
                    var rect = EditorGUILayout.GetControlRect();
                    bool canViewChild = !isRoot && modules[i].gameObject.scene.IsValid();
                    if (canViewChild)
                        rect.width -= 20;
                    
                    EditorGUI.LabelField(rect, labelString);

                    if (canViewChild)
                    {
                        rect.x += rect.width;
                        rect.width = 20;
                        if (GUI.Button(rect, EditorGUIUtility.FindTexture("d_ViewToolOrbit"), EditorStyles.label))
                            EditorGUIUtility.PingObject(modules[i].gameObject);
                    }
                }
            }
            else
                EditorGUILayout.LabelField("<none>");
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

     List<Type> GetScriptTypes(Type baseClass)
     {
         List<Type> result = new List<Type>();

         var guids = AssetDatabase.FindAssets("t:MonoScript");
         for (int i = 0; i < guids.Length; ++i)
         {
             var script = AssetDatabase.LoadAssetAtPath<MonoScript>(AssetDatabase.GUIDToAssetPath(guids[i]));
             var t = script.GetClass();
             if (t != null && baseClass.IsAssignableFrom(t) && script.GetClass().IsSubclassOf(typeof(MonoBehaviour)))
                 result.Add(t);
         }

         return result;
     }
}
