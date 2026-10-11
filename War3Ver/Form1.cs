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
        public string iniFilePath = null;

        public string WAR = "war3";

        // Safe string builder from Unicode code points to avoid any file encoding issues
        private static string S(params int[] codes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (int c in codes)
            {
                sb.Append((char)c);
            }
            return sb.ToString();
        }

        // Pre-defined Chinese UI strings
        private readonly string TXT_TITLE = "War3 " + S(0x7248, 0x672C, 0x5207, 0x6362, 0x5668) + " Ver1.2";
        private readonly string TXT_GROUP = S(0x7248, 0x672C, 0x8F6C, 0x6362, 0x5668) + " (" + S(0x70B9, 0x51FB, 0x81EA, 0x52A8, 0x89E3, 0x9664, 0x5927, 0x5730, 0x56FE, 0x9650, 0x5236) + ")";
        private readonly string TXT_ABOUT = S(0x5173, 0x4E8E, 0x8F6F, 0x4EF6);
        private readonly string TXT_RUN = S(0x8FD0, 0x884C, 0x9B54, 0x517D);
        private readonly string TXT_LOADED_1 = "war3...." + S(0x52A0, 0x8F7D, 0x914D, 0x7F6E, 0x6210, 0x529F, 0xFF01, 0x672C, 0x6B21, 0x5171, 0x52A0, 0x8F7D) + " ";
        private readonly string TXT_LOADED_2 = " " + S(0x7248, 0x672C, 0xFF01);
        private readonly string TXT_SWITCHED = S(0x7248, 0x672C, 0x6210, 0x529F, 0x66F4, 0x6362, 0x4E3A, 0xFF1A);
        private readonly string TXT_PATCHED = S(0x5DF2, 0x81EA, 0x52A8, 0x89E3, 0x9664, 0x5730, 0x56FE, 0x5927, 0x5C0F, 0x9650, 0x5236) + " (Max: 64MB)";
        private readonly string TXT_NATIVE = S(0x5B98, 0x65B9, 0x539F, 0x751F, 0x652F, 0x6301, 0x5927, 0x5730, 0x56FE) + " (Max: 128MB)";
        private readonly string TXT_NO_WAR3 = S(0x672A, 0x627E, 0x5230) + " War3.exe";

        public Form1()
        {
            InitializeComponent();
            this.Text = TXT_TITLE;
            this.groupBox1.Text = TXT_GROUP;
            this.btnAbout.Text = TXT_ABOUT;
            this.btnRun.Text = TXT_RUN;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private bool IsNativeLargeMapVersion(string verName)
        {
            string v = verName.ToLower();
            return v.Contains("1.27") || v.Contains("1.28") || v.Contains("1.29") || v.Contains("1.30") || v.Contains("1.31") || v.Contains("1.32");
        }

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
                if (n == 0 && i != 0)
                {
                    temp++;
                }
                int x = n * 120, y = 70 * temp;

                string verName = keys[i].ToString();

                Button btn = new Button();
                btn.Location = new System.Drawing.Point(x + 15, y + 30);
                btn.Name = verName;
                btn.Size = new System.Drawing.Size(108, 48);
                btn.TabIndex = 18 + i;

                if (IsNativeLargeMapVersion(verName))
                {
                    btn.Text = verName + " (128M)";
                }
                else
                {
                    btn.Text = verName + " (64M)";
                }

                btn.UseVisualStyleBackColor = true;
                btn.Click += new EventHandler(btn_Click);

                this.groupBox1.Controls.Add(btn);
            }

            this.label1.Text = TXT_LOADED_1 + keys.Count + TXT_LOADED_2;
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
                var p = Path.Combine(dir, "ver", value);

                try
                {
                    zip.UnZipFile(p, dir, true);

                    if (IsNativeLargeMapVersion(nameid))
                    {
                        this.label1.Text = TXT_SWITCHED + nameid + " - " + TXT_NATIVE;
                        MessageBox.Show(TXT_SWITCHED + nameid + "\n\n" + TXT_NATIVE, "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        int patchCount = ApplyLargeMapPatch(dir);
                        if (patchCount > 0)
                        {
                            this.label1.Text = TXT_SWITCHED + nameid + " - " + TXT_PATCHED;
                            MessageBox.Show(TXT_SWITCHED + nameid + "\n\n" + TXT_PATCHED + " [Patched: " + patchCount + "]", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            this.label1.Text = TXT_SWITCHED + nameid + " (64M Ready)";
                            MessageBox.Show(TXT_SWITCHED + nameid + "\n\n" + TXT_PATCHED, "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Missing: " + this.iniFilePath);
            }
        }

        private int ApplyLargeMapPatch(string gameDir)
        {
            var gameDllPath = Path.Combine(gameDir, "Game.dll");
            if (!File.Exists(gameDllPath))
            {
                gameDllPath = Path.Combine(gameDir, "game.dll");
            }
            if (!File.Exists(gameDllPath))
            {
                return 0;
            }

            byte[] data = File.ReadAllBytes(gameDllPath);
            int patchCount = 0;

            for (int i = 3; i < data.Length - 8; i++)
            {
                if (data[i] == 0x00 && data[i + 1] == 0x00 && (data[i + 2] == 0x80 || data[i + 2] == 0x40) && data[i + 3] == 0x00)
                {
                    bool isCmpPrefix = (data[i - 1] == 0x3D) ||
                                       (data[i - 1] >= 0xF8 && data[i - 2] == 0x81) ||
                                       (data[i - 2] >= 0x78 && data[i - 2] <= 0x7F && data[i - 3] == 0x81);

                    byte nextByte = data[i + 4];
                    bool isJumpSuffix = (nextByte == 0x77 || nextByte == 0x7F || nextByte == 0x73 || nextByte == 0x76 || nextByte == 0x7E) ||
                                        (nextByte == 0x0F && (data[i + 5] >= 0x82 && data[i + 5] <= 0x8F));

                    bool isMov8M = (data[i + 2] == 0x80) && (
                                   (data[i - 1] >= 0xB8 && data[i - 1] <= 0xBF) ||
                                   (data[i - 2] == 0x45 && data[i - 3] == 0xC7));

                    if ((isCmpPrefix && isJumpSuffix) || isMov8M)
                    {
                        data[i + 2] = 0xFF;
                        data[i + 3] = 0x03;
                        patchCount++;
                    }
                }
            }

            if (patchCount > 0)
            {
                File.WriteAllBytes(gameDllPath, data);
            }

            return patchCount;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }
            catch { }

            string directory = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "ver");
            this.iniFilePath = Path.Combine(directory, "war3version.ini");
            CreateVerButton();
        }

        public void ReadIniKeys()
        {
            if (File.Exists(this.iniFilePath))
            {
                MessageBox.Show("OK");
            }
            else
            {
                MessageBox.Show("Missing: " + this.iniFilePath);
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
                MessageBox.Show(TXT_NO_WAR3);
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
