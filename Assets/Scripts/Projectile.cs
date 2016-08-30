using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {


	public LayerMask collisionMask;

	public Color trailColour;

	float speed = 10f;
	float damage = 1f;

	float lifetime = 3f;
	float skinWidth = .1f;

	public void SetSpeed(float sp){
		speed = sp;
	}

	// Use this for initialization
	void Start () {
		Destroy(gameObject,lifetime);

		Collider[] intitalCollisions = Physics.OverlapSphere(transform.position, .1f,collisionMask);
		if(intitalCollisions.Length > 0){
			OnHitObject(intitalCollisions[0],transform.position);
		}

		GetComponent<TrailRenderer>().material.SetColor("_TintColor",trailColour);

	}
	
	// Update is called once per frame
	void Update () {
		float moveDistance = speed * Time.deltaTime;
		CheckCollisions(moveDistance);
		transform.Translate(Vector3.forward * moveDistance);


	}
	void CheckCollisions(float moveDistance){
		Ray ray = new Ray(transform.position,transform.forward);
		RaycastHit hit;
		if(Physics.Raycast(ray,out hit, moveDistance + skinWidth,collisionMask,QueryTriggerInteraction.Collide)){
			OnHitObject(hit.collider, hit.point);
		}
	}


	void OnHitObject(Collider c, Vector3 hitPoint){
		IDamageable damageableObject = c.GetComponent<IDamageable>();
		if(damageableObject != null){
			damageableObject.TakeHit(damage,hitPoint,transform.forward);
		}
		Debug.Log(c.gameObject.name);
		GameObject.Destroy(gameObject);
	}
}
