using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class BoutiqueManager : MonoBehaviour {

    // Bouton pour passer le tour
    public GameObject tourButton;

    // Panel de la boutique
    public GameObject shopPanel;

    // Panel du panier
    private GameObject cart;
    private GameObject totalText;
    private int total = 0;
    private Joueur joueur;

    // Boutons d'achat de chaque unité
    private Dictionary<int, GameObject> shopSelections;

    // Prefabs pour le panier
    public GameObject infanterieItem;
    public GameObject tankItem;
    public GameObject artillerieItem;
    public GameObject dcaItem;
    public GameObject croiseurItem;
    public GameObject submarinItem;
    public GameObject bombardierItem;

    // Prefabs des unités
    public GameObject infanterieModel;
    public GameObject tankModel;
    public GameObject artillerieModel;
    public GameObject dcaModel;
    public GameObject croiseurModel;
    public GameObject submarinModel;
    public GameObject bombardierModel;
    
    // La classe TurnManager gérant les tours de jeu
    public GameObject guiManager;


    ///<summary>
    /// Dictionnaire regroupant les GameObjects instanciés dans le panier et indexés par le numéro correspondant à l'unité :
    /// 0 -> infanterie  1 -> tank
    /// 2 -> artillerie  3 -> DCA
    /// 4 -> croiseur    5 -> sous-marin
    /// 6 -> bombardier
    /// </summary>
    private Dictionary<int, GameObject> cartItems = new Dictionary<int, GameObject>();

    /// <summary>
    /// Ouvre ou ferme le menu de la boutique.
    /// </summary>
	public void BoutiqueButton()
    {
        if (!guiManager.GetComponent<GUIController>().dialogOpened || shopPanel.activeSelf) // S'il n'y a pas d'autre fenêtre de dialogue d'ouverte
        {
            joueur = guiManager.GetComponent<TurnManager>().GetJoueurActif();
            OpenCloseShop();

            if (shopSelections == null && shopPanel.activeSelf)
            {
                shopSelections = new Dictionary<int, GameObject>();
                shopSelections.Add(0, GameObject.Find("Infanterie").transform.Find("Achat").gameObject as GameObject);
                shopSelections.Add(1, GameObject.Find("Tank").transform.Find("Achat").gameObject as GameObject);
                shopSelections.Add(2, GameObject.Find("Artillerie").transform.Find("Achat").gameObject as GameObject);
                shopSelections.Add(3, GameObject.Find("DCA").transform.Find("Achat").gameObject as GameObject);
                shopSelections.Add(4, GameObject.Find("Croiseur").transform.Find("Achat").gameObject as GameObject);
                shopSelections.Add(5, GameObject.Find("Sous-marin").transform.Find("Achat").gameObject as GameObject);
                shopSelections.Add(6, GameObject.Find("Bombardier").transform.Find("Achat").gameObject as GameObject);
            }

            if (shopPanel.activeSelf)
                darkenUnavailable();
        }
    }

    /// <summary>
    /// Grise les boutons des unités si le joueur n'a pas les fonds
    /// </summary>
    public void darkenUnavailable()
    {
        Color32 grey = new Color32(128, 128, 128, 128);
        Color32 transparent = new Color32(0, 0, 0, 0);
        int fonds = joueur.Credits;
        bool canBuyNavalUnits = joueur.CanBuyNavalUnits();

        // Le joueur peut-il acheter l'unité ? bouton grisé : bouton transparent
        shopSelections[0].GetComponent<Image>().color = fonds - total < Utils.GetUnitPrice((int) Utils.unitCode.Infanterie) ? grey : transparent;
        shopSelections[1].GetComponent<Image>().color = fonds - total < Utils.GetUnitPrice((int) Utils.unitCode.Tank) ? grey : transparent;
        shopSelections[2].GetComponent<Image>().color = fonds - total < Utils.GetUnitPrice((int) Utils.unitCode.Artillerie) ? grey : transparent;
        shopSelections[3].GetComponent<Image>().color = fonds - total < Utils.GetUnitPrice((int) Utils.unitCode.DCA) ? grey : transparent;
        shopSelections[6].GetComponent<Image>().color = fonds - total < Utils.GetUnitPrice((int) Utils.unitCode.Bombardier) ? grey : transparent;

        // Pour le croiseur, il faut vérifier si le joueur a les fonds et qu'un des territoires du joueur est connecté à une route libre ou occupée
        // par le joueur
        if (fonds - total > Utils.GetUnitPrice((int)Utils.unitCode.Croiseur) && canBuyNavalUnits)
                shopSelections[4].GetComponent<Image>().color = transparent;
        else
            shopSelections[4].GetComponent<Image>().color = grey;

        // De même pour le sous-marin
        if (fonds - total < Utils.GetUnitPrice((int)Utils.unitCode.Submarin) && canBuyNavalUnits)
            shopSelections[5].GetComponent<Image>().color = grey;
        else
            shopSelections[5].GetComponent<Image>().color = transparent;
    }

    /// <summary>
    /// Modifie le texte affichant le total du panier actuel du joueur
    /// </summary>
    /// <param name="total">int Le total de la transaction</param>
    private void SetTotalText(int total)
    {
        if(total > 0)
            totalText.GetComponent<Text>().text = "Total : " + total + "k Cr";
        else
            totalText.GetComponent<Text>().text = "Total : 0 Cr";
    }

    /// <summary>
    /// Ajoute une unite si le joueur actif a les fonds nécessaires.
    /// </summary>
    public void uniteButton(int code)
    {
        if (guiManager.GetComponent<TurnManager>().PhaseActive == TurnManager.phases.Deploiement)
        {
            if (joueur.Credits - total >= Utils.GetUnitPrice(code))
            {
                GameObject cartItem;
                if (cartItems.TryGetValue(code, out cartItem)) // = Si l'unité est déjà dans le panier
                {
                    // Récupération du compte de l'unité pour l'incrémenter
                    GameObject count = cartItem.transform.Find("Count").gameObject;
                    int newText = 1 + Int32.Parse(count.GetComponent<Text>().text);
                    count.GetComponent<Text>().text = "" + newText;
                }
                else
                {
                    // Y = -25 * le nombre d'unité déjà dans le panier
                    Vector3 position = new Vector3(0.0f, -25.0f * cartItems.Count, 0.0f);
                    Quaternion rotation = new Quaternion();
                    GameObject prefab = GetRightPrefab(code, false);

                    Instantiate(prefab, cart.transform.Find("Liste")); // Création d'une entrée dans le panier pour l'unité demandée

                    cartItems[code] = cart.transform.Find("Liste").FindChild(Utils.GetUnitsNameByCode(code) + "(Clone)").gameObject;
                    cartItems[code].transform.localPosition = position;
                    cartItems[code].transform.rotation = rotation;
                    cartItems[code].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }

                total += Utils.GetUnitPrice(code);
                SetTotalText(total);
                darkenUnavailable();
            }
        }
        else
            guiManager.GetComponent<GUIController>().ErrorMessage("Vous ne pouvez acheter d'unité en-dehors de votre phase de déploiement.");
    }

    public Transform CreateUnit(int unitCode, Transform unitList, Vector3 position, Joueur joueur)
    {
        GameObject prefab = GetRightPrefab(unitCode, true);
        Instantiate(prefab, unitList); // Création d'une unité

        Transform newUnit = unitList.GetChild(unitList.childCount - 1); // Récupération de l'unité nouvellement crée pour la modifier
        newUnit.localPosition = position;
        newUnit.rotation = new Quaternion();
        newUnit.GetComponent<Unite>().Joueur = joueur;

        joueur.Unites.Add(newUnit.GetComponent<Unite>());

        return newUnit;
    }

    void OpenCloseShop()
    {
        shopPanel.SetActive(!shopPanel.activeSelf);
        tourButton.SetActive(!tourButton.activeSelf);

        guiManager.GetComponent<GUIController>().dialogOpened = shopPanel.activeSelf;

        if (cart == null)
            cart = GameObject.Find("Cart Panel");
        if (totalText == null)
            totalText = GameObject.Find("Total");
    }

    /// <summary>
    /// Valide les achats de l'utilisateur :
    /// - Créé les nouvelles unités
    /// - Déduit les fonds correspondants
    /// - Vide la liste du panier
    /// - Ferme le panel ShopWrapper
    /// </summary>
    public void ValiderButton()
    {
        if (!guiManager.GetComponent<GUIController>().dialogOpened || shopPanel.activeSelf) // S'il n'y a pas d'autre fenêtre de dialogue d'ouverte
        {
            if (cartItems.Count > 0)
            {
                int spawned = 0;
                Transform unitList = GameObject.Find("Unites").transform;

                // Pour chaque unité
                for (int i = 0; i < 7; i++)
                {
                    GameObject item;

                    // S'il y a une entrée dans le panier
                    if (cartItems.TryGetValue(i, out item))
                    {
                        // Récupération du nombre demandé par le joueur
                        int count = Int32.Parse(item.transform.Find("Count").gameObject.GetComponent<Text>().text);

                        // Création de chaque instance demandée de cette unité
                        for (int j = 0; j < count; j++)
                        {
                            Vector3 position = new Vector3(41.0f, 1.2f, 19.0f - 0.75f * spawned);

                            CreateUnit(i, unitList, position, joueur);

                            spawned++;
                        }
                    }
                }

                joueur.AjouterCredits(-total);
                SetTotalText(0);
                AnnulerButton();
            }
        }
    }

    /// <summary>
    /// Annule les achats de l'utilisateur :
    /// - Vide le panier
    /// - Ferme le panel ShopWrapper
    /// </summary>
    public void AnnulerButton()
    {
        if (!guiManager.GetComponent<GUIController>().dialogOpened || shopPanel.activeSelf) // S'il n'y a pas d'autre fenêtre de dialogue d'ouverte
        {
            VidePanier();
            OpenCloseShop();
        }
    }

    /// <summary>
    /// Vide le panier
    /// </summary>
    public void VidePanier()
    {

        if (!guiManager.GetComponent<GUIController>().dialogOpened || shopPanel.activeSelf) // S'il n'y a pas d'autre fenêtre de dialogue d'ouverte
        {
            total = 0;

            foreach (KeyValuePair<int, GameObject> item in cartItems)
            {
                Destroy(item.Value);
            }

            cartItems.Clear();
            SetTotalText(0);
            darkenUnavailable();
        }
    }

    /// <summary>
    /// Renvoie le prefab de l'unité dont le numéro est code
    /// </summary>
    /// <param name="code">int Le code de l'unité désirée</param>
    /// <param name="model">bool Indique s'il s'agit du model ou de l'entrée dans le panier</param>
    /// <returns>GameObject Le prefab de l'unité associée à code</returns>
    public GameObject GetRightPrefab(int code, bool model)
    {
        switch (code)
        {
            case ((int)Utils.unitCode.Infanterie):
                if (model)
                    return infanterieModel;
                else
                    return infanterieItem;
            case ((int)Utils.unitCode.Tank):
                if (model)
                    return tankModel;
                else
                    return tankItem;
            case ((int)Utils.unitCode.Artillerie):
                if (model)
                    return artillerieModel;
                else
                    return artillerieItem;
            case ((int)Utils.unitCode.DCA):
                if (model)
                    return dcaModel;
                else
                    return dcaItem;
            case ((int)Utils.unitCode.Croiseur):
                if (model)
                    return croiseurModel;
                else
                    return croiseurItem;
            case ((int)Utils.unitCode.Submarin):
                if (model)
                    return submarinModel;
                else
                    return submarinItem;
            case ((int)Utils.unitCode.Bombardier):
                if (model)
                    return bombardierModel;
                else
                    return bombardierItem;
            default:
                throw new InvalidUnitCodeException(code);
        }
    }
}
