using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour 
{
	public float viewRadius;
	[Range (0, 360)]
	public float viewAngle;

	public LayerMask targetMask;
	public LayerMask obstacleMask;

	public List<Transform> visibleTargets = new List<Transform>();

	void Start ()
	{
		StartCoroutine(FindTargetsWithDelay(.2f));
	}

	IEnumerator FindTargetsWithDelay (float delay)
	{
		while (true)
		{
			yield return new WaitForSeconds(delay);
			FindVisibleTargets();
		}
	}

	void FindVisibleTargets ()
	{
		visibleTargets.Clear();
		Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

		for (int i = 0; i < targetsInViewRadius.Length; i++) 
		{
			Transform target = targetsInViewRadius[i].transform;

			Vector3 directionToTarget = (target.position - transform.position).normalized;

			if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
			{
				float distanceToTarget = Vector3.Distance(transform.position, target.position);

				if (Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask) == false)
				{
					visibleTargets.Add(target);
				}
			}
		}
	}

	public Vector3 DirectionFromAngle (float angleInDegrees, bool angleIsGlobal)
	{
		if (angleIsGlobal == false) {
			angleInDegrees += transform.eulerAngles.y;
		}

		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0f, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}


}
