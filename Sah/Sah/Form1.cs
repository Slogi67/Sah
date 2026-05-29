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
    }
}
