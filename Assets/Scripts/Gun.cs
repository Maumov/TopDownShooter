﻿using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {
	
	public enum FireMode { Auto, Burst, Single}
	[Header ("Main")]
	public FireMode firemode;
	public Transform[] projectileSpawn;
	public Projectile projectile;
	public float msBetweenShots = 100f;
	public float muzzleVelocity = 35f;
	public int burstCount;
	public int projectilesPerMag;
	public float reloadTime = .3f;
	[Header("Recoil")]
	public Vector2 kickMinMax = new Vector2( .05f, .2f);
	public Vector2 recoilAngleMinMax = new Vector2(3f,5f);
	public float recoilMoveSettleTime = .1f;
	public float recoilRotationSettleTime = .1f;

	[Header("Effects")]
	public Transform shell;
	public Transform shellEjection;
	public AudioClip shootAudio;
	public AudioClip reloadAudio;

	MuzzleFlash muzzleFlash;
	float nextShotTime;
	bool triggerReleasedSinceLastShot = true;
	int shotsRemainingInBurst;
	int projectilesRemainingInMag;
	bool isReloading;

	Vector3 recoilSmoothDampVelocity;
	float recoilRotSmoothDampVelocity;
	float recoilAngle;

	void Start(){
		muzzleFlash = GetComponent<MuzzleFlash>();
		shotsRemainingInBurst = burstCount;
		projectilesRemainingInMag = projectilesPerMag;
	}

	void LateUpdate(){
		transform.localPosition = Vector3.SmoothDamp(transform.localPosition,Vector3.zero,ref recoilSmoothDampVelocity, recoilMoveSettleTime);
		recoilAngle = Mathf.SmoothDamp(recoilAngle,0,ref recoilRotSmoothDampVelocity, recoilRotationSettleTime);
		transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;

		if(!isReloading && projectilesRemainingInMag == 0){
			Reload();
		}
	}

	void Shoot(){
		if(!isReloading && Time.time > nextShotTime && projectilesRemainingInMag > 0){
			
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
				
				if(projectilesRemainingInMag == 0){
					
					break;
				}

				projectilesRemainingInMag --;
				nextShotTime = Time.time + msBetweenShots / 1000f;
				Projectile newProjectile = Instantiate (projectile,projectileSpawn[i].position,projectileSpawn[i].rotation) as Projectile;
				newProjectile.SetSpeed(muzzleVelocity);
			}
			Instantiate(shell,shellEjection.position,shellEjection.rotation);
			muzzleFlash.Activate();	
			transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x,kickMinMax.y);
			recoilAngle += Random.Range(recoilAngleMinMax.x,recoilAngleMinMax.y);
			recoilAngle = Mathf.Clamp(recoilAngle, 0f , 30f);

			AudioManager.instance.PlaySound(shootAudio,transform.position);
		}


	}
	public void Reload(){
		if(!isReloading && projectilesRemainingInMag != projectilesPerMag){
			AudioManager.instance.PlaySound(reloadAudio,transform.position);
			StartCoroutine(AnimateReload());	
		}

	}
	IEnumerator AnimateReload(){
		isReloading = true;
		yield return new WaitForSeconds(.2f);

		float reloadSpeed = 1f / reloadTime;
		float percent = 0f;
		Vector3 initialRot = transform.localEulerAngles;
		float maxReloadAngle = 30f;

		while (percent < 1f){
			
			percent += Time.deltaTime * reloadSpeed;
			float interpolation = (-(percent * percent) + percent) * 4f;
			float reloadAngle = Mathf.Lerp(0,maxReloadAngle,interpolation);
			transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;
			yield return null;

		}

		isReloading = false;
		projectilesRemainingInMag = projectilesPerMag;
	}

	public void OnTriggerHold(){
		Shoot();
		triggerReleasedSinceLastShot = false;
	}
	public void OnTriggerRelease(){
		triggerReleasedSinceLastShot = true;
		shotsRemainingInBurst = burstCount;
	}

	public void Aim(Vector3 aimPoint){
		if(!isReloading){
			transform.LookAt(aimPoint);	
		}

	}
}
