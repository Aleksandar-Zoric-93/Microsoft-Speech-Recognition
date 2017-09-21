﻿using System;
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
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Diagnostics;

namespace Voice
{

    public partial class MainWindow : Window
    {
        string copyright = (string)(App.Current.Resources[Convert.ToChar(169) + " Aleksandar Zoric"]);
        SpeechSynthesizer sSynth = new SpeechSynthesizer();
        PromptBuilder pBuilder = new PromptBuilder();
        SpeechRecognitionEngine sRecognize = new SpeechRecognitionEngine();
        String aiName;
        String myName;
        Process facebook;





        string word = System.IO.File.ReadAllText(@"words.txt").Replace(@"""", String.Empty);
        string[] words = System.IO.File.ReadAllLines(@"words.txt");

        Boolean listenMode;



        public MainWindow()
        {
            InitializeComponent();
            listen();
        }

        public void listen()
        {
            sSynth.Rate = 1;
            sSynth.SelectVoice("Microsoft Zira Desktop");
            sSynth.Speak("start up sequence initialized.  Welcome " + myName);

            Choices sList = new Choices();

            sList.Add(words);
            Grammar gr = new Grammar(new GrammarBuilder(sList));

            try
            {
                sRecognize.RequestRecognizerUpdate();
                sRecognize.LoadGrammar(gr);
                sRecognize.SpeechRecognized += sRecognize_SpeechRecognized;
                sRecognize.SetInputToDefaultAudioDevice();
                sRecognize.RecognizeAsync(RecognizeMode.Multiple);
                sRecognize.Recognize();


            }

            catch
            {
                return;
            }



        }



        private void sRecognize_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Window1 setupScreen = new Window1(this);

            if (e.Result.Text == "listen")
            {
                listenMode = true; //resume listening
                sSynth.Speak("listening mode initializing.  Listening now");
                enableMic.Visibility = System.Windows.Visibility.Visible;
            }

            if (listenMode == true)
            {
                switch (e.Result.Text)
                {
                    case "exit application":
                        sSynth.Speak("terminating " + aiName + ", goodbye " + myName);
                        System.Windows.Application.Current.Shutdown();
                        break;

                    case "hello":
                        sSynth.Speak("Hello there");
                        break;

                    case "how are you":
                        sSynth.Speak("great. how are you?");
                        break;

                    case "test":
                        sSynth.Speak("testing initializing");
                        pBuilder.ClearContent();
                        pBuilder.AppendText(answer.Text);
                        sSynth.Speak(pBuilder);
                        break;

                    case "what is your name":
                        if (string.IsNullOrEmpty(aiName))
                        {
                            sSynth.Speak("My name has not been setup. To name me, command me to setup");
                        }
                        else
                        {
                            sSynth.Speak("my name is " + aiName);
                        }
                        break;

                    case "read memory":
                        sSynth.Speak("reading memory. initializing memory sequence");
                        sSynth.Speak(word);
                        break;

                    case "sleep":
                        enableMic.Visibility = System.Windows.Visibility.Hidden;
                        sSynth.Speak("Sleep mode initialized");
                        listenMode = false;

                        break;

                    case "who are you":
                    case "what are you":
                        if (string.IsNullOrEmpty(myName))
                        {
                            sSynth.Speak("i am a personal artificial intelligence assistant");
                        }
                        else
                        {
                            sSynth.Speak("i am " + myName + "'s personal artificial intelligence assistant");
                        }
                        break;

                    case "voice reset":
                        sSynth.Speak("voice reset initialized");
                        sSynth.SelectVoice("Microsoft Zira Desktop");
                        break;


                    case "setup reset":
                        sSynth.Speak("setup reset initialized");
                        myName = null;
                        aiName = null;
                        if (string.IsNullOrEmpty(aiName) && string.IsNullOrEmpty(myName))
                        {
                            sSynth.Speak("setup reset complete");
                        }
                        else
                        {
                            sSynth.Speak("setup reset failed");
                        }
                        break;

                    case "setup":
                        if (myName != null || aiName != null)
                        {
                            sSynth.Speak("setup already initialized");
                        }
                        else
                        {
                            sSynth.Speak("setup initialized");
                            setupScreen.Show();
                        }
                        break;


                    case "open facebook":
                        sSynth.Speak("Opening Facebook");
                        facebook = System.Diagnostics.Process.Start("https://www.facebook.com/");
                        break;

                    case "open youtube":
                        sSynth.Speak("Opening youtube");
                        facebook = System.Diagnostics.Process.Start("https://www.youtube.com/");
                        break;




                    default:
                        answer.Text = answer.Text + " " + e.Result.Text.ToString();
                        break;
                }

            }
        }



        private void answer_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        public void assignNames(String username, String AIname)
        {

            myName = username;
            aiName = AIname;
        }




    }
}

