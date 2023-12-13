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
                    setBrand(e);
                    setColor(e);
                    setFuel(e);
                    sre.SpeechRecognized -= Sre_SpeechRecognized;
                    orderEnd(sre, e);
                }
                else if (brand && color)
                {
                    setBrand(e);
                    setColor(e);
                    sre.SpeechRecognized -= Sre_SpeechRecognized;
                    ss.Speak("Proszę podać rodzaj paliwa samochodu");
                    sre.SpeechRecognized += Sre_SpeechRecognized_Fuel;
                }
                else if (brand && fuel)
                {
                    setBrand(e);
                    setFuel(e);
                    sre.SpeechRecognized -= Sre_SpeechRecognized;
                    ss.Speak("Proszę podać kolor samochodu");
                    sre.SpeechRecognized += Sre_SpeechRecognized_Color;
                }
                else if (color && fuel)
                {
                    setColor(e);
                    setFuel(e);
                    sre.SpeechRecognized -= Sre_SpeechRecognized;
                    ss.Speak("Proszę podać markę samochodu");
                    sre.SpeechRecognized += Sre_SpeechRecognized_Brand;
                }
                else if (brand)
                {
                    setBrand(e);
                    sre.SpeechRecognized -= Sre_SpeechRecognized;
                    ss.Speak("Proszę podać kolor i rodzaj paliwa samochodu");
                    sre.SpeechRecognized += Sre_SpeechRecognized_Color_Fuel;
                }
                else if (color)
                {
                    setColor(e);
                    sre.SpeechRecognized -= Sre_SpeechRecognized;
                    ss.Speak("Proszę podać markę i rodzaj paliwa samochodu");
                    sre.SpeechRecognized += Sre_SpeechRecognized_Brand_Fuel;
                }
                else if (fuel)
                {
                    setFuel(e);
                    sre.SpeechRecognized -= Sre_SpeechRecognized;
                    ss.Speak("Proszę podać markę i rodzaj paliwa samochodu");
                    sre.SpeechRecognized += Sre_SpeechRecognized_Brand_Color;
                }
            }
            else
            {
                ss.Speak("Proszę powtórzyć");
                Dispatcher.Invoke(() => {
                    wynik.Text = "Proszę powtórzyć, ponieważ rozpoznawanie wynosi: " + confidence;
                });
            }
        }

        private void Sre_SpeechRecognized_Color(object sender, SpeechRecognizedEventArgs e)
        {
            float confidence = e.Result.Confidence;
            if (confidence > 0.7)
            {
                setColor(e);
                orderEnd(sre, e);
            }
            else
            {
                ss.Speak("Proszę powtórzyć");
                Dispatcher.Invoke(() => {
                    wynik.Text = "Proszę powtórzyć, ponieważ rozpoznawanie wynosi: " + confidence;
                });
            }
        }

        private void Sre_SpeechRecognized_Fuel(object sender, SpeechRecognizedEventArgs e)
        {
            float confidence = e.Result.Confidence;
            if (confidence > 0.7)
            {
                setFuel(e);
                orderEnd(sre, e);
            }
            else
            {
                ss.Speak("Proszę powtórzyć");
                Dispatcher.Invoke(() => {
                    wynik.Text = "Proszę powtórzyć, ponieważ rozpoznawanie wynosi: " + confidence;
                });
            }
        }

        private void Sre_SpeechRecognized_Brand(object sender, SpeechRecognizedEventArgs e)
        {
            float confidence = e.Result.Confidence;
            if (confidence > 0.7)
            {
                setBrand(e);
                orderEnd(sre, e);
            }
            else
            {
                ss.Speak("Proszę powtórzyć");
                Dispatcher.Invoke(() => {
                    wynik.Text = "Proszę powtórzyć, ponieważ rozpoznawanie wynosi: " + confidence;
                });
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
                    setColor(e);
                    setFuel(e);
                    orderEnd(sre, e);
                }
                else if (e.Result.Semantics.ContainsKey("color"))
                {
                    setColor(e);
                    sre.SpeechRecognized -= Sre_SpeechRecognized_Color_Fuel;
                    ss.Speak("Proszę podać rodzaj paliwa samochodu");
                    sre.SpeechRecognized += Sre_SpeechRecognized_Fuel;
                }
                else if (e.Result.Semantics.ContainsKey("fuel"))
                {
                    setFuel(e);
                    sre.SpeechRecognized -= Sre_SpeechRecognized_Color_Fuel;
                    ss.Speak("Proszę podać kolor samochodu");
                    sre.SpeechRecognized += Sre_SpeechRecognized_Color;
                }
            }
            else
            {
                ss.Speak("Proszę powtórzyć");
                Dispatcher.Invoke(() => {
                    wynik.Text = "Proszę powtórzyć, ponieważ rozpoznawanie wynosi: " + confidence;
                });
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
                    setBrand(e);
                    setColor(e);
                    orderEnd(sre, e);
                }
                else if (e.Result.Semantics.ContainsKey("brand"))
                {
                    setBrand(e);
                    sre.SpeechRecognized -= Sre_SpeechRecognized_Brand_Color;
                    ss.Speak("Proszę podać kolor samochodu");
                    sre.SpeechRecognized += Sre_SpeechRecognized_Color;
                }
                else if (e.Result.Semantics.ContainsKey("color"))
                {
                    setColor(e);
                    sre.SpeechRecognized -= Sre_SpeechRecognized_Brand_Color;
                    ss.Speak("Proszę podać markę samochodu");
                    sre.SpeechRecognized += Sre_SpeechRecognized_Brand;
                }
            }
            else
            {
                ss.Speak("Proszę powtórzyć");
                Dispatcher.Invoke(() => {
                    wynik.Text = "Proszę powtórzyć, ponieważ rozpoznawanie wynosi: " + confidence;
                });
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
                    setBrand(e);
                    setFuel(e);
                    orderEnd(sre, e);
                }
                else if (e.Result.Semantics.ContainsKey("brand"))
                {
                    setBrand(e);
                    sre.SpeechRecognized -= Sre_SpeechRecognized_Brand_Fuel;
                    ss.Speak("Proszę podać rodzaj paliwa samochodu");
                    sre.SpeechRecognized += Sre_SpeechRecognized_Fuel;
                }
                else if (e.Result.Semantics.ContainsKey("fuel"))
                {
                    setFuel(e);
                    sre.SpeechRecognized -= Sre_SpeechRecognized_Brand_Fuel;
                    ss.Speak("Proszę podać markę samochodu");
                    sre.SpeechRecognized += Sre_SpeechRecognized_Brand;
                }
            }
            else
            {
                ss.Speak("Proszę powtórzyć");
                Dispatcher.Invoke(() => {
                    wynik.Text = "Proszę powtórzyć, ponieważ rozpoznawanie wynosi: " + confidence;
                });
            }
        }

        private void setBrand(SpeechRecognizedEventArgs e)
        {
            string brand = e.Result.Semantics["brand"].Value.ToString();
            Dispatcher.Invoke(() => {
                brand1.Text = brand;
            }); 
        }

        private void setColor(SpeechRecognizedEventArgs e)
        {
            string color = e.Result.Semantics["color"].Value.ToString();
            Dispatcher.Invoke(() => {
                color1.Text = color;
            });
        }

        private void setFuel(SpeechRecognizedEventArgs e)
        {
            string fuel = e.Result.Semantics["fuel"].Value.ToString();
            Dispatcher.Invoke(() => {
                fuel1.Text = fuel;
            });
        }

        private void orderEnd(SpeechRecognitionEngine sre, SpeechRecognizedEventArgs e)
        {
            sre.UnloadAllGrammars();
            ss.Speak("Posiadam już wszystkie informacje.");
            string brand = e.Result.Semantics["brand"].Value.ToString();
            string color = e.Result.Semantics["color"].Value.ToString();
            string fuel = e.Result.Semantics["fuel"].Value.ToString();
            Dispatcher.Invoke(() => {
                wynik.Text = "Wybrałeś " + brand + " o kolorze " + color + " z silnikiem " + fuel;
            });
            ss.Speak("Wybrałeś " + brand + " o kolorze " + color + " z silnikiem " + fuel);
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
                ss.Speak("Proszę powtórzyć.");
                Dispatcher.Invoke(() => {
                    wynik.Text = "Proszę powtórzyć, ponieważ rozpoznawanie wynosi: " + confidence;
                });
            }
        }

        /*sre.UnloadAllGrammars();
        Grammar grammar1 = new Grammar(".\\Grammars\\Grammar1.xml", "choseColor");
        grammar.Enabled = true;
        sre.LoadGrammarAsync(grammar1);*/

        /*private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            float confidence = e.Result.Confidence;
            List<TextBox> textBoxesToChange = new List<TextBox> { brand1, color1, fuel1 };
            int check = 0;
            if (confidence > 0.7)
            {
                foreach (var textBox in textBoxesToChange)
                {
                    textBox.Text = null;
                }
                if (e.Result.Semantics.ContainsKey("brand") && e.Result.Semantics.ContainsKey("fuel"))
                {
                    check += 1;
                    string brand = e.Result.Semantics["brand"].Value.ToString();
                    brand1.Text = brand;
                    sre.SpeechRecognized -= Sre_SpeechRecognized;
                    sre.UnloadAllGrammars();
                    Grammar grammar1 = new Grammar(".\\Grammars\\Grammar2.xml", "rootRule");
                    //grammar1.Enabled = true;
                    sre.LoadGrammarAsync(grammar1);
                    sre.SpeechRecognized += Sre_SpeechRecognized2;
                    ss.Speak("Proszę podać kolor samochodu");
                }
                else
                {
                    ss.Speak("Proszę podać markę samochodu");
                }
                if (e.Result.Semantics.ContainsKey("color"))
                {
                    string color = e.Result.Semantics["color"].Value.ToString();
                    color1.Text = color;
                }
                else
                {
                    ss.Speak("Proszę podać kolor samochodu");
                }
                if (e.Result.Semantics.ContainsKey("fuel"))
                {
                    string fuel = e.Result.Semantics["fuel"].Value.ToString();
                    fuel1.Text = fuel;
                }
                else
                {
                    ss.Speak("Proszę podać rodzaj paliwa samochodu");
                }
            }
            else
            {
                ss.Speak("Proszę powtórzyć");
                wynik.Text = "Proszę powtórzyć, ponieważ rozpoznawanie wynosi: " + confidence;
            }
        }*/

        /*private void Sre_SpeechRecognized2(object sender, SpeechRecognizedEventArgs e)
        {
            float confidence = e.Result.Confidence;
            if (confidence > 0.7)
            {
                string color = e.Result.Semantics["color"].Value.ToString();
                Dispatcher.Invoke(() => {
                    color1.Text = color;
                });
                // Przykładowy kod
                ss.Speak("Sre_SpeechRecognized2 działa");
            }
            else
            {
                ss.Speak("to dziala");
            }
        }

        private void Sre_SpeechRecognized1(object sender, SpeechRecognizedEventArgs e)
        {
            float confidence = e.Result.Confidence;
            List<TextBox> textBoxesToChange = new List<TextBox> { brand1, color1, fuel1 };
            if (confidence > 0.7)
            {
                if (e.Result.Semantics.ContainsKey("color"))
                {
                    string color = e.Result.Semantics["color"].Value.ToString();
                    color1.Text = color;
                }
                else
                {
                    ss.Speak("Proszę podać kolor samochodu");
                }
                if (e.Result.Semantics.ContainsKey("fuel"))
                {
                    string fuel = e.Result.Semantics["fuel"].Value.ToString();
                    fuel1.Text = fuel;
                }
                else
                {
                    ss.Speak("Proszę podać rodzaj paliwa samochodu");
                }
            }
            else
            {
                ss.Speak("Proszę powtórzyć");
                wynik.Text = "Proszę powtórzyć, ponieważ rozpoznawanie wynosi: " + confidence;
                ss.Speak("Proszę podać markę, kolor i rodzaj silnika samochodu");
            }
        }*/

    }
}
