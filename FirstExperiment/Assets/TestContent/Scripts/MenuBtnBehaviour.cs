using UnityEngine;
using System.Collections;

public class MenuBtnBehaviour : MonoBehaviour {

    public enum SelectionState { Waiting, Selecting, Selected };
    public enum SelectionMode { Time, HoverSelect };

    public Texture2D defaultTexture; // red texture
    public Texture2D selectingTexture; // orange texture
    public Texture2D selectedTexture; // green texture

    private SelectionState selectionState = SelectionState.Waiting;
    public bool canBeSelected;
    private float selectionProgress; // this can be private; just public to view property for now
    //private float startProgressTime;
    private float progressTime;
    public float TIME_TO_SELECT; //= 0.5f;//2.0f;
    private SelectionMode selectionMode;

    public bool objectEnabled;
    public int menuAction;
    public string buttonAction;

	// Use this for initialization
	void Start () {
        objectEnabled = false;
        //gameObject.SetActive(false);
        GetComponent<Renderer>().enabled = false;
        buttonAction = "";
        menuAction = 0;
	}
	
	// Update is called once per frame
	void Update () {
        //performUpdate(Time.deltaTime);
	}

    public void performUpdate(float dTime)
    {
        if (!objectEnabled)
        {
            return;
        }

        // if cube is selecting
        if (selectionState == SelectionState.Selecting)
        {
            // update the selection progress
            //selectionProgress = (Time.time - startProgressTime) / TIME_TO_SELECT;
            progressTime += dTime;
            selectionProgress = progressTime / TIME_TO_SELECT;// if selection time is complete
            if (selectionProgress >= 1)
            {
                if (selectionMode == SelectionMode.Time)
                {
                    completeSelection();
                }
                else
                {
                    selectionProgress = 1;
                }
            }
        }
    }

    public void beginSelection(SelectionMode mode)
    {
        if (canBeSelected && selectionState == SelectionState.Waiting)
        {
            GetComponent<Renderer>().material.mainTexture = selectingTexture;
            selectionState = SelectionState.Selecting;
            //startProgressTime = Time.time;
            progressTime = 0;
            selectionProgress = 0;
            selectionMode = mode;
        }
    }

    public void cancelSelection()
    {
        if (selectionState != SelectionState.Selected)
        {
            GetComponent<Renderer>().material.mainTexture = defaultTexture;
            selectionState = SelectionState.Waiting;
        }
    }

    public void completeSelection()
    {
        GetComponent<Renderer>().material.mainTexture = selectedTexture;
        selectionState = SelectionState.Selected;
        //gameObject.SetActive(false);
        // Notify level object
    }

    public void setButtonContext(Texture2D defaultTexture, Texture2D selectingTexture, Texture2D selectedTexture, int menuAction, string buttonAction)
    {
        this.menuAction = menuAction;
        this.buttonAction = buttonAction;
        this.defaultTexture = defaultTexture;
        this.selectedTexture = selectedTexture;
        this.selectingTexture = selectingTexture;
        GetComponent<Renderer>().material.mainTexture = defaultTexture;
        selectionState = SelectionState.Waiting;
    }

    public SelectionState getSelectionState()
    {
        return selectionState;
    }

    public void setEnabledState(bool newState)
    {
        objectEnabled = newState;

        if (newState)
        {
            GetComponent<Renderer>().material.mainTexture = defaultTexture;
            selectionState = SelectionState.Waiting;
        }
    }
}
