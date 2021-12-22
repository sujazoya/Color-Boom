using UnityEngine;
using System.Collections;
using UnityEditorInternal;
using UnityEditor;

[CustomEditor(typeof(LevelData))]
public class LevelEditor : Editor
{

    private ReorderableList list;
    private LevelData levelData;



    private void OnEnable()
    {

        levelData = (LevelData)target;

        list = new ReorderableList(serializedObject, serializedObject.FindProperty("items"), true, true, true, true);

        list.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "    Element                                          Speed        Offset         Distance"); };

        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {

            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            EditorGUI.PropertyField(new Rect(40, rect.y, 200, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("elementHolder"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(250, rect.y, 60, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("speed"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(320, rect.y, 60, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("offset"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(390, rect.y, 60, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("distance"), GUIContent.none);

        };


    }


    public override void OnInspectorGUI()
    {


        GUILayout.BeginHorizontal();
        GUILayout.Label("Start Position");
        levelData.startPos = EditorGUILayout.FloatField(levelData.startPos, GUILayout.Width(50));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Default Speed");
        levelData.defSpeed = EditorGUILayout.FloatField(levelData.defSpeed, GUILayout.Width(50));
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.Label("Default Camera Distance");
        levelData.defDistance = EditorGUILayout.FloatField(levelData.defDistance, GUILayout.Width(50));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Time");
        levelData.levelTime = EditorGUILayout.FloatField(levelData.levelTime, GUILayout.Width(50));
        GUILayout.EndHorizontal();


        if (GUILayout.Button("Generate Random"))
        {
            if (EditorUtility.DisplayDialog("Warning!", "Are you sure you want to create new set ?", "Yes", "No"))
            {


                if (levelData.randomElementCount > 0)
                {

                    levelData.items.Clear();

                    for (int i = 0; i < levelData.randomElementCount; i++)
                    {

                        LevelData.LevelItem newItem = new LevelData.LevelItem();

                        if (i == 0)
                        {
                            newItem.elementHolder = getRandomElement(LevelData.ObstacleCategory.Simple);
                        }
                        else if (i > 0)
                        {
                            newItem.elementHolder = getRandomElement(levelData.randomElementCategory);
                        }



                        if (i == 0)
                        {
                            newItem.distance = 15;
                        }
                        if (i == 1)
                        {
                            newItem.offset = 2;
                        }

                        levelData.items.Add(newItem);


                        if (i < levelData.randomElementCount - 1)
                        {
                            LevelData.LevelItem newColorChanger = new LevelData.LevelItem();
                            newColorChanger.elementHolder = getColorChanger();


                            if (i == 0)
                            {
                                newColorChanger.offset = 8;
                                newColorChanger.distance = 28;
                            }

                            levelData.items.Add(newColorChanger);

                        }



                    }

                    EditorUtility.SetDirty(target);
                }

            }
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("Random Element Category");
        levelData.randomElementCategory = (LevelData.ObstacleCategory)EditorGUILayout.EnumPopup(levelData.randomElementCategory, GUILayout.Width(100));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Random Element Count");
        levelData.randomElementCount = EditorGUILayout.IntField(levelData.randomElementCount, GUILayout.Width(100));
        GUILayout.EndHorizontal();


        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();

    }


    private GameObject getRandomElement(LevelData.ObstacleCategory cat)
    {
        EditorAccess infinitController = GameObject.Find("EditorAccess").GetComponent<EditorAccess>();

        switch (cat)
        {
            case LevelData.ObstacleCategory.Simple:
                return infinitController.obstaclesSimple[Random.Range(0, infinitController.obstaclesSimple.Length)];
            case LevelData.ObstacleCategory.Cat1:
                return infinitController.obstaclesCat1[Random.Range(0, infinitController.obstaclesCat1.Length)];
            case LevelData.ObstacleCategory.Cat2:
                return infinitController.obstaclesCat2[Random.Range(0, infinitController.obstaclesCat2.Length)];
            case LevelData.ObstacleCategory.Cat3:
                return infinitController.obstaclesCat3[Random.Range(0, infinitController.obstaclesCat3.Length)];
        }

        return null;

    }


    private GameObject getColorChanger()
    {
        EditorAccess infinitController = GameObject.Find("EditorAccess").GetComponent<EditorAccess>();
        return infinitController.colorChanger;
    }

}
