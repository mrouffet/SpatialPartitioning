using UnityEngine;

[ExecuteAlways]
public class GridObj : MonoBehaviour
{
	MeshRenderer _mesh = null;
	public Bounds bounds { get { return _mesh.bounds; } }

#if UNITY_EDITOR
	Vector3 _prevPosition = Vector3.zero;
#endif

	void Awake()
	{
		_mesh = GetComponent<MeshRenderer>();

		_prevPosition = transform.position;
	}

	//void Update()
	//{
	//	// Editor update code.
	//	if(!Application.IsPlaying(gameObject))
	//	{
	//		if (_prevPosition != transform.position)
	//		{

	//		}

	//		return;
	//	}


	//	// Play update code.
	//}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireCube(bounds.center, bounds.extents * 2.0f);
	}
}
