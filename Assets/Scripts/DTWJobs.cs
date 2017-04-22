using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UCRSuite;

public class DTWJobs : ThreadedJob
{
	List<DTW> dtwL = new List<DTW> ();

	public int animIndex = 0; // RESULT
	public int offset = 0; // RESULT

	public bool working = false; // infinite loop controller

	public AnimationPlayer ra; // data source

	protected override void ThreadFunction()
	{
		if (dtwL.Count == 0) {
			foreach (List<MDVector> l in ra.animationData)
				dtwL.Add (new DTW (l, ra.bodyParts.Length * ra.jointDimensionality, 0.05f));
		}
			
		while (working) {
			if (ra.currentQueryData.Count == ra.queryLength) {
				// create new query from the collected data
				Query q = new Query (0.05f, ra.bodyParts.Length * ra.jointDimensionality);
				for (int j = 0; j < ra.queryLength; j++) {
					q.addQueryItem (new MDVector (ra.currentQueryData [j]));
				}

				// run the query through all dtw and find the closest 
				double min = double.PositiveInfinity;	
				int minAnimationIndex = 0;

				for (int j = 0; j < dtwL.Count; j++) {
					DTWResult result = dtwL [j].warp (q);
					if (result.Distance < min) {
						min = result.Distance;
						minAnimationIndex = j;
						offset = result.Location;
					}
				}
				// output result
				animIndex = minAnimationIndex;
				Debug.Log ("Animation recognized as: #" + minAnimationIndex + " score=" + min + " offset=" + offset);
			}
		}
	}
	protected override void OnFinished()
	{

	}
}