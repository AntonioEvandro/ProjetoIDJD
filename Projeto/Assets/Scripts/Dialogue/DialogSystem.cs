using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public enum State{
    Quest,Dialog,Market,Partner, Island, Default
}

public class DialogSystem : MonoBehaviour
{
    [Space(20)]
    [Tooltip(
        "Objeto para obter a função de salvar diálogo e bloquer o movimento do player"
    )]
    [SerializeField]
    private GameObject player;

    [Space(15)]

    [Tooltip("Id do diálogo, recebido pelo botão")]
    public int id;

    [Space(15)]

    [SerializeField]
    [Tooltip("Caixa de diálogo para aparecer os elementos da conversa")]
    private GameObject dialogueBox;
    
    [Space(15)]

    /*[Tooltip("Script para exibir texto linha por linha")]
    [SerializeField]*/
    private Writing writing;
    
    [Space(15)]

    [Tooltip("Animações de entrada e saida da caixa de diálogo")]
    [SerializeField]
    private Animator animator;

    [Space(15)]
    
    [Tooltip("Texto para por o nome do personagem que está falando")]
    [SerializeField]
    private TextMeshProUGUI nameCharacter;

    private Dialog currentDialog;
    private int lineIndex;
    private Queue<string> Lines;
    private bool btnNext=true;
    public bool act = false;
    public State tipo;

    // Start is called before the first frame update
    void Start()
    {
        writing = GetComponent<Writing>();
    }
    // Update is called once per frame
    void Update()
    {
        KeyNext();
    }

    public void StartDialog(Dialog dialog){
        player.GetComponent<ControlePersonagem>().BlockMovent();

        // Dialogo fica com estado Exibindo, até ser salvo como exibido.
        if(!(player.GetComponent<Items>().LoadMercado() && id == 0) && player.GetComponent<Items>().LoadDialogue(id) != DialogState.Exibindo){
            player.GetComponent<Items>().SaveDialogue(id, DialogState.Exibindo);
        }
        //Deixa a caixa de diálogo visível
        dialogueBox.SetActive(true);
        // Inicializa a fila de falas
        Lines = new Queue<string>();
        currentDialog = dialog;
        lineIndex = 0;

        NextLine();
    }

    public void NextLine(){
        if(writing.ItsShow){
            writing.ShowFullText();
            return;
        }
        if (Lines.Count == 0){
            if(lineIndex < currentDialog.dialogues.Length){
                //Coloca o nome do personagem na caixa de diálogo
                nameCharacter.text = currentDialog.dialogues[lineIndex].name;

                //Coloca todas as falas numa fila
                foreach (string line in currentDialog.dialogues[lineIndex].lines){
                    Lines.Enqueue(line);
                }
                lineIndex++;
            }else{
                //bloqueia o botão de pular diálogo
                btnNext = false;
                // Torna a o diálogo inacessível
                if(!(player.GetComponent<Items>().LoadMercado() && id == 0)){
                    player.GetComponent<Items>().SaveDialogue(id, DialogState.Exibido);
                }
                // Chama a função de fechar a caixa de diálogo
                StartCoroutine(CloseBox());
                return;
            }
        }
        writing.ShowTextWriting(Lines.Dequeue());
    }
    // Fechar caixa de diálogo
    private IEnumerator CloseBox(){
        animator.Play("DialogBoxExit");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        dialogueBox.SetActive(false);
        btnNext = true;
        Action();
        player.GetComponent<ControlePersonagem>().UnBlockMovent();
    }

    // Botão de pular linhas de diálogos
    public void BtnNext(){
        if(btnNext){
            NextLine();
        }
    }
    // Tecla de pular linhas de diálogos
    public void KeyNext(){
        if(Input.GetKeyDown(KeyCode.F) && btnNext && dialogueBox.activeSelf){
            NextLine();
        }
    }

    // Faz algo após terminar o diálogo
    private void Action(){
        if(act){
            switch (tipo)
            {
                case State.Quest:
                    //
                break;
                case State.Dialog:
                break;
                case State.Market:
                    if(!player.GetComponent<Items>().LoadMercado()){
                        player.GetComponent<Items>().SaveMarket();
                        Debug.Log("Mercado desbloqueado!");
                    }
                    GetComponent<SwitchPanels>().MarketOn();
                break;
                case State.Partner:
                    if(!player.GetComponent<Items>().LoadPartner()){
                        player.GetComponent<Items>().SavePartner(true);
                        Debug.Log("Companheiro ativado!");
                    }else{
                        player.GetComponent<Items>().SavePartner(false);
                        Debug.Log("Companheiro descansando!");
                    }
                break;
                case State.Island:
                    if(!player.GetComponent<Items>().LoadIsland()){
                        player.GetComponent<Items>().SaveIsland(true);
                        Debug.Log("TP p/ 2ª ilha ativado" + player.GetComponent<Items>().LoadIsland());
                    }else{
                        player.GetComponent<Items>().SaveIsland(false);
                        Debug.Log("TP desativado" + player.GetComponent<Items>().LoadIsland());
                    }
                    GetComponent<Island>().Quest();
                break;
                default:
                    Debug.Log("Nada a se fazer");
                break;
            }
            act = false;
            tipo = State.Default;
        }
    }
}
