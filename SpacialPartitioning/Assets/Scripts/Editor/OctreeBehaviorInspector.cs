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
	}
}