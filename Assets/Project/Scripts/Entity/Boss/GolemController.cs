using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GolemController : MonoBehaviour, IBattle
{
    [SerializeField] private Transform _player;
    [SerializeField] private float _attackDistance = 2.0f;
    [SerializeField] private EntityData _data;

    private NavMeshAgent _agent;
    private Animator _animator;

    // animation IDs
    private int _animIDSpeed;
    private int _animIDAttack;
    private int _animIDAttackCount;
    private int _animIDDeath;

    // State
    private bool _isAttacking = false;
    private bool _isDead = false;

    // ������
    private float _currentHp; // max���� ���߿� �����ϱ� ������.
    private float _currentDamage;

    GameManager _gameManager;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _agent.stoppingDistance = _attackDistance;
    }

    void Start()
    {
        _gameManager = GameManager.instance;
        AssignAnimationIDs();
        _currentHp = _data.Hp;
        _currentDamage = _data.Damage;
    }

    void Update()
    {
        DeadCheck();
        TraceCheck();
        WalkAnimationBlend();
    }

    private void AssignAnimationIDs() // �ִϸ��̼� id�� string����int��
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDAttack = Animator.StringToHash("Attack");
        _animIDAttackCount = Animator.StringToHash("AttackCount");
        _animIDDeath = Animator.StringToHash("Death");
    }

    public void DeadCheck()
    {
        if (_currentHp <= 0f)
        {
            _isDead = true;
            _animator.SetBool(_animIDDeath, true);
            _agent.Stop();
        }
    }

    private void TraceCheck()
    {
        Vector3 selfVector = new Vector3(transform.position.x, 0, transform.position.z); 
        Vector3 playerVector = new Vector3(_player.position.x, 0, _player.position.z); 
        float distance = Vector3.Distance(selfVector, playerVector);
        bool isFar = distance > _attackDistance;
        if (isFar)
        {
            _agent.SetDestination(_player.position);
        }
        else// ���� ���
        {   //TODO : �ָ��־ �¾ƹ���..
            _animator.SetTrigger(_animIDAttack);
            _animator.SetInteger(_animIDAttackCount, Random.Range(0, 2));
        }

        // ���� ��� && ���� �ȿ� �ȵ��� ���
        // -> ȸ��
    }

    private void WalkAnimationBlend()
    {
        _animator.SetFloat(_animIDSpeed, _agent.velocity.magnitude);
    }

    public void OnAttack() // �ִϸ��̼� �̺�Ʈ�� �߰����� ģ��.
    {
        _gameManager.Player.GetComponent<PlayerManager>().OnDamage(_data.Damage); // �������� ��� ����!

    }

    public void OnDamage(float damage)
    {
        _currentHp -= damage; //��.. �� ����ü��
        Debug.Log($"���� ü�� : {_currentHp}");
    }

}
