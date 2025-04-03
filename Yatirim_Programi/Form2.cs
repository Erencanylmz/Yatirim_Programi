using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Yatirim_Programi
{
    public partial class Form2 : Form
    {
        public Form2()
        {


            InitializeComponent();
            label5.MouseEnter += label5_MouseEnter;
            label5.MouseLeave += label5_MouseLeave;
        }
        //Bağlantı kodu
        string connectionString = "Server=192.168.11.190;Database=KullaniciDB;User Id=sa;Password=metasoft123;";
        private void label5_MouseEnter(object sender, EventArgs e)
        {
            label5.Font = new Font(label5.Font.FontFamily, label5.Font.Size + 1, label5.Font.Style);
            label5.ForeColor = Color.Black; // Opsiyonel: Renk değiştirme
            label5.Cursor = Cursors.Hand; // El işareti
        }

        private void label5_MouseLeave(object sender, EventArgs e)
        {
            label5.Font = new Font(label5.Font.FontFamily, label5.Font.Size - 1, label5.Font.Style);
            label5.ForeColor = Color.Black; // Eski rengine dön
            label5.Cursor = Cursors.Default;
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO Kullanicilar (KullaniciAdi, Sifre) VALUES (@kullaniciAdi, @sifre)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@kullaniciAdi", textBox3.Text);
                cmd.Parameters.AddWithValue("@sifre", textBox4.Text); // NOT: Gerçek projede şifreyi hashle!

                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show($"{textBox1.Text} {textBox2.Text} kaydınız başarıyla oluşturuldu.");
                    Form1 form = new Form1();
                    form.Show();
                    this.Hide();
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627) // Unique constraint
                        MessageBox.Show("Bu kullanıcı adı zaten var.");
                    else
                        MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }

        private void label5_Click_1(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }
    }
}