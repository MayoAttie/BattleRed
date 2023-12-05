using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HandlePauseTool;

public class ViewRange : Subject
{
    // raycast 히트, 도착위치, 길이와 각을 저장하는 구조체.
    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;
        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }
    [Header("폴리곤 ViewRange 활성화")]
    public bool isViewRangeActive;

    // 두 간선을 저장할 구조체
    public struct Edge
    {
        public Vector3 PointA, PointB;
        public Edge(Vector3 _PointA, Vector3 _PointB)
        {
            PointA = _PointA;
            PointB = _PointB;
        }
    }

    // 시야 영역의 반지름과 시야 각도
    [SerializeField] float viewRadius;
    [Range(0, 360)]
    [SerializeField] float viewAngle;

    // 마스크 2종
    [SerializeField] LayerMask targetMask, obstacleMask;

    // Target Mask에 Ray Hit된 Transform을 보관하는 리스트
    List<Transform> visibleTargets = new List<Transform>();

    [SerializeField] float meshResolution;
    Mesh viewMesh;
    [SerializeField] MeshFilter ViewMeshFilter;

    // 이진탐색 반복 횟수.
    [SerializeField] int edgeResolveIterations;
    // ray의 길이 임계치를 정할 변수
    [SerializeField] float edgeDstThreshold;



    void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mehsh";
        ViewMeshFilter.mesh = viewMesh;

    }

    private void OnEnable()
    {
        //0.2초 간격으로 코루틴 호출
        StartCoroutine(FindTargetWithDelay(0.2f));
        // 게임매니저의 이벤트에 구독
        GameManager.OnPauseStateChanged += HandlePauseStateChanged;
    }

    private void OnDisable()
    {
        // 게임매니저의 이벤트 구독 해제
        GameManager.OnPauseStateChanged -= HandlePauseStateChanged;
    }

    private void LateUpdate()
    {
        // 폴리곤 범위 그리기 함수 호출
        if(isViewRangeActive)
            DrawFieldOfView();
    }

    #region 부채꼴, 캐릭터 유닛시야 코드
    IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            // Range 탐색 함수 호출
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        // ViewRadius를 반지름으로 한 원 영역 내 TargetMask레이어인 콜라이더를 모두 가져옵니다.
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            //플레이어와 Forward와 target이 이루는 각이 설정한 각도 내라면
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.transform.position);
                // 타겟으로 가는 레이캐스트에 obstacleMask가 걸리지 않으면 VisibleTargets에 Add합니다.
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);

                }
            }
        }
        // 변경된 visibleTargets 리스트를 Observer들에게 알립니다.
        NotifyGetEnemyFind(visibleTargets);
    }

    //y축 오일러 각을 3차원 1방향 벡터로 변환합니다.
    public Vector3 DirFromAngle(float angleDegrees, bool anglesGlobal)
    {
        if (!anglesGlobal)
        {
            angleDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Cos((-angleDegrees + 90) * Mathf.Deg2Rad), 0, Mathf.Sin((-angleDegrees + 90) * Mathf.Deg2Rad));
    }
    #endregion

    #region Range범위 폴리곤 표시
    void DrawFieldOfView()
    {
        // 시야각을 구성하는 레이 갯수
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        // 레이 하나당 처리할 각도
        float stepAngleSize = viewAngle / stepCount;
        // 시야에 포함되는 정점 좌표를 저장할 리스트
        List<Vector3> viewPoints = new List<Vector3>();
        // 이전 레이캐스트 결과를 저장할 변수
        ViewCastInfo prevViewCast = new ViewCastInfo();

        for (int i = 0; i <= stepCount; i++)
        {
            // 현재 레이 각도
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            // 이전 레이캐스트 정보와 비교하여 정점 보간할 부분 찾기
            if (i > 0)
            {
                bool edgeDstThresholdExceed = Mathf.Abs(prevViewCast.dst - newViewCast.dst) > edgeDstThreshold;

                // 이전 레이와 현재 레이의 장애물 충돌 여부와 거리차이, edgeDstThreshold 값을 비교하여 보간할 부분 찾기
                if (prevViewCast.hit != newViewCast.hit || (prevViewCast.hit && newViewCast.hit && edgeDstThresholdExceed))
                {
                    // 보간할 부분의 좌표 찾기
                    Edge edge = FindEdge(prevViewCast, newViewCast);

                    // 보간할 부분 좌표가 zero가 아닌 경우에만 viewPoints 리스트에 추가
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

            viewPoints.Add(newViewCast.point);
            prevViewCast = newViewCast;
        }

        // 시야에 포함되는 정점 갯수
        int vertexCount = viewPoints.Count + 1;
        // 시야에 포함되는 정점 좌표 배열
        Vector3[] vertices = new Vector3[vertexCount];
        // 시야에 포함되는 삼각형 배열
        int[] triangles = new int[(vertexCount - 2) * 3];
        // 원점(캐릭터 위치) 좌표 추가
        vertices[0] = Vector3.zero;
        // viewPoints 리스트의 좌표를 캐릭터 기준 로컬 좌표계로 변환하여 vertices 배열에 추가
        for (int i = 0; i < vertexCount - 1; i++)
        {
            // viewPoints에서 각 점의 위치를 가져오고, InverseTransformPoint를 사용하여 로컬 좌표계에서의 상대적인 위치를 가져옵니다.
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
            if (i < vertexCount - 2)
            {
                // 삼각형들의 인덱스를 설정합니다.
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }
        // 기존에 그려졌던 뷰 메시를 지웁니다.
        viewMesh.Clear();
        // 새로운 뷰 메시의 점들을 설정합니다.
        viewMesh.vertices = vertices;
        // 새로운 뷰 메시의 삼각형을 설정합니다.
        viewMesh.triangles = triangles;
        // 새로운 뷰 메시의 법선 벡터를 재계산합니다.
        viewMesh.RecalculateNormals();
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;
        //플레이어 캐릭터의 좌표 위치를 보정하여 0.4f만큼 y축을 올려 ray를 쏩니다.
        Vector3 raycastOrigin = transform.position + Vector3.up * 0.4f;

        if (Physics.Raycast(raycastOrigin, dir, out hit, viewRadius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    Edge FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        // 시야 캐스트 각도
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        // 포인트 초기화
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++) // 가장자리 해결 반복문
        {
            // 중간 각도 계산
            float angle = minAngle + (maxAngle - minAngle) / 2;

            // 중간 각도에 대한 시야 캐스트 정보 가져오기
            ViewCastInfo newViewCast = ViewCast(angle);

            // 중간 각도 계산
            bool edgeDstThresholdExceed = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceed)
            {
                // 가장자리가 아니라고 판단되는 경우
                minAngle = angle;               // 최소 각도 업데이트
                minPoint = newViewCast.point;   // 최소 각도 업데이트
            }
            else
            {
                // 가장자리인 경우
                maxAngle = angle;               // 최대 각도 업데이트
                maxPoint = newViewCast.point;   // 최대 포인트 업데이트
            }
        }
        // 최소 포인트와 최대 포인트로 Edge 객체 생성하여 반환
        return new Edge(minPoint, maxPoint);
    }


    #endregion

}
