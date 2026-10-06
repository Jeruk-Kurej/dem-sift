using System;
using DEMSIFT.Data;
using TMPro;
using UnityEngine;

namespace DEMSIFT.Admin
{
    public class AdminPanel : MonoBehaviour
    {
        [SerializeField] private GameObject resetConfirm;
        [SerializeField] private TMP_Text statusLabel;

        // --- Called by buttons ---
        public void Open()
        {
            resetConfirm.SetActive(false);
            statusLabel.text = string.Empty;
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void AskReset()
        {
            if (HasData()) resetConfirm.SetActive(true);
        }

        public void CancelReset()
        {
            resetConfirm.SetActive(false);
        }

        public void ConfirmReset()
        {
            resetConfirm.SetActive(false);
            statusLabel.text = $"Data direset. Arsip: {ResultFile.Archive()}";
        }

        public void Export()
        {
            if (HasData()) statusLabel.text = $"Tersimpan: {ResultFile.ExportCsv()}";
        }

        public void OpenFolder()
        {
            Application.OpenURL(new Uri(ResultFile.FolderPath).AbsoluteUri);
        }

        private bool HasData()
        {
            if (ResultFile.Exists) return true;

            statusLabel.text = "Belum ada data.";
            return false;
        }
    }
}
