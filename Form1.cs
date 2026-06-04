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
        bool potez = true;
        Button dugme = null;
        int ind = -1;
        public Form1()
        {
            InitializeComponent();

            this.Width = 820;
            this.Height = 840;

            CreateBoard();
        }

        void CreateBoard()
        {
            SetupBoard();
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
                    int indeksPolja = row * 8 + col;
                    btn.Tag = indeksPolja; // Pakujemo indeks u Tag

                    // Povezujemo klik event
                    btn.Click += Button_Click;
                    this.Controls.Add(btn);
                    btn.Font = new Font("Arial", 24, FontStyle.Bold);  
                    board[row, col] = btn;
                    board[row, col].Text = boardf[row, col];
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
        bool isPossible(int ppolje, int kpolje, string figura, bool potez)
        {
            int pRed = ppolje / 8;
            int pKol = ppolje % 8;

            int kRed = kpolje / 8;
            int kKol = kpolje % 8;

            int dr = Math.Abs(pRed - kRed);
            int dk = Math.Abs(pKol - kKol);

            switch (figura)
            {
                case "R": // Top
                    return pRed == kRed || pKol == kKol;

                case "B": // Lovac
                    return dr == dk;

                case "Q": // Dama
                    return pRed == kRed || pKol == kKol || dr == dk;

                case "N": // Konj
                    return (dr == 2 && dk == 1) || (dr == 1 && dk == 2);

                case "K": // Kralj
                    return dr <= 1 && dk <= 1 && (dr + dk > 0);

                case "P": // Pešak
                    if (potez) // beli
                    {
                        return (kKol == pKol && kRed == pRed - 1) ||
                               (kKol == pKol && pRed == 6 && kRed == pRed - 2) ||
                               (Math.Abs(kKol - pKol) == 1 && kRed == pRed - 1);
                    }
                    else // crni
                    {
                        return (kKol == pKol && kRed == pRed + 1) ||
                               (kKol == pKol && pRed == 1 && kRed == pRed + 2) ||
                               (Math.Abs(kKol - pKol) == 1 && kRed == pRed + 1);
                    }
            }
            return false;
        }
        private void Button_Click(object sender, EventArgs e)
        {
            Button kliknutoDugme = (Button)sender;
            int kPoljeIndeks = (int)kliknutoDugme.Tag; // Indeks polja na koje je kliknuto (0-63)

            int kRed = kPoljeIndeks / 8;
            int kKol = kPoljeIndeks % 8;

            // --- PRVI KLIK: Selektovanje figure ---
            if (dugme == null)
            {
                // Provera da li na polju uopšte ima figura
                if (string.IsNullOrEmpty(boardf[kRed, kKol])) return;

                // Provera da li igrač klikće na svoju figuru
                string bojaFigure = boardc[kRed, kKol];
                if ((potez && bojaFigure != "W") || (!potez && bojaFigure != "B"))
                {
                    MessageBox.Show("Nije tvoj red, brate!");
                    return;
                }

                // Ako je sve kul, selektuj figuru
                dugme = kliknutoDugme;
                ind = kPoljeIndeks;
                kliknutoDugme.BackColor = Color.Cyan; // Obeleži selektovano polje plavom bojom
            }
            // --- DRUGI KLIK: Pokušaj pomeranja figure ---
            else
            {
                int pRed = ind / 8;
                int pKol = ind % 8;

                string figura = boardf[pRed, pKol];

                // 1. Provera preko tvoje isPossible funkcije
                if (isPossible(ind, kPoljeIndeks, figura, potez))
                {
                    // Dodatna provera: Ne možeš da pojedeš svoju figuru
                    if (boardc[kRed, kKol] == boardc[pRed, pKol])
                    {
                        ResetujSelekciju();
                        return;
                    }

                    // --- IZVRŠI POTEZ U MATRICAMA ---
                    boardf[kRed, kKol] = boardf[pRed, pKol]; // Pomeri figuru na novo mesto
                    boardc[kRed, kKol] = boardc[pRed, pKol]; // Pomeri boju na novo mesto

                    boardf[pRed, pKol] = null; // Staro polje ostaje prazno
                    boardc[pRed, pKol] = null;

                    // --- OSVEŽI GRAFIKU NA EKRANU ---
                    board[kRed, kKol].Text = boardf[kRed, kKol];
                    board[pRed, pKol].Text = "";

                    // Promeni ko je na potezu
                    potez = !potez;
                }

                // Bez obzira da li je potez uspeo ili ne, resetuj selekciju i vrati boje tabli
                ResetujSelekciju();
            }
        }

        // Pomoćna funkcija koja vraća fabričke boje tabli nakon poteza
        private void ResetujSelekciju()
        {
            dugme = null;
            ind = -1;

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if ((row + col) % 2 == 0)
                        board[row, col].BackColor = Color.Beige;
                    else
                        board[row, col].BackColor = Color.Brown;
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

    }   
}
