using System.Collections.Generic;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;


public class GameManager : StaticInstance<GameManager>
{
    // public static event Action<GameState> OnBeforeStateChanged;
    // public static event Action<GameState> OnAfterStateChanged;
    public float timeChangeForce = 1;
    public DialogueManager DialogueManager;
    public ChooseManager ChooseManager;
    public GameState State { get; private set; }
    private Camera camera;
    
    
    public GameObject npc1;
    public GameObject npc2;
    public GameObject npc3;
    
    private Vector3 npc1StartPos;
    private Vector3 npc2StartPos;
    private Vector3 npc3StartPos;
    
    private Quaternion npc1Startrot;
    private Quaternion npc2Startrot;
    private Quaternion npc3Startrot;
    
    public Vector3 npc1EndPos;
    public Vector3 npc2EndPos;
    public Vector3 npc3EndPos;
    
    public Quaternion npc1Endrot;
    public Quaternion npc2Endrot;
    public Quaternion npc3Endrot;

    public Vector3 playerEndPos;
    public Quaternion playerEndrot;
    void Start()
    {
        npc1StartPos = npc1.GetComponent<Transform>().position;
        npc2StartPos = npc2.GetComponent<Transform>().position;
        npc3StartPos = npc3.GetComponent<Transform>().position;
        
        npc1Startrot = npc1.GetComponent<Transform>().rotation;
        npc2Startrot = npc2.GetComponent<Transform>().rotation;
        npc3Startrot = npc3.GetComponent<Transform>().rotation;
        
        camera = Camera.main;
        ChangeState(GameState.OpenWorld);
    }

    public void ChangeState(GameState newState)
    {
        EventManager.Instance.ChangeGameState(newState);
        State = newState;
        switch (newState)
        {
            case GameState.firstDialogue:
            {
                HandleStarting();
                break;
            }
            case GameState.firstChoose:
            {
                ChooseManager.SetChoose(0, (b, i) => { ChangeState(GameState.OpenWorld); });
                break;
            }
            case GameState.OpenWorld:
            {
                DialogueManager.PlayDialogue(3, () => {
                    QuestManager.Instance.setQuest(Quests.getLampe);          
                    ThirdPersonController.Instance.ChangeSittingState(false);
                });
                break;
            }

            case GameState.truthOrDare:
            {
                truthorDareHandle();
                break;
            }
            
            case GameState.aboutToDie:
            {
                aboutToDieHandle();
                break;
            }
            case GameState.dieng:
            {
                dieng();
                break;
            }
        }
    }

    private void HandleStarting()
    {
        ThirdPersonController.Instance.ChangeSittingState(true); 
        DialogueManager.PlayDialogue(0, () =>
        {
            ChangeState(GameState.firstChoose);
        });
    }

    private void truthorDareHandle()
    {
        ChooseManager.SetChoose(1, (b, i) =>
        {
            if (b)
            {
                StartCoroutine(configureSneek());
            }
            else
            {
                StartCoroutine(configureSneekContinues());
            }
        });
    }

    private IEnumerator configureSneek()
    {
        BonFIreVibeAudio.Instance.changeSoundTO(soundType.creepy);
        yield return Transactor.Instance.FadeIn();
        camera.gameObject.SetActive(false);
        yield return Transactor.Instance.FadeOut();

        DialogueManager.PlayDialogue(4, () =>
        { 
            StartCoroutine(configureSneekContinues());
        });
    }

    private IEnumerator configureSneekContinues()
    {
        BonFIreVibeAudio.Instance.changeSoundTO(soundType.creepy);
        yield return Transactor.Instance.FadeIn();
        camera.gameObject.SetActive(true);
        ThirdPersonController.Instance.StartPos();
        yield return Transactor.Instance.FadeOut();
        DialogueManager.PlayDialogue(5, () =>
        {
            DialogueManager.PlayDialogue(6, () =>
            {
                ChooseManager.SetChoose(2, (b, i) =>
                {
                    DialogueManager.PlayDialogue(9, () =>
                    {
                        ChangeState(GameState.aboutToDie);
                    });
                });
            });
        });
    }

    private void aboutToDieHandle()
    {
        StartCoroutine(aboutToDieConfigure());
    }

    private IEnumerator aboutToDieConfigure()
    {
        yield return Transactor.Instance.FadeIn();
        npc1.transform.position = npc1EndPos;
        npc2.transform.position = npc2EndPos;
        npc3.transform.position = npc3EndPos;
        
        npc1.transform.rotation = npc1Endrot;
        npc2.transform.rotation = npc2Endrot;
        npc3.transform.rotation = npc3Endrot;
        npc1.GetComponentInChildren<Animator>().SetBool("isTalking", false);
        npc1.GetComponentInChildren<Animator>().SetBool("isCharing", true);
        npc2.GetComponentInChildren<Animator>().SetBool("isTalking", false);
        npc2.GetComponentInChildren<Animator>().SetBool("isCharing", true);
        npc3.GetComponentInChildren<Animator>().SetBool("isTalking", false);
        npc3.GetComponentInChildren<Animator>().SetBool("isCharing", true);
        
        ThirdPersonController.Instance.endPos(playerEndPos, playerEndrot);
        yield return Transactor.Instance.FadeOut();

        DialogueManager.PlayDialogue(10, () =>
            {
               ChangeState(GameState.dieng);

            }
        );

    }

    private void dieng()
    {
        ThirdPersonController.Instance.CanWalk = true;
        ThirdPersonController.Instance.JustCanMoveForverd = true;
    }
}

[Serializable]
public enum GameState {
    startMonologue = 0,
    firstDialogue = 1,
    firstChoose = 2,
    OpenWorld = 3,
    truthOrDare = 4,
    aboutToDie = 5,
    dieng = 6,
}