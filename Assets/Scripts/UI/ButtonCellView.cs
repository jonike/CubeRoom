using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Sorumi.UI;

[RequireComponent(typeof(CellView))]
public class ButtonCellView : MonoBehaviour {

    // Use this for initialization
    public int count;
    private CellView cellView;

	private ScrollRect scrollRect;
    void Start()
    {
		scrollRect = GetComponent<ScrollRect>();

        cellView = GetComponent<CellView>();
        cellView.CountOfCell = CountOfCell;
        cellView.CellAtIndex = CellAtIndex;
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetMouseButton(0))
        //  {
        //      //Get the mouse position on the screen and send a raycast into the game world from that position.
        //      Vector2 worldPoint = camera.ScreenToWorldPoint(Input.mousePosition);
        //      RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        // 	 Debug.Log(1);

        //      //If something was hit, the RaycastHit2D.collider will not be null and the the object must have the "Monkey" tag and target has to be null
        //     //  if (hit.collider != null && hit.collider.tag == "Monkey" && target == null)
        //     //  {
        //     //      target = hit.collider.transform; // Sets the target to be the transform that was hit
        //     //  }

        //     //  if (target != null)
        //     //  {
        //     //      target.position = worldPoint; // Moves target with the mouse
        //     //  }
        //  }

        //  if (Input.GetMouseButtonUp(0))
        //  {
        //     //  target = null; // Sets the target to null again
        // 	Debug.Log(2);
        //  }
    
    }

    void CellAtIndex(GameObject gameObject, int index)
    {
		// gameObject.GetComponent<ItemButton>().parentScroll = scrollRect;
        gameObject.transform.Find("Text").GetComponent<Text>().text = index.ToString();
    }
    int CountOfCell()
    {
        return count;
    }
}
