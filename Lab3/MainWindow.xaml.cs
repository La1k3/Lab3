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

namespace Lab3
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static SpeechSynthesizer ss;
        static SpeechRecognitionEngine sre;
        static bool done = false;
        public MainWindow()
        {
            ss = new SpeechSynthesizer();
            ss.SetOutputToDefaultAudioDevice();
            ss.Speak("Witam w salonie samochodowym");
            ss.Speak("Jaki samochód Pan sobie życzy");
            CultureInfo ci = new CultureInfo("pl-PL");
            sre = new SpeechRecognitionEngine(ci);
            sre.SetInputToDefaultAudioDevice();
            sre.SpeechRecognized += Sre_SpeechRecognized;
            
            Grammar grammar = new Grammar(".\\Grammars\\Grammar1.xml", "rootRule");
            grammar.Enabled = true;
            sre.LoadGrammar(grammar);
            sre.RecognizeAsync(RecognizeMode.Multiple);
            InitializeComponent();
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
            }
        }

        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            float confidence = e.Result.Confidence;
            List<TextBox> textBoxesToChange = new List<TextBox> { brand1, color1, fuel1 };
            if (confidence > 0.7)
            {
                foreach (var textBox in textBoxesToChange)
                {
                    textBox.Text = null;
                }
                if (e.Result.Semantics.ContainsKey("brand"))
                {
                    string brand = e.Result.Semantics["brand"].Value.ToString();
                    brand1.Text = brand;
                    ss.Speak("Proszę podać kolor i rodzaj paliwa samochodu");
                    sre.SpeechRecognized += Sre_SpeechRecognized1;
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
        }
    }
}
