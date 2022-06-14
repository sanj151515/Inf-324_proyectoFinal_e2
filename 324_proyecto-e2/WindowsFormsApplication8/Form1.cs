using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using System.Data.SqlClient;



namespace WindowsFormsApplication8
{

    public partial class Form1 : Form
    {
        string cadena = "Data Source=(local)\\sqlexpress;Initial Catalog=sistema_texturas;Integrated Security=True";
        public SqlConnection sqlCon = new SqlConnection();
        public string path;

        Bitmap bmp;
        Bitmap bmpR;
        int pR, pG, pB;
        int contadorPuntos = 0;

        int[] vectorR = new int[3];
        int[] vectorG = new int[3];
        int[] vectorB = new int[3];
        public Form1()
        {
            InitializeComponent();

            loadDB();
            
        }
        private void loadDB()
        {
            conectarDB();
            textBox7.Text = "";
            SqlCommand comando = new SqlCommand("select * from imagen", sqlCon);

            SqlDataReader registros = comando.ExecuteReader();
            while (registros.Read())
            {
                textBox7.AppendText(registros["nombre"].ToString());
                textBox7.AppendText(" - ");
                textBox7.AppendText(registros["foto"].ToString());
                textBox7.AppendText(" - ");
                textBox7.AppendText(registros["punto1"].ToString());
                textBox7.AppendText(" - ");
                textBox7.AppendText(registros["punto2"].ToString());
                textBox7.AppendText(" - ");
                textBox7.AppendText(registros["punto3"].ToString());
                textBox7.AppendText(Environment.NewLine);
            }
            sqlCon.Close();
        }
        private void conectarDB()
        {
            sqlCon.ConnectionString = cadena;
            try
            {
                sqlCon.Open();
                //textBox6.Text = "siii";
            }
            catch (Exception ex)
            {
                //textBox6.Text = "no: " + ex.Message;
                throw;
            }
        }
               
        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }       
      
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Color c = new Color();            
            pR = 0;
            pG = 0;
            pB = 0;
            for (int ki = e.X; ki < e.X + 5; ki++)
                for (int kj = e.Y; kj < e.Y + 5; kj++)
                {
                    c = bmp.GetPixel(ki, kj);
                    pR = pR + c.R;
                    pG = pG + c.G;
                    pB = pB + c.B;
                }
            pR = pR / 25;
            pG = pG / 25;
            pB = pB / 25;
            textBox1.Text = c.R.ToString();
            textBox2.Text = c.G.ToString();
            textBox3.Text = c.B.ToString();
        }
        
        public void CargarImagenGuardada(object sender, EventArgs e) {
            
            SqlCommand comando = new SqlCommand("select * from imagen where nombre=@nom", sqlCon);
            comando.Parameters.AddWithValue("@nom", textBox5.Text);
            conectarDB();
            SqlDataReader registros = comando.ExecuteReader();
            while (registros.Read())
            {
                textBox7.AppendText(registros["nombre"].ToString());
                textBox7.AppendText(" - ");
                var m = (byte[])registros["foto"];
                Image i = byteArrayToImage(m);
                bmp = new Bitmap(i);
                pictureBox1.Image = bmp;
                bmpR = new Bitmap(i);
                pictureBox2.Image = bmpR;
                contadorPuntos = 0;
                string[] auxp1 = (registros["punto1"].ToString()).Split('-');
                string[] auxp2 = (registros["punto2"].ToString()).Split('-');
                string[] auxp3 = (registros["punto3"].ToString()).Split('-');
                vectorR[0] = int.Parse(auxp1[0]);
                vectorG[0] = int.Parse(auxp1[1]);
                vectorB[0] = int.Parse(auxp1[2]);
                vectorR[1] = int.Parse(auxp2[0]);
                vectorG[1] = int.Parse(auxp2[1]);
                vectorB[1] = int.Parse(auxp2[2]);
                vectorR[2] = int.Parse(auxp3[0]);
                vectorG[2] = int.Parse(auxp3[1]);
                vectorB[2] = int.Parse(auxp3[2]);

            }
            sqlCon.Close();
        }
        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
        private void GuardarImagen(object sender, EventArgs e)
        {
            string prgb1 = vectorR[0] + "-" + vectorG[0] + "-" + vectorB[0];
            string prgb2 = vectorR[1] + "-" + vectorG[1] + "-" + vectorB[1];
            string prgb3 = vectorR[2] + "-" + vectorG[2] + "-" + vectorB[2];
            byte[] b = imageToByteArray(Image.FromFile(path));

            string cadenaSQL = "insert into imagen values(@v1,@v2,@v3,@v4,@v5)";
            conectarDB();
            SqlCommand command = new SqlCommand(cadenaSQL, sqlCon);
            command.Parameters.AddWithValue("@v1", textBox4.Text);
            command.Parameters.AddWithValue("@v2", b);
            command.Parameters.AddWithValue("@v3", prgb1);
            command.Parameters.AddWithValue("@v4", prgb2);
            command.Parameters.AddWithValue("@v5", prgb3);
            command.ExecuteNonQuery();
            sqlCon.Close();

            loadDB();
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void reconocerTextura(object sender, EventArgs e)
        {
            int mR = 0, mG = 0, mB = 0;
            Color c = new Color();



            for (int i = 0; i < bmp.Width - 6; i = i + 6)
                for (int j = 0; j < bmp.Height - 6; j = j + 6)
                {
                    mR = 0;
                    mG = 0;
                    mB = 0;
                    for (int ki = i; ki < i + 6; ki++)
                        for (int kj = j; kj < j + 6; kj++)
                        {
                            c = bmp.GetPixel(ki, kj);
                            mR = mR + c.R;
                            mG = mG + c.G;
                            mB = mB + c.B;
                        }
                    mR = mR / 36;
                    mG = mG / 36;
                    mB = mB / 36;

                    c = bmp.GetPixel(i, j);

                    for (int cont1 = 0; cont1 < 3; cont1++)
                        if ((vectorR[cont1] - 6 <= mR && mR <= vectorR[cont1] + 6) && (vectorG[cont1] - 6 <= mG && mG <= vectorG[cont1] + 6) && (vectorB[cont1] - 6 <= mB && mB <= vectorB[cont1] + 6))
                        {
                            for (int ki = i; ki < i + 6; ki++)
                                for (int kj = j; kj < j + 6; kj++)
                                {
                                    switch (cont1)
                                    {
                                        case 0:
                                            bmpR.SetPixel(ki, kj, Color.Fuchsia);
                                            break;
                                        case 1:
                                            bmpR.SetPixel(ki, kj, Color.Cyan);
                                            break;
                                        case 2:
                                            bmpR.SetPixel(ki, kj, Color.Coral);
                                            break;
                                        default:

                                            break;
                                    }
                                }
                        }
                        else
                        {
                            if (cont1 == 0)
                                for (int ki = i; ki < i + 6; ki++)
                                    for (int kj = j; kj < j + 6; kj++)
                                    {
                                        c = bmpR.GetPixel(ki, kj);
                                        bmpR.SetPixel(ki, kj, Color.FromArgb(c.R, c.G, c.B));
                                    }
                        }
                }

            pictureBox2.Image = bmpR;
        }
        public int MaxOccurrence(int[] numbers)
        {
            var groups = numbers.GroupBy(x => x);
            var largest = groups.OrderByDescending(x => x.Count()).First();
            return largest.Key;//, largest.Count());
        }


        private void cortarTextura(object sender, EventArgs e)
        {
            int mR = 0, mG = 0, mB = 0;
            Color c = new Color();



            for (int i = 0; i < bmp.Width - 6; i = i + 6)
                for (int j = 0; j < bmp.Height - 6; j = j + 6)
                {
                    mR = 0;
                    mG = 0;
                    mB = 0;
                    for (int ki = i; ki < i + 6; ki++)
                        for (int kj = j; kj < j + 6; kj++)
                        {
                            c = bmp.GetPixel(ki, kj);
                            mR = mR + c.R;
                            mG = mG + c.G;
                            mB = mB + c.B;
                        }
                    mR = mR / 36;
                    mG = mG / 36;
                    mB = mB / 36;

                    c = bmp.GetPixel(i, j);

                    for (int cont1 = 0; cont1 < 3; cont1++)
                        if ((vectorR[cont1] - 6 <= mR && mR <= vectorR[cont1] + 6) && (vectorG[cont1] - 6 <= mG && mG <= vectorG[cont1] + 6) && (vectorB[cont1] - 6 <= mB && mB <= vectorB[cont1] + 6))
                        {
                            for (int ki = i; ki < i + 6; ki++)
                                for (int kj = j; kj < j + 6; kj++)
                                {
                                    switch (cont1)
                                    {
                                        case 0:
                                            bmpR.SetPixel(ki, kj, Color.Fuchsia);
                                            break;
                                        case 1:
                                            bmpR.SetPixel(ki, kj, Color.Cyan);
                                            break;
                                        case 2:
                                            bmpR.SetPixel(ki, kj, Color.Coral);
                                            break;
                                        default:

                                            break;
                                    }
                                }
                        }
                        else
                        {
                            if(cont1==0)
                            for (int ki = i; ki < i + 6; ki++)
                                for (int kj = j; kj < j + 6; kj++)
                                {
                                    c = bmpR.GetPixel(ki, kj);
                                    bmpR.SetPixel(ki, kj, Color.White);
                                }
                        }
                }

            pictureBox2.Image = bmpR;
            
        }
        
        private void acoplarTextura(object sender, EventArgs e)
        {
            Color c = new Color();
            for (int i = 0; i < bmpR.Width - 6; i = i + 6)
                for (int j = 0; j < bmpR.Height - 6; j = j + 6)
                {                    
                    bool swCambiar=false;
                    for (int ki = i; ki < i + 6; ki++)
                        for (int kj = j; kj < j + 6; kj++)
                        {
                            c = bmpR.GetPixel(ki, kj);
                            
                            if (c.R==255&& c.G == 255 && c.B == 255)
                            {
                                swCambiar = true;
                                ki = i + 6;
                                kj = j + 6;
                            }
                        }
                    
                    if (swCambiar)
                    {
                        int auxsw = buscarColorCercano(i, j, bmpR);
                        for (int ki = i; ki < i + 6; ki++)
                            for (int kj = j; kj < j + 6; kj++)
                            {                                
                                switch (auxsw)
                                {
                                    case 0:
                                        bmpR.SetPixel(ki, kj, Color.Fuchsia);
                                        break;
                                    case 1:
                                        bmpR.SetPixel(ki, kj, Color.Cyan);
                                        break;
                                    case 2:
                                        bmpR.SetPixel(ki, kj, Color.Coral);
                                        break;
                                    default:
                                        break;
                                }
                            }
                    }                    
                }
            pictureBox2.Image = bmpR;
        }
        public int buscarColorCercano(int i, int j, Bitmap bm)
        {
            
            int cont = 3;
            Color ca = new Color();
            int contadorF=0;
            int contadorC = 0;
            int contadorCo = 0;
            while (cont<4)
            {
                if (i - cont > 0)
                {
                    ca = bm.GetPixel(i - cont, j+3);
                    if (!(ca.R == 255 && ca.G == 255 && ca.B == 255))
                    {
                        if (ca.R == 255 && ca.G == 0 && ca.B == 255)
                            contadorF++;
                        if (ca.R == 0 && ca.G == 255 && ca.B == 255)
                            contadorC++;
                        if (ca.R == 255 && ca.G == 127 && ca.B == 80)
                            contadorCo++;
                    }
                }
                if (i +6+ cont < bm.Height)
                {
                    ca = bm.GetPixel(i +6+ cont, j + 3);
                    if (!(ca.R == 255 && ca.G == 255 && ca.B == 255))
                    {
                        if (ca.R == 255 && ca.G == 0 && ca.B == 255)
                            contadorF++;
                        if (ca.R == 0 && ca.G == 255 && ca.B == 255)
                            contadorC++;
                        if (ca.R == 255 && ca.G == 127 && ca.B == 80)
                            contadorCo++;
                    }
                }
                if (j - cont > 0)
                {
                    ca = bm.GetPixel(i +3, j -cont);
                    if (!(ca.R == 255 && ca.G == 255 && ca.B == 255))
                    {
                        if (ca.R == 255 && ca.G == 0 && ca.B == 255)
                            contadorF++;
                        if (ca.R == 0 && ca.G == 255 && ca.B == 255)
                            contadorC++;
                        if (ca.R == 255 && ca.G == 127 && ca.B == 80)
                            contadorCo++;
                    }
                }
                if ((j +6+ cont) < bm.Width)
                {
                    Console.Write(j + 6 + cont + "");
                    ca = bm.GetPixel(i + 3, j +6+ cont);
                    if (!(ca.R == 255 && ca.G == 255 && ca.B == 255))
                    {
                        if (ca.R == 255 && ca.G == 0 && ca.B == 255)
                            contadorF++;
                        if (ca.R == 0 && ca.G == 255 && ca.B == 255)
                            contadorC++;
                        if (ca.R == 255 && ca.G == 127 && ca.B == 80)
                            contadorCo++;
                    }
                }

                if (contadorF + contadorC + contadorCo > 2)
                {
                    if (contadorF < contadorC)
                    {
                        if (contadorC < contadorCo)
                            return 2;
                        else
                            return 1;
                    }
                    else if (contadorF > contadorC)
                    {
                        if (contadorF < contadorCo)
                            return 2;
                        else
                            return 0;
                    }                
                }
                else
                    return 3;
                
                
                cont +=6;
            }
            return 3;
        }
        public int buscarColorCercano2(int i, int j, Bitmap bm)
        {

            int cont = 3;
            Color ca = new Color();
            int contadorF = 0;
            int contadorC = 0;
            int contadorCo = 0;
            while (cont < 4)
            {
                if (i - cont > 0)
                {
                    ca = bm.GetPixel(i - cont, j + 3);
                    if (!(ca.R == 255 && ca.G == 255 && ca.B == 255))
                    {
                        if (ca.R == 255 && ca.G == 0 && ca.B == 255)
                            contadorF++;
                        if (ca.R == 0 && ca.G == 255 && ca.B == 255)
                            contadorC++;
                        if (ca.R == 255 && ca.G == 127 && ca.B == 80)
                            contadorCo++;
                    }
                }
                if (i + 6 + cont < bm.Height)
                {
                    ca = bm.GetPixel(i + 6 + cont, j + 3);
                    if (!(ca.R == 255 && ca.G == 255 && ca.B == 255))
                    {
                        if (ca.R == 255 && ca.G == 0 && ca.B == 255)
                            contadorF++;
                        if (ca.R == 0 && ca.G == 255 && ca.B == 255)
                            contadorC++;
                        if (ca.R == 255 && ca.G == 127 && ca.B == 80)
                            contadorCo++;
                    }
                }
                if (j - cont > 0)
                {
                    ca = bm.GetPixel(i + 3, j - cont);
                    if (!(ca.R == 255 && ca.G == 255 && ca.B == 255))
                    {
                        if (ca.R == 255 && ca.G == 0 && ca.B == 255)
                            contadorF++;
                        if (ca.R == 0 && ca.G == 255 && ca.B == 255)
                            contadorC++;
                        if (ca.R == 255 && ca.G == 127 && ca.B == 80)
                            contadorCo++;
                    }
                }
                if ((j + 6 + cont) < bm.Width)
                {
                    ca = bm.GetPixel(i + 3, j + 6 + cont);
                    if (!(ca.R == 255 && ca.G == 255 && ca.B == 255))
                    {
                        if (ca.R == 255 && ca.G == 0 && ca.B == 255)
                            contadorF++;
                        if (ca.R == 0 && ca.G == 255 && ca.B == 255)
                            contadorC++;
                        if (ca.R == 255 && ca.G == 127 && ca.B == 80)
                            contadorCo++;
                    }
                }
                if (i - cont*2 > 0)
                {
                    ca = bm.GetPixel(i - cont*2, j + 3);
                    if (!(ca.R == 255 && ca.G == 255 && ca.B == 255))
                    {
                        if (ca.R == 255 && ca.G == 0 && ca.B == 255)
                            contadorF++;
                        if (ca.R == 0 && ca.G == 255 && ca.B == 255)
                            contadorC++;
                        if (ca.R == 255 && ca.G == 127 && ca.B == 80)
                            contadorCo++;
                    }
                }
                if (i + 6 + cont*2 < bm.Height)
                {
                    ca = bm.GetPixel(i + 6 + cont*2, j + 3);
                    if (!(ca.R == 255 && ca.G == 255 && ca.B == 255))
                    {
                        if (ca.R == 255 && ca.G == 0 && ca.B == 255)
                            contadorF++;
                        if (ca.R == 0 && ca.G == 255 && ca.B == 255)
                            contadorC++;
                        if (ca.R == 255 && ca.G == 127 && ca.B == 80)
                            contadorCo++;
                    }
                }
                if (j - cont*2 > 0)
                {
                    ca = bm.GetPixel(i + 3, j - cont*2);
                    if (!(ca.R == 255 && ca.G == 255 && ca.B == 255))
                    {
                        if (ca.R == 255 && ca.G == 0 && ca.B == 255)
                            contadorF++;
                        if (ca.R == 0 && ca.G == 255 && ca.B == 255)
                            contadorC++;
                        if (ca.R == 255 && ca.G == 127 && ca.B == 80)
                            contadorCo++;
                    }
                }
                if ((j + 6 + cont*2) < bm.Width)
                {
                    ca = bm.GetPixel(i + 3, j + 6 + cont*2);
                    if (!(ca.R == 255 && ca.G == 255 && ca.B == 255))
                    {
                        if (ca.R == 255 && ca.G == 0 && ca.B == 255)
                            contadorF++;
                        if (ca.R == 0 && ca.G == 255 && ca.B == 255)
                            contadorC++;
                        if (ca.R == 255 && ca.G == 127 && ca.B == 80)
                            contadorCo++;
                    }
                }

                if (contadorF + contadorC + contadorCo > 4)
                {
                    if (contadorF < contadorC)
                    {
                        if (contadorC < contadorCo)
                            return 2;
                        else
                            return 1;
                    }
                    else if (contadorF > contadorC)
                    {
                        if (contadorF < contadorCo)
                            return 2;
                        else
                            return 0;
                    }
                }
                else
                    return 3;
                cont += 6;
            }
            return 3;
        }
        private void acloparTextura2(object sender, EventArgs e)
        {
            Color c = new Color();
            for (int i = 0; i < bmpR.Width - 6; i = i + 6)
                for (int j = 0; j < bmpR.Height - 6; j = j + 6)
                {
                    bool swCambiar = false;
                    for (int ki = i; ki < i + 6; ki++)
                        for (int kj = j; kj < j + 6; kj++)
                        {
                            c = bmpR.GetPixel(ki, kj);

                            if (c.R == 255 && c.G == 255 && c.B == 255)
                            {
                                swCambiar = true;
                                ki = i + 6;
                                kj = j + 6;
                            }
                        }

                    if (swCambiar)
                    {
                        int auxsw = buscarColorCercano2(i, j, bmpR);
                        for (int ki = i; ki < i + 6; ki++)
                            for (int kj = j; kj < j + 6; kj++)
                            {
                                switch (auxsw)
                                {
                                    case 0:
                                        bmpR.SetPixel(ki, kj, Color.Fuchsia);
                                        break;
                                    case 1:
                                        bmpR.SetPixel(ki, kj, Color.Cyan);
                                        break;
                                    case 2:
                                        bmpR.SetPixel(ki, kj, Color.Coral);
                                        break;
                                    default:
                                        break;
                                }
                            }
                    }
                }
            pictureBox2.Image = bmpR;
        }

        private void RegionGeografica(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Todos *.*|";
            openFileDialog1.ShowDialog();
            path = openFileDialog1.FileName;
            bmp = new Bitmap(openFileDialog1.FileName);
            pictureBox1.Image = bmp;
            bmpR = (Bitmap)bmp.Clone();
            Color actual = new Color();
            for (int i = 0; i < bmpR.Width; i++)
                for (int j = 0; j < bmpR.Height; j++)
                {
                    actual = bmpR.GetPixel(i, j);
                    actual = Color.FromArgb(actual.R, actual.R, actual.R);
                    bmpR.SetPixel(i, j, actual);
                }
            pictureBox2.Image = bmpR;
            contadorPuntos = 0;
            //Reconoce
            int mR = 0, mG = 0, mB = 0;
            Color c = new Color();
            for (int i = 0; i < bmp.Width - 3; i = i + 3)
                for (int j = 0; j < bmp.Height - 3; j = j + 3)
                {
                    mR = 0;
                    mG = 0;
                    mB = 0;
                    for (int ki = i; ki < i + 3; ki++)
                        for (int kj = j; kj < j + 3; kj++)
                        {
                            c = bmp.GetPixel(ki, kj);
                            mR = mR + c.R;
                            mG = mG + c.G;
                            mB = mB + c.B;
                        }
                    mR = mR / 9;
                    mG = mG / 9;
                    mB = mB / 9;

                    c = bmp.GetPixel(i, j);

                    if ((170 <= mR ) && (170 <= mG ) && (170<= mB))
                    {
                        for (int ki = i; ki < i + 3; ki++)
                            for (int kj = j; kj < j + 3; kj++)
                            {
                                bmpR.SetPixel(ki, kj, Color.White);
                            }
                    }
                    else
                    {
                        if ((85 <= mR) && (85 <= mG) && (85<= mB))
                        {
                            for (int ki = i; ki < i + 3; ki++)
                                for (int kj = j; kj < j + 3; kj++)
                                {
                                    bmpR.SetPixel(ki, kj, Color.Gray);
                                }
                        }
                        else
                        {
                            for (int ki = i; ki < i + 3; ki++)
                                for (int kj = j; kj < j + 3; kj++)
                                {
                                    bmpR.SetPixel(ki, kj, Color.Black);
                                }
                        }
                    }

                }
            pictureBox2.Image = bmpR;
           
            //acopla
            /*
            for (int i = 0; i < bmpR.Width - 6; i = i + 6)
                for (int j = 0; j < bmpR.Height - 6; j = j + 6)
                {
                    bool swCambiar = false;
                    for (int ki = i; ki < i + 6; ki++)
                        for (int kj = j; kj < j + 6; kj++)
                        {
                            c = bmpR.GetPixel(ki, kj);

                            if (c.R == 255 && c.G == 255 && c.B == 255)
                            {
                                swCambiar = true;
                                ki = i + 6;
                                kj = j + 6;
                            }
                        }

                    if (swCambiar)
                    {
                        int auxsw = buscarColorCercano2(i, j, bmpR,5);
                        for (int ki = i; ki < i + 6; ki++)
                            for (int kj = j; kj < j + 6; kj++)
                            {
                                switch (auxsw)
                                {
                                    case 0:
                                        bmpR.SetPixel(ki, kj, Color.Fuchsia);
                                        break;
                                    case 1:
                                        bmpR.SetPixel(ki, kj, Color.Cyan);
                                        break;
                                    case 2:
                                        bmpR.SetPixel(ki, kj, Color.Coral);
                                        break;
                                    default:
                                        break;
                                }
                            }
                    }
                }
                */
            pictureBox2.Image = bmpR;
        }

        private void AbrirImagenGeo(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Todos *.*|";
            openFileDialog1.ShowDialog();
            path = openFileDialog1.FileName;
            bmp = new Bitmap(openFileDialog1.FileName);
            Color actual = new Color();
            for (int i = 0; i < bmp.Width; i++)
                for (int j = 0; j < bmp.Height; j++)
                {
                    actual = bmp.GetPixel(i,j);
                    actual = Color.FromArgb(actual.R, actual.R, actual.R);
                    bmp.SetPixel(i,j,actual);
                }
            pictureBox1.Image = bmp;
            bmpR = (Bitmap)bmp.Clone();
            pictureBox2.Image = bmpR;
            contadorPuntos = 0;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void cambiar10x10(object sender, EventArgs e)
        {
            int mR = 0, mG = 0, mB = 0;
            Color c = new Color();

            for (int i = 0; i < bmp.Width -6; i = i + 6)
                for (int j = 0; j < bmp.Height - 6; j = j + 6)
                {
                    mR = 0;
                    mG = 0;
                    mB = 0;
                    for (int ki = i; ki < i + 6; ki++)
                        for (int kj = j; kj < j + 6; kj++)
                        {
                            c = bmp.GetPixel(ki, kj);
                            mR = mR + c.R;
                            mG = mG + c.G;
                            mB = mB + c.B;
                        }
                    mR = mR / 36;
                    mG = mG / 36;
                    mB = mB / 36;

                    c = bmp.GetPixel(i, j);
                    if ((pR - 6 <= mR && mR <= pR + 6) && (pG - 6 <= mG && mG <= pG + 6) && (pB - 6 <= mB && mB <= pB + 6))
                    {
                        for (int ki = i; ki < i + 6; ki++)
                            for (int kj = j; kj < j + 6; kj++)
                            {
                                switch (contadorPuntos)
                                {
                                    case 0:
                                        bmpR.SetPixel(ki, kj, Color.Fuchsia);
                                        vectorR[0] = pR;
                                        vectorG[0] = pG;
                                        vectorB[0] = pB;
                                        break;
                                    case 1:
                                        bmpR.SetPixel(ki, kj, Color.Cyan);
                                        vectorR[1] = pR;
                                        vectorG[1] = pG;
                                        vectorB[1] = pB;
                                        break;
                                    case 2:
                                        bmpR.SetPixel(ki, kj, Color.Coral);
                                        vectorR[2] = pR;
                                        vectorG[2] = pG;
                                        vectorB[2] = pB;
                                        break;
                                    default:
                                        break;
                                }
                            }
                    }                    
                }
            pictureBox2.Image = bmpR;
            contadorPuntos++;
        }
        

        private void cargarImagenNueva(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Todos *.*|";
            openFileDialog1.ShowDialog();
            path = openFileDialog1.FileName;
            bmp = new Bitmap(openFileDialog1.FileName);
            pictureBox1.Image = bmp;
            bmpR = (Bitmap)bmp.Clone();
            pictureBox2.Image = bmpR;
            contadorPuntos = 0;
        }
    }
}

