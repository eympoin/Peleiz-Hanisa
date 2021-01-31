﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CinematicManager : MonoBehaviour
{

    public GameObject character;
    public List<GameObject> charaPoints = new List<GameObject>();
    public GameObject monster;
    public List<GameObject> monsterPoints = new List<GameObject>();
    public Image image;
    public bool isDialogDone;
    public bool isDialog2Done;
    private bool isTransitionDone;
    private bool isTransitionOpenDone;
    private CharacterAnimation charaAnimation;
    private enum characterPosStates { START, POS1, POS2, BED };
    private enum monsterPosStates { START, POS1, POS2};
    private characterPosStates characterPosState;
    private monsterPosStates monsterPosState;
    private float minPointDistance = 0.1f;

    public List<DialogPage> m_dialogPlayer;
    public List<DialogPage> m_dialogMonster;
    public DialogManager m_dialogDisplayer;
    private bool go_monster = false;

    private void Start()
    {
        charaAnimation = character.GetComponent<CharacterAnimation>();
        charaAnimation.state = CharacterAnimation.states.WALKING;
        characterPosState = characterPosStates.START;
        monsterPosState = monsterPosStates.START;
        monster.SetActive(false);
        isDialogDone = false;
        isDialog2Done = false;
        image.color = new Color(0, 0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!character.GetComponent<OpeningDialog>().alreadyTalk || m_dialogDisplayer.isActiveAndEnabled) {
            return; 
        }

        switch (characterPosState)
        {
            case (characterPosStates.START):
                character.transform.position = 
                    Vector3.MoveTowards(character.transform.position, charaPoints[0].transform.position, 0.1f);
                character.transform.LookAt(charaPoints[0].transform.position);
                if (Vector3.Distance(character.transform.position, charaPoints[0].transform.position) <= minPointDistance)
                    characterPosState = characterPosStates.POS1;
                break;
            case (characterPosStates.POS1):
                character.transform.position =
                    Vector3.MoveTowards(character.transform.position, charaPoints[1].transform.position, 0.1f);
                character.transform.LookAt(charaPoints[1].transform.position);
                if (Vector3.Distance(character.transform.position, charaPoints[1].transform.position) <= minPointDistance)
                    characterPosState = characterPosStates.POS2;
                break;
            case (characterPosStates.POS2):
                StartCoroutine(FadeImage(false, true));
                charaAnimation.state = CharacterAnimation.states.IDLE;
                character.transform.LookAt(new Vector3(charaPoints[2].transform.position.x,1, charaPoints[2].transform.position.z));
                //playCharaDialog();
                break;
            case (characterPosStates.BED):
                StartCoroutine(FadeImage2(true));
                charaAnimation.state = CharacterAnimation.states.HIDING;
                character.transform.position = charaPoints[2].transform.position;
                character.transform.LookAt(charaPoints[3].transform.position);
                break;
            default:
                break;

        }

        if (characterPosState == characterPosStates.BED && go_monster)
        {
            monster.SetActive(true);
            switch (monsterPosState)
            {
                case (monsterPosStates.START):
                    monster.transform.position =
                        Vector3.MoveTowards(monster.transform.position, monsterPoints[0].transform.position, 0.1f);
                    monster.transform.LookAt(monsterPoints[0].transform.position);
                    if (Vector3.Distance(monster.transform.position, monsterPoints[0].transform.position) <= minPointDistance)
                        monsterPosState = monsterPosStates.POS1;
                    break;
                case (monsterPosStates.POS1):
                    monster.transform.position =
                        Vector3.MoveTowards(monster.transform.position, monsterPoints[1].transform.position, 0.1f);
                    monster.transform.LookAt(monsterPoints[1].transform.position);
                    if (Vector3.Distance(monster.transform.position, monsterPoints[1].transform.position) <= minPointDistance)
                        monsterPosState = monsterPosStates.POS2;
                    break;
                case (monsterPosStates.POS2):
                    monster.transform.LookAt(new Vector3(monsterPoints[2].transform.position.x, 1, monsterPoints[2].transform.position.z));
                    playMonsterDialog();
                    break;
                default:
                    break;
            }
        }

    }

    private void playCharaDialog() 
    {
        characterPosState = characterPosStates.BED;
        m_dialogDisplayer.SetDialog(m_dialogPlayer);
    }

    private void playMonsterDialog()
    {
            StartCoroutine(FadeImage(false, false));
            m_dialogDisplayer.SetDialog(m_dialogMonster);
            goToScene nextScene = new goToScene();
            nextScene.SceneName = "Menu";
            nextScene.goToMyScene();
    }

    IEnumerator FadeImage(bool fadeAway, bool chara)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 2 second backwards
            for (float i = 1; i >= 0; i -= Time.deltaTime/2)
            {
                // set color with i as alpha
                image.color = new Color(0, 0, 0, i);
                yield return null;
            }
        }
        // fade from transparent to opaque
        else
        {
            // loop over 2 second
            for (float i = 0; i <= 1; i += Time.deltaTime/2)
            {
                // set color with i as alpha
                image.color = new Color(0, 0, 0, i);
                yield return null;
            }
            choose_dialog(chara);
        }
    }

    IEnumerator FadeImage2(bool fadeAway)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 2 second backwards
            for (float i = 1; i >= 0; i -= Time.deltaTime / 2)
            {
                // set color with i as alpha
                image.color = new Color(0, 0, 0, i);
                yield return null;
            }
            go_monster = true;
        }
        // fade from transparent to opaque
        else
        {
            // loop over 2 second
            for (float i = 0; i <= 1; i += Time.deltaTime / 2)
            {
                // set color with i as alpha
                image.color = new Color(0, 0, 0, i);
                yield return null;
            }
        }
    }

    void choose_dialog(bool chara) {
        if (chara)
        {
            playCharaDialog();
        }
        else {
            playMonsterDialog();
        }
    }
}
