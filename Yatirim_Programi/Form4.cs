using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using HtmlAgilityPack;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using OfficeOpenXml;
using OfficeOpenXml.Core.ExcelPackage;
using System.Data.OleDb;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.IO;

namespace Yatirim_Programi
{
    public partial class Form4 : Form
    {
        private const string baglanti = "https://piyasa.paratic.com/altin/gram/";
        private readonly string url = baglanti;


        public Form4()
        {
            InitializeComponent();
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

        }

        private static readonly OleDbConnection baglanti1 = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Erencan YILMAZ\Desktop\Kitap1.xlsx;Extended Properties='Excel 8.0;HDR=YES'");
        private readonly OleDbConnection connection = baglanti1;
        private async void Form2_Load_1(object sender, EventArgs e)
        {
            while (true)
            {
                await GetDivDataAsync();
                await Task.Delay(5000); // 5 saniyede bir yenile
            }
        }

        private async Task GetDivDataAsync()
        {
            try
            {
                HttpClient client = new HttpClient();
                var html = await client.GetStringAsync(url);

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                // class='price' olan ilk div'i seçiyoruz
                var div = doc.DocumentNode.SelectSingleNode("//div[@class='ins_alsat al']/div[@class='price']");


                if (div != null)
                {
                    string altinFiyatiAl = div.InnerText.Trim();

                    Invoke(new Action(() =>
                    {
                        lblAltinFiyatiAl.Text = $"Gram Altın: {altinFiyatiAl}";
                    }));
                }
                else
                {
                    Invoke(new Action(() =>
                    {
                        lblAltinFiyatiAl.Text = "Altın fiyatı div'i bulunamadı.";
                    }));
                }
                var div1 = doc.DocumentNode.SelectSingleNode("//div[@class='ins_alsat sat']/div[@class='price']");


                if (div1 != null)
                {
                    string altinFiyatiSat = div1.InnerText.Trim();

                    Invoke(new Action(() =>
                    {
                        lblAltinFiyatiSat.Text = $"Gram Altın: {altinFiyatiSat}";
                        tarih.Text = $"Güncelleme Zamanı:({DateTime.Now:T})";
                    }));
                }
                else
                {
                    Invoke(new Action(() =>
                    {
                        lblAltinFiyatiAl.Text = "Altın fiyatı div'i bulunamadı.";
                        lblAltinFiyatiSat.Text = "Altın fiyatı div'i bulunamadı.";

                    }));
                }
            }

            catch (Exception ex)
            {
                Invoke(new Action(() =>
                {
                    lblAltinFiyatiAl.Text = "Hata: " + ex.Message;
                    lblAltinFiyatiSat.Text = "Hata: " + ex.Message;
                }));
            }

        }
        private void Hesaplama_Click(object sender, EventArgs e)
        {
            try
            {
                double gramMiktar = Convert.ToDouble(textBox1.Text);
                string altinFiyatiAlStr = lblAltinFiyatiAl.Text.Replace("Gram Altın: ", "").Trim();

                if (double.TryParse(altinFiyatiAlStr, out double altinFiyatiAl))
                {
                    double toplamFiyat = altinFiyatiAl * gramMiktar;
                    TOPLAM.Text = $"{toplamFiyat:F2} ₺";
                }
                else
                {
                    TOPLAM.Text = "Geçerli bir fiyat alınamadı.";
                }
            }
            catch (Exception ex)
            {
                TOPLAM.Text = "Hata: " + ex.Message;
            }
        }

        private void Ekle_Click(object sender, EventArgs e)
        {
            try
            {
                int nextRow = GetNextEmptyRow(); // otomatik boş satırı bul
                connection.Open();

                string komut = $"INSERT INTO [Sayfa1$] ([ALIS_T], [FIYAT], [TUR], [ADET]) VALUES (@p1, @p2, @p3, @p4)";
                OleDbCommand cmd = new OleDbCommand(komut, connection);

                cmd.Parameters.AddWithValue("@p1", textBox2.Text);
                cmd.Parameters.AddWithValue("@p2", textBox3.Text);
                cmd.Parameters.AddWithValue("@p3", comboBox1.Text);
                cmd.Parameters.AddWithValue("@p4", textBox5.Text);

                cmd.ExecuteNonQuery();
                MessageBox.Show($"Veri satır {nextRow}. sıraya eklendi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
           

        }




        private void ExcelVerileriniDataGridVieweYaz()
        {
            try
            {
                DataTable dt = new DataTable();
                using (OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM [Sayfa1$]", connection))
                {
                    da.Fill(dt);
                }

                dataGridView1.DataSource = dt;

                // 🔥 FIYAT × ADET hesapla
                double toplam = dt.AsEnumerable().Sum(row =>
                {
                    double fiyat = 0;
                    double adet = 0;

                    double.TryParse(row["FIYAT"]?.ToString(), out fiyat);
                    double.TryParse(row["ADET"]?.ToString(), out adet);

                    return fiyat * adet;
                });

                GİDER.Text = $"{toplam:F2} ₺";

            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }



        private void Button2_Click_1(object sender, EventArgs e)
        {
            ExcelVerileriniDataGridVieweYaz();
            int toplamAdet = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                // Satır boş değilse ve "ADET" değeri varsa topla
                if (row.Cells["ADET"].Value != null && int.TryParse(row.Cells["ADET"].Value.ToString(), out int adet))
                {
                    toplamAdet += adet;
                }
            }

            textBox1.Text = toplamAdet.ToString();

        }
  
        private void ÇIKIŞToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 frm = new Form1();
            frm.Show();
            this.Hide();
        }
        private int GetNextEmptyRow()
        {
            int nextRow = 1;
            try
            {
                connection.Open();
                OleDbCommand cmd = new OleDbCommand("SELECT * FROM [Sayfa1$]", connection);
                OleDbDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    nextRow++;
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Satır bulma hatası: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
            return nextRow;
        }
        private void ExcelFormulHesaplat()
        {
            try
            {
                var excelApp = new Microsoft.Office.Interop.Excel.Application();
                var workbook = excelApp.Workbooks.Open(@"C:\Users\Erencan YILMAZ\Desktop\Kitap1.xlsx");
                var sheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];

                // Tüm formülleri yeniden hesaplat
                sheet.Calculate();

                workbook.Save();
                workbook.Close(false);
                excelApp.Quit();

                Marshal.ReleaseComObject(sheet);
                Marshal.ReleaseComObject(workbook);
                Marshal.ReleaseComObject(excelApp);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Excel hesaplama hatası: " + ex.Message);
            }
        }


        private void ÇIKIŞToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Form1 frm = new Form1();
            frm.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen silinecek satırı seçin.");
                return;
            }

            int selectedIndex = dataGridView1.SelectedRows[0].Index;
            string path = @"C:\Users\Erencan YILMAZ\Desktop\Kitap1.xlsx";

            FileInfo fileInfo = new FileInfo(path);
            using (OfficeOpenXml.ExcelPackage package = new OfficeOpenXml.ExcelPackage(fileInfo))
            {
                OfficeOpenXml.ExcelWorksheet worksheet = package.Workbook.Worksheets["Sayfa1"];
                int rowCount = worksheet.Dimension.End.Row;

                // Yeni bir liste oluştur, sadece silinmeyecek satırları ekle
                List<object[]> satirlar = new List<object[]>();

                for (int i = 2; i <= rowCount; i++) // Başlıklar varsa 2'den başla
                {
                    if (i - 2 != selectedIndex) // seçilen satırı atla
                    {
                        object[] rowData = new object[4];
                        for (int j = 1; j <= 4; j++)
                        {
                            rowData[j - 1] = worksheet.Cells[i, j].Value;
                        }
                        satirlar.Add(rowData);
                    }
                }

                // Sayfayı temizle
                worksheet.Cells.Clear();

                // Başlıkları geri yaz
                worksheet.Cells[1, 1].Value = "ALIS_T";
                worksheet.Cells[1, 2].Value = "FIYAT";
                worksheet.Cells[1, 3].Value = "TUR";
                worksheet.Cells[1, 4].Value = "ADET";

                // Kalan satırları geri yaz
                int rowNum = 2;
                foreach (var row in satirlar)
                {
                    for (int col = 0; col < row.Length; col++)
                    {
                        worksheet.Cells[rowNum, col + 1].Value = row[col];
                    }
                    rowNum++;
                }

                package.Save();
            }

            MessageBox.Show("Satır silindi.");
            ExcelVerileriniDataGridVieweYaz(); //
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Label'lardaki ₺ sembolünü temizleyip sayıya çeviriyoruz
            string metin1 = TOPLAM.Text.Replace("₺", "").Trim();
            string metin2 = GİDER.Text.Replace("₺", "").Trim();

            if (double.TryParse(metin1, out double sayi1) &&
                double.TryParse(metin2, out double sayi2))
            {
                double fark = sayi1 - sayi2;
                KAR.Text = fark.ToString("N2") + " ₺"; // Sonucu yazdır
            }
            else
            {
                KAR.Text = "Geçersiz sayı!";
            }

        }
    }
}