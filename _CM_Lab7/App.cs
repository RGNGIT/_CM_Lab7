using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace _CM_Lab7
{
    public partial class App : Form
    {
        public App()
        {
            InitializeComponent();
            chart.Series.Clear();
        }

        List<double> x = new List<double>()
        {
            0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0, 1.1
            /*1.5, 1.7, 1.71, 1.8, 2, 2.1, 2.2, 2.3, 2.35, 2.4*/
        };

        List<double> y = new List<double>() 
        {
            18.422, 12.844, 9.659, 7.394, 5.670, 4.303, 3.196, 2.297, 1.572, 1.000, 0.569
            /*7.76, 11.30, 11.50, 13.41, 18.40, 21.30, 24.50, 28.00, 29.85, 31.80*/
        };

        List<double> lnx = new List<double>();

        List<double> lny = new List<double>();

        IEnumerable<double> Squared(List<double> list)
        {
            for(int i = 0; i < list.Count; i++)
            {
                yield return Math.Pow(list[i], 2);
            }
        }

        IEnumerable<double> Mul(List<double> list1, List<double> list2)
        {
            for (int i = 0; i < list1.Count; i++)
            {
                yield return list1[i] * list2[i];
            }
        }

        List<double> Cramer(List<List<double>> Mtrx, List<double> Equal)
        {
            double FindDet(List<List<double>> Mtr) 
            => Mtr[0][0] * Mtr[1][1] - Mtr[0][1] * Mtr[1][0];
            if (FindDet(Mtrx) != 0) // Проверка на совместимость с методом
            {
                return new List<double>
                {
                    FindDet(new List<List<double>> // x
                    {
                        new List<double> { Equal[0],  Mtrx[0][1] },
                        new List<double> { Equal[1],  Mtrx[1][1] }
                    }) / FindDet(Mtrx),
                    FindDet(new List<List<double>> // y
                    {
                        new List<double> { Mtrx[0][0],  Equal[0] },
                        new List<double> { Mtrx[1][0],  Equal[1] }
                    }) / FindDet(Mtrx)
                };
            } 
            else
            {
                listBox.Items.Add("Система несовместима с методом Крамера!");
                return null;
            }
        }

        double PreferedValue;
        string PreferedString;

        void BuildLinear(double k, double b)
        {
            listBox.Items.Add("/// Линейный ///");
            chart.Series.Add(new Series("Линейная") { ChartType = SeriesChartType.Line });
            listBox.Items.Add($"Линейное уравнение: y = {k}x + ({b})");
            List<double> Array = new List<double>();
            for(int i = 0; i < x.Count; i++)
            {
                chart.Series["Линейная"].Points.AddXY(x[i], k * x[i] + b);
                Array.Add(k * x[i] + b);
            }
            PreferedValue = ToShitDeltaY(Array);
            PreferedString = "линейный";
            listBox.Items.Add($"Зигма: {ToShitDeltaY(Array)}");
        }

        double ToShitDeltaY(List<double> Array)
        {
            double min = Math.Abs(Array[1] - Array[0]);
            for(int i = 1; i < Array.Count; i++)
            {
                if(min > Math.Abs(Array[i] - Array[i - 1]))
                {
                    min = Math.Abs(Array[i] - Array[i - 1]);
                }
            }
            return min;
        }

        void BuildLogs()
        {
            List<double> Sum = new List<double>() { 0, 0, 0, 0 };
            dataGridView.Rows.Add("ln(x)");
            dataGridView.Rows.Add("ln(y)");
            dataGridView.Rows.Add("Sqr(ln(x))");
            dataGridView.Rows.Add("ln(x)ln(y)");
            for (int i = 0; i < x.Count; i++)
            {
                dataGridView.Rows[4].Cells[i + 1].Value = Math.Log(x[i]);
                lnx.Add(Math.Log(x[i]));
                Sum[0] += Math.Log(x[i]);
                dataGridView.Rows[5].Cells[i + 1].Value = Math.Log(y[i]);
                lny.Add(Math.Log(y[i]));
                Sum[1] += Math.Log(y[i]);
                dataGridView.Rows[6].Cells[i + 1].Value = Squared(lnx).ToArray()[i];
                Sum[2] += Squared(lnx).ToArray()[i];
                dataGridView.Rows[7].Cells[i + 1].Value = Mul(lnx, lny).ToArray()[i];
                Sum[3] += Mul(lnx, lny).ToArray()[i];
            }
            dataGridView.Rows[4].Cells[12].Value = Sum[0];
            dataGridView.Rows[5].Cells[12].Value = Sum[1];
            dataGridView.Rows[6].Cells[12].Value = Sum[2];
            dataGridView.Rows[7].Cells[12].Value = Sum[3];
            BuildDegree(Sum[0], Sum[1], Sum[2], Sum[3]);
        }


        void BuildDegree(double sumx, double sumy, double sumsqx, double sumyx)
        {
            listBox.Items.Add("/// Степенной ///");
            chart.Series.Add(new Series("Степенная") { ChartType = SeriesChartType.Line });
            listBox.Items.Add("Решение методом Крамера...");
            double m = Cramer(new List<List<double>> { new List<double> { sumsqx, sumx }, new List<double> { sumx, x.Count } }, new List<double> { sumyx, sumy })[0];
            double c = Math.Exp(Cramer(new List<List<double>> { new List<double> { sumsqx, sumx }, new List<double> { sumx, x.Count } }, new List<double> { sumyx, sumy })[1]);
            List<double> Array = new List<double>();
            listBox.Items.Add($"Значение c: {c}");
            listBox.Items.Add($"Значение m: {m}");
            listBox.Items.Add($"Степенное уравнение: y = {c}x^{m}");
            for(int i = 0; i < x.Count; i++)
            {
                chart.Series["Степенная"].Points.AddXY(x[i], c * Math.Pow(x[i], m));
                Array.Add(c * Math.Pow(x[i], m));
            }
            if(PreferedValue > ToShitDeltaY(Array))
            {
                PreferedString = "степенной";
            }
            listBox.Items.Add($"Зигма: {ToShitDeltaY(Array)}");
        }

        private void Start_Click(object sender, EventArgs e)
        {
            chart.Series.Add(new Series("Исходная") { ChartType = SeriesChartType.Line });
            chart.Series["Исходная"].Points.DataBindXY(x, y);
            List<double> Sum = new List<double>() { 0, 0, 0, 0 };
            dataGridView.Rows.Add("x");
            dataGridView.Rows.Add("y");
            dataGridView.Rows.Add("Sqr(x)");
            dataGridView.Rows.Add("xy");
            for(int i = 0; i < x.Count; i++)
            {
                dataGridView.Rows[0].Cells[i + 1].Value = x[i];
                Sum[0] += x[i];
                dataGridView.Rows[1].Cells[i + 1].Value = y[i];
                Sum[1] += y[i];
                dataGridView.Rows[2].Cells[i + 1].Value = Squared(x).ToArray()[i];
                Sum[2] += Squared(x).ToArray()[i];
                dataGridView.Rows[3].Cells[i + 1].Value = Mul(x, y).ToArray()[i];
                Sum[3] += Mul(x, y).ToArray()[i];
            }
            dataGridView.Rows[0].Cells[12].Value = Sum[0];
            dataGridView.Rows[1].Cells[12].Value = Sum[1];
            dataGridView.Rows[2].Cells[12].Value = Sum[2];
            dataGridView.Rows[3].Cells[12].Value = Sum[3];
            label.Text = $"{Sum[2]}k + {Sum[0]}b = {Sum[3]}\n{Sum[0]}k + {x.Count}b = {Sum[1]}";
            listBox.Items.Add("Решение методом Крамера...");
            double k = Cramer(new List<List<double>> { new List<double> { Sum[2], Sum[0] }, new List<double> { Sum[0], x.Count } }, new List<double> { Sum[3], Sum[1] })[0];
            double b = Cramer(new List<List<double>> { new List<double> { Sum[2], Sum[0] }, new List<double> { Sum[0], x.Count } }, new List<double> { Sum[3], Sum[1] })[1];
            listBox.Items.Add($"Значение k: {k}");
            listBox.Items.Add($"Значение b: {b}");
            BuildLinear(k, b);
            BuildLogs();
            listBox.Items.Add($"По результатам зигмы побеждает {PreferedString} метод!");
            Start.Visible = false;
        }
    }
}
