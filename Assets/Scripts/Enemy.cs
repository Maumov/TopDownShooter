using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity {

	public enum State{Idle, Chasing,Attacking}
	State currentState;

	public ParticleSystem deathEffect;

	public static event System.Action OnDeathStatic;

	NavMeshAgent pathFinder;
	Transform target;
	LivingEntity targetEntity;
	Material skinMaterial;

	Color originalColour;

	float AttackDistanceThreshold = .5f;
	float timeBetweenAttacks = 1f;
	float damage = 1f;

	float nextAttackTime;
	float myCollisionRadius;
	float targetCollisionRadius;

	bool hasTarget;
	// Use this for initialization
	void Awake(){
		
		pathFinder = GetComponent<NavMeshAgent>();

		if( GameObject.FindGameObjectWithTag("Player") != null){
			
			hasTarget = true;

			target = GameObject.FindGameObjectWithTag("Player").transform;
			targetEntity = target.GetComponent<LivingEntity>();


			myCollisionRadius = GetComponent<CapsuleCollider>().radius;
			targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;


		}


	}

	protected  override void Start () {
		base.Start();

		if(hasTarget){
			currentState = State.Chasing;
			targetEntity.OnDeath += OnTargetDeath;
			StartCoroutine(UpdatePath());
		}
	}

	public void SetCharacteristics(float moveSpeed,int hitsToKillPlayer,float enemyHealth, Color skinColour){
		pathFinder.speed = moveSpeed;

		if(hasTarget){
			damage = Mathf.Ceil(targetEntity.startingHealth / hitsToKillPlayer);
		}
		startingHealth = enemyHealth;

		deathEffect.startColor = new Color(skinColour.r,skinColour.g,skinColour.b,1f);
		skinMaterial = GetComponent<Renderer>().material;
		skinMaterial.color = skinColour;
		originalColour = skinMaterial.color;

	}

	public override void TakeHit (float damage, Vector3 hitPoint, Vector3 hitDirection)
	{
		AudioManager.instance.PlaySound("Impact",transform.position);
		if(damage >= health ){
			if(OnDeathStatic != null){
				OnDeathStatic();
			}
			AudioManager.instance.PlaySound("Enemy Death",transform.position);
			Destroy(Instantiate(deathEffect.gameObject,hitPoint,Quaternion.FromToRotation(Vector3.forward,hitDirection)) as GameObject,deathEffect.startLifetime);
		}
		base.TakeHit (damage, hitPoint, hitDirection);

	}


	void OnTargetDeath(){
		hasTarget = false;
		currentState = State.Idle;
	}
	
	// Update is called once per frame
	void Update () {
		if(hasTarget){
			if(Time.time > nextAttackTime){
				float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;	

				if(sqrDstToTarget < Mathf.Pow(AttackDistanceThreshold + myCollisionRadius + targetCollisionRadius,2)){
					nextAttackTime = Time.time + timeBetweenAttacks;
					AudioManager.instance.PlaySound("Enemy Attack",transform.position);
					StartCoroutine(Attack());
				}	
			}	
		}
	}
	IEnumerator Attack(){

		currentState = State.Attacking;
		pathFinder.enabled = false;


		Vector3 originalPosition = transform.position;
		Vector3 dirToTarget = (target.position - transform.position).normalized;
		Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius);

		float attackSpeed = 3f;
		float percent = 0f;

		skinMaterial.color = Color.red;

		bool hasAppliedDamage = false;

		while (percent <= 1f){

			if(percent >= .5f && !hasAppliedDamage){
				hasAppliedDamage = true;
				targetEntity.TakeDamage(damage);
			}
			percent += Time.deltaTime * attackSpeed;
			float interpolation = (-(percent * percent) + percent) * 4f;
			transform.position = Vector3.Lerp(originalPosition,attackPosition,interpolation);

			yield return null;

		}
		skinMaterial.color = originalColour;
		currentState = State.Chasing;
		pathFinder.enabled = true;
	}

	IEnumerator UpdatePath(){
		float refreshRate = .25f;
		while(hasTarget){
			if(currentState == State.Chasing){
				Vector3 dirToTarget = (target.position - transform.position).normalized;
				Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + (AttackDistanceThreshold * 0.5f));
					
				if(!dead){
					pathFinder.SetDestination(targetPosition);	
				}
			}				
			yield return new WaitForSeconds(refreshRate);
		}
	}
}
