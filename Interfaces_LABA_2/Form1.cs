using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace Interfaces_LABA_2
{
    public partial class Form1 : Form
    {
        public List<Files> Files { get; set; }
        public Form1()
        {
            Files = new List<Files>();
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            chart1.Series.Clear();
        }

        private void Data_ListChanged(object sender, ListChangedEventArgs e)
        {
            chart1.Series[checkedListBox1.SelectedIndex].Points.DataBind(Files[checkedListBox1.SelectedIndex].Data.List, "X", "Y", "");
        }

        private void AddButton_Click(object sender, EventArgs e)
        {

            Files[checkedListBox1.SelectedIndex].Data.Add(new Data() { X = 0, Y = 0 });
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (int i in checkedListBox1.SelectedIndices)
            {
                switch (comboBox1.SelectedIndex)
                {
                    case 0:
                        {
                            chart1.Series[i].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                            break;
                        }
                    case 1:
                        {
                            chart1.Series[i].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
                            break;
                        }
                    case 2:
                        {
                            chart1.Series[i].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
                            break;
                        }
                }
            }
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            openFileDialog.ShowDialog();

            var s = File.ReadAllLines(openFileDialog.FileName);

            Random r = new Random();
            Files file = new Files();
            file.Data = new BindingSource();
            file.FileName = openFileDialog.FileName;
            file.Color = Color.FromArgb(r.Next(255), r.Next(255), r.Next(255));

            foreach (var str in s)
            {
                var coords = str.Split('\t');
                file.Data.Add(new Data() { X = double.Parse(coords[0]), Y = double.Parse(coords[1]) });
            }
            file.Data.ListChanged += Data_ListChanged;
            Files.Add(file);

            Series series = new Series();
            series.LegendText = file.FileName;
            series.Color = file.Color;
            series.Points.DataBind(file.Data.List, "X", "Y", "");
            series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series.Add(series);

            checkedListBox1.Items.Add(file.FileName, true);
            checkedListBox1.SelectedIndex = checkedListBox1.Items.Count - 1;
            dataGridView1.DataSource = file.Data;
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt";
            saveFileDialog.ShowDialog();
            
            var file = new StreamWriter(saveFileDialog.FileName);
            foreach (Data point in Files[checkedListBox1.SelectedIndex].Data)
            {
                file.WriteLine($"{point.X}\t{point.Y}");
            }
            file.Close();      
        }

        private void checkedListBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            dataGridView1.DataSource = Files[checkedListBox1.SelectedIndex].Data;
            switch (chart1.Series[checkedListBox1.SelectedIndex].ChartType)
            {
                case System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line:
                    {
                        comboBox1.SelectedIndex = 0;
                        break;
                    }
                case System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline:
                    {
                        comboBox1.SelectedIndex = 1;
                        break;
                    }
                case System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point:
                    {
                        comboBox1.SelectedIndex = 2;
                        break;
                    }
            }
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            chart1.Series[e.Index].Enabled = e.NewValue == CheckState.Checked;
        }
    }

    public class Data
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class Files
    {
        public string FileName { get; set; }
        public Color Color { get; set; }
        public BindingSource Data { get; set; }
    }
}
