using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	Vector3 mousePos;
	Vector3 target;
	public float ExpSpeed;
	Vector3 targetPos;

	Vector3 Mod360(Vector3 a)
    {
		a += Vector3.one * 180f;
		return new Vector3(a.x % 360f, a.y % 360f, a.z % 360f) - Vector3.one * 180f;
    }
	
	Vector3 Mod360Special(Vector3 a)
    {
		a += Vector3.one * 180f;
		return new Vector3(a.x % 360f - 180f, (a.y + 180f) % 360f, (a.z + 180f) % 360f);
    }

	void Start()
    {
		target = transform.eulerAngles;
		targetPos = transform.position;
    }
	// Update is called once per frame
	void Update () {
		Vector3 del = Input.mousePosition - mousePos;
		mousePos = Input.mousePosition;
		del.x *= 360f / Screen.width;
		del.y *= 180f / Screen.height;

		if (Input.GetMouseButton(1))
			target = Mod360Special(new Vector3(Mathf.Clamp(target.x - del.y, -89f, 89f), target.y + del.x));
		transform.eulerAngles += Mod360(target - Mod360(transform.eulerAngles)) * (1f - Mathf.Exp(-Time.deltaTime * 8f));

		Vector3 move = Vector3.zero;
		if (Input.GetKey(KeyCode.W))
			move.z++;
		if (Input.GetKey(KeyCode.S))
			move.z--;
		if (Input.GetKey(KeyCode.Q))
			move.y--;
		if (Input.GetKey(KeyCode.E))
			move.y++;
		if (Input.GetKey(KeyCode.A))
			move.x--;
		if (Input.GetKey(KeyCode.D))
			move.x++;
		targetPos += transform.rotation * move * Mathf.Exp(ExpSpeed);
		transform.position = Vector3.Lerp(targetPos, transform.position, Mathf.Exp(-Time.deltaTime * 6f));
		ExpSpeed += Input.mouseScrollDelta.y * 0.1f;
	}
}
