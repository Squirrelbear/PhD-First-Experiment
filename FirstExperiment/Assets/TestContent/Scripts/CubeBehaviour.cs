using UnityEngine;
using System.Collections;

public class CubeBehaviour : MonoBehaviour {

    public enum CubeType { Static, MovingToPoint, Draggable };
    public enum SelectionState { Waiting, Selecting, Selected };
    public enum SelectionMode { Time, HoverSelect };

    public Texture2D defaultTexture; // red texture
    public Texture2D selectingTexture; // orange texture
    public Texture2D selectedTexture; // green texture

    public Texture2D[] extraColours;

    public CubeType cubeType;
    public GameObject levelObject;

    public GameObject nextMoveObject;
    private Vector3 nextMoveV3;
    private float curMoveSpeed;
    public float minMoveSpeed;
    public float maxMoveSpeed;

    private SelectionState selectionState = SelectionState.Waiting;
    public bool canBeSelected;
    private float selectionProgress; // this can be private; just public to view property for now
    //private float startProgressTime;
    private float progressTime;
    public float TIME_TO_SELECT; //= 0.5f;//2.0f;
    private SelectionMode selectionMode;

    public bool objectEnabled;
    //public Bounds levelBounds;
    private Vector3 startPos;
    public bool cubeObjComplete;
    //private bool usingOculus;
    private MenuBehaviour childMenu;

	// Use this for initialization
	void Start () {
        //levelBounds = new Bounds();
        objectEnabled = false;
        GetComponent<Renderer>().material.mainTexture = defaultTexture;
        cubeObjComplete = false;
        if (cubeType == CubeType.MovingToPoint)
        {
            nextMoveV3 = nextMoveObject.transform.position;
            curMoveSpeed = maxMoveSpeed;
        }
        Transform[] children = transform.GetComponentsInChildren<Transform>();
        childMenu = null;
        foreach (Transform child in children)
        {
            if (child.gameObject.name.Contains("MenuController"))
            {
                childMenu = (MenuBehaviour)child.gameObject.GetComponent("MenuBehaviour");
                break;
            }
        }
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
            selectionProgress = progressTime / TIME_TO_SELECT;
            // update the current movement speed 
            curMoveSpeed = (maxMoveSpeed - minMoveSpeed) * (1 - selectionProgress) + minMoveSpeed;
            // if selection time is complete
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
        else if (cubeType == CubeType.MovingToPoint && curMoveSpeed < maxMoveSpeed)
        {
            curMoveSpeed = maxMoveSpeed;
        }

        // If cube is MovingToPoint 
        if (cubeType == CubeType.MovingToPoint)
        {
            // update move toward nextMoveTransform at speed curMoveSpeed
            // The step size is equal to speed times frame time.
            var step = curMoveSpeed * dTime;//Time.deltaTime;

            // Move our position a step closer to the target.
            transform.position = Vector3.MoveTowards(transform.position, nextMoveV3, step);

            // if has reached point or passed it
            if (transform.position == nextMoveV3)
            {
                // reference nextMoveObject to find the object after
                PathCornerBehaviour nextMoveScript = (PathCornerBehaviour)nextMoveObject.GetComponent("PathCornerBehaviour");
                // set nextMoveObject to the next object
                nextMoveObject = nextMoveScript.nextCorner;
                // update the transform to that of the new object
                nextMoveV3 = nextMoveObject.transform.position;

            }
        }
        else if (cubeType == CubeType.Draggable)
        {
            if (selectionState == SelectionState.Selected)
            {
                Ray ray;
                //if (usingOculus)
                //{
                ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                //}
                //else
                //{
                //    ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                //}
                // create a plane at 0,0,0 whose normal points to +Y:
                Plane hPlane = new Plane(Vector3.up, Vector3.zero);
                // Plane.Raycast stores the distance from ray.origin to the hit point in this variable:
                float distance = 0;
                // if the ray hits the plane...
                if (hPlane.Raycast(ray, out distance))
                {
                    // get the hit point:
                    Vector3 newPoint = ray.GetPoint(distance);
                    newPoint.y = 0.3f;
                    /*if (newPoint.x < levelBounds.min.x) newPoint.x = levelBounds.min.x;
                    if (newPoint.x > levelBounds.max.x) newPoint.x = levelBounds.max.x;
                    if (newPoint.z < levelBounds.min.z) newPoint.z = levelBounds.min.z;
                    if (newPoint.x > levelBounds.max.z) newPoint.z = levelBounds.max.z;*/
                    if (newPoint.x < -0.437f * 12) newPoint.x = -0.437f * 12;
                    if (newPoint.x > 0.437f * 12) newPoint.x = 0.437f * 12;
                    if (newPoint.z < -0.406f * 8) newPoint.z = -0.406f * 8;
                    if (newPoint.z > 0.406f * 8) newPoint.z = 0.406f * 8;
                    gameObject.transform.position = newPoint;
                }

                /*Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 point = ray.origin + (ray.direction * 10f);
                point.y = 0.5f;
                print(point.y);
     
                gameObject.transform.position = point;*/
            }
        }

        if (childMenu != null)
        {
            childMenu.performUpdate(dTime);
        }
    }

    public void beginSelection(SelectionMode mode) {
        //print("begin selection");
        if (canBeSelected && selectionState == SelectionState.Waiting) {
            GetComponent<Renderer>().material.mainTexture = selectingTexture;
            selectionState = SelectionState.Selecting;
            //startProgressTime = Time.time;
            progressTime = 0;
            selectionProgress = 0;
            selectionMode = mode;
        }
    }

    public void cancelSelection() {
        //print("cancel selection");
        if (selectionState != SelectionState.Selected)
        {
            removeSelection();
        }
    }

    public void removeSelection()
    {
        //print("remove selection");
        GetComponent<Renderer>().material.mainTexture = defaultTexture;
        selectionState = SelectionState.Waiting;
    }


    public void completeSelection()
    {
        //print("complete selection");
        if (selectionState == SelectionState.Selected)
        {
            removeSelection();
        }
        else
        {
            GetComponent<Renderer>().material.mainTexture = selectedTexture;
            selectionState = SelectionState.Selected;
        }
        //gameObject.SetActive(false);
        // Notify level object
    }

    public SelectionState getSelectionState()
    {
        return selectionState;
    }

    public bool getObjComplete()
    {
        return cubeObjComplete;
    }

    public void setEnabledState(bool newState)
    {
        objectEnabled = newState;

        if (cubeType == CubeType.MovingToPoint)
        {
            nextMoveV3 = nextMoveObject.transform.position;
            curMoveSpeed = maxMoveSpeed;
        }

        if (newState)
        {
            GetComponent<Renderer>().material.mainTexture = defaultTexture;
            selectionState = SelectionState.Waiting;
            startPos = gameObject.transform.position;

            /*GameObject oculus = GameObject.Find("OVRCameraRig");
            if (oculus == null)
            {
                usingOculus = false;
            }
            else
            {
                usingOculus = oculus.activeSelf;
            }*/
        }
    }
    
    public bool getObjEnabled()
    {
        return objectEnabled;
    }


    void OnCollisionEnter(Collision other)
    {
        if (!objectEnabled)
        {
            return;
        }

        //print("Collision!");
        //print("Collided Cube Script");
        if (other.collider.name.StartsWith("Wall"))
        {
            gameObject.transform.position = startPos;
            GetComponent<Renderer>().material.mainTexture = defaultTexture;
            selectionState = SelectionState.Waiting;
        }
        else if (other.collider.transform.name.StartsWith("EndPoint"))
        {
            cubeObjComplete = true;
            gameObject.transform.position = startPos;
            GetComponent<Renderer>().material.mainTexture = defaultTexture;
            selectionState = SelectionState.Waiting;
        }
    }

    public void setExtraColourTex(int id)
    {
        if (id >= 0 && id < extraColours.Length)
        {
            defaultTexture = extraColours[id];
            GetComponent<Renderer>().material.mainTexture = defaultTexture;
            selectedTexture = defaultTexture;
        }
    }

    /*void OnTriggerEnter(Collider other)
    {
        print("Collision!");
        if (other.name.StartsWith("Wall"))
        {
            gameObject.transform.position = startPos;
            GetComponent<Renderer>().material.mainTexture = defaultTexture;
            selectionState = SelectionState.Waiting;
        }
    }*/
    /*public void setBounds(Bounds bounds)
    {
        this.levelBounds = bounds;
    }*/
}
