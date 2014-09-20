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

	string navigationState;//pan,rotate,zoom
	void Start ()
	{
		controller = new Controller();
		navigationState = "pan";
		controller.EnableGesture (Gesture.GestureType.TYPECIRCLE);
		controller.EnableGesture (Gesture.GestureType.TYPESWIPE);
	}
	
	void Update ()
	{
		Frame frame = controller.Frame();
		handList = frame.Hands;
		leftHand = handList.Leftmost;
		rightHand = handList.Rightmost;
		//print (leftHand.PalmPosition);
		//print (facingForward ());
		//print (facingForward ());
		print (facingForward ());
		print (rightHand.Direction.z + " " + leftHand.Direction.z);
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
			&& leftHand.Direction.z < -.30 && leftHand.Direction.z > -.80;
		}


}//eof
