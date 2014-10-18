/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MadWWWFormSender : EditorWindow {

    private WWW request;
    private FinishedCallback callback;

    public static void Execute(WWW request, FinishedCallback callback) {
        var window = GetWindow(typeof(MadWWWFormSender), true) as MadWWWFormSender;
        window.request = request;
        window.callback = callback;

        window.autoRepaintOnSceneChange = true;
        window.minSize = new Vector2(200, 200);
        window.Show();
    }

    void Update() {
        if (request != null) {
            if (!request.isDone) {
                //StartCoroutine(ExecuteRequest(request));
            }
        }
    }

    IEnumerator ExecuteRequest() {
        yield return request;
        if (callback != null) {
            callback(request);
        }
        Close();
    }

    public delegate void FinishedCallback(WWW request);
}