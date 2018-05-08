// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using HoloToolkit.Unity.InputModule;


    public class TapResponder : MonoBehaviour, IInputClickHandler
    {   
        public GameManager gameManager;
        public void OnInputClicked(InputClickedEventData eventData)
        {
            // Increase the scale of the object just as a response.


            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }
    }
