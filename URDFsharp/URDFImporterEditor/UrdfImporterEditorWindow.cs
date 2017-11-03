﻿/*
© Siemens AG, 2017
Author: Dr. Martin Bischoff (martin.bischoff@siemens.com)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

<http://www.apache.org/licenses/LICENSE-2.0>.

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/


using System.IO;
using System.Threading;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using RosBridgeClient;
using Urdf;

public class UrdfImporterEditorWindow : EditorWindow
{
    private string address = "ws://192.168.64.3:9090";
    private string timeout = "10";
    private string urdfAssetPath = "/Users/warriermac/ros_ws/KinovaROSSharp/Assets/Urdf/";

    private Thread rosSocketConnectThread;
    private Thread urdfImportThread;
    private UrdfImporter urdfImporter;

    private Dictionary<string, ManualResetEvent> status = new Dictionary<string, ManualResetEvent>
    {
        { "connected", new ManualResetEvent(false) },
        { "robotNameReceived",new ManualResetEvent(false) },
        { "robotDescriptionReceived", new ManualResetEvent(false) },
        { "resourceFilesReceived", new ManualResetEvent(false) },
        { "disconnected", new ManualResetEvent(false) },
        { "databaseRefreshStarted", new ManualResetEvent(false) },
        { "databaseRefreshed", new ManualResetEvent(false) },
        { "importModelDialogShown", new ManualResetEvent(false) },
        { "jointStatesMapped", new ManualResetEvent(false) },
    };

    [MenuItem("ROSbridge/Import URDF Assets...")]
    private static void Init()
    {
        UrdfImporterEditorWindow urdfImport = GetWindow<UrdfImporterEditorWindow>();
        urdfImport.Show();
    }

    private void OnGUI()
    {        
        GUILayout.Label("ROSbridge WebSocket", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = 100;
        address = EditorGUILayout.TextField("Address", address);
        timeout = EditorGUILayout.TextField("Timeout [s]", timeout);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(20);
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Read Robot Description."))
        {
            rosSocketConnectThread = new Thread(() => rosSocketConnect());
            rosSocketConnectThread.Start();
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(20);

        EditorGUIUtility.labelWidth = 300;

        drawLabelField("1. ROSbridge_server Connected:", "connected");
        drawLabelField("2. Robot Name Received:", "robotNameReceived");
        drawLabelField("3. Robot Description Received:", "robotDescriptionReceived");
        drawLabelField("4. Resource Files Received:", "resourceFilesReceived");
        drawLabelField("5. ROSBridge_server Disconnected:", "disconnected");
        drawLabelField("6. Asset Database Refresh Completed:", "databaseRefreshed");
        drawLabelField("7. Joint States Mapping Completed:", "jointStatesMapped");
    }

    private void drawLabelField(string label, string stage)
    {
        GUIStyle guiStyle = new GUIStyle(EditorStyles.textField);
        bool state = status[stage].WaitOne(0);
        guiStyle.normal.textColor = state ? Color.green : Color.red;
        EditorGUILayout.LabelField(label, state ? "done" : "open", guiStyle);
    }

    private void rosSocketConnect()
    {
        // intialize
        foreach (ManualResetEvent manualResetEvent in status.Values)
            manualResetEvent.Reset();

        // connectto ROSbridge
        RosSocket rosSocket = new RosSocket(address);
        status["connected"].Set();

        // setup urdfImporter
        urdfImporter = new UrdfImporter(rosSocket, urdfAssetPath);
        status["robotNameReceived"] = urdfImporter.Status["robotNameReceived"];
        status["robotDescriptionReceived"] = urdfImporter.Status["robotDescriptionReceived"];
        status["resourceFilesReceived"] = urdfImporter.Status["resourceFilesReceived"];
        urdfImportThread = new Thread(() => urdfImporter.Import());
        urdfImportThread.Start();

        // import URDF assets:
        if (status["resourceFilesReceived"].WaitOne(int.Parse(timeout) * 1000))
            Debug.Log("Imported urdf resources to " + urdfImporter.LocalDirectory);
        else
            Debug.LogWarning("Not all resource files have been received before timeout.");

        // close the ROSBridge socket
        rosSocket.Close();
        status["disconnected"].Set();

    }

    private void TeleoperationPatcher()
    {
        GameObject robot = GameObject.Find(urdfImporter.robotName);
        
        if (robot != null)
        {
            SpotLightPatcher.patch(robot);
            KinematicPatcher.patch(robot);
            OdometryPatcher.patch(robot);
            JointStatePatcher.patch(robot);

            var rosConnector = new GameObject("ROSConnector");
            rosConnector.AddComponent<RosConnector>().RosBridgeServerUrl = address;
            rosConnector.AddComponent<JointStateSubscriber>();

            Application.runInBackground = true;

            status["jointStatesMapped"].Set();
        }
        else
        {
            Debug.LogWarning("Joint States Patching did not work! Robot not found...");
        }
    }

    private void OnInspectorUpdate()
    {
        // some methods can only be called from main thread:
        // We check the status to call the methods at the right step in the process:

        Repaint();

        // import Model
        if (status["databaseRefreshed"].WaitOne(0) && !status["importModelDialogShown"].WaitOne(0))
        {
            status["importModelDialogShown"].Set();
            if (EditorUtility.DisplayDialog(
                "Urdf Assets imported.",
                "Do you want to generate a " + urdfImporter.robotName + " GameObject now?",
                "Yes", "No"))
            {
                Model.CreateModel(Path.Combine(urdfImporter.LocalDirectory, "robot_description.urdf"));
                TeleoperationPatcher();
            }
                
        }

        // refresh Asset Database 
        if (status["resourceFilesReceived"].WaitOne(0) && !status["databaseRefreshed"].WaitOne(0))
        {
            status["databaseRefreshStarted"].Set();
            AssetDatabase.Refresh();
            status["databaseRefreshed"].Set();
        }
    }
}
