using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour {

	//how much player can see
	public float viewRadius;
	[Range(0,360)]
	public float viewAngle;

	//layer which stops our field of view
	public LayerMask obstacleMask;

	//how many raycast
	public float meshResolution;

	public MeshFilter viewMeshFilter;
	Mesh viewMesh;

	//number of deep iterations checking edges
	public int edgeResolveIterations;

	public float edgeDistanceThreshold;

	void Start() {
		viewMesh = new Mesh ();
		viewMesh.name = "View Mesh";
		viewMeshFilter.mesh = viewMesh;
	}

	void LateUpdate() {
		DrawFieldOfView ();
	}

	void DrawFieldOfView() {
		//number of rays
		int stepCount = Mathf.RoundToInt (viewAngle * meshResolution);
		//for example if we want look at 3 degrees (viewAngle) and we rays 2 times at one degree (meshResolution)
		//we have 6 of all steps=rays (stepCount) and 1/2 rays for degree (stepAngleSize)
		float stepAngleSize = viewAngle / stepCount;
		List<Vector2> viewPoints = new List<Vector2> ();
		//to find edge we need to have 2 rays at same moment
		ViewCastInfo oldViewCast = new ViewCastInfo ();

		//iterations for all steps
		for (int i = 0; i <= stepCount; i++) {
			//ray at angle
			float angle = transform.eulerAngles.z - viewAngle / 2 + stepAngleSize * i;
			ViewCastInfo newViewCast = ViewCast (angle);

			if (i > 0) {
				//secures when there is more than one edge
				//for example two rays one after one hit different obstacle
				bool edgeDistanceThresholdExceeded = Mathf.Abs (oldViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
				//checking if ray is an edge (first ray in row which hits smth or last)
				if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistanceThresholdExceeded)) {
					//finding more accurate edge (deep iteration)
					EdgeInfo edge = FindEdge (oldViewCast, newViewCast);
					if (edge.pointA != Vector2.zero) {
						viewPoints.Add (edge.pointA);
					} 
					if (edge.pointB != Vector2.zero) {
						viewPoints.Add (edge.pointB);
					}
				}
			}

			viewPoints.Add (newViewCast.point);
			oldViewCast = newViewCast;
		}

		//truly drawing part of fun
		//it draws triangles which go from one end of ray to source, next to end of second ray and back to end of first ray

		//init of vertex and triangles
		int vertexCount = viewPoints.Count + 1;
		Vector3[] vertices = new Vector3[vertexCount];
		int []triangles = new int[(vertexCount - 2) * 3];

		vertices [0] = Vector3.zero;
		for (int i = 0; i < vertexCount - 1; i++) {
			vertices [i + 1] = transform.InverseTransformPoint(viewPoints [i]);
			//setting vertexes for each triangle
			if (i < vertexCount - 2) {
				triangles [i * 3] = 0;
				triangles [i * 3 + 1] = i + 1;
				triangles [i * 3 + 2] = i + 2;
			}
		}

		//setting mesh which will be drawn
		viewMesh.Clear ();
		viewMesh.vertices = vertices;
		viewMesh.triangles = triangles;
		viewMesh.RecalculateNormals ();
	}

	//function which return info about one ray
	ViewCastInfo ViewCast(float globalAngle) {
		Vector2 direction = DirectionFromAngle (globalAngle);
		//raycast
		RaycastHit2D hit = Physics2D.Raycast (transform.position, direction, viewRadius, obstacleMask);
		//checking if ray hit smth at its way
		if (hit) {
			return new ViewCastInfo (true, hit.point, hit.distance, globalAngle);
		} else {
			return new ViewCastInfo (false, new Vector2(transform.position.x, transform.position.y) + direction*viewRadius, viewRadius, globalAngle);
		}
	}

	public Vector2 DirectionFromAngle(float angleInDegrees) {
		return new Vector2 (Mathf.Sin (angleInDegrees * Mathf.Deg2Rad), Mathf.Cos (angleInDegrees * Mathf.Deg2Rad));
	}


	EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast) {
		float minAngle = minViewCast.angle;
		float maxAngle = maxViewCast.angle;
		Vector2 minPoint = minViewCast.point;
		Vector2 maxPoint = maxViewCast.point;

		//iterations findig most accurate edge which deep depend on var edgeResolveIterations
		//it take exactly middle of angles and finds out if is it max or min angle and does it continously
		for (int i = 0; i < edgeResolveIterations; i++) {
			//middle of angles
			float angle = (minAngle + maxAngle) / 2;
			ViewCastInfo newViewCast = ViewCast (angle);

			bool edgeDistanceThresholdExceeded = Mathf.Abs (minViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
			//check if middle is min or max
			if (newViewCast.hit == minViewCast.hit && !edgeDistanceThresholdExceeded) {
				minAngle = angle;
				minPoint = newViewCast.point;
			} else {
				maxAngle = angle;
				maxPoint = newViewCast.point;
			}
		}

		return new EdgeInfo (minPoint, maxPoint);
	}

	//struct with info about ray
	public struct ViewCastInfo {
		public bool hit; //did raycast hit smth
		public Vector2 point;
		public float distance;
		public float angle;

		public ViewCastInfo(bool _hit, Vector2 _point, float _distance, float _angle) {
			hit = _hit;
			point = _point;
			distance = _distance;
			angle = _angle;
		}
	}

	//struct with info about edge
	public struct EdgeInfo {
		public Vector2 pointA; 	//closest point to the edge on obstacle
		public Vector2 pointB;	//closest point to the edge off obstacle

		public EdgeInfo(Vector2 _pointA, Vector2 _pointB) {
			pointA = _pointA;
			pointB = _pointB;
		}
	}
}