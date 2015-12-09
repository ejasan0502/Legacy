using UnityEngine;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

// ONLY USE IN UNITY EDITOR
// Used to create game scenes, NOT USED FOR UNITY SCENES
// *Requires tag, "SceneObject"
public class SaveSceneToXml : MonoBehaviour {

    void Start(){
        // Find/Create xml file
        string path = GlobalVariables.PATH_XMLDATA_SCENES_FULL;
        XmlDocument xmlDoc = new XmlDocument();
        XmlElement root;
        if ( !File.Exists(path) ){
            FileStream fs = File.Create(path);
            fs.Close();

            root = xmlDoc.CreateElement("Scenes");
        } else {
            xmlDoc.LoadXml(File.ReadAllText(path));

            root = xmlDoc.FirstChild as XmlElement;
        }

        // Look for and remove any duplicates
        foreach (XmlNode xmlNode in root.ChildNodes){
            if ( xmlNode.Attributes["name"].Value == Application.loadedLevelName ){
                root.RemoveChild(xmlNode);
            }
        }

        // Add scene to xml file
        XmlElement scene = xmlDoc.CreateElement("Scene");
        scene.SetAttribute("name",Application.loadedLevelName);
        foreach (GameObject o in GameObject.FindGameObjectsWithTag("SceneObject")){
            XmlElement obj = xmlDoc.CreateElement("SceneObject");
            XmlElement n = xmlDoc.CreateElement("Name");
            XmlElement pos = xmlDoc.CreateElement("Position");
            XmlElement rot = xmlDoc.CreateElement("Rotation");

            string desiredName = o.name;
            if ( desiredName.Contains("(Clone)") ){
                desiredName = desiredName.Split('(')[0];
            }
            n.InnerText = desiredName;
            pos.InnerText = o.transform.position+"";
            rot.InnerText = o.transform.rotation+"";

            obj.AppendChild(n);
            obj.AppendChild(pos);
            obj.AppendChild(rot);

            if ( o.name.ToLower().Contains("portal") ){
                XmlElement p = xmlDoc.CreateElement("Portal");
                p.InnerText = o.GetComponent<Portal>().scene;
                obj.AppendChild(p);
            }

            scene.AppendChild(obj);
        }
        root.AppendChild(scene);

        xmlDoc.Save(path);
    }
}
