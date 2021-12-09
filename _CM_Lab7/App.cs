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
        };

        List<double> y = new List<double>() 
        {
            18.422, 12.844, 9.659, 7.394, 5.670, 4.303, 3.196, 2.297, 1.572, 1.000, 0.569
        };
        
        IEnumerable<double> Squared()
        {
            for(int i = 0; i < x.Count; i++)
            {
                yield return Math.Pow(x[i], 2);
            }
        }

        IEnumerable<double> Mul()
        {
            for (int i = 0; i < x.Count; i++)
            {
                yield return x[i] * y[i];
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
                    }),
                    FindDet(new List<List<double>> // y
                    {
                        new List<double> { Mtrx[0][0],  Equal[0] },
                        new List<double> { Mtrx[1][0],  Equal[1] }
                    })
                };
            } 
            else
            {
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

        double FindM()
        {
            double Sum1() 
            {
                double sum = 0;
                for(int i = 0; i < x.Count; i++)
                {
                    sum += Math.Log(x[i]) * Math.Log(y[i]);
                }
                return sum;
            }
            double Sum2()
            {
                double sum = 0;
                for (int i = 0; i < x.Count; i++)
                {
                    sum += Math.Log(x[i]);
                }
                return sum;
            }
            double Sum3()
            {
                double sum = 0;
                for (int i = 0; i < x.Count; i++)
                {
                    sum += Math.Log(y[i]);
                }
                return sum;
            }
            double Sum4()
            {
                double sum = 0;
                for (int i = 0; i < x.Count; i++)
                {
                    sum += Math.Pow(Math.Log(x[i]), 2);
                }
                return sum;
            }
            return (x.Count * Sum1() - Sum2() * Sum3()) / (Sum4() - Math.Pow(Sum2(), 2));
        }

        double FindC(double m)
        {
            double Sum1()
            {
                double sum = 0;
                for(int i = 0; i < x.Count; i++)
                {
                    sum += Math.Log(y[i]);
                }
                return sum;
            }
            double Sum2()
            {
                double sum = 0;
                for (int i = 0; i < x.Count; i++)
                {
                    sum += Math.Log(x[i]);
                }
                return sum;
            }
            return Math.Exp(/*(1 / x.Count)*/0.09 * (Sum1() - m * Sum2()));
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

        void BuildDegree(double sumx, double sumyx, double sumsqx)
        {
            listBox.Items.Add("/// Степенной ///");
            chart.Series.Add(new Series("Степенная") { ChartType = SeriesChartType.Line });
            double m = FindM();
            double c = FindC(m);
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
                dataGridView.Rows[2].Cells[i + 1].Value = Squared().ToArray()[i];
                Sum[2] += Squared().ToArray()[i];
                dataGridView.Rows[3].Cells[i + 1].Value = Mul().ToArray()[i];
                Sum[3] += Mul().ToArray()[i];
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
            BuildDegree(Sum[0], Sum[3], Sum[2]);
            listBox.Items.Add($"По результатам зигмы побеждает {PreferedString} метод!");
            Start.Visible = false;
        }
    }
}
