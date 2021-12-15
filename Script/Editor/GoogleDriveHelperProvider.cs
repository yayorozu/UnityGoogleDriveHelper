using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Yorozu.GoogleDriveHelper.EditorTool
{
    public class GoogleDriveHelperProvider : SettingsProvider
    {
        private GoogleDriveHelperProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
        }
        
        /// <summary>
        /// Preference に表示させる
        /// </summary>
        /// <returns></returns>
        [SettingsProvider]
        public static SettingsProvider RegisterUser()
        {
            return new GoogleDriveHelperProvider(
                "Preferences/Google Drive Helper", 
                SettingsScope.User, new []{"GoogleDrive"})
            {
                label = "Google Drive Helper",
            };
        }

        private GoogleOAuthClientData _data;
        
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);
            var finds = AssetDatabase.FindAssets($"t:{nameof(GoogleOAuthClientData)}");
            if (finds.Length <= 0)
                return;

            var path = AssetDatabase.GUIDToAssetPath(finds[0]);
            _data = AssetDatabase.LoadAssetAtPath<GoogleOAuthClientData>(path);
        }

        public override void OnDeactivate()
        {
            _data = null;
        }

        public override void OnGUI(string searchContext)
        {
            GUILayout.Space(10);
            // アセットがなければ作らせる
            if (_data == null)
            {
                if (GUILayout.Button("Create Asset"))
                {
                    var path = EditorUtility.SaveFilePanelInProject(
                        "Select Save Folder",
                        "GoogleOAuthClientData",
                        "asset", "");
                    if (string.IsNullOrEmpty(path))
                        return;
                    
                    _data = ScriptableObject.CreateInstance<GoogleOAuthClientData>();
                    AssetDatabase.CreateAsset(_data, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
                return;
            }

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.TextField("ClientId", _data.ClientId);
                EditorGUILayout.TextField("ClientSecret", _data.ClientSecret);
                if (check.changed)
                {
                    
                }
            }

        }
    }
}