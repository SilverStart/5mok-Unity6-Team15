using TMPro;

using UnityEngine;
using UnityEngine.Events;

using static common.Constants;

public class TimerUIManger : MonoBehaviour
{
    [SerializeField] private GameObject blackOverlay;
    [SerializeField] private TextMeshProUGUI blackText;
    [SerializeField] private TextMeshProUGUI blackTimer;

    [SerializeField] private GameObject whiteOverlay;
    [SerializeField] private TextMeshProUGUI whiteText;
    [SerializeField] private TextMeshProUGUI whiteTimer;

    [Header("Settings")]
    [SerializeField] private float turnTimeLimit = 60f;

    public UnityEvent<StoneColor> OnTurnTimeOver;

    private StoneColor currentTurn;
    private float remainingTime;
    private bool isRunning;

    void Start()
    {
        Init(turnTimeLimit);
    }

    public void StartTimer(StoneColor startingColor)
    {
        ChangeTurn(startingColor);
    }

    private void Init(float timeLimit)
    {
        blackTimer.text = CalculateTime(timeLimit);
        whiteTimer.text = CalculateTime(timeLimit);
    }

    public void SwitchTurn()
    {
        ChangeTurn(currentTurn == StoneColor.Black ? StoneColor.White : StoneColor.Black);
    }

    public void ChangeTurn(StoneColor color)
    {
        bool isBlackTurn = color == StoneColor.Black;

        blackOverlay.SetActive(isBlackTurn);
        blackText.text = isBlackTurn ? "Black's Turn" : "Black";
        blackText.color = isBlackTurn ? Color.green : Color.gray;
        blackTimer.color = isBlackTurn ? Color.white : Color.gray;

        whiteOverlay.SetActive(!isBlackTurn);
        whiteText.text = isBlackTurn ? "White" : "White's Turn";
        whiteText.color = isBlackTurn ? Color.gray : Color.green;
        whiteTimer.color = isBlackTurn ? Color.gray : Color.white;

        Init(turnTimeLimit);
        currentTurn = color;
        remainingTime = turnTimeLimit;
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    void Update()
    {
        if (!isRunning) return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            isRunning = false;
            SetTime(currentTurn, remainingTime);

            StoneColor nextTurn = (currentTurn == StoneColor.Black)
                ? StoneColor.White
                : StoneColor.Black;
            OnTurnTimeOver?.Invoke(nextTurn);
            return;
        }

        SetTime(currentTurn, remainingTime);
    }

    private void SetTime(StoneColor color, float time)
    {
        if (color == StoneColor.Black) blackTimer.text = CalculateTime(time);
        else if (color == StoneColor.White) whiteTimer.text = CalculateTime(time);
    }

    private string CalculateTime(float time)
    {
        int min = (int)time / 60;
        int sec = (int)time % 60;

        string secString = sec < 10 ? "0" + sec.ToString() : sec.ToString();
        string minString = min < 10 ? "0" + min.ToString() : min.ToString();
        return minString + " : " + secString;
    }
}
