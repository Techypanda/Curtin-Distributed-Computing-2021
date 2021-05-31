using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDLL {
    internal class DatabaseGenerator {
        private Random rnd;
        private List<Bitmap> icons;
        public DatabaseGenerator() {
            icons = new List<Bitmap>();
            rnd = new Random();
            Bitmap Bmp = new Bitmap(64, 64);
            icons.Add(GenerateColorImage(new Bitmap(Bmp), 255, 0, 0));
            icons.Add(GenerateColorImage(new Bitmap(Bmp), 0, 255, 0));
            icons.Add(GenerateColorImage(new Bitmap(Bmp), 0, 0, 255));
            icons.Add(GenerateColorImage(new Bitmap(Bmp), 255, 255, 255));
        }
        public void GetNextAccount(out uint pin, out uint acctNo, out string firstName, out string lastName, out int balance, out Bitmap bitmap) {
            pin = GetPIN();
            acctNo = GetAcctNo();
            firstName = GetFirstname();
            lastName = GetLastname();
            balance = GetBalance();
            bitmap = GetBitmap();
        }
        private string GetFirstname() { 
            return GenerateFirstName();
        }
        private Bitmap GenerateColorImage(Bitmap bmp, int red, int blue, int green)
        { // https://stackoverflow.com/questions/1720160/how-do-i-fill-a-bitmap-with-a-solid-color
            using (Graphics gfx = Graphics.FromImage(bmp))
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(red, blue, green)))
            {
                gfx.FillRectangle(brush, 0, 0, 64, 64);
            }
            return bmp;
        }
        public Bitmap GetBitmap()
        { // https://stackoverflow.com/questions/1720160/how-do-i-fill-a-bitmap-with-a-solid-color
            return icons[rnd.Next(0, icons.Count)];
        }
        private string GetLastname() { 
            return GenerateLastName();
        }
        private uint GetPIN() { // 4 - 12 digits
            return (uint)rnd.Next(4, 13);
        }
        private uint GetAcctNo() { // 8 - 12 digits
            return (uint)rnd.Next(8, 13);
        }
        private int GetBalance() { // 0 - 999999$
            return rnd.Next(0, 100000);
        }
        private string GenerateFirstName() {
            string[] firstnames = { "Kevin", "Curtis", "Faraz", "Anurag", "Sam", "Antoni", "Sajib" };
            string fn = firstnames[rnd.Next(0, firstnames.Length)];
            return fn;
        }
        private string GenerateLastName()
        {
            string[] lastnames = { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis" };
            string ln = lastnames[rnd.Next(0, lastnames.Length)];
            return ln;
        }
    }
}
