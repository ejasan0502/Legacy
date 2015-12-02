using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Xml;
using System.Reflection;

// Value class
// Used to save data into a byte array
public class Value {
	public string label;
	public byte[] data;
};

// DataSaver.cs
// Saves data into bytes on the current storage device
public class DataSaver
{
	#region PIV
	private List<Value> values;     // List of values with bytes data
	private bool debug = true;      // Display debug messages
	#endregion
	
	#region Contructors
	public DataSaver() {
        // Initialize variables
		values = new List<Value>();
	}
	#endregion
	
	#region Public Functions
    // Add a value to list of value
    // @param1: label of the value
	public void RegisterLabel(string label) {
		bool exists = CheckLabel(label);
		
        // Check if the value exists with the label
		if(exists) {
            // Send error message
			if(debug) DebugWindow.Log("Label Registering Failed: label exists");	
		} else {
            // Create value and save to list
			Value v = new Value();
			v.label = label;
			values.Add(v);
			if(debug) DebugWindow.Log("Label Registering Successful");	
		}
	}
	
    // Set data of a value
    // @param1: label of value
    // @param2: string data to save on value
	public void SetData(string label, string data) {
		bool exists = CheckLabel(label);
		
        // Check if the value exists with the label
		if(!exists) {
            // Send error message
			if(debug) DebugWindow.Log("Set Data Failed: label does not exist");
		} else {
            // Get value and save data to value
			Value v = getValue(label);
			v.data = stringToBytes(data);
			if(debug) DebugWindow.Log("Set Data Successful");
		}
	}
	
    // Set data of a value
    // @param1: label of value
    // @param2: int data to save on value
	public void SetData(string label, int data) {
		bool exists = CheckLabel(label);
		
        // Check if the value exists with the label
		if(!exists) {
            // Send error message
			if(debug) DebugWindow.Log("Set Data Failed: label does not exist");	
		} else {
            // Get value and save data to value
			Value v = getValue(label);
			v.data = intToBytes(data);
			if(debug) DebugWindow.Log("Set Data Successful");	
		}
	}
	
    // Set data of a value
    // @param1: label of value
    // @param2: float data to save on value
	public void SetData(string label, float data) {
		bool exists = CheckLabel(label);
		
        // Check if the value exists with the label
		if(!exists) {
            // Send error message
			if(debug) DebugWindow.Log("Set Data Failed: label does not exist");	
		} else {
            // Get value and save data to value
			Value v = getValue(label);
			v.data = intToBytes((int)(data * 1000));
			if(debug) DebugWindow.Log("Set Data Successful");	
		}
	}
	
    // Set data of a value
    // @param1: label of value
    // @param2: bool data to save on value
	public void SetData(string label, bool data) {
		bool exists = CheckLabel(label);
		
        // Check if the value exists with the label
		if(!exists) {
            // Send error message
			if(debug) DebugWindow.Log("Set Data Failed: label does not exist");	
		} else {
            // Get value and save data to value
			Value v = getValue(label);
			v.data = boolToBytes(data);
			if(debug) DebugWindow.Log("Set Data Successful");	
		}
	}
	
    // Get data from list as a string
    // @param1: label of value
	public string GetDataAsString(string label) {
		bool exists = CheckLabel(label);

        // Check if the value exists with the label
		if(exists) {
            // Get value and return data 
			Value v = getValue(label);
			if(debug) DebugWindow.Log("Got Data as 'string'");
			return bytesToString(v.data);
		} else {
            // Send error message
			if(debug) DebugWindow.Log("Get Data Failed: label does not exist");
			return "";
		}
	}
	
    // Get data from list as an int
    // @param1: label of value
	public int GetDataAsInt(string label) {
		bool exists = CheckLabel(label);

        // Check if the value exists with the label
		if(exists) {
            // Get value and return data 
			Value v = getValue(label);
			if(debug) DebugWindow.Log("Got Data as 'int'");
			return bytesToInt(v.data);
		} else {
            // Send error message
			if(debug) DebugWindow.Log("Get Data Failed: label does not exist");
			return -1;
		}
	}
	
    // Get data from list as a float
    // @param1: label of value
	public float GetDataAsFloat(string label) {
		bool exists = CheckLabel(label);

        // Check if the value exists with the label
		if(exists) {
            // Get value and return data 
			Value v = getValue(label);
			if(debug) DebugWindow.Log("Got Data as 'int'");
			return bytesToInt(v.data) / 1000.00f;
		} else {
            // Send error message
			if(debug) DebugWindow.Log("Get Data Failed: label does not exist");
			return -1.0f;
		}
	}
	
    // Get data from list as a bool
    // @param1: label of value
	public bool GetDataAsBool(string label) {
		bool exists = CheckLabel(label);

        // Check if the value exists with the label
		if(exists) {
            // Get value and return data 
			Value v = getValue(label);
			if(debug) DebugWindow.Log("Got Data as 'bool'");
			return bytesToBool(v.data);
		} else {
            // Send error message
			if(debug) DebugWindow.Log("Get Data Failed: label does not exist");
			return false;
		}
	}

    // Save values in list as bytes on the current storage device
	public void SaveData(string path, params string[] dataLabel) {
		List<byte[]> blocks = new List<byte[]>();
		
		int size = 0;
		
        if ( dataLabel[0].ToLower() == "all" ){
		    foreach(Value v in values) {
			    byte[] label = Encoding.ASCII.GetBytes(v.label);
			    for(int i = 0; i < label.Length; i++) {
				    label[i] = (byte)(label[i] ^ 0xff);
			    }
			    byte[] data = v.data;
			
			    byte[] value_bytes = new byte[label.Length + 5];
			
			    value_bytes[0] = (byte)label.Length;
			
			    System.Buffer.BlockCopy(label, 0, value_bytes, 1, label.Length);
			    System.Buffer.BlockCopy(data, 0, value_bytes, 1 + label.Length, 4);
			
			    blocks.Add(value_bytes);
			
			    size += value_bytes.Length;
		    }
        } else {
            for (int k = 0; k < dataLabel.Length; k++){
                Value v = getValue(dataLabel[k]);
                byte[] label = Encoding.ASCII.GetBytes(v.label);
			    for(int i = 0; i < label.Length; i++) {
				    label[i] = (byte)(label[i] ^ 0xff);
			    }
			    byte[] data = v.data;
			
			    byte[] value_bytes = new byte[label.Length + 5];
			
			    value_bytes[0] = (byte)label.Length;
			
			    System.Buffer.BlockCopy(label, 0, value_bytes, 1, label.Length);
			    System.Buffer.BlockCopy(data, 0, value_bytes, 1 + label.Length, 4);
			
			    blocks.Add(value_bytes);
			
			    size += value_bytes.Length;
            }
        }
		
		byte[] values_bytes = new byte[size];
		int index = 0;
		for(int i = 0; i < blocks.Count; i++) {
			System.Buffer.BlockCopy(blocks[i], 0, values_bytes, index, blocks[i].Length);
			index += blocks[i].Length;
		}
		
		if (!System.IO.File.Exists(path))
		{
			FileStream fs = System.IO.File.Create(path);
			fs.Close();
		}
		
		File.WriteAllBytes(path, values_bytes);
		if(debug) DebugWindow.Log("Values Saved");
	}
    public void SaveData(PlayerCharacter pc){
        NetworkManager nm = NetworkManager.instance;

        #region Save data to xml string
        XmlDocument xmlDoc = new XmlDocument();
        XmlElement root = xmlDoc.CreateElement("Player");

        // Inventory
        if ( pc.inventory.items.Count > 0 )
            root.AppendChild(pc.inventory.ToXmlElement(xmlDoc));

        // Equipment
        XmlElement equipment = xmlDoc.CreateElement("Equipment");
        foreach (Equip e in pc.equipment){
            if ( e.id != "" )
                equipment.AppendChild(e.ToXmlElement(xmlDoc));
        }
        root.AppendChild(equipment);

        // Base Traits
        XmlElement baseTraits = xmlDoc.CreateElement("BaseTraits");
        foreach(FieldInfo fi in pc.baseTraits.GetType().GetFields()){
            XmlElement content = xmlDoc.CreateElement(fi.Name);
            content.InnerText = fi.GetValue(pc.baseTraits).ToString();
            baseTraits.AppendChild(content);
        }
        root.AppendChild(baseTraits);

        // Trait Points
        XmlElement traitPoints = xmlDoc.CreateElement("TraitPoints");
        traitPoints.InnerText = pc.traitPoints+"";
        root.AppendChild(traitPoints);

        // Skills
        XmlElement skills = xmlDoc.CreateElement("Skills");
        foreach (Skill s in pc.skills){
            skills.AppendChild(s.ToXmlElement(xmlDoc));
        }
        root.AppendChild(skills);

        // Last saved scene
        XmlElement lastSavedScene = xmlDoc.CreateElement("LastSavedScene");
        lastSavedScene.InnerText = pc.lastSavedScene;
        root.AppendChild(lastSavedScene);

        xmlDoc.AppendChild(root);

        // Save data as xml string
        StringWriter sw = new StringWriter();
        XmlWriter xw = XmlWriter.Create(sw);
        xmlDoc.WriteTo(xw);
        if ( !CheckLabel("playerData") ){
            RegisterLabel("playerData");
        }
        SetData("playerData",sw.ToString());
        xw.Close();
        sw.Close();
        #endregion
        #region Convert data to bytes
		List<byte[]> blocks = new List<byte[]>();
		int size = 0;

        Value v = getValue("playerData");
        byte[] label = Encoding.ASCII.GetBytes(v.label);
		for(int i = 0; i < label.Length; i++) {
			label[i] = (byte)(label[i] ^ 0xff);
		}
		byte[] data = v.data;
			
		byte[] value_bytes = new byte[label.Length + 5];
			
		value_bytes[0] = (byte)label.Length;
			
		System.Buffer.BlockCopy(label, 0, value_bytes, 1, label.Length);
		System.Buffer.BlockCopy(data, 0, value_bytes, 1 + label.Length, 4);
			
		blocks.Add(value_bytes);
			
		size += value_bytes.Length;

		byte[] values_bytes = new byte[size];
		int index = 0;
		for(int i = 0; i < blocks.Count; i++) {
			System.Buffer.BlockCopy(blocks[i], 0, values_bytes, index, blocks[i].Length);
			index += blocks[i].Length;
		}
        #endregion
        #region Save bytes data to server
        nm.SavePlayerDataToDatabase(values_bytes);
        #endregion
    }

    // Load values to list from bytes on the current storage device
	public void LoadData(string path) {
		byte[] bytes;
		values.Clear();
		
		if(File.Exists(path)) {
			bytes = File.ReadAllBytes(path);
		} else {
			FileStream fs = System.IO.File.Create(path);
			fs.Close();
			if(debug) DebugWindow.Log("No data file, data file created");
			return;
		}
		
		int index = 0;
		while(index < bytes.Length) {
			int l = bytes[index];
			byte[] labelBytes = new byte[l];
			byte[] dataBytes = new byte[4];
			System.Buffer.BlockCopy(bytes, index + 1, labelBytes, 0, l);
			System.Buffer.BlockCopy(bytes, index + 1 + l, dataBytes, 0, 4);
			
			for(int i = 0; i < labelBytes.Length; i++) {
				labelBytes[i] = (byte)(labelBytes[i] ^ 0xff);
			}
			string label = Encoding.ASCII.GetString(labelBytes);
			byte[] data = dataBytes;
			
			Value v = new Value();
			v.label = label;
			v.data = data;
			values.Add(v);
			
			if(debug) DebugWindow.Log("Value loaded with label: " + label + " and data: " + data);
			
			index += 5 + l;
		}
		
		if(debug) DebugWindow.Log("Values Loaded");
	}
    public void LoadDataFromBytes(byte[] _data){
		values.Clear();

        int index = 0;
		while(index < _data.Length) {
			int l = _data[index];
			byte[] labelBytes = new byte[l];
			byte[] dataBytes = new byte[4];
			System.Buffer.BlockCopy(_data, index + 1, labelBytes, 0, l);
			System.Buffer.BlockCopy(_data, index + 1 + l, dataBytes, 0, 4);
			
			for(int i = 0; i < labelBytes.Length; i++) {
				labelBytes[i] = (byte)(labelBytes[i] ^ 0xff);
			}
			string label = Encoding.ASCII.GetString(labelBytes);
			byte[] data = dataBytes;
			
			Value v = new Value();
			v.label = label;
			v.data = data;
			values.Add(v);
			
			if(debug) DebugWindow.Log("Value loaded with label: " + label + " and data: " + data);
			
			index += 5 + l;
		}
    }
	
	public void SetDebug(bool debug) {
		this.debug = debug;
	}
	
	public bool CheckLabel(string label) {
		foreach(Value v in values) {
			if(v.label == label)
				return true;
		}
		return false;
	}
	#endregion
	
	#region Private Functions
	private Value getValue(string label) {
		foreach(Value v in values) {
			if(v.label == label)
				return v;
		}
		return null;
	}
	
	private byte[] stringToBytes(string str) {
		return Encoding.ASCII.GetBytes(str);
	}
	
	private string bytesToString(byte[] byteArray) {
		return Encoding.ASCII.GetString(byteArray);
	}
	
	private byte[] intToBytes(int myInteger){
		byte[] byteArray = new byte[4];
		byteArray[0] = (byte)(myInteger >> 24);
		byteArray[1] = (byte)(myInteger >> 16);
		byteArray[2] = (byte)(myInteger >> 8);
		byteArray[3] = (byte)myInteger;
		return byteArray;
	}
	
	private int bytesToInt(byte[] byteArray){
		if (System.BitConverter.IsLittleEndian)
			System.Array.Reverse(byteArray);
		int myInteger = System.BitConverter.ToInt32(byteArray, 0);
		return myInteger;
	}
	
	private byte[] boolToBytes(bool myBool) {
		int num = (myBool) ? 1 : 0;
		byte[] byteArray = new byte[4];
		byteArray[0] = (byte)0;
		byteArray[1] = (byte)0;
		byteArray[2] = (byte)0;
		byteArray[3] = (byte)num;
		return byteArray;
	}
	
	private bool bytesToBool(byte[] byteArray) {
		return (byteArray[3] == 1);
	}		
	#endregion
}