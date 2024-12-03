using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class QuizUI : MonoBehaviour
{
    [SerializeField] private QuizManager quizManager;
    [SerializeField] private GameObject gameOverPanel, mainMenuPanel, gameMenuPanel;
    [SerializeField] private Color correctCol, wrongCol, normalCol;
    [SerializeField] private Image questionImage;
    [SerializeField] private List<Image> lifeImageList;
    [SerializeField] private UnityEngine.Video.VideoPlayer questionVideo;
    [SerializeField] private AudioSource questionAudio;
    [SerializeField] private TMPro.TextMeshProUGUI questionText, scoreText, timerText;
    [SerializeField] private List<Button> options, uiButtons;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    private Question question;
    private bool answered;
    private float audioLength;

    public TextMeshProUGUI ScoreText { get { return scoreText; } }
    public TextMeshProUGUI TimerText { get { return timerText; } }
    public GameObject GameOverPanel { get { return gameOverPanel; } }


    void Awake()
    {

        for (int i = 0; i < options.Count; i++)
        {
            Button localBtn = options[i];
            localBtn.onClick.AddListener(() => OnClick(localBtn));
        }

        for (int i = 0; i < uiButtons.Count; i++)
        {
            Button localBtn = uiButtons[i];
            localBtn.onClick.AddListener(() => OnClick(localBtn));
        }

    }

    public void SetQuestion(Question question)
    {
        //set the question
        this.question = question;
        //check for questionType
        switch (question.questionType)
        {
            case QuestionType.TEXT:
                questionImage.transform.parent.gameObject.SetActive(false);
                break;
            case QuestionType.IMAGE:
                ImageHolder();
                questionImage.transform.gameObject.SetActive(true);
                questionImage.sprite = question.qustionImage;
                break;
            case QuestionType.VIDEO:
                ImageHolder();
                questionVideo.transform.gameObject.SetActive(true);
                questionVideo.clip = question.qustionVideo;
                questionVideo.Play();
                break;
            case QuestionType.AUDIO:
                ImageHolder();
                questionAudio.transform.gameObject.SetActive(true);
                audioLength = question.qustionClip.length;
                StartCoroutine(PlayAudio());
                break;

        }


        questionText.text = question.questionInfor;
        List<string> answerList = ShuffleList.ShuffleListItems<string>(question.options);

        for (int i = 0; i < options.Count; i++)
        {
            options[i].GetComponentInChildren<TextMeshProUGUI>().text = answerList[i];
            options[i].name = answerList[i];
            options[i].image.color = normalCol;
        }

        answered = false;
    }

    IEnumerator PlayAudio()
    {
        if (question.questionType == QuestionType.AUDIO)
        {
            questionAudio.PlayOneShot(question.qustionClip);
            yield return new WaitForSeconds(audioLength + 0.5f);
            StartCoroutine(PlayAudio());
        }
        else
        {
            StopCoroutine(PlayAudio());
            yield return null;
        }
    }

    void ImageHolder()
    {
        questionImage.transform.parent.gameObject.SetActive(true);
        questionImage.transform.gameObject.SetActive(false);
        questionAudio.transform.gameObject.SetActive(false);
        questionVideo.transform.gameObject.SetActive(false);
    }

    private void OnClick(Button btn)
    {
        if (quizManager.GameStatus == QuizManager.QuizGameStatus.Playing)
        {
            if (!answered)
            {
                answered = true;

                bool val = quizManager.Answer(btn.name);

                if (val)
                {
                    btn.image.color = Color.green;
                    Debug.Log("xanh");
                }
                else
                {
                    btn.image.color = Color.red;
                    Debug.Log("do");
                }
            }
        }

        switch (btn.name)
        {
            case "Animal":
                quizManager.StartGame(0);
                mainMenuPanel.SetActive(false);
                gameMenuPanel.SetActive(true);
                break;
            case "Bird":
                quizManager.StartGame(1);
                mainMenuPanel.SetActive(false);
                gameMenuPanel.SetActive(true);
                break;
            case "Mix":
                quizManager.StartGame(2);
                mainMenuPanel.SetActive(false);
                gameMenuPanel.SetActive(true);
                break;

        }
    }
    public void ShowGameOverPanel(int score)
    {
        finalScoreText.text = "Score: " + score.ToString();  // Cập nhật điểm số
        gameOverPanel.SetActive(true);  // Hiển thị panel Game Over
    }

    public void RetryButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReduceLife(int index)
    {
        lifeImageList[index].color = Color.black;
    }


}