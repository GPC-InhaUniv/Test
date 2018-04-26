﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerCtrl : MonoBehaviour {

    const float RayCastMaxDistance = 100.0f;

    GameRuleCtrl gameRuleCtrl;
    InputManager inputManager;
    CharacterStatus status;
    CharaAnimation charaAnimation;
    Transform attackTarget;
    public float attackRange = 1.5f;

    public GameObject hitEffect;
    TargetCursor targetCursor;

    public AudioClip deathSeClip;
    AudioSource deathSeAudio;
    public new NetworkView networkView;

    public float rushSpeed = 0.1f;
    public float rotSpeed = 10000.0f;
    Vector3 destination;

    enum State
    {
        Walking,
        Attacking,
        Died,
        Whirlwinding,
        Rushing,
    };

    State state = State.Walking;
    State nextState = State.Walking;

    // Use this for initialization
    void Start () {
        status = GetComponent<CharacterStatus>();
        charaAnimation = GetComponent<CharaAnimation>();
        inputManager = FindObjectOfType<InputManager>();
        gameRuleCtrl = FindObjectOfType<GameRuleCtrl>();

        targetCursor = FindObjectOfType<TargetCursor>();
        targetCursor.SetPosition(transform.position);

        deathSeAudio = gameObject.AddComponent<AudioSource>();
        deathSeAudio.loop = false;
        deathSeAudio.clip = deathSeClip;

        //networkView = GetComponent<NetworkView>();
        destination = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        // 관리 오브젝트가 아니면 아무것도 하지 않는다.
        if (!networkView.isMine) return;

        switch(state)
        {
            case State.Walking:
                Walking(); break;
            case State.Attacking:
                Attacking(); break;
            case State.Whirlwinding:
                Whirlwinding(); break;
            case State.Rushing:
                Rushing(); break;
        }
        if(state != nextState)
        {
            state = nextState;
            switch(state)
            {
                case State.Walking:
                    WalkStart(); break;
                case State.Attacking:
                    AttackStart(); break;
                case State.Died:
                    Died(); break;
                case State.Whirlwinding:
                    WhirlwindStart(); break;
                case State.Rushing:
                    RushStart(); break;
            }
        }
	}

    private void OnNetworkInstantiate(NetworkMessageInfo info)
    {
        networkView = GetComponent<NetworkView>();
        if (!networkView.isMine)
        {
            CharacterMove move = GetComponent<CharacterMove>();
            Destroy(move);

            AttackArea[] attackAreas = GetComponentsInChildren<AttackArea>();
            foreach (AttackArea attackArea in attackAreas)
            {
                Destroy(attackArea);
            }

            AttackAreaActivator attackAreaActivator = GetComponent<AttackAreaActivator>();
            Destroy(attackAreaActivator);
        }
    }

    void ChangeState(State nextState)
    {
        this.nextState = nextState;
    }

    void Died()
    {
        status.died = true;
        gameRuleCtrl.GameOver();
        deathSeAudio.Play();
        Invoke("DelayedDestory", 8.0f);
    }

    void DelayedDestory()
    {
        Network.Destroy(gameObject);
        Network.RemoveRPCs(networkView.viewID);
    }

    void Damage(AttackArea.AttackInfo attackInfo)
    {
        GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity) as GameObject;
        effect.transform.localPosition = transform.position + new Vector3(0.0f, 0.5f, 0.0f);
        Destroy(effect, 0.3f);

        if (networkView.isMine)
            DamageMine(attackInfo.attackPower);
        else
            networkView.RPC("DamageMine", networkView.owner, attackInfo.attackPower);
    }

    [RPC]
    void DamageMine(int damage)
    {
        status.HP -= damage;
        if(status.HP <= 0)
        {
            status.HP = 0;
            ChangeState(State.Died);
        }
    }

    void AttackStart()
    {
        StateStartCommon();
        status.attacking = true;

        targetDirection = (attackTarget.position - transform.position).normalized;
        SendMessage("SetDirection", targetDirection);
        SendMessage("StopMove");
    }

    void Attacking()
    {
        if (charaAnimation.IsAttacked())
            ChangeState(State.Walking);
    }

    Vector3 targetDirection;
    void RushStart()
    {
        StateStartCommon();
        status.attacking = true;
        targetDirection = (attackTarget.position - transform.position).normalized;
        SendMessage("SetDirection", targetDirection);
        SendMessage("StopMove");
    }

    void Rushing()
    {
        transform.Translate(targetDirection * rushSpeed * Time.deltaTime);
        //transform.position = Vector3.MoveTowards(transform.position, transform.position + targetDirection, rushSpeed * Time.deltaTime);

        Attacking();
    }

    void WhirlwindStart()
    {
        StateStartCommon();
        status.attacking = true;
        SendMessage("StopMove");
    }

    void Whirlwinding()
    {
        Attacking();
        transform.RotateAround(transform.position, new Vector3(0, 300.0f, 0), rotSpeed * Time.deltaTime);
    }

    void WalkStart()
    {
        StateStartCommon();
    }

    void Walking()
    {
        if(inputManager.Clicked() == 1) // 마우스 왼쪽
        {
            Ray ray = Camera.main.ScreenPointToRay(inputManager.GetCursorPosition());
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, RayCastMaxDistance, 
                1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("EnemyHit")))
            {
                if(hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    SendMessage("SetDestination", hitInfo.point);
                    targetCursor.SetPosition(hitInfo.point);
                }
                if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("EnemyHit"))
                {
                    // 수평 거리를 체크해서 공격할지 결정한다.
                    Vector3 hitPoint = hitInfo.point;
                    hitPoint.y = transform.position.y;
                    float distance = Vector3.Distance(hitPoint, transform.position);

                    if (distance < attackRange)
                    {
                        attackTarget = hitInfo.collider.transform;
                        targetCursor.SetPosition(attackTarget.position);
                        ChangeState(State.Attacking);
                    }
                    else
                    {
                        SendMessage("SetDestination", hitInfo.point);
                        targetCursor.SetPosition(hitInfo.point);
                    }
                }
            }
        }
        else if(inputManager.Clicked() == 2) // 마우스 오른쪽
        {
            Ray ray = Camera.main.ScreenPointToRay(inputManager.GetCursorPosition());
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, RayCastMaxDistance,
                1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("EnemyHit")))
            {
                if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    SendMessage("SetDestination", hitInfo.point);
                    targetCursor.SetPosition(hitInfo.point);
                }
                if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("EnemyHit"))
                {
                    // 수평 거리를 체크해서 공격할지 결정한다.
                    Vector3 hitPoint = hitInfo.point;
                    hitPoint.y = transform.position.y;
                    float distance = Vector3.Distance(hitPoint, transform.position);


                    attackTarget = hitInfo.collider.transform;
                    targetCursor.SetPosition(attackTarget.position);
                    ChangeState(State.Rushing);
                }
            }
        }
        else if(inputManager.Clicked() == 11) // 키보드 1
        {
            ChangeState(State.Whirlwinding);
        }
    }

    void StateStartCommon()
    {
        status.attacking = false;
        status.died = false;
    }
}
