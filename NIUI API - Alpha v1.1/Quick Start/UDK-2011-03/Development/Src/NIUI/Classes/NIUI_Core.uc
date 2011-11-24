/*****************************************************************************
*                                                                            *
*  Natural Interaction Unreal Interface (NIUI) Alpha                         *
*  Copyright (C) 2011 Matthew Robbins                                        *
*                                                                            *
*  This file is part of NIUI.                                                *
*                                                                            *
*  NIUI is free software: you can redistribute it and/or modify              *
*  it under the terms of the GNU Lesser General Public License as published  *
*  by the Free Software Foundation, either version 3 of the License, or      *
*  (at your option) any later version.                                       *
*                                                                            *
*  NIUI is distributed in the hope that it will be useful,                   *
*  but WITHOUT ANY WARRANTY; without even the implied warranty of            *
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the              *
*  GNU Lesser General Public License for more details.                       *
*                                                                            *
*  You should have received a copy of the GNU Lesser General Public License  *
*  along with NIUI. If not, see <http://www.gnu.org/licenses/>.              *
*                                                                            *
*****************************************************************************/


/**
*
*   NIUICore.uc
*
*   Copyright (c) 2011 Matthew Robbins
*
*   Author:  Matthew Robbins
*   Created: 01/2011
*
*   Desc: NIUI_Core is the core of the NIUI library on the UnrealScript side.
*   It binds all needed functions to the NIUI library, handles event 
*   processing, user detection etc.
*   Your NIUI_Core object should exist in your main game info class.
*/

class NIUI_Core extends Actor
	hidecategories(Movement,Display,Collision,Attachment,Physics,Advanced,Debug, Object, Actor) // Hide the catergories within the editor.
	DLLBind(NIUI);

enum NIUI_BoneID
{
	NIUI_JOINT_HEAD,                // 0
	NIUI_JOINT_NECK,                // 1
	NIUI_JOINT_TORSO,               // 2
	NIUI_JOINT_WAIST,               // 3
	NIUI_JOINT_LEFT_COLLAR,         // 4
	NIUI_JOINT_LEFT_SHOULDER,       // 5
	NIUI_JOINT_LEFT_ELBOW,          // 6
	NIUI_JOINT_LEFT_WRIST,          // 7
	NIUI_JOINT_LEFT_HAND,           // 8
	NIUI_JOINT_LEFT_FINGERTIP,      // 9
	NIUI_JOINT_RIGHT_COLLAR,        // 10
	NIUI_JOINT_RIGHT_SHOULDER,      // 11
	NIUI_JOINT_RIGHT_ELBOW,         // 12
	NIUI_JOINT_RIGHT_WRIST,         // 13
	NIUI_JOINT_RIGHT_HAND,          // 14
	NIUI_JOINT_RIGHT_FINGERTIP,     // 15
	NIUI_JOINT_LEFT_HIP,            // 16
	NIUI_JOINT_LEFT_KNEE,           // 17
	NIUI_JOINT_LEFT_ANKLE,          // 18
	NIUI_JOINT_LEFT_FOOT,           // 18
	NIUI_JOINT_RIGHT_HIP,           // 20
	NIUI_JOINT_RIGHT_KNEE,          // 21
	NIUI_JOINT_RIGHT_ANKLE,         // 22
	NIUI_JOINT_RIGHT_FOOT,          // 23

	NIUI_JOINT_COUNT,               // 24
};


enum NIUI_Event
{
	NIUI_EVENT_UserLost,                /* A previously detected user was lost. */
	NIUI_EVENT_UserDetected,            /* A new user was detected. */
	NIUI_EVENT_UserPoseDetected,        /* A user has been detected in the calibration pose. */
	NIUI_EVENT_UserPoseLost,            /* A user previously detected in the calibration pose is no longer in the pose. */
	NIUI_EVENT_CalibrationStart,        /* A user has started calibration. */
	NIUI_EVENT_CalibrationSuccess,      /* A user has been sucessfully calibrated. Skeletal mapping / tracking begins after this. */
	NIUI_EVENT_CalibrationFail,         /* An attempted user calibration has failed. Likely the user left the calibration pose or the camera lost sight of the user. */
	NIUI_EVENT_TrackingStart,           /* A user has started to be tracked. IE Skeletal mapping is being applied. */

	NIUI_EVENT_Count,
};

enum NIUI_DebugLevel
{
	NIUI_LogAll,            // All information regarding NIUI is outputed.
	NIUI_WarningAndAbove,   // Things that are problems and critical errors are outputed.
	NIUI_ErrorOnly,         // Only Critical errors (things that will crash the game) are outputed.
	NIUI_LogNone,           // Nothing is outputed.
};

var const int NIUI_SUCCESS;
var const int NIUI_FAIL;

var const int INVALID_NIUI_HANDLE;

// The next handle to assign the next registered NIUI_CallbackInterface.
var private int nextCallbackHandle;

// If classes that implement NIUI_DependencyInterface have been notified of core creation.
var private bool HasNotifiedDependencies;


//***********************************************************************************************************
//                                                  NIUI Event Handlling.
//***********************************************************************************************************

/** The array of actors that implement the CallbackInterface that are to be informed of events. */
var private array<NIUI_CallbackInterface> NIUIEventListeners;

//***********************************************************************************************************
//                                                  NIUI API IMPORTING
//***********************************************************************************************************

/** Initilises NIUI and OpenNI contexts.  */
dllimport final function int InitiliseNIUI(int requestedNumberOfPlayers);

/** Destroys the NIUI and OpenNI contexts.  */
dllimport final function  ReleaseNIUI();

/** Updates the NIUI and OpenNI contexts.  */
dllimport final function UpdateNIUI();

/** Retrieves the position in a space relative to the depth camera device. */
dllimport final function GetSkeletalJointLocation(int userID, int joint, out Vector out_location);

/** Retrieves the position in a space relative to the users centre of mass. */
dllimport final function GetLocalSkeletalJointLocation(int userID, int joint, out Vector out_location);

/** Retrieves the users centre of mass relative to the depth camera. */
dllimport final function GetUserCentre(int userID, out Vector out_location);

/** Retrieves the X, Y and Z Axis of the bone. Pass this into OrthoRotation to derive a rotation. */
dllimport final function GetBoneAxes(int userID, int joint, out Vector out_forward, out Vector out_side, out Vector out_up);

/** 
 *  Simplified user state querying. 
 *  Strongly recomended that you use the event broadcasting system if possible.
 *  */
dllimport final function bool IsUserDetected(int userID);
dllimport final function bool IsUserCalibrating(int userID);
dllimport final function bool IsUserTracking(int userID);
dllimport final function bool IsUserInPose(int userID);

/** Starts calibration pose detection.*/
dllimport final function StartPoseDetection(int userID);

/** Stops calibration pose detection. */
dllimport final function StopPoseDetection(int userID);

/** Starts calibration on users.*/
dllimport final function StartCalibration(int userID);

/** Stops calibration. */
dllimport final function StopCalibration(int userID);

/** Configures the NIUI core to either accept or discard pose / calibration tests.
*   Use this to flag calibration and user tracking to automatically begin when a
*   user calibrates or a pose is detected.*/
dllimport final function AllowPoseDetection(int enable, int userID);
dllimport final function AllowCalibration(int enable, int userID);
dllimport final function AllowTracking(int enable, int userID);

/** Configures the NIUI core to automatically accept pose, calibration and tracking commands on the user.
*   Flags pose detection, calibration and user tracking to begin automatically when calbacks
*   are activated.*/
dllimport final function ActivateUser(int userID);

/** Returns the amount of user's currently being tracked (ie: Skeleton Tracking)*/
dllimport final function int GetNumberOfTrackedUsers();

/** Returns the total amount of events in the processing queue */
dllimport final function int GetNumberOfCalibratingUsers();
dllimport final function int GetNumberOfDetectedUsers();
dllimport final function int GetMaxNumberOfUsers();

/** Changes the behaviour of NIUI's debug dumping.
 *  By default, is set to NIUI_Log_All.
 *  See enum above.*/
dllimport final function SetDebugInfoLevel(int level);

/** Enables / Disables dumping the debug file.
 *  Is enabled by default. */
dllimport final function SetDebugFileDump(int enable);

/** Enables / Disables console debug dumping.
 *  Is enabled by default */
dllimport final function SetConsoleDump(int enable);

/** Returns the total amount of events in the processing queue */
dllimport final function int GetTotalEvents();

/** Gets the event from NIUI and the user it concerns. */
dllimport final function GetEvent(int number, out int out_user, out int out_event);

/** Returns the total amount of events in the processing queue */
dllimport final function ClearEvents();

/** Activates/Deactivates console dumping of events as they are recieved. Recommended that you have this off.*/
dllimport final function EnableEventDumping(int enable);


//***********************************************************************************************************

/** On construction of the NIUICore object, the NIUI and OpenNI context's are created in native code. 
*   The OpenNI context creates a Depth-Map production node, a User-Generator (skeletal information) 
*   production node and also a Image-Generator production node. */
function bool Initilise()
{
	local int result;

	`log("*************************************************");

	// Configure NIUI to spit out everything to the debug log.
	SetDebugLevel(NIUI_LogAll);

	// Configure NIUI to dump the log file.
	SetDebugFileDump(1);

	result  = InitiliseNIUI(2);
	if (result != NIUI_SUCCESS)
	{
		`log("NIUI has failed to initilise. Please check the output log for details.");
		return false;
	}

	nextCallbackHandle = 0;

	// Turn on event dumping to the console.
	EnableEventDumping(1);

	`log("*************************************************");

	HasNotifiedDependencies = false;
	return true;
}

function ShutdownNIUI()
{
	// Alert dependencies that the core is no longer safe to use.
	NotifyDependenciesOfDestruction();
	ReleaseNIUI();
}

function NotifyDependenciesOfCreation()
{
	local Actor Act;
	local NIUI_DependencyInterface Dependency;

	foreach AllActors(class'Actor', Act, class'NIUI_DependencyInterface')
	{
		Dependency = NIUI_DependencyInterface(Act);
		if (Dependency != none)
		{
			`log("NIUI Dependency found of class" @Act.class $". Notifying of Core Creation.");
			Dependency.OnNIUICoreStart(self);
		}
	}

	HasNotifiedDependencies = true;
}

function NotifyDependenciesOfDestruction()
{
	local Actor Act;
	local NIUI_DependencyInterface Dependency;

	foreach AllActors(class'Actor', Act, class'NIUI_DependencyInterface')
	{
		Dependency = NIUI_DependencyInterface(Act);
		if (Dependency != none)
		{
			`log("NIUI Dependency found of class" @Act.class$". Notifying of Core Destruction.");
			Dependency.OnNIUICoreShutdown();
		}
	}
}

function bool ShouldNotifyDepencies()
{
	return !HasNotifiedDependencies;
}

function UpdateNIUICore(float DeltaTime)
{
	UpdateNIUI();
	UpdateAndBroadcastEvents();
}

private function UpdateAndBroadcastEvents()
{
	local int eventCount;
	local int iter;
	local int eventUserID;
	local int theEvent;

	eventCount = GetTotalEvents();

	if (eventCount > 0 && NIUIEventListeners.Length > 0)
	{
		for (iter = 0; iter < eventCount; iter++)
		{
			GetEvent(iter, eventUserID, theEvent);
			BroadCastEvent(eventUserID, theEvent);
		}

		// Clear all events recieved from the dll.
		ClearEvents();
	}
}

private function BroadCastEvent(int userID, int theEvent)
{
	local int iter;
	for (iter = 0; iter < NIUIEventListeners.Length; iter++)
	{
		NIUIEventListeners[iter].OnNIUIEvent(theEvent, userID);
	}
}

private function int GetNextCallbackHandle()
{
	local int next;

	next  = nextCallbackHandle;
	nextCallbackHandle++;

	return next;
}

function RegisterEventListener(NIUI_CallbackInterface TheListener)
{
	TheListener.SetNIUIHandle(GetNextCallbackHandle());
	NIUIEventListeners.AddItem(TheListener);
}

function ReleaseEventListener(NIUI_CallbackInterface TheListener)
{
	local int iter;
	local int handle;
	handle = TheListener.GetNIUIHandle();

	for (iter = 0; iter < NIUIEventListeners.Length; iter++)
	{
		if (NIUIEventListeners[iter].GetNIUIHandle() == handle)
		{
			NIUIEventListeners.Remove(iter, 1);
			return;
		}
	}
}

// Wrapper function to alter the debug level.
function SetDebugLevel(NIUI_DebugLevel level)
{
	SetDebugInfoLevel(int(level));
}

// Wrapper function to retrieve bone orientation in local space.
// Use this instead of the GetSkeletalJointRotation function.
function Rotator GetBoneRotation(int userID, int boneID)
{
	local Vector xAxis, yAxis, zAxis;
	local Rotator theRotation, Temp;

	self.GetBoneAxes(userID, boneID, xAxis, yAxis, zAxis);

	Temp = OrthoRotation(xAxis, yAxis, zAxis);
	theRotation = Temp;
	theRotation.Yaw = Temp.Yaw * -1;
	theRotation.Pitch = Temp.Pitch * -1;

	return theRotation;
}

event Destroyed()
{
	ShutdownNIUI();
}

DefaultProperties
{
	// The invalid NIUI handle type.
	INVALID_NIUI_HANDLE = -1;
	
	NIUI_SUCCESS = 1;
	NIUI_FAIL = -1;
}
