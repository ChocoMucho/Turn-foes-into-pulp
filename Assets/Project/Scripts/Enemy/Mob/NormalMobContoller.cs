using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NormalMobContoller : MonoBehaviour, IBattle
{
    [SerializeField] private Transform _player;
    [SerializeField] private float _attackDistance = 2.0f;

    private NavMeshAgent _agent;
    private Animator _animator;

    // animation IDs
    private int _animIDSpeed;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _agent.stoppingDistance = _attackDistance;
    }

    // Start is called before the first frame update
    void Start()
    {
        AssignAnimationIDs();
    }

    // Update is called once per frame
    void Update()
    {
        AttackCheck();
        WalkAnimationBlend();
    }

    private void AssignAnimationIDs() // �ִϸ��̼� id�� string����int��
    {
        _animIDSpeed = Animator.StringToHash("Speed"); 
    }

    private void AttackCheck()
    {
        float distance = Vector3.Distance(transform.position, _player.position);
        if (distance > _attackDistance)
        {
            _agent.SetDestination(_player.position);
        }
        else
        {
            OnAttack();
        }
    }

    private void WalkAnimationBlend()
    {
        Debug.Log($"������Ʈ �ӷ� {_agent.velocity.magnitude}");
        _animator.SetFloat(_animIDSpeed, _agent.velocity.magnitude);
    }

    public void OnAttack()
    {
        // ���� �ִϸ��̼� ������ Ʈ���� �ߵ�
        Debug.Log("����");
    }
    public void OnDamage()
    {

    }
}
