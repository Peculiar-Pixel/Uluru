using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class ParallaxScroller : MonoBehaviour
{
    [SerializeField] public List<BackgroundLayer> backgroundLayers = new List<BackgroundLayer>();
    [SerializeField] private float speedFactor = 0.01f;

    [Tooltip("The z value from which your background layers will be sorted. Should be more than 0 for background elements, and less than zero for foreground elemts, but always more than the camera's z position")]
    [SerializeField] private float zDepth = 20;

    // Start is called before the first frame update
    private void Start()
    {
        ApplyStartOffsets();
        ArrangeLayersInScene();
    }

    private void Update()
    {
        Scroll();
    }

    public void ApplyStartOffsets()
    {
        foreach (BackgroundLayer layer in backgroundLayers)
        {
            layer.renderer.sharedMaterial.mainTextureOffset = layer.startOffset;
        }
    }

    public void ArrangeLayersInScene()
    {
        foreach (BackgroundLayer layer in backgroundLayers)
        {
            layer.renderer.gameObject.transform.position = new Vector3(0, 0, zDepth + layer.depthIndex);
        }
    }

    public void Scroll()
    {
        foreach (BackgroundLayer layer in backgroundLayers)
        {
            if (layer.renderer != null)
            {
                layer.renderer.sharedMaterial.mainTextureOffset += new Vector2(layer.scrollSpeed.x * Time.deltaTime, layer.scrollSpeed.y * Time.deltaTime) * speedFactor;
            }
        }
    }

    public void GetLayersFromChildren()
    {
        List<BackgroundLayer> newLayers = new List<BackgroundLayer>();

        for (int i = 0; i < transform.childCount; i++)
        {
            //generate a layer for child
            BackgroundLayer newLayer = new BackgroundLayer(transform.GetChild(i).gameObject, i);

            //check if renderer exists in previous list (renderer is unique, and thus the perfect attribute to compare with)
            BackgroundLayer duplicate = backgroundLayers.FirstOrDefault(l => l.renderer == newLayer.renderer);
            if (newLayer.renderer == duplicate.renderer)
            {
                duplicate.depthIndex = i;
                newLayers.Add(duplicate);
            }
            else
            {
                newLayers.Add(newLayer);
            }
        }
        backgroundLayers = newLayers;
    }
}

[System.Serializable]
public struct BackgroundLayer
{
    public BackgroundLayer(GameObject obj, int depth)
    {
        name = obj.name;
        renderer = obj.GetComponent<Renderer>();
        startOffset = Vector2.zero;
        scrollSpeed = Vector2.zero;
        depthIndex = depth;
    }

    [HideInInspector] public string name;
    [SerializeField] public Vector2 startOffset;
    [SerializeField] public Vector2 scrollSpeed;
    [HideInInspector] public Renderer renderer;
    [HideInInspector] public int depthIndex;
}