using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CallDialogue : MonoBehaviour
{
    [SerializeField] private Items items;//Encurta chamada do GameObjeto: player.GetComponent<Items>() apenas por: Items

    [SerializeField] private Dialog dialog;
    [SerializeField] private int id;
    [SerializeField] private DialogSystem dialogManager; //Encurtado GetComponent<DialogSystem>(). -> dialogManager

    [SerializeField] private State call;
    
    [HideInInspector] 
    public bool isResponse = false; // Ligado é resposta a um desafio, desligado apenas um diálogo normal. Será exibido se o desafio anterior estiver concluido.
    [Header("Hidde fields")]
    [HideInInspector] public int missionId; // ID do desafio anterior
    private bool stopVerify = false; // Fazer o Update() parar de chamar o Verify()

    /*/ Start is called before the first frame update
    void Start()
    {
        
    }*/

    // Update is called once per frame
    void Update()
    {
        if(isResponse){
            if(!stopVerify && items.LoadMission(missionId).missionActive){
                Verify();
                stopVerify = true;
            }
        }
    }
    private void Verify(){
        if (id == 0){
            if (items.LoadDialogue(id) != DialogState.Exibido){
                Activate();        
            }
        }else if(id > 0){
            if(items.LoadDialogue(id-1) == DialogState.Exibido && items.LoadDialogue(id) != DialogState.Exibido){
                Activate();        
            }
        }else{
            Debug.Log("Ops! Provávelmente o diálogo já foi exibido ou houve algum erro.");
        }
    }
    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.CompareTag("Player")){ // Verifica se diálogo pode ser exibido
            Verify();
        }
    }

    private void SendDialogue(bool act){
        if(act){
            dialogManager.act = true;
            dialogManager.tipo = call;
        }
        dialogManager.id = id;
        dialogManager.StartDialog(dialog);
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
                if(!items.LoadPartner()){
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

#if UNITY_EDITOR
[CustomEditor(typeof(CallDialogue))]
class CallDialogueEditor : Editor {
    public override void OnInspectorGUI() {
        var script = (CallDialogue)target;
        script.isResponse = EditorGUILayout.Toggle(label: "Resposta", script.isResponse);
        if(script.isResponse == false)
            return;
        script.missionId = EditorGUILayout.IntField(label:"ID da missão", script.missionId);
    }
}
#endif