using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
   public class FieldOfView : MonoBehaviour {

        public float ViewRadius;
        [Range (0, 360)]
        public float ViewAngle;

        public LayerMask TargetMask;
        public LayerMask ObstacleMask;

        [Header("General View Mesh settings")]
        public float MeshResolution;
        public int EdgeResolveIterations;
        public float EdgeDistanceThreshold;
        public float MaskCutawayDistance = .1f;
        public MeshFilter ViewMeshFilter;

        private List<Transform> visibleTargets = new List<Transform>();
        public List<Transform> VisibleTargets
        {
            get { return visibleTargets; }
            private set { visibleTargets = value; }
        }

        private Mesh viewMesh;

        #region Lifecycle
        private void Start () {
            StartCoroutine (SearchForTargets (.1f));

            viewMesh = new Mesh { name = "Vision Visualisation" };
            ViewMeshFilter.mesh = viewMesh;
        }

        private void LateUpdate()
        {
            DrawVisionMesh();
        }
        #endregion

        public Vector3 DirectionFromAngle (float angle, bool isAngleGlobal)
        {
            angle += isAngleGlobal ? 0f : transform.eulerAngles.y;

            return new Vector3 (Mathf.Sin (angle * Mathf.Deg2Rad), 0f, Mathf.Cos (angle * Mathf.Deg2Rad));
        }

        private IEnumerator SearchForTargets (float delay) {
            while (true) {
                Collider[] targets = Physics.OverlapSphere (transform.position, ViewRadius, TargetMask);
                visibleTargets.Clear();

                foreach (Collider target in targets) {
                    Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
                    
                    // CurrentTarget is within view angle
                    if (Vector3.Angle (transform.forward, directionToTarget) < ViewAngle / 2f) {
                        float distanceToTarget = Vector3.Distance (transform.position, target.transform.position);

                        // check for obstacles in a way
                        if (!Physics.Raycast (transform.position, directionToTarget, distanceToTarget, ObstacleMask)) {
                            visibleTargets.Add (target.transform);
                        }
                    }
                }

                yield return new WaitForSeconds (delay);
            }
        }

        #region View Mesh
        public void DrawVisionMesh()
        {
            int stepCount = Mathf.RoundToInt(ViewAngle * MeshResolution);
            float stepAngleSize = ViewAngle / stepCount;

            List<Vector3> viewPoints = new List<Vector3>();
            ViewCastInfo oldViewCast = new ViewCastInfo();

            for (int i = 0; i <= stepCount; i++)
            {
                float angle = transform.eulerAngles.y - ViewAngle / 2 + stepAngleSize * i;
                ViewCastInfo newViewCast = ViewCast(angle);

                if (i > 0)
                {
                    bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.Distance - newViewCast.Distance) > EdgeDistanceThreshold;
                    if (oldViewCast.Hit != newViewCast.Hit || (oldViewCast.Hit && newViewCast.Hit && edgeDstThresholdExceeded))
                    {
                        EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                        if (edge.PointA != Vector3.zero)
                        {
                            viewPoints.Add(edge.PointA);
                        }
                        if (edge.PointB != Vector3.zero)
                        {
                            viewPoints.Add(edge.PointB);
                        }
                    }

                }
                viewPoints.Add(newViewCast.Point);
                oldViewCast = newViewCast;
            }

            int vertexCount = viewPoints.Count + 1;
            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[(vertexCount - 2) * 3];
            vertices[0] = Vector3.zero;

            for (int i = 0; i < vertexCount - 1; i++)
            {
                vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]) + Vector3.forward * MaskCutawayDistance;

                if (i >= vertexCount - 2)
                    continue;

                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }

            // Clearing and setting up view mesh.
            viewMesh.Clear();
            viewMesh.vertices = vertices;
            viewMesh.triangles = triangles;
            viewMesh.RecalculateNormals();
        }

        EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
        {
            float minAngle = minViewCast.Angle;
            float maxAngle = maxViewCast.Angle;
            Vector3 minPoint = Vector3.zero;
            Vector3 maxPoint = Vector3.zero;

            for (int i = 0; i < EdgeResolveIterations; i++)
            {
                float angle = (minAngle + maxAngle) / 2;
                ViewCastInfo newViewCast = ViewCast(angle);

                bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.Distance - newViewCast.Distance) > EdgeDistanceThreshold;
                if (newViewCast.Hit == minViewCast.Hit && !edgeDstThresholdExceeded)
                {
                    minAngle = angle;
                    minPoint = newViewCast.Point;
                }
                else
                {
                    maxAngle = angle;
                    maxPoint = newViewCast.Point;
                }
            }

            return new EdgeInfo(minPoint, maxPoint);
        }

        ViewCastInfo ViewCast(float globalAngle)
        {
            Vector3 dir = DirectionFromAngle(globalAngle, true);
            RaycastHit hit;

            if (Physics.Raycast(transform.position, dir, out hit, ViewRadius, ObstacleMask))
            {
                return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
            }
            else
            {
                return new ViewCastInfo(false, transform.position + dir * ViewRadius, ViewRadius, globalAngle);
            }
        }

        public struct ViewCastInfo
        {
            public bool Hit;
            public Vector3 Point;
            public float Distance;
            public float Angle;

            public ViewCastInfo(bool hit, Vector3 point, float distance, float angle)
            {
                Hit = hit;
                Point = point;
                Distance = distance;
                Angle = angle;
            }
        }

        public struct EdgeInfo
        {
            public Vector3 PointA;
            public Vector3 PointB;

            public EdgeInfo(Vector3 pointA, Vector3 pointB)
            {
                PointA = pointA;
                PointB = pointB;
            }
        }
        #endregion
    }
}
