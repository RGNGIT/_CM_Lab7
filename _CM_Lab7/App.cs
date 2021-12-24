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
            1.5, 1.7, 1.71, 1.8, 2, 2.1, 2.2, 2.3, 2.35, 2.4
        };

        List<double> y = new List<double>() 
        {
            7.76, 11.30, 11.50, 13.41, 18.40, 21.30, 24.50, 28.00, 29.85, 31.80
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
        List<double> LinearArray = new List<double>();
        List<double> DegreeArray = new List<double>();
        List<double> SquareArray = new List<double>();

        void BuildLinear(double k, double b)
        {
            listBox.Items.Add("/// Линейный ///");
            chart.Series.Add(new Series("Линейная") { ChartType = SeriesChartType.Spline });
            listBox.Items.Add($"Линейное уравнение: y = {k}x + ({b})");
            for (int i = 0; i < x.Count; i++)
            {
                chart.Series["Линейная"].Points.AddXY(x[i], k * x[i] + b);
                LinearArray.Add(k * x[i] + b);
            }
            PreferedValue = ToShitDeltaY(LinearArray);
            PreferedString = "линейный";
            //listBox.Items.Add($"Зигма: {ToShitDeltaY(LinearArray)}");
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
            dataGridView.Rows[4].Cells[11].Value = Sum[0];
            dataGridView.Rows[5].Cells[11].Value = Sum[1];
            dataGridView.Rows[6].Cells[11].Value = Sum[2];
            dataGridView.Rows[7].Cells[11].Value = Sum[3];
            BuildDegree(Sum[0], Sum[1], Sum[2], Sum[3]);
        }


        void BuildDegree(double sumx, double sumy, double sumsqx, double sumyx)
        {
            listBox.Items.Add("/// Степенной ///");
            chart.Series.Add(new Series("Степенная") { ChartType = SeriesChartType.Spline });
            listBox.Items.Add("Решение методом Крамера...");
            double m = Cramer(new List<List<double>> { new List<double> { sumsqx, sumx }, new List<double> { sumx, x.Count } }, new List<double> { sumyx, sumy })[0];
            double c = Math.Exp(Cramer(new List<List<double>> { new List<double> { sumsqx, sumx }, new List<double> { sumx, x.Count } }, new List<double> { sumyx, sumy })[1]);
            listBox.Items.Add($"Значение c: {c}");
            listBox.Items.Add($"Значение m: {m}");
            listBox.Items.Add($"Степенное уравнение: y = {c}x^{m}");
            labelDegree.Text = $"{(float)sumsqx}k + {(float)sumx}b = {(float)sumyx}\n{(float)sumx}k + {x.Count}b = {(float)sumy}";
            for (int i = 0; i < x.Count; i++)
            {
                chart.Series["Степенная"].Points.AddXY(x[i], c * Math.Pow(x[i], m));
                DegreeArray.Add(c * Math.Pow(x[i], m));
            }
            if(PreferedValue > ToShitDeltaY(DegreeArray))
            {
                PreferedString = "степенной";
            }
            //listBox.Items.Add($"Зигма: {ToShitDeltaY(DegreeArray)}");
        }

        void BuildSquare(double sumxy, double sumy, double sqrx, double sumx)
        {
            listBox.Items.Add("/// Квадратичный ///");
            chart.Series.Add(new Series("Квадратичная") { ChartType = SeriesChartType.Spline });
            dataGridView.Rows.Add("x^3");
            dataGridView.Rows.Add("x^4");
            dataGridView.Rows.Add("Sqr(x) * y");
            List<double> Sum = new List<double>() { 0, 0, 0 };
            for (int i = 0; i < x.Count; i++)
            {
                dataGridView.Rows[8].Cells[i + 1].Value = Math.Pow(x[i], 3);
                Sum[0] += Math.Pow(x[i], 3);
                dataGridView.Rows[9].Cells[i + 1].Value = Math.Pow(x[i], 4);
                Sum[1] += Math.Pow(x[i], 4);
                dataGridView.Rows[10].Cells[i + 1].Value = Math.Pow(x[i], 2) * y[i];
                Sum[2] += Math.Pow(x[i], 2) * y[i];
            }
            dataGridView.Rows[8].Cells[11].Value = Sum[0];
            dataGridView.Rows[9].Cells[11].Value = Sum[1];
            dataGridView.Rows[10].Cells[11].Value = Sum[2];
            Gauss gauss = new Gauss(3, 3);
            double[][] Mtr = new double[][] 
            {
                new double[] { Sum[1], Sum[0], sqrx },
                new double[] { Sum[0], sqrx, sumx }, 
                new double[] { sqrx, sumx, x.Count },
            };
            gauss.RightPart[0] = Sum[2];
            gauss.RightPart[1] = sumxy;
            gauss.RightPart[2] = sumy;
            gauss.Matrix = Mtr;
            gauss.SolveMatrix();
            listBox.Items.Add($"Значение a: {gauss.Answer[0]}");
            listBox.Items.Add($"Значение b: {gauss.Answer[1]}");
            listBox.Items.Add($"Значение c: {gauss.Answer[2]}");
            listBox.Items.Add($"Квад. уравнение: y = {gauss.Answer[0]}x^2 + {gauss.Answer[1]}x + {gauss.Answer[2]}");
            for (int i = 0; i < x.Count; i++)
            {
                chart.Series["Квадратичная"].Points.AddXY(x[i], (gauss.Answer[0] * Math.Pow(x[i], 2)) + (gauss.Answer[1] * x[i]) + gauss.Answer[2]);
                SquareArray.Add((gauss.Answer[0] * Math.Pow(x[i], 2)) + (gauss.Answer[1] * x[i]) + gauss.Answer[2]);
            }
        }

        void FindSuareDifference()
        {
            dataGridView.Rows.Add("y(лин)");
            dataGridView.Rows.Add("y(стп)");
            dataGridView.Rows.Add("y(квд)");
            dataGridView.Rows.Add("y - y(лин)");
            dataGridView.Rows.Add("y - y(стп)");
            dataGridView.Rows.Add("y - y(квд)");
            List<double> Sum = new List<double>() { 0, 0, 0, 0, 0, 0 };
            for (int i = 0; i < x.Count; i++)
            {
                dataGridView.Rows[11].Cells[i + 1].Value = LinearArray[i];
                Sum[0] += LinearArray[i];
                dataGridView.Rows[12].Cells[i + 1].Value = DegreeArray[i];
                Sum[1] += DegreeArray[i];
                dataGridView.Rows[13].Cells[i + 1].Value = SquareArray[i];
                Sum[2] += SquareArray[i];
                dataGridView.Rows[14].Cells[i + 1].Value = Math.Pow(y[i] - LinearArray[i], 2);
                Sum[3] += Math.Pow(y[i] - LinearArray[i], 2);
                dataGridView.Rows[15].Cells[i + 1].Value = Math.Pow(y[i] - DegreeArray[i], 2);
                Sum[4] += Math.Pow(y[i] - DegreeArray[i], 2);
                dataGridView.Rows[16].Cells[i + 1].Value = Math.Pow(y[i] - SquareArray[i], 2);
                Sum[5] += Math.Pow(y[i] - SquareArray[i], 2);
            }
            dataGridView.Rows[11].Cells[11].Value = Sum[0];
            dataGridView.Rows[12].Cells[11].Value = Sum[1];
            dataGridView.Rows[13].Cells[11].Value = Sum[2];
            dataGridView.Rows[14].Cells[11].Value = Sum[3];
            dataGridView.Rows[15].Cells[11].Value = Sum[4];
            dataGridView.Rows[16].Cells[11].Value = Sum[5];
        }

        private void Start_Click(object sender, EventArgs e)
        {
            chart.Series.Add(new Series("Исходная") { ChartType = SeriesChartType.Point });
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
            dataGridView.Rows[0].Cells[11].Value = Sum[0];
            dataGridView.Rows[1].Cells[11].Value = Sum[1];
            dataGridView.Rows[2].Cells[11].Value = Sum[2];
            dataGridView.Rows[3].Cells[11].Value = Sum[3];
            labelLinear.Text = $"{Sum[2]}k + {Sum[0]}b = {Sum[3]}\n{Sum[0]}k + {x.Count}b = {Sum[1]}";
            listBox.Items.Add("Решение методом Крамера...");
            double k = Cramer(new List<List<double>> { new List<double> { Sum[2], Sum[0] }, new List<double> { Sum[0], x.Count } }, new List<double> { Sum[3], Sum[1] })[0];
            double b = Cramer(new List<List<double>> { new List<double> { Sum[2], Sum[0] }, new List<double> { Sum[0], x.Count } }, new List<double> { Sum[3], Sum[1] })[1];
            listBox.Items.Add($"Значение k: {k}");
            listBox.Items.Add($"Значение b: {b}");
            BuildLinear(k, b);
            BuildLogs();
            BuildSquare(Sum[3], Sum[1], Sum[2], Sum[0]);
            FindSuareDifference();
            //listBox.Items.Add($"По результатам зигмы побеждает {PreferedString} метод!");
            Start.Visible = false;
        }
    }
}
