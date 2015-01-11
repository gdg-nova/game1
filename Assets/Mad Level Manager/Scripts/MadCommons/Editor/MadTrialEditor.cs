/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using System;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using MadLevelManager;
using System.Security.Cryptography;
using System.Text;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

[InitializeOnLoad]
public class MadTrialEditor : Editor {

    public const string KeyPrefix = "_TRIAL_KEY_PREFIX_";
    public const string StartKey = "Start";
    public const string DayKey = "Day";

    public const string PurchaseUrl = "_TRIAL_PURCHASE_URL_";
    public const string TrialDuration = "_TRIAL_DURATION_"; // in seconds
    public const string TrialKey = "_TRIAL_KEY_";
    //public const string PurchaseUrl = "http://madpixelmachine.com/";
    //public const string TrialDuration = "100"; // in seconds
    //public const string TrialKey = "qc47yrb734vc";

    private static int duration;

    static MadTrialEditor() {
        if (!isTrialVersion) {
            return;
        }

        duration = int.Parse(TrialDuration);

        if (!TrialStarted()) {
            StartTrial();
            NotifyTrialStarted();
        } else if (expired) {
            if (CanNotifyTrialEnded()) {
                NotifyTrialEnded();
            }
        }

    }

    public static bool isTrialVersion {
        get {
            if (TrialDuration.StartsWith("_")) {
                return false;
            } else {
                return true;
            }
        }
    }

    public static bool expired {
        get {
            if (!isTrialVersion) {
                Debug.LogError("This is not a trail version");
                return true;
            }

            int startDate = EditorPrefs.GetInt(KeyPrefix + StartKey);
            int now = Now();

            if (now - startDate <= duration) {
                return false;
            }

            return true;
        }
    }

    private static bool TrialStarted() {
        return EditorPrefs.HasKey(KeyPrefix + StartKey);
    }

    private static void StartTrial() {
        int startTime = Now();
        EditorPrefs.SetInt(KeyPrefix + StartKey, startTime);
    }

    private static int Now() {
        return (int) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
    }

    private static void NotifyTrialStarted() {
        Notify("TrialStarted");
    }

    private static bool CanNotifyTrialEnded() {
        int day = DateTime.UtcNow.DayOfYear;
        bool result = false;
        if (EditorPrefs.GetInt(KeyPrefix + DayKey, -1) != day) {
            result = true;
        }

        EditorPrefs.SetInt(KeyPrefix + DayKey, day);

        return result;
    }

    private static void NotifyTrialEnded() {
        Notify("TrialEnded");
    }

    private static void Notify(string methodName) {
        string ns = typeof(MadTrialEditor).Namespace;
        Type t;

        if (!string.IsNullOrEmpty(ns)) {
            t = Type.GetType(ns + ".TrialInfo", true);
        } else {
            t = Type.GetType("TrialInfo", true);
        }

        if (t != null) {
            MethodInfo method = t.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
            if (method != null) {
                method.Invoke(null, null);
            }
        }
        
    }

    public static void OnEditorGUIExpired(string toolName) {
        MadGUI.Error("This is an evaluation version of " + toolName + " and your evaluation period has expired. If you want to continue using " + toolName + " please purchase it or send a request for another evaluation period.");

        EditorGUILayout.Space();

        if (MadGUI.Button("Purchase", Color.yellow)) {
            Application.OpenURL(PurchaseUrl);
        }

        EditorGUILayout.Space();

        if (MadGUI.Button("Request Another Evaluation Period", Color.magenta)) {
            RequestExtend(toolName);
        }

        if (MadGUI.Button("I Have An Evaluation Key!", Color.green)) {
            var builder = new MadInputDialog.Builder("Enter Key", "Enter new evaluation key.", (key) => {
                if (!string.IsNullOrEmpty(key)) {
                    Extend(key.Trim());
                }
            });
            builder.BuildAndShow();
        }
    }

    public static void RequestExtend(string toolName) {
        var builder = new MadInputDialog.Builder("Request Evalutaion", "Please enter your e-mail address. New evaluation key will be sent to it.", (email) => {
            if (!string.IsNullOrEmpty(email)) {
                Evaluate(email, toolName);
            }
        });
        builder.BuildAndShow();
    }

    private static void Evaluate(string email, string toolName) {
        var form = new WWWForm();
        form.AddField("email", email);
        form.AddField("tool", toolName);

        EditorUtility.DisplayProgressBar("Sending request...", "Seding request...", -1);
        var www = new WWW("http://docs.madpixelmachine.com/trial/request.php", form);
        while (!www.isDone) {
            System.Threading.Thread.Sleep(500);
        }
        EditorUtility.ClearProgressBar();
        
        if (www.text.Trim() == "1") {
            EditorUtility.DisplayDialog("Request Sent!", "Request has been sent! You will shortly receive a response to your e-mail address.", "OK");
        } else {
            EditorUtility.DisplayDialog("Error Sending Request", "Couldn't send request. Please write to support@madpixelmachine.com", "OK");
            Debug.LogError(www.text);
        }
    }

    public static void Extend(string key) {
        int newStart;
        if (LoadKey(key, out newStart)) {
            EditorPrefs.SetInt(KeyPrefix + StartKey, newStart);
            EditorUtility.DisplayDialog("Evaluation Extended", "Evaluation extended. Have fun!", "OK");
        } else {
            EditorUtility.DisplayDialog("Invalid Key", "Given key is invalid.", "OK");
        }
    }

    static bool LoadKey(string key, out int newStart) {
        if (!key.Contains("-")) {
            newStart = 0;
            return false;
        }

        var parts = key.Split('-');
        string first = parts[0];
        string second = parts[1];

        using (MD5 md5 = MD5.Create()) {
            var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(first + TrialKey));
            var sb = new StringBuilder();
            foreach (byte b in bytes) {
                sb.Append(b.ToString("x2"));
            }

            if (sb.ToString() != second) {
                newStart = 0;
                return false;
            }
        }

        newStart = int.Parse(first, System.Globalization.NumberStyles.HexNumber);
        return true;
    }


    public static int DaysLeft() {
        int startDate = EditorPrefs.GetInt(KeyPrefix + StartKey);
        int now = Now();

        int timeLeft = duration - (now - startDate);
        int daysLeft = timeLeft / (3600 * 24);

        return daysLeft;
    }
}

#if !UNITY_3_5
} // namespace
#endif