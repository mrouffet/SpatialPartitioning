using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(OctreeBehavior), true)]
public class OctreeBehaviorInspector : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		OctreeBehavior octree = (OctreeBehavior)target;

		if (GUILayout.Button("Refresh"))
			octree.OnValidate();

		if (GUILayout.Button("Query Collisions"))
		{
			List<OctreePair> collisions = octree.QueryCollisions();

			Debug.Log(" Collisions: " + collisions.Count);

			foreach (OctreePair pair in collisions)
				Debug.Log(pair.first.gameObject.name + " / " + pair.second.gameObject.name);
		}
	}
}