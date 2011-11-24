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

#ifndef NIUI_LIBRARY_EXPORT_H
#define NIUI_LIBRARY_EXPORT_H

//****************************************************************************
//**
//**    NI Unreal Interface.h
//**
//**    Copyright (c) 2011 Matthew Robbins
//**
//**    Author:  Matthew Robbins
//**    Created: 01/2011
//**
//****************************************************************************

//---------------------------------------------------------------------------
// API Export/Import Macros
//---------------------------------------------------------------------------
/** Indicates an exported shared library function. */ 
#define NIUI_API_EXPORT __declspec(dllexport)

/** Indicates an imported shared library function. */ 
#define NIUI_API_IMPORT __declspec(dllimport)

// Declare an API export.
#define NIUI_EXPORT

// API Exporting/ Importing defines.
#ifdef __cplusplus
	#define NIUI_C_API_EXPORT extern "C" NIUI_API_EXPORT
	#define NIUI_C_API_IMPORT extern "C" NIUI_API_IMPORT
	#define NIUI_CPP_API_EXPORT NIUI_API_EXPORT
	#define NIUI_CPP_API_IMPORT NIUI_API_IMPORT
#else
	#define NIUI_C_API_EXPORT NIUI_API_EXPORT
	#define NIUI_C_API_IMPORT NIUI_API_IMPORT
#endif

#ifdef NIUI_EXPORT
	#define NIUI_C_API NIUI_C_API_EXPORT
	#define NIUI_CPP_API NIUI_CPP_API_EXPORT
#else
	#define NIUI_C_API NIUI_C_API_IMPORT
	#define NIUI_CPP_API NIUI_CPP_API_IMPORT
#endif

typedef void (__stdcall * pfn_user_callback)(int nUserId);
typedef void (__stdcall * pfn_focus_callback)(bool focused);
typedef void (__stdcall * pfn_handpoint_callback)(float x, float y, float z);
typedef void (__stdcall * pfn_item_callback)(int item_index, int direction);
typedef void (__stdcall * pfn_value_callback)(float value);

// Data type Forward declarations for the C wrapper.
extern "C"
{
	struct UDKRotator;
	struct UDKVector3;
	struct UserMirror;
	struct UDKString;
}

// -------------------------------------------------------
// The C wrapper functions for accessing the NIUI library.
// -------------------------------------------------------

/** Initilises the OpenNI Context.  */  
NIUI_C_API int InitiliseNIUI(int requestedNumberOfPlayers);

/** Closes the OpenNI Context and deletes references.  */ 
NIUI_C_API void ReleaseNIUI();

/** Updates the NIUI and OpenNI context */
NIUI_C_API void UpdateNIUI();

/** Depth buffer information */
NIUI_C_API void GetDepthBuffer(short* out_map);
NIUI_C_API int GetDepthBufferWidth();
NIUI_C_API int GetDepthBufferHeight();

/** User label map information */
NIUI_C_API void GetUserLabelMap(unsigned char* out_map);
NIUI_C_API int GetUserLabelMapWidth();
NIUI_C_API int GetUserLabelMapHeight();

/** Image buffer information */
NIUI_C_API void GetImageBuffer(float* out_map);
NIUI_C_API int GetImageBufferWidth();
NIUI_C_API int GetImageBufferHeight();

/** User skelton joint information buffer information */
NIUI_C_API void GetSkeletalJointLocation(int userID, int joint, UDKVector3* out_location);
NIUI_C_API void GetLocalSkeletalJointLocation(int userID, int joint, UDKVector3* out_location);
NIUI_C_API void GetUserCentre(int userID, UDKVector3* out_location);
NIUI_C_API void GetBoneAxes(int userID, int joint, UDKVector3* out_forward, UDKVector3* out_side, UDKVector3* out_up);

/** User state polling. Recomended that you use the event processing instead to determine this.*/
NIUI_C_API bool IsUserDetected(int userID);
NIUI_C_API bool IsUserCalibrating(int userID);
NIUI_C_API bool IsUserTracking(int userID);
NIUI_C_API bool IsUserInPose(int userID);

/** Starts calibration pose detection.*/
NIUI_C_API void StartPoseDetection(int userID);
NIUI_C_API void StopPoseDetection(int userID);

/** Starts calibration on users.*/
NIUI_C_API void StartCalibration(int userID);
NIUI_C_API void StopCalibration(int userID);

/** Configures the NIUI core to either accept or discard pose / calibration tests.
*   Use this to flag calibration and user tracking to automatically begin when a
*   user calibrates or a pose is detected.*/
NIUI_C_API void AllowPoseDetection(int enable, int userID);
NIUI_C_API void AllowCalibration(int enable, int userID);
NIUI_C_API void AllowTracking(int enable, int userID);

/** Configures the NIUI core to accept pose, calibration and tracking commands on the user
*   Flags pose detection, calibration and user tracking to begin automatically when calbacks
*   are activated.*/
NIUI_C_API void ActivateUser(int userID);

/** Gets the flat data mirror of the users state.*/
NIUI_C_API void GetUserMirror(int userID, UserMirror* out_mirror);

/** Determines if the user is "dirty", that is either lost but not marked lost.*/
NIUI_C_API bool IsUserDirty(int userID);

/** Functions to retrieve the total amount of users in a certain state.*/
NIUI_C_API int GetNumberOfTrackedUsers();
NIUI_C_API int GetNumberOfCalibratingUsers();
NIUI_C_API int GetNumberOfDetectedUsers();
NIUI_C_API int GetMaxNumberOfUsers();

/** Changes the behaviour of NIUI's debug dumping.*/
NIUI_C_API void SetDebugInfoLevel(int level);
NIUI_C_API void SetDebugFileDump(int enable);
NIUI_C_API void SetConsoleDump(int enable);

/** Event processing.*/
NIUI_C_API int GetTotalEvents();
NIUI_C_API void GetEvent(int number, int* out_user, int* out_event);
NIUI_C_API void ClearEvents();

/** Activates/Deactivates console dumping of events as they are recieved. Recommended that you have this off.*/
NIUI_C_API void EnableEventDumping(int enable);

/** Starts a seperate thread that will continue to track users. Use this when a new level is loading to hold onto user calibration*/
NIUI_C_API void StartIdleMode();

/** Stops the seperate thread used to track users while loading a level. */
NIUI_C_API void StopIdleMode();

NIUI_C_API void GetBasisVectors(UDKVector3* side, UDKVector3* forward, UDKVector3* up);


#endif // #ifndef NIUI_LIBRARY_EXPORT_H