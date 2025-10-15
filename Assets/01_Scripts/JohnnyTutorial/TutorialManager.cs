using UnityEngine;
using TMPro; // <— IMPORTANTE

public class TutorialManager : MonoBehaviour
{
    public TMP_Text ui;                 // <- antes era UnityEngine.UI.Text
    public PlayerJohnny player;
    public PlayerCombatJohnny combat;
   
    bool moved, jumped, picked, switchedTo1, switchedTo2, imbued, attackedR, attackedM;

    void Start() { Show("Muévete con A/D. (o flechas)"); }

    void Update()
    {
        if (!moved && Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0)
        { moved = true; Show("Salta con SPACE."); return; }

        if (moved && !jumped && Input.GetKeyDown(KeyCode.Space))
        { jumped = true; Show("Acércate a la espada y presiona E para recogerla."); return; }

        if (picked && !switchedTo2 && Input.GetKeyDown(KeyCode.Alpha2))
        { switchedTo2 = true; Show("Clic izquierdo para atacar con espada. (Q = Imbuir energía)"); return; }

        if (switchedTo2 && !attackedM && Input.GetMouseButtonDown(0))
        { attackedM = true; Show("Pulsa Q para imbuir la espada con energía Elarion."); return; }

        if (attackedM && !imbued && Input.GetKeyDown(KeyCode.Q))
        { /* lo confirma NotifyImbue */ }

        if (imbued && !switchedTo1 && Input.GetKeyDown(KeyCode.Alpha1))
        { switchedTo1 = true; Show("Modo 1 activo: Clic izquierdo dispara energía condensada."); return; }

        if (switchedTo1 && !attackedR && Input.GetMouseButtonDown(0))
        { attackedR = true; Show("Avanza a la derecha; al acercarte, aparecerán enemigos cada 10s."); return; }

        if (moved && jumped && picked && switchedTo2 && attackedM && imbued && switchedTo1 && attackedR)
        { Show("¡Tutorial completo! Explora el Mundo 1."); enabled = false; }
    }

    public void NotifyPickedSword()
    {
        picked = true;
        Show("Pulsa 2 para equipar la espada.");
    }

    public void NotifyImbue()
    {
        imbued = true;
        if (!switchedTo1) Show("¡Espada imbuida! Pulsa 1 para cambiar a energía condensada.");
    }

    void Show(string msg)
    {
        if (ui != null) ui.text = msg;
    }
}
