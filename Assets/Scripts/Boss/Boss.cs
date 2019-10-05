﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossState
{
	INITIAL, INTRO, PHASE1, PHASE2, PHASE3, DEAD
}

public class Boss : MonoBehaviour
{
	public float dodge = 2.0f;
	public Vector2 maneuverTime = new Vector2(2, 5);

	private Eye leftEye, rightEye, bigEye;
	private float lifetime = 0.0f;
	private float eyeToggle = 0.0f;
	private float currentXSpeed = 0.0f;
	private float currentYSpeed = 0.0f;
	private float targetXManeuver = 0.0f;
	private float targetYManeuver = 0.0f;
	private int health = 100;
	private BossState state = BossState.INITIAL;
	private GameController gameController;
	private Rigidbody2D rb;

	void Start()
    {
		leftEye = transform.Find("LeftEye").GetComponent<Eye>();
		rightEye = transform.Find("RightEye").GetComponent<Eye>();
		bigEye = transform.Find("BigEye").GetComponent<Eye>();
		gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
		rb = GetComponent<Rigidbody2D>();
		NextPhase();
	}

	// Update is called once per frame
	void Update()
    {
		lifetime += Time.deltaTime;
		eyeToggle -= Time.deltaTime;

		switch (state)
		{
			case BossState.INTRO:
				Intro();
				break;
			case BossState.PHASE1:
				Phase1();
				break;
			case BossState.PHASE2:
				Phase2();
				break;
			case BossState.PHASE3:
				Phase3();
				break;
			case BossState.DEAD:
				//gameController.Win();
				break;
			default:
				break;
		}


	}

	public void NextPhase()
	{
		state = (BossState)((int)state + 1);
		Debug.Log("BOSS IN PHASE: " + state.ToString());

		if (state == BossState.INTRO)
		{
			StartCoroutine(Appear());
		}

		if (state == BossState.PHASE1)
		{
			StopCoroutine(Appear());
			leftEye.ToggleEye();
		}

		if (state == BossState.PHASE2)
		{
			StartCoroutine(Evade());
			leftEye.Kill();
			rightEye.Kill();
		}
		else if (state == BossState.DEAD)
		{
			Destroy(gameObject);
		}
	}

	void Intro()
	{

	}

	void Phase1()
	{
		if (eyeToggle < 0.0f)
		{
			leftEye.ToggleEye();
			rightEye.ToggleEye();
			eyeToggle = 5.0f;
		}

		if (leftEye.IsDead() && rightEye.IsDead())
		{
			NextPhase();
		}

		if (lifetime > 20.0f)
		{
			NextPhase();
		}
	}

	void Phase2()
	{
		if (eyeToggle < 0.0f)
		{
			bigEye.ToggleEye();
			eyeToggle = 5.0f;
		}

		if (lifetime > 40.0f)
		{
			NextPhase();
		}
	}

	void Phase3()
	{
		if (eyeToggle < 0.0f)
		{
			leftEye.ToggleEye();
			rightEye.ToggleEye();
			bigEye.ToggleEye();
			eyeToggle = 2.0f;
		}

		if (lifetime > 60.0f)
		{
			NextPhase();
		}
	}

	void FixedUpdate()
	{
		var camMin = Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, 0.0f));
		var camMax = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, 0.0f));

		var newXManeuver = Mathf.MoveTowards(rb.velocity.x, targetXManeuver, Time.deltaTime * 0.9f);
		var newYManeuver = Mathf.MoveTowards(rb.velocity.y, targetYManeuver, Time.deltaTime * 0.9f);
		rb.velocity = new Vector3(newXManeuver, newYManeuver, 0.0f);
		rb.position = new Vector3
		(
			Mathf.Clamp(rb.position.x, camMin.x, camMax.x),
			Mathf.Clamp(rb.position.y, camMin.y - 100.0f, camMax.y + 100.0f),
			0.0f
		);

		//rb.rotation = Quaternion.Euler(0.0f, 0.0f, rb.velocity.x * -tilt);
	}

	IEnumerator Evade()
	{
		yield return new WaitForSeconds(2);

		while (true)
		{
			targetXManeuver = Random.Range(1, dodge) * -Mathf.Sign(transform.position.x);
			yield return new WaitForSeconds(Random.Range(maneuverTime.x, maneuverTime.y));
			targetXManeuver = 0;
			yield return new WaitForSeconds(Random.Range(1.0f, 3.0f));
		}
	}

	IEnumerator Appear()
	{
		yield return new WaitForSeconds(2);
		targetYManeuver = -0.295f;
		yield return new WaitForSeconds(10);
		targetYManeuver = 0;
		NextPhase();
	}

}
