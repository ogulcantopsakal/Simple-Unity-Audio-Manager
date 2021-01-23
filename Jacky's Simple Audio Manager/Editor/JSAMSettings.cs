﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace JSAM.JSAMEditor
{
    public class JSAMSettings : ScriptableObject
    {
        [SerializeField] string packagePath;
        public string PackagePath
        {
            get
            {
                if (packagePath.IsNullEmptyOrWhiteSpace())
                {
                    packagePath = JSAMEditorHelper.GetAudioManagerPath();
                    packagePath = packagePath.Remove(packagePath.IndexOf("/Scripts/AudioManager.cs"));
                }
                return packagePath;
            }
        }
        [SerializeField] string presetsPath;
        public string PresetsPath
        {
            get
            {
                if (presetsPath.IsNullEmptyOrWhiteSpace())
                {
                    presetsPath = PackagePath + "/Presets";
                }
                return presetsPath;
            }
        }
        [SerializeField] string generatedEnumsPath;
        public string GeneratedEnumsPath
        {
            get
            {
                if (generatedEnumsPath.IsNullEmptyOrWhiteSpace())
                {
                    generatedEnumsPath = PackagePath + "/JSAMGenerated";
                }
                return generatedEnumsPath;
            }
        }

        static string settingsAssetName = "JSAMSettings.asset";

        public static string SettingsSavePath
        {
            get
            {
                string path = JSAMEditorHelper.GetAudioManagerPath();
                path = path.Remove(path.IndexOf("/Scripts/AudioManager.cs"));
                path += "/Editor/Preferences";
                return path;
            }
        }
        
        public static JSAMSettings Settings
        {
            get
            {
                var settings = AssetDatabase.LoadAssetAtPath<JSAMSettings>(SettingsSavePath + "/" + settingsAssetName);
                if (settings == null)
                {
                    if (!AssetDatabase.IsValidFolder(SettingsSavePath))
                    {
                        JSAMEditorHelper.GenerateFolderStructure(SettingsSavePath, true);
                    }
                    settings = ScriptableObject.CreateInstance<JSAMSettings>();
                    JSAMEditorHelper.CreateAssetSafe(settings, SettingsSavePath + "/" + settingsAssetName);
                    AssetDatabase.SaveAssets();
                }
                return settings;
            }
        }
        
        public static SerializedObject SerializedObject
        {
            get
            {
                return new SerializedObject(Settings);
            }
        }
    }

    // Register a SettingsProvider using IMGUI for the drawing framework:
    static class JSAMSettingsRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Project Settings window.
            var provider = new SettingsProvider("Project/Audio - JSAM", SettingsScope.Project)
            {
                // By default the last token of the path is used as display name if no label is provided.
                //label = "Audio - JSAM",
                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = (searchContext) =>
                {
                    var settings = JSAMSettings.SerializedObject;
                    SerializedProperty packagePath = settings.FindProperty("packagePath");
                    SerializedProperty presetsPath = settings.FindProperty("presetsPath");
                    SerializedProperty enumPath = settings.FindProperty("generatedEnumsPath");

                    EditorGUILayout.PropertyField(packagePath);
                    EditorGUILayout.PropertyField(presetsPath);
                    EditorGUILayout.PropertyField(enumPath);
                    if (GUILayout.Button("Reset to Default", new GUILayoutOption[] { GUILayout.ExpandWidth(false) }))
                    {
                        packagePath.stringValue = JSAMEditorHelper.GetAudioManagerPath();
                        packagePath.stringValue = packagePath.stringValue.Remove(packagePath.stringValue.IndexOf("/Scripts/AudioManager.cs"));
                        presetsPath.stringValue = packagePath.stringValue + "/Presets";
                        enumPath.stringValue = packagePath.stringValue + "/JSAMGenerated";
                    }

                    if (settings.hasModifiedProperties)
                    {
                        settings.ApplyModifiedProperties();
                    }
                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] { "JSAM", "AudioManager", "Package", "Presets", "Enums" })
            };
    
            return provider;
        }
    }
}