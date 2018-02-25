using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchSpin : MonoBehaviour {

	float speed = 0.1f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            // Get movement of the finger since last frame
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;

            transform.Rotate(0, touchDeltaPosition.x * speed, 0);
        }
		if (Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved)
        {
            
            Vector2 touch0DeltaPosition = Input.GetTouch(0).deltaPosition;
			Vector2 touch1DeltaPosition = Input.GetTouch(1).deltaPosition;

			float delta = (touch0DeltaPosition.y + touch1DeltaPosition.y)/2;
            transform.localScale = transform.localScale + Vector3.one * delta * 0.01f;
        }

	}
}
