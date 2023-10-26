using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace OptovayaBazaMDK
{
    public partial class Form3 : Form
    {
        private bool drawingInProgress = false;
        private Timer drawTimer = new Timer();
        private int lineWidth = 0;
        private string q1 = "Васильев Василий Васильевич";
        private string q2 = "+7 951 1895 78 43";
        private string q3 = "г. Ижевскб, ул. Молодёжная, д 109Б, кв. 705";
        private string q4 = "2284 4352 4001 4824";
        private string q5 = "11/24";
        private string q6 = "666";

        public Form3()
        {
            InitializeComponent();
            p2.Hide();

            // Настройте таймер
            drawTimer.Interval = 10; // Интервал в миллисекундах (можно изменить)
            drawTimer.Tick += DrawTimer_Tick;
            PlaceholderAdder();
        }

        private void PlaceholderAdder()
        {
            tbFIO.AddPlaceholder(q1);
            tbNumber.AddPlaceholder(q2);
            tbAdress.AddPlaceholder(q3);
            textBox3.AddPlaceholder(q4);
            textBox2.AddPlaceholder(q5);
            textBox1.AddPlaceholder(q6);
        }
        public void SetListBoxResult(ListBox listBox)
        {
            foreach (var item in listBox.Items)
            {
                ListBoxResultForm3.Items.Add(item);
            }
            ListBoxResultForm3 = listBox;
        }

        public void SetLabel1Text(string text)
        {
            label1.Text = text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            p1.Hide();
            p2.Show();

            // Начните рисование при нажатии на кнопку
            drawingInProgress = true;
            lineWidth = 0;

            // Запустите таймер
            drawTimer.Start();

            // Перерисуйте панель
            panelCard.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (tbFIO.Text != q1 || tbNumber.Text != q2 || tbAdress.Text != q3 || textBox3.Text != q4 || textBox2.Text != q5 || textBox1.Text != q6)
            {
                if (checkBox1.Checked)
                {
                    Form4 form4 = new Form4();
                    form4.SetListBoxResult(ListBoxResultForm3);

                    form4.SetTextSum(label1.Text);
                    form4.SetTextFIO($"{tbFIO.Text}!");
                    form4.SetTextAdress(tbAdress.Text);
                    form4.SetTextNumber(tbNumber.Text);

                    form4.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Согласитель на обработку персональных данных");
                }
            }
            else
            {
                MessageBox.Show("Введите корректные значения");
            }
        }

        private void DrawTimer_Tick(object sender, EventArgs e)
        {
            if (drawingInProgress)
            {
                // Увеличивайте ширину линии на каждом такте таймера
                lineWidth += 1;

                // Перерисуйте панель
                panelCard.Invalidate();

                // Если достигнута желаемая ширина линии, завершите рисование
                if (lineWidth >= 4) // Измените 4 на желаемую ширину линии
                {
                    drawingInProgress = false;
                    drawTimer.Stop();
                }
            }
        }

        private void panelCard_Paint(object sender, PaintEventArgs e)
        {
            if (drawingInProgress)
            {
                // Определите цвет линии и толщину
                Color borderColor = Color.Purple;

                // Определите радиус закругленных углов
                int cornerRadius = 30;

                // Определите прямоугольник с учетом отступов
                Rectangle rect = new Rectangle(
                    panelCard.Padding.Left,
                    panelCard.Padding.Top,
                    panelCard.Width - panelCard.Padding.Horizontal,
                    panelCard.Height - panelCard.Padding.Vertical);

                // Получите объект Graphics для элемента panelCard
                Graphics g = e.Graphics;

                // Нарисуйте прямоугольник с закругленными углами
                using (Pen borderPen = new Pen(borderColor, lineWidth)) // Используйте текущую ширину линии
                using (GraphicsPath path = RoundedRectangle(rect, cornerRadius))
                {
                    g.DrawPath(borderPen, path);
                }
            }
            else
            {
                // Если рисование завершено, просто отобразите окончательный результат
                panelCard_PaintStatic(sender, e);
            }
        }

        private GraphicsPath RoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;

            Rectangle arc = new Rectangle(rect.Location, new Size(diameter, diameter));

            // Верхний левый угол
            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;

            // Верхний правый угол
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;

            // Нижний правый угол
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;

            // Нижний левый угол
            path.AddArc(arc, 90, 90);
            path.CloseFigure();

            return path;
        }

        private void panelCard_PaintStatic(object sender, PaintEventArgs e)
        {
                // Определите цвет линии и толщину
                Color borderColor = Color.Purple;

                // Определите радиус закругленных углов
                int cornerRadius = 30;

                // Определите прямоугольник с учетом отступов
                Rectangle rect = new Rectangle(
                    panelCard.Padding.Left,
                    panelCard.Padding.Top,
                    panelCard.Width - panelCard.Padding.Horizontal,
                    panelCard.Height - panelCard.Padding.Vertical);

                // Получите объект Graphics для элемента panelCard
                Graphics g = e.Graphics;

                // Нарисуйте прямоугольник с закругленными углами
                using (Pen borderPen = new Pen(borderColor, lineWidth)) // Используйте текущую ширину линии
                using (GraphicsPath path = RoundedRectangle(rect, cornerRadius))
                {
                    g.DrawPath(borderPen, path);
                }
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Если запись найдена, откройте Form2
            Form2 form2 = new Form2();
            form2.Show();
            this.Hide();
        }
    }
}
