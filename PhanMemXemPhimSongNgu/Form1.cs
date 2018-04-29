using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
namespace PhanMemXemPhimSongNgu
{
    public partial class Form1 : Form
    {
        static int index=0;
        static long timeStart = 0;
        static long timeEnd = 0;
        static double temp;
        static string sub;
        static List<Match> list =new List<Match>();
        private static Regex unit = new Regex(
           @"(?<sequence>\d+)\r\n(?<start>\d{2}\:\d{2}\:\d{2},\d{3}) --\> (?<end>\d{2}\:\d{2}\:\d{2},\d{3})\r\n(?<text>[\s\S]*?\r\n\r\n)",
           RegexOptions.Compiled | RegexOptions.ECMAScript);
        public Form1()
        {
            InitializeComponent();
        }

        private void btnchoosefile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openfileDialog1 = new OpenFileDialog();
            if (openfileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.txtPath.Text = openfileDialog1.FileName;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.URL = this.txtPath.Text;
            axWindowsMediaPlayer1.Ctlcontrols.play();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();
        }

        private void axWindowsMediaPlayer1_Enter(object sender, EventArgs e)
        {
            readData();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(temp==0)
            {
                this.txt1.Text = "Sang dep trai";
            }
            else   this.txt1.Text = list[index].Groups[4].Value;
        }

        static void readData()
        {   
            using (StreamReader r = new StreamReader("abc.srt"))
            {
                string line,ans = "";
                while ((line = r.ReadLine()) != null )
                {
                    if (line == "")
                    {
                        ans = ans + "\r\n";
                        Match m = unit.Match(ans);
                        if (m.Success)
                            list.Add(m);
                        ans = "";
                    }
                    else ans = ans + line + "\r\n";
                }
            }
        }

         static bool isNext()
        {
            if (temp >= timeStart && temp <= timeEnd)
                return false;
            else
            {
                index++;
                timeStart = convertMs(list[index].Groups[2].Value);
                timeEnd = convertMs(list[index].Groups[3].Value);

                if (temp >= timeStart && temp <= timeEnd)
                    return false;
            }
            return false;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            temp = 1000 * axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            timeStart = convertMs(list[index].Groups[2].Value);
            timeEnd = convertMs(list[index].Groups[3].Value);
            textBox1_TextChanged(sender, e);
            timer1.Start();
        }
        static long convertMs(string time)
        {
            long ans = 0;
            string h, m, s, ms;

            h = time.Substring(0, 2);
            m = time.Substring(3, 2);
            s = time.Substring(6, 2);
            m = time.Substring(9, 3);

            ans = Int32.Parse(h) * 60 * 60 * 1000 + Int32.Parse(m) * 60 * 1000 + Int32.Parse(s) * 1000 + Int32.Parse(m);
            return ans;
        }
    }
}
