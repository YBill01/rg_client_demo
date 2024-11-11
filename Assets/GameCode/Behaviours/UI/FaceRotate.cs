using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceRotate : MonoBehaviour
{
	public bool BillboardX = true;
	public bool BillboardY = true;
	public bool BillboardZ = true;
	public float OffsetToCamera;
	//protected Vector3 localStartPosition;

	// Use this for initialization
	void Start()
	{
		//localStartPosition = transform.localPosition;
		updatePos();
	}

	// Update is called once per frame
	void Update()
	{
		updatePos();
		//if (!BillboardX || !BillboardY || !BillboardZ)
		//	transform.rotation = Quaternion.Euler(BillboardX ? transform.rotation.eulerAngles.x : 0f, BillboardY ? transform.rotation.eulerAngles.y : 0f, BillboardZ ? transform.rotation.eulerAngles.z : 0f);
		//transform.localPosition = localStartPosition;
		//transform.position = transform.position + transform.rotation * Vector3.forward * OffsetToCamera;
		//transform.position = Camera.main.WorldToScreenPoint(transform.position);

		//Vector3 position = Camera.main.WorldToScreenPoint(this.transform.position);
		//this.transform.position = new Vector3(position.x, position.y, 0f);
	}

	private void updatePos()
	{
		if (Camera.main == null) return;
		transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
															   Camera.main.transform.rotation * Vector3.up);
	}
}