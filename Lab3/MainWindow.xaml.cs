using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using System.Globalization;
using System.ComponentModel;

namespace Lab3
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly BackgroundWorker worker = new BackgroundWorker();
        static SpeechSynthesizer ss;
        static SpeechRecognitionEngine sre;
        string brand;
        string color;
        string fuel;

        public MainWindow()
        {
            InitializeComponent();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            ss = new SpeechSynthesizer();
            ss.SetOutputToDefaultAudioDevice();
            ss.Speak("Witam w salonie samochodowym. Jaki samochód Pan sobie życzy");
            CultureInfo ci = new CultureInfo("pl-PL");
            sre = new SpeechRecognitionEngine(ci);
            sre.SetInputToDefaultAudioDevice();
            sre.SpeechRecognized += Sre_SpeechRecognized;
            Grammar grammar = new Grammar(".\\Grammars\\Grammar1.xml", "rootRule");
            grammar.Enabled = true;
            sre.LoadGrammarAsync(grammar);
            sre.RecognizeAsync(RecognizeMode.Multiple);
        }

        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            float confidence = e.Result.Confidence;
            List<TextBox> textBoxesToChange = new List<TextBox> { brand1, color1, fuel1 };
            bool brand = e.Result.Semantics.ContainsKey("brand");
            bool color = e.Result.Semantics.ContainsKey("color");
            bool fuel = e.Result.Semantics.ContainsKey("fuel");
            if (confidence > 0.7)
            {
                foreach (var textBox in textBoxesToChange)
                {
                    Dispatcher.Invoke(() => {
                        textBox.Text = null;
                    });
                }
                if (brand && color && fuel)
                {
                    SetBrand(e);
                    SetColor(e);
                    SetFuel(e);
                    sre.SpeechRecognized -= Sre_SpeechRecognized;
                    orderEnd(sre);
                }
                else if (brand && color)
                {
                    SetBrand(e);
                    SetColor(e);
                    sre.SpeechRecognized -= Sre_SpeechRecognized;
                    ss.Speak("Proszę podać rodzaj paliwa samochodu");
                    sre.SpeechRecognized += Sre_SpeechRecognized_Fuel;
                }
                else if (brand && fuel)
                {
                    SetBrand(e);
                    SetFuel(e);
                    sre.SpeechRecognized -= Sre_SpeechRecognized;
                    ss.Speak("Proszę podać kolor samochodu");
                    sre.SpeechRecognized += Sre_SpeechRecognized_Color;
                }
                else if (color && fuel)
                {
                    SetColor(e);
                    SetFuel(e);
                    sre.SpeechRecognized -= Sre_SpeechRecognized;
                    ss.Speak("Proszę podać markę samochodu");
                    sre.SpeechRecognized += Sre_SpeechRecognized_Brand;
                }
                else if (brand)
                {
                    SetBrand(e);
                    sre.SpeechRecognized -= Sre_SpeechRecognized;
                    ss.Speak("Proszę podać kolor i rodzaj paliwa samochodu");
                    sre.SpeechRecognized += Sre_SpeechRecognized_Color_Fuel;
                }
                else if (color)
                {
                    SetColor(e);
                    sre.SpeechRecognized -= Sre_SpeechRecognized;
                    ss.Speak("Proszę podać markę i rodzaj paliwa samochodu");
                    sre.SpeechRecognized += Sre_SpeechRecognized_Brand_Fuel;
                }
                else if (fuel)
                {
                    SetFuel(e);
                    sre.SpeechRecognized -= Sre_SpeechRecognized;
                    ss.Speak("Proszę podać markę i rodzaj paliwa samochodu");
                    sre.SpeechRecognized += Sre_SpeechRecognized_Brand_Color;
                }
            }
            else
            {
                repeat(confidence);
            }
        }

        private void Sre_SpeechRecognized_Color(object sender, SpeechRecognizedEventArgs e)
        {
            float confidence = e.Result.Confidence;
            if (confidence > 0.7)
            {
                SetColor(e);
                sre.SpeechRecognized -= Sre_SpeechRecognized_Color;
                orderEnd(sre);
            }
            else
            {
                repeat(confidence);
            }
        }

        private void Sre_SpeechRecognized_Fuel(object sender, SpeechRecognizedEventArgs e)
        {
            float confidence = e.Result.Confidence;
            if (confidence > 0.7)
            {
                SetFuel(e);
                sre.SpeechRecognized -= Sre_SpeechRecognized_Fuel;
                orderEnd(sre);
            }
            else
            {
                repeat(confidence);
            }
        }

        private void Sre_SpeechRecognized_Brand(object sender, SpeechRecognizedEventArgs e)
        {
            float confidence = e.Result.Confidence;
            if (confidence > 0.7)
            {
                SetBrand(e);
                sre.SpeechRecognized -= Sre_SpeechRecognized_Brand;
                orderEnd(sre);
            }
            else
            {
                repeat(confidence);
            }
        }

        private void Sre_SpeechRecognized_Color_Fuel(object sender, SpeechRecognizedEventArgs e)
        {
            float confidence = e.Result.Confidence;
            if (confidence > 0.7)
            {
                bool color = e.Result.Semantics.ContainsKey("color");
                bool fuel = e.Result.Semantics.ContainsKey("fuel");
                if (e.Result.Semantics.ContainsKey("color") && e.Result.Semantics.ContainsKey("fuel"))
                {
                    SetColor(e);
                    SetFuel(e);
                    sre.SpeechRecognized -= Sre_SpeechRecognized_Color_Fuel;
                    orderEnd(sre);
                }
                else if (e.Result.Semantics.ContainsKey("color"))
                {
                    SetColor(e);
                    sre.SpeechRecognized -= Sre_SpeechRecognized_Color_Fuel;
                    ss.Speak("Proszę podać rodzaj paliwa samochodu");
                    sre.SpeechRecognized += Sre_SpeechRecognized_Fuel;
                }
                else if (e.Result.Semantics.ContainsKey("fuel"))
                {
                    SetFuel(e);
                    sre.SpeechRecognized -= Sre_SpeechRecognized_Color_Fuel;
                    ss.Speak("Proszę podać kolor samochodu");
                    sre.SpeechRecognized += Sre_SpeechRecognized_Color;
                }
            }
            else
            {
                repeat(confidence);
            }
        }

        private void Sre_SpeechRecognized_Brand_Color(object sender, SpeechRecognizedEventArgs e)
        {
            float confidence = e.Result.Confidence;
            if (confidence > 0.7)
            {
                bool brand = e.Result.Semantics.ContainsKey("brand");
                bool color = e.Result.Semantics.ContainsKey("color");
                if (e.Result.Semantics.ContainsKey("brand") && e.Result.Semantics.ContainsKey("color"))
                {
                    SetBrand(e);
                    SetColor(e);
                    sre.SpeechRecognized -= Sre_SpeechRecognized_Brand_Color;
                    orderEnd(sre);
                }
                else if (e.Result.Semantics.ContainsKey("brand"))
                {
                    SetBrand(e);
                    sre.SpeechRecognized -= Sre_SpeechRecognized_Brand_Color;
                    ss.Speak("Proszę podać kolor samochodu");
                    sre.SpeechRecognized += Sre_SpeechRecognized_Color;
                }
                else if (e.Result.Semantics.ContainsKey("color"))
                {
                    SetColor(e);
                    sre.SpeechRecognized -= Sre_SpeechRecognized_Brand_Color;
                    ss.Speak("Proszę podać markę samochodu");
                    sre.SpeechRecognized += Sre_SpeechRecognized_Brand;
                }
            }
            else
            {
                repeat(confidence);
            }
        }

        private void Sre_SpeechRecognized_Brand_Fuel(object sender, SpeechRecognizedEventArgs e)
        {
            float confidence = e.Result.Confidence;
            if (confidence > 0.7)
            {
                bool brand = e.Result.Semantics.ContainsKey("brand");
                bool fuel = e.Result.Semantics.ContainsKey("fuel");
                if (e.Result.Semantics.ContainsKey("brand") && e.Result.Semantics.ContainsKey("fuel"))
                {
                    SetBrand(e);
                    SetFuel(e);
                    sre.SpeechRecognized -= Sre_SpeechRecognized_Brand_Fuel;
                    orderEnd(sre);
                }
                else if (e.Result.Semantics.ContainsKey("brand"))
                {
                    SetBrand(e);
                    sre.SpeechRecognized -= Sre_SpeechRecognized_Brand_Fuel;
                    ss.Speak("Proszę podać rodzaj paliwa samochodu");
                    sre.SpeechRecognized += Sre_SpeechRecognized_Fuel;
                }
                else if (e.Result.Semantics.ContainsKey("fuel"))
                {
                    SetFuel(e);
                    sre.SpeechRecognized -= Sre_SpeechRecognized_Brand_Fuel;
                    ss.Speak("Proszę podać markę samochodu");
                    sre.SpeechRecognized += Sre_SpeechRecognized_Brand;
                }
            }
            else
            {
                repeat(confidence);
            }
        }

        private void repeat(float confidence)
        {
            ss.Speak("Proszę powtórzyć");
            Dispatcher.Invoke(() => {
                wynik.Text = "Proszę powtórzyć, ponieważ rozpoznawanie wynosi: " + confidence;
            });
        }

        private void SetBrand(SpeechRecognizedEventArgs e)
        {
            if (e.Result.Semantics.ContainsKey("brand"))
            {
                brand = e.Result.Semantics["brand"].Value.ToString();
                Dispatcher.Invoke(() => {
                    brand1.Text = brand;
                });
            }
        }

        private void SetColor(SpeechRecognizedEventArgs e)
        {
            color = e.Result.Semantics["color"].Value.ToString();
            Dispatcher.Invoke(() => {
                color1.Text = color;
            });
        }

        private void SetFuel(SpeechRecognizedEventArgs e)
        {
            fuel = e.Result.Semantics["fuel"].Value.ToString();
            Dispatcher.Invoke(() => {
                fuel1.Text = fuel;
            });
        }

        private void orderEnd(SpeechRecognitionEngine sre)
        {
            sre.UnloadAllGrammars();
            ss.Speak("Posiadam już wszystkie informacje.");
            Dispatcher.Invoke(() => {
                wynik.Text = "Wybrałeś " + brand + " o kolorze " + color + " z silnikiem " + fuel + ".";
            });
            ss.Speak("Wybrałeś " + brand + " o kolorze " + color + " z silnikiem " + fuel + ".");
            Choices ch_yesno = new Choices();
            ch_yesno.Add("Tak");
            ch_yesno.Add("Nie");
            GrammarBuilder yesno = new GrammarBuilder();
            yesno.Append(ch_yesno);
            Grammar yesno_grammar = new Grammar(yesno);
            sre.LoadGrammar(yesno_grammar);
            /*Grammar grammar1 = new Grammar(".\\Grammars\\Grammar2.xml", "rootRule");
            grammar1.Enabled = true;
            sre.LoadGrammarAsync(grammar1);*/
            sre.SpeechRecognized += Sre_SpeechRecognized_End;
            ss.Speak("Czy Twoje zamówienie się zgadza? Tak czy nie?");
        }

        private void Sre_SpeechRecognized_End(object sender, SpeechRecognizedEventArgs e)
        {
            string txt = e.Result.Text;
            float confidence = e.Result.Confidence;
            if (confidence > 0.7)
            {
                if (txt.IndexOf("Tak") >= 0)
                {
                    ss.Speak("Postępuj zgodnie z poleceniami terminala. Do zobaczenia.");
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        MainWindow mw = new MainWindow();
                        sre.Dispose();
                        ss.Dispose();
                        this.Close();
                        Application.Current.Shutdown();
                    }));
                }
                else if (txt.IndexOf("Nie") >= 0)
                {
                    ss.Speak("Wracam od początku.");
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        sre.UnloadAllGrammars();
                        sre.SpeechRecognized -= Sre_SpeechRecognized_End;
                        MainWindow mw = new MainWindow();
                        mw.Show();
                        this.Close();
                    }));
                }
            }
            else
            {
                repeat(confidence);
            }
        }
    }
}
