using UnityEngine;

public class PlayerAnimationHash 
{
    // animation IDs
    public int Speed { get; private set; } // ������ ������ ���̴� ��
    public int Grounded { get; private set; } // ���� -> ���� �� �� ���� ��ȯ�� ���̴� ��
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
        MotionSpeed = Animator.StringToHash("MotionSpeed");//���� Ʈ���� ��Ƽ�ö��̾� ������ ����
        Dodge = Animator.StringToHash("Dodge");
    }
}
