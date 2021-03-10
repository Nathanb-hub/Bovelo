﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Resources;

namespace Bovelo
{
    public partial class Form1 : Form
    {

        Order order = new Order(new Dictionary<Bike, List<int>>());
        Catalog c = new Catalog();


        public Form1()
        {
            InitializeComponent();
            modelBox.SelectedIndex = 0;
            sizeBox.SelectedIndex = 0;
            colorBox.SelectedIndex = 0;
            quantityBox.SelectedText = "1";
            // si on souhaite ajouter un velo different
            c.addBike(new Bike(new Type("Electric"), new Size("26"), new Color("Black"), 100, false), "C:/Users/nathanbuchin/Pictures/OtherBikeModels/ElectricBlack.jpg");
            c.addBike(new Bike(new Type("City"), new Size("26"), new Color("Red"), 100, false), "C:/Users/nathanbuchin/Pictures/Ville/CityRed.png");
            NewGen_Catalog();
        }

        private void label1_Click(object sender, EventArgs e)
        {
           
        }
        // pour instancier autant de velo que le client renseigne dans la case quantity
        private void orderBikeBuilder(Type model, Size size, Color color, int quantity)
        {
            for (int value = 0; value < quantity; value++)
            {
                Bike bike_name = new Bike(model, size, color, model.Price, true);
                order.AddBike(bike_name);
            }
        }

        // les 3 fonctions qui suivent servent juste a determiner la page active
        private void recapBtn_Click(object sender, EventArgs e)
        {
            panelRecap.Visible = true;
            panelOrder.Visible = false;
            panelDelay.Visible = false;
            panelCatalog.Visible = false;
        }
        private void orderPageBtn_Click(object sender, EventArgs e)
        {
            panelOrder.Visible = true;
            panelDelay.Visible = false;
            panelRecap.Visible = false;
            panelCatalog.Visible = false;
        }

        private void delayBtn_Click(object sender, EventArgs e)
        {
            panelDelay.Visible = true;
            panelOrder.Visible = false;
            panelRecap.Visible = false;
            panelCatalog.Visible = false;
            // chercher dans la bdd si vélo en stock puis estimer delay
            //delayEstimater();

        }

        // pour ajouter les elements selectiones dans commande
        private void addBtn_Click(object sender, EventArgs e)
        {
            string model = modelBox.Text;
            string size = sizeBox.Text;
            string color = colorBox.Text;
            string bikeSize = size.Substring(0, 2);
            int quantity = Int32.Parse(quantityBox.Text);

            Type t = new Type(model);
            Color c = new Color(color);
            Size s = new Size(bikeSize);
            if (model == "Adventure" || model == "Explorer" || model == "City")
            {
                orderBikeBuilder(t, s, c, quantity);
            }
            generate_Recap();

        }
        private void generate_Recap()
        {
            string recap = "";
            int totalPrice = 0;
            Dictionary<string, int> bikesCounter = new Dictionary<string, int>(){ };
            Dictionary<Bike, List<int>> orderedBikes = order.Bikes;

            // boucle pour reordonner le dictionnaire
            foreach (KeyValuePair<Bike, List<int>> bike in orderedBikes)
            {
                string b = bike.Key.Type.Types + bike.Key.Color.Colors + bike.Key.Size.Sizes;

                if (!bikesCounter.ContainsKey(b))
                {
                    bikesCounter.Add((b), bike.Value[0]);
                }
                else
                {
                    bikesCounter[b] += bike.Value[0];
                }     
            }

            // on parcourir le dictionnaire trier pour afficher le recap
            foreach (KeyValuePair<string, int> bike in bikesCounter)
            {
                if (bike.Key.Substring(0, 3) == "Cit")
                {
                    totalPrice += (bike.Value * 100);
                }
                else if (bike.Key.Substring(0, 3) == "Adv")
                {
                    totalPrice += (bike.Value * 200);
                }
                else if (bike.Key.Substring(0, 3) == "Exp")
                {
                    totalPrice += (bike.Value * 150);
                }
                if (bike.Value != 0)
                {
                    recap += String.Format("Vélo : {0}, quantité : {1} piece(s) \n", bike.Key, bike.Value);
                }
            }
            totalPriceTxt.Text = totalPrice.ToString();
            recapTxt.Text = recap;
        }

        // pour vider le panier et recommencer une commande 
        private void resetBtn_Click(object sender, EventArgs e)
        {
            order.Bikes.Clear();
            totalPriceTxt.Text = "";
            recapTxt.Text = "";
        }

        // pour confirmer une commande 
        private void confirmBtn_Click(object sender, EventArgs e)
        {
            if (order.Bikes.Count != 0)
            {
                Customer customer = new Customer("FrigoFri");
                customer.SetOrder(order);
                Console.WriteLine(customer.ToString());
                order.Bikes.Clear();
                totalPriceTxt.Text = "";
                recapTxt.Text = "";
            }
            else
            {
                Console.WriteLine("Please choose articles first :)");
            }

            // doit retirer la commande de la bdd
        }

        private void catalogBtn_Click(object sender, EventArgs e)
        {
            panelCatalog.Visible = true;
            panelOrder.Visible = false;
            panelDelay.Visible = false;
            panelRecap.Visible = false;
        }

        // fonction qui va générer le contenu de la page catalogue de manière dynamique
        private void NewGen_Catalog()
        {
            FlowLayoutPanel fp = new FlowLayoutPanel();
            fp.Location = new System.Drawing.Point(200, 50);
            fp.Size = new System.Drawing.Size(1105, 658);
            fp.FlowDirection = FlowDirection.LeftToRight;
            fp.AutoScroll = true;
            panelCatalog.Controls.Add(fp);

            foreach (KeyValuePair<Bike, string> item in c.getDico)
            {
                // panel qui va contenir une image et un bouton 
                Panel p = new Panel();
                p.Size = new System.Drawing.Size(400, 350);//250, 150
                p.BackColor = System.Drawing.Color.FromArgb(51, 51, 76);


                //bouton pour aller a la page de commande du velo
                Button btn = new Button();
                btn.Click += new EventHandler(this.btn_Click);
                btn.Name = item.Key.Type.Types +'/'+ item.Key.Color.Colors; //Le Name contient le modele et la couleur 
                btn.Size = new System.Drawing.Size(400, 75);
                btn.Text = "Order me";
                btn.Font = new Font("EuroRoman", 12.5f);
                btn.FlatAppearance.BorderSize = 1;
                btn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(204, 240, 200);
                btn.BackColor = System.Drawing.Color.FromArgb(114, 140, 170);

                //image du velo
                PictureBox b = new PictureBox();
                Console.WriteLine(item.Value);
                b.Image = new Bitmap(item.Value);
                b.SizeMode = PictureBoxSizeMode.Zoom;
                b.Size = new System.Drawing.Size(375, 225);
                b.BackColor = System.Drawing.Color.FromArgb(51, 51, 76);
               

                // on ajoute le bouton et l'image dans le panel
                p.Controls.Add(b);
                b.Dock = DockStyle.None;
                p.Controls.Add(btn);
                btn.Dock = DockStyle.Bottom;

                //on ajoute le panel actuel au FLowPanel
                fp.Controls.Add(p);

            }
        }



        private void exitBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        // fonction qui est executee lorsqu'on clique sur une image du catalogue
        private void btn_Click(object sender, EventArgs e)
        {
            panelOrder.Visible = true;
            string bikeRef = (sender as Button).Name;
            string[] elems = bikeRef.Split(new char[] { '/' });
            modelBox.SelectedItem = elems[0];
            colorBox.SelectedItem = elems[1];
        }

        //pour recuperer le model selectionnee par l'utilisateur
        private Type Detect_Model(string model)
        {
            Type t;
            switch (model) // pour recuperer le model
            {
                case "City":
                    t = new Type("City");
                    break;
                case "Adventure":
                    t = new Type("Adventure");
                    break;
                case "Explorer":
                    t = new Type("Explorer");
                    break;
                default:
                    t = new Type("City");
                    break;
            }

            return t;
        }
        
        // pour recuperer la couleur selectionnee par l'utilisateur
        private Color Detect_Color(string color)
        {
            Color col;
            switch (color) 
            {
                case "Red":
                    col = new Color("Red");
                    break;
                case "Blue":
                    col = new Color("Blue");
                    break;
                case "Black":
                    col = new Color("Black");
                    break;
                default:
                    col = new Color("Red");
                    break;
            }
            return col;
        }
    }
}
