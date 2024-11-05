using UnityEngine;

public class PlayerAnimationHash 
{
    // animation IDs
    public int Speed { get; private set; } // 움직임 블렌딩에 쓰이는 값
    public int Grounded { get; private set; } // 공중 -> 착지 할 때 상태 변환에 쓰이는 값
    public int Jump { get; private set; }
    public int Fall { get; private set; }
    public int MotionSpeed { get; private set; }
    public int Dodge { get; private set; }

    public PlayerAnimationHash()
    {
        Speed = Animator.StringToHash("Speed");
        Grounded = Animator.StringToHash("Grounded");
        Jump = Animator.StringToHash("Jump");
        Fall = Animator.StringToHash("Fall");
        MotionSpeed = Animator.StringToHash("MotionSpeed");//블렌드 트리의 멀티플라이어 값으로 쓰임
        Dodge = Animator.StringToHash("Dodge");
    }
}
