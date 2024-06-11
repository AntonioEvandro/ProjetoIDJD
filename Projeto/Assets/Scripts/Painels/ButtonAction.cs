using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAction : MonoBehaviour
{
    public GameObject player;
    public GameObject botaoAcao;
    public GameObject painelUI;

    [Header("Mission")]
    [Tooltip("ID deste desafio.")]
    [SerializeField] private int idMission;

    [Tooltip("ID do diálogo anterior.\nLibera o desafio atual se o diálogo anterior já foi exibido.")]
    [SerializeField] private int idDialog;

    [Header("Diálogo")]
    [Tooltip("Diálogo a ser enviado para exibição.\nCaso o botão seja para chamar o diálogo.")]
    [SerializeField]
    private Dialog dialog;
    [SerializeField]
    [Tooltip(
        "Pega o script DialogManager dentro do objeto GameManager\npara usar a função de mostrar o diálogo"
    )]
    private DialogSystem dialogManager;
/*
    [Header("Mercado")]
    [Tooltip("Adicionar a tela de mercado para ser exibida.")]
    [SerializeField]
    private SwitchPanels market;*/


    [Header("Chamada")]
    [Tooltip(
        "Variável usada para chamar uma função de acordo com o estado.\nSe for Quest chamará o desafio,\n caso for Dialog será chamado o diálogo\n ou se for Market mostrará o mercado."
    )]
    [SerializeField]
    private State call;//chama determinada função
    private GameObject bt;
    private bool playerInRange = false; // Adicionado para rastrear se o jogador está no range
    private bool SpaceKeyPressed = false; // Adicionado para rastrear se a tecla F foi pressionada
    
    // Função para mostrar o botão de ação enquanto estiver perto da quest
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
            switch (call){
                case State.Quest://chama o desafio
                    if(player.GetComponent<Items>().LoadDialogue(idDialog)){//verifica se exibiu o diálogo anterior
                        if (idMission == 0){
                            if (!player.GetComponent<Items>().LoadMission(idMission).missionActive){
                                Send4Button();
                            }
                        }else if(idMission > 0){
                            if(player.GetComponent<Items>().LoadMission(idMission-1).missionActive && !player.GetComponent<Items>().LoadMission(idMission).missionActive){
                                Send4Button();
                            }
                        }else{
                            Debug.Log("Ops! Houve um erro.");
                        }
                    }
                    break;
                case State.Dialog://chama o diálogo
                    if (idDialog == 0){
                        if (!player.GetComponent<Items>().LoadDialogue(idDialog)){
                            Send4Button();
                        }
                    }else if(idDialog > 0){
                        if(player.GetComponent<Items>().LoadDialogue(idDialog-1) && !player.GetComponent<Items>().LoadDialogue(idDialog)){
                            Send4Button();
                        }
                    }else{
                        Debug.Log("Ops! Houve um erro.");
                    }
                    break;
                case State.Market://Chama o mercado
                    if (player.GetComponent<Items>().LoadDialogue(idDialog)){
                        Send4Button();
                    }
                    break;
            }
        }
    }
    // Função para ocultar o botão de ação enquanto estiver perto da quest
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player")){
            playerInRange = false;
            Destroy(bt);
        }
    }
    // Função para ocultar o botão de ação depois que for clicado
    public void BtnClick(){
        bt.SetActive(false);
        switch (call){
            case State.Quest:
                GetComponent<ActivateChallenge>().ActiveQuest();
                break;
            case State.Dialog:
                dialogManager.GetComponent<DialogSystem>().id = idDialog;
                dialogManager.GetComponent<DialogSystem>().StartDialog(dialog);
                break;
            case State.Market:
                dialogManager.GetComponent<DialogSystem>().act = true;
                dialogManager.GetComponent<DialogSystem>().tipo = call;
                dialogManager.GetComponent<DialogSystem>().StartDialog(dialog);
                break;
        }
        SpaceKeyPressed = false; // Define a tecla F como não pressionada após chamar BtnClick()
    }
    // Ativa o botão de ação e envia a função BtnClick para ele
    public void Send4Button(){
        bt = Instantiate(botaoAcao);
        bt.transform.SetParent(painelUI.transform,false);
        bt.SetActive(true);
        bt.GetComponent<Button>().onClick.RemoveAllListeners();
        bt.GetComponent<Button>().onClick.AddListener(BtnClick);
        /*if(Input.GetKey(KeyCode.F)){
            BtnClick();
        }*/
    }

    /*private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && bt != null && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Clicado!");
            BtnClick();
        }
    }*/
    void Update(){
        if (playerInRange && bt != null && bt.activeSelf && Input.GetKeyDown(KeyCode.Space) && !SpaceKeyPressed)
        {
            BtnClick();
            SpaceKeyPressed = true; // Define a tecla Espaço como pressionada após chamar BtnClick()
        }
        
    }
}
