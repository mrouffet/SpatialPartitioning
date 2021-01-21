using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(OctreeObj), true)]
public class OctreeObjInspector : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		OctreeObj obj = (OctreeObj)target;

		if (GUILayout.Button("Query Collisions"))
		{
			List<OctreeObj> collisions = obj.QueryCollisions();

			Debug.Log(obj.gameObject.name + " Collisions: " + collisions.Count);

			foreach (OctreeObj collision in collisions)
				Debug.Log(collision.gameObject.name);
		}
	}
}