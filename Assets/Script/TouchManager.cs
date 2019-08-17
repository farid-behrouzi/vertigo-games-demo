using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchManager : Helper {

    private bool validTouch;
    private GridSystem gridSystem;
    private Vector2 touchStartPosition;
    private Hexagon selectedHexagon;
    private int selectionStatus;

    RaycastHit hit;

    void Start() {
        gridSystem = GridSystem.instance;
    }

    void Update()
    {
        if (inputAvailable() && Input.touchCount > zero)
        {
            if (Input.GetMouseButtonDown(0))
            {

                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    gridSystem.findThreeValidHexagons(hit.collider.gameObject.GetComponent<Hexagon>(), selectionStatus);
                    selectedHexagon = hit.collider.gameObject.GetComponent<Hexagon>();
                    selectionStatus = (++selectionStatus) % selectionCounter;
                }
            }
            TouchDetection();
            CheckSelection(hit.collider);
            CheckRotation();
        }
    }

    private bool inputAvailable() {
        return (!gameOver && !rotating && !exploding && !reassigning && !producing && !checkingExplosion);
    }

    private void TouchDetection()
    {
        if (Input.GetTouch(zero).phase == TouchPhase.Began)
        {
            validTouch = true;
            Debug.Log("Touch Validation: " + validTouch);
            touchStartPosition = Input.GetTouch(zero).position;
        }
    }



    private void CheckSelection(Collider collider)
    {
        if (collider != null && collider.transform.tag == hexagonTag)
        {
            if (Input.GetTouch(zero).phase == TouchPhase.Ended && validTouch)
            {
                Debug.Log("Check Selection");
                validTouch = false;
//                GridManagerObject.Select(collider);
            }
        }
    }



    //This function is for realizing rotation path
    private void CheckRotation()
    {
        if (Input.GetTouch(zero).phase == TouchPhase.Moved && validTouch)
        {
            Debug.Log("CheckRotation");
            Vector2 touchCurrentPosition = Input.GetTouch(zero).position;
            float distanceX = touchCurrentPosition.x - touchStartPosition.x;
            float distanceY = touchCurrentPosition.y - touchStartPosition.y;


            if ((Mathf.Abs(distanceX) > hexagonSlideDistance || Mathf.Abs(distanceY) > hexagonSlideDistance) && selectedHexagon != null)
            {
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(selectedHexagon.transform.position);

                bool triggerOnX = Mathf.Abs(distanceX) > Mathf.Abs(distanceY);
                bool swipeRightUp = triggerOnX ? distanceX > zero : distanceY > zero;
                bool touchThanHex = triggerOnX ? touchCurrentPosition.y > screenPosition.y : touchCurrentPosition.x > screenPosition.x;
                bool clockWise = triggerOnX ? swipeRightUp == touchThanHex : swipeRightUp != touchThanHex;

                validTouch = false;
                StartCoroutine(gridSystem.swapHexagons(clockWise));
            }
        }
    }
}
