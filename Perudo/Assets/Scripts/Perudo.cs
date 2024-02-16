using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
public class Perudo : MonoBehaviour
{
    public TMP_Text PlayersCubes;
    public TMP_Text PlayersCubes2;
    public TMP_Text BotCubes2;
    public TMP_Text BotCubes3;
    public TMP_Text BotCubes4;
    public TMP_Text BetCount;
    // public TMP_Text BetInfo;
    public TMP_Text[] Info;
    public GameObject[] GameFinishInfo;
    public TMP_Text PlayersCountCubes;
    public TMP_Text BotCountCubes2;
    public TMP_Text BotCountCubes3;
    public TMP_Text BotCountCubes4;
    public GameObject BetResult;

    public Button CallTrueButton;
    public Button CallWrongButton;
    public Button CallWrong2Button;
    public Button CallFaceIncreaseButton;
    public Button CallFaceDeincreaseButton;
    public Button CallCountIncreaseButton;
    public Button CallCountDeincreaseButton;

    public TMP_Text TextBetCubes;

    private int currentBetFace = 1;
    private int currentFace = 1;
    private int roundIndex = 1;
    private int currentBetCount = 1;
    private int currentCount = 1;
    private int maxDiceCount = 20;
    private int currentPlayerIndex = 0;
    private TMP_Text[] _betResultInfo;
    private bool _isTrueButton;

    // Start is called before the first frame update
    void Start()
    {
        GenerateCube(5, 5, 5, 5);
        UpdateMaxCubeCount();
        _betResultInfo = BetResult.GetComponentsInChildren<TMP_Text>();
        _betResultInfo[0].gameObject.SetActive(false);
        _betResultInfo[1].gameObject.SetActive(false);
        Info[0].text = currentFace.ToString() + " : " + currentCount.ToString();
        Info[1].text = " Поднимите Ставку  ";
        Info[2].text = " Раунд : " + roundIndex; 
       // BetInfo.text = "Раунд:" + roundIndex;
        CheckBet();
    }


    private void GenerateCube(int count1, int count2, int count3, int count4)
    {
        PlayersCubes.text = GenerateCubeForOnePlayer(count1);
        ShowAnimForCube(PlayersCubes);
        PlayersCubes2.text = PlayersCubes.text;
        BotCubes2.text = GenerateCubeForOnePlayer(count2);
        BotCubes3.text = GenerateCubeForOnePlayer(count3);
        BotCubes4.text = GenerateCubeForOnePlayer(count4);
    }


    private string GenerateCubeForOnePlayer(int count)
    {
        string cubeResult = "";
        for (int i = 0; i < count; i++)
        {
            cubeResult += Random.Range(1, 7) + " ";
        }
        return cubeResult;
    }
    private void UpdateUI()
    {
        CheckWin();
        //Debug.Log(currentPlayerIndex);
        if (currentPlayerIndex > 0)
        {
            TurnOnButtons(false);
         if( GetCubeCount(currentPlayerIndex) > 0)
            {
                StartCoroutine(Bot());
            }
            else
            {
                currentPlayerIndex++;
                ChangeRound();
                UpdateUI();
            }

           
        }
        else
        {
            TurnOnButtons(true);
        }

    }

    IEnumerator Bot()
    {
        yield return new WaitForSeconds(5f);

        bool callBluff = (Random.Range(0f, 1f)/ GetCubeCount(currentPlayerIndex)) * Random.Range(0f, 0.5f) < 0.5f;
        if (callBluff && currentCount != 1 && currentFace != 1)
        {
            CalltrueButton();

        }
        else
        {
            bool betCountCheckForBot = (GetCubeCount(currentPlayerIndex) * 2f < currentCount);

            if ((currentCount == maxDiceCount && currentFace == 6) || betCountCheckForBot)
            {
                CallfalseButton();
            }
            else
            {
                int BetRandom = Random.Range(0, 3);
                if (BetRandom == 0 || BetRandom == 2 || currentFace == 6)
                {
                    currentBetCount = Random.Range(currentCount + 1, maxDiceCount + 1);
                }
                if (BetRandom == 1 || BetRandom == 2 || currentCount == maxDiceCount)
                {
                    currentBetFace = Random.Range(currentFace, 7);
                }
                CallUpBetButton();
                
            } 
        }

    }

    private void ChangeRound()
    {
        if (currentPlayerIndex > 3)
        {
            currentPlayerIndex = 0;

        }
        // BetInfo.text = " Раунд:" + roundIndex;
        // Debug.Log(" Ходит бот №" + currentPlayerIndex);
        Info[3].text = (currentPlayerIndex + 1).ToString();
        CheckBet();
    }
     
    public void OnCubeIncrease()

    {
        currentBetFace++;
        currentBetFace = Mathf.Clamp(currentBetFace, 1, 6);
        TextBetCubes.text = currentBetFace.ToString();
        //BetInfo.text += "\nГрань изменена на: " + (currentBetFace - currentFace);
        CheckBet();
    }
    public void OnCubeDeincrease()
    {
        currentBetFace--;
        currentBetFace = Mathf.Clamp(currentBetFace, 1, 6);
        TextBetCubes.text = currentBetFace.ToString();
        //BetInfo.text += "\nГрань изменена на: " + (currentBetFace - currentFace);
        CheckBet();
    }

    public void OnCountIncrease()
    {
        currentBetCount++;
        currentBetCount = Mathf.Clamp(currentBetCount, 1, maxDiceCount);
        BetCount.text = currentBetCount.ToString();
       // BetInfo.text += "\nКоличество кубов изменено на: " + (currentBetCount - currentCount);
        CheckBet();
    }
    public void OnCountDeincrease()
    {
        currentBetCount--;
        currentBetCount = Mathf.Clamp(currentBetCount, 1, maxDiceCount);
        BetCount.text = currentBetCount.ToString();
        //BetInfo.text += "\nКоличество кубов изменено на: " + (currentBetCount - currentCount);
        CheckBet();
    }

    public void CalltrueButton()
    {
        //Debug.Log(" Верю ");
        TurnOnButtons(false);
        _isTrueButton = true;
        Info[1].text = " Игрок " + (currentPlayerIndex + 1) + " ПОВЕРИЛ";

        StartCoroutine(ShowResult());
    }
    public void CallfalseButton()
    {
        //Debug.Log(" Верю ");
        TurnOnButtons(false);
        _isTrueButton = false;
        Info[1].text = " Игрок " + (currentPlayerIndex + 1) + " HE ПОВЕРИЛ";

        StartCoroutine(ShowResult());
    }
    IEnumerator ShowResult()
    {
        int betResult = CheckCubes();
        if ((betResult >= currentCount && _isTrueButton) || ( betResult < currentCount && !_isTrueButton))
        {
            _betResultInfo[0].gameObject.SetActive(true);
        }
        else
        {
            _betResultInfo[1].gameObject.SetActive(true);
        }
        ChangeCubePlayersInfo(true);

        yield return new WaitForSeconds(5f);

        DeliteCubeFromPlayer(betResult);
        _betResultInfo[0].gameObject.SetActive(false);
        _betResultInfo[1].gameObject.SetActive(false);
        ChangeCubePlayersInfo(false);
        GenerateCube(GetCubeCount(0), GetCubeCount(1), GetCubeCount(2), GetCubeCount(3));
        currentBetCount = 1;
        currentBetFace = 1;
        currentCount = currentBetCount;
        currentFace = currentBetFace;
        BetCount.text = currentCount.ToString();
        TextBetCubes.text = currentFace.ToString();
        Info[0].text = currentFace.ToString() + " : " + currentCount.ToString();
        roundIndex++;
        Info[2].text = " Раунд : " + roundIndex;

        currentPlayerIndex++;
        ChangeRound();
        UpdateUI();
    }
    public void ChangeCubePlayersInfo(bool flag)
    {
        PlayersCubes.gameObject.SetActive(flag);
        ShowAnimForCube(PlayersCubes);
        BotCubes2.gameObject.SetActive(flag);
        BotCubes3.gameObject.SetActive(flag);
        BotCubes4.gameObject.SetActive(flag);

        PlayersCountCubes.gameObject.SetActive(!flag);
        BotCountCubes2.gameObject.SetActive(!flag);
        BotCountCubes3.gameObject.SetActive(!flag);
        BotCountCubes4.gameObject.SetActive(!flag);

    }
    public void CallUpBetButton()
    {
        currentCount = currentBetCount;
        currentFace = currentBetFace;
        BetCount.text = currentCount.ToString();
        TextBetCubes.text = currentFace.ToString();
        _isTrueButton = false;
        //Debug.Log(" Текущая ставка грани: " + currentFace);
        //Debug.Log(" Текущая ставка количества кубов:" + currentCount);
        Info[0].text = currentFace.ToString() + " : " + currentCount.ToString();
        Info[1].text = " Игрок " + (currentPlayerIndex + 1) + " поднял ставку ";
        currentPlayerIndex++;
        ChangeRound();
        UpdateUI();
    }

    private int CheckCubes()
    {
        string[] playersResult = PlayersCubes.text.Split(' ');
        string[] bot2Result = BotCubes2.text.Split(' ');
        string[] bot3Result = BotCubes3.text.Split(' ');
        string[] bot4Result = BotCubes4.text.Split(' ');
        int result = 0;
        for (int i = 0; i < playersResult.Length; i++)
        {
            if (playersResult[i] == currentFace.ToString())
            {
                result++;
            }
        }
        for (int i = 0; i < bot2Result.Length; i++)
        {
            if (bot2Result[i] == currentFace.ToString())
            {
                result++;
            }
        }

        for (int i = 0; i < bot3Result.Length; i++)
        {
            if (bot3Result[i] == currentFace.ToString())
            {
                result++;
            }
        }

        for (int i = 0; i < bot4Result.Length; i++)
        {
            if (bot4Result[i] == currentFace.ToString())
            {
                result++;
            }
        }
        return result;
    }
    private void CheckBet()
    {
        if (currentBetFace > currentFace)
        {
            CallFaceDeincreaseButton.interactable = true;
        }
        else
        {
            CallFaceDeincreaseButton.interactable = false;
        }
        if (currentBetFace < 6)
        {
            CallFaceIncreaseButton.interactable = true;
        }
        else
        {
            CallFaceIncreaseButton.interactable = false;
        }

        if (currentBetCount > currentCount || currentBetFace > currentFace)
        {
            CallWrongButton.interactable = true;
        }
        else
        {
            CallWrongButton.interactable = false;
        }
        if (currentBetCount > currentCount || (currentBetFace > currentFace && currentBetCount > 1))
        {
            CallCountDeincreaseButton.interactable = true;
        }
        else
        {
            CallCountDeincreaseButton.interactable = false;
        }
        if (currentBetCount < maxDiceCount)
        {
            CallCountIncreaseButton.interactable = true;
        }
        else
        {
            CallCountIncreaseButton.interactable = false;
        }

        if (currentBetFace == currentFace && currentBetCount < currentCount)
        {
            CallCountDeincreaseButton.interactable = false;
            while (currentBetCount < currentCount)
            {
                OnCountIncrease();
            }


        }
        if(currentCount == 1 && currentFace == 1)
        {
            CallTrueButton.interactable = false;
            CallWrong2Button.interactable = false;
        }
        else
        {
            CallTrueButton.interactable = true;
            CallWrong2Button.interactable = true;
}
    }

    private int GetCubeCount(int playerIndex)
    {
        int CountCube = 0;
        if (playerIndex == 0)
        {
            CountCube = System.Convert.ToInt32(PlayersCountCubes.text);
        }
        else if (playerIndex == 1)
        {
            CountCube = System.Convert.ToInt32(BotCountCubes2.text);
        }
        else if (playerIndex == 2)
        {
            CountCube = System.Convert.ToInt32(BotCountCubes3.text);
        }
        else if (playerIndex == 3)
        {
            CountCube = System.Convert.ToInt32(BotCountCubes4.text);
        }
        return CountCube;
    }
    private void DeliteCubeFromPlayer(int betResult)
    {
        if (!(betResult >= currentCount && _isTrueButton) ) 
        { 
            if (currentPlayerIndex == 0)
            {
                int countCube1 = GetCubeCount(0);
                PlayersCountCubes.text = (countCube1 - 1).ToString();
                if((countCube1 - 1) < 1)
                {
                    PlayersCountCubes.color = Color.grey;
                    Info[1].text = " Вы Проиграли!";
                    GameFinishInfo[0].SetActive(true);
                    GameFinishInfo[1].SetActive(true);
                    currentPlayerIndex = -1;
                    Info[4].text = Info[0].text;
                    Info[5].text = Info[2].text;

                }

            }
            if (currentPlayerIndex == 1)
            {
                int countCube1 = GetCubeCount(1);
                BotCountCubes2.text = (countCube1 - 1).ToString();
                if ((countCube1 - 1) < 1)
                {
                    BotCountCubes2.color = Color.grey;
                    Info[1].text = " Игрок 2 вышел ";
                }
            }
            if (currentPlayerIndex == 2)
            {
                int countCube1 = GetCubeCount(2);
                BotCountCubes3.text = (countCube1 - 1).ToString();
                if ((countCube1 - 1) < 1)
                {
                    BotCountCubes3.color = Color.grey;
                    Info[1].text = " Игрок 3 вышел ";
                }
            }
            if (currentPlayerIndex == 3)
            {
                int countCube1 = GetCubeCount(3);
                BotCountCubes4.text = (countCube1 - 1).ToString();
                if ((countCube1 - 1) < 1)
                {
                    BotCountCubes4.color = Color.grey;
                    Info[1].text = " Игрок 4 вышел ";
                }
            }
        }
        else if (betResult >= currentCount && !_isTrueButton)
        {
            if (currentPlayerIndex == 0)
            {
                int countCube1 = GetCubeCount(0);
                PlayersCountCubes.text = (countCube1 - 1).ToString();
                if ((countCube1 - 1) < 1)
                {
                    PlayersCountCubes.color = Color.grey;
                    Info[1].text = " Вы Проиграли!";
                    GameFinishInfo[0].SetActive(true);
                    GameFinishInfo[1].SetActive(true);
                    currentPlayerIndex = -1;
                    Info[4].text = Info[0].text;
                    Info[5].text = Info[2].text;

                }

            }
            if (currentPlayerIndex == 1)
            {
                int countCube1 = GetCubeCount(1);
                BotCountCubes2.text = (countCube1 - 1).ToString();
                if ((countCube1 - 1) < 1)
                {
                    BotCountCubes2.color = Color.grey;
                    Info[1].text = " Игрок 2 вышел ";
                }
            }
            if (currentPlayerIndex == 2)
            {
                int countCube1 = GetCubeCount(2);
                BotCountCubes3.text = (countCube1 - 1).ToString();
                if ((countCube1 - 1) < 1)
                {
                    BotCountCubes3.color = Color.grey;
                    Info[1].text = " Игрок 3 вышел ";
                }
            }
            if (currentPlayerIndex == 3)
            {
                int countCube1 = GetCubeCount(3);
                BotCountCubes4.text = (countCube1 - 1).ToString();
                if ((countCube1 - 1) < 1)
                {
                    BotCountCubes4.color = Color.grey;
                    Info[1].text = " Игрок 4 вышел ";
                }
            }
        }
        else
        {
            if (currentPlayerIndex == 0)
            {
                int countCube1 = GetCubeCount(3);
                BotCountCubes4.text = (countCube1 - 1).ToString();
                if ((countCube1 - 1) < 1)
                {
                    BotCountCubes4.color = Color.grey;
                    Info[1].text = " Игрок 4 вышел ";
                }
            }
            if (currentPlayerIndex == 1)
            {
                int countCube1 = GetCubeCount(0);
                PlayersCountCubes.text = (countCube1 - 1).ToString();
                if ((countCube1 - 1) < 1)
                {
                    PlayersCountCubes.color = Color.grey;
                    Info[1].text = " Вы проиграли !  ";
                    GameFinishInfo[0].SetActive(true);
                    GameFinishInfo[1].SetActive(true);
                    currentPlayerIndex = -1;
                    Info[4].text = Info[0].text;
                    Info[5].text = Info[2].text;
                }
            }
            if (currentPlayerIndex == 2)
            {
                int countCube1 = GetCubeCount(1);
                BotCountCubes2.text = (countCube1 - 1).ToString();
                if ((countCube1 - 1) < 1)
                {
                    BotCountCubes2.color = Color.grey;
                    Info[1].text = " Игрок 2 вышел ";
                }
            }
            if (currentPlayerIndex == 3)
            {
                int countCube1 = GetCubeCount(2);
                BotCountCubes3.text = (countCube1 - 1).ToString();
                if ((countCube1 - 1) < 1)
                {
                    BotCountCubes3.color = Color.grey;
                    Info[1].text = " Игрок 3 вышел ";
                }
            }
        }
        UpdateMaxCubeCount();
    }
    private void UpdateMaxCubeCount()
    {
        maxDiceCount = GetCubeCount(0) + GetCubeCount(1) + GetCubeCount(2) + GetCubeCount(3);
    }
    private void TurnOnButtons(bool changeFlag)
    {
        CallTrueButton.gameObject.SetActive(changeFlag);
        CallWrongButton.gameObject.SetActive(changeFlag);
        CallWrong2Button.gameObject.SetActive(changeFlag);
        CallFaceIncreaseButton.gameObject.SetActive(changeFlag);
        CallFaceDeincreaseButton.gameObject.SetActive(changeFlag);
        CallCountIncreaseButton.gameObject.SetActive(changeFlag);
        CallCountDeincreaseButton.gameObject.SetActive(changeFlag);
    }
    private void CheckWin()
    {
        if(GetCubeCount(1) == 0 && GetCubeCount(2) == 0 && GetCubeCount(3) == 0)
        {
            GameFinishInfo[0].SetActive(true);
            GameFinishInfo[2].SetActive(true);
            currentPlayerIndex = -1;
            Info[4].text = Info[0].text;
            Info[5].text = Info[2].text;

        }   
    }
    public void ReloadGame()
    {
        SceneManager.LoadScene("Perudo");
    }
    private void ShowAnimForCube(TMP_Text player)
    {
        string[] nubmers = player.text.Split(" ");
        Animator[] animator = player.GetComponentsInChildren<Animator>();
        for (int i = 0; i < nubmers.Length; i++)
        {
            if (nubmers[i].Equals("1"))
            {
                animator[i].Play("Face1");
            }
            else if (nubmers[i].Equals("2"))
            {
                animator[i].Play("Face2");
            }
            else if (nubmers[i].Equals("3"))
            {
                animator[i].Play("Face3");
            }
            else if (nubmers[i].Equals("4"))
            {
                animator[i].Play("Face4");
            }
            else if (nubmers[i].Equals("5"))
            {
                animator[i].Play("Face5");
            }
            
        }
    }
}

