using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour {

	public float viewRadius;
	[Range(0, 360)]
	public float viewAngle;

	public LayerMask targetMask, obstacleMask;

	[HideInInspector]
	public List<Transform> visableTargets = new List<Transform>();

	void Start() {
		StartCoroutine ("FindTargetsWithDelay", .2f);
	}

	IEnumerator FindTargetsWithDelay(float delay) {
		while (true) {
			yield return new WaitForSeconds (delay);
			FindVisibleTargets ();
		}
	}

	void FindVisibleTargets() {
		visableTargets.Clear ();
		Collider[] targetsInViewRadius = Physics.OverlapSphere (transform.position, viewRadius, targetMask);

		for (int i = 0; i < targetsInViewRadius.Length; i++) {
			Transform target = targetsInViewRadius [i].transform;
			Vector3 directionToTarget = (target.position - transform.position).normalized;
			if (Vector3.Angle (transform.forward, directionToTarget) < viewAngle / 2) {
				float distanceToTarget = Vector3.Distance (transform.position, target.position);

				if(!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask)) {
					visableTargets.Add(target);
				}
			}
		}
	}

	public Vector3 DirectionFromAngle(float angleInDegrees, bool globalAngle) {
		if (!globalAngle) {
			angleInDegrees += transform.eulerAngles.y;
		}
		return new Vector3 (Mathf.Sin (angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos (angleInDegrees * Mathf.Deg2Rad));
	}
}
