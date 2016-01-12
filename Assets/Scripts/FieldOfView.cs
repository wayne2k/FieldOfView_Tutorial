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

	[HideInInspector]
	public List<Transform> visibleTargets = new List<Transform>();

	public float meshResolution;
	public MeshFilter viewMeshFilter;

	Mesh viewMesh;
	 
	void Start ()
	{
		viewMesh = new Mesh(); 
		viewMesh.name = "View Mesg";
		viewMeshFilter.mesh = viewMesh;

		StartCoroutine(FindTargetsWithDelay(.2f));
	}

	void LateUpdate ()
	{
		DrawFieldOfView();
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

	void DrawFieldOfView ()
	{
		int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
		float stepAngleSize = viewAngle / stepCount;
		List<Vector3> viewPoints = new List<Vector3>();

		for (int i = 0; i <= stepCount; i++) 
		{
			float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
			ViewCastInfo newViewCast = ViewCast(angle);
			viewPoints.Add(newViewCast.point);
		}

		int vertexCount = viewPoints.Count + 1;
		Vector3[] verticies = new Vector3[vertexCount];
		int[] triangles = new int[(vertexCount - 2) * 3];

		verticies[0] = Vector3.zero;
		for (int i = 0; i < vertexCount - 1; i++) 
		{
			verticies[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

			if (i < vertexCount - 2)
			{
				triangles[i * 3] = 0;
				triangles[i * 3 + 1] = i + 1;
				triangles[i * 3 + 2] = i + 2;
			}
		}
		viewMesh.Clear();
		viewMesh.vertices = verticies;
		viewMesh.triangles = triangles;
		viewMesh.RecalculateNormals();
	}

	ViewCastInfo ViewCast (float globalAngle)
	{
		Vector3 direction = DirectionFromAngle(globalAngle, true);
		RaycastHit hit;
		if (Physics.Raycast(transform.position, direction, out hit, viewRadius, obstacleMask))
		{
			return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
		}
		else
		{
			return new ViewCastInfo(false, transform.position + direction * viewRadius, viewRadius, globalAngle);
		}
	}

	public Vector3 DirectionFromAngle (float angleInDegrees, bool angleIsGlobal)
	{
		if (angleIsGlobal == false) {
			angleInDegrees += transform.eulerAngles.y;
		}

		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0f, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}

	// Just a different implementation of DirectionFromAngle function.
	public Vector3 VectorFromAngle (float angleInDegrees, bool angleIsGlobal)
	{
		if (angleIsGlobal == false) {
			angleInDegrees += transform.eulerAngles.y;
		}

		return Quaternion.AngleAxis(angleInDegrees, Vector3.up) * Vector3.forward;
	}

	public struct ViewCastInfo
	{
		public bool hit;
		public Vector3 point;
		public float distance;
		public float angle;

		public ViewCastInfo (bool hit, Vector3 point, float distance, float angle)
		{
			this.hit = hit;
			this.point = point;
			this.distance = distance;
			this.angle = angle;
		}
	}
}
