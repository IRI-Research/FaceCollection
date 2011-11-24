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


//****************************************************************************
//**
//**    NIUI Library Export.cpp
//**
//**    Copyright (c) 2011 Matthew Robbins
//**
//**    Author:  Matthew Robbins
//**    Created: 01/2011
//**
//****************************************************************************

#include "NIUI Library Export.h"

#include <XnOpenNI.h>
#include <XnCppWrapper.h>
#include <XnTypes.h>

#include "NIUI.h"
#include "NIUICore.h"
#include "NIUIEventManager.h"
#include "NIUIUser.h"
#include "NIUIDataTypes.h"
#include "NIUIConversion.h"
#include "Logger.h"

using namespace NIUI;

						
static Core g_core;
static EventManager g_eventManager;

XnChar g_strPose[20] = "";

// Dummy stuff as far as i am aware.
static XnCallbackHandle		g_hUserCallbacks		= NULL;
static XnCallbackHandle		g_hCalibrationCallbacks = NULL;

XnCallbackHandle hUserCallbacks, hCalibrationCallbacks, hPoseCallbacks;

//-----------------------------------------------------------------------------
// OpenNI event handlers
//-----------------------------------------------------------------------------

static void XN_CALLBACK_TYPE OnNewUser(xn::UserGenerator& generator, const XnUserID nUserId, void* pCookie)
{
	//printf("OnNewUser. %i", nUserId);
	g_core.OnNewUser(nUserId);
}

static void XN_CALLBACK_TYPE OnLostUser(xn::UserGenerator& generator, const XnUserID nUserId, void* pCookie)
{	
	//printf("OnLostUser.");
	g_core.OnLostUser(nUserId);
}

// Callback: Detected a pose
static void XN_CALLBACK_TYPE UserPoseDetected(xn::PoseDetectionCapability& capability, const XnChar* strPose, XnUserID nId, void* pCookie)
{
	//printf("UserPoseDetected.");
	g_core.OnUserPoseDetected(nId);
}

static void XN_CALLBACK_TYPE UserPoseLost(xn::PoseDetectionCapability& capability, const XnChar* strPose, XnUserID nId, void* pCookie)
{
	//printf("UserPoseLost.");
	g_core.OnUserPoseLost(nId);
}

static void XN_CALLBACK_TYPE OnCalibrationStart(xn::SkeletonCapability& skeleton, const XnUserID nUserId, void* pCookie)
{
	//printf("OnCalibrationStart.");
	g_core.OnCalibrationStart(nUserId);
}

static void XN_CALLBACK_TYPE OnCalibrationEnd(xn::SkeletonCapability& skeleton, const XnUserID nUserId, XnBool bSuccess, void* pCookie)
{
	//printf("OnCalibrationEnd.");
	g_core.OnCalibrationEnd(nUserId, bSuccess, skeleton);
}

//-----------------------------------------------------------------------------
// Idle thread function for when a level is loading. This will hold onto all user
// calibrations.
//-----------------------------------------------------------------------------

static bool useIdleMode = false;
static bool inIdleMode = true;
extern "C" DWORD IdleMode(LPVOID param);
HANDLE idleThread = 0;

void StartIdleMode()
{
	LOG("Idle mode has been triggered to start.", LOG_Standard);

	if (idleThread != 0)
	{
		// Try close the existing thread.
		if (!CloseHandle(idleThread))
		{
			LOG("Could not close NIUI idle thread.", LOG_Error);
		}
	}

	useIdleMode = true;
	idleThread = CreateThread( 
		NULL,										// default security attributes
		0,											// use default stack size  
		(LPTHREAD_START_ROUTINE)IdleMode,	// thread function name
		NULL,										// argument to thread function 
		0,											// use default creation flags 
		NULL);


}

void StopIdleMode(int timeToWait)
{
	useIdleMode = false;

	while (inIdleMode)
	{
		Sleep(1);
		timeToWait -= 2;
		if (timeToWait <= 0)
		{
			LOG("Idle thread shutdown was unsucessful.", LOG_Error);
			return;
		}
	}
}

DWORD IdleMode(LPVOID param)
{
	LOG("Idle thread has been entered.", LOG_Standard);
	while (useIdleMode)
	{
		// Shift this over to networking in future, it will be a lot more thread safe.
		inIdleMode = true;
		UpdateNIUI();
	}

	inIdleMode = false;
	return 0;
}

//-----------------------------------------------------------------------------
// The C wrapper functions for accessing the NIUI library.
//-----------------------------------------------------------------------------

/** Initilises the OpenNI Context.  */  
int InitiliseNIUI(int requestedNumberOfPlayers)
{
	XnStatus result = XN_STATUS_OK;

	NIUI_STATUS status = NIUI_STATUS_OK;

	status = g_core.Initilise("NIUIConfig.xml", requestedNumberOfPlayers);
	if (status != NIUI_STATUS_OK)
		return NIUI_STATUS_FAIL;

	// Bind the event manager.
	g_core.RegisterEventManager(&g_eventManager);
	g_eventManager.Initilise(100);

	// Register callbacks.
	NIUI::OpenNIContextData* openNIInfo = g_core.GetOpenNIData();
	
	LOG("Registering user detection and calibration callbacks.", LOG_Init);
	openNIInfo->userGenerator.RegisterUserCallbacks(OnNewUser, OnLostUser, NULL, g_hUserCallbacks);
	openNIInfo->userGenerator.GetSkeletonCap().SetSkeletonProfile(XN_SKEL_PROFILE_ALL);
	openNIInfo->userGenerator.GetSkeletonCap().RegisterCalibrationCallbacks(&OnCalibrationStart, &OnCalibrationEnd, NULL, g_hCalibrationCallbacks);
	openNIInfo->userGenerator.GetSkeletonCap().SetSmoothing(0.5);

	openNIInfo->userGenerator.GetPoseDetectionCap().RegisterToPoseCallbacks(UserPoseDetected, UserPoseLost, NULL, hPoseCallbacks);
	openNIInfo->userGenerator.GetSkeletonCap().GetCalibrationPose(g_strPose);
	
	LOG("NIUI: NIUI.dll initilisation was successful.", LOG_Init);

	g_core.Start();
	return NIUI_STATUS_OK;
}

/** Closes the OpenNI Context and deletes references.  */ 
void ReleaseNIUI()
{
	LOG("Event generation is being stopped.", LOG_Shutdown);
	g_core.Stop();


	LOG("Callbacks are being unregistered.", LOG_Shutdown);
	// Unregister all the callbacks.
	if (NULL != hPoseCallbacks)
	{
		g_core.GetOpenNIData()->userGenerator.GetPoseDetectionCap().UnregisterFromPoseCallbacks(hPoseCallbacks);
		hPoseCallbacks = NULL;
	}

	if (NULL != g_hCalibrationCallbacks)
	{
		g_core.GetOpenNIData()->userGenerator.GetSkeletonCap().UnregisterCalibrationCallbacks(g_hCalibrationCallbacks);
		g_hCalibrationCallbacks = NULL;
	}

	if (NULL != g_hUserCallbacks)
	{
		g_core.GetOpenNIData()->userGenerator.GetSkeletonCap().UnregisterCalibrationCallbacks(g_hUserCallbacks);
		g_hUserCallbacks = NULL;
	}

	// Destory core.
	LOG("Releasing NIUICore.", LOG_Shutdown);
	g_core.Shutdown();
	g_eventManager.Destroy();

	//gLogger.DumpLog();
	//gLogger.EmptyLog();
}

/** Updates the NIUI and OpenNI contexts.  */
void UpdateNIUI()
{
	g_core.Update(false); // No async yet, will reserve this for final release.
}

void GetDepthBuffer(short* out_map)
{
	out_map = NULL;
}

int GetDepthBufferWidth()
{
	return 1;
}

int GetDepthBufferHeight()
{
	return 1;
}

void GetUserLabelMap(unsigned char* out_map)
{
	out_map = NULL;
}

int GetImageBufferWidth()
{
	return 1;
}

int GetImageBufferHeight()
{
	return 1;
}

void GetSkeletalJointLocation(int userID, int joint, UDKVector3* out_location)
{
	g_core.GetJointLocation(userID, joint, out_location);
}


void GetLocalSkeletalJointLocation(int userID, int joint, UDKVector3* out_location)
{
	g_core.GetLocalJointLocation(userID, joint, out_location);
}

void GetUserCentre(int userID, UDKVector3* out_location)
{
	g_core.GetUserCentre(userID, out_location);
}

void GetBoneAxes(int userID, int joint, UDKVector3* out_forward, UDKVector3* out_side, UDKVector3* out_up)
{
	g_core.GetBoneAxes(userID, joint, out_forward, out_side, out_up);
}

bool IsUserDetected(int userID)
{
	User* user = NULL;
	g_core.GetUserWithID(userID, user);
	if (user != NULL)
	{
		return user->GetState() != Lost;
	}
	return false;
}

bool IsUserCalibrating(int userID)
{
	User* user = NULL;
	g_core.GetUserWithID(userID, user);
	if (user != NULL)
	{
		return user->GetState() == Calibrating;
	}

	return false;
}

bool IsUserTracking(int userID)
{
	User* user = NULL;
	g_core.GetUserWithID(userID, user);
	if (user != NULL)
	{
		return user->GetState() == Tracking;
	}

	return false;
}

bool IsUserInPose(int userID)
{
	// Dummy function. Use the event pump.
	return false;
}

/** Starts calibration pose detection.*/
void StartPoseDetection(int userID)
{
	g_core.StartPoseDetection(userID);
}

void StopPoseDetection(int userID)
{
	g_core.StopPoseDetection(userID);
}

void StartCalibration(int userID)
{
	g_core.StartCalibration(userID);
}

void StopCalibration(int userID)
{
	g_core.StopCalibration(userID);
}

/** Configures the NIUI core to either accept or discard pose / calibration tests.*/
void AllowPoseDetection(int enable, int userID)
{
	if (enable)
	{
		g_core.EnablePoseDetectionOnUser(userID);
	}
	else
	{
		g_core.DisablePoseDetectionOnUser(userID);
	}
}

void AllowCalibration(int enable, int userID)
{
	if (enable)
	{
		g_core.EnablePoseDetectionOnUser(userID);
	}
	else
	{
		g_core.DisablePoseDetectionOnUser(userID);
	}
}

void AllowTracking(int enable, int userID)
{
	if (enable)
	{
		g_core.EnablePoseDetectionOnUser(userID);
	}
	else
	{
		g_core.DisablePoseDetectionOnUser(userID);
	}
}

/** Configures the NIUI core to accept pose, calibration and tracking commands on the user.*/
void ActivateUser(int userID)
{
	g_core.EnablePoseDetectionOnUser(userID);
	g_core.EnableCalibrationOnUser(userID);
	g_core.EnableTrackingOnUser(userID);
}

/** Gets the flat data mirror of the users state.*/
void GetUserMirror(int userID, UserMirror* out_mirror)
{
	// Not implemented yet. To come when I fix NIUI's memory fragmentation.
}

/** Determines if the user is "dirty", that is either lost but not marked lost.*/
bool IsUserDirty(int userID)
{
	return false; // Unimplemented.
}

/** Functions to retrieve the total amount of users in a certain state.*/
int GetNumberOfTrackedUsers()
{
	return g_core.GetNumberOfTrackingUsers();
}

int GetNumberOfCalibratingUsers()
{
	return g_core.GetNumberOfCalibratingUsers();
}

int GetNumberOfDetectedUsers()
{
	return g_core.GetNumberOfDetectedUsers();
}

int GetMaxNumberOfUsers()
{
	return g_core.GetMaxNumberOfUsers();
}

/** Changes the behaviour of NIUI's debug dumping.*/
void SetDebugInfoLevel(int level)
{
	gLogger.SetLogLevel((LogLevel)level);
	
}

void SetDebugFileDump(int enable)
{
	gLogger.EnableFileDumping(enable == 1);
}

void SetConsoleDump(int enable)
{
	gLogger.EnableConsoleDumping(enable == 1);
}

/** Event processing.*/
int GetTotalEvents()
{
	return g_eventManager.GetEventCount();
}

void GetEvent(int number, int* out_user, int* out_event)
{
	static Event theEvent;
	if (g_eventManager.GetEvent(number, theEvent))
	{
		*out_user = theEvent.user;
		*out_event = theEvent.theEvent;
	}
}

void ClearEvents()
{
	g_eventManager.ClearEvents();
}

void EnableEventDumping(int enable)
{
	g_eventManager.EnableEventLogging(enable == 1);
}


void GetBasisVectors(UDKVector3* side, UDKVector3* forward, UDKVector3* up)
{
	XnMatrix3X3 identity;

	identity.elements[0] = 1.0f;
	identity.elements[1] = 0.0f;
	identity.elements[2] = 0.0f;
	identity.elements[3] = 0.0f;
	identity.elements[4] = 1.0f;
	identity.elements[5] = 0.0f;
	identity.elements[6] =0.0f;
	identity.elements[7] = 0.0f;
	identity.elements[8] = 1.0f;

	ExtractBasisVectors(identity.elements, side, forward, up);
}