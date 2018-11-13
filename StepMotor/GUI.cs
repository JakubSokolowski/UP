using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StepMotor
{
    public partial class GUI : Form
    {
        public GUI()
        {
            InitializeComponent();
        }
        private MotorDriver driver = new MotorDriver();

        private void Connect_Click(object sender, EventArgs e)
        {
            this.richTextBox1.Text = "Test";
            this.richTextBox1.Text =  driver.Connect();

        }

        private void prawoKat_Click(object sender, EventArgs e)
        {

        }

        private void LHS_Click(object sender, EventArgs e)
        {
            driver.RotationDirection = Direction.Left;
            driver.Mode = ControlMode.HalfStep;
            driver.RotationValue = 1;
            driver.Rotate();
        }
        private void RHS_Click(object sender, EventArgs e)
        {
            driver.RotationDirection = Direction.Right;
            driver.Mode = ControlMode.HalfStep;
            driver.RotationValue = 1;
            driver.Rotate();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            driver.Disconnect();
            richTextBox1.Text = "Disconnected";
        }

       

        private void LFS_Click(object sender, EventArgs e)
        {
            driver.RotationDirection = Direction.Left;
            driver.Mode = ControlMode.FullStep;
            driver.RotationValue = 1;
            driver.Rotate();
        }

        private void RFS_Click(object sender, EventArgs e)
        {
            driver.RotationDirection = Direction.Right;
            driver.Mode = ControlMode.FullStep;
            driver.RotationValue = 1;
            driver.Rotate();
        }

        private void GUI_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
           
            try
            {
                driver.RotationDirection = Direction.Left;
                driver.Mode = ControlMode.HalfStep;
                driver.Interval = Int32.Parse(kat.Text);
                int steps = Int32.Parse(kroki.Text);
                driver.RotationValue = steps;
                driver.Rotate();
              
            }
            catch(Exception ex)
            {
                this.richTextBox1.Text = ex.Message;
            }
        
      
        }

        private void kat_TextChanged(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                driver.RotationDirection = Direction.Right;
                driver.Mode = ControlMode.HalfStep;
                driver.Interval = Int32.Parse(kat.Text);
                int steps = Int32.Parse(kroki.Text);
                driver.RotationValue = steps;
                driver.Rotate();

            }
            catch (Exception ex)
            {
                this.richTextBox1.Text = ex.Message;
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                driver.RotationDirection = Direction.Left;
                driver.Mode = ControlMode.FullStep;
                driver.Interval = Int32.Parse(kat.Text);
                int steps = Int32.Parse(kroki.Text);
                driver.RotationValue = steps;
                driver.Rotate();

            }
            catch (Exception ex)
            {
                this.richTextBox1.Text = ex.Message;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                driver.RotationDirection = Direction.Right;
                driver.Mode = ControlMode.FullStep;
                driver.Interval = Int32.Parse(kat.Text);
                int steps = Int32.Parse(kroki.Text);
                driver.RotationValue = steps;
                driver.Rotate();

            }
            catch (Exception ex)
            {
                this.richTextBox1.Text = ex.Message;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            driver.RotationDirection = Direction.Left;
            driver.Mode = ControlMode.WaveStep;
            driver.RotationValue = 1;
            driver.Rotate();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            driver.RotationDirection = Direction.Right;
            driver.Mode = ControlMode.WaveStep;
            driver.RotationValue = 1;
            driver.Rotate();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            try
            {
                driver.RotationDirection = Direction.Left;
                driver.Mode = ControlMode.WaveStep;
                driver.Interval = Int32.Parse(kat.Text);
                int steps = Int32.Parse(kroki.Text);
                driver.RotationValue = steps;
                driver.Rotate();

            }
            catch (Exception ex)
            {
                this.richTextBox1.Text = ex.Message;
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {            
            try
            {
                driver.RotationDirection = Direction.Right;
                driver.Mode = ControlMode.WaveStep;
                driver.Interval = Int32.Parse(kat.Text);
                int steps = Int32.Parse(kroki.Text);
                driver.RotationValue = steps;
                driver.Rotate();

            }
            catch (Exception ex)
            {
                this.richTextBox1.Text = ex.Message;
            }
        }
    }
}
