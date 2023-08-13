using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//thanks to:
//https://catlikecoding.com/unity/tutorials/editor/custom-list/

[CustomEditor(typeof(ParallaxScroller))]
public class ParallaxBGEditor : Editor
{
    //GUI
    private static GUIContent applyStartOffButton = new GUIContent("Apply Start Offsets", "Applies the start offsets to all layers");
    private static GUIContent scrollButton = new GUIContent("Preview Scroll Effect", "Simulates the scrolling effect");
    private static GUIContent stopScrollButton = new GUIContent("Stop Scrolling", "Stops the scolling effect simulation");
    private bool scrolling = false;

    //Inspector texts
    private string howTo = $"Put all your background layers as children of this GameObject.\n" +
    "Every child object's renderer is collected, and the texture offset is modified by a given value to create a parallax effect.";

    //target
    private ParallaxScroller script;
    private GameObject targetObj;

    //target attributes
    private int targetChildCount = 0;
    private SerializedProperty backgroundLayers;
    private SerializedProperty speedFactor;
    private SerializedProperty zDepth;

    private void OnEnable()
    {
        script = (ParallaxScroller)target;
        targetObj = script.gameObject;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox(howTo, MessageType.Info);

        //Grab properties
        serializedObject.Update();
        backgroundLayers = serializedObject.FindProperty("backgroundLayers");
        speedFactor = serializedObject.FindProperty("speedFactor");
        zDepth = serializedObject.FindProperty("zDepth");

        //Notify programmer of child count registered
        int prevChildCount = targetChildCount;
        targetChildCount = targetObj.transform.childCount;
        script.GetLayersFromChildren();
        script.ArrangeLayersInScene();

        if (targetChildCount == 0)
        {
            string warning = "This GameObject has no children";
            EditorGUILayout.HelpBox(warning, MessageType.Warning);
        }
        else
        {
            string message = $"Registered {targetChildCount} children of this GameObject";
            EditorGUILayout.HelpBox(message, MessageType.None);
        }

        //Notify programmer of children's missing renderer components
        int missingRenderers = 0;
        foreach (BackgroundLayer layer in script.backgroundLayers)
        {
            Renderer renderer = layer.renderer;
            if (renderer == null)
            {
                missingRenderers++;
            }
        }
        if (missingRenderers > 0)
        {
            string warning = $"Warning: {missingRenderers} child ";
            warning += missingRenderers == 1 ? "object's renderer" : "objects's renderers";
            warning += " were not found.";

            EditorGUILayout.HelpBox(warning, MessageType.Warning);
        }

        //Demo options
        GUILayout.Label("Preview", EditorStyles.boldLabel);
        if (Application.isPlaying)
        {
            scrolling = false;
            string message = "Preview is disabled while the application is running.";
            EditorGUILayout.HelpBox(message, MessageType.None);
        }
        else
        {
            if (scrolling)
            {
                script.Scroll();
                Repaint();
                if (GUILayout.Button(stopScrollButton))
                {
                    scrolling = false;
                }
                string message = "Note: the scroll speed can appear much faster in the editor than in the game (the Update function isn't called as often as Repaint). Please adjust the speed while the game is running";
                EditorGUILayout.HelpBox(message, MessageType.None);
            }
            else
            {
                if (GUILayout.Button(scrollButton))
                {
                    scrolling = true;
                }
            }

            if (GUILayout.Button(applyStartOffButton))
            {
                script.ApplyStartOffsets();
            }
        }

        //Display attributes
        GUILayout.Label("General Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(speedFactor);
        if (speedFactor.floatValue == 0)
        {
            string message = "A factor of size 0 means the background does not move at all.";
            EditorGUILayout.HelpBox(message, MessageType.None);
        }
        EditorGUILayout.PropertyField(zDepth);

        //Display all layers and changeable values by background object name
        GUILayout.Label("Layer Settings", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Sort the layers from foreground to background, ie. the front-most layer as the first child.", MessageType.Info);
        for (int i = 0; i < targetObj.transform.childCount; i++)
        {
            EditorGUILayout.PropertyField(backgroundLayers.GetArrayElementAtIndex(i));
            serializedObject.ApplyModifiedProperties();
        }
        serializedObject.ApplyModifiedProperties();
    }
}