using UnityEngine;
using System.Collections;
using Leap;

public class TrackingScript : MonoBehaviour {
	Controller controller;
	public GameObject createdObjects;
	HandList handList;
	Hand leftHand;
	Hand rightHand;
	bool debug;

	Leap.Vector leftHandPrev;
	Leap.Vector rightHandPrev;

	Leap.Vector leftDirPrev;
	Leap.Vector rightDirPrev;

	string navigationState;//pan,rotate,zoom
	void Start ()
	{
		/*leftHandPrev = new Leap.Vector (0, 0, 0);
		rightHandPrev = new Leap.Vector (0, 0, 0);
		leftDirPrev = new Leap.Vector (0, 0, 0);
		rightDirPrev = new Leap.Vector (0, 0, 0);*/
		controller = new Controller();
		navigationState = "pan";
		controller.EnableGesture (Gesture.GestureType.TYPECIRCLE);
		controller.EnableGesture (Gesture.GestureType.TYPESWIPE);
		controller.EnableGesture (Gesture.GestureType.TYPEKEYTAP);
		controller.EnableGesture (Gesture.GestureType.TYPESCREENTAP);
	}
	
	void Update ()
	{
		//transform gives you main camera
		//initialize hand positions
		/*
		if (leftHandPrev.Equals( new Leap.Vector(0,0,0))) {
			leftHandPrev = leftHand.PalmPosition;
				}
		if (rightHandPrev.Equals( new Leap.Vector(0,0,0))) {
			rightHandPrev = rightHand.PalmPosition;
				}
		if (leftDirPrev.Equals( new Leap.Vector(0,0,0))) {
			leftDirPrev = leftHand.Direction;
				}
		if (rightDirPrev.Equals( new Leap.Vector(0,0,0))) {
			rightDirPrev = rightHand.Direction;
		}
		print (rightHandPrev);*/
		Frame frame = controller.Frame();
		handList = frame.Hands;
		leftHand = handList.Leftmost;
		rightHand = handList.Rightmost;
		if (facingForward ()) {
			print ("forward");
				}
		// do something with the tracking data in the frame...
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
				print(((SwipeGesture) g).ToString());
			}

		}

	}

	bool facingForward(){
		return rightHand.Direction.z < -.30 && rightHand.Direction.z > -.80
			&& leftHand.Direction.z < -.30 && leftHand.Direction.z > -.80
			&& handList.Count==2;
		}


}//eof
