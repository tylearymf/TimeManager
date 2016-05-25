using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using LitJson;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TimeManager : MonoBehaviour
{
    [Serializable]
    public struct MyDate
    {
        public int Year;
        public int Month;
        public int Day;

        public MyDate(int Year, int Month, int Day)
        {
            this.Year = Year;
            this.Month = Month;
            this.Day = Day;
        }

        public string DateFormatter
        {
            get
            {
                return Year + "-" + Month + "-" + Day;
            }
        }
    }
    [SerializeField]
    private MyDate _StartDate;
    public MyDate StartDate
    {
        set { _StartDate = value; }
        get { return _StartDate; }
    }
    [SerializeField]
    private MyDate _EndDate;
    public MyDate EndDate
    {
        set { _EndDate = value; }
        get { return _EndDate; }
    }
    [SerializeField]
    private int _RunDay;
    public int RunDay
    {
        get
        {
            if (_RunDay <= 0)
                _RunDay = 1;
            else if (_RunDay > 730)
                _RunDay = 730;
            return _RunDay;
        }
    }

    [HideInInspector]
    [SerializeField]
    private string _TimeGUIDKey;
    public string TimeGUIDKey
    {
        set { _TimeGUIDKey = value; }
        get { return _TimeGUIDKey; }
    }

    string DefaultString = "";
    string RunString = "InRunTime";
    string QuitString = "TimeOut";

    string timeURL = "http://apis.baidu.com/3023/time/time";//百度API时间地址
    float timeOut = 60; ///一分钟超时


    void Awake()
    {
#if UNITY_EDITOR
        CryptoPrefs.SetString(TimeGUIDKey, DefaultString);
#endif

        ///首先判断runStr的值是否为TimeOut，是则退出，否则联网取服务器时间，真则设置TimeGUIDKey为InRunTime，否则设置为TimeOut
        /// 如果联网失败，则取本地时间
        ///本地时间不正确，直接退出程序，设置TimeGUIDKey为“”

        string runStr = CryptoPrefs.GetString(TimeGUIDKey);

        if (runStr == DefaultString || runStr == RunString)
        {
            ///判断是否时间过期
            StartCoroutine(NetWorkTime());
        }
        else if (runStr == QuitString)
        {
            ///退出程序
            Quit();
        }
    }

    IEnumerator NetWorkTime()
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("apikey", "ee9f45dd50463fba83555cc8061179d1");

        WWW www = new WWW(timeURL, null, dic);
        float timer = 0;
        bool failed = false;

        while (!www.isDone)
        {
            if (timer > timeOut)
            {
                failed = true;
                break;
            }
            yield return null;
        }


        if (failed || !string.IsNullOrEmpty(www.error))
        {
            Debug.Log("<color>" + www.error + "</color>");
            www.Dispose();
            ///读取网络时间超时
            /// 读取本地时间赋值至PlayerPrefs
            Debug.Log("<color>读取网络时间超时！！！读取本地时间中。。。" + "</color>");
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            CheckDateTime(currentDate, true);
        }
        else
        {
            ///读取网络时间赋值至PlayerPrefs
            //Debug.Log("<color>" + www.text + "</color>");

            try
            {
                JsonData jd = JsonMapper.ToObject(www.text);

                string timeString = jd["stime"].ToString();

                DateTime currentDate = ConvertIntDateTime(timeString);
                string date = currentDate.ToString("yyyy-MM-dd");
                Debug.Log("<color>读取服务器时间成功！！！当前服务器时间：" + date + "</color>");
                CheckDateTime(date);
            }
            catch
            {
                Debug.Log("<color>读取服务器时间失败！！！读取本地时间中。。。" + "</color>");
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
                CheckDateTime(currentDate, true);
            }
        }
    }

    /// <summary>
    ///  时间戳转DateTime
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public DateTime ConvertIntDateTime(string timeStamp)
    {
        DateTime dtStart = DateTime.Now;
        TimeSpan toNow = new TimeSpan();
        try
        {
            dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            toNow = new TimeSpan(lTime);
        }
        catch { }

        return dtStart.Add(toNow);
    }

    void CheckDateTime(string currentDate, bool isLocal = false)
    {
        string startDate = _StartDate.DateFormatter;
        string endDate = _EndDate.DateFormatter;

        if (isLocal)
            Debug.Log("<color>当前电脑时间为:" + currentDate + "  " + "开始时间为:" + startDate + "  " + "结束时间为:" + endDate + "  " + "日期差:" + GetDateDiff() + "</color>");
        else
            Debug.Log("<color>当前服务器时间为:" + currentDate + "  " + "开始时间为:" + startDate + "  " + "结束时间为:" + endDate + "  " + "日期差:" + GetDateDiff() + "</color>");

        if (DateTime.Parse(currentDate) >= DateTime.Parse(startDate) && DateTime.Parse(currentDate) <= DateTime.Parse(endDate))
        {
            CryptoPrefs.SetString(TimeGUIDKey, RunString);
        }
        else
        {
            if (!isLocal)
                CryptoPrefs.SetString(TimeGUIDKey, QuitString);

            Quit();
        }
    }

    /// <summary>
    /// 计算开始日期与结束日期的差(天数)
    /// </summary>
    /// <returns></returns>
    public int GetDateDiff()
    {
        try
        {
            string startDate = _StartDate.DateFormatter;
            string endDate = _EndDate.DateFormatter;

            DateTime dt = DateTime.Parse(startDate);
            DateTime dt2 = DateTime.Parse(endDate);

            return (int)(dt2 - dt).TotalDays;
        }
        catch
        {
            return 0;
        }
    }

    public bool ChangeDate()
    {
        try
        {
            DateTime nowDateTime = DateTime.Now;

            this.StartDate = new MyDate(nowDateTime.Year, nowDateTime.Month, nowDateTime.Day);
            DateTime endDateTime = nowDateTime.AddDays(Convert.ToDouble(RunDay));
            this.EndDate = new MyDate(endDateTime.Year, endDateTime.Month, endDateTime.Day);

            TimeGUIDKey = Guid.NewGuid().ToString();
            //Debug.Log("RunKey=" + _RunDayKey);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private void Quit()
    {
        Debug.Log("程序退出！！");
#if UNITY_EDITOR
        EditorApplication.isPaused = true;
#else
        Application.Quit();
#endif
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TimeManager))]
public class MyTimeManager : Editor
{
    private TimeManager mtm;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        mtm = (TimeManager)target;
        //EditorGUILayout.LabelField("日期差：", mtm.GetDateDiff().ToString(), GUILayout.Width(150));
        EditorGUILayout.LabelField("运行天数Key：", mtm.TimeGUIDKey, GUILayout.Width(800));

        if (GUILayout.Button(new GUIContent("调整为当前日期-期限为" + mtm.RunDay + "天")))
        {
            if (!mtm.ChangeDate())
            {
                Debug.Log("修改失败！！！");
            }
        }
    }
}
#endif
