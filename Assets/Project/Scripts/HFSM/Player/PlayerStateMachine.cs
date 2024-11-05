using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    public Player Player { get; private set; }
    public PlayerController Controller { get; private set; }
    public PlayerStateProvider StateProvider { get; private set; }
    public Animator Animator { get; private set; }
    public BaseState CurrentState { get; set; } //최상위 상태

    private void Awake()
    {
        Player = GetComponent<Player>();
        Controller = GetComponent<PlayerController>();
        StateProvider = new PlayerStateProvider(this);
        Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        CurrentState?.UpdateStates();
    }

    private void Initialize()
    {
        CurrentState = StateProvider.GetState(Define.PlayerStates.Ground);
        CurrentState?.EnterState();
    }
}
