using System.Collections.Generic;
using UnityEngine;


public class PhysicsObject : MonoBehaviour {

	public float gravityModifier = 1f;
	public float minGroundNormalY = 0.65f;

	protected Vector2 targetVelocity;
	public bool isGrounded;
	protected Vector2 groundNormal = Vector2.up;
	public Vector2 velocity;
	protected Rigidbody2D rb2d;
	protected ContactFilter2D contactFilter;
	protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
	protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);
	
	protected const float shellRadius = 0.01f;
	protected const float MIN_MOVE_DISTANCE = 0.001f; 

	void OnEnable() {
		rb2d = GetComponent<Rigidbody2D> ();
	}

	// Use this for initialization
	protected void Start () {
		contactFilter.useTriggers = false;
		contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
		contactFilter.useLayerMask = true;

		this.velocity = Vector2.zero;
	}
	
	// Update is called once per frame
	void Update () {
		targetVelocity = Vector2.zero;
		ComputeVelocity();
	}

	protected virtual void ComputeVelocity() {

	}

	protected void FixedUpdate() {
		velocity += gravityModifier * Physics2D.gravity * Time.fixedDeltaTime;
		//velocity.x = targetVelocity.x;

		isGrounded = false;

		Vector2 deltaPosition = velocity * Time.fixedDeltaTime;
		
		Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

		Vector2 move = moveAlongGround * deltaPosition.x; 	

		Movement(move, false);

		move = Vector2.up * deltaPosition.y;

		Movement(move, true);
	}

	void Movement(Vector2 move, bool yMovement) {
		float distance = move.magnitude;

		if (distance > MIN_MOVE_DISTANCE) {
			int count = rb2d.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
			hitBufferList.Clear();
			for (int i = 0; i < count;i++) {
				hitBufferList.Add(hitBuffer[i]);
			}

			for (int i = 0; i < hitBufferList.Count; i++) {
				Vector2 currentNormal = hitBufferList [i].normal;

				if (currentNormal.y > minGroundNormalY)
				{
					isGrounded = true;
					if (yMovement) {
						groundNormal = currentNormal;
						currentNormal.x = 0;
					}
				}

				float projection = Vector2.Dot(velocity, currentNormal);
				if (projection < 0) {
					velocity = velocity - (projection * currentNormal);
				}

				GameObject hitObject = hitBufferList[i].transform.gameObject;
				if (hitObject.layer == 13) {
					float modifiedDistance  = hitBufferList[i].distance - shellRadius;
					distance = modifiedDistance < distance ? modifiedDistance : distance;
				}
			}
		}
		
		rb2d.position += move.normalized * distance;
	}

	protected void SetLayer(int layer) {
		this.gameObject.layer = layer;
		this.contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(layer));
	}
}

