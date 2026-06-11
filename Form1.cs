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

            // --- PRVI KLIK: Selektovanje figure i obeležavanje mogućih poteza ---
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

                // Selektuj figuru
                dugme = kliknutoDugme;
                ind = kPoljeIndeks;
                kliknutoDugme.BackColor = Color.Cyan; // Obeleži selektovano polje plavom bojom

                // --- OVDE OBELEŽAVAMO MOGUĆA POLJA ---
                ObeleziMogucePoteze(ind, boardf[kRed, kKol], potez);
            }
            // --- DRUGI KLIK: Pokušaj pomeranja figure ---
            else
            {
                int pRed = ind / 8;
                int pKol = ind % 8;

                string figura = boardf[pRed, pKol];

                // Dodatna provera: Ako klikneš ponovo na isto polje ili na svoju drugu figuru, samo resetuj/promeni selekciju
                if (boardc[kRed, kKol] == boardc[pRed, pKol])
                {
                    ResetujSelekciju();
                    // Ako je kliknuo na drugu svoju figuru (umesto da poništi), odmah je selektuj
                    if (kPoljeIndeks != ind)
                    {
                        Button_Click(sender, e);
                    }
                    return;
                }

                // Provera da li je potez validan geometrijski + put prazan
                bool validanPotez = isPossible(ind, kPoljeIndeks, figura, potez);
                if (validanPotez && (figura == "R" || figura == "B" || figura == "Q"))
                {
                    validanPotez = Put(ind, kPoljeIndeks);
                }
                // Posebna provera za pešaka (ne može da jede pravo, mora prazno polje)
                if (validanPotez && figura == "P" && pKol == kKol && !string.IsNullOrEmpty(boardf[kRed, kKol]))
                {
                    validanPotez = false;
                }
                // Posebna provera za pešaka kad jede (mora biti protivnička figura dijagonalno)
                if (validanPotez && figura == "P" && Math.Abs(pKol - kKol) == 1 && string.IsNullOrEmpty(boardf[kRed, kKol]))
                {
                    validanPotez = false;
                }

                if (validanPotez)
                {
                    // --- IZVRŠI POTEZ U MATRICAMA ---
                    boardf[kRed, kKol] = boardf[pRed, pKol];
                    boardc[kRed, kKol] = boardc[pRed, pKol];

                    boardf[pRed, pKol] = null;
                    boardc[pRed, pKol] = null;

                    // --- OSVEŽI GRAFIKU NA EKRANU ---
                    board[kRed, kKol].Text = boardf[kRed, kKol];
                    board[pRed, pKol].Text = "";

                    // Provera šaha nakon odigranog poteza
                    if (potez && IsKingInCheck("B")) MessageBox.Show("CRNI, ŠAH!");
                    if (!potez && IsKingInCheck("W")) MessageBox.Show("BELI, ŠAH!");

                    // Promeni ko je na potezu
                    potez = !potez;
                }

                // Resetuj selekciju i očisti zelenu boju sa table
                ResetujSelekciju();
            }
        }
        void ObeleziMogucePoteze(int pocetnoPolje, string figura, bool bojaIgraca)
        {
            string oznakaBoje = bojaIgraca ? "W" : "B";

            for (int i = 0; i < 64; i++)
            {
                // Preskoči samo sebe (početno polje)
                if (i == pocetnoPolje) continue;

                int ciljRed = i / 8;
                int ciljKol = i % 8;

                // 1. Ne možeš da staneš na polje gde je već tvoja figura
                if (boardc[ciljRed, ciljKol] == oznakaBoje) continue;

                // 2. Proveri osnovnu geometriju kretanja
                if (isPossible(pocetnoPolje, i, figura, bojaIgraca))
                {
                    // 3. Za figure koje ne skaču, proveri da li je put čist
                    if (figura == "R" || figura == "B" || figura == "Q")
                    {
                        if (!Put(pocetnoPolje, i)) continue;
                    }

                    // 4. Specifična pravila za pešaka (kretanje napred vs jedenje sa strane)
                    if (figura == "P")
                    {
                        int pKol = pocetnoPolje % 8;
                        // Ako ide pravo, polje mora biti prazno
                        if (pKol == ciljKol && !string.IsNullOrEmpty(boardf[ciljRed, ciljKol])) continue;
                        // Ako ide ukoso (jede), polje MORA imati protivničku figuru
                        if (Math.Abs(pKol - ciljKol) == 1 && string.IsNullOrEmpty(boardf[ciljRed, ciljKol])) continue;
                    }

                    // Ako je polje prošlo sve filtere, obeleži ga kao moguće!
                    board[ciljRed, ciljKol].BackColor = Color.LightGreen;
                }
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
        bool Put(int ppolje, int kpolje)
        {
            int pRed = ppolje / 8;
            int pKol = ppolje % 8;
            int kRed = kpolje / 8;
            int kKol = kpolje % 8;

            int korakRed = Math.Sign(kRed - pRed); // Vraća 1, -1 ili 0
            int korakKol = Math.Sign(kKol - pKol); // Vraća 1, -1 ili 0

            int trenutniRed = pRed + korakRed;
            int trenutniKol = pKol + korakKol;

            // Idemo polje po polje od početka do cilja (ne računajući samo ciljno polje)
            while (trenutniRed != kRed || trenutniKol != kKol)
            {
                if (!string.IsNullOrEmpty(boardf[trenutniRed, trenutniKol]))
                {
                    return false; // Našli smo figuru na putu, put je blokiran!
                }
                trenutniRed += korakRed;
                trenutniKol += korakKol;
            }

            return true; // Put je čist
        }
        bool IsKingInCheck(string bojaKralja)
        {
            int kraljIndeks = -1;

            // 1. Nađi poziciju kralja tražene boje na tabli
            for (int i = 0; i < 64; i++)
            {
                int r = i / 8;
                int c = i % 8;
                if (boardf[r, c] == "K" && boardc[r, c] == bojaKralja)
                {
                    kraljIndeks = i;
                    break;
                }
            }

            // Ako kralj uopšte nije nađen (što ne bi smelo da se desi), vrati false
            if (kraljIndeks == -1) return false;

            // 2. Prođi kroz sva polja na tabli i traži protivničke figure
            for (int i = 0; i < 64; i++)
            {
                int r = i / 8;
                int c = i % 8;

                // Ako polje nije prazno i na njemu je protivnička figura
                if (!string.IsNullOrEmpty(boardf[r, c]) && boardc[r, c] != bojaKralja)
                {
                    string protivnickaFigura = boardf[r, c];
                    bool protivnickaBojaBool = (boardc[r, c] == "W"); // isPossible traži bool za potez

                    // Proveri da li ta figura geometrijski može da napadne kralja
                    if (isPossible(i, kraljIndeks, protivnickaFigura, protivnickaBojaBool))
                    {
                        // Za figure koje ne skaču (Top, Lovac, Dama), moramo proveriti i da li im je put čist
                        if (protivnickaFigura == "R" || protivnickaFigura == "B" || protivnickaFigura == "Q")
                        {
                            if (Put(i, kraljIndeks))
                            {
                                return true; // Put je čist, kralj je napadnut! -> ŠAH
                            }
                        }
                        // Za skakača (N) i pešaka (P) put ne mora biti prazan
                        else if (protivnickaFigura == "N")
                        {
                            return true; // Skakač preskače, kralj je napadnut! -> ŠAH
                        }
                        else if (protivnickaFigura == "P")
                        {
                            // Kod pešaka tvoja funkcija isPossible već pokriva dijagonalno kretanje,
                            // ali u pravom šahu pešak napada kralja SAMO dijagonalno.
                            // Pošto proveravamo napad, gledamo samo ako je promena kolone jednaka 1
                            int pKol = i % 8;
                            int kKol = kraljIndeks % 8;
                            if (Math.Abs(pKol - kKol) == 1)
                            {
                                return true; // Pešak napada kralja dijagonalno -> ŠAH
                            }
                        }
                    }
                }
            }
            return false; // Nijedna figura ne napada kralja
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

    }
}