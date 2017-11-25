using System;
using System.Windows;
using System.Windows.Controls;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Diagnostics;
using Syn.Bot;
using System.IO;
using Syn.Bot.Siml;


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
        public String myName;
        Process proc;
        string ISeventStarted;


        public SimlBot simlBot;

        string word = System.IO.File.ReadAllText("../../../../Voice/Voice/words.txt").Replace(@"""", String.Empty);
        string[] words = System.IO.File.ReadAllLines("../../../../Voice/Voice/words.txt");

        Boolean ISListenModeEnabled;



        public MainWindow()
        {
            
            InitializeComponent();
            loadState();
            simlBot = new SimlBot();
            simlBot.PackageManager.LoadFromString(File.ReadAllText("Knowledge.simlpk"));
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
            string speech = e.Result.Text.ToLower();

            //Keyword to enable listening mode
            if (e.Result.Text == "ella")
            {
                ISListenModeEnabled = true; //resume listening
                sSynth.Speak("I am listening");
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

                //Start setup               
                if (speech == "setup")
                {
                    ISeventStarted = speech;
                    sSynth.SpeakAsync("what is your name?");
                    speech = string.Empty;
                }
                else if (speech != string.Empty && ISeventStarted == "setup")
                {
                    assignNames(speech);
                    myName = speech;
                    sSynth.SpeakAsync("hi there " + speech);
                    ISeventStarted = string.Empty;
                }
                //End setup
               
                //test
                var result = simlBot.Chat(speech);
                var botMessage = result.BotMessage;

                if (result.Success)
                {
                    sSynth.SpeakAsync(botMessage);
                }

                //Start of commands
                switch (e.Result.Text)
                {
                    case "exit application":
                        sSynth.Speak("terminating " + aiName + ", goodbye " + myName);
                        System.Windows.Application.Current.Shutdown();
                        break;

                   // case "how are you":
                     //   sSynth.Speak("great. how are you?");
                      //  break;

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

                    case "ella sleep":
                        enableMic.Visibility = System.Windows.Visibility.Hidden;
                        sSynth.Speak("going to sleep");
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
                        sSynth.Speak("voice reset complete");
                        break;


                    case "setup reset":
                        sSynth.Speak("setup reset initialized");
                        myName = null;
                        deleteAllSavedStates();
                        if (string.IsNullOrEmpty(myName))
                        {
                            sSynth.Speak("setup reset complete");
                        }
                        else
                        {
                            sSynth.Speak("setup reset failed");
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
        public void assignNames(String username)
        {

            myName = username;
            saveState();
        }

        //Start of save, load or delete chosen objects.  Refer to SaveLoadStateObjects class
        public void saveState()
        {
            SaveLoadStateObjects saveState = new SaveLoadStateObjects(this);
            saveState.save();

        }
        
        public void loadState()
        {
            SaveLoadStateObjects loadState = new SaveLoadStateObjects(this);
            loadState.load();
        }
      
        public void deleteAllSavedStates()
        {
            SaveLoadStateObjects deleteState = new SaveLoadStateObjects(this);
            deleteState.delete();
        }
        //End of save, load or delete chosen objects.  

        

       
    }
}

