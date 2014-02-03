using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PiCoder
{
    public partial class Form1 : Form
    {

        private byte[] buff;
        private string strin;
        private PiCoder pdec;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool success = true;
            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.Load(openFileDialog1.FileName);
                }
            }
            catch
            {
                success = false;
            }
            if (success)
            {
                PiCoder enc = new PiCoder(pictureBox1.Width, pictureBox1.Height);


                FileStream f1 = new FileStream("./output.dat", FileMode.Create);



                string rrr = enc.Encode(pictureBox1.Image as Bitmap);
                int length = rrr.Length;

                byte[] buf = new byte[length];
                char[] str = rrr.ToCharArray(0, length);

                for (int i = 0; i < length; i++)
                    buf[i] = (byte)str[i];

                f1.Write(buf, 0, length);

                f1.Flush();
                f1.Close();
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            tabControl1.Size = Size;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            FileStream f1 = new FileStream("./output.dat", FileMode.Open);

            int length = (int)numericUpDown1.Value * (int)numericUpDown2.Value * 4;


            byte[] buf = new byte[length];
            char[] str = new char[length];

            f1.Read(buf, 0, length);

            f1.Close();

            for (int i = 0; i < length; i++)
                str[i] = (char)buf[i];

            string input = new string(str);

            PiCoder dec = new PiCoder((int)numericUpDown1.Value, (int)numericUpDown2.Value);
            pictureBox2.Width = (int)numericUpDown1.Value;
            pictureBox2.Height = (int)numericUpDown2.Value;

            pictureBox2.Image = dec.Decode(input);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FileStream f1 = new FileStream("./output.dat", FileMode.Open);

            int length = (int)numericUpDown3.Value * (int)numericUpDown4.Value * 4;


            buff = new byte[length];
            char[] str = new char[length];

            f1.Read(buff, 0, length);

            f1.Close();

            for (int i = 0; i < length; i++)
                str[i] = (char)buff[i];

            string input = new string(str);

            strin = input;

            pdec = new PiCoder((int)numericUpDown3.Value, (int)numericUpDown4.Value);
            pictureBox3.Width = (int)numericUpDown3.Value;
            pictureBox3.Height = (int)numericUpDown4.Value;

            progressBar1.Value = 0;
            progressBar1.Maximum = (int)numericUpDown3.Value * (int)numericUpDown4.Value;

            timer1.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // for (int i = 0; i < 10000; i++)
            if (strin.Length > 0)
            {
                pictureBox3.Image = pdec.DecodePartial(strin.Substring(0, 4));
                strin = strin.Substring(4);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int k = trackBar1.Value;
            if (strin.Length > 0)
            {
                if (k < strin.Length)
                {
                    pictureBox3.Image = pdec.DecodePartial(strin.Substring(0, k));
                    strin = strin.Substring(k);
                }
                else
                {
                    pictureBox3.Image = pdec.DecodePartial(strin);
                    strin = "";
                }

                progressBar1.Value = progressBar1.Maximum - (int)(strin.Length / 4);
            }


            else timer1.Enabled = false;
        }

    }
}
