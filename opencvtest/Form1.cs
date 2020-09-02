using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace opencvtest
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        //String AppPlayerName = "Whale";
        Screen[] scr = Screen.AllScreens;
        int iClicked = 0;
        Bitmap Stone_Img = null;
        int[] count;
        PictureBox[] pictureBoxArray;
        TextBox[] textBoxArray;
        public Form1()
        {
            InitializeComponent();

            DirectoryInfo macroFileDirectory = new DirectoryInfo(Application.StartupPath + "\\Img");
            if (macroFileDirectory.Exists == false)
            {
                macroFileDirectory.Create();
            }
            foreach (var item in macroFileDirectory.GetFiles("*.png"))
            {
                comboBox1.Items.Add(item.ToString());
            }
            pictureBoxArray = new PictureBox[comboBox1.Items.Count];
            textBoxArray = new TextBox[comboBox1.Items.Count];
            count = new int[comboBox1.Items.Count];
            for (int i = 0; i < comboBox1.Items.Count; i++)
            {
                count[i] = 0;
                pictureBoxArray[i] = new PictureBox();
                pictureBoxArray[i].Location = new System.Drawing.Point(this.pictureBox_reference.Location.X + (90 * i), this.pictureBox_reference.Location.Y);
                pictureBoxArray[i].Name = "pictureBox_" + i.ToString();
                pictureBoxArray[i].Size = this.pictureBox_reference.Size;
                pictureBoxArray[i].SizeMode = pictureBox_reference.SizeMode;
                pictureBoxArray[i].Text = pictureBox_reference.Text;
                pictureBoxArray[i].Tag = i;
                pictureBoxArray[i].BackColor = Color.White;
                pictureBoxArray[i].Click += new System.EventHandler(this.pictureBox_reference_Click);
                Bitmap bmp;
                bmp = new Bitmap(Application.StartupPath + "\\Img\\" + comboBox1.Items[i].ToString());
                pictureBoxArray[i].Image = (Image)bmp;

                textBoxArray[i] = new TextBox();
                textBoxArray[i].Location = new System.Drawing.Point(this.textBox_reference.Location.X + (90 * i), this.textBox_reference.Location.Y);
                textBoxArray[i].Margin = this.textBox_reference.Margin;
                textBoxArray[i].Name = "pictureBox_" + i.ToString();
                textBoxArray[i].Size = this.textBox_reference.Size;
                textBoxArray[i].Text = 0.ToString();
                textBoxArray[i].TextAlign = HorizontalAlignment.Center;
                textBoxArray[i].Tag = i;

                this.Controls.Add(pictureBoxArray[i]);
                this.Controls.Add(textBoxArray[i]);
            }
        }        
        public void searchIMG(Bitmap screen_img, Bitmap find_img)
        {
            //스크린 이미지 선언
            using (Mat ScreenMat = OpenCvSharp.Extensions.BitmapConverter.ToMat(screen_img))
            //찾을 이미지 선언
            using (Mat FindMat = OpenCvSharp.Extensions.BitmapConverter.ToMat(find_img))
            //스크린 이미지에서 FindMat 이미지를 찾아라
            using (Mat res = ScreenMat.MatchTemplate(FindMat, TemplateMatchModes.CCoeffNormed))
            {
                //찾은 이미지의 유사도를 담을 더블형 최대 최소 값을 선언합니다.
                double minval, maxval = 0;
                //찾은 이미지의 위치를 담을 포인트형을 선업합니다.
                OpenCvSharp.Point minloc, maxloc;
                //찾은 이미지의 유사도 및 위치 값을 받습니다. 
                Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);
                Debug.WriteLine("찾은 이미지의 유사도 : " + maxval);

                if (maxval >= 0.9)
                {
                    OpenCvSharp.Point clickpoint = new OpenCvSharp.Point(maxloc.X + FindMat.Width / 2, maxloc.Y + FindMat.Height / 2);
                    MacroMouseClick(clickpoint);                    
                }

            }
        }
        private void MacroMouseClick(OpenCvSharp.Point savePoint)
        {
            SetCursorPos(savePoint.X, savePoint.Y);
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        private void pictureBox_reference_Click(object sender, EventArgs e)
        {
            var senderPictureBox = sender as PictureBox;
            count[(int)senderPictureBox.Tag] = count[(int)senderPictureBox.Tag] + 1;
            textBoxArray[(int)senderPictureBox.Tag].Text = count[(int)senderPictureBox.Tag].ToString();
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < count.Length; i++)
            {
                count[i] = 0;
                textBoxArray[i].Text = count[i].ToString();
            }
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "") return;
            Stone_Img = new Bitmap(Application.StartupPath + "\\Img\\" + comboBox1.Text);
            //플레이어 창 크기 만큼의 비트맵을 선언해줍니다.
            Bitmap bmp = new Bitmap(scr[0].Bounds.Width, scr[0].Bounds.Height);
            var captureimg = Graphics.FromImage(bmp);
            captureimg.CopyFromScreen(scr[0].Bounds.X, scr[0].Bounds.Y, 0, 0, scr[0].Bounds.Size);

            // pictureBox1 이미지를 표시해줍니다.
            pictureBox1.Image = bmp;

            searchIMG(bmp, Stone_Img);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"iexplore.exe", Application.StartupPath + "\\Img\\license.html");
        }
    }
}
