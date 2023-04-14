using UnityEngine;
using UnityEditor;
using Amheklerior.NordicGameJam2023.Audio;

[CustomEditor(typeof(AudioEvent), true)]
public class AudioEventEditor : Editor
{
    [SerializeField] private AudioSource _previewer;

    public void OnEnable() => CreatePreviewer();
    public void OnDisable() => DestroyPreviewer();

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
        if (GUILayout.Button("Preview")) ((AudioEvent)target).Play(_previewer);
        EditorGUI.EndDisabledGroup();
    }

    #region Internals

    private void CreatePreviewer()
    {
        _previewer = EditorUtility.CreateGameObjectWithHideFlags(
            "Audio preview",
            HideFlags.HideAndDontSave,
            typeof(AudioSource)
        ).GetComponent<AudioSource>();
    }

    private void DestroyPreviewer()
    {
        DestroyImmediate(_previewer.gameObject);
    }

    #endregion
}