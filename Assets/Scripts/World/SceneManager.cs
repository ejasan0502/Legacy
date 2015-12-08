using UnityEngine;
using System.Xml;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class SceneManager {
    
    private Scene currentScene;
    private List<Scene> scenes;


    public SceneManager(){
        currentScene = null;
        scenes = new List<Scene>();

        LoadSceneData();
    }

    public void LoadScene(string s){
        // Clear previous scene
        foreach (GameObject o in currentScene.gameObjects){
            GameObject.Destroy(o);
        }
        currentScene.gameObjects = new List<GameObject>();

        // Load scene
        currentScene = (Scene) scenes.Where(sc => sc.name.ToLower() == s.ToLower());
        if ( !DebugWindow.Assert(currentScene == null, "Cannot find scene, " + s) ){
            foreach (SceneObject so in currentScene.sceneObjects){
                GameObject o = (GameObject) GameObject.Instantiate(Resources.Load(GlobalVariables.PATH_SCENEOBJECTS+so.name));
                o.transform.position = so.position;
                o.transform.rotation = so.rotation;
                currentScene.gameObjects.Add(o);
            }
        }
    }
    private void LoadSceneData(){
        TextAsset textAsset = (TextAsset) Resources.Load(GlobalVariables.PATH_XMLDATA_SCENES,typeof(TextAsset));
        if ( DebugWindow.Assert(textAsset == null, "Cannot find scene data xml") ) return;
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(textAsset.text);

        XmlNode root = xmlDoc.FirstChild;
        foreach(XmlNode sceneNode in root.ChildNodes){
            Scene scene = new Scene();
            scene.name = sceneNode.Attributes["name"].Value;
            SceneObject so = new SceneObject();
            foreach (XmlNode obj in sceneNode.ChildNodes){
                if ( obj.Name == "Name" ) 
                    so.name = obj.InnerText;
                else if ( obj.Name == "Position" ) {
                    // Remove parathesis
                    string posString = obj.InnerText.Remove(0);
                    posString.Remove(posString.Length);

                    string[] pos = posString.Split(',');
                    so.position = new Vector3(float.Parse(pos[0]),float.Parse(pos[1]),float.Parse(pos[2]));
                } else {
                    // Remove parathesis
                    string rotString = obj.InnerText.Remove(0);
                    rotString.Remove(rotString.Length);

                    string[] rot = rotString.Split(',');
                    so.rotation = new Quaternion(float.Parse(rot[0]),float.Parse(rot[1]),float.Parse(rot[2]),float.Parse(rot[3]));
                }
            }
            scenes.Add(scene);
        }
    }
}

public class Scene {
    public string name;
    public List<SceneObject> sceneObjects;  // Reference of objects from xml file
    public List<GameObject> gameObjects;    // Actual game objects in scene

    public Scene(){
        name = "";
        sceneObjects = new List<SceneObject>();
        gameObjects = new List<GameObject>();
    }
}
public class SceneObject {
    public string name;
    public Vector3 position;
    public Quaternion rotation;

    public SceneObject(){
        name = "";
        position = Vector3.zero;
        rotation = Quaternion.identity;
    }
}
