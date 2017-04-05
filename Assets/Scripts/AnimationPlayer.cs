using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UCRSuite;

public class AnimationPlayer : MonoBehaviour {

	public GameObject[] bodyParts;
	public int jointDimensionality = 4;
	public int queryLength = 10; // how many animation frames we compare
	public DTWJobs job = null;
	private int previousTrigger = 0;
	public bool finishedRecording = false;
	public GameObject sourceCharacter;
	public GameObject destinationCharacter;
	public Animator animator;
	public int currentIndex = 0; 
	private int max; // number of animations to record
	public List<List<MDVector>> animationData = new List<List<MDVector>>();
	public List<MDVector> currentQueryData = new List<MDVector>();

	public int getNoOfAnimations() {
		return max;
	}
	void Start(){

		// collect those body parts that we are going to track. 
		// they need to be tagged "bodyPart"s
		bodyParts = GameObject.FindGameObjectsWithTag ("bodyPart") ;
		animator = destinationCharacter.GetComponent<Animator>();
		// count the number of animation states to record
		max = sourceCharacter.GetComponent<Animator> ().runtimeAnimatorController.animationClips.Length;
		Debug.Log ("# of animations to record = " + max);
	}

	void Update() {
		// if all animations were recorded, create dtw for each.
		if (finishedRecording) {
			if (job == null) {
				job = new DTWJobs ();
				job.ra = this;
				job.Start ();
			}

			// read current input (here it is simulated by reading the current animation)
			int dimensions = bodyParts.Length * jointDimensionality;
			double[] dl = new double[dimensions];
			int i = 0;
			foreach (GameObject t in bodyParts) {
				/*dl [i] = (double)(t.transform.position.x - t.transform.root.position.x);
				dl [i + 1] = (double)(t.transform.position.y - t.transform.root.position.y);
				dl [i + 2] = (double)(t.transform.position.z - t.transform.root.position.z);
				i += 3;*/
				//dl [i] = Mathf.Acos (2 * Mathf.Pow(Quaternion.Dot (t.transform.rotation, t.transform.root.rotation),2) - 1);
				//i++;
				dl [i] = (double)t.transform.rotation.x;
				dl [i + 1] = (double)t.transform.rotation.y;
				dl [i + 2] = (double)t.transform.rotation.z;
				dl [i + 3] = (double)t.transform.rotation.w;
				i += 4;
			}

			// prepare data for the query
			MDVector v = new MDVector (dimensions, dl);
			currentQueryData.Add (v);

			// keep query no longer than queryLength
			if (currentQueryData.Count > queryLength)
				currentQueryData.RemoveAt (0);

			//trigger appropriate animation
			int currentTrigger = job.animIndex; // this is set by a DTWJobs (a separate thread, not safe)
			if (previousTrigger != currentTrigger) {
				animator.SetTrigger ("t" + currentTrigger.ToString());
				previousTrigger = currentTrigger;
				Debug.Log ("Triggering animation #" + currentTrigger.ToString ());

				//animator.CrossFade(job.animIndex.ToString(), 0.01f, 0, 0f);

			}

		}

	}

	void OnDestroy() {
		if (job != null) job.working = false; // stop threads
	}
}
