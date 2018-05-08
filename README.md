# Crane Operator Universal App

This sample illustrates for emerging Mixed Reality headsets can be used for virtual training. 

In this scenario:   
- The crane operator is using an immersive headset to practice operating  the crane.  
- The instructor is using a Hololens device and can connect to multiple students' devices, and monitor their progress. The instructor can also interact with the students' activity, altering the difficulty of the tests. 

## How to build the demonstrator  


### Pre-requisites 
You will need all the pre-requisites to build Mixed Reality apps. Follow these instructions: https://docs.microsoft.com/en-us/windows/mixed-reality/install-the-tools 

You can use the Hololens emulator if you do not have a hololens device; or, you can also export as a desktop app (by unchecking the VR settings in Unity).  
The occluded headset is highly recommended to get the immersive experience. 

### Building the sample
A single universal binary can be used for Hololens and Windows Mixed Reality. Just build from Unity within Unity with VR settings enabled. 

To build the Unity scene, you need an extra .unitypackage with the city. We can't include it in the repo because it is too large (> 100 MB), but you can get it from [this onedrive folder](https://1drv.ms/f/s!AkqhcmztwT4Sk8Bl9YX7InEuMvfCJw). 
The file is called StreamingAssetsCity.unitypackage.  Just import it into your package; all contents go into the streaming assets folder. 


## Build 2018 Conference extras 
For build 2018, we added some silliness (raining meatballs) and some furious birds that dropped missiles attacking the operator. This code is in the "buildsilliness" branch. 

 

