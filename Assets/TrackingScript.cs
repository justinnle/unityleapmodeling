using UnityEngine;
using System.Collections;
using Leap;

public class TrackingScript : MonoBehaviour {
	Controller controller;
	public GameObject createdObjects;
	HandList handList;
	Hand leftHand;
	Hand rightHand;
	bool panEnabled;


	Leap.Vector leftHandPrev;
	Leap.Vector rightHandPrev;

	Leap.Vector leftDirPrev;
	Leap.Vector rightDirPrev;

	void Start ()
	{
		leftHandPrev = new Leap.Vector (0, 0, 0);
		rightHandPrev = new Leap.Vector (0, 0, 0);
		leftDirPrev = new Leap.Vector (0, 0, 0);
		rightDirPrev = new Leap.Vector (0, 0, 0);
		leftHand = new Hand ();
		rightHand = new Hand ();
		controller = new Controller();
		panEnabled = true;
		controller.EnableGesture (Gesture.GestureType.TYPECIRCLE);
		controller.EnableGesture (Gesture.GestureType.TYPESWIPE);
		controller.EnableGesture (Gesture.GestureType.TYPEKEYTAP);
		controller.EnableGesture (Gesture.GestureType.TYPESCREENTAP);
	}
	
	void Update ()
	{
		//transform gives you main camera
		//initialize hand positions
		Frame frame = controller.Frame();
		handList = frame.Hands;
		leftHand = handList.Leftmost;
		rightHand = handList.Rightmost;

		if (!rightFistForward() && !leftFistForward ()) {
			panEnabled = true;
				}
		if (rightHandForward() && leftHandForward () && handList.Count==2 && panEnabled) {//both hands facing forward
			toggleGestures(false);
			transform.position += (rightHandPrev.x - rightHand.PalmPosition.x) * transform.right * Mathf.Abs(rightHand.PalmVelocity.x) * Time.deltaTime * 1/5;
			transform.position += (rightHandPrev.y - rightHand.PalmPosition.y) * transform.up * Mathf.Abs(rightHand.PalmVelocity.y) * Time.deltaTime * 1/5;
		}
		if (rightFistForward () && leftHandForward () && handList.Count==2) {
			toggleGestures(false);
			print ("rotateleft");
		}
		if (rightHandForward () && leftFistForward () && handList.Count==2) {
			toggleGestures(false);
			print ("rotateleft");
		}


		foreach (Gesture g in frame.Gestures()){
			//print (g.Type.ToString());
			if(g.Type == Gesture.GestureType.TYPECIRCLE && g.State==Gesture.GestureState.STATE_STOP){
				print ("circlestop");
				GameObject newSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				newSphere.transform.position = new Vector3(Random.Range(4,7),Random.Range(4,5),Random.Range(0,1));
				newSphere.AddComponent<Rigidbody>();
				newSphere.transform.parent = createdObjects.transform;
			}
			if(g.Type == Gesture.GestureType.TYPESWIPE && g.State==Gesture.GestureState.STATE_STOP){
				//g = (SwipeGesture)g;
			}

		}
		leftHandPrev = leftHand.PalmPosition;
		rightHandPrev = rightHand.PalmPosition;
		leftDirPrev = leftHand.Direction;
		rightDirPrev = rightHand.Direction;
	}//eo update

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
	
}//eof
