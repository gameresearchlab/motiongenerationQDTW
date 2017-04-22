using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UCRSuite;

public class RecordAnimation : StateMachineBehaviour {
	int currentQueryCount;
	List<MDVector> queryList = new List<MDVector>(); // we keep input in this list
	AnimationPlayer ra;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		// initiate animation recording
		ra = GameObject.Find ("AnimationRecordingManager").GetComponent < AnimationPlayer> ();
		if (ra.currentIndex < ra.getNoOfAnimations()) {
			ra.animationData.Add(new List<MDVector> ());
			Debug.Log ("Recording animation #"+ra.currentIndex+" of "+(ra.getNoOfAnimations()-1));
			ra.currentIndex++;

		}
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		// record animation (only joints specified in ra.bodyParts
		if (ra.currentIndex <= ra.getNoOfAnimations()) {
			int dimensions = ra.bodyParts.Length * ra.jointDimensionality;
			double[] dl = new double[dimensions];
			int i = 0;
			foreach (GameObject t in ra.bodyParts) {
				/*dl [i] = (double)(t.transform.position.x - t.transform.root.position.x);
				dl [i + 1] = (double)(t.transform.position.y - t.transform.root.position.y);
				dl [i + 2] = (double)(t.transform.position.z - t.transform.root.position.z);
				i += 3;*/
				dl [i] = (double)t.transform.rotation.x;
				dl [i + 1] = (double)t.transform.rotation.y;
				dl [i + 2] = (double)t.transform.rotation.z;
				//dl [i + 3] = (double)t.transform.rotation.w;
				i += 3;
			}
			MDVector v = new MDVector (dimensions, dl);
			ra.animationData[ra.animationData.Count-1].Add (v);
		} 


	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (ra.currentIndex == ra.getNoOfAnimations ())
			ra.finishedRecording = true; // recording is over

	}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
