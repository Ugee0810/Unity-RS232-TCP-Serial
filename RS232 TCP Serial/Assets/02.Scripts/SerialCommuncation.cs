using System;
using System.Collections;
using System.Text;
using System.IO.Ports; // 시리얼 통신을 위해 추가
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class SerialCommuncation : MonoBehaviour
{
    //........Serial Field
    static SerialPort m_RS232Port = new SerialPort();
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
        dropdowns[0].onValueChanged.AddListener(delegate { DropdownPortName(m_PortName); });
        dropdowns[1].onValueChanged.AddListener(delegate { DropdownBaudRate(m_BaudRate); });
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
                TMP_Dropdown.OptionData newData = new TMP_Dropdown.OptionData();
                newData.text = strings[temp];
                m_TMP_Dropdown[0].options.Add(newData);
            }
            // BaudRate 옵션 추가
            for (int i = 0; i < ints.Length; i++)
            {
                temp = i;
                TMP_Dropdown.OptionData newData = new TMP_Dropdown.OptionData();
                newData.text = ints[temp].ToString();
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
        //----- RS232 수신부
        int RecvSize = m_RS232Port.BytesToRead;
        string RecvStr = string.Empty;
        tmp_ReceivedData.text = string.Empty;
        // Recv Data가 있는 경우
        if (RecvSize != 0)
        {
            byte[] buff = new byte[RecvSize];
            // Size 만큼 Read
            m_RS232Port.Read(buff, 0, RecvSize);
            // Hex 변환
            for (int i = 0; i < RecvSize; i++)
                RecvStr += " " + buff[i].ToString("X2");
            tmp_ReceivedData.text += RecvStr;
        }
        //----- TCP 송신부
        if (tmp_ReceivedData.text != string.Empty)
        {
            Server server = new Server();
            server.OnStartButton();
            videoPlayer.Play();
        }
    }

    // 영상 출력을 위한 RawImage
    [SerializeField] RawImage rawImageDrawVideo;
    // 영상 재생용 VideoPlayer
    [SerializeField] VideoPlayer videoPlayer;
    //// 사운드 재생용 AudioSource
    //[SerializeField] AudioSource audioSource;

    //public void OnLoad(System.IO.FileInfo file)
    //{
    //    // MP4 파일을 불러와서 재생
    //    StartCoroutine(LoadVideo(file.FullName));
    //}

    //IEnumerator LoadVideo(string fullPath)
    //{
    //    // 경로 정보 설정
    //    videoPlayer.url = "file://" + fullPath;
    //    // 동영상 소리 재생 모드 : AudioSource
    //    videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
    //    // 동영상 소리를 재생할 AudioSource 설정
    //    videoPlayer.EnableAudioTrack(0, true);
    //    // 오디오 트랙 디코딩 활성/비활성화(VideoPlayer가 재생 중이 아닐 때 설정)
    //    videoPlayer.SetTargetAudioSource(0, audioSource);
    //    // videoPlayer에 등록된 영상이 사운드 재생으로 사용하기 때문에 audioSource.clip은 비워둔다.
    //    audioSource.clip = null;
    //    // 동영상에 출력되는 이미지를 imageDrawTexture에 설정(설정이 안되어 있으면 코드로 설정)
    //    rawImageDrawVideo.texture = videoPlayer.targetTexture;
    //    // clip 정보를 동적으로 변경할 때는 Prepare() 호출 후 Prepare가 완료되어야 재생 가능
    //    videoPlayer.Prepare();
    //    while (!videoPlayer.isPrepared) yield return null;
    //    // MP4 동영상/사운드 재생
    //    videoPlayer.Play();
    //    audioSource.Play();
    //}
}