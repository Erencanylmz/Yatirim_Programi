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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            label1.MouseEnter += label1_MouseEnter;
            label1.MouseLeave += label1_MouseLeave;

        }
        //Bağlantı kodu
        string connectionString = "Server=192.168.11.190;Database=KullaniciDB;User Id=sa;Password=metasoft123;";

        


        private void label1_Click_1(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
            this.Hide();
        }
        private void label1_MouseEnter(object sender, EventArgs e)
        {
            label1.Font = new Font(label1.Font.FontFamily, label1.Font.Size + 1, label1.Font.Style);
            label1.ForeColor = Color.Black; // Opsiyonel: Renk değiştirme
            label1.Cursor = Cursors.Hand; // El işareti
        }

        private void label1_MouseLeave(object sender, EventArgs e)
        {
            label1.Font = new Font(label1.Font.FontFamily, label1.Font.Size - 1, label1.Font.Style);
            label1.ForeColor = Color.Black; // Eski rengine dön
            label1.Cursor = Cursors.Default;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Kullanicilar WHERE KullaniciAdi = @kullaniciAdi AND Sifre = @sifre";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@kullaniciAdi", textBox1.Text);
                cmd.Parameters.AddWithValue("@sifre", textBox2.Text);

                int result = (int)cmd.ExecuteScalar();
                if (result > 0)
                {
                    MessageBox.Show("Giriş başarılı!");
                    Form4 form4 = new Form4();
                    form4.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Kullanıcı adı veya şifre yanlış.");
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            textBox2.PasswordChar = '*';  // '*' yerine istediğin karakter

        }
    }
}