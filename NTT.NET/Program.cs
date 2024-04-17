using System;
using System.Collections.Generic;
using System.Linq;

namespace ParkingSystem
{
    // Definisikan tipe kendaraan
    public enum JenisKendaraan
    {
        Mobil,
        Motor
    }

    // Definisikan kelas untuk merepresentasikan kendaraan
    public class Kendaraan
    {
        public string NomorRegistrasi { get; }
        public string Warna { get; }
        public JenisKendaraan Tipe { get; }

        public Kendaraan(string nomorRegistrasi, string warna, JenisKendaraan tipe)
        {
            NomorRegistrasi = nomorRegistrasi;
            Warna = warna;
            Tipe = tipe;
        }
    }

    // Definisikan kelas untuk merepresentasikan tempat parkir
    public class TempatParkir
    {
        public int Nomor { get; }
        public Kendaraan Kendaraan { get; private set; }

        public TempatParkir(int nomor)
        {
            Nomor = nomor;
        }

        public bool IsTersedia()
        {
            return Kendaraan == null;
        }

        public void ParkirKendaraan(Kendaraan kendaraan)
        {
            Kendaraan = kendaraan;
        }

        public void KosongkanTempat()
        {
            Kendaraan = null;
        }
    }

    // Definisikan kelas untuk merepresentasikan area parkir (lot parkir)
    public class AreaParkir
    {
        private List<TempatParkir> tempatParkir;

        public AreaParkir(int kapasitas)
        {
            tempatParkir = new List<TempatParkir>();
            for (int i = 1; i <= kapasitas; i++)
            {
                tempatParkir.Add(new TempatParkir(i));
            }
        }

        // Metode untuk memeriksa kendaraan masuk
        public string Masuk(string nomorRegistrasi, string warna, JenisKendaraan tipe)
        {
            var tempatKosong = tempatParkir.FirstOrDefault(tempat => tempat.IsTersedia());
            if (tempatKosong != null)
            {
                var kendaraan = new Kendaraan(nomorRegistrasi, warna, tipe);
                tempatKosong.ParkirKendaraan(kendaraan);
                return $"Nomor slot yang dialokasikan: {tempatKosong.Nomor}";
            }
            else
            {
                return "Maaf, area parkir penuh.";
            }
        }

        // Metode untuk memeriksa kendaraan keluar
        public string Keluar(int nomorSlot)
        {
            var tempat = tempatParkir.FirstOrDefault(tempat => tempat.Nomor == nomorSlot);
            if (tempat != null && !tempat.IsTersedia())
            {
                tempat.KosongkanTempat();
                return $"Slot nomor {nomorSlot} telah kosong.";
            }
            else
            {
                return $"Slot nomor {nomorSlot} sudah kosong.";
            }
        }

        // Metode untuk mendapatkan status parkir
        public string StatusParkir()
        {
            var status = tempatParkir.Select(tempat => $"{tempat.Nomor}\t{tempat.Kendaraan?.NomorRegistrasi ?? "-"}\t\t{tempat.Kendaraan?.Tipe ?? JenisKendaraan.Mobil}\t{tempat.Kendaraan?.Warna ?? "-"}");
            return "Slot\tNomor Registrasi\tTipe\tWarna\n" + string.Join("\n", status);
        }

        // Metode untuk mendapatkan jumlah slot yang tersedia
        public int JumlahSlotTersedia()
        {
            return tempatParkir.Count(tempat => tempat.IsTersedia());
        }

        // Metode untuk mendapatkan jumlah slot yang terisi
        public int JumlahSlotTerisi()
        {
            return tempatParkir.Count(tempat => !tempat.IsTersedia());
        }

        // Metode untuk mendapatkan jumlah kendaraan berdasarkan jenis
        public int JumlahKendaraanBerdasarkanTipe(JenisKendaraan tipe)
        {
            return tempatParkir.Count(tempat => tempat.Kendaraan != null && tempat.Kendaraan.Tipe == tipe);
        }

        // Metode untuk mendapatkan nomor registrasi kendaraan berdasarkan warna
        public List<string> NomorRegistrasiBerdasarkanWarna(string warna)
        {
            return tempatParkir.Where(tempat => tempat.Kendaraan != null && tempat.Kendaraan.Warna.Equals(warna, StringComparison.OrdinalIgnoreCase))
                               .Select(tempat => tempat.Kendaraan.NomorRegistrasi)
                               .ToList();
        }

        // Metode untuk mendapatkan nomor slot kendaraan berdasarkan warna
        public List<int> NomorSlotBerdasarkanWarna(string warna)
        {
            return tempatParkir.Where(tempat => tempat.Kendaraan != null && tempat.Kendaraan.Warna.Equals(warna, StringComparison.OrdinalIgnoreCase))
                               .Select(tempat => tempat.Nomor)
                               .ToList();
        }

        // Metode untuk mendapatkan nomor slot berdasarkan nomor registrasi kendaraan
        public int NomorSlotBerdasarkanNomorRegistrasi(string nomorRegistrasi)
        {
            var tempat = tempatParkir.FirstOrDefault(tempat => tempat.Kendaraan != null && tempat.Kendaraan.NomorRegistrasi.Equals(nomorRegistrasi, StringComparison.OrdinalIgnoreCase));
            return tempat != null ? tempat.Nomor : -1;
        }
    }

    public class Program
    {
        static AreaParkir areaParkir;

        static void Main(string[] args)
        {
            Console.WriteLine("Selamat datang di Sistem Parkir!");
            areaParkir = null;

            while (true)
            {
                Console.WriteLine("\nSilakan masukkan perintah Anda:");
                string perintah = Console.ReadLine();
                string[] tokens = perintah.Split(' ');

                switch (tokens[0].ToLower())
                {
                    case "buat_area_parkir":
                        BuatAreaParkir(int.Parse(tokens[1]));
                        break;
                    case "masuk":
                        KendaraanMasuk(tokens[1], tokens[2], tokens[3]);
                        break;
                    case "keluar":
                        KendaraanKeluar(int.Parse(tokens[1]));
                        break;
                    case "status":
                        CekStatusParkir();
                        break;
                    case "jumlah_slot_tersedia":
                        CekJumlahSlotTersedia();
                        break;
                    case "jumlah_slot_terisi":
                        CekJumlahSlotTerisi();
                        break;
                    case "jumlah_kendaraan_berdasarkan_tipe":
                        CekJumlahKendaraanBerdasarkanTipe(tokens[1]);
                        break;
                    case "nomor_registrasi_berdasarkan_warna":
                        CekNomorRegistrasiBerdasarkanWarna(tokens[1]);
                        break;
                    case "nomor_slot_berdasarkan_warna":
                        CekNomorSlotBerdasarkanWarna(tokens[1]);
                        break;
                    case "nomor_slot_berdasarkan_nomor_registrasi":
                        CekNomorSlotBerdasarkanNomorRegistrasi(tokens[1]);
                        break;
                    case "exit": // Mengganti "keluar" dengan "exit"
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Perintah tidak valid.");
                        break;
                }
            }
        }

        static void BuatAreaParkir(int kapasitas)
        {
            areaParkir = new AreaParkir(kapasitas);
            Console.WriteLine($"Area parkir telah dibuat dengan {kapasitas} slot.");
        }

        static void KendaraanMasuk(string nomorRegistrasi, string warna, string tipe)
        {
            if (areaParkir == null)
            {
                Console.WriteLine("Harap buat area parkir terlebih dahulu.");
                return;
            }

            var jenisKendaraan = tipe.ToLower() == "mobil" ? JenisKendaraan.Mobil : JenisKendaraan.Motor;
            var hasil = areaParkir.Masuk(nomorRegistrasi, warna, jenisKendaraan);
            Console.WriteLine(hasil);
        }

        static void KendaraanKeluar(int nomorSlot)
        {
            if (areaParkir == null)
            {
                Console.WriteLine("Harap buat area parkir terlebih dahulu.");
                return;
            }

            var hasil = areaParkir.Keluar(nomorSlot);
            Console.WriteLine(hasil);
        }

        static void CekStatusParkir()
        {
            if (areaParkir == null)
            {
                Console.WriteLine("Harap buat area parkir terlebih dahulu.");
                return;
            }

            var status = areaParkir.StatusParkir();
            Console.WriteLine(status);
        }

        static void CekJumlahSlotTersedia()
        {
            if (areaParkir == null)
            {
                Console.WriteLine("Harap buat area parkir terlebih dahulu.");
                return;
            }

            var jumlahTersedia = areaParkir.JumlahSlotTersedia();
            Console.WriteLine($"Jumlah slot tersedia: {jumlahTersedia}");
        }

        static void CekJumlahSlotTerisi()
        {
            if (areaParkir == null)
            {
                Console.WriteLine("Harap buat area parkir terlebih dahulu.");
                return;
            }

            var jumlahTerisi = areaParkir.JumlahSlotTerisi();
            Console.WriteLine($"Jumlah slot terisi: {jumlahTerisi}");
        }

        static void CekJumlahKendaraanBerdasarkanTipe(string tipe)
        {
            if (areaParkir == null)
            {
                Console.WriteLine("Harap buat area parkir terlebih dahulu.");
                return;
            }

            var jenisKendaraan = tipe.ToLower() == "mobil" ? JenisKendaraan.Mobil : JenisKendaraan.Motor;
            var jumlahKendaraan = areaParkir.JumlahKendaraanBerdasarkanTipe(jenisKendaraan);
            Console.WriteLine($"Jumlah kendaraan tipe {tipe}: {jumlahKendaraan}");
        }

        static void CekNomorRegistrasiBerdasarkanWarna(string warna)
        {
            if (areaParkir == null)
            {
                Console.WriteLine("Harap buat area parkir terlebih dahulu.");
                return;
            }

            var nomorRegistrasi = areaParkir.NomorRegistrasiBerdasarkanWarna(warna);
            Console.WriteLine("Nomor registrasi kendaraan dengan warna {0}: {1}", warna, string.Join(", ", nomorRegistrasi));
        }

        static void CekNomorSlotBerdasarkanWarna(string warna)
        {
            if (areaParkir == null)
            {
                Console.WriteLine("Harap buat area parkir terlebih dahulu.");
                return;
            }

            var nomorSlot = areaParkir.NomorSlotBerdasarkanWarna(warna);
            Console.WriteLine("Nomor slot kendaraan dengan warna {0}: {1}", warna, string.Join(", ", nomorSlot));
        }

        static void CekNomorSlotBerdasarkanNomorRegistrasi(string nomorRegistrasi)
        {
            if (areaParkir == null)
            {
                Console.WriteLine("Harap buat area parkir terlebih dahulu.");
                return;
            }

            var nomorSlot = areaParkir.NomorSlotBerdasarkanNomorRegistrasi(nomorRegistrasi);
            Console.WriteLine(nomorSlot != -1 ? $"Nomor slot kendaraan dengan nomor registrasi {nomorRegistrasi}: {nomorSlot}" : "Kendaraan tidak ditemukan.");
        }
    }
}
