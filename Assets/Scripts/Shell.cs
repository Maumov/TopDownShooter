using UnityEngine;
using System.Collections;

public class Shell : MonoBehaviour {

	public Rigidbody myRigidBody;
	public float forceMin;
	public float forceMax;

	float lifetime = 4f;
	float fadetime = 2f;


	// Use this for initialization
	void Start () {
		float force = Random.Range(forceMin,forceMax);
		myRigidBody.AddForce(transform.right * force);
		myRigidBody.AddTorque(Random.insideUnitSphere * force);
		StartCoroutine(Fade());
	}
	
	IEnumerator Fade(){
		yield return new WaitForSeconds(lifetime);

		float percent = 0f;
		float fadeSpeed = 1 / fadetime;
		Material mat = GetComponent<Renderer>().material;
		Color initialColour = mat.color;

		while(percent < 1){
			percent += Time.deltaTime * fadeSpeed;
			mat.color = Color.Lerp(initialColour,Color.clear,percent);
			yield return null;
		}

		Destroy(gameObject);
	}
}
