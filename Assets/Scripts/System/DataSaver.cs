using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class Value {
	public string label;
	public byte[] data;
};

public class DataSaver : MonoBehaviour
{
	#region PIV
	private List<Value> values;
	private bool debug;
	#endregion
	
	#region Singleton
	public static DataSaver GetInstance() {
		return GameObject.FindObjectOfType<DataSaver>();
	}
	#endregion
	
    void Awake(){
        // Destroy duplicates
        DataSaver[] list = GameObject.FindObjectsOfType<DataSaver>();
        if ( list.Length > 1 ){
            // Found more than 1 object, meaning this instance is a duplicate
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
    }

	void Start() {
		values = new List<Value>();
	}
	
	#region Public Functions
	public void RegisterLabel(string label) {
		bool exists = CheckLabel(label);
		
		if(exists) {
			if(debug) Debug.Log("Label Registering Failed: label exists");	
		} else {
			Value v = new Value();
			v.label = label;
			values.Add(v);
			if(debug) Debug.Log("Label Registering Successful");	
		}
	}
	
	public void SetData(string label, string data) {
		bool exists = CheckLabel(label);
		
		if(!exists) {
			if(debug) Debug.Log("Set Data Failed: label does not exist");
		} else {
			Value v = getValue(label);
			v.data = stringToBytes(data);
			if(debug) Debug.Log("Set Data Successful");
		}
	}
	
	public void SetData(string label, int data) {
		bool exists = CheckLabel(label);
		
		if(!exists) {
			if(debug) Debug.Log("Set Data Failed: label does not exist");	
		} else {
			Value v = getValue(label);
			v.data = intToBytes(data);
			if(debug) Debug.Log("Set Data Successful");	
		}
	}
	
	public void SetData(string label, float data) {
		bool exists = CheckLabel(label);
		
		if(!exists) {
			if(debug) Debug.Log("Set Data Failed: label does not exist");	
		} else {
			Value v = getValue(label);
			v.data = intToBytes((int)(data * 1000));
			if(debug) Debug.Log("Set Data Successful");	
		}
	}
	
	public void SetData(string label, bool data) {
		bool exists = CheckLabel(label);
		
		if(!exists) {
			if(debug) Debug.Log("Set Data Failed: label does not exist");	
		} else {
			Value v = getValue(label);
			v.data = boolToBytes(data);
			if(debug) Debug.Log("Set Data Successful");	
		}
	}
	
	public string GetDataAsString(string label) {
		bool exists = CheckLabel(label);
		if(exists) {
			Value v = getValue(label);
			if(debug) Debug.Log("Got Data as 'string'");
			return bytesToString(v.data);
		} else {
			if(debug) Debug.Log("Get Data Failed: label does not exist");
			return "";
		}
	}
	
	public int GetDataAsInt(string label) {
		bool exists = CheckLabel(label);
		if(exists) {
			Value v = getValue(label);
			if(debug) Debug.Log("Got Data as 'int'");
			return bytesToInt(v.data);
		} else {
			if(debug) Debug.Log("Get Data Failed: label does not exist");
			return -1;
		}
	}
	
	public float GetDataAsFloat(string label) {
		bool exists = CheckLabel(label);
		if(exists) {
			Value v = getValue(label);
			if(debug) Debug.Log("Got Data as 'int'");
			return bytesToInt(v.data) / 1000.00f;
		} else {
			if(debug) Debug.Log("Get Data Failed: label does not exist");
			return -1.0f;
		}
	}
	
	public bool GetDataAsBool(string label) {
		bool exists = CheckLabel(label);
		if(exists) {
			Value v = getValue(label);
			if(debug) Debug.Log("Got Data as 'bool'");
			return bytesToBool(v.data);
		} else {
			if(debug) Debug.Log("Get Data Failed: label does not exist");
			return false;
		}
	}
	
	public byte[] ConvertToBytes() {
		List<byte[]> blocks = new List<byte[]>();
		
		int size = 0;
		
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
		
		byte[] values_bytes = new byte[size];
		int index = 0;
		for(int i = 0; i < blocks.Count; i++) {
			System.Buffer.BlockCopy(blocks[i], 0, values_bytes, index, blocks[i].Length);
			index += blocks[i].Length;
		}

        return values_bytes;
	}
	
	public void LoadData(byte[] rawData) {
		byte[] bytes = rawData;
		values.Clear();
		
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
			
			if(debug) Debug.Log("Value loaded with label: " + label + " and data: " + data);
			
			index += 5 + l;
		}
		
		if(debug) Debug.Log("Values Loaded");
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