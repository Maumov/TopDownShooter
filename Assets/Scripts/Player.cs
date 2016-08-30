using UnityEngine;
using System.Collections;

[RequireComponent (typeof(PlayerController))]
[RequireComponent (typeof(GunController))]
public class Player : LivingEntity {
	public float moveSpeed = 5f;

	public Crosshair crosshairs;

	public Camera viewCamera;

	PlayerController controller;
	GunController gunController;
	// Use this for initialization
	protected  override void Start () {
		base.Start();

	}
	void Awake(){
		controller = GetComponent<PlayerController>();
		viewCamera = Camera.main;
		gunController = GetComponent<GunController>();
		FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
	}
	void OnNewWave(int waveNumber){
		health = startingHealth;
		gunController.EquipGun(waveNumber -1);
	}
	// Update is called once per frame
	void Update () {

		//move input

		Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"),0f,Input.GetAxisRaw("Vertical"));
		Vector3 moveVelocity = moveInput.normalized * moveSpeed;
		controller.Move(moveVelocity);


		//look input

		Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
		Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);
		float rayDistance;
		//RaycastHit hit;
		if(groundPlane.Raycast(ray,out rayDistance)){
			Vector3 point = ray.GetPoint(rayDistance);

			//draw the line in debug
			//Debug.DrawRay(ray.origin,point,Color.red);
			controller.LookAt(point);
			crosshairs.transform.position = point;
			crosshairs.DetectTargets(ray);
			if(((new Vector2(point.x,point.z) - new Vector2(transform.position.x,transform.position.z)).sqrMagnitude) > 1f){
				gunController.Aim(point);
			}


			//Debug.Log(new Vector2(point.x,point.z) - new Vector2(transform.position.x,transform.position.z).magnitude);
		}

		//weapon input
		if(Input.GetButton("Fire1")){
			gunController.OnTriggerHold();
		}
		if(Input.GetButtonUp("Fire1")){
			gunController.OnTriggerRelease();
		}
		if(Input.GetButtonDown("R")){
			
			gunController.Reload();
		}
		if(transform.position.y < -10f){
			TakeDamage(health);
		}
	}
	public override void Die ()
	{
		AudioManager.instance.PlaySound("Player Death", transform.position);
		base.Die ();
	}
}
