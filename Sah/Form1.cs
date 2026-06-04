using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sah
{
    public partial class Form1 : Form
    {

        Button[,] board = new Button[8, 8];
        string[,] boardf = new string[8, 8];
        string[,] boardc = new string[8, 8];
        Dictionary<string, Image> figure = new Dictionary<string, Image>(); 

        public Form1()
        {
            InitializeComponent();

            this.Width = 820;
            this.Height = 840;
            UcitajSlike();
            CreateBoard();
        }

        void CreateBoard()
        {
            SetupBoard();
            MessageBox.Show(GetKljuc(0, 0)); // treba da pise "top"
            int size = 80;
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Button btn = new Button();
                    btn.Width = size;
                    btn.Height = size;
                    btn.Left = col * size;
                    btn.Top = row * size;
                    if ((row + col) % 2 == 0)
                        btn.BackColor = Color.Beige;
                    else
                        btn.BackColor = Color.Brown;

                    btn.Text = "";
                    string kljuc = GetKljuc(row, col);
                    if (kljuc != null && figure.ContainsKey(kljuc))
                    {
                        btn.Image = figure[kljuc];
                        btn.ImageAlign = ContentAlignment.MiddleCenter;
                    }

                    this.Controls.Add(btn);
                    board[row, col] = btn;
                }
            }
        }

        void SetupBoard()
        {
            // ----- Crne figure -----
            boardf[0, 0] = "R";
            boardf[0, 1] = "N";
            boardf[0, 2] = "B";
            boardf[0, 3] = "Q";
            boardf[0, 4] = "K";
            boardf[0, 5] = "B";
            boardf[0, 6] = "N";
            boardf[0, 7] = "R";
            for (int i = 0; i < 8; i++)
            {
                boardf[1, i] = "P";
            }
            // Boje crnih
            for (int row = 0; row <= 1; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    boardc[row, col] = "B";
                }
            }
            // ----- Bele figure -----
            boardf[7, 0] = "R";
            boardf[7, 1] = "N";
            boardf[7, 2] = "B";
            boardf[7, 3] = "Q";
            boardf[7, 4] = "K";
            boardf[7, 5] = "B";
            boardf[7, 6] = "N";
            boardf[7, 7] = "R";
            for (int i = 0; i < 8; i++)
            {
                boardf[6, i] = "P";
            }
            // Boje belih
            for (int row = 6; row <= 7; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    boardc[row, col] = "W";
                }
            }
        }
        void UcitajSlike()
        {
            string[] nazivi = { "kralj", "kraljica", "top", "lovac", "konj", "pijun" };

            foreach (string naziv in nazivi)
            {
                string putanja = System.IO.Path.Combine(Application.StartupPath, "images", naziv + ".png");
                if (System.IO.File.Exists(putanja))
                    figure[naziv] = Image.FromFile(putanja);
                else
                    MessageBox.Show("Ne postoji: " + putanja);
            }
        }

        string GetKljuc(int row, int col)
        {
            if (boardf[row, col] == null) return null;

            switch (boardf[row, col])
            {
                case "K": return "kralj";
                case "Q": return "kraljica";
                case "R": return "top";
                case "B": return "lovac";
                case "N": return "konj";
                case "P": return "pijun";
                default: return null;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
