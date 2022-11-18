using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using System.Windows.Forms;
using NNLibrary;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        Matrix mres = new Matrix(1, 10);
        Matrix matrix = new Matrix(28, 28);
        Matrix input = new Matrix(1, 1);
        Matrix output = new Matrix(1, 1);
        static int[] config =  { 784, 100, 6 };
        Network network = new Network(config, 0.01);
        Bitmap bitmap/*, bitmap2*/;
        Graphics graphics/*, graphics2*/;
        Pen pen;
        int x_previous = 0, y_previous = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            input = new Matrix(1, 1);
            output = new Matrix(1, 1);
            matrix = new Matrix(28, 28);
            var oldImage = pictureBox1.Image;
            if (oldImage != null) 
                ((IDisposable)oldImage).Dispose();
            bitmap = new Bitmap(280, 280);
            pictureBox1.Image = bitmap;
            graphics = Graphics.FromImage(bitmap);
        }

        public Form1()
            {
            bitmap = new Bitmap(280, 280); 
            graphics = Graphics.FromImage(bitmap); 
            pen = new Pen(Color.Black, 20f);
            //bitmap2 = new Bitmap(280, 280);
            //graphics2 = Graphics.FromImage(bitmap2);
            pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            InitializeComponent();
            }
        
        private void button2_Click(object sender, EventArgs e)
        {
            label1.Text = "Teaching process..."; 
            network.train(@"training_012345.csv");
            label1.Text = "End of teaching!";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            network.Upload(@"trained.csv");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            mres.EditElement(0, Convert.ToInt32(textBox1.Text), 1);
            mres = new Matrix(1, 6);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            using (StreamWriter strwr = new StreamWriter(@"saved_number.csv", false))
            {

                    for (int j = 0; j < matrix.Row; j++)
                    {
                        for (int k = 0; k < matrix.Column; k++)
                        {
                            strwr.Write("{0}:", matrix.GetElement(j, k));
                        }
                   
                }
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            { 
                //graphics.DrawLine(pen, x_previous, y_previous, e.X, e.Y);
                //graphics2.DrawRectangle(pen, e.X/10*10, e.Y/10*10, 10, 10);
                if (e.X < 280 && e.Y < 280 && e.X > 0 && e.Y > 0)
                {
                    matrix.EditElement(e.X / 10, e.Y / 10, 255);
                    //SolidBrush mySolidBrush = new SolidBrush(Color.Black);
                    //graphics.FillRectangle(mySolidBrush, e.X, e.Y, 10, 10);
                    label2.Text = e.X/10 + "; " + e.Y/10;
                    for (int i = -3; i <= 3; i++)
                    {
                        if (e.X / 10 < 28 && e.Y / 10 + i < 28 && e.X / 10 > 0 && e.Y / 10 + i > 0)
                        {
                            if (matrix.GetElement(e.X / 10, e.Y / 10 + i) < 255 / (Math.Abs(i) + 1))
                            {
                                matrix.EditElement(e.X / 10, e.Y / 10 + i, 255 / (Math.Abs(i) + 1));
                                SolidBrush mySolidBrush = new SolidBrush(Color.FromArgb(255 - 255 / (Math.Abs(i) + 1), 255 - 255 / (Math.Abs(i) + 1), 255 - 255 / (Math.Abs(i) + 1)));
                                graphics.FillRectangle(mySolidBrush, e.X, e.Y+10*i, 10, 10);
                            }
                        }
                        if (e.X / 10 + i < 28 && e.Y / 10 < 28 && e.X + i / 10 > 0 && e.Y / 10 > 0)
                        {
                            if (matrix.GetElement(e.X / 10 + i, e.Y / 10) < 255 / (Math.Abs(i) + 1))
                            { 
                                matrix.EditElement(e.X / 10 + i, e.Y / 10, 255 / (Math.Abs(i) + 1));
                            SolidBrush mySolidBrush = new SolidBrush(Color.FromArgb(255 - 255 / (Math.Abs(i) + 1), 255 - 255 / (Math.Abs(i) + 1), 255 - 255 / (Math.Abs(i) + 1)));
                            graphics.FillRectangle(mySolidBrush, e.X + 10 * i, e.Y, 10, 10);
                        }
                    }
                    }
                    SolidBrush mySolidBrus = new SolidBrush(Color.Black);
                    graphics.FillRectangle(mySolidBrus, e.X, e.Y, 10, 10);
                }
            }
            x_previous = e.X; y_previous = e.Y;
            pictureBox1.Image = bitmap;
            //pictureBox2.Image = bitmap2;
            ///////////////////////
            ///
            input = matrix.Transpose();
            input = input.ToVectorOfRows_horizontal();
            output = Matrix.softmax(network.forward(input));
            //double buf = output.GetElement(0,0);
            //for (int i = 1; i < 10; i++)
            //{
            //    if (buf > output.GetElement(0, i))
            //        buf = output.GetElement(0, i);
            //}
            //for (int i = 0; i < 10; i++)
            //{
            //    output.EditElement(0, i, output.GetElement(0, i) - buf);
            //}
            //Matrix.softmax(output);
            chart1.Series[0].Points.Clear();
            for(int i = 0;i<6;i++)
                chart1.Series[0].Points.AddXY(i,output.GetElement(0,i));
            ///////////////////////
            ///
            label1.Text = "4 - " + output.GetElement(0, 4) + "%";
        }

    }
    }
