using UnityEngine;

public class NeedyShapeMemoryScript : MonoBehaviour {
    public KMNeedyModule NeedyModule;
    public KMSelectable yesButton;
    public KMSelectable noButton;
    int currentShape = 0;
    int previousShape = 0;
    string currentShapeName = "";
    string previousShapeName = "";
    public GameObject shape;
    public Material circleMat;
    public Material squareMat;
    public Material triangleMat;
    //Empty = 0, Circle = 1, Square = 2, Triangle = 3
    private string[] ShapeNames = { "blank", "circle", "square", "triangle" };
    private Material[] ShapeMats;
    bool isActive = false;
    int timeGain = 15;
    int timeMax = 99;
    public static int moduleID = 1;
    public int thisModuleID;

    void Start()
    {
        thisModuleID = moduleID++;
        ShapeMats = new Material[4]{ null, circleMat, squareMat, triangleMat };
        currentShape = UnityEngine.Random.Range(1,4);
        shape.GetComponent<MeshRenderer>().material = ShapeMats[currentShape];
    }

	void Awake () {
        GetComponent<KMNeedyModule>().OnNeedyActivation += OnNeedyActivation;
        yesButton.OnInteract += yesSolve;
        noButton.OnInteract += noSolve;
        GetComponent<KMNeedyModule>().OnTimerExpired += OnTimerExpired;
	}
    void DebugMsg(string msg)
    {
        Debug.LogFormat("[Needy Shape Memory #{0}] {1}", thisModuleID, msg);
    }
    protected void setShapeNames()
    {
        currentShapeName = ShapeNames[currentShape];
        previousShapeName = ShapeNames[previousShape];
    }
    protected bool yesSolve()
    {
        if (isActive)
        {
            GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
            if (currentShape == previousShape)
            {
                GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.NeedyActivated, transform);
                AddTime();
                setShapeNames();
                previousShape = currentShape;
                currentShape = UnityEngine.Random.Range(1, 4);
                shape.GetComponent<MeshRenderer>().material = ShapeMats[currentShape];
                DebugMsg("The previous shape was " + previousShapeName + ", and the current shape was " + currentShapeName + ". These two shapes are the same. The green button was pressed. Added 15 seconds to the timer.");
            }
            else
            {
                GetComponent<KMNeedyModule>().OnStrike();
                GetComponent<KMNeedyModule>().OnPass();
                setShapeNames();
                DebugMsg("The previous shape was " + previousShapeName + ", and the current shape was " + currentShapeName + ". These two shapes are not the same. The green button was pressed. Strike! Module temporarily disabled.");
            }
        } return true;
    }
    protected bool noSolve()
    {
        if (isActive)
        {
            GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
            if (currentShape == previousShape)
            {
                GetComponent<KMNeedyModule>().OnStrike();
                GetComponent<KMNeedyModule>().OnPass();
                setShapeNames();
                DebugMsg("The previous shape was " + previousShapeName + ", and the current shape was " + currentShapeName + ". These two shapes are the same. The red button was pressed. Strike! Module temporarily disabled.");
            }
            else
            {
                GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.NeedyActivated, transform);
                AddTime();
                setShapeNames();
                previousShape = currentShape;
                currentShape = UnityEngine.Random.Range(1, 4);
                shape.GetComponent<MeshRenderer>().material = ShapeMats[currentShape];
                DebugMsg("The previous shape was " + previousShapeName + ", and the current shape was " + currentShapeName + ". These two shapes not the same. The red button was pressed. Added 15 seconds to the timer.");
            }
        } return true;
    }
    protected void OnNeedyActivation()
    {
        isActive = true;
        previousShape = currentShape;
        currentShape = UnityEngine.Random.Range(1, 4);
        if (currentShape == 1)
        {
            shape.GetComponent<MeshRenderer>().material = circleMat;
        }
        else if (currentShape == 2)
        {
            shape.GetComponent<MeshRenderer>().material = squareMat;
        }
        else if (currentShape == 3)
        {
            shape.GetComponent<MeshRenderer>().material = triangleMat;
        }
    }
    protected void OnTimerExpired()
    {
        GetComponent<KMNeedyModule>().OnStrike();
        isActive = false;
        DebugMsg("The timer reached 0. Strike! Module temporarily disabled.");
    }
    protected void AddTime()
    {
        float time = NeedyModule.GetNeedyTimeRemaining();
        if (time > 0)
        {
            NeedyModule.SetNeedyTimeRemaining(Mathf.Min(time + timeGain, timeMax));
        }
    }
}
