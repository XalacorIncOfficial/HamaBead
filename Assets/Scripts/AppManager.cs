using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class AppManager : MonoBehaviour
{
    public enum ToolStateValues { BRUSH, EREASE, CLEAR, FILL, PATTERN }
    public ToolStateValues toolState;

    public RectTransform mainCanvas, parentBoard;

    public GameObject bead;
    public Transform board;

    public Color currentColor;

    public List<GameObject> allBeads;

    public GameObject showCurrentColor;

    public GameObject currentToolName;

    [System.Serializable]
    public class PatternProperties
    {
        public List<Vector2> position;
    }

    public List<PatternProperties> pattern;
    public int currentPattern;


    // Start is called before the first frame update
    void Start()
    {
        float sizeX = mainCanvas.sizeDelta.x;
        float sizeY = mainCanvas.sizeDelta.y;

        float boardLeft = parentBoard.offsetMin.x;
        float boardRight = parentBoard.offsetMax.x;

        float boardUp = parentBoard.offsetMin.y;
        float boardDown = parentBoard.offsetMax.y;

        // Maße des ParentBoards
        float totalBoardX = sizeX - (boardLeft + Mathf.Abs(boardRight));
        float totalBoardY = sizeY - (boardUp + Mathf.Abs(boardDown));

        // Summe der Beads
        int beadsX = (int)(totalBoardX / 20);
        int beadsY = (int)(totalBoardY / 20);

        int totalBeads = beadsX * beadsY;

        // Ausgabe der Beads
        for (int i = 0; i < totalBeads; i++)
        {
            GameObject newBead = Instantiate(bead, board);                              // Instanzierung (was, wo); CAVE: hier public generieren und in Unity im AppManager zuweisen

            Image beadImage = newBead.GetComponent<Image>();

            EventTrigger newTrigger = newBead.AddComponent<EventTrigger>();             // CAVE: using UnityEngine.EventSystems; wird hier benötigt; 
            EventTrigger.Entry pointerDown = new EventTrigger.Entry();                  // Erzeugung des PointerDown
            pointerDown.eventID = EventTriggerType.PointerDown;
            pointerDown.callback.AddListener(delegate { PaintBead(beadImage); });
            newTrigger.triggers.Add(pointerDown);

            EventTrigger.Entry pointerEnter = new EventTrigger.Entry();                 // Erzeugung des PointerEnter / Steuerung für`s Smartphone
            pointerEnter.eventID = EventTriggerType.PointerEnter;
            pointerEnter.callback.AddListener(delegate { PaintBead(beadImage); });
            newTrigger.triggers.Add(pointerEnter);

            allBeads.Add(newBead);

        }
    }

    private void PaintBead(Image _bead)                                                 // CAVE: using UnityEngine.UI; wird hier benötigt
    {
        if (toolState == ToolStateValues.BRUSH)
        {
            // Beads bemalen
            if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0))
            {
                _bead.color = currentColor;
            }
        }
        else if (toolState == ToolStateValues.EREASE)
        {
            if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0))
            {
                _bead.color = Color.white;
            }
        }
        else if (toolState == ToolStateValues.FILL)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Color savedColor = _bead.color;                                         // CAVE: Die Farbe, dich ich behalten möchte, muss ich vorher abspeichern -> Color savedColor

                for (int i = 0; i < allBeads.Count; i++)
                {
                    if (allBeads[i].GetComponent<Image>().color == savedColor)
                    {
                        allBeads[i].GetComponent<Image>().color = currentColor;
                    }
                }
                SetBrush();
            }
        }
        else if (toolState == ToolStateValues.PATTERN)
        {
            if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0))
            {
                for (int i = 0; i < pattern[currentPattern].position.Count; i++)
                {
                    float beadX = _bead.transform.localPosition.x + pattern[currentPattern].position[i].x * 20;
                    float beadY = _bead.transform.localPosition.y + pattern[currentPattern].position[i].y * 20;

                    SetBead(new Vector2((int)beadX, beadY));
                }
            }
        }
    }

    private void SetBead(Vector2 _position)
    {
        for (int i = 0; i < allBeads.Count; i++)
        {
            int positionX = (int)allBeads[i].transform.localPosition.x;                                 // Fkt. besagt: suche nach beiden Positionen (X, Y) die wir in Unity als Position definiert haben
            int positionY = (int)allBeads[i].transform.localPosition.y;

            if (new Vector2(positionX, positionY) == _position)                                         // wenn beides übereinstimmt
            {
                allBeads[i].GetComponent<Image>().color = currentColor;                                 // befülle das Bild mit der aktuellen Farbe
                break;
            }
        }
    }

    public void SetBrush()
    {
        toolState = ToolStateValues.BRUSH;
        //currentToolName.text = "BRUSH";
    }

    public void SetErease()
    {
        toolState = ToolStateValues.EREASE;
        //currentToolName.text = "EREASE";
    }

    public void SetClear()
    {
        toolState = ToolStateValues.CLEAR;
        //currentToolName.text = "CLEAR";

        for (int i = 0; i < allBeads.Count; i++)
        {
            allBeads[i].GetComponent<Image>().color = Color.white;
        }
        Invoke("SetBrush", 0.1f);
        //SetBrush();
    }

    public void SetFill()
    {
        toolState = ToolStateValues.FILL;
        //currentToolName.text = "FILL";
    }

    public void SetPattern(int _currentPattern)
    {
        toolState = ToolStateValues.PATTERN;
        //currentToolName.text = "PATTERN " + currentPattern.ToString();
        currentPattern = _currentPattern;
    }

    public void ChangeColor(Image _toolColor)
    {
        currentColor = _toolColor.color;
    }


    // Update is called once per frame
    void Update()
    {
        showCurrentColor.GetComponent<Image>().color = currentColor;
    }
}
