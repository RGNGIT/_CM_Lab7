using System;
using System.Collections.Generic;
using System.Linq;
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
            //1, 2, 3, 4, 5, 6, 7, 8, 9, 10
        };

        List<double> y = new List<double>() 
        {
            7.76, 11.30, 11.50, 13.41, 18.40, 21.30, 24.50, 28.00, 29.85, 31.80
            //0.55, 0.8359, 1.1447, 1.4642, 1.6218, 1.9668, 2.1017, 2.3284, 2.5480, 2.8617
        };

        List<double> xx = new List<double>() 
        {
            2.022, 2.033, 2.055, 2.077, 2.099
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

        List<double> LinearArray = new List<double>();
        List<double> DegreeArray = new List<double>();
        List<double> SquareArray = new List<double>();
        List<double> HyperbArray = new List<double>();

        void BuildLinear(double k, double b)
        {
            chart.Series.Add(new Series("Линейная") { ChartType = SeriesChartType.Spline });
            listBox.Items.Add($"Линейное уравнение: y = {k}x + ({b})");
            for (int i = 0; i < x.Count; i++)
            {
                chart.Series["Линейная"].Points.AddXY(x[i], k * x[i] + b);
                LinearArray.Add((float)(k * x[i] + b));
            }
            foreach(var i in xx)
            {
                listBox.Items.Add($"Линейные результаты: {(float)(k * i + b)}");
            }
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
                dataGridView.Rows[4].Cells[i + 1].Value = (float)Math.Log(x[i]);
                lnx.Add(Math.Log(x[i]));
                Sum[0] += Math.Log(x[i]);
                dataGridView.Rows[5].Cells[i + 1].Value = (float)Math.Log(y[i]);
                lny.Add(Math.Log(y[i]));
                Sum[1] += Math.Log(y[i]);
                dataGridView.Rows[6].Cells[i + 1].Value = (float)Squared(lnx).ToArray()[i];
                Sum[2] += Squared(lnx).ToArray()[i];
                dataGridView.Rows[7].Cells[i + 1].Value = (float)Mul(lnx, lny).ToArray()[i];
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
            chart.Series.Add(new Series("Степенная") { ChartType = SeriesChartType.Spline, BorderWidth = 2 });
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
                DegreeArray.Add((float)(c * Math.Pow(x[i], m)));
            }
            foreach (var i in xx)
            {
                listBox.Items.Add($"Степенные результаты: {(float)Math.Pow(c * i, m)}");
            }
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
                dataGridView.Rows[8].Cells[i + 1].Value = (float)Math.Pow(x[i], 3);
                Sum[0] += Math.Pow(x[i], 3);
                dataGridView.Rows[9].Cells[i + 1].Value = (float)Math.Pow(x[i], 4);
                Sum[1] += Math.Pow(x[i], 4);
                dataGridView.Rows[10].Cells[i + 1].Value = (float)(Math.Pow(x[i], 2) * y[i]);
                Sum[2] += Math.Pow(x[i], 2) * y[i];
            }
            dataGridView.Rows[8].Cells[11].Value = Sum[0];
            dataGridView.Rows[9].Cells[11].Value = Sum[1];
            dataGridView.Rows[10].Cells[11].Value = Sum[2];
            labelSuqared.Text = $"{Sum[1]}a + {Sum[0]}b + {sqrx}c = {Sum[2]}\n{Sum[0]}a + {sqrx}b + {sumx}c = {sumxy}\n{sqrx}a + {sumx}b + {x.Count}c = {sumy}";
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
            listBox.Items.Add("Решение методом Гаусса...");
            gauss.SolveMatrix();
            listBox.Items.Add($"Значение a: {gauss.Answer[0]}");
            listBox.Items.Add($"Значение b: {gauss.Answer[1]}");
            listBox.Items.Add($"Значение c: {gauss.Answer[2]}");
            listBox.Items.Add($"Квад. уравнение: y = {gauss.Answer[0]}x^2 + {gauss.Answer[1]}x + {gauss.Answer[2]}");
            for (int i = 0; i < x.Count; i++)
            {
                chart.Series["Квадратичная"].Points.AddXY(x[i], (gauss.Answer[0] * Math.Pow(x[i], 2)) + (gauss.Answer[1] * x[i]) + gauss.Answer[2]);
                SquareArray.Add((float)((gauss.Answer[0] * Math.Pow(x[i], 2)) + (gauss.Answer[1] * x[i]) + gauss.Answer[2]));
            }
            foreach (var i in xx)
            {
                listBox.Items.Add($"Квадратичные результаты: {(float)(gauss.Answer[0] * Math.Pow(i, 2) + gauss.Answer[1] * i + gauss.Answer[2])}");
            }
        }

        void BuildHyperbola(double sumy)
        {
            listBox.Items.Add("/// Гиперболический ///");
            chart.Series.Add(new Series("Гиперболическая") { ChartType = SeriesChartType.Spline });
            dataGridView.Rows.Add("1/x");
            dataGridView.Rows.Add("Sqr(1/x)");
            dataGridView.Rows.Add("y/x");
            List<double> Sum = new List<double>() { 0, 0, 0 };
            for (int i = 0; i < x.Count; i++)
            {
                dataGridView.Rows[11].Cells[i + 1].Value = (float)(1 / x[i]);
                Sum[0] += 1 / x[i];
                dataGridView.Rows[12].Cells[i + 1].Value = (float)Math.Pow(1 / x[i], 2);
                Sum[1] += Math.Pow(1 / x[i], 2);
                dataGridView.Rows[13].Cells[i + 1].Value = (float)(y[i] / x[i]);
                Sum[2] += y[i] / x[i];
            }
            dataGridView.Rows[11].Cells[11].Value = Sum[0];
            dataGridView.Rows[12].Cells[11].Value = Sum[1];
            dataGridView.Rows[13].Cells[11].Value = Sum[2];
            double a = (sumy * Sum[1] - Sum[0] * Sum[2]) / (x.Count * Sum[1] - Sum[0] * Sum[0]);
            double b = (x.Count * Sum[2] - Sum[0] * sumy) / (x.Count * Sum[1] - Sum[0] * Sum[0]);
            listBox.Items.Add($"Значение a: {a}");
            listBox.Items.Add($"Значение b: {b}");
            for (int i = 0; i < x.Count; i++)
            {
                chart.Series["Гиперболическая"].Points.AddXY(x[i], a + (b / x[i]));
                HyperbArray.Add((float)(a + (b / x[i])));
            }
            foreach (var i in xx)
            {
                listBox.Items.Add($"Гиперболические результаты: {(float)(a + (b / i))}");
            }
        }

        void FindSquareDifference()
        {
            dataGridView.Rows.Add("y(лин)");
            dataGridView.Rows.Add("y(стп)");
            dataGridView.Rows.Add("y(квд)");
            dataGridView.Rows.Add("y(гпб)");
            dataGridView.Rows.Add("y - y(лин)");
            dataGridView.Rows.Add("y - y(стп)");
            dataGridView.Rows.Add("y - y(квд)");
            dataGridView.Rows.Add("y - y(гпб)");
            List<string> Methods = new List<string>() 
            {
                "Линейный",
                "Степенной",
                "Квадратичный",
                "Гиперболический"
            };
            List<double> SqDiff = new List<double>();
            List<double> Sum = new List<double>() { 0, 0, 0, 0, 0, 0, 0, 0 };
            for (int i = 0; i < x.Count; i++)
            {
                dataGridView.Rows[14].Cells[i + 1].Value = LinearArray[i];
                Sum[0] += LinearArray[i];
                dataGridView.Rows[15].Cells[i + 1].Value = DegreeArray[i];
                Sum[1] += DegreeArray[i];
                dataGridView.Rows[16].Cells[i + 1].Value = SquareArray[i];
                Sum[2] += SquareArray[i];
                dataGridView.Rows[17].Cells[i + 1].Value = HyperbArray[i];
                Sum[3] += HyperbArray[i];
                dataGridView.Rows[18].Cells[i + 1].Value = (float)Math.Pow(y[i] - LinearArray[i], 2);
                Sum[4] += Math.Pow(y[i] - LinearArray[i], 2);
                dataGridView.Rows[19].Cells[i + 1].Value = (float)Math.Pow(y[i] - DegreeArray[i], 2);
                Sum[5] += Math.Pow(y[i] - DegreeArray[i], 2);
                dataGridView.Rows[20].Cells[i + 1].Value = (float)Math.Pow(y[i] - SquareArray[i], 2);
                Sum[6] += Math.Pow(y[i] - SquareArray[i], 2);
                dataGridView.Rows[21].Cells[i + 1].Value = (float)Math.Pow(y[i] - HyperbArray[i], 2);
                Sum[7] += Math.Pow(y[i] - HyperbArray[i], 2);
            }
            dataGridView.Rows[14].Cells[11].Value = Sum[0];
            dataGridView.Rows[15].Cells[11].Value = Sum[1];
            dataGridView.Rows[16].Cells[11].Value = Sum[2];
            dataGridView.Rows[17].Cells[11].Value = Sum[3];
            dataGridView.Rows[18].Cells[11].Value = Sum[4];
            SqDiff.Add(Sum[4]);
            dataGridView.Rows[19].Cells[11].Value = Sum[5];
            SqDiff.Add(Sum[5]);
            dataGridView.Rows[20].Cells[11].Value = Sum[6];
            SqDiff.Add(Sum[6]);
            dataGridView.Rows[21].Cells[11].Value = Sum[7];
            SqDiff.Add(Sum[7]);
            int index = 0;
            double min = SqDiff[0];
            for(int i = 0; i < SqDiff.Count; i++)
            {
                if(min > SqDiff[i])
                {
                    index = i;
                    min = SqDiff[i];
                }
            }
            listBox.Items.Add($"Предпочтительный метод: {Methods[index]}");
        }

        private void Start_Click(object sender, EventArgs e)
        {
            listBox.Items.Add("/// Линейный ///");
            chart.Series.Add(new Series("Исходная") { ChartType = SeriesChartType.Point, MarkerSize = 7 });
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
                dataGridView.Rows[2].Cells[i + 1].Value = (float)Squared(x).ToArray()[i];
                Sum[2] += Squared(x).ToArray()[i];
                dataGridView.Rows[3].Cells[i + 1].Value = (float)Mul(x, y).ToArray()[i];
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
            BuildHyperbola(Sum[1]);
            FindSquareDifference();
            //chart.Series.Remove(chart.Series["Гиперболическая"]);
            //chart.Series.Remove(chart.Series["Степенная"]);
            //chart.Series.Remove(chart.Series["Линейная"]);
            //chart.Series.Remove(chart.Series["Квадратичная"]);
            Start.Visible = false;
        }
    }
}
