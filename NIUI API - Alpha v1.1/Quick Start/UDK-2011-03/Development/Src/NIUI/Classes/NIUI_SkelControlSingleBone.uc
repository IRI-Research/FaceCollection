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
*   NIUI_SkelControlSingleBone.uc
*
*   Copyright (c) 2011 Matthew Robbins
*
*   Author:  Matthew Robbins
*   Created: 01/2011
*
*   Desc: 
*   A bone controller overriden to replicate the rotations of a bone of a user
*   picked up from a Natural Interaction device.
*/

class NIUI_SkelControlSingleBone extends SkelControlSingleBone;

var private NIUI_Core niuiCore;
var(NIUI) NIUI_BoneID TargetBone;
var(NIUI) name TargetBoneName;
var private Rotator InitialBoneRotation;
var private Quat qBoneDelta;
var private int UserID;
var private bool Bound; // If the control has been allocated a target NIUI user.
var private int intTargetBone; // Precached conversion of the target bone enum to an int to save a squillion conversions.
var int myIndex;

function CacheInitialRotation(SkeletalMeshComponent SkelComp, Quat qOwnerRotInv)
{
	local Quat qBone;

	qBone = SkelComp.GetBoneQuaternion(TargetBoneName, 0);

	// Cache the initial bone rotation.
	InitialBoneRotation = QuatToRotator(qBone);

	// Find the quaternion delta.
	qBoneDelta = QuatProduct(qOwnerRotInv, qBone);

	// Get the bone index.
	myIndex = SkelComp.MatchRefBone(TargetBoneName);
}

function Bind(NIUI_Core core, int user)
{
	niuiCore = core;
	UserID = user;
	Bound = true;
	intTargetBone = int(TargetBone);

	// Transformation is done in world space, however we apply the OpenNI bone rotation in local space and
	// then transform into world for the final rotation.
	BoneRotationSpace = BCS_WorldSpace;
}

function UpdateBoneOverride(float DeltaTime, SkeletalMeshComponent SkelComp, Quat qOwner)
{
	local Quat qRot;

	if (Bound)
	{
		// Apply rotation but don't add it to the animation rotation.
		bApplyRotation = true;
		bAddRotation = false;

		// Get the bones rotation.
		qRot = QuatFromRotator(niuiCore.GetBoneRotation(UserID, intTargetBone));   

		// Transform the cached delta by the OpenNI rotation.
		qROt = QuatProduct(qRot, self.qBoneDelta);

		// Transform into world space based on the owner's orientation.
		BoneRotation = QuatToRotator(QuatProduct(qOwner, qRot));
	}
}

function NIUI_BoneID GetTargetBone()
{
	return self.TargetBone;
}

DefaultProperties
{
	Bound = false;
}
