using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Debug class for drawing the different type of boxes
public class DebugBoxManager : MonoBehaviour
{
    private static Texture2D _texture;
    private static GUIStyle _style;

    public bool debugDraw;

    public Color physicsBoxColor;
    [Range(0.0f, 1.0f)]
    public float physicsBoxOpacity = 1.0f;
    public bool showPhysicsBox = true;
    [Range(0, 2)]
    public int physicsBoxOrder = 0;

    [Space(30)]

    public Color hurtBoxColor;
    [Range(0.0f, 1.0f)]
    public float hurtBoxOpacity = 1.0f;
    public bool showHurtBox = true;
    [Range(0, 2)]
    public int hurtBoxOrder = 1;

    [Space(30)]

    public Color hitBoxColor;
    [Range(0.0f, 1.0f)]
    public float hitBoxOpacity = 1.0f;
    public bool showHitBox = true;
    [Range(0, 2)]
    public int hitBoxOrder = 2;

    private List<Box>[] boxes;

    public static DebugBoxManager instance;

    void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);

        boxes = new List<Box>[3];

        for (int i = 0; i < 3; i++)
        {
            boxes[i] = new List<Box>();
        }
    }

    private void Update()
    {
        for (int i = 0; i < 3; i++)
        {
            if (boxes[i] == null)
            {
                boxes[i] = new List<Box>();
            }
        }
    }

    void OnGUI()
    {
        if (debugDraw)
        {
            if (boxes != null)
            {
                foreach (List<Box> boxGroup in boxes)
                {
                    if (boxGroup.Count > 0.0f)
                    {
                        foreach (Box box in boxGroup)
                        {
                            if (box && box.isActiveAndEnabled && box.isOpen)
                            {
                                Color boxColor = Color.magenta;
                                float opacity = 1.0f;

                                if (box.gameObject.layer == 7 || box.gameObject.layer == 9)
                                {
                                    if (!showPhysicsBox)
                                    {
                                        continue;
                                    }
                                    boxColor = physicsBoxColor;
                                    opacity = physicsBoxOpacity;
                                }
                                else if (box.gameObject.layer == 10)
                                {
                                    if (!showHurtBox)
                                    {
                                        continue;
                                    }
                                    boxColor = hurtBoxColor;
                                    opacity = hurtBoxOpacity;
                                }
                                else if (box.gameObject.layer == 11)
                                {
                                    if (!showHitBox)
                                    {
                                        continue;
                                    }
                                    boxColor = hitBoxColor;
                                    opacity = hitBoxOpacity;
                                }

                                boxColor.a = opacity;
                                BoxCollider2D boxCollider = box.GetComponent<BoxCollider2D>();
                                Vector2 origin;
                                Vector2 extent;
                                if (boxCollider)
                                {
                                    origin = Camera.main.WorldToScreenPoint(new Vector2(boxCollider.bounds.min.x, boxCollider.bounds.max.y));
                                    extent = Camera.main.WorldToScreenPoint(new Vector2(boxCollider.bounds.max.x, boxCollider.bounds.min.y));
                                }
                                else
                                {
                                    origin = Camera.main.WorldToScreenPoint(new Vector2(box.transform.position.x - (box.boxSize.x)/2, box.transform.position.y + (box.boxSize.y)/2));
                                    extent = Camera.main.WorldToScreenPoint(new Vector2(box.transform.position.x + (box.boxSize.x)/2, box.transform.position.y - (box.boxSize.y)/2));
                                }
                                DrawGUIRect(new Rect(new Vector2(origin.x, Screen.height - origin.y), new Vector2(extent.x - origin.x, origin.y - extent.y)), -box.transform.eulerAngles.z, boxColor);
                            }
                        }
                    }
                }
            }
        }
    }

    public static void DrawGUIRect(Rect position, float angle, Color color)
    {
        if (_texture == null)
        {
            _texture = new Texture2D(1, 1);
        }

        if (_style == null)
        {
            _style = new GUIStyle();
        }
        _texture.SetPixel(0, 0, color);
        _texture.wrapMode = TextureWrapMode.Repeat;
        _texture.Apply();

        Matrix4x4 originalMatrix = GUI.matrix;

        float xValue = ((position.x + position.width / 2.0f));
        float yValue = ((position.y + position.height / 2.0f));
        GUIUtility.RotateAroundPivot(angle, new Vector2(xValue, yValue));

        _style.normal.background = _texture;
        GUI.Box(new Rect(position.x, position.y, position.width, position.height), GUIContent.none, _style);

        GUI.matrix = originalMatrix;
    }

    public void AddToDebugBoxes(Box box)
    {
        if (box)
        {
            int boxGroupIndex = GetBoxGroupIndex(box);
            if (boxes[boxGroupIndex] != null)
            {
                if (!boxes[boxGroupIndex].Contains(box))
                {
                    boxes[boxGroupIndex].Add(box);
                }
            }
        }
    }

    public void RemoveFromDebugBoxes(Box box)
    {
        if (box)
        {
            int boxGroupIndex = GetBoxGroupIndex(box);

            if (boxes[boxGroupIndex] != null)
            {
                if (boxes[boxGroupIndex].Contains(box))
                {
                    boxes[boxGroupIndex].Remove(box);
                }
            }
        }
    }

    private int GetBoxGroupIndex(Box box)
    {
        int boxGroupIndex = 0;
        if (box.gameObject.layer == 7 || box.gameObject.layer == 9)
        {
            boxGroupIndex = physicsBoxOrder;
        }
        else if (box.gameObject.layer == 10)
        {
            boxGroupIndex = hurtBoxOrder;
        }
        else if (box.gameObject.layer == 11)
        {
            boxGroupIndex = hitBoxOrder;
        }

        return boxGroupIndex;
    }
}
