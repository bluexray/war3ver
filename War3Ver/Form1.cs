using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace War3Ver
{
    public partial class Form1 : Form
    {
        public  string  iniFilePath=null;

        public string WAR = "war3";
      
        public Form1()
        {
            InitializeComponent();
        }


        
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }


        //自动创建按钮
        public void CreateVerButton()
        {

            List<string> keys = new List<string>();

            if (File.Exists(this.iniFilePath))
            {
                keys = IniHelper.GetKeys(WAR, this.iniFilePath);
            }


            int temp = 0;

            for (int i = 0; i < keys.Count; i++)
            {
                int n = i % 4;
                if (n==0&&i!=0)
                {
                    temp++;    //这个条件语句的意思是：如果控件的数量正好是4的倍数的话那么tmp+1  这个是用于控制y轴的
                }
                int x = n * 120, y = 70 * temp;

                //创建按钮

                Button btn = new Button();

                btn.Location = new System.Drawing.Point(x + 0, y + 25);

                btn.Name = keys[i].ToString();

                btn.Size = new System.Drawing.Size(102, 48);

                btn.TabIndex = 18;

                btn.Text = keys[i].ToString();

                btn.UseVisualStyleBackColor = true;

                btn.Click += new EventHandler(btn_Click);//button的单击事件

                this.groupBox1.Controls.Add(btn);

                this.label1.Text = "war3....加载配置成功！本次共加载"+ keys.Count + "版本!";
            }

        }

        private void btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            string nameid = btn.Name;

            FileZipOpr zip = new FileZipOpr();

            var dir = System.AppDomain.CurrentDomain.BaseDirectory;

            if (File.Exists(this.iniFilePath))
            {
                var value = IniHelper.GetValue(WAR, nameid, this.iniFilePath);
                
                var p = dir+"\\ver\\"+value;
                try
                {
                    zip.UnZipFile(p, dir,true);
                }
                catch (Exception ex)
                {

                   MessageBox.Show(ex.Message);
                }

                this.label1.Text = "版本成功更换为：" + nameid + "，请进游戏体验！";

                MessageBox.Show("版本成功更换为："+nameid);
            }
            else
            {
                MessageBox.Show("文件加载失败，请确认是否存在此文件：" + this.iniFilePath);
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
                string directory = System.AppDomain.CurrentDomain.BaseDirectory+"\\ver";
                this.iniFilePath = Path.Combine(directory, "war3version.ini");
                //ReadIniFile();
                //ReadIniKeys();
            CreateVerButton();
        }


        public void ReadIniKeys()
        {
            if (File.Exists(this.iniFilePath))
            {
                //this.listBox1.DataSource= IniHelper.GetKeys("war3",this.iniFilePath);
                MessageBox.Show("读取文档成功");
            }
            else
            {
                MessageBox.Show("文件加载失败，请确认是否存在此文件：" + this.iniFilePath);
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {

            var dir = System.AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(dir, "War3.exe");

            try
            {
                System.Diagnostics.Process.Start(path);
            }
            catch (Exception)
            {

                MessageBox.Show("未找到WAR3.exe");
            }

        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.Owner = this;
            about.Show();
        }
    }
}
