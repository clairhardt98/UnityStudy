using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum State
    {
        PATROL,
        TRACE,
        ATTACK,
        DIE
    }//상태 저장 변수
    public State state = State.PATROL;

    private Transform playerTr;
    private Transform enemyTr;

    private Animator animator;
    //player와 enemy의 tr
    public float attackDist;
    public float traceDist;
    
    public bool isDie = false;
    
    private WaitForSeconds ws; 
    private MoveAgent moveAgent;

    private readonly int hashMove = Animator.StringToHash("IsMove");
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private void Awake()
    {
        var player = GameObject.FindGameObjectWithTag("PLAYER");
        if(player != null)  playerTr = player.GetComponent<Transform>();
        //player를 태그로 찾아서 선언, player의 transform은 따로 지정
        
        enemyTr = GetComponent<Transform>();
        moveAgent = GetComponent<MoveAgent>();
        animator = GetComponent<Animator>();
        
        ws = new WaitForSeconds(0.3f);
        //코루틴 시간 생성
    }

    private void OnEnable()
    {
        StartCoroutine(CheckState());
        StartCoroutine(Action());
    }

    IEnumerator CheckState()
    {
        while (!isDie)
        {
            if(state == State.DIE)  yield break;

            float dist = Vector3.Distance(playerTr.position, enemyTr.position);
            
            if (dist <= attackDist)
            {
                state = State.ATTACK;
            }
            else if (dist <= traceDist) 
            {
                state = State.TRACE;
            }
            else
            {
                state = State.PATROL;
            }
            yield return ws;
        }
    }

    IEnumerator Action()
    {
        while (!isDie)
        {
            yield return ws;
            switch (state)
            {
                case State.PATROL:
                    moveAgent.patrolling = true;
                    animator.SetBool(hashMove,true);
                    break;
                
                case State.TRACE:
                    moveAgent.traceTarget = playerTr.position;
                    animator.SetBool(hashMove,true);
                    break;
                
                case State.ATTACK:
                    moveAgent.Stop();
                    animator.SetBool(hashMove, false);
                    break;
                
                case State.DIE:
                    moveAgent.Stop();
                    break;
            }
        }
    }

    private void Update()
    {
        animator.SetFloat(hashSpeed,moveAgent.speed);
    }
}
