using System.Collections.Generic;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;


public class GameManager : StaticInstance<GameManager>
{
    // public static event Action<GameState> OnBeforeStateChanged;
    // public static event Action<GameState> OnAfterStateChanged;
    public string rebornText = "Life is short... but not for us!";
    public GameObject gun; 
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

    public Vector3 playerStartPos;
    public Quaternion playerStartrot; 
    public Vector3 playerEndPos;
    public Quaternion playerEndrot; 
    public Vector3 playerReburnPos;
    public Quaternion playerReburnrot;
    void Start()
    {
        npc1StartPos = npc1.GetComponent<Transform>().position;
        npc2StartPos = npc2.GetComponent<Transform>().position;
        npc3StartPos = npc3.GetComponent<Transform>().position;
        
        npc1Startrot = npc1.GetComponent<Transform>().rotation;
        npc2Startrot = npc2.GetComponent<Transform>().rotation;
        npc3Startrot = npc3.GetComponent<Transform>().rotation;
        
        camera = Camera.main;
        ChangeState(GameState.reburnQuestions);
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
            case GameState.reburn:
            {
                reburnHandle();
                break;
            }
            case GameState.reburnQuestions:
            {
                rebornQuestions();
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
        ThirdPersonController.Instance.endPos(playerStartPos, playerStartrot);
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
        ThirdPersonController.Instance.CanWalk = true;
        ThirdPersonController.Instance.JustCanMoveForverd = true;
        
        yield return Transactor.Instance.FadeOut();

        DialogueManager.PlayDialogue(10, () => { });
        ChangeState(GameState.dieng);
    }

    private void dieng()
    {
        StartCoroutine(AudioSystem.Instance.StartAlmostToDie());
    }

    private void reburnHandle()
    {
        StartCoroutine(reBorn());
    }
    private IEnumerator reBorn()
    {
        yield return Transactor.Instance.FadeIn();
        npc1.SetActive(true);
        npc2.SetActive(true);
        npc3.SetActive(true);
        npc1.transform.position = npc1StartPos;
        npc2.transform.position = npc2StartPos;
        npc3.transform.position = npc3StartPos;
        
        npc1.transform.rotation = npc1Startrot;
        npc2.transform.rotation = npc2Startrot;
        npc3.transform.rotation = npc3Startrot;
        npc1.GetComponentInChildren<Animator>().SetBool("isTalking", true);
        npc1.GetComponentInChildren<Animator>().SetBool("isCharing", false);
        npc2.GetComponentInChildren<Animator>().SetBool("isTalking", true);
        npc2.GetComponentInChildren<Animator>().SetBool("isCharing", false);
        npc3.GetComponentInChildren<Animator>().SetBool("isTalking", true);
        npc3.GetComponentInChildren<Animator>().SetBool("isCharing", false);
        
        yield return new WaitForSeconds(0.4f);
        
         Transactor.Instance.ShowText(rebornText);
        
         yield return new WaitForSeconds(6f);


        ThirdPersonController.Instance.endPos(playerReburnPos, playerReburnrot, true );
        
        ThirdPersonController.Instance.JustCanMoveForverd = false;
        
        yield return Transactor.Instance.FadeOut();

        ChangeState(GameState.reburnQuestions);
    }
    
    private void rebornQuestions()
    {
        ChooseManager.SetChoose(3, (b, i) =>
        {
            ChooseManager.SetChoose(4, (b1, i1) =>
            {
                ThirdPersonController.Instance.sprintAdittion += 4;
                ChooseManager.SetChoose(5, (b2, i2) =>
                {
                    gun.SetActive(true);
                    gun.transform.position = ThirdPersonController.Instance.transform.position + Vector3.up;
                    AudioSystem.Instance.PlayFonk();
                });
            });
        });
    }

    /// <summary>True when all three NPCs have been killed (inactive).</summary>
    public bool AllNpcsDead()
    {
        return npc1 != null && !npc1.activeInHierarchy
            && npc2 != null && !npc2.activeInHierarchy
            && npc3 != null && !npc3.activeInHierarchy;
    }

    /// <summary>Called when player shoots self after killing all NPCs. Plays shot, fades, completes quest, then reborn.</summary>
    public void PlayerSuicide()
    {
        StartCoroutine(PlayerSuicideRoutine());
    }

    private IEnumerator PlayerSuicideRoutine()
    {
        ThirdPersonController.Instance.CanWalk = false;
        AudioSystem.Instance.PlayShotgunSound();
        if (gun != null)
        {
            var fh = gun.GetComponent<FireHelper>();
            if (fh != null && fh.shootParticles != null)
                fh.shootParticles.Play();
        }
        yield return new WaitForSeconds(0.5f);
        yield return Transactor.Instance.FadeIn();
        if (QuestManager.Instance.currentQuest != null && QuestManager.Instance.currentQuest.guest == Quests.getAllOfThem)
            QuestManager.Instance.ifQuestIsThisThenQuestDone(Quests.getAllOfThem);
        yield return new WaitForSeconds(0.3f);
        ChangeState(GameState.reburn);
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
    reburn = 7,
    reburnQuestions = 8
}