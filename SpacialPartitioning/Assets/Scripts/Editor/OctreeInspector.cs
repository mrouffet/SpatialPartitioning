using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Octree), true)]
public class OctreeInspector : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		Octree octree = (Octree)target;

		if (GUILayout.Button("Refresh"))
			octree.OnValidate();
	}
}