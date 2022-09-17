using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
    private EnemyFire enemyFire;

    private readonly int hashMove = Animator.StringToHash("IsMove");
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashDieIdx = Animator.StringToHash("DieIdx");
    private readonly int hashOffset = Animator.StringToHash("Offset");
    private readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed");
    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");
    private void Awake()
    {
        var player = GameObject.FindGameObjectWithTag("PLAYER");
        if(player != null)  playerTr = player.GetComponent<Transform>();
        //player를 태그로 찾아서 선언, player의 transform은 따로 지정
        
        enemyTr = GetComponent<Transform>();
        moveAgent = GetComponent<MoveAgent>();
        animator = GetComponent<Animator>();
        enemyFire = GetComponent<EnemyFire>();
        
        ws = new WaitForSeconds(0.3f);
        //코루틴 시간 생성
        animator.SetFloat(hashOffset,Random.Range(0.0f,1.0f));
        animator.SetFloat(hashWalkSpeed,Random.Range(1.0f,1.2f));
    }

    private void OnEnable()
    {
        StartCoroutine(CheckState());
        StartCoroutine(Action());
        Damage.OnPlayerDie += this.OnPlayerDie;

    }

    private void OnDisable()
    {
        Damage.OnPlayerDie -= this.OnPlayerDie;
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
                    enemyFire.isFire = false;
                    moveAgent.patrolling = true;
                    animator.SetBool(hashMove,true);
                    break;
                
                case State.TRACE:
                    enemyFire.isFire = false;
                    moveAgent.traceTarget = playerTr.position;
                    animator.SetBool(hashMove,true);
                    break;
                
                case State.ATTACK:
                    moveAgent.Stop();
                    animator.SetBool(hashMove, false);
                    if (enemyFire.isFire == false)
                        enemyFire.isFire = true;
                    break;

                case State.DIE:
                    this.gameObject.tag = "Untagged";
                    isDie = true;
                    enemyFire.isFire = false;
                    moveAgent.Stop();
                    animator.SetBool(hashMove, false);
                    animator.SetInteger(hashDieIdx,Random.Range(0,3));
                    animator.SetTrigger(hashDie);

                    GetComponent<CapsuleCollider>().enabled = false;
                    break;
            }
        }
    }

    private void Update()
    {
        animator.SetFloat(hashSpeed,moveAgent.speed);
    }

    public void OnPlayerDie()
    {
        moveAgent.Stop();
        enemyFire.isFire = false;
        StopAllCoroutines();
        
        animator.SetTrigger(hashPlayerDie);
    }
}
