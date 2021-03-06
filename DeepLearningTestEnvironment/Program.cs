﻿using System;
using System.Collections.Generic;
using System.IO;
using DeepLearning.MathSyntax;


namespace DeepLearning
{
    static class Program
    {
        class CharSpace
        {
            public CharSpace(int number)
            {
                for (int i = 97; i < 123; i++)
                {
                    letters.Add(new ArgumentValue(number.ToString() + "." + ((char)i).ToString()));
                }
            }
            public List<ArgumentValue> letters = new List<ArgumentValue>();

            public void SetLetter(char letter)
            {
                foreach(var i in letters)
                {
                    i.Value = 0;
                }
                if((int)letter <97 || (int)letter > 122)
                {
                    return;
                }
                letters[(int)letter - 97].Value = 1;
            }
        }

        [STAThreadAttribute]
        static void Main()
        {

            var EngWord = File.ReadAllLines("EN.filtered");
            var ITWord = File.ReadAllLines("IT.filtered");

            List<CharSpace> Wordspace = new List<CharSpace>();
            for (int i = 0; i < 8; i++)
            {
                Wordspace.Add(new CharSpace(i));
            }
             
            var allinputs = new List<ArgumentValue>();
            foreach(var i in Wordspace)
            {
                allinputs.AddRange(i.letters);
            }

            OutputData English, Italian;
            English = new OutputData();
            Italian = new OutputData();

            var outputs = new List<OutputData>();
            outputs.Add(English);
            outputs.Add(Italian);
            Italian.MustBeHigh = true;

            bool makeNew = true;

            NeuralNetwork LanguageNeuralNet;

            if (!makeNew)
            {
                //LanguageNeuralNet = NeuralNetwork.Load(allinputs, outputs);
                LanguageNeuralNet = NeuralNetwork.LoadMatrix(allinputs, outputs);
            }
            else
            {
                System.Diagnostics.Stopwatch builder = new System.Diagnostics.Stopwatch();
                builder.Start();
                LanguageNeuralNet = new NeuralNetwork(allinputs, outputs, new int[] { 10 });
                builder.Stop();

                System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                timer.Start();

                int counterEN, counterIT;
                counterEN = 0;
                counterIT = 0;
                var random = new Random();
                int Right, Wrong;
                Right = 0;
                Wrong = 0;

                while (counterEN + counterIT < EngWord.Length + ITWord.Length)
                {
                    string word;
                    if (random.Next() % 2 == 0)
                    {
                        if (counterEN != EngWord.Length)
                        {
                            word = EngWord[counterEN];
                            counterEN++;
                            English.MustBeHigh = true;
                            Italian.MustBeHigh = false;
                        }
                        else
                        {
                            word = ITWord[counterIT];
                            counterIT++;
                            English.MustBeHigh = false;
                            Italian.MustBeHigh = true;
                        }
                    }
                    else
                    {
                        if (counterIT != ITWord.Length)
                        {
                            word = ITWord[counterIT];
                            counterIT++;
                            English.MustBeHigh = false;
                            Italian.MustBeHigh = true;
                        }
                        else
                        {
                            word = EngWord[counterEN];
                            counterEN++;
                            English.MustBeHigh = true;
                            Italian.MustBeHigh = false;
                        }
                    }

                    for (int characternum = 0; characternum < 8; characternum++)
                    {
                        Wordspace[characternum].SetLetter(word[characternum]);
                    }
                    LanguageNeuralNet.Learn();
                    LanguageNeuralNet.CalculateResults();
                    if (English.Value > Italian.Value)
                    {
                        if (English.MustBeHigh)
                            Right++;
                        else
                            Wrong++;
                    }
                    else
                    {
                        if (Italian.MustBeHigh)
                            Right++;
                        else
                            Wrong++;
                    }
                }
                timer.Stop();
                LanguageNeuralNet.SaveMatrix();
            }
            
            int definitiveRight = 0;
            int definitiveWrong = 0;
            foreach(var i in EngWord)
            {
                for (int characternum = 0; characternum < 8; characternum++)
                {
                    Wordspace[characternum].SetLetter(i[characternum]);
                }
                LanguageNeuralNet.CalculateResults();
                if (English.Value > Italian.Value)
                    definitiveRight++;
                else
                    definitiveWrong++;
            }
            foreach (var i in ITWord)
            {
                for (int characternum = 0; characternum < 8; characternum++)
                {
                    Wordspace[characternum].SetLetter(i[characternum]);
                }
                LanguageNeuralNet.CalculateResults();
                if (Italian.Value > English.Value)
                    definitiveRight++;
                else
                    definitiveWrong++;
            }
            double endPercentage = (double)definitiveRight / (double)(definitiveRight + definitiveWrong) * 100.0;
        }
    }
}
