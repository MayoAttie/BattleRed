using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathFinder : MonoBehaviour
{
    Vector3 target;
    NavMeshAgent agent;
    LineRenderer render;
    bool isOn;
    private void Awake()
    {
        target = Vector3.zero;
        isOn = false;
        agent = gameObject.GetComponent<NavMeshAgent>();
        render = gameObject.GetComponent<LineRenderer>();
        render.enabled = isOn;
    }
    private void Start()
    {
        render.startWidth = render.endWidth = 0.1f;
        //render.material.color = Color.green;
    }

    public void FindPathStart(Vector3 target)
    {
        this.target = target;
        // 이동 시작
        agent.SetDestination(target);
        // 플레이어 수동 이동
        agent.isStopped = false;
        agent.updateRotation = false;

        isOn = true;
        render.enabled = true;

        StartCoroutine(FindPathStartCoroutine());
    }

    IEnumerator FindPathStartCoroutine()
    {
        render.SetPosition(0, this.transform.position);

        while(Vector3.Distance(this.transform.position, target) > 1.5f && isOn == true)
        {
            render.SetPosition(0, this.transform.position);
            DrawPath();
            yield return null;
        }

        agent.ResetPath();
        render.enabled = false;
        isOn = false;
        yield break;
    }

    void DrawPath()
    {
        int len = agent.path.corners.Length;
        render.positionCount = len;

        for (int i = 1; i < len; i++)
        {
            render.SetPosition(i, agent.path.corners[i]);
        }

    }

    bool IsOnSet
    {
        get { return isOn; }
        set
        {
            isOn = value;
        }
    }


}
