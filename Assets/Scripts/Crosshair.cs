using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour {

	public LayerMask targetMask;
	public SpriteRenderer dot;
	public float rotationSpeed;
	public Color dotHighlightColour;
	Color originalColor;


	// Use this for initialization
	void Start () {
		Cursor.visible = false;
		originalColor = dot.color;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
	}
	public void DetectTargets(Ray ray){
		if(Physics.Raycast(ray,100,targetMask)){
			dot.color = dotHighlightColour;
		}else{
			dot.color = originalColor;
		}
	}
}
