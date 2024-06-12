using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallDialogue : MonoBehaviour
{
    [SerializeField] private GameObject player;

    [SerializeField] private Dialog dialog;
    [SerializeField] private int id;
    [SerializeField] private GameObject dialogManager;

    [SerializeField] private State call;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.CompareTag("Player")){ // Verifica se diálogo pode ser exibido
            if (id == 0){
                if (player.GetComponent<Items>().LoadDialogue(id) == Dialogs.DialogType.Exibir){
                    Activate();        
                }
            }else if(id > 0){
                if(player.GetComponent<Items>().LoadDialogue(id-1) == Dialogs.DialogType.Exibir && player.GetComponent<Items>().LoadDialogue(id) != Dialogs.DialogType.Exibir){
                    Activate();        
                }
            }else{
                Debug.Log("Ops! Houve um erro.");
            }
        }
    }

    private void SendDialogue(bool act){
        if(act){
            dialogManager.GetComponent<DialogSystem>().act = true;
            dialogManager.GetComponent<DialogSystem>().tipo = call;
        }
        dialogManager.GetComponent<DialogSystem>().id = id;
        dialogManager.GetComponent<DialogSystem>().StartDialog(dialog);
    }
    public void Activate(){
        switch (call)// Verifica se após o diálogo irá acontecer algo
        {
            case State.Quest:// Ativar  desafio
                SendDialogue(true);
            break;
            case State.Dialog:// Chamar outro diálogo após exibir esse
                SendDialogue(true);
            break;
            case State.Market:// Ativa o mercado
                SendDialogue(true);
            break;
            case State.Partner:// Chamar ativar/desativar companheiro
                if(!player.GetComponent<Items>().LoadPartner()){
                    SendDialogue(true);
                }
            break;
            case State.Island: // Ativar segunda ilha
                SendDialogue(true);
            break;
            default: // Apenas chama o diálogo;
                SendDialogue(false);
            break;
        }
    }
}
