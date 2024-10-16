using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NormalMobContoller : MonoBehaviour, IBattle
{
    [SerializeField] private Transform _player;
    [SerializeField] private float _attackDistance = 2.0f;
    [SerializeField] private EntityData _data;

    private NavMeshAgent _agent;
    private Animator _animator;

    // animation IDs
    private int _animIDSpeed;
    private int _animIDAttack;
    private int _animIDDead;

    // State
    private bool _isAttacking = false;
    private bool _isDead = false;

    // 데이터
    private float _currentHp; // max값은 나중에 생각하기 귀찮음.
    private float _currentDamage;

    GameManager _gameManager;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _agent.stoppingDistance = _attackDistance;
    }

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameManager.instance;
        AssignAnimationIDs();
        _currentHp = _data.Hp;
        _currentDamage = _data.Damage;
    }

    // Update is called once per frame
    void Update()
    {
        DeadCheck();
        TraceCheck();
        WalkAnimationBlend();
    }

    private void AssignAnimationIDs() // 애니메이션 id를 string에서int로
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDAttack = Animator.StringToHash("Attack");
        _animIDDead = Animator.StringToHash("Dead");
    }

    private void TraceCheck()
    {
        float distance = Vector3.Distance(transform.position, _player.position);
        bool isFar = distance > _attackDistance;
        if (isFar)
        {
            _agent.SetDestination(_player.position);
        }
        else// 멈춘 경우
        {   //TODO : 멀리있어도 맞아버림..
            _animator.SetTrigger(_animIDAttack);
        }

        // 멈춘 경우 && 각도 안에 안들어온 경우
        // -> 회전
    }

    private void WalkAnimationBlend()
    {
        _animator.SetFloat(_animIDSpeed, _agent.velocity.magnitude);
    }

    public void OnAttack() // 애니메이션 이벤트에 추가해줄 친구.
    {
        _gameManager.Player.GetComponent<PlayerManager>().OnDamage(_data.Damage); // 가져오는 방법 구림!

    }
    public void OnDamage(float damage)
    {
        _currentHp -= damage; //음.. 아 현재체력
        Debug.Log($"몹 체력 : {_currentHp}");
    }

    public void DeadCheck()
    {
        if(_currentHp <= 0f)
        {
            _isDead = true;
            _animator.SetBool(_animIDDead, true);
            _agent.Stop();
        }
    }
}
