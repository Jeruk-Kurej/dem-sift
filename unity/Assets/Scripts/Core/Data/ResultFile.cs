using System;
using System.Collections.Generic;
using System.IO;
using DEMSIFT.Player;
using DEMSIFT.Scoring;
using UnityEngine;

namespace DEMSIFT.Data
{
    public static class ResultFile
    {
        private const string FileName = "hasil-pengetahuan.txt";
        private const string Separator = "\t";
        private const string TimestampFormat = "yyyy-MM-dd HH:mm:ss";

        public static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

        public static void AppendCurrentPlayer()
        {
            if (!File.Exists(FilePath))
            {
                File.WriteAllText(FilePath, Header() + Environment.NewLine);
            }

            File.AppendAllText(FilePath, CurrentPlayerLine() + Environment.NewLine);
        }

        // --- Line format ---
        private static string Header()
        {
            var columns = new List<string> { "waktu", "nama", "kelas" };
            for (int soal = 1; soal <= ScoreSession.SoalCount; soal++) columns.Add($"skor{soal}");
            for (int soal = 1; soal <= ScoreSession.SoalCount; soal++) columns.Add($"salah{soal}");
            columns.Add("total");
            return string.Join(Separator, columns);
        }

        private static string CurrentPlayerLine()
        {
            var columns = new List<string>
            {
                DateTime.Now.ToString(TimestampFormat),
                Clean(PlayerSession.PlayerName),
                Clean(PlayerSession.ClassName)
            };
            for (int soal = 1; soal <= ScoreSession.SoalCount; soal++) columns.Add(ScoreSession.ScoreOf(soal).ToString());
            for (int soal = 1; soal <= ScoreSession.SoalCount; soal++) columns.Add(ScoreSession.WrongAttemptsOf(soal).ToString());
            columns.Add(ScoreSession.Total.ToString());
            return string.Join(Separator, columns);
        }

        private static string Clean(string value)
        {
            return value.Replace('\t', ' ').Replace('\n', ' ').Replace('\r', ' ');
        }
    }
}
