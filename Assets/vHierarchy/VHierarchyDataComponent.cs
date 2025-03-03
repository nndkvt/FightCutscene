#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using System.Reflection;
using System.Linq;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;
using Type = System.Type;
using static VHierarchy.VHierarchyData;
using static VHierarchy.Libs.VUtils;
using static VHierarchy.Libs.VGUI;



namespace VHierarchy
{
    [ExecuteInEditMode]
    public abstract class VHierarchyDataComponent : MonoBehaviour, ISerializationCallbackReceiver
    {
        public void Awake() => VHierarchy.dataComponents_byScene[gameObject.scene] = this;

        public SceneData sceneData;


        public void OnBeforeSerialize() => VHierarchy.firstDataCacheLayer.Clear();
        public void OnAfterDeserialize() => VHierarchy.firstDataCacheLayer.Clear();



        [CustomEditor(typeof(VHierarchyDataComponent), true)]
        class Editor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                var style = EditorStyles.label;
                style.wordWrap = true;


                SetGUIEnabled(false);
                BeginIndent(0);

                Space(4);
                EditorGUILayout.LabelField("This component stores vHierarchy's data about which icons and colors are assigned to objects in this scene", style);

                Space(2);

                EndIndent(10);
                ResetGUIEnabled();

            }
        }

    }
}
#endif