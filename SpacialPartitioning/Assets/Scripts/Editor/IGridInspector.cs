using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IGrid), true)]
public class IGridInspector : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		IGrid grid = (IGrid)target;

		if (GUILayout.Button("Refresh"))
			grid.OnValidate();
	}
}