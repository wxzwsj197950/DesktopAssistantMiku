using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Assistant
{
    public partial class Form1 : Form
    {
        private Image stand = Image.FromFile(@"miku/stand.png");
        private Image standRight;
        private Image left = Image.FromFile(@"miku/left.png");
        private Image right;
        private Image sleep = Image.FromFile(@"miku/sleep.png");
        private Image fly = Image.FromFile(@"miku/fly.png");
        private Image fall = Image.FromFile(@"miku/fall.png");
        private Image dialog = Image.FromFile(@"miku/bubble.png");
        private Image[] walkAni = new Image[] {
            Image.FromFile(@"miku/walk1.png"),
            Image.FromFile(@"miku/walk2.png"),
            Image.FromFile(@"miku/walk3.png")
        };

        private Image[] bordAni = new Image[] {
            Image.FromFile(@"miku/bord0.png"),
            Image.FromFile(@"miku/bord1.png"),
            Image.FromFile(@"miku/bord2.png"),
            Image.FromFile(@"miku/bord3.png"),
            Image.FromFile(@"miku/bord4.png"),
            Image.FromFile(@"miku/bord5.png"),
            Image.FromFile(@"miku/bord6.png"),
            Image.FromFile(@"miku/bord7.png"),
            Image.FromFile(@"miku/bord8.png"),
            Image.FromFile(@"miku/bord9.png"),
        };

        private int nowFrame = 0;

        private bool facingLeft = true;

        private int PADDING = 40;

        public Form1()
        {
            InitializeComponent();
        }

        string helpInfo = "帮助:\n输入help查看帮助\n按下Del删除输入\n右键退出程序\n输入为空时按上下左右方向键试试\n试试输入boring\n更多命令查看按F1";

        bool dragEnable = true;
        bool dragging = false;
        int xOld;
        int yOld;

        private void Form1_Load(object sender, EventArgs e)
        {
            BackgroundImage = stand;
            FormBorderStyle = FormBorderStyle.None;
            BackgroundImageLayout = ImageLayout.Center;
            Color c = Color.Gray;
            BackColor = c;
            TransparencyKey = c;
            Width = BackgroundImage.Width;
            Height = BackgroundImage.Height + this.textBox1.Height + PADDING; 

            Bitmap bmp = new Bitmap(stand);
            bmp.RotateFlip(RotateFlipType.Rotate180FlipY);
            this.standRight = bmp;
            bmp = new Bitmap(left);
            bmp.RotateFlip(RotateFlipType.Rotate180FlipY);
            this.right = bmp;

            timer1.Start();
            timer2.Start();

            xOld = this.Bounds.X;
            yOld = this.Bounds.Y;

        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (rightKeyDown || leftKeyDown) return;
            if (dragEnable && e.Button == MouseButtons.Left)
            {
                // 保存当前鼠标的位置，可以用它来计算鼠标移动的距离
                xOld = MousePosition.X;
                yOld = MousePosition.Y;
                // 标识鼠标正在拖动窗体
                dragging = true;

                timer1.Stop();
            }
            else if (e.Button == MouseButtons.Right)
            {
                Application.Exit();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (dragEnable && dragging)
            {
                // 计算出鼠标在 X 和 Y 坐标方向上移动的距离
                int dx = MousePosition.X - xOld;
                int dy = MousePosition.Y - yOld;

                if (Math.Abs(dy) >= 10)
                {
                    BackgroundImage = fly;
                }
                else if (dx >= 5)
                {
                    BackgroundImage = right;
                    facingLeft = false;
                }
                else if (dx <= -5)
                {
                    BackgroundImage = left;
                    facingLeft = true;
                }

                // 根据上面的结果计算出窗体偏移后的位置
                Point point = this.Location;
                point.Offset(dx, dy);

                // 设置上面的偏移位置为窗体的位置
                this.Location = point;

                // 保存当前鼠标的位置，用于下一个循环的计算
                xOld = MousePosition.X;
                yOld = MousePosition.Y;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (dragEnable && e.Button == MouseButtons.Left)
                dragging = false;
            if (facingLeft)
            {
                BackgroundImage = stand;
            }
            else
            {
                BackgroundImage = standRight;
            }
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.BackgroundImage = sleep;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {  
                string txt = this.textBox1.Text;
                if (txt.ToLower().Trim() == "help")
                {
                    BackgroundImage = facingLeft?stand : standRight;
                    toolTip1.Show(
                        helpInfo, 
                        this, 
                        new Point(this.Width,0),
                        8000);
                }
                else if (txt.ToLower().Trim() == "boring")
                {
                    this.boring = true;
                    toolTip1.Show(
                        "无聊~~~~~",
                        this,
                        new Point(this.Width, 0),
                        3000);
                }
                else if (txt.Trim().ToLower().StartsWith("open") || txt.Trim().StartsWith("打开"))
                {
                    int idx = txt.IndexOf(" ");
                    if (idx >= (txt.Length - 1) || idx <= 0) return;
                    else
                    {
                        string res=Utils.OpenProcess(txt.Substring(idx).Trim());
                        toolTip1.Show(
                        res,
                        this,
                        new Point(this.Width, 0),
                        3000);
                    }
                }
                else if (txt.Trim().ToLower().StartsWith("date") 
                    || txt.Trim().StartsWith("time")
                    || txt.Trim().StartsWith("时间")
                    || txt.Trim().StartsWith("日期"))
                {
                    string str = Utils.NowDate();
                    toolTip1.Show(
                        str,
                        this,
                        new Point(this.Width, 0),
                        3000);
                }
                else if (txt.Trim().ToLower().StartsWith("cal"))
                {
                    int idx = txt.IndexOf(" ");
                    if (idx >= (txt.Length - 1) || idx <= 0) return;
                    else
                    {
                        string res = Utils.Cal(txt.Substring(idx).Trim());
                        toolTip1.Show(
                        res,
                        this,
                        new Point(this.Width, 0),
                        3000);
                    }
                }
                else
                {
                    string res = Utils.FindAnswer(txt);
                    int min = 3000;
                    int duration = res.Length*200;
                    if (duration < min) duration = min;
                    toolTip1.Show(
                    res,
                    this,
                    new Point(this.Width, 0),
                    duration);
                }
            }
            else if (e.KeyCode == Keys.Delete)
            {
                this.textBox1.Text = "";
            }
            else if (e.KeyCode == Keys.F1)
            {
                Utils.OpenProcess(AppDomain.CurrentDomain.BaseDirectory+"/data/help.txt");
            }
            Form1_KeyDown(sender,e);
        }

        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        bool rightKeyDown = false;
        bool leftKeyDown = false;
        bool upKeyDown = false;
        bool downKeyDOwn = false;
        bool boring = false;

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (boring) return;
            switch (e.KeyCode)
            {
                case Keys.Right:
                    if (textBox1.SelectionStart != textBox1.Text.Length) break;
                    this.rightKeyDown = true;
                    facingLeft = false;
                    break;
                case Keys.Left:
                    if (textBox1.SelectionStart != 0) break;
                    this.leftKeyDown = true;
                    facingLeft = true;
                    break;
                case Keys.Up:
                    this.upKeyDown = true;
                    break;
                case Keys.Down:
                    this.downKeyDOwn = true;
                    break;
                default:
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Right:
                    this.rightKeyDown = false;
                    BackgroundImage = standRight;
                    break;
                case Keys.Left:
                    this.leftKeyDown = false;
                    BackgroundImage = stand;
                    break;
                case Keys.Up:
                    this.upKeyDown = false;
                    BackgroundImage = facingLeft ? stand : standRight;
                    break;
                case Keys.Down:
                    this.downKeyDOwn = false;
                    BackgroundImage = facingLeft ? stand : standRight;
                    break;
                default:
                    break;
            }
        }

        private int loop = 1;
        private int nowLoop = 0;

        private void timer2_Tick(object sender, EventArgs e)
        {
            Point p = new Point(Location.X, Location.Y);
            int step = 8;

            if (boring)
            {
                if (nowLoop > loop)
                {
                    boring = false;
                    nowLoop = 0;
                    BackgroundImage = stand;
                    return;
                }
                BackgroundImage = bordAni[nowFrame];
                nowFrame = (nowFrame + 1) % bordAni.Length;
                if (nowFrame == 0) nowLoop++;
            }
            else if (upKeyDown)
            {
                if (!facingLeft)
                {
                    Bitmap bmp = new Bitmap(fly);
                    bmp.RotateFlip(RotateFlipType.Rotate180FlipY);
                    BackgroundImage = bmp;
                }
                else
                {
                    BackgroundImage = fly;
                }
                if (p.Y >= step)
                {
                    p.Y -= step;
                    this.Location = p;
                }
            }
            else if (downKeyDOwn)
            {
                if (!facingLeft)
                {
                    Bitmap bmp = new Bitmap(fall);
                    bmp.RotateFlip(RotateFlipType.Rotate180FlipY);
                    BackgroundImage = bmp;
                }
                else
                {
                    BackgroundImage = fall;
                }
                if (p.Y <= Screen.PrimaryScreen.Bounds.Height - step - this.Height)
                {
                    p.Y += step;
                    this.Location = p;
                }
            }
            else if (leftKeyDown)
            {
                BackgroundImage = walkAni[nowFrame];
                nowFrame = (nowFrame + 1) % walkAni.Length;
                if (p.X >= step)
                {
                    p.X -= step;
                    this.Location = p;
                }
            }
            else if (rightKeyDown)
            {
                Bitmap bmp = new Bitmap(walkAni[nowFrame]);
                bmp.RotateFlip(RotateFlipType.Rotate180FlipY);
                BackgroundImage = bmp;
                nowFrame = (nowFrame + 1) % walkAni.Length;
                if (p.X <= Screen.PrimaryScreen.Bounds.Width - step - this.Width)
                {
                    p.X += step;
                    this.Location = p;
                }
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            toolTip1.Show(
                helpInfo,
                this,
                new Point(this.Width, 0),
                8000);
        }

    }
} 
