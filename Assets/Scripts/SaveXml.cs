using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections.Generic;

public class SaveXml : MonoBehaviour {

	SaveConfig sc;
	Joueur info;
	public int _credits;
	public string _date;/*.ToLongDateString () + System.DateTime.Now.Hour.ToString () + System.DateTime.Now.Minute.ToString () + System.DateTime.Now.Second.ToString ();
	*/
	public string _name;
	public Utils.couleurs _couleur;
	public bool _humain;
	public Dictionary<int, Territoire> _territoires;
	public Dictionary<int, Route> _routes;
	public List<Continent> _continents;
	/*
	public string _name;
	public int _age;
	public Color _bestColor;
*/
	public bool save = false;

	// Use this for initialization
	void Start () {
		sc = GetComponent<SaveConfig> ();

		if (save)
			Save ();
		else {
		} 
	}
	


	public void Save(){
		string filePath = sc.dataPath + "YoutubeGame.xml";

		XmlDocument xmlDoc = new XmlDocument ();

		if (File.Exists (filePath)) 
		{
			xmlDoc.Load (filePath);

			_date = System.DateTime.Now.ToString ();

			XmlElement elmRoot = xmlDoc.DocumentElement;

			XmlElement elmNew = xmlDoc.CreateElement ("player");

			XmlElement name = xmlDoc.CreateElement ("nom");
			name.InnerText = _name;
			/*XmlElement age = xmlDoc.CreateElement ("age");
			age.InnerText = _age.ToString();
			XmlElement bestColor = xmlDoc.CreateElement ("bestColor");
			bestColor.InnerText = _bestColor.ToString();
*/
			XmlElement credits = xmlDoc.CreateElement ("credits");
			credits.InnerText = _credits.ToString ();
			XmlElement couleur = xmlDoc.CreateElement ("couleur");
			couleur.InnerText = _couleur.ToString ();
			XmlElement humain = xmlDoc.CreateElement ("humain");
			humain.InnerText = _humain.ToString ();
			/*XmlElement territoires = xmlDoc.CreateElement ("territoires");
			territoires.InnerText = _territoires.ToString ();
			XmlElement routes = xmlDoc.CreateElement ("routes");
			routes.InnerText = _routes.ToString ();*/
			XmlElement continents = xmlDoc.CreateElement ("continents");
			continents.InnerText = _continents.ToString ();

			XmlElement date = xmlDoc.CreateElement("date");
			date.InnerText = _date.ToString();

			elmNew.AppendChild (name);
			elmNew.AppendChild (credits);
			elmNew.AppendChild (couleur);
			elmNew.AppendChild (humain);
			elmNew.AppendChild (date);
			//elmNew.AppendChild (territoires);
			//elmNew.AppendChild (routes);
			elmNew.AppendChild (continents) ;
			/*
			elmNew.AppendChild (name);
			elmNew.AppendChild (age);
			elmNew.AppendChild (bestColor);
*/
			elmRoot.AppendChild (elmNew);

			xmlDoc.Save (filePath);

		}
	
	}

	public void LoadFromXml(){
		string filepath = Application.dataPath + @"/MenuSauvegarde/YoutubeGame.xml";
		XmlDocument xmlDoc = new XmlDocument ();

		if (File.Exists (filepath)) {
			xmlDoc.Load (filepath);
			XmlNodeList transformList = xmlDoc.GetElementsByTagName ("Joueur");
			foreach (XmlNode transformInfo in transformList) {
				XmlNodeList transformcontent = transformInfo.ChildNodes;
				foreach (XmlNode transformItens in transformcontent) {
						CheckValueAndLoad (transformItens);
				}
			}
		}
	}

	/// <summary>
	/// Recupère l'ensemble des sections et recupère chaque valeur.
	/// </summary>
	/// <param name="transformItens">Transform itens.</param>
	private void CheckValueAndLoad (XmlNode transformItens)
	{
		if (transformItens.Name == "Credits") {
			_credits = int.Parse (transformItens.InnerText);
		}
		if (transformItens.Name == "Nom") {
			_name = (transformItens.InnerText).ToString();
		}
		/*if (transformItens.Name == "Couleur") {
			_couleur = transformItens.ToString ();
		}
		if (transformItens.Name == "Humain") {
			_humain = (transformItens.InnerText).ToString();
		}
		if (transformItens.Name == "Territoires") {
			_territoires = (transformItens.InnerText).ToString();
		}
		if (transformItens.Name == "Routes") {
			_routes = (transformItens.InnerText).ToString();
		}
		if (transformItens.Name == "Continents") {
			_continents = (transformItens.InnerText).ToString();
		}*/
	}

	public void RemoveSave(XmlElement elmRoot){
		/*Supprime toute les sauvegardes présentent dans le fichier xml.*/
		//elmRoot.RemoveAll ();
		elmRoot.RemoveChild(elmRoot);
	}
}
