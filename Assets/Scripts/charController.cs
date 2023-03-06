using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class charController : MonoBehaviour
{

	public float movementSpeed = 5.0f;
	public float mouseSensitivity = 5.0f;

	float verticalRotation = 0;
	public float upDownRange = 90.0f;

	float verticalVelocity = 0;

	CharacterController characterController;

	private void OnCollisionEnter(Collision col)
	{
		if (col.collider.tag == "Arcade")
		{
			Cursor.lockState = CursorLockMode.None;
			SceneManager.LoadScene("joculet 2d");
		}

		if (col.collider.tag == "Usa")
		{
			Debug.Log("iesi joc");
			Application.Quit();
		}
	}

	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		characterController = GetComponent<CharacterController>();
	}

	void Update()
	{
		float rotLeftRight = Input.GetAxis("Mouse X") * mouseSensitivity;
		transform.Rotate(0, rotLeftRight, 0);

		verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
		verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
		Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

		float forwardSpeed = Input.GetAxis("Vertical") * movementSpeed;
		float sideSpeed = Input.GetAxis("Horizontal") * movementSpeed;

		verticalVelocity += Physics.gravity.y * Time.deltaTime;

		Vector3 speed = new Vector3(sideSpeed, verticalVelocity, forwardSpeed);

		speed = transform.rotation * speed;

		characterController.Move(speed * Time.deltaTime);
	}
}
