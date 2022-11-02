using System;
using System.IO.Ports; // 시리얼 통신을 위해 추가
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SerialCommuncation : MonoBehaviour
{
    //........Serial Field
    static SerialPort port = new SerialPort();
    string m_Data = string.Empty;
    ////SerialDataReceivedEventArgs e;
    //........Scene UI
    public TMP_Text tmp_Status;
    public TMP_Text tmp_ErrorMessage;
    public TMP_Text tmp_ReceivedData;
    public TMP_Dropdown dropdown_BaudRate;
    public TMP_Dropdown dropdown_PortName;
    string[] strings_PortName = new string[] { "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "COM10", "COM11", "COM12", "COM13", "COM14", "COM15", "COM16" };
    int[] ints_BaudRate = new int[] { 300, 600, 1200, 1800, 2400, 4800, 7200, 9600, 14400, 19200, 38400, 57600, 115200, 230400, 460800, 921600 };
    static string selectedPortName;
    static int selectedBaudRate;
    public Button btn_Connect;
    public Button btn_DisConnect;

    private void Awake()
    {
        AllReset();

        dropdown_PortName.onValueChanged.AddListener(delegate { DropdownPortName(dropdown_PortName); });
        dropdown_BaudRate.onValueChanged.AddListener(delegate { DropdownBaudRate(dropdown_BaudRate); });
        btn_Connect.onClick.AddListener(BTN_Connect);
        btn_DisConnect.onClick.AddListener(BTN_Disconnect);
    }

    private void Update()
    {
        if (port.IsOpen)
        {
            // 시리얼 버터에 수신된 데이터를 읽어오기
            m_Data = port.ReadLine();
            Debug.Log("m_Data : " + m_Data);
            // 수신된 데이터를 UI에 전달
            tmp_ReceivedData.text = m_Data;
            // 수신한 데이터를 출력
            port.WriteLine(tmp_ReceivedData.text);
            Debug.Log("출력 : " + tmp_ReceivedData.text);

            switch (Input.inputString)
            {
                case "A":
                case "a":
                    Debug.Log("press A");
                    port.WriteLine("A");
                    break;
                case "S":
                case "s":
                    Debug.Log("press S");
                    port.WriteLine("S");
                    break;
                case "D":
                case "d":
                    Debug.Log("press D");
                    port.WriteLine("D");
                    break;
                case "F":
                case "f":
                    Debug.Log("press F");
                    port.WriteLine("F");
                    break;
            }
        }
    }

    void OnApplicationQuit()
    {
        port.Close();
    }

    void DropdownPortName(TMP_Dropdown select)
    {
        selectedPortName = select.options[select.value].text;
        Debug.Log("Port Changed.\n---> " + selectedPortName);
    }

    void DropdownBaudRate(TMP_Dropdown select)
    {
        selectedBaudRate = Int32.Parse(select.options[select.value].text);
        Debug.Log("BaudRate Changed.\n---> " + selectedBaudRate);
    }

    void AllReset()
    {
        // Serial Port Close
        if (port.IsOpen)
            port.Close();
        // Value Reset
        selectedPortName = "COM1";
        selectedBaudRate = 300;
        tmp_ReceivedData.text = "";
        tmp_ErrorMessage.text = "";
        m_Data = string.Empty;
        // Dropdown PortName Reset
        dropdown_PortName.onValueChanged.RemoveAllListeners();
        dropdown_PortName.options.Clear();
        for (int i = 0; i < strings_PortName.Length; i++)
        {
            TMP_Dropdown.OptionData newData = new TMP_Dropdown.OptionData();
            newData.text = strings_PortName[i];
            dropdown_PortName.options.Add(newData);
        }
        dropdown_PortName.SetValueWithoutNotify(-1);
        dropdown_PortName.SetValueWithoutNotify(0);
        // Dropdown BaudRate Reset
        dropdown_BaudRate.onValueChanged.RemoveAllListeners();
        dropdown_BaudRate.options.Clear();
        for (int i = 0; i < ints_BaudRate.Length; i++)
        {
            TMP_Dropdown.OptionData newData = new TMP_Dropdown.OptionData();
            newData.text = ints_BaudRate[i].ToString();
            dropdown_BaudRate.options.Add(newData);
        }
        dropdown_BaudRate.SetValueWithoutNotify(-1);
        dropdown_BaudRate.SetValueWithoutNotify(0);
    }

    void BTN_Connect()
    {
        if (!port.IsOpen)
        {
            try
            {
                port.PortName = selectedPortName;
                port.BaudRate = selectedBaudRate;
                port.Parity = Parity.None;
                port.DataBits = 8;
                port.StopBits = StopBits.One;
                //port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
                port.Open();
                tmp_Status.text = "연결 상태 - 연결";
                dropdown_PortName.interactable = false;
                dropdown_BaudRate.interactable = false;
                btn_Connect.interactable = false;
                btn_DisConnect.interactable = true;
            }
            catch (Exception e)
            {
                tmp_ErrorMessage.text = e.ToString();
                Debug.Log(e.ToString());
            }
        }
    }

    void BTN_Disconnect()
    {
        //port.DataReceived -= new SerialDataReceivedEventHandler(DataReceived);
        port.Close();
        AllReset();
        tmp_Status.text = "연결 상태 - 해제";
        dropdown_PortName.interactable = true;
        dropdown_BaudRate.interactable = true;
        btn_Connect.interactable = true;
        btn_DisConnect.interactable = false;
    }

    //// 수신 이벤트가 발생하면 이 부분이 실행
    //void DataReceived(object sender, SerialDataReceivedEventArgs e)
    //{
    //    // 시리얼 버터에 수신된 데이터를 읽어오기
    //    m_Data = port.ReadLine();
    //    Debug.Log("m_Data : " + m_Data);
    //    // 수신된 데이터를 UI에 전달
    //    tmp_ReceivedData.text = m_Data;
    //    // 수신한 데이터를 출력
    //    port.WriteLine(tmp_ReceivedData.text);
    //    Debug.Log("출력 : " + tmp_ReceivedData.text);
    //    ////읽기 작업을 마쳐야 하는 제한 시간(ms)
    //    //port.ReadTimeout = 30;
    //}
}