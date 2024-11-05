public class Define
{
    public enum Scene
    {
        Unknown,
        Lobby,
        Battle,
    }

    public enum PlayerStates
    {
        Ground,
        Air,

        Move,
        Dodge,

        NonCombat,
        Shoot, // 이름 변경
    }
}
