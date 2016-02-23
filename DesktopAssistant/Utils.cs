using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Diagnostics;
using System.Media;
using System.IO;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using System.Threading;

namespace Assistant
{
    public class Utils
    {
        public static string Cal(string exp)
        {
            try
            {
                DataTable eval = new DataTable();
                object result = eval.Compute(exp, "");
                return result.ToString();
            }
            catch
            {
                return "无法计算的表达式";
            }
        }

        public static string NowDate()
        {
            string format = "{0}年{1}月{2}日{3}时{4}分";
            return string.Format(format, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute);
        }

        public static string OpenProcess(string url)
        {
            try
            {
                Process.Start(url);
                return "Success";
            }
            catch
            {
                return "找不到程序";
            }
        }

        public static string PlaySound(string path)
        {
            try
            {
                byte[] data;
                if (path.StartsWith("http"))
                {
                    System.Net.WebClient client = new System.Net.WebClient();
                    data = client.DownloadData(path);
                }
                else
                {
                    data = File.ReadAllBytes(path);
                }
                using (var ms = new MemoryStream(data))
                using (var rdr = new Mp3FileReader(ms))
                using (var wavStream = WaveFormatConversionStream.CreatePcmStream(rdr))
                using (var baStream = new BlockAlignReductionStream(wavStream))
                using (var waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
                {
                    waveOut.Init(baStream);
                    waveOut.Play();
                    /*while (waveOut.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(100);
                    }*/
                }
            }
            catch
            {
                return "播放错误";
            }
            return "Success";
        }

        public static string FindAnswer(string q)
        {
            return q;
        }
        
    }
}
