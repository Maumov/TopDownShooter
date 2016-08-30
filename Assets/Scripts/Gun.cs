using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

	public enum FireMode { Auto, Burst, Single}
	public FireMode firemode;

	public Transform[] projectileSpawn;
	public Projectile projectile;
	public float msBetweenShots = 100f;
	public float muzzleVelocity = 35f;

	public int burstCount;



	public Transform shell;
	public Transform shellEjection;
	MuzzleFlash muzzleFlash;
	float nextShotTime;

	bool triggerReleasedSinceLastShot;
	int shotsRemainingInBurst;


	void Start(){
		muzzleFlash = GetComponent<MuzzleFlash>();
		shotsRemainingInBurst = burstCount;
	}



	void Shoot(){
		if(Time.time > nextShotTime){
			if(firemode ==FireMode.Burst){
				if(shotsRemainingInBurst == 0){
					return;
				}
				shotsRemainingInBurst--;
			}else if(firemode ==FireMode.Single){
				if(!triggerReleasedSinceLastShot){
					return;
				}
			}
			for(int i = 0; i < projectileSpawn.Length; i++){
				nextShotTime = Time.time + msBetweenShots / 1000f;
				Projectile newProjectile = Instantiate (projectile,projectileSpawn[i].position,projectileSpawn[i].rotation) as Projectile;
				newProjectile.SetSpeed(muzzleVelocity);


			}
			Instantiate(shell,shellEjection.position,shellEjection.rotation);
			muzzleFlash.Activate();	
		}


	}
	public void OnTriggerHold(){
		Shoot();
		triggerReleasedSinceLastShot = false;
	}
	public void OnTriggerRelease(){
		triggerReleasedSinceLastShot = true;
		shotsRemainingInBurst = burstCount;
	}


}
