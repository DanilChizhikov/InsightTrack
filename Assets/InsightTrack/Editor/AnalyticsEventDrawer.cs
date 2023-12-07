using System.Collections.Generic;
using MbsCore.InsightTrack.Infrastructure;
using MbsCore.InsightTrack.Runtime;
using UnityEditor;
using UnityEngine;

namespace MbsCore.InsightTrack.Editor
{
    [CustomPropertyDrawer(typeof(AnalyticsEventAttribute))]
    internal sealed class AnalyticsEventDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                string message = $"Unsupported property type [{property.propertyType.ToString()}].\n" +
                                 $"Please use property with [{SerializedPropertyType.String.ToString()}] type!";
                EditorGUI.HelpBox(position, message, MessageType.Error);
                return;
            }

            GUIContent[] options = GetOptions();
            int lastEventIndex = GetLastEventIndex(options, property.stringValue);
            EditorGUI.BeginChangeCheck();
            int selectedIndex = EditorGUI.Popup(position, label, lastEventIndex, options);
            if (EditorGUI.EndChangeCheck())
            {
                if (selectedIndex > -1 && selectedIndex != lastEventIndex)
                {
                    property.stringValue = options[selectedIndex].text;
                }
            }
        }

        private int GetLastEventIndex(GUIContent[] options, string value)
        {
            for (int i = options.Length - 1; i >= 0; i--)
            {
                if (options[i].text == value)
                {
                    return i;
                }
            }

            return -1;
        }

        private IReadOnlyCollection<IAnalyticsConfig> GetConfigs()
        {
            var configs = new HashSet<IAnalyticsConfig>();
            string[] assetGuids = AssetDatabase.FindAssets($"t:{nameof(ScriptableObject)}");
            for (int i = assetGuids.Length - 1; i >= 0; i--)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[i]);
                Object asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                if (asset is not AnalyticsConfig config)
                {
                    continue;
                }

                configs.Add(config);
            }

            return configs;
        }

        private GUIContent[] GetOptions()
        {
            var options = new List<GUIContent>();
            IReadOnlyCollection<IAnalyticsConfig> configs = GetConfigs();
            foreach (var config in configs)
            {
                var events = new List<string>(config.Events);;
                for (int i = events.Count - 1; i >= 0; i--)
                {
                    var option = new GUIContent(events[i]);
                    if (!options.Contains(option))
                    {
                        options.Add(option);
                    }
                }
            }

            return options.ToArray();
        }
    }
}