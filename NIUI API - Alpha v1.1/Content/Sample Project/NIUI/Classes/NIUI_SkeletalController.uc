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
*   NIUI_SkeletalController.uc
*
*   Copyright (c) 2011 Matthew Robbins
*
*   Author:  Matthew Robbins
*   Created: 01/2011
*
*   Desc: 
*   Overrides a skeleton to replicate the rotations of a user picked up from a 
*   Natural Interaction device.
*   Spawn one of these within your pawn class and use the Bind() function to 
*   make the bones access the rotations of the target NIUI user.
*   You also need to have a T-Pose animation so that the rotation overiding will 
*   work.
*/

class NIUI_SkeletalController extends Actor;

var() int NIUI_UserID;
var private array<NIUI_SkelControlSingleBone> BoneControllers;

var private SkeletalMeshComponent TargetSkeleton;
var private AnimTree TargetAnimTree;

// If to allow skeletal tracking.
var private bool TrackSkeleton;
var NIUI_Core theCore;

// Caches all NIUI bone controllers from the target anim tree.
function CacheSkeletonControls(SkeletalMeshComponent SkelComp, array<name> TargetNodeNames)
{
	local int iter;
	local NIUI_SkelControlSingleBone BoneControl;

	// Cache all NIUI_SkelControlSingleBone
	for (iter = 0; iter < TargetNodeNames.Length; iter++)
	{
		BoneControl = NIUI_SkelControlSingleBone(SkelComp.FindSkelControl(TargetNodeNames[iter]));
		if (BoneControl != none)
		{
			// Add the bone controller.
			BoneControllers.AddItem(BoneControl);
			`log(BoneControl @"targeting joint:" @BoneControl.GetTargetBone());
		}
	}
}

/** Takes the userID and makes all the cached bones start inheriting the rotations
 *  of the linked bones. */
function Bind(NIUI_Core core, int userID)
{
	local int iter;

	theCore = core;

	// Set the target ID and bind the bones to the user.
	NIUI_UserID = userID;

	for (iter = 0; iter < BoneControllers.Length; iter++)
	{
		BoneControllers[iter].Bind(core, userID);
	}
}

function CacheRotations(SkeletalMeshComponent SkelComp, Rotator OwnerRotation)
{
	local int iter;
	local Quat qOwnerInv;

	qOwnerInv = QuatFromRotator(OwnerRotation);
	qOwnerInv = QuatInvert(qOwnerInv);

	for (iter = 0; iter < BoneControllers.Length; iter++)
	{
		BoneControllers[iter].CacheInitialRotation(SkelComp, qOwnerInv);
	}
}

/** Enforces the bone rotation tracking. */
function UpdateBoneOverides(float DeltaTime, Vector OwnerPos, SkeletalMeshComponent SkelComp, Rotator OwnerRot)
{
	local int iter;
	local Quat qOwner;

	qOwner = QuatFromROtator(OwnerRot);

	for (iter = 0; iter < BoneControllers.Length; iter++)
	{
		BoneControllers[iter].UpdateBoneOverride(DeltaTime, SkelComp, qOwner);
	}
}

DefaultProperties
{
}
