using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SPUPTestScriptSimple : MonoBehaviour
{
    #region Property

    [Header("시뮬레이션 스타트 플래그")]
    [SerializeField] bool isStart = false;

    [Header("Serial Ports(RS232)")]
    [SerializeField] SerialPortUtility.SerialPortUtilityPro serialPortCOM3 = null;
    [SerializeField] SerialPortUtility.SerialPortUtilityPro serialPortCOM5 = null;

	[Header("Cylinder Timers")]
	[SerializeField] float[] timers = new float[]
	{
		// [실린더 전진($buson.1,)] - 버스 앞 바퀴
		48.0f,
		// [실린더 전진($buson.1,)] - 버스 뒷 바퀴
		48.7f,
		// [실린더 전진($buson.1,)] - 아이
		56.3f,
		// [실린더 전진($buson.1,)] - 차
		112.0f,
		// [실린더 후진($buson.0,)] - 버스 앞, 뒤
		0.3f,
		// [실린더 후진($buson.0,)] - 아이, 차
		0.5f
    };

	[Header("Edit Timer Input Fields")]
 	[SerializeField] TMP_InputField[] tmpIF_TimingValues;

    // PlayerPrefs Key Names
    string[] keyNames = new string[]
	{
        "Value_BusFrontW",
        "Value_BusBackW",
        "Value_StrikeHuman",
        "Value_StrikeVehicle",
        "Value_BusDelay",
        "Value_StrikeDelay"
    };

	#endregion Property

	private void Awake()
	{
        #region 기존 타이머 변경사항 가져오기

        for (int i = 0; i < timers.Length; i++)
		{
			if (PlayerPrefs.HasKey(keyNames[i]))
			{
				timers[i] = PlayerPrefs.GetFloat(keyNames[i]);
				tmpIF_TimingValues[i].text = timers[i].ToString();
			}
		}

        #endregion 기존 타이머 변경사항 가져오기

        #region InputField AddListener

        tmpIF_TimingValues[0].onEndEdit.AddListener(ValueChanged0);
		tmpIF_TimingValues[1].onEndEdit.AddListener(ValueChanged1);
		tmpIF_TimingValues[2].onEndEdit.AddListener(ValueChanged2);
		tmpIF_TimingValues[3].onEndEdit.AddListener(ValueChanged3);
		tmpIF_TimingValues[4].onEndEdit.AddListener(ValueChanged4);
		tmpIF_TimingValues[5].onEndEdit.AddListener(ValueChanged5);

        void ValueChanged0(string text)
		{
			timers[0] = float.Parse(text);
			PlayerPrefs.SetFloat(keyNames[0], timers[0]);
			PlayerPrefs.Save();
		}

		void ValueChanged1(string text)
		{
            timers[1] = float.Parse(text);
			PlayerPrefs.SetFloat(keyNames[1], timers[1]);
            PlayerPrefs.Save();
        }

		void ValueChanged2(string text)
		{
            timers[2] = float.Parse(text);
			PlayerPrefs.SetFloat(keyNames[2], timers[2]);
            PlayerPrefs.Save();
		}

		void ValueChanged3(string text)
		{
            timers[3] = float.Parse(text);
			PlayerPrefs.SetFloat(keyNames[3], timers[3]);
            PlayerPrefs.Save();
		}

		void ValueChanged4(string text)
		{
            timers[4] = float.Parse(text);
			PlayerPrefs.SetFloat(keyNames[4], timers[4]);
            PlayerPrefs.Save();
		}

		void ValueChanged5(string text)
		{
            timers[5] = float.Parse(text);
			PlayerPrefs.SetFloat(keyNames[5], timers[5]);
            PlayerPrefs.Save();
		}

        #endregion InputField AddListener

        #region StartCoroutines

        LogConnectedDeviceList();
        // PC ---> VR
        StartCoroutine(RStoServerStart());
        // 버스 앞 바퀴
        StartCoroutine(CylinderMove(timers[0], timers[4]));
        // 버스 뒷 바퀴
        StartCoroutine(CylinderMove(timers[1], timers[4]));
        // 아이 충돌
        StartCoroutine(CylinderMove(timers[2], timers[5]));
        // 차 충돌
        StartCoroutine(CylinderMove(timers[3], timers[5]));

        #endregion StartCoroutines
    }

	#region 버튼 이벤트 : 실린더 전/후진

	public void BUSON()
	{
        serialPortCOM5.Write("$buson.1,");
    }

	public void BUSOFF()
	{
        serialPortCOM5.Write("$buson.0,");
    }

    #endregion 버튼 이벤트 : 실린더 전/후진

    #region TCP, RS232(COM5) 전송

    /// <summary>
    /// WaitUntil에 의해 대기 ---> COM3 버튼에 의해 통과 ---> VR에게 TCP 송신
    /// </summary>
    /// <returns></returns>
    IEnumerator RStoServerStart()
    {
        Debug.Log("RStoServerStart() 코루틴 진입");
        yield return new WaitUntil(() => isStart);
        Debug.Log("RStoServerStart() WaitUntil 통과");
        Server.Instance.OnStartButton();
        Debug.Log("시리얼 통신에 의해 시뮬레이션 시작함");
        yield break;
    }

    /// <summary>
    /// WaitUntil에 의해 대기 ---> COM3 버튼에 의해 통과 ---> COM5에게 string 송신
    /// </summary>
    /// <returns></returns>
    IEnumerator CylinderMove(float firstTime, float afterTime)
    {
        Debug.Log("CylinderMove() 코루틴 진입");
        yield return new WaitUntil(() => isStart);
        Debug.Log("CylinderMove() WaitUntil 통과");
        yield return new WaitForSeconds(firstTime);
        serialPortCOM5.Write("$buson.1,");
        Debug.Log("실린더 전진");
        yield return new WaitForSeconds(afterTime);
        serialPortCOM5.Write("$buson.0,");
        Debug.Log("실린더 후진");
        yield break;
    }

    #endregion TCP, RS232(COM5) 전송

    #region Asset Methods

    //for List data
    public void ReadComplateList(object data)
	{
		var text = data as List<string>;
		for (int i = 0; i < text.Count; ++i)
			Debug.Log(text[i]);
	}

	//Sensor Example
	public void ReadComplateSensorAB(object data)
	{
		var text = data as List<string>;
		if(text.Count != 4)
			return; //discard

		string[] SensorA = text[1].Split(",".ToCharArray());
		string[] SensorB = text[3].Split(",".ToCharArray());

		Vector3 SensorAv = new Vector3(float.Parse(SensorA[0]), float.Parse(SensorA[1]), float.Parse(SensorA[2]));
		Vector3 SensorBv = new Vector3(float.Parse(SensorB[0]), float.Parse(SensorB[1]), float.Parse(SensorB[2]));
		Debug.Log(SensorAv);
		Debug.Log(SensorBv);
	}

	//for Dictonary data
	public void ReadComplateDictonary(object data)
	{
		var text = data as Dictionary<string, string>;
		foreach (KeyValuePair<string, string> kvp in text)
		{
		  Debug.Log(string.Format("{0}={1}", kvp.Key, kvp.Value));
		}
	}

	//for String data
	public void ReadComplateString(object data)
	{
		var text = data as string;
		Debug.Log(text);

		if (text != string.Empty)
		{
			isStart = true;
		}
	}

	//for Streaming Binary Data
	public void ReadStreamingBinary(object data)
	{
		var bin = data as byte[];
		string byteArray = System.BitConverter.ToString(bin);
		Debug.Log(byteArray);
	}

	//for System Binary data
	public void ReadComplateProcessing(object data)
	{
		var binData = data as byte[];	//total 14 byte
		string header = System.Text.Encoding.ASCII.GetString(binData, 0, 3);	//Header
		byte[] mainData = new byte[9];	//9byte
		byte[] checkSum = new byte[2];	//2byte

		Array.Copy(binData, 3, mainData, 0, 9);	//main data 3-12 : 9 byte
		Array.Copy(binData, 12, checkSum, 0, 2);	//sum data 12-14 : 2byte

		//processing
		ushort checksumINT = BitConverter.ToUInt16(checkSum,0);	

		// This is heavy!
		//Debug.Log(header);
		//mainData[]
		//Debug.Log(checksumINT);
	}

	//for String data
	public void ReadComplateModbus(object data)
	{
		var mudbus = data as SerialPortUtility.SPUPMudbusData;

		string byteArray = System.BitConverter.ToString(mudbus.Data);
		Debug.Log(string.Format("ADDRESS:{0}, FUNCTION:{1}, DATA:{2}", mudbus.Address, mudbus.Function, byteArray));

		bool isRtuMode = serialPortCOM3.ReadProtocol == SerialPortUtility.SerialPortUtilityPro.MethodSystem.ModbusRTU;
		serialPortCOM3.Write(mudbus, isRtuMode); //echo
	}

    //Deviceinfo(기기 정보)
    public void LogConnectedDeviceList()
    {
        SerialPortUtility.SerialPortUtilityPro.DeviceInfo[] devicelistCOM3 = SerialPortUtility.SerialPortUtilityPro.GetConnectedDeviceList(serialPortCOM3.OpenMethod);
        foreach (SerialPortUtility.SerialPortUtilityPro.DeviceInfo d in devicelistCOM3)
        {
            Debug.Log("COM3 VendorID:" + d.Vendor + " COM3 DeviceName:" + d.SerialNumber);
        }

        SerialPortUtility.SerialPortUtilityPro.DeviceInfo[] devicelistCOM5 = SerialPortUtility.SerialPortUtilityPro.GetConnectedDeviceList(serialPortCOM5.OpenMethod);
        foreach (SerialPortUtility.SerialPortUtilityPro.DeviceInfo d in devicelistCOM5)
        {
            Debug.Log("COM5 VendorID:" + d.Vendor + " COM5 DeviceName:" + d.SerialNumber);
        }
    }

    #endregion Asset Methods
}