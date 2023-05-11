using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml.Serialization;

namespace ArabaListesi
{
    // BU PROGRAM BİLGİSAYAR PROGRAMCILIĞI 2. SINIF 2. ÖĞRETİM ÖĞRENCİLERİNDEN 21015222019 NUMARALI SEMİH ÖZDEMİR TARAFINDAN KODLANMIŞTIR
     
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            string temp = Path.Combine(Application.CommonAppDataPath, "data");
            if (File.Exists(temp))
            {
                string jsondata = File.ReadAllText(temp);
                cars = JsonSerializer.Deserialize<List<Car>>(jsondata);
            }
           
            ShowList();
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        List<Car> cars = new List<Car>()
        {
            new Car()
            {
                Model = "Kuga",
                Marka = "Ford",
                Yil = new DateTime(2011,1,1),
                VitesTipi = "Manuel",
                Durumu = "Sıfır",
                Yakit = "Dizel",
                KasaTipi = "Sedan",
                MotorGucu = "120",
                Rengi = "Beyaz",
                },
             new Car()
            {
                Model = "Corsa",
                Marka = "Opel",
                Yil = new DateTime(2019,1,1),
                VitesTipi = "Manuel",
                Durumu = "İkinci El",
                Yakit = "Benzin",
                KasaTipi = "Sedan",
                MotorGucu = "90",
                Rengi = "Mavi",
                }

        };

      
        public void ShowList()
        {
            listView1.Items.Clear();
            foreach (Car car in cars)
            {
                AddCarToListView(car);
            }
        }

        public void AddCarToListView(Car car)
        {
            ListViewItem item = new ListViewItem(new string[]
                            {
                    car.Model,
                    car.Marka,
                    car.Yil.Year.ToString(),
                    car.Durumu,
                    car.VitesTipi,
                    car.Yakit,
                    car.KasaTipi,
                    car.MotorGucu,
                    car.Rengi,
                            });
            item.Tag = car;
            listView1.Items.Add(item);
        }

        public void EditCarOnListView(ListViewItem cItem, Car car)
        {
            cItem.SubItems[0].Text = car.Model;
            cItem.SubItems[1].Text = car.Marka;
            cItem.SubItems[2].Text = car.Yil.Year.ToString();
            cItem.SubItems[3].Text = car.Durumu;
            cItem.SubItems[4].Text = car.VitesTipi;
            cItem.SubItems[5].Text = car.Yakit;
            cItem.SubItems[6].Text = car.KasaTipi;
            cItem.SubItems[7].Text = car.MotorGucu;
            cItem.SubItems[8].Text = car.Rengi;

            cItem.Tag = car;
        }
        private void AddCommand(object sender, EventArgs e)
        {
            FrmCar frm = new FrmCar() { 
                Text = "Araç Ekle",
                StartPosition = FormStartPosition.CenterScreen,
                car = new Car()
            };

            if(frm.ShowDialog()== DialogResult.OK)
            {
                cars.Add(frm.car);
                AddCarToListView(frm.car);
            }
        }

        private void EditCommand(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;

            
            ListViewItem cItem = listView1.SelectedItems[0];

            Car secili = cItem.Tag as Car;

            FrmCar frm = new FrmCar()
            {
                Text = "Araç Düzenle",
                StartPosition = FormStartPosition.CenterScreen,
                car = Clone(secili),
            };

            if(frm.ShowDialog() == DialogResult.OK)
            {
               secili = frm.car;
                EditCarOnListView(cItem, secili);
            }

            Car Clone(Car car)
            {
                return new Car()
                {
                    id = car.ID,
                    Model = car.Model,
                    Marka = car.Marka,
                    Yil = car.Yil,
                    Durumu = car.Durumu,
                    VitesTipi = car.VitesTipi,
                    Yakit = car.Yakit,
                    KasaTipi = car.KasaTipi,
                    MotorGucu = car.MotorGucu,
                    Rengi = car.Rengi,

                };
            }
}

        private void DeleteCommand(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;


            ListViewItem cItem = listView1.SelectedItems[0];

            Car secili = cItem.Tag as Car;


            var sonuc= MessageBox.Show($"Seçili Araç Silinsin mi ? \n\n {secili.Marka} {secili.Model} | {secili.Yil.Year.ToString()} | {secili.Rengi}",
                "Silmeyi Onayla",
                 MessageBoxButtons.YesNo,
                 MessageBoxIcon.Question);

            if (sonuc == DialogResult.Yes)
            {
                cars.Remove(secili);
                listView1.Items.Remove(cItem);
            }
        }

        private void SaveCommand(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog()
            {
                Filter = "Json Formatı|*.json|Xml Formatı|*.xml"
            };

            if(sf.ShowDialog() == DialogResult.OK)
            {
                if(sf.FileName.EndsWith("json"))
                {
                    string data = JsonSerializer.Serialize(cars);
                    File.WriteAllText(sf.FileName, data);
                }
                else if(sf.FileName.EndsWith("xml")) 
                {
                    StreamWriter streamWriter = new StreamWriter(sf.FileName);
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Car>));
                    serializer.Serialize(streamWriter, cars);
                    streamWriter.Close();
                }
            }
        }

        private void LoadCommand(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog()
            {
                Filter = "Json, Xml Formatları|*.json;*.xml"
            };

            if(of.ShowDialog() == DialogResult.OK)
            {
                if (of.FileName.ToLower().EndsWith("json"))
                {
                    string jsondata = File.ReadAllText(of.FileName);

                  cars = JsonSerializer.Deserialize<List<Car>>(jsondata);
                }
                else if (of.FileName.ToLower().EndsWith("xml"))
                {
                  StreamReader sr = new StreamReader(of.FileName);
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Car>));
                   cars = (List<Car>)serializer.Deserialize(sr);
                    sr.Close();
                }
                ShowList();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            string temp = Path.Combine(Application.CommonAppDataPath, "data");

            string data = JsonSerializer.Serialize(cars);
            File.WriteAllText(temp, data);

            base.OnClosing(e);
        }

        private void hakkındaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox1().ShowDialog();
        }
    }


    // aşağıdaki iki yorum satırı çicgilerinin arasındaki kodu internetten alıp kendime göre düzenledim
    
    //
    public class YearEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            NumericUpDown control = new NumericUpDown();
            control.Maximum = 2023; 
            control.Minimum = 1900; 

            DateTime dateValue = (DateTime)value;
            control.Value = dateValue.Year >= control.Minimum && dateValue.Year <= control.Maximum ? dateValue.Year : control.Minimum;

            editorService.DropDownControl(control);

            return new DateTime((int)control.Value, dateValue.Month, dateValue.Day);
        }
    }



  


    //
    [Serializable]
    public class Car
    {
        public string id;

        [Browsable(false)]
        public string ID
        {
            get
            {
                if (id == null)
                    id = Guid.NewGuid().ToString();
                return id;
            }
            set { id = value; }
        }
        public string Model { get; set; }
        public string Marka { get; set; }

        [Editor(typeof(YearEditor), typeof(UITypeEditor))]
        public DateTime Yil { get; set; }
        public string VitesTipi { get; set; }
        public string Durumu { get; set; }
        public string Yakit { get; set; }
        public string KasaTipi { get; set; }
        public string MotorGucu { get; set; }
        public string Rengi { get; set; }

    }

}
