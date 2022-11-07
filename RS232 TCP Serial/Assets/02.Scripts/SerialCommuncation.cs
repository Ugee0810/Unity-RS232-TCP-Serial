using System;
using System.IO.Ports; // 시리얼 통신을 위해 추가
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using UnityEditor.Experimental.GraphView;

public class SerialCommuncation : MonoBehaviour
{
    //........Serial Field
    static SerialPort m_RS232Port = new();
    int readCnt = 0;
    byte recvByte = 0;
    byte[] recvBuf = new byte[1024];

    string m_Data = string.Empty;
    //........Scene UI
    public TMP_Text tmp_Status;
    public TMP_Text tmp_ErrorMessage;
    public TMP_Text tmp_ReceivedData;
    public TMP_Dropdown[] dropdowns;
    public TMP_Dropdown m_PortName;
    public TMP_Dropdown m_BaudRate;
    string[] strings_PortName = new string[] { "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "COM10", "COM11", "COM12", "COM13", "COM14", "COM15", "COM16" };
    string selectedPortName;
    int[] ints_BaudRate = new int[] { 300, 600, 1200, 1800, 2400, 4800, 7200, 9600, 14400, 19200, 38400, 57600, 115200, 230400, 460800, 921600 };
    int selectedBaudRate;
    public Button btn_Connect;
    public Button btn_Disconnect;

    private void Awake()
    {
        ValueReset();
        // Serial Port Close
        if (m_RS232Port.IsOpen) m_RS232Port.Close();
        // UI AddListener
        dropdowns[0].onValueChanged.AddListener( delegate { DropdownPortName(m_PortName); });
        dropdowns[1].onValueChanged.AddListener( delegate { DropdownBaudRate(m_BaudRate); });
        btn_Connect.onClick.AddListener(BTN_Connect);
        btn_Disconnect.onClick.AddListener(BTN_Disconnect);
    }

    void OnApplicationQuit()
    {
        m_RS232Port.Close();
    }

    // Selected PortName
    void DropdownPortName(TMP_Dropdown select)
    {
        selectedPortName = select.options[select.value].text;
        Debug.Log("Port Changed. ---> " + selectedPortName);
    }

    // Selected BaudRate
    void DropdownBaudRate(TMP_Dropdown select)
    {
        selectedBaudRate = Int32.Parse(select.options[select.value].text);
        Debug.Log("BaudRate Changed. ---> " + selectedBaudRate);
    }

    void ValueReset()
    {
        selectedPortName = "COM1";
        selectedBaudRate = 300;
        m_Data = string.Empty;
        tmp_ErrorMessage.text = "";
        tmp_ReceivedData.text = "";
        RESET_TMP_Dropdown(dropdowns, strings_PortName, ints_BaudRate);

        /// <summary>
        /// Dropdowns Reset
        /// </summary>
        /// <param name="m_TMP_Dropdown">PortName([0]), BaudRate([1]) Dropdowns</param>
        /// <param name="strings">PortName Array</param>
        /// <param name="ints">BaudRate Array</param>
        void RESET_TMP_Dropdown(TMP_Dropdown[] m_TMP_Dropdown, string[] strings, int[] ints)
        {
            int temp;
            // 리스너 할당 해제, 옵션 초기화
            for (int i = 0; i < m_TMP_Dropdown.Length; i++)
            {
                temp = i;
                m_TMP_Dropdown[temp].onValueChanged.RemoveAllListeners();
                m_TMP_Dropdown[temp].options.Clear();
            }
            // PortName 옵션 추가
            for (int i = 0; i < strings.Length; i++)
            {
                temp = i;
                TMP_Dropdown.OptionData newData = new() { text = strings[temp] };
                m_TMP_Dropdown[0].options.Add(newData);
            }
            // BaudRate 옵션 추가
            for (int i = 0; i < ints.Length; i++)
            {
                temp = i;
                TMP_Dropdown.OptionData newData = new() { text = ints[temp].ToString() };
                m_TMP_Dropdown[1].options.Add(newData);
            }
            for (int i = 0; i < m_TMP_Dropdown.Length; i++)
            {
                temp = i;
                m_TMP_Dropdown[temp].SetValueWithoutNotify(-1);
                m_TMP_Dropdown[temp].SetValueWithoutNotify(0);
            }
        }
    }

    // Connect OnClick Listener
    void BTN_Connect()
    {
        if (!m_RS232Port.IsOpen)
        {
            try
            {
                SET_SerialPort(m_RS232Port);
                tmp_Status.text = "연결 상태 - 연결";
                tmp_ErrorMessage.text = "";
                dropdowns[0].interactable = false;
                dropdowns[1].interactable = false;
                btn_Connect.interactable = false;
                btn_Disconnect.interactable = true;
                Debug.Log($"Status is : {m_RS232Port.IsOpen}\nPortName : {m_RS232Port.PortName}\nBaudRate : {m_RS232Port.BaudRate}");
                Debug.Log("수고하셨습니다. 퇴근하세요.");
            }
            catch (Exception e)
            {
                tmp_ErrorMessage.text = e.ToString();
                Debug.Log(e.ToString());
            }
        }

        void SET_SerialPort(SerialPort port)
        {
            port.PortName = selectedPortName;
            port.BaudRate = selectedBaudRate;
            port.Parity = Parity.None;
            port.DataBits = 8;
            port.StopBits = StopBits.One;
            port.Encoding = Encoding.Default;
            port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            port.Open();
        }
    }

    // Disconnect OnClick Listener
    void BTN_Disconnect()
    {
        m_RS232Port.DataReceived -= new SerialDataReceivedEventHandler(DataReceived);
        if (m_RS232Port.IsOpen) m_RS232Port.Close();
        ValueReset();
        tmp_Status.text = "연결 상태 - 해제";
        dropdowns[0].interactable = true;
        dropdowns[1].interactable = true;
        btn_Connect.interactable = true;
        btn_Disconnect.interactable = false;
    }

    // 수신 이벤트가 발생하면 이 부분이 실행
    void DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        //----- 시리얼 통신 방법 -----//
        //// 시리얼 버터에 수신된 데이터를 읽어오기
        //m_Data = m_RS232Port.ReadLine();
        //Debug.Log("m_Data : " + m_Data);
        //// 수신된 데이터를 UI에 전달
        //tmp_ReceivedData.text = m_Data;
        //// 수신한 데이터를 출력
        //m_RS232Port.WriteLine(tmp_ReceivedData.text);
        //Debug.Log("출력 : " + tmp_ReceivedData.text);
        

        if (m_RS232Port.BytesToRead >= 0)
        {
            readCnt = m_RS232Port.Read(recvBuf, 0, 1024);
            recvByte = recvBuf[readCnt - 1];

            tmp_ReceivedData.text = readCnt.ToString();
        }
    }
}