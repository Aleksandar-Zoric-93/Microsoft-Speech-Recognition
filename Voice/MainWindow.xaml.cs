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
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Diagnostics;
using System.Speech.Recognition.SrgsGrammar;

namespace Voice
{

    public partial class MainWindow : Window
    {
        string copyright = (string)(App.Current.Resources[Convert.ToChar(169) + " Aleksandar Zoric"]);
        SpeechSynthesizer sSynth = new SpeechSynthesizer();
        PromptBuilder pBuilder = new PromptBuilder();
        SpeechRecognitionEngine sRecognize = new SpeechRecognitionEngine();
        SpeechRecognitionEngine sRecognize1 = new SpeechRecognitionEngine();
        String aiName = "Ella";
        String myName;
        Process proc;
        string ISeventStarted;


        string word = System.IO.File.ReadAllText("../../../../Voice/Voice/words.txt").Replace(@"""", String.Empty);
        string[] words = System.IO.File.ReadAllLines("../../../../Voice/Voice/words.txt");

        string mediaWords = System.IO.File.ReadAllText("../../../../Voice/Voice/media.txt").Replace(@"""", String.Empty);
        string[] mediaWordsList = System.IO.File.ReadAllLines("../../../../Voice/Voice/media.txt");


        Boolean ISListenModeEnabled;



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
            Choices mediaChoices = new Choices();

            sList.Add(words);
            mediaChoices.Add(mediaWordsList);

            Grammar gr = new Grammar(new GrammarBuilder(sList));
            Grammar mediaMenuGrammar = new Grammar(new GrammarBuilder(mediaChoices));


            try
            {
                sRecognize.RequestRecognizerUpdate();
                sRecognize.LoadGrammar(gr);
                sRecognize.LoadGrammar(mediaMenuGrammar);
                sRecognize.LoadGrammarAsync(new DictationGrammar());
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
            SetupWindow setupScreen = new SetupWindow(this);
            string speech = e.Result.Text.ToLower();

            //Keyword to enable listening mode
            if (e.Result.Text == "listen")
            {
                ISListenModeEnabled = true; //resume listening
                sSynth.Speak("listening mode initializing.  Listening now");
                enableMic.Visibility = System.Windows.Visibility.Visible;
            }

            //Check if AI is in listening mode
            if (ISListenModeEnabled == true)
            {
                //Start Google Search
                if (speech == "search google")
                {
                    ISeventStarted = speech;
                    sSynth.SpeakAsync("what do you want to search google for?");
                    speech = string.Empty;
                }
                else if (speech != string.Empty && ISeventStarted == "search google")
                {
                    Process.Start("http://google.com/search?q=" + speech);
                    ISeventStarted = string.Empty;
                }
                //End Google Search

                //Start of commands
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
                        ISListenModeEnabled = false;

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
                        proc = System.Diagnostics.Process.Start("https://www.facebook.com/");
                        break;

                    case "open youtube":
                        sSynth.Speak("Opening youtube");
                        proc = System.Diagnostics.Process.Start("https://www.youtube.com/");
                        break;


                    default:
                        answer.Text = answer.Text + " " + e.Result.Text.ToString();
                        break;
                }
                //End of commands

            }
            //End of listening mode check
        }



        private void answer_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        //Assign AI and user name to local variables
        public void assignNames(String username, String AIname)
        {

            myName = username;
            aiName = AIname;
        }




    }
}

