using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
//using Aisino.MES.Model.DeviceModels;
//using Aisino.MES.Led.Model;
using System.Xml;

namespace Aisino.MES.Client.Common
{
   
    public class DeviceHelper
    {
        #region 对USB接口的使用(PHILIPH卡)
        [DllImport("dcrf32.dll")]
        public static extern int dc_init(Int16 port, Int32 baud);  //初试化
        [DllImport("dcrf32.dll")]
        public static extern short dc_exit(int icdev);
        [DllImport("dcrf32.dll")]
        public static extern short dc_reset(int icdev, uint sec);
        [DllImport("dcrf32.dll")]
        public static extern short dc_request(int icdev, char _Mode, ref uint TagType);

        [DllImport("dcrf32.dll")]
        public static extern short dc_pro_reset(int icdev, ref byte rlen, ref byte rbuff);
        [DllImport("dcrf32.dll")]
        public static extern short dc_pro_reset_hex(int icdev, ref byte rlen, ref byte rbuff);
        [DllImport("dcrf32.dll")]
        public static extern short dc_pro_resethex(int icdev, ref byte rlen, ref byte rbuff);



        [DllImport("dcrf32.dll")]
        public static extern short dc_card(int icdev, char _Mode, ref ulong Snr);
        [DllImport("dcrf32.dll")]
        public static extern short dc_halt(int icdev);
        [DllImport("dcrf32.dll")]
        public static extern short dc_anticoll(int icdev, char _Bcnt, ref ulong IcCardNo);
        [DllImport("dcrf32.dll")]
        public static extern short dc_beep(int icdev, uint _Msec);
        [DllImport("dcrf32.dll")]
        public static extern short dc_config_card(int icdev, char _CardType);
        [DllImport("dcrf32.dll")]
        public static extern short dc_authentication(int icdev, int _Mode, int _SecNr);

        [DllImport("dcrf32.dll")]
        public static extern short dc_load_key(int icdev, int mode, int secnr, [In] byte[] nkey);  //密码装载到读写模块中
        [DllImport("dcrf32.dll")]
        public static extern short dc_load_key_hex(int icdev, int mode, int secnr, string nkey);  //密码装载到读写模块中

        [DllImport("dcrf32.dll")]
        public static extern short dc_write(int icdev, int adr, [In] byte[] sdata);  //向卡中写入数据
        [DllImport("dcrf32.dll")]
        public static extern short dc_write(int icdev, int adr, [In] string sdata);  //向卡中写入数据
        [DllImport("dcrf32.dll")]
        public static extern short dc_write_hex(int icdev, int adr, [In] string sdata);  //向卡中写入数据(转换为16进制)

        [DllImport("dcrf32.dll")]
        public static extern short dc_read(int icdev, int adr, [Out] byte[] sdata);

        [DllImport("dcrf32.dll")]
        public static extern short dc_read(int icdev, int adr, [MarshalAs(UnmanagedType.LPStr)] StringBuilder sdata);  //从卡中读数据
        [DllImport("dcrf32.dll")]
        public static extern short dc_read_hex(int icdev, int adr, [MarshalAs(UnmanagedType.LPStr)] StringBuilder sdata);  //从卡中读数据(转换为16进制)
        [DllImport("dcrf32.dll")]
        public static extern short a_hex(ref byte oldValue, ref byte newValue, int len);  //普通字符转换成十六进制字符
        [DllImport("dcrf32.dll")]
        public static extern void hex_a(ref byte oldValue, ref byte newValue, int len);  //十六进制字符转换成普通字符

        [DllImport("dcrf32.dll")]
        public static extern short dc_request_b(int icdev, byte mode, byte AFI, byte N, ref byte atqb);

        [DllImport("dcrf32.dll")]
        public static extern short dc_attrib(int icdev, ref byte PUPI, byte CID);



        [DllImport("dcrf32.dll")]
        public static extern short dc_pro_command(int icdev, byte len, ref byte sbuff, ref byte rlen, ref byte rbuff, byte tt);
        [DllImport("dcrf32.dll")]
        public static extern short dc_pro_commandlink(int icdev, byte len, ref byte sbuff, ref byte rlen, ref byte rbuff, byte tt, byte FG);
        [DllImport("dcrf32.dll")]
        public static extern short dc_pro_commandlink_hex(int icdev, byte len, ref byte sbuff, ref byte rlen, ref byte rbuff, byte tt, byte FG);
        [DllImport("dcrf32.dll")]
        public static extern short dc_pro_commandsource(int icdev, byte len, ref byte sbuff, ref byte rlen, ref byte rbuff, byte timeout);
        [DllImport("dcrf32.dll")]
        public static extern short dc_pro_commandsourcehex(int icdev, byte len, ref byte sbuff, ref byte rlen, ref byte rbuff, byte timeout);


        [DllImport("dcrf32.dll")]
        public static extern short dc_setcpu(int icdev, byte SAMID);
        [DllImport("dcrf32.dll")]
        public static extern short dc_setcpupara(int icdev, byte cputype, byte cpupro, byte cpuetu);
        [DllImport("dcrf32.dll")]
        public static extern short dc_cpureset_hex(int icdev, [Out] byte[] rlen, [Out] byte[] rbuff);
        [DllImport("dcrf32.dll")]
        public static extern short dc_cpuapdu_hex(int icdev, [In] byte slen, [In] byte[] senddata, [Out] byte[] rlen, [Out] byte[] databuffer);

        #endregion
        //#region property
        //private static LedDevice leddeivce;

        //public static LedDevice Leddeivce
        //{
        //    get { return DeviceHelper.leddeivce; }
        //    set { DeviceHelper.leddeivce = value; }
        //}
        //private static VoiceDevice voicedeivce;

        //public static VoiceDevice Voicedeivce
        //{
        //    get { return DeviceHelper.voicedeivce; }
        //    set { DeviceHelper.voicedeivce = value; }
        //}
        //private static MettlerDevice mettlerdevice;

        //public static MettlerDevice Mettlerdevice
        //{
        //    get { return DeviceHelper.mettlerdevice; }
        //    set { DeviceHelper.mettlerdevice = value; }
        //}
        //private static DeskReader deskReader;

        //public static DeskReader DeskReader
        //{
        //    get { return DeviceHelper.deskReader; }
        //    set { DeviceHelper.deskReader = value; }
        //}

        //private static bool isScale;

        //public static bool IsScale
        //{
        //    get { return DeviceHelper.isScale; }
        //    set { DeviceHelper.isScale = value; }
        //}

        //private static string scale_id;

        //public static string Scale_id
        //{
        //    get { return DeviceHelper.scale_id; }
        //    set { DeviceHelper.scale_id = value; }
        //}

        ////private static LEDControlResult ledControlResult;

        ////public static LEDControlResult LedControlResult
        ////{
        ////    get { return DeviceHelper.ledControlResult; }
        ////    set { DeviceHelper.ledControlResult = value; }
        ////}

        //private static bool csl_used;

        //public static bool Csl_used
        //{
        //    get { return DeviceHelper.csl_used; }
        //    set { DeviceHelper.csl_used = value; }
        //}

        //private static string csl_ip;

        //public static string Csl_ip
        //{
        //    get { return DeviceHelper.csl_ip; }
        //    set { DeviceHelper.csl_ip = value; }
        //}
        //#endregion

        #region 硬件配置文件操作
        public static string LoadXMLFile()
        {
            XmlDocument dom = new XmlDocument();
            dom.Load("DeviceConfig.xml");//装载XML文档
            return dom.InnerXml.ToString();
            //XmlNode siteScaleConfig = dom.GetElementsByTagName("LoadoMettlerNumber").Item(0);
            //scale_id = siteScaleConfig.SelectSingleNode("number").InnerText;
            //int isscale = Convert.ToInt16(siteScaleConfig.SelectSingleNode("isLoadoMettler").InnerText);
            //if (isscale == 0)
            //{
            //    isScale = false;
            //}
            //else
            //{
            //    isScale = true;
            //}

            //XmlNode ledConfig = dom.GetElementsByTagName("LEDConfig").Item(0);

            //leddeivce = new LedDevice();
            //leddeivce.Used = Convert.ToInt16(ledConfig.SelectSingleNode("used").InnerText);
            //leddeivce.Ipaddress = ledConfig.SelectSingleNode("ipAddress").InnerText;
            //leddeivce.Port = Convert.ToInt16(ledConfig.SelectSingleNode("port").InnerText);
            //leddeivce.Width = Convert.ToInt16(ledConfig.SelectSingleNode("width").InnerText);
            //leddeivce.Height = Convert.ToInt16(ledConfig.SelectSingleNode("height").InnerText);
            //leddeivce.Singlerow = Convert.ToInt16(ledConfig.SelectSingleNode("singlerow").InnerText);
            //leddeivce.Font = Convert.ToInt16(ledConfig.SelectSingleNode("font").InnerText);
            //leddeivce.ControlType = ledConfig.SelectSingleNode("controlType").InnerText;
            //leddeivce.Row = Convert.ToInt16(ledConfig.SelectSingleNode("row").InnerText);
            //leddeivce.Column = Convert.ToInt16(ledConfig.SelectSingleNode("column").InnerText);
            //leddeivce.TxtAlige = Convert.ToInt16(ledConfig.SelectSingleNode("txtAlige").InnerText);
            //leddeivce.DefaultText = ledConfig.SelectSingleNode("defaultText").InnerText;
            //leddeivce.Dataoe = Convert.ToInt16(ledConfig.SelectSingleNode("dataoe").InnerText);
            //leddeivce.StopTime = Convert.ToInt16(ledConfig.SelectSingleNode("stoptime").InnerText);

            //XmlNode voiceConfig = dom.GetElementsByTagName("VoiceConfig").Item(0);
            //voicedeivce = new VoiceDevice();
            //voicedeivce.Used = Convert.ToInt16(voiceConfig.SelectSingleNode("used").InnerText);
            //voicedeivce.Ipaddress = voiceConfig.SelectSingleNode("ipAddress").InnerText;
            //voicedeivce.Port = Convert.ToInt16(voiceConfig.SelectSingleNode("port").InnerText);

            //XmlNode mettlerConfig = dom.GetElementsByTagName("MettlerConfig").Item(0);
            //mettlerdevice = new MettlerDevice();
            //mettlerdevice.Port = mettlerConfig.SelectSingleNode("port").InnerText;
            //mettlerdevice.Rate = Convert.ToInt16(mettlerConfig.SelectSingleNode("rate").InnerText);
            //mettlerdevice.Length = Convert.ToInt16(mettlerConfig.SelectSingleNode("length").InnerText);
            //mettlerdevice.Start = Convert.ToInt16(mettlerConfig.SelectSingleNode("start").InnerText);
            //mettlerdevice.End = Convert.ToInt16(mettlerConfig.SelectSingleNode("end").InnerText);
            //mettlerdevice.DataStart = Convert.ToInt16(mettlerConfig.SelectSingleNode("datastart").InnerText);
            //mettlerdevice.DataEnd = Convert.ToInt16(mettlerConfig.SelectSingleNode("dataend").InnerText);
            //mettlerdevice.StartData = mettlerConfig.SelectSingleNode("startdata").InnerText;
            //mettlerdevice.EndData = mettlerConfig.SelectSingleNode("enddata").InnerText;

            //XmlNode deskReaderConfig = dom.GetElementsByTagName("DeskReader").Item(0);
            //deskReader = new DeskReader();
            //deskReader.Used = Convert.ToInt16(deskReaderConfig.SelectSingleNode("used").InnerText);
            //deskReader.Port = deskReaderConfig.SelectSingleNode("port").InnerText;
            //deskReader.Rate = Convert.ToInt16(deskReaderConfig.SelectSingleNode("rate").InnerText);
            //deskReader.CardType = deskReaderConfig.SelectSingleNode("cardtype").InnerText;
        }

        public static void SaveXMLFile()
        {
            XmlDocument dom = new XmlDocument();
            dom.Load("DeviceConfig.xml");//装载XML文档 

            //#region 保存磅点信息
            //XmlNode scaleConfig = dom.GetElementsByTagName("LoadoMettlerNumber").Item(0);
            //scaleConfig.SelectSingleNode("number").InnerText = Scale_id;
            //if (isScale)
            //{
            //    scaleConfig.SelectSingleNode("isLoadoMettler").InnerText = "1";
            //}
            //else
            //{
            //    scaleConfig.SelectSingleNode("isLoadoMettler").InnerText = "0";
            //}
            //#endregion

            //#region 保存led信息
            //XmlNode ledConfig = dom.GetElementsByTagName("LEDConfig").Item(0);

            //ledConfig.SelectSingleNode("used").InnerText = DeviceHelper.Leddeivce.Used.ToString();
            //ledConfig.SelectSingleNode("singlerow").InnerText = DeviceHelper.Leddeivce.Singlerow.ToString();

            //ledConfig.SelectSingleNode("ipAddress").InnerText = DeviceHelper.Leddeivce.Ipaddress;
            //ledConfig.SelectSingleNode("port").InnerText = DeviceHelper.Leddeivce.Port.ToString();
            //ledConfig.SelectSingleNode("width").InnerText = DeviceHelper.Leddeivce.Width.ToString();
            //ledConfig.SelectSingleNode("height").InnerText = DeviceHelper.Leddeivce.Height.ToString();
            //ledConfig.SelectSingleNode("defaultText").InnerText = DeviceHelper.Leddeivce.DefaultText;
            //ledConfig.SelectSingleNode("font").InnerText = DeviceHelper.Leddeivce.Font.ToString();
            //ledConfig.SelectSingleNode("controlType").InnerText = DeviceHelper.Leddeivce.ControlType;
            //ledConfig.SelectSingleNode("row").InnerText = DeviceHelper.Leddeivce.Row.ToString();
            //ledConfig.SelectSingleNode("column").InnerText = DeviceHelper.Leddeivce.Column.ToString();
            //ledConfig.SelectSingleNode("txtAlige").InnerText = DeviceHelper.Leddeivce.TxtAlige.ToString();
            //ledConfig.SelectSingleNode("dataoe").InnerText = DeviceHelper.Leddeivce.Dataoe.ToString();
            //#endregion

            //#region 保存语音信息
            //XmlNode voiceConfig = dom.GetElementsByTagName("VoiceConfig").Item(0);
            //voiceConfig.SelectSingleNode("used").InnerText = DeviceHelper.voicedeivce.Used.ToString();

            //voiceConfig.SelectSingleNode("ipAddress").InnerText = DeviceHelper.voicedeivce.Ipaddress;
            //voiceConfig.SelectSingleNode("port").InnerText = DeviceHelper.voicedeivce.Port.ToString();
            //#endregion

            //#region 保存读卡器信息
            //XmlNode deskReaderConfig = dom.GetElementsByTagName("DeskReader").Item(0);

            //deskReaderConfig.SelectSingleNode("used").InnerText = DeviceHelper.deskReader.Used.ToString();

            //deskReaderConfig.SelectSingleNode("port").InnerText = DeviceHelper.deskReader.Port.ToString();
            //deskReaderConfig.SelectSingleNode("rate").InnerText = DeviceHelper.deskReader.Rate.ToString();
            //deskReaderConfig.SelectSingleNode("cardtype").InnerText = DeviceHelper.deskReader.CardType.ToString();
            //#endregion

            //#region 保存仪表配置
            //XmlNode mettlerConfig = dom.GetElementsByTagName("MettlerConfig").Item(0);
            //mettlerConfig.SelectSingleNode("port").InnerText = DeviceHelper.mettlerdevice.Port;
            //mettlerConfig.SelectSingleNode("rate").InnerText = DeviceHelper.mettlerdevice.Rate.ToString();
            //mettlerConfig.SelectSingleNode("length").InnerText = DeviceHelper.mettlerdevice.Length.ToString();
            //mettlerConfig.SelectSingleNode("start").InnerText = DeviceHelper.mettlerdevice.Start.ToString();
            //mettlerConfig.SelectSingleNode("end").InnerText = DeviceHelper.mettlerdevice.End.ToString();
            //mettlerConfig.SelectSingleNode("datastart").InnerText = DeviceHelper.mettlerdevice.DataStart.ToString();
            //mettlerConfig.SelectSingleNode("dataend").InnerText = DeviceHelper.mettlerdevice.DataEnd.ToString();
            //mettlerConfig.SelectSingleNode("startdata").InnerText = DeviceHelper.mettlerdevice.StartData;
            //mettlerConfig.SelectSingleNode("enddata").InnerText = DeviceHelper.mettlerdevice.EndData;
            //#endregion

            dom.Save("DeviceConfig.xml");
        }
        #endregion

        /// <summary>
        /// 将原始字符串，转换成符合LED（一整行）长度的字符串
        /// </summary>
        /// <param name="sourceStr"></param>
        /// <param name="goalLength"></param>
        /// <returns></returns>
        //public static string GetLedStringLength(string sourceStr,ref int row)
        //{
        //    string rtLedString = "";
        //    List<string> rtLedStrings = sourceStr.Split('|').ToList();
        //    //查找是否有超出超度的内容，则要截断
        //    for(int i = 0;i<rtLedStrings.Count;i++)
        //    {
        //        string ledString = rtLedStrings[i];
        //        int theLength = Encoding.Default.GetByteCount(ledString);
        //        if (theLength > leddeivce.Column)
        //        {
        //            string temp = "";
        //            List<string> temps = new List<string>();
        //            int stringLength = ledString.Length;
        //            for (int k = 0; k < stringLength; k++)
        //            {
        //                if (Encoding.Default.GetByteCount(temp) + Encoding.Default.GetByteCount(ledString[k].ToString()) > leddeivce.Column)
        //                {
        //                    string ledStringTemp = ledString.Substring(temp.Length, ledString.Length - temp.Length);
        //                    rtLedStrings.Insert(i + 1, ledStringTemp);
        //                    rtLedStrings[i] = temp;
        //                    break;
        //                }
        //                temp += ledString.Substring(k, 1);
        //            }
        //        }
        //    }
        //    row = rtLedStrings.Count;
        //    //重新整理填充值
        //    foreach (string ledString in rtLedStrings)
        //    {
        //        string rtLedStringTemp = ledString;
        //        int theLength = Encoding.Default.GetByteCount(ledString);
        //        //输出项长度小于led列数
        //        if (theLength < leddeivce.Column)
        //        {
        //            if (leddeivce.TxtAlige == 1)
        //            {
        //                //左对齐
        //                for (int i = 0; i < leddeivce.Column - theLength; i++)
        //                {
        //                    rtLedStringTemp += " ";
        //                }
        //            }
        //            else if (leddeivce.TxtAlige == 2)
        //            {
        //                //居中
        //                int fillLength = leddeivce.Column - theLength;
        //                int halfLength = fillLength / 2;
        //                for (int i = 0; i < halfLength; i++)
        //                {
        //                    rtLedStringTemp = " " + ledString;
        //                }
        //                for (int i = 0; i < halfLength; i++)
        //                {
        //                    rtLedStringTemp += " ";
        //                }
        //            }
        //            else
        //            {
        //                //右对齐
        //                for (int i = 0; i < leddeivce.Column - theLength; i++)
        //                {
        //                    rtLedStringTemp = " " + ledString;
        //                }
        //            }
        //        }
        //        rtLedString += rtLedStringTemp;
        //    }
        //    return rtLedString;
        //}

        public static byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        public static string GetD8CardId(ref string errorMessage)
        {
            int icdev = dc_init(100, 115200);//第一个参数100为USB口，0为串口一，1为串口二等等。
            if (icdev <= 0)
            {
                errorMessage = "打开端口失败";
                return string.Empty;
            }
            int st = dc_beep(icdev, 10);

            //uint tagType = 8;//TypeA的非接触式CPU卡的特征值
            ulong icCardNo = 0;

            st = dc_card(icdev, '0', ref icCardNo);
            if (st != 0)
            {
                errorMessage = "卡号读取失败";
                return string.Empty;
            }
            else
            {
                errorMessage = string.Empty;
                return icCardNo.ToString();
            }
        }
    }
}
