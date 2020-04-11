// <copyright file="PlayerInputModule2D.cs" company="DIS Copenhagen">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author>Benno Lueders</author>
// <date>07/14/2017</date>

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Script for general purpose 2D controls for any object that can move and jump when grounded.
/// </summary>
[RequireComponent (typeof(Rigidbody2D))]
public class PlatformerController2D : MonoBehaviour
{
	[HideInInspector] public Vector2 input;
    private bool _inputJump;
    public bool inputJump { set { if (value) { _inputJump = true; } } } // buffer jump input also from regular Update in _inputJump

	private bool _inputFlip;
	public bool inputFlip { set { if (value) { _inputFlip = true; } } } // buffer jump input also from regular Update in _inputJump

	[HideInInspector] public bool IsGrounded { get { return grounded; } }

	[Tooltip ("Can this object move.")]
	public bool canMove = true;

	[Header ("Controls")]
	[Tooltip ("Base maximum horizontal movement speed.")]
	[SerializeField] float speed = 5;
	[Tooltip ("Start velocity when jumping.")]
	[SerializeField] float jumpVelocity = 15;
	[Tooltip ("Downwards acceleration.")]
	[SerializeField] public float gravity = 40;
	[Tooltip ("Time delay a jump is still performed, when grounding is gained after the jump button was pressed in the air.")]
	[SerializeField] float jumpingToleranceTimer = .1f;
	[Tooltip ("Time delay that a jump is still allowed when grounding is lost.")]
	[SerializeField] float groundingToleranceTimer = .1f;
	[Tooltip("Time delay a flip is still performed, when grounding is gained after the flip button was pressed in the air.")]
	[SerializeField] float flippingToleranceTimer = .1f;

	[Header ("Grounding")]
	[Tooltip ("Offset of the grounding raycasts (red lines)")]
	[SerializeField] Vector2 groundCheckOffset = new Vector2 (0, 0.1f);
	[Tooltip ("Width of the grounding raycasts.")]
	[SerializeField] float groundCheckWidth = 1;
	[Tooltip ("Distance of the grounding raycasts.")]
	[SerializeField] float groundCheckDepth = 0.2f;
	[Tooltip ("Number of the grounding Raycsts. Will be evenly spread over the width")]
	[SerializeField] int groundCheckRayCount = 3;
	[Tooltip ("Layers to be considered ground.")]
	[SerializeField] LayerMask groundLayers = 0;

    [Header("Animations")]
    [Tooltip("How fast does the animation play")]
    public float animationFPS = 5;
    [Tooltip("Animation Frames (Sprites) to be played back when not moving.")]
    public Sprite[] idleFrames;
    [Tooltip("Animation Frames (Sprites) to be played back when moving.")]
    public Sprite[] movingFrames;
    [Tooltip("Animation Frames (Sprites) to be played back while not grounded (jumping or falling).")]
    public Sprite[] airFrames;

	[Header("GameObjects")]
	[Tooltip("Used to get positioning")]
	public GameObject groundObject;

	public AudioClip jumpSound;
	public AudioClip flipSound;
	public AudioClip collect;

	AudioSource audio;

	SpriteRenderer sr = null;
    int currentFrame = 0;
    float animationTimer = 0;

    bool grounded = true;
	bool touchingGround = true;
	Rigidbody2D rb2d = null;

	float lostGroundingTime = 0;
	float lastJumpTime = 0;
	float lastFlipTime = 0;
	float lastInputJump = 0;
	float lastInputFlip = 0;
	float lastHurtTime = 0;

	int facing = 1;
	int raycastMultiplier = 1;

	int coinScore = 0;

	int numLives = 5;

	public bool isImgOn;
	public Image spPart;

	public GameManager gameManager;

	
	void Start ()
	{
		audio = gameObject.AddComponent<AudioSource>();
		lastInputFlip = float.NegativeInfinity;
		lastInputJump = float.NegativeInfinity;
		canMove = true;
		rb2d = GetComponent<Rigidbody2D> ();
		sr = GetComponent<SpriteRenderer> ();
		numLives = 5;
	}

	/// <summary>
	/// Controls the basic update of the controller. This uses fixed update, since the movement is physics driven and has to be synched with the physics step.
	/// </summary>
	void FixedUpdate ()
	{
		UpdateGrounding ();

		Vector2 vel = rb2d.velocity;

		if (canMove) {
			vel.x = input.x * speed;

			if (CheckJumpInput () && PermissionToJump ()) {
				vel = ApplyJump (vel);
			}

			if (CheckFlipInput() && PermissionToFlip())
			{
				ApplyFlip();
			}
		}

		vel.y += -gravity * Time.deltaTime;
		rb2d.velocity = vel;

		UpdateAnimations ();

        // reset jump input every FixedUpdate to buffer from Update based input
        _inputJump = false;
		_inputFlip = false;


	}

	Vector2 ApplyJump (Vector2 vel)
	{
		audio.PlayOneShot(jumpSound);
		Debug.Log("jump");
		float relativeJumpVelocity = jumpVelocity;
        if(gravity < 0) { relativeJumpVelocity = -jumpVelocity;  }
		vel.y = relativeJumpVelocity;
		lastJumpTime = Time.time;
		grounded = false;
		return vel;
	}

	void ApplyFlip()
	{
		audio.PlayOneShot(flipSound);
		Debug.Log("flip");
		lastFlipTime = Time.time;
        grounded = false;

        Vector3 tmp = transform.localScale;
		tmp.y *= -1;
		transform.localScale = tmp;

		SpriteRenderer groundSprite = groundObject.GetComponent<SpriteRenderer>();
		float groundHeight = ((groundSprite.GetComponent<BoxCollider2D>().size.y + groundSprite.GetComponent<BoxCollider2D>().offset.y)* groundSprite.GetComponent<BoxCollider2D>().transform.localScale.y) + 2.5f;
        if(gravity < 0)
        {
			groundHeight = groundHeight * (-1);

		}

		transform.Translate(0,-groundHeight,0);
        gravity = -gravity;
		raycastMultiplier *= (-1);


    }

	/// <summary>
	/// Updates grounded and lastGroundingTime.
	/// </summary>
	void UpdateGrounding ()
	{
		Vector2 groudCheckCenter = new Vector2 (transform.position.x + groundCheckOffset.x, transform.position.y + groundCheckOffset.y);
		Vector2 groundCheckStart = groudCheckCenter + Vector2.left * groundCheckWidth * 0.5f;
		if (groundCheckRayCount > 1) {
			for (int i = 0; i < groundCheckRayCount; i++) {
				RaycastHit2D hit = Physics2D.Raycast (groundCheckStart, Vector2.down*(raycastMultiplier), groundCheckDepth, groundLayers);
				touchingGround = false;
				if (hit.collider != null) {
					grounded = true;
                    if (hit.collider.gameObject.CompareTag("Ground"))
                    {
						touchingGround = true;
                    }
					return;
				}
				groundCheckStart += Vector2.right * (1.0f / (groundCheckRayCount - 1.0f)) * groundCheckWidth;
			}
		}
		if (grounded) {
			lostGroundingTime = Time.time;
		}
		grounded = false;
	}

	void UpdateAnimations ()
	{
		if (!canMove)
        {
            PlayBackAnimation(idleFrames);
			return;
		}

        // calculate facing
		if (rb2d.velocity.x > 0 && facing == -1)
        {
			facing = 1;
		} else if (rb2d.velocity.x < 0 && facing == 1)
        {
			facing = -1;
        }
		sr.flipX = facing == -1;

        // update animations
        if (!grounded)
        {
            PlayBackAnimation(airFrames);
        } else if (Mathf.Abs(rb2d.velocity.x) > 0.1f)
        {
            PlayBackAnimation(movingFrames);
        } else
        {
            PlayBackAnimation(idleFrames);
        }
	}

    void PlayBackAnimation(Sprite[] anim)
    {
        animationTimer -= Time.deltaTime;
        if (animationTimer <= 0 && anim.Length > 0)
        {
            animationTimer = 1f / animationFPS;
            currentFrame++;
            if (currentFrame >= anim.Length)
            {
                currentFrame = 0;
            }
            sr.sprite = anim[currentFrame];
        }
    }

	/// <summary>
	/// Return true if the character can jump right now.
	/// </summary>
	/// <returns><c>true</c>, if to jump was permissioned, <c>false</c> otherwise.</returns>
	bool PermissionToJump ()
	{
		bool wasJustgrounded = Time.time < lostGroundingTime + groundingToleranceTimer;
		bool hasJustJumped = Time.time <= lastJumpTime + Time.deltaTime;
		return (grounded || wasJustgrounded) && !hasJustJumped;
	}

	/// <summary>
	/// Checks if the jump input is true right now.
	/// </summary>
	/// <returns><c>true</c>, if jump input detected, <c>false</c> otherwise.</returns>
	bool CheckJumpInput ()
	{
		if (_inputJump) {
			lastInputJump = Time.time;
			return true;
		}
		if (Time.time < lastInputJump + jumpingToleranceTimer) {
			return true;
		}
		return false;
	}


	/// <summary>
	/// Return true if the character can flip right now.
	/// </summary>
	/// <returns><c>true</c>, if to flip was permissioned, <c>false</c> otherwise.</returns>
	bool PermissionToFlip()
	{
		bool wasJustgrounded = Time.time < lostGroundingTime + groundingToleranceTimer;
        bool hasJustFlipped = Time.time <= lastFlipTime + Time.deltaTime;
		return (grounded || wasJustgrounded) && !hasJustFlipped && touchingGround;
	}

	/// <summary>
	/// Checks if the flip input is true right now.
	/// </summary>
	/// <returns><c>true</c>, if flip input detected, <c>false</c> otherwise.</returns>
	bool CheckFlipInput()
	{
		if (_inputFlip)
		{
			lastInputFlip = Time.time;
			return true;
		}
		if (Time.time < lastInputFlip + flippingToleranceTimer)
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// Pushback the object controlled by this instance with the specified force.
	/// </summary>
	/// <param name="force">Force to push the character back</param>
	public void Pushback (Vector2 force)
	{
		Debug.Log("pushback");
		rb2d.velocity = force;
		lastJumpTime = Time.time;
		grounded = false;
		lostGroundingTime = Time.time;
	}

	/// <summary>
	/// Make the object controlled by this instance jump immediately.
	/// </summary>
	/// <param name="strength">Strength.</param>
	public void ForceJump (float strength)
	{
		Debug.Log("forcejump");
		rb2d.velocity = new Vector2 (rb2d.velocity.x, strength);
		lastJumpTime = Time.time;
		grounded = false;
		lostGroundingTime = Time.time;
	}

	/// <summary>
	/// Used to draw the red lines for the grounding raycast. Only active in the editor and when the instance is selected.
	/// </summary>
	void OnDrawGizmosSelected(){
		Vector2 groudCheckCenter = new Vector2 (transform.position.x + groundCheckOffset.x, transform.position.y + groundCheckOffset.y);
		Vector2 groundCheckStart = groudCheckCenter + Vector2.left * groundCheckWidth * 0.5f;
		if (groundCheckRayCount > 1) {
			for (int i = 0; i < groundCheckRayCount; i++) {
				Debug.DrawLine (groundCheckStart, groundCheckStart + Vector2.down * groundCheckDepth * (raycastMultiplier), Color.red);
				groundCheckStart += Vector2.right * (1.0f / (groundCheckRayCount - 1.0f)) * groundCheckWidth;
			}
		}
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Spike"))
        {
			hurt();
			if (numLives == 0)
            {
				die();
            }

		}

		if (collision.gameObject.tag.Equals("Boundary"))
		{
			die();
			
		}

		//if (collision.gameObject.tag.Equals("Enemy"))
		//{
		//	numLives--;
		//	if (numLives == 0)
		//	{
		//		die();
		//	}
		//}
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
		if (collision.gameObject.tag.Equals("Enemy"))
        {
			hurt();
			if (numLives == 0)
			{
				die();
			}
		}

        Debug.Log(collision.gameObject.name + " : " + gameObject.name + " : " + Time.time);
		if (collision.CompareTag("coin"))
		{
			audio.PlayOneShot(collect);
			Debug.Log("collided with coin");
			coinScore++;
			Destroy(collision.gameObject);
			Debug.Log("score is: " + coinScore);
			GameManager.instance.AdjustScore(1);

		}

	}


    private void die()
    {
		Debug.Log("die"); //Restart level or end game?
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}


    private void hurt()
    {
		if(Time.time <= lastJumpTime + Time.deltaTime)
        {
			return;
        }
		lastHurtTime = Time.deltaTime;
		numLives--;
        GameManager.instance.DecreaseLives();
        StartCoroutine(blinkSprite());
	}


    private IEnumerator blinkSprite()
	{
		for (var n = 0; n < 5; n++)
		{
			sr.enabled = true;
            sr.material.SetColor("_Color", Color.red);
			yield return new WaitForSeconds(0.1f);
			sr.enabled = false;
			yield return new WaitForSeconds(0.1f);

		}
		sr.material.SetColor("_Color", Color.white);
		sr.enabled = true;
	}
}
