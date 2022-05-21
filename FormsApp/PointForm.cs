using Newtonsoft.Json;
using PointLib;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace FormsApp
{
    public partial class PointForm : Form
    {
        private Point[] points = null; 
        public PointForm()
        {
            InitializeComponent();
        }

        private void btnCreate_Click(object sender, System.EventArgs e)
        {
            points = new Point[5];

            Random rnd = new Random();

            for (int i = 0; i < points.Length; i++)
            {
                points[i] = (rnd.Next(3) % 2 == 0) ? new Point() : new Point3D();
            }

            listBox.DataSource = points;
        }

        private void btnSort_Click(object sender, System.EventArgs e)
        {
            if (points == null)
            {
                return;
            }

            Array.Sort(points);

            listBox.DataSource = null;
            listBox.DataSource = points;
        }

        private void btnSerialize_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "SOAP|*.soap|XML|*.xml|JSON|*.json|Binary|*.bin";

            if (dlg.ShowDialog() != DialogResult.OK) 
            {
                return;
            }

            using (FileStream fs = new FileStream(dlg.FileName, FileMode.Create, FileAccess.Write))
            { 
                switch (Path.GetExtension(dlg.FileName))
                {
                    case ".bin":
                        BinaryFormatter bf = new BinaryFormatter();
                        bf.Serialize(fs, points);
                        break;
                    case ".soap":
                        SoapFormatter sf = new SoapFormatter();
                        sf.Serialize(fs, points);
                        break;
                    case ".xml":
                        XmlSerializer xs = new XmlSerializer(typeof(Point[]), new[] { typeof(Point3D) });
                        xs.Serialize(fs, points);
                        break;
                    case ".json":
                        JsonSerializer js = new JsonSerializer();
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            js.Serialize(sw, points);
                        }
                        break;
                }
            }
        }

        private void btnDeserialize_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "SOAP|*.soap|XML|*.xml|JSON|*.json|Binary|*.bin";

            if (dlg.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            using (FileStream fs = new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read))
            {
                switch (Path.GetExtension(dlg.FileName))
                {
                    case ".bin":
                        BinaryFormatter bf = new BinaryFormatter();
                        points = (Point[])bf.Deserialize(fs);
                        break;
                    case ".soap":
                        SoapFormatter sf = new SoapFormatter();
                        points = (Point[])sf.Deserialize(fs);
                        break;
                    case ".xml":
                        XmlSerializer xs = new XmlSerializer(typeof(Point[]), new[] { typeof(Point3D) });
                        points = (Point[])xs.Deserialize(fs);
                        break;
                    case ".json":
                        JsonSerializer js = new JsonSerializer();
                        using (StreamReader sr = new StreamReader(fs))
                        {
                            points = (Point[])js.Deserialize(sr, typeof(Point[]));
                        }
                        break;
                }
            }

            listBox.DataSource = null;
            listBox.DataSource = points;
        }
    }
}
