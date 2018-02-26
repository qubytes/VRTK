namespace VRTK
{
    using System;
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(VRTK_ControllerEvents))]
    public class VRTK_ControllerEvents_UnityEventsEditor : Editor
    {
        SerializedProperty delegatesProp;

        GUIContent iconToolbarMinus;
        GUIContent eventIDName;
        GUIContent[] eventTypes;
        GUIContent addButtonContent;

        private void OnEnable()
        {
            delegatesProp = serializedObject.FindProperty("delegates");
            addButtonContent = new GUIContent("Add New Event");
            eventIDName = new GUIContent("");

            iconToolbarMinus = new GUIContent(EditorGUIUtility.IconContent("Toolbar Minus"));
            iconToolbarMinus.tooltip = "Remove this event";

            string[] eventNames = Enum.GetNames(typeof(ControllerInteractionType));
            eventTypes = new GUIContent[eventNames.Length];
            for (int i = 0; i < eventNames.Length; i++)
            {
                eventTypes[i] = new GUIContent(eventNames[i]);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            int toBeRemovedEntry = -1;

            EditorGUILayout.Space();

            Vector2 removeButtonSize = GUIStyle.none.CalcSize(iconToolbarMinus);

            for (int i = 0; i < delegatesProp.arraySize; i++)
            {
                SerializedProperty delegateProperty = delegatesProp.GetArrayElementAtIndex(i);
                SerializedProperty eventIDProp = delegateProperty.FindPropertyRelative("eventID");
                SerializedProperty callbackProp = delegateProperty.FindPropertyRelative("callback");

                eventIDName.text = eventIDProp.enumDisplayNames[eventIDProp.enumValueIndex];
                EditorGUILayout.PropertyField(callbackProp, eventIDName);
                Rect unityEventRect = GUILayoutUtility.GetLastRect();

                float x = unityEventRect.xMax - removeButtonSize.x - 8;
                float y = unityEventRect.y + 1;
                Rect removeButtonRect = new Rect(x, y, removeButtonSize.x, removeButtonSize.y);
                if (GUI.Button(removeButtonRect, iconToolbarMinus, GUIStyle.none))
                {
                    //toBeRemovedEntry = i;
                    RemoveEntry(i);
                }

                EditorGUILayout.Space();
            }

            if (toBeRemovedEntry > -1)
            {
                //RemoveEntry(toBeRemovedEntry);
            }

            Rect addButtonRect = GUILayoutUtility.GetRect(addButtonContent, GUI.skin.button);
            const float addButtonWidth = 200f;
            addButtonRect.x = addButtonRect.x + (addButtonRect.width - addButtonWidth) / 2;
            addButtonRect.width = addButtonWidth;

            if (GUI.Button(addButtonRect, addButtonContent))
            {
                ShowAddTriggerMenu();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void RemoveEntry(int index)
        {
            delegatesProp.DeleteArrayElementAtIndex(index);
        }

        private void ShowAddTriggerMenu()
        {
            bool active = true;
            GenericMenu triggerMenu = new GenericMenu();

            for (int typeIndex = 0; typeIndex < eventTypes.Length; typeIndex++)
            {
                active = true;

                for (int delegateIndex = 0; delegateIndex < delegatesProp.arraySize; delegateIndex++)
                {
                    SerializedProperty delegateEntryProp = delegatesProp.GetArrayElementAtIndex(delegateIndex);
                    SerializedProperty eventIdProp = delegateEntryProp.FindPropertyRelative("eventID");

                    if (eventIdProp.enumValueIndex == typeIndex)
                    {
                        triggerMenu.AddItem(eventTypes[typeIndex], false, OnAddNewSelected, typeIndex);
                    }
                }
            }
        }

        private void OnAddNewSelected(object index)
        {
            int selectedIndex = (int)index;

            delegatesProp.arraySize += 1;
            SerializedProperty delegateEntry = delegatesProp.GetArrayElementAtIndex(delegatesProp.arraySize - 1);
            SerializedProperty eventIDProp = delegateEntry.FindPropertyRelative("eventID");
            eventIDProp.enumValueIndex = selectedIndex;
            serializedObject.ApplyModifiedProperties();
        }
    }
}