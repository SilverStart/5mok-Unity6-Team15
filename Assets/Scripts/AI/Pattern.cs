using UnityEngine;

public enum PatternType
{
    None,
    Overline,               // 장목
    DoubleFour,             // 사사
    OpenDoubleThree,        // 열린 삼삼
    Five,                   // 오목
    OpenFourThree,          // 사가 열린 사삼
    OpenFour,               // 열린 사
    ClosedFourThree,        // 사가 닫힌 사삼
    BlockableFourThree,     // 막을 수 있는 사삼
    JumpOpenFour,           // 한 칸 띄어진 열린 사
    OpenThreeTwo,           // 열린 삼이
    Open222,                // 열린 이이이
    OpenThree,              // 열린 삼
    JumpOpenThree,          // 한 칸 띄어진 열린 삼
    ClosedFour,             // 막힌 사
    JumpClosedFour,         // 한 칸 띄어진 막힌 사
    JumpClosedThree,        // 한 칸 띄어진 막힌 삼
    ClosedThree,            // 막힌 삼
    OpenTwo,                // 열린 이
    HalfOpenTwo             // 막힌 열린 이
}

public struct PatternTemplate
{
    public readonly PatternType patternType;
    public readonly string Template;
    public readonly int Score;

    public PatternTemplate(PatternType patternType, string template, int score)
    {
        this.patternType = patternType;
        this.Template = template;
        this.Score = score;
    }
}

public static class PatternContainer
{
    // 단일 라인 패턴 템플릿 (복합 패턴은 여기서 바로 판정하지 않음)
    public static readonly PatternTemplate[] LineTemplates =
    {
        new PatternTemplate(PatternType.None, "", 0),

        // -----------------------
        // 6개 이상 연속: 장목(Overline)
        // -----------------------
        new PatternTemplate(PatternType.Overline, "SSSSSS", 2000),
        new PatternTemplate(PatternType.Overline, "SSSSSSS", 2000), // 여유 (라인이 더 길면 잡힘)

        // -----------------------
        // 정확히 5: 오목(Five)
        // (주의) Overline과 구분하려면: 먼저 Overline 체크 후 Five 체크
        // -----------------------
        new PatternTemplate(PatternType.Five, "SSSSS", 2000),

        // -----------------------
        // 열린 사: E S S S S E
        // -----------------------
        new PatternTemplate(PatternType.OpenFour, "ESSSSE", 1500),

        // -----------------------
        // 막힌 사: 한쪽이 B
        // -----------------------
        new PatternTemplate(PatternType.ClosedFour, "BSSSSE", 200),
        new PatternTemplate(PatternType.ClosedFour, "ESSSSB", 200),

        // -----------------------
        // 한 칸 띄어진 열린 사 (JumpOpenFour)
        // 대표 형태들:
        //   E S S S E S E
        //   E S E S S S E
        //   E S S E S S E
        // -----------------------
        new PatternTemplate(PatternType.JumpOpenFour, "ESSSESE", 660), // ESSSESE
        new PatternTemplate(PatternType.JumpOpenFour, "ESESSSE", 660), // ESESSSE
        new PatternTemplate(PatternType.JumpOpenFour, "ESSESSE", 660), // ESSESSE

        // -----------------------
        // 한 칸 띄어진 막힌 사 (JumpClosedFour)
        // 위 JumpOpenFour에서 한쪽 끝이 B로 막힌 변형들
        // -----------------------
        new PatternTemplate(PatternType.JumpClosedFour, "BSSSESE", 190), // BSSSESE
        new PatternTemplate(PatternType.JumpClosedFour, "ESSSESB", 190), // ESSSESB

        new PatternTemplate(PatternType.JumpClosedFour, "BSESSSE", 190), // BSESSSE
        new PatternTemplate(PatternType.JumpClosedFour, "ESESSSB", 190), // ESESSSB

        new PatternTemplate(PatternType.JumpClosedFour, "BSSESSE", 190), // BSSESSE
        new PatternTemplate(PatternType.JumpClosedFour, "ESSESSB", 190), // ESSESSB

        // -----------------------
        // 열린 삼: E S S S E
        // -----------------------
        new PatternTemplate(PatternType.OpenThree, "ESSSE", 400),

        // -----------------------
        // 막힌 삼: B S S S E  /  E S S S B
        // -----------------------
        new PatternTemplate(PatternType.ClosedThree, "BSSSE", 60),
        new PatternTemplate(PatternType.ClosedThree, "ESSSB", 60),

        // -----------------------
        // 한 칸 띄어진 열린 삼 (JumpOpenThree)
        // 대표 형태:
        //   E S S E S E
        //   E S E S S E
        // -----------------------
        new PatternTemplate(PatternType.JumpOpenThree, "ESSESE", 360),
        new PatternTemplate(PatternType.JumpOpenThree, "ESESSE", 360),

        // -----------------------
        // 한 칸 띄어진 막힌 삼 (JumpClosedThree)
        // 위에서 한쪽 끝이 B로 막힌 변형들
        // -----------------------
        new PatternTemplate(PatternType.JumpClosedThree, "BSSESE", 120),
        new PatternTemplate(PatternType.JumpClosedThree, "ESSESB", 120),
        new PatternTemplate(PatternType.JumpClosedThree, "BESESSE", 120),
        new PatternTemplate(PatternType.JumpClosedThree, "ESESSB", 120),

        // -----------------------
        // 열린 이: E S S E
        // -----------------------
        new PatternTemplate(PatternType.OpenTwo, "ESSE", 40),

        // -----------------------
        // 막힌 열린 이 (HalfOpenTwo)
        // 한쪽이 막혀 있는 형태들: B S S E / E S S B
        // -----------------------
        new PatternTemplate(PatternType.HalfOpenTwo, "BSSE", 30),
        new PatternTemplate(PatternType.HalfOpenTwo, "ESSB", 30),
    };
}