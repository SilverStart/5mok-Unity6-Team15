public enum InputType { PlaceStone, Resign, Undo }

// 입력 결과를 담는 구조체
public struct PlayerInput
{
    public InputType Type;
    public int x;
    public int y;

    // 편의를 위한 생성자들
    public static PlayerInput Move(int x, int y) => new PlayerInput { Type = InputType.PlaceStone, x = x, y = y };
    public static PlayerInput Resign() => new PlayerInput { Type = InputType.Resign };
    public static PlayerInput Undo() => new PlayerInput { Type = InputType.Undo };
}