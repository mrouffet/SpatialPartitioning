using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StaticGrid), true)]
public class StaticInspector : IGridInspector
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		StaticGrid grid = (StaticGrid)target;



		//if (GUILayout.Button("Refresh"))
		//	grid.OnValidate();
	}
}