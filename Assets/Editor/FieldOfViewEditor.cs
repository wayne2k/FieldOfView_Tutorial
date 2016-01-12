using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
	void OnSceneGUI ()
	{
		FieldOfView fov = target as FieldOfView;

		Handles.color = Color.white;
		Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.viewRadius);

		Vector3 viewAngleA = fov.DirectionFromAngle(-fov.viewAngle/2, false);
		Vector3 viewAngleB = fov.DirectionFromAngle(fov.viewAngle/2, false);

		Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.viewRadius);
		Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.viewRadius);

		Handles.color = Color.red;
		foreach (var visibleTarget in fov.visibleTargets) {
			Handles.DrawLine(fov.transform.position, visibleTarget.position);
		}
	}
}
