using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace GasCalculation_AutoRefresh_DesktopApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GasCalculationForm());

        }

        public partial class GasCalculationForm : Form
        {



            private Label fuelTypeLabel;
            private ComboBox fuelTypeComboBox;
            private Label distanceLabel;
            private TextBox distanceTextBox;
            private Label averageGasSpendLabel;
            private TextBox averageGasSpendTextBox;
            private Button calculateButton;
            private Label priceLabel;
            private Label totalFuelConsumptionLabel;
            private Label totalPriceLabel;
            private Label returnLabel;

            public GasCalculationForm()
            {
                InitializeComponent();
                InitializeComponents();
            }

            private void InitializeComponent()
            {
                // Initialize any components already created using the Designer
                // This method is usually empty for manually created forms
            }

            private void InitializeComponents()
            {
                this.Text = "Gas Calculation";
                this.Size = new System.Drawing.Size(400, 400);
                this.StartPosition = FormStartPosition.CenterScreen;

                fuelTypeLabel = new Label();
                fuelTypeLabel.Text = "Fuel Type:";
                fuelTypeLabel.Location = new System.Drawing.Point(20, 20);

                fuelTypeComboBox = new ComboBox();
                fuelTypeComboBox.Items.AddRange(new object[] { "benzin", "dizel", "lpg" });
                fuelTypeComboBox.Location = new System.Drawing.Point(250, 20);

                distanceLabel = new Label();
                distanceLabel.Text = "Distance (km):";
                distanceLabel.Location = new System.Drawing.Point(20, 60);

                distanceTextBox = new TextBox();
                distanceTextBox.Location = new System.Drawing.Point(250, 60);

                averageGasSpendLabel = new Label();
                averageGasSpendLabel.Text = "Average Gas Spend (liters per 100 km):";
                averageGasSpendLabel.Location = new System.Drawing.Point(20, 100);

                averageGasSpendLabel.Size = new System.Drawing.Size(200, 20); // Example size, adjust as needed

                averageGasSpendTextBox = new TextBox();
                averageGasSpendTextBox.Location = new System.Drawing.Point(250, 100);

                calculateButton = new Button();
                calculateButton.Text = "Calculate";
                calculateButton.Location = new System.Drawing.Point(150, 140);
                calculateButton.Click += CalculateButton_Click;

                priceLabel = new Label();
                priceLabel.Text = "Price: ";
                priceLabel.Location = new System.Drawing.Point(20, 180);

                priceLabel.Size = new System.Drawing.Size (200, 20);

                totalFuelConsumptionLabel = new Label();
                totalFuelConsumptionLabel.Text = "Total Fuel Consumption:";
                totalFuelConsumptionLabel.Location = new System.Drawing.Point(20, 220);

                totalFuelConsumptionLabel.Size = new System.Drawing.Size(200, 20);

                totalPriceLabel = new Label();
                totalPriceLabel.Text = "Total Price:";
                totalPriceLabel.Location = new System.Drawing.Point(20, 260);

                totalPriceLabel.Size = new System.Drawing.Size(200, 20);

                returnLabel = new Label();
                returnLabel.Text = "Return:";
                returnLabel.Location = new System.Drawing.Point(20, 300);

                returnLabel.Size = new System.Drawing.Size(200, 20);

                this.Controls.Add(fuelTypeLabel);
                this.Controls.Add(fuelTypeComboBox);
                this.Controls.Add(distanceLabel);
                this.Controls.Add(distanceTextBox);
                this.Controls.Add(averageGasSpendLabel);
                this.Controls.Add(averageGasSpendTextBox);
                this.Controls.Add(calculateButton);
                this.Controls.Add(priceLabel);
                this.Controls.Add(totalFuelConsumptionLabel);
                this.Controls.Add(totalPriceLabel);
                this.Controls.Add(returnLabel);
            }

            private void CalculateButton_Click(object sender, EventArgs e)
            {
                double distance, averageGasSpend, totalGasSpend, totalPrice;
                string price = null;

                string fuelType = fuelTypeComboBox.Text.ToLower();

                var fuelTypeLabels = new Dictionary<string, string>
            {
                {"benzin", "V/Max Kurşunsuz 95"},
                {"dizel", "V/Max Diesel"},
                {"lpg", "PO/gaz Otogaz"}
            };

                if (fuelTypeLabels.ContainsKey(fuelType))
                {
                    string fuelTypeLabel = fuelTypeLabels[fuelType];

                    HtmlWeb web = new HtmlWeb();
                    HtmlAgilityPack.HtmlDocument doc = web.Load("https://www.petrolofisi.com.tr/akaryakit-fiyatlari");


                    HtmlNode fuelTypeNode = doc.DocumentNode.SelectSingleNode($"//div[contains(div[@class='mb-1 fs-7 text-primary'], '{fuelTypeLabel}')]");
                    if (fuelTypeNode != null)
                    {
                        string priceText = fuelTypeNode.InnerText.Trim();
                        int startIndex = priceText.IndexOf(fuelTypeLabel) + fuelTypeLabel.Length;
                        int endIndex = priceText.IndexOf("TL", startIndex);
                        price = priceText.Substring(startIndex, endIndex - startIndex).Trim();
                        price = price.Replace(".", ",");
                        priceLabel.Text = $"Price for {fuelType}: " + price + " tl";
                    }
                    else
                    {
                        priceLabel.Text = $"Price for {fuelType} not found on the website.";
                        return;
                    }
                }
                else
                {
                    priceLabel.Text = "Invalid fuel type selected.";
                    return;
                }

                distance = double.Parse(distanceTextBox.Text);
                averageGasSpend = double.Parse(averageGasSpendTextBox.Text);

                totalGasSpend = Math.Round((averageGasSpend * distance) / 100, 2);
                totalPrice = Math.Round(totalGasSpend * double.Parse(price), 2);

                totalFuelConsumptionLabel.Text = $"Total fuel consumption = {totalGasSpend} liters";
                totalPriceLabel.Text = $"Total price = {totalPrice} tl";

                double times = Math.Round(totalPrice * 2, 2);
                returnLabel.Text = "Gidiş-Dönüş Fiyatı =  " + times + " tl ";
            }

        }
    }

}
