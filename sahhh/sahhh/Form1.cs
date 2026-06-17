using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
// Obavezno proveri da li tvoj projekat ima pristup Properties.Resources

namespace sahhh
{
    public partial class Form1 : Form
    {
        Button[,] board = new Button[8, 8];
        string[,] boardf = new string[8, 8];
        string[,] boardc = new string[8, 8];
        bool potez = true; // true = beli, false = crni
        Button dugme = null;
        int ind = -1;

        // --- REZULTAT ---
        double resultadoBeli = 0;
        double rezultatCrni = 0;

        // --- PROMENLJIVE ZA ROKADU ---
        bool beliKraljSePomerio = false;
        bool crniKraljSePomerio = false;
        bool leviBeliTopSePomerio = false;
        bool desniBeliTopSePomerio = false;
        bool leviCrniTopSePomerio = false;
        bool desniCrniTopSePomerio = false;

        // --- PROMENLJIVA ZA EN PASSANT ---
        int enPassantCiljKolona = -1;

        // --- DUGME ZA REMI ---
        Button btnRemi;

        public Form1()
        {
            InitializeComponent();

            this.Width = 970;
            this.Height = 840;

            OsveziNaslovProzora();
            CreateBoard();
            KreirajDugmeZaRemi();
        }

        void OsveziNaslovProzora()
        {
            this.Text = string.Format("Šah | Rezultat - Beli: {0} | Crni: {1}", resultadoBeli, rezultatCrni);
        }

        void KreirajDugmeZaRemi()
        {
            btnRemi = new Button();
            btnRemi.Width = 120;
            btnRemi.Height = 50;
            btnRemi.Left = 810;
            btnRemi.Top = 300;
            btnRemi.Text = "Ponudi remi";
            btnRemi.Font = new Font("Arial", 12, FontStyle.Bold);
            btnRemi.BackColor = Color.LightGray;
            btnRemi.ForeColor = Color.Black;
            btnRemi.Click += BtnRemi_Click;
            this.Controls.Add(btnRemi);
        }

        private void BtnRemi_Click(object sender, EventArgs e)
        {
            string koNudi = potez ? "BELI" : "CRNI";
            string koPrima = potez ? "CRNOM" : "BELOM";

            string tekstPoruke = string.Format("Igrač ({0}) nudi remi igraču ({1}).\n\nDa li prihvatate nerešen rezultat?", koNudi, koPrima);

            DialogResult odgovor = MessageBox.Show(tekstPoruke, "Ponuda za remi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (odgovor == DialogResult.Yes)
            {
                resultadoBeli += 0.5;
                rezultatCrni += 0.5;
                OsveziNaslovProzora();

                MessageBox.Show("Partija je završena remijem (nerešeno)! Svakome po pola poena.", "Remi");
                ResetujCeoMec();
            }
            else
            {
                string odbijenoTekst = string.Format("Igrač ({0}) je odbio remi. Igra se nastavlja!", koPrima);
                MessageBox.Show(odbijenoTekst, "Odbijeno");
            }
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
                        btn.BackColor = Color.FromArgb(240, 241, 214);
                    else
                        btn.BackColor = Color.FromArgb(118, 150, 86);

                    int indeksPolja = row * 8 + col;
                    btn.Tag = indeksPolja;

                    btn.Click += Button_Click;
                    this.Controls.Add(btn);

                    board[row, col] = btn;

                    // NOVO: Umesto teksta, dodeljujemo PNG sliku iz resursa
                    PostaviSlikuFigure(row, col);
                }
            }
        }

        // NOVO: Funkcija koja mapira slova/boje u PNG grafiku iz Resursa
        void PostaviSlikuFigure(int red, int kol)
        {
            string figura = boardf[red, kol];
            string boja = boardc[red, kol];

            if (string.IsNullOrEmpty(figura))
            {
                board[red, kol].Image = null;
                return;
            }

            if (boja == "W")
            {
                switch (figura)
                {
                    case "R": board[red, kol].Image = Properties.Resources.w_rook; break;
                    case "N": board[red, kol].Image = Properties.Resources.w_knight; break;
                    case "B": board[red, kol].Image = Properties.Resources.w_bishop; break;
                    case "Q": board[red, kol].Image = Properties.Resources.w_queen; break;
                    case "K": board[red, kol].Image = Properties.Resources.w_king; break;
                    case "P": board[red, kol].Image = Properties.Resources.w_pawn; break;
                }
            }
            else if (boja == "B")
            {
                switch (figura)
                {
                    case "R": board[red, kol].Image = Properties.Resources.b_rook; break;
                    case "N": board[red, kol].Image = Properties.Resources.b_knight; break;
                    case "B": board[red, kol].Image = Properties.Resources.b_bishop; break;
                    case "Q": board[red, kol].Image = Properties.Resources.b_queen; break;
                    case "K": board[red, kol].Image = Properties.Resources.b_king; break;
                    case "P": board[red, kol].Image = Properties.Resources.b_pawn; break;
                }
            }
        }

        void SetupBoard()
        {
            // PROMENJENO: Sada interna matrica odmah čuva jednostavna slova (R, N, B, Q, K, P)
            // ----- Crne figure -----
            boardf[0, 0] = "R"; boardf[0, 1] = "N"; boardf[0, 2] = "B"; boardf[0, 3] = "Q";
            boardf[0, 4] = "K"; boardf[0, 5] = "B"; boardf[0, 6] = "N"; boardf[0, 7] = "R";
            for (int i = 0; i < 8; i++) boardf[1, i] = "P";
            for (int row = 0; row <= 1; row++)
                for (int col = 0; col < 8; col++) boardc[row, col] = "B";

            // ----- Bele figure -----
            boardf[7, 0] = "R"; boardf[7, 1] = "N"; boardf[7, 2] = "B"; boardf[7, 3] = "Q";
            boardf[7, 4] = "K"; boardf[7, 5] = "B"; boardf[7, 6] = "N"; boardf[7, 7] = "R";
            for (int i = 0; i < 8; i++) boardf[6, i] = "P";
            for (int row = 6; row <= 7; row++)
                for (int col = 0; col < 8; col++) boardc[row, col] = "W";
        }

        // POJEDNOSTAVLJENO: Više ne moramo da konvertujemo Unikod simbole, radimo direktno sa slovima
        string KonvertujSimbolUSlovo(string simbol)
        {
            return simbol ?? "";
        }

        bool isPossible(int ppolje, int kpolje, string simbolFigure, bool trenutniPotez)
        {
            string figura = simbolFigure;

            int pRed = ppolje / 8;
            int pKol = ppolje % 8;
            int kRed = kpolje / 8;
            int kKol = kpolje % 8;

            int dr = Math.Abs(pRed - kRed);
            int dk = Math.Abs(pKol - kKol);

            string protivnickaBoja = trenutniPotez ? "B" : "W";

            switch (figura)
            {
                case "R":
                    return pRed == kRed || pKol == kKol;

                case "B":
                    return dr == dk;

                case "Q":
                    return pRed == kRed || pKol == kKol || dr == dk;

                case "N":
                    return (dr == 2 && dk == 1) || (dr == 1 && dk == 2);

                case "K":
                    if (dr <= 1 && dk <= 1 && (dr + dk > 0)) return true;

                    if (dr == 0 && dk == 2)
                    {
                        if (trenutniPotez)
                        {
                            if (beliKraljSePomerio || IsKingInCheck("W")) return false;
                            if (kKol == 6 && !desniBeliTopSePomerio)
                                return string.IsNullOrEmpty(boardf[7, 5]) && string.IsNullOrEmpty(boardf[7, 6]) && DaLiJePoljeBezbedno(7, 5, "W") && DaLiJePoljeBezbedno(7, 6, "W");
                            if (kKol == 2 && !leviBeliTopSePomerio)
                                return string.IsNullOrEmpty(boardf[7, 1]) && string.IsNullOrEmpty(boardf[7, 2]) && string.IsNullOrEmpty(boardf[7, 3]) && DaLiJePoljeBezbedno(7, 3, "W") && DaLiJePoljeBezbedno(7, 2, "W");
                        }
                        else
                        {
                            if (crniKraljSePomerio || IsKingInCheck("B")) return false;
                            if (kKol == 6 && !desniCrniTopSePomerio)
                                return string.IsNullOrEmpty(boardf[0, 5]) && string.IsNullOrEmpty(boardf[0, 6]) && DaLiJePoljeBezbedno(0, 5, "B") && DaLiJePoljeBezbedno(0, 6, "B");
                            if (kKol == 2 && !leviCrniTopSePomerio)
                                return string.IsNullOrEmpty(boardf[0, 1]) && string.IsNullOrEmpty(boardf[0, 2]) && string.IsNullOrEmpty(boardf[0, 3]) && DaLiJePoljeBezbedno(0, 3, "B") && DaLiJePoljeBezbedno(0, 2, "B");
                        }
                    }
                    return false;

                case "P":
                    if (trenutniPotez)
                    {
                        if (kKol == pKol && kRed == pRed - 1 && string.IsNullOrEmpty(boardf[kRed, kKol])) return true;
                        if (kKol == pKol && pRed == 6 && kRed == 4 && string.IsNullOrEmpty(boardf[4, kKol]) && string.IsNullOrEmpty(boardf[5, kKol])) return true;
                        if (Math.Abs(kKol - pKol) == 1 && kRed == pRed - 1 && boardc[kRed, kKol] == protivnickaBoja) return true;
                        if (pRed == 3 && kRed == 2 && Math.Abs(kKol - pKol) == 1 && enPassantCiljKolona != -1 && enPassantCiljKolona == kKol && string.IsNullOrEmpty(boardf[kRed, kKol])) return true;
                    }
                    else
                    {
                        if (kKol == pKol && kRed == pRed + 1 && string.IsNullOrEmpty(boardf[kRed, kKol])) return true;
                        if (kKol == pKol && pRed == 1 && kRed == 3 && string.IsNullOrEmpty(boardf[3, kKol]) && string.IsNullOrEmpty(boardf[2, kKol])) return true;
                        if (Math.Abs(kKol - pKol) == 1 && kRed == pRed + 1 && boardc[kRed, kKol] == protivnickaBoja) return true;
                        if (pRed == 4 && kRed == 5 && Math.Abs(kKol - pKol) == 1 && enPassantCiljKolona != -1 && enPassantCiljKolona == kKol && string.IsNullOrEmpty(boardf[kRed, kKol])) return true;
                    }
                    return false;
            }
            return false;
        }

        bool DaLiJePoljeBezbedno(int red, int kol, string mojaBoja)
        {
            string staraFigura = boardf[red, kol]; string staraBoja = boardc[red, kol];
            boardf[red, kol] = "K"; boardc[red, kol] = mojaBoja;
            bool uSahu = IsKingInCheck(mojaBoja);
            boardf[red, kol] = staraFigura; boardc[red, kol] = staraBoja;
            return !uSahu;
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button kliknutoDugme = (Button)sender;
            int kPoljeIndeks = (int)kliknutoDugme.Tag;

            int kRed = kPoljeIndeks / 8;
            int kKol = kPoljeIndeks % 8;

            if (dugme == null)
            {
                if (string.IsNullOrEmpty(boardf[kRed, kKol])) return;

                string bojaFigure = boardc[kRed, kKol];
                if ((potez && bojaFigure != "W") || (!potez && bojaFigure != "B"))
                {
                    MessageBox.Show("Nije tvoj red!");
                    return;
                }

                dugme = kliknutoDugme;
                ind = kPoljeIndeks;
                kliknutoDugme.BackColor = Color.Cyan;

                ObeleziMogucePoteze(ind, boardf[kRed, kKol], potez);
            }
            else
            {
                int pRed = ind / 8;
                int pKol = ind % 8;
                string simbolFigure = boardf[pRed, pKol];
                string figura = simbolFigure;

                if (boardc[kRed, kKol] == boardc[pRed, pKol])
                {
                    ResetujSelekciju();
                    if (kPoljeIndeks != ind) Button_Click(sender, e);
                    return;
                }

                bool validanPotez = isPossible(ind, kPoljeIndeks, simbolFigure, potez);
                if (validanPotez && (figura == "R" || figura == "B" || figura == "Q"))
                {
                    validanPotez = Put(ind, kPoljeIndeks);
                }

                if (validanPotez)
                {
                    string staraCiljFigura = boardf[kRed, kKol];
                    string staraCiljBoja = boardc[kRed, kKol];

                    bool jeEnPassant = (figura == "P" && Math.Abs(pKol - kKol) == 1 && string.IsNullOrEmpty(staraCiljFigura));
                    string epFigura = null; string epBoja = null;
                    if (jeEnPassant)
                    {
                        epFigura = boardf[pRed, kKol]; epBoja = boardc[pRed, kKol];
                        boardf[pRed, kKol] = null; boardc[pRed, kKol] = null;
                    }

                    boardf[kRed, kKol] = boardf[pRed, pKol];
                    boardc[kRed, kKol] = boardc[pRed, pKol];
                    boardf[pRed, pKol] = null;
                    boardc[pRed, pKol] = null;

                    string mojaBoja = potez ? "W" : "B";
                    bool kraljUgrozen = IsKingInCheck(mojaBoja);

                    boardf[pRed, pKol] = boardf[kRed, kKol];
                    boardc[pRed, pKol] = boardc[kRed, kKol];
                    boardf[kRed, kKol] = staraCiljFigura;
                    boardc[kRed, kKol] = staraCiljBoja;
                    if (jeEnPassant)
                    {
                        boardf[pRed, kKol] = epFigura; boardc[pRed, kKol] = epBoja;
                    }

                    if (kraljUgrozen)
                    {
                        MessageBox.Show("Moraš da odbraniš kralja! Potez nije legalan.");
                        ResetujSelekciju();
                        return;
                    }

                    if (jeEnPassant)
                    {
                        boardf[pRed, kKol] = null;
                        boardc[pRed, kKol] = null;
                        board[pRed, kKol].Image = null; // NOVO: Čistimo grafiku
                    }

                    if (figura == "K" && Math.Abs(pKol - kKol) == 2)
                    {
                        int tIzvorK = (kKol == 6) ? 7 : 0;
                        int tCiljK = (kKol == 6) ? 5 : 3;
                        boardf[kRed, tCiljK] = boardf[kRed, tIzvorK];
                        boardc[kRed, tCiljK] = boardc[kRed, tIzvorK];
                        boardf[kRed, tIzvorK] = null;
                        boardc[kRed, tIzvorK] = null;

                        PostaviSlikuFigure(kRed, tCiljK);    // NOVO: Osveži sliku topa
                        board[kRed, tIzvorK].Image = null;  // NOVO: Briši staru poziciju topa
                    }

                    boardf[kRed, kKol] = boardf[pRed, pKol];
                    boardc[kRed, kKol] = boardc[pRed, pKol];
                    boardf[pRed, pKol] = null;
                    boardc[pRed, pKol] = null;

                    if (figura == "P" && (kRed == 0 || kRed == 7))
                    {
                        Form promocijaForma = new Form();
                        promocijaForma.Text = "Izaberi figuru";
                        promocijaForma.Size = new Size(360, 120);
                        promocijaForma.StartPosition = FormStartPosition.CenterParent;
                        promocijaForma.FormBorderStyle = FormBorderStyle.FixedDialog;

                        string izabranaFigura = "Q";

                        // Promociona dugmad mogu koristiti slike ako želiš, ovde privremeno ostaju slova radi jednostavnosti dijaloga
                        Button btnQ = new Button() { Text = "Dama (Q)", Location = new Point(10, 20), Size = new Size(75, 40) };
                        Button btnR = new Button() { Text = "Top (R)", Location = new Point(95, 20), Size = new Size(75, 40) };
                        Button btnB = new Button() { Text = "Lovac (B)", Location = new Point(180, 20), Size = new Size(75, 40) };
                        Button btnN = new Button() { Text = "Skakač (N)", Location = new Point(265, 20), Size = new Size(75, 40) };

                        btnQ.Click += (s, args) => { izabranaFigura = "Q"; promocijaForma.Close(); };
                        btnR.Click += (s, args) => { izabranaFigura = "R"; promocijaForma.Close(); };
                        btnB.Click += (s, args) => { izabranaFigura = "B"; promocijaForma.Close(); };
                        btnN.Click += (s, args) => { izabranaFigura = "N"; promocijaForma.Close(); };

                        promocijaForma.Controls.Add(btnQ); promocijaForma.Controls.Add(btnR);
                        promocijaForma.Controls.Add(btnB); promocijaForma.Controls.Add(btnN);

                        promocijaForma.ShowDialog();
                        boardf[kRed, kKol] = izabranaFigura;
                    }

                    // NOVO: Osvežavanje slika na tabli nakon uspešnog poteza
                    PostaviSlikuFigure(kRed, kKol);
                    board[pRed, pKol].Image = null;

                    if (figura == "K" && mojaBoja == "W") beliKraljSePomerio = true;
                    if (figura == "K" && mojaBoja == "B") crniKraljSePomerio = true;
                    if (figura == "R")
                    {
                        if (pRed == 7 && pKol == 0) leviBeliTopSePomerio = true;
                        if (pRed == 7 && pKol == 7) desniBeliTopSePomerio = true;
                        if (pRed == 0 && pKol == 0) leviCrniTopSePomerio = true;
                        if (pRed == 0 && pKol == 7) desniCrniTopSePomerio = true;
                    }

                    int sledeciEnPassantKol = -1;
                    if (figura == "P" && Math.Abs(pRed - kRed) == 2) sledeciEnPassantKol = kKol;

                    potez = !potez;
                    enPassantCiljKolona = sledeciEnPassantKol;

                    string trenutniIgracBoja = potez ? "W" : "B";
                    string imeIgraca = potez ? "BELI" : "CRNI";
                    string imeProtivnika = potez ? "CRNI" : "BELI";

                    bool podSahom = IsKingInCheck(trenutniIgracBoja);
                    bool imaPoteza = IgracImaLegalnihPoteza(trenutniIgracBoja);

                    if (!imaPoteza)
                    {
                        if (podSahom)
                        {
                            if (trenutniIgracBoja == "B") resultadoBeli++; else rezultatCrni++;
                            OsveziNasworProzoraPrekoFormata(imeIgraca, imeProtivnika, podSahom);
                            return;
                        }
                        else
                        {
                            resultadoBeli += 0.5;
                            rezultatCrni += 0.5;
                            OsveziNaslovProzora();

                            DialogResult odgovor = MessageBox.Show("PAT pozicija! Nerešeno je (svakom po 0.5 boda).\n\nDa li želiš novu igru?", "Nerešeno", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (odgovor == DialogResult.Yes) ResetujCeoMec(); else this.Close();
                            return;
                        }
                    }
                    else if (podSahom)
                    {
                        MessageBox.Show(string.Format("{0}, tvoj kralj je pod šahom!", imeIgraca), "Šah!");
                    }
                }
                ResetujSelekciju();
            }
        }

        void OsveziNasworProzoraPrekoFormata(string imeIgraca, string imeProtivnika, bool podSahom)
        {
            OsveziNaslovProzora();
            string porukaMat = string.Format("ŠAH-MAT! {0} je pobedio!\n\nDa li želiš novu igru?", imeProtivnika);
            DialogResult odgovor = MessageBox.Show(porukaMat, "Kraj igre", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (odgovor == DialogResult.Yes) ResetujCeoMec(); else this.Close();
        }

        void ObeleziMogucePoteze(int pocetnoPolje, string figura, bool bojaIgraca)
        {
            string oznakaBoje = bojaIgraca ? "W" : "B";
            int pRed = pocetnoPolje / 8; int pKol = pocetnoPolje % 8;

            for (int i = 0; i < 64; i++)
            {
                if (i == pocetnoPolje) continue;
                int ciljRed = i / 8; int ciljKol = i % 8;

                if (boardc[ciljRed, ciljKol] == oznakaBoje) continue;

                if (isPossible(pocetnoPolje, i, figura, bojaIgraca))
                {
                    string slovoFigure = figura;
                    if (slovoFigure == "R" || slovoFigure == "B" || slovoFigure == "Q")
                    {
                        if (!Put(pocetnoPolje, i)) continue;
                    }

                    string staraCiljFigura = boardf[ciljRed, ciljKol]; string staraCiljBoja = boardc[ciljRed, ciljKol];
                    bool jeEP = (slovoFigure == "P" && Math.Abs(pKol - ciljKol) == 1 && string.IsNullOrEmpty(staraCiljFigura));
                    string epF = null; string epB = null;
                    if (jeEP) { epF = boardf[pRed, ciljKol]; epB = boardc[pRed, ciljKol]; boardf[pRed, ciljKol] = null; boardc[pRed, ciljKol] = null; }

                    boardf[ciljRed, ciljKol] = boardf[pRed, pKol]; boardc[ciljRed, ciljKol] = boardc[pRed, pKol];
                    boardf[pRed, pKol] = null; boardc[pRed, pKol] = null;

                    bool kraljUgrozen = IsKingInCheck(oznakaBoje);

                    boardf[pRed, pKol] = boardf[ciljRed, ciljKol]; boardc[pRed, pKol] = boardc[ciljRed, ciljKol];
                    boardf[ciljRed, ciljKol] = staraCiljFigura; boardc[ciljRed, ciljKol] = staraCiljBoja;
                    if (jeEP) { boardf[pRed, ciljKol] = epF; boardc[pRed, ciljKol] = epB; }

                    if (kraljUgrozen) continue;

                    board[ciljRed, ciljKol].BackColor = Color.LightGreen;
                }
            }
        }

        private void ResetujSelekciju()
        {
            dugme = null; ind = -1;
            for (int row = 0; row < 8; row++)
                for (int col = 0; col < 8; col++)
                    board[row, col].BackColor = ((row + col) % 2 == 0) ? Color.FromArgb(240, 241, 214) : Color.FromArgb(118, 150, 86);
        }

        bool Put(int ppolje, int kpolje)
        {
            int pRed = ppolje / 8; int pKol = ppolje % 8;
            int kRed = kpolje / 8; int kKol = kpolje % 8;

            int korakRed = Math.Sign(kRed - pRed); int korakKol = Math.Sign(kKol - pKol);
            int trenutniRed = pRed + korakRed; int trenutniKol = pKol + korakKol;

            while (trenutniRed != kRed || trenutniKol != kKol)
            {
                if (!string.IsNullOrEmpty(boardf[trenutniRed, trenutniKol])) return false;
                trenutniRed += korakRed; trenutniKol += korakKol;
            }
            return true;
        }

        bool IsKingInCheck(string bojaKralja)
        {
            int kraljIndeks = -1;

            for (int i = 0; i < 64; i++)
            {
                if (boardf[i / 8, i % 8] == "K" && boardc[i / 8, i % 8] == bojaKralja) { kraljIndeks = i; break; }
            }
            if (kraljIndeks == -1) return false;

            for (int i = 0; i < 64; i++)
            {
                int r = i / 8; int c = i % 8;
                if (!string.IsNullOrEmpty(boardf[r, c]) && boardc[r, c] != bojaKralja)
                {
                    string protivnickaFiguraSlovo = boardf[r, c];
                    bool protivnickaBojaBool = (boardc[r, c] == "W");

                    if (isPossible(i, kraljIndeks, protivnickaFiguraSlovo, protivnickaBojaBool))
                    {
                        if ("RBQ".Contains(protivnickaFiguraSlovo)) { if (Put(i, kraljIndeks)) return true; }
                        else if (protivnickaFiguraSlovo == "N") return true;
                        else if (protivnickaFiguraSlovo == "P" && Math.Abs(c - (kraljIndeks % 8)) == 1) return true;
                    }
                }
            }
            return false;
        }

        bool IgracImaLegalnihPoteza(string bojaIgraca)
        {
            bool jeBeli = (bojaIgraca == "W");
            for (int i = 0; i < 64; i++)
            {
                int pRed = i / 8; int pKol = i % 8;
                if (!string.IsNullOrEmpty(boardf[pRed, pKol]) && boardc[pRed, pKol] == bojaIgraca)
                {
                    string figSlovo = boardf[pRed, pKol];
                    for (int j = 0; j < 64; j++)
                    {
                        int kRed = j / 8; int kKol = j % 8;
                        bool validan = isPossible(i, j, figSlovo, jeBeli);
                        if (validan && "RBQ".Contains(figSlovo)) validan = Put(i, j);

                        if (validan && boardc[kRed, kKol] != bojaIgraca)
                        {
                            string staraCF = boardf[kRed, kKol]; string staraCB = boardc[kRed, kKol];
                            bool jeEP = (figSlovo == "P" && Math.Abs(pKol - kKol) == 1 && string.IsNullOrEmpty(staraCF));
                            string epF = null; string epB = null;
                            if (jeEP) { epF = boardf[pRed, kKol]; epB = boardc[pRed, kKol]; boardf[pRed, kKol] = null; boardc[pRed, kKol] = null; }

                            boardf[kRed, kKol] = boardf[pRed, pKol]; boardc[kRed, kKol] = boardc[pRed, pKol];
                            boardf[pRed, pKol] = null; boardc[pRed, pKol] = null;

                            bool ugrozen = IsKingInCheck(bojaIgraca);

                            boardf[pRed, pKol] = boardf[kRed, kKol]; boardc[pRed, pKol] = boardc[kRed, kKol];
                            boardf[kRed, kKol] = staraCF; boardc[kRed, kKol] = staraCB;
                            if (jeEP) { boardf[pRed, kKol] = epF; boardc[pRed, kKol] = epB; }

                            if (!ugrozen) return true;
                        }
                    }
                }
            }
            return false;
        }

        void ResetujCeoMec()
        {
            Array.Clear(boardf, 0, boardf.Length);
            Array.Clear(boardc, 0, boardc.Length);
            SetupBoard();

            for (int row = 0; row < 8; row++)
                for (int col = 0; col < 8; col++)
                {
                    board[row, col].BackColor = ((row + col) % 2 == 0) ? Color.FromArgb(240, 241, 214) : Color.FromArgb(118, 150, 86);

                    // NOVO: Osveži grafiku i kod reseta
                    PostaviSlikuFigure(row, col);
                }

            potez = true; dugme = null; ind = -1;
            enPassantCiljKolona = -1;
            beliKraljSePomerio = false; crniKraljSePomerio = false;
            leviBeliTopSePomerio = false; desniBeliTopSePomerio = false;
            leviCrniTopSePomerio = false; desniCrniTopSePomerio = false;

            MessageBox.Show("Igra je resetovana. Beli je na potezu!", "Šah");
        }

        private void Form1_Load(object sender, EventArgs e) { }
    }
}
