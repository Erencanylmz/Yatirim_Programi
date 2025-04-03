SQL de database ve bir adet excel dosyası oluşturup istenilenlere göre ayarlayıp kullanmaya başlayabilirsiniz.

                                        --- SQL ---
                                Database name: KullaniciDB

                                Tables name:dbo.Kullaniciler

                                Column Name1: KullaniciAdi

                                Column Name2: Sifre



                                        --- excel ---
                                  A        B      C      D
                              1 ALIS_T   FIYAT   TUR   ADET


Şeklinde ayarlayın.

string connectionString = "Server=localhost;Database=KullaniciDB;User Id=sa;Password=password;";
kısmında SQL bağlantınıza göre düzenleyin.
