﻿using UnityEngine;
using System.Collections;
using Leap;

public class TrackingScript : MonoBehaviour {
	Controller controller;
	public GameObject createdObjects;
	HandList handList;
	Hand leftHand;
	Hand rightHand;
	bool panEnabled;
	bool rotateEnabled;
	bool zoomEnabled;
	public GUIText guiText;
	public GameObject bangParticles;

	public GameObject rightGroundMarker;
	public GameObject leftGroundMarker;

	public Transform focalPoint;
	Leap.Vector leftHandPrev;
	Leap.Vector rightHandPrev;

	bool thumbWasExtended;

	Leap.Vector leftDirPrev;
	Leap.Vector rightDirPrev;

	public GameObject cube;

	Vector3 rightTracker;
	Vector3 leftTracker;

	bool locked;
	void Start ()
	{
		leftHandPrev = new Leap.Vector (0, 0, 0);
		rightHandPrev = new Leap.Vector (0, 0, 0);
		rightTracker = new Vector3 (0, 0, 0);
		leftTracker = new Vector3 (0, 0, 0);
		leftHand = new Hand ();
		rightHand = new Hand ();
		controller = new Controller();
		panEnabled = true;
		rotateEnabled = true;
		zoomEnabled = true;
		controller.EnableGesture (Gesture.GestureType.TYPECIRCLE);
		controller.EnableGesture (Gesture.GestureType.TYPESWIPE);
		controller.EnableGesture (Gesture.GestureType.TYPEKEYTAP);
		controller.EnableGesture (Gesture.GestureType.TYPESCREENTAP);
		locked = false;
		thumbWasExtended = true;
		guiText.text = "";
	}
	
	void Update ()
	{
		//initialize hand positions
		Frame frame = controller.Frame();
		handList = frame.Hands;
		leftHand = handList.Leftmost;
		rightHand = handList.Rightmost;
		toggleGestures (true);
		if (rightPoint () != null && !locked) {
						rightTracker = rightMark ();
				}
		if (leftPoint () != null && !locked) {
						leftTracker = leftMark ();
				}
		/*
			RaycastHit rhInfo = new RaycastHit();
			Ray leftRay = new Ray(vectorConvert (leftHand.PalmPosition),
			                      vectorConvert(leftHand.PalmNormal)/100);
			if (Physics.Raycast (leftRay, out rhInfo, 400.0f)) {
				leftGroundMarker.transform.position = rhInfo.point;
			}
			rhInfo = new RaycastHit ();
			Ray rightRay = new Ray(vectorConvert (rightHand.PalmPosition),
			                      vectorConvert(rightHand.PalmNormal.Normalized)/100);
			if (Physics.Raycast (rightRay, out rhInfo, 400.0f)) {
				rightGroundMarker.transform.position = rhInfo.point;
		}
		*/
		panEnabled = true;
		zoomEnabled = true;
		rotateEnabled = true;
		//pan
		if (rightHandForward() && leftHandForward () && handList.Count==2 && panEnabled) {//both hands facing forward
			guiText.text = "Pan Mode";
			toggleGestures(false);
			transform.position += (rightHandPrev.x - rightHand.PalmPosition.x) * transform.right * Mathf.Abs(rightHand.PalmVelocity.x) * Time.deltaTime * 1/5;
			transform.position += (rightHandPrev.y - rightHand.PalmPosition.y) * transform.up * Mathf.Abs(rightHand.PalmVelocity.y) * Time.deltaTime * 1/5;
		}
		//rotate
		if (rightFistForward () && leftHandForward () && handList.Count==2 && rotateEnabled) {
			guiText.text = "Rotate Mode";
			panEnabled = false;
			zoomEnabled = false;
			toggleGestures(false);
			transform.position += (leftHandPrev.x - leftHand.PalmPosition.x) * transform.right * Mathf.Abs(leftHand.PalmVelocity.x) * Time.deltaTime * 1/5;
			transform.LookAt(focalPoint);
			transform.position += (leftHandPrev.y - leftHand.PalmPosition.y) * transform.up * Mathf.Abs(leftHand.PalmVelocity.y) * Time.deltaTime * 1/5;
			transform.LookAt(focalPoint);
		}
		//zoom
		if (rightHandForward () && leftFistForward () && handList.Count==2 && zoomEnabled) {
			guiText.text = "Zoom Mode";
			toggleGestures(false);
			transform.position += (rightHandPrev.x - rightHand.PalmPosition.x) * transform.forward * Mathf.Abs(rightHand.PalmVelocity.x) * Time.deltaTime * 1/5;
		}
		//create/destroy
		if (leftPoint () != null){
			guiText.text = "Create/Destroy Mode";
			panEnabled = false;
			rotateEnabled = false;
			zoomEnabled = false;
			if (rightPinch ()) {//vert box creation
					panEnabled = false;
					locked = true;
					GameObject newCube = GameObject.CreatePrimitive (PrimitiveType.Cube);
					newCube.transform.position = leftTracker;
					newCube.AddComponent<Collider>();
					newCube.transform.parent = createdObjects.transform;

				if(rightHandPrev.y < rightHand.PalmPosition.y){
					newCube.transform.localScale += newCube.transform.up * Time.deltaTime * rightHand.PalmVelocity.y * 1/5;
					newCube.transform.position += newCube.transform.up * Time.deltaTime * rightHand.PalmVelocity.y * 1/5;
				}else{
					newCube.transform.position += -newCube.transform.up * Time.deltaTime * rightHand.PalmVelocity.y * 1/5;
				}
			} else {
				locked = false;
				Finger thumb = null;
				foreach(Finger f in leftHand.Fingers){
					if(f.Type() == Finger.FingerType.TYPE_THUMB){
						thumb = f;
					}
				}//eoforeach
				if(thumbWasExtended){
					if(!thumb.IsExtended){
						bang();
					}
				}

			}
		}

		foreach (Gesture g in frame.Gestures()) {
			//print (g.Type.ToString());
			if (leftFistForward() && g.Type == Gesture.GestureType.TYPECIRCLE && g.State == Gesture.GestureState.STATE_STOP) {
					GameObject newSphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
					newSphere.transform.position = new Vector3 (Random.Range (4, 7), Random.Range (4, 5), Random.Range (0, 1));
					newSphere.AddComponent<Rigidbody> ();
					newSphere.transform.parent = createdObjects.transform;
			}
			if (g.Type == Gesture.GestureType.TYPESWIPE) {/*
				if(leftPoint () != null){
					RaycastHit rhInfo = new RaycastHit ();
					Ray shootRay = new Ray (vectorConvert(rightPoint ().TipPosition),
					                        vectorConvert(rightPoint ().Direction));
					if (Physics.Raycast (shootRay, out rhInfo, 400.0f)) {
						GameObject.Destroy(rhInfo.collider);
						print ("destroy");
					}
				}
				*/}

			}
		//movement
		leftHandPrev = leftHand.PalmPosition;
		rightHandPrev = rightHand.PalmPosition;
		leftDirPrev = leftHand.Direction;
		rightDirPrev = rightHand.Direction;
	}//eo update

	void bang(){
		if(rightFistForward()){
			GameObject.Instantiate (bangParticles, leftTracker+transform.up*1.2f, Quaternion.LookRotation (transform.forward));
			Collider[] affectInRange = Physics.OverlapSphere (leftTracker, 3.0f);
			for(int ii=0;ii<affectInRange.Length;ii++){
				if(!affectInRange[ii].name.Equals("Ground") && !affectInRange[ii].name.Contains("Marker")){
					GameObject.Destroy(affectInRange[ii].gameObject);
				}
			}
		}
	}//eobang

	void toggleGestures(bool enabled){
		controller.EnableGesture (Gesture.GestureType.TYPECIRCLE,enabled);
		controller.EnableGesture (Gesture.GestureType.TYPESWIPE, enabled);
		controller.EnableGesture (Gesture.GestureType.TYPEKEYTAP, enabled);
		controller.EnableGesture (Gesture.GestureType.TYPESCREENTAP, enabled);

	}
	bool rightHandForward(){
		return rightHand.Direction.z < -.30 && rightHand.Direction.z > -.80;
		}//eorighthandforward
	bool leftHandForward(){
		return leftHand.Direction.z < -.30 && leftHand.Direction.z > -.80;
	}//eolefthandforward

	bool rightFistForward(){
		panEnabled = false;
		bool fist = true;
		foreach(Pointable p in rightHand.Pointables){
			if(p.IsExtended){
				fist = false;
			}
		}
			return rightHand.Direction.z < -.30 && rightHand.Direction.z > -.80
						&& fist;
		}//eorightfistforward
	bool leftFistForward(){
		panEnabled = false;
		bool fist = true;
		foreach(Pointable p in leftHand.Pointables){
			if(p.IsExtended){
				fist = false;
			}
		}
		return leftHand.Direction.z < -.30 && leftHand.Direction.z > -.80
			&& fist;
	}//eoleftfistforward

	bool rightPinch(){
		panEnabled = false;
		return rightHand.Pointables.Extended ().Count == 3;
	}//eorightpinch

	bool leftPinch(){
		panEnabled = false;
		return leftHand.Pointables.Extended ().Count == 3;
	}//eoleftpinch

	Finger rightPoint(){
		Finger index = null;
		foreach (Finger f in rightHand.Fingers) {
			if(f.IsExtended && f.Type() == Finger.FingerType.TYPE_INDEX){
				index = f;
			}
			if(f.IsExtended && f.Type() != Finger.FingerType.TYPE_INDEX){
				index = null;
			}
				}
		return index;
	}//eorightpoint

	Vector3 rightMark(){
		Vector3 output = new Vector3 (0, 0, 0);
		if (rightPoint () != null) {
			RaycastHit rhInfo = new RaycastHit ();
			Ray shootRay = new Ray (vectorConvert(rightPoint ().TipPosition),
			                        vectorConvert(rightPoint ().Direction));
			if (Physics.Raycast (shootRay, out rhInfo, 400.0f)) {

				output = new Vector3(rhInfo.point.x,rhInfo.point.y,-rhInfo.point.z);
			}
		}
		rightGroundMarker.transform.position = output;
		return output;
	}//eoRightMark
	Finger leftPoint(){
		Finger index = null;
		foreach (Finger f in leftHand.Fingers) {
			if(f.IsExtended && f.Type() == Finger.FingerType.TYPE_INDEX){
				index = f;
			}
			if(f.IsExtended && f.Type() != Finger.FingerType.TYPE_INDEX){
				index = null;
			}
		}
		return index;
	}//eoleftpoint

	Vector3 leftMark(){
		Vector3 output = new Vector3 (0, 0, 0);
		if (leftPoint () != null) {
			RaycastHit rhInfo = new RaycastHit ();
			Ray shootRay = new Ray (vectorConvert(leftPoint ().TipPosition),
			                        vectorConvert(leftPoint ().Direction));
			if (Physics.Raycast (shootRay, out rhInfo, 400.0f)) {
				output = new Vector3(rhInfo.point.x,rhInfo.point.y,-rhInfo.point.z);
			}
		}
		leftGroundMarker.transform.position = output;
		return output;
	}//eoleftmark
	void rightHoverOver(GameObject gameObj){
		if (leftIsWithinBounds('x',cube)){
			char majorDir = 'x';
			if(Mathf.Abs(rightHand.PalmNormal.x) > Mathf.Abs(rightHand.PalmNormal.y)){
				majorDir = (Mathf.Abs(rightHand.PalmNormal.x) > Mathf.Abs(rightHand.PalmNormal.z))? 'x' : 'z';
			}else{
				majorDir = (Mathf.Abs(rightHand.PalmNormal.y) > Mathf.Abs(rightHand.PalmNormal.z))? 'y' : 'z';
			}
			print (majorDir);
		}
	}//eorighthover

	bool leftIsWithinBounds(char dir, GameObject gameObj){
		switch (dir) {
		case 'x':
			return leftHand.PalmPosition.x > gameObj.transform.position.x - gameObj.transform.localScale.x/2
				&& leftHand.PalmPosition.x < gameObj.transform.position.x + gameObj.transform.localScale.x/2;
		case 'y':
			return leftHand.PalmPosition.y > gameObj.transform.position.y - gameObj.transform.localScale.y/2
				&& leftHand.PalmPosition.y < gameObj.transform.position.y + gameObj.transform.localScale.y/2;	
		case 'z':
			return leftHand.PalmPosition.z > gameObj.transform.position.z - gameObj.transform.localScale.z/2
				&& leftHand.PalmPosition.z < gameObj.transform.position.z + gameObj.transform.localScale.z/2;
		default:
			return false;
				}
	}//eoleftwithinbounds

	Vector3 vectorConvert(Leap.Vector lVec){
		return new Vector3(lVec.x, lVec.y,lVec.z)/100;
	}
}//eof
