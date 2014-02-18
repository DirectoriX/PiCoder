using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PiCoder
{
    class PiCoder
    {
        private int Width;
        private int Height;

        private Bitmap bmp;
        private string str;
        private int k, i, j;
        private bool[,] dim;
        private bool started;

        private string Clr2Str(Color clr)
        {
            return "" + (char)clr.R + (char)clr.G + (char)clr.B;
        }

        private Color Str2Clr(string str)
        {
            if (str.Length > 0)
                return Color.FromArgb((int)str[0], (int)str[1], (int)str[2]/*, (int)str[3]*/);
            else return Color.White;
        }

        private void rect(Color c, int x, int y, int w, int h)
        {
            for (int a = x; a < x + w; a++)
                if (a < this.Width)
                    for (int b = y; b < y + h; b++)
                        if (b < this.Height)
                            bmp.SetPixel(a, b, c);
                        else break;
                else break;
        }

        public PiCoder(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;

            bmp = null;
            str = "";
            i = 1;
            j = 1;
            dim = null;
            started = false;
        }

        public string Encode(Bitmap pic)
        {
            bool[,] dim = new bool[this.Width, this.Height];

            for (int i = 0; i < this.Width; i++)
                for (int j = 0; j < this.Height; j++)
                    dim[i, j] = false;

            int k = (int)Math.Floor(Math.Max(Math.Log(this.Width - 1, 2), Math.Log(this.Height - 1, 2)));

            string res = "";

            for (; k > 1; k--)
                for (int i = 1; i * Math.Pow(2, k - 1) < this.Width; i++)
                    for (int j = 1; j * Math.Pow(2, k - 1) < this.Height; j++)
                    {
                        int x = (int)(i * Math.Pow(2, k - 1));
                        int y = (int)(j * Math.Pow(2, k - 1));
                        if (dim[x, y]) continue;
                        res += Clr2Str(pic.GetPixel(x, y));
                        dim[x, y] = true;
                    }

            for (int i = 0; i < this.Width; i++)
                for (int j = 0; j < this.Height; j++)
                {
                    int x = (int)(i);
                    int y = (int)(j);
                    if (dim[x, y]) continue;
                    res += Clr2Str(pic.GetPixel(x, y));
                    dim[x, y] = true;
                }

            string err = "";
            for (int i = 0; i < 16; i++)
                for (int j = 0; j < 16; j++)
                    if (!dim[i, j]) err += (i.ToString() + ":" + j.ToString() + " ");

            return res + err;
        }

        public Bitmap Decode(string str)
        {
            bool[,] dim = new bool[this.Width, this.Height];

            for (int i = 0; i < this.Width; i++)
                for (int j = 0; j < this.Height; j++)
                    dim[i, j] = false;

            Bitmap res = new Bitmap(this.Width, this.Height);
            int k = (int)Math.Floor(Math.Max(Math.Log(this.Width, 2), Math.Log(this.Height, 2)));

            for (; k > 1; k--)
                for (int i = 1; i * Math.Pow(2, k - 1) < this.Width; i++)
                    for (int j = 1; j * Math.Pow(2, k - 1) < this.Height; j++)
                    {
                        int x = (int)(i * Math.Pow(2, k - 1));
                        int y = (int)(j * Math.Pow(2, k - 1));
                        if (dim[x, y]) continue;
                        res.SetPixel(x, y, Str2Clr(str));
                        str = str.Substring(3);
                        dim[x, y] = true;
                    }

            for (int i = 0; i < this.Width; i++)
                for (int j = 0; j < this.Height; j++)
                {
                    int x = (int)(i);
                    int y = (int)(j);
                    if (dim[x, y]) continue;
                    res.SetPixel(x, y, Str2Clr(str));
                    str = str.Substring(3);
                    dim[x, y] = true;
                }


            return res;
        }

        public Bitmap DecodePartial(string input)
        {
            bool continued = true;

            if (!started)
            {
                dim = new bool[this.Width, this.Height];

                for (int i5 = 0; i5 < this.Width; i5++)
                    for (int j5 = 0; j5 < this.Height; j5++)
                        dim[i5, j5] = false;

                this.i = 1;
                this.j = 1;

                bmp = new Bitmap(this.Width, this.Height);
                this.k = (int)Math.Floor(Math.Max(Math.Log(this.Width, 2), Math.Log(this.Height, 2)));
                started = true;
            }

            this.str += input;

            int length = this.str.Length / 3;

            if (this.k > 1)
            {
                int k1 = (int)Math.Floor(Math.Max(Math.Log(this.Width, 2), Math.Log(this.Height, 2)));
                for (; k1 > 1; k1--) if (length > 0)
                    {
                        if (continued)
                        {
                            k1 = this.k;
                        }
                        for (int i1 = 1; i1 * Math.Pow(2, k1 - 1) < this.Width; i1++)
                            if (length > 0)
                                for (int j1 = 1; j1 * Math.Pow(2, k1 - 1) < this.Height; j1++)
                                {
                                    if (continued)
                                    {
                                        i1 = this.i;
                                        j1 = this.j;
                                        continued = false;
                                    }

                                    this.i = i1;
                                    this.j = j1;
                                    this.k = k1;

                                    if (length > 0)
                                    {

                                        int x = (int)(i1 * Math.Pow(2, k1 - 1));
                                        int y = (int)(j1 * Math.Pow(2, k1 - 1));
                                        if (dim[x, y]) continue;
                                        int size = (int)(Math.Pow(2, k1 - 1));
                                        rect(Str2Clr(str), x, y, size, size);
                                        this.str = this.str.Substring(3);
                                        dim[x, y] = true;
                                        length--;



                                    }
                                    else break;
                                }
                            else break;

                        if (length > 0)
                        {
                            this.i = 0;
                            this.j = 0;
                            this.k = 0;
                        }
                    }
                    else break;
            }

            for (int i2 = 0; i2 < this.Width; i2++)
                if (length > 0)
                    for (int j2 = 0; j2 < this.Height; j2++)
                    {
                        if (continued)
                        {
                            i2 = this.i;
                            j2 = this.j;
                            continued = false;
                        }

                        this.i = i2;
                        this.j = j2;

                        if (length > 0)
                        {
                            int x = (int)(i2);
                            int y = (int)(j2);
                            if (dim[x, y]) continue;
                            bmp.SetPixel(x, y, Str2Clr(str));
                            str = str.Substring(3);
                            dim[x, y] = true;
                            length--;


                        }
                        else break;
                    }
                else break;

            return bmp;
        }
    }
}
