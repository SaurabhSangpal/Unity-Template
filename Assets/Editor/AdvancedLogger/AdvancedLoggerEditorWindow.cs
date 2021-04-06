using UnityEditor;
using UnityEngine;
using Logger = AdvancedLogger.Logger;

public class AdvancedLoggerEditorWindow : EditorWindow
{
    private Logger logger;
    private Vector2 scrollPos;
    private GUIStyle guiStyle;

    private bool enableTrace, enableInfo, enableWarning, enableErrors, enableFatal;

    private void OnEnable() {
        logger   = Logger.instance;
        guiStyle = new GUIStyle {richText = true};

        enableTrace = enableInfo = enableWarning = enableErrors = enableFatal = true;
    }

    private void OnGUI() {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        enableTrace =
            EditorGUILayout.ToggleLeft("Trace", enableTrace,
                                       GUILayout.MaxWidth(position.width / 5));
        enableInfo =
            EditorGUILayout.ToggleLeft("Info", enableInfo, GUILayout.MaxWidth(position.width / 5));
        enableWarning =
            EditorGUILayout.ToggleLeft("Warning", enableWarning,
                                       GUILayout.MaxWidth(position.width / 5));
        enableErrors =
            EditorGUILayout.ToggleLeft("Error", enableErrors,
                                       GUILayout.MaxWidth(position.width / 5));
        enableFatal =
            EditorGUILayout.ToggleLeft("Fatal", enableFatal,
                                       GUILayout.MaxWidth(position.width / 5));
        EditorGUILayout.EndHorizontal();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width),
                                                    GUILayout.Height(position.height));

        logger ??= Logger.instance;
        // This check is required to avoid errors when not in play mode
        if (logger != null) {
            var _stream = logger.GetStream();
            for (var i = 0; i < _stream.Length; i++) {
                var _log = _stream[i];
                if (string.IsNullOrEmpty(_log)) continue;

                if (enableTrace && _log.StartsWith("T")) {
                    EditorGUILayout.LabelField($"<color=grey>{_log}</color>", guiStyle);
                }
                else if (enableInfo && _log.StartsWith("I")) {
                    EditorGUILayout.LabelField($"<color=white>{_log}</color>", guiStyle);
                }
                else if (enableWarning && _log.StartsWith("W")) {
                    EditorGUILayout.LabelField($"<color=yellow>{_log}</color>", guiStyle);
                }
                else if (enableErrors && _log.StartsWith("E")) {
                    EditorGUILayout.LabelField($"<color=orange>{_log}</color>", guiStyle);
                }
                else if (enableFatal && _log.StartsWith("F")) {
                    EditorGUILayout.LabelField($"<color=red>{_log}</color>", guiStyle);
                }
            }
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    [MenuItem("Window/Advanced Logger")]
    public static void ShowWindow() {
        GetWindow<AdvancedLoggerEditorWindow>("Advanced Logger");
    }
}