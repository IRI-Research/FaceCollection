Flexible Action and Articulated Skeleton Toolkit
Version: 0.06

Author: Evan A. Suma
Email: faast@ict.usc.edu
Project Website: http://projects.ict.usc.edu/mxr/faast

Developed at the University of Southern California Institute for Creative Technologies in collaboration with Belinda Lange, Skip Rizzo, David Krum, and Mark Bolas.

FAAST is middleware to facilitate integration of full-body control with games and VR applications. The toolkit relies upon software from OpenNI and PrimeSense to track the user's motion using the PrimeSensor or the Microsoft Kinect sensors. FAAST includes a custom VRPN server to stream the user's skeleton over a network, allowing VR applications to read the skeletal joints as trackers using any VRPN client. Additionally, the toolkit can also emulate keyboard input triggered by body posture and specific gestures. This allows the user add custom body-based control mechanisms to existing off-the-shelf games that do not provide official support for depth sensors. 

FAAST is free to use and distribute. If you use FAAST to support your research project, we request that any publications resulting from the use of this software include a reference to the toolkit (a tech report will be posted here within the next week for this purpose). Additionally, please send us an email about your project, so we can compile a list of projects that use FAAST. This will be help us pursue funding to maintain the software and add new functionality. 

The preliminary version of FAAST is currently available for Windows only. We are currently preparing to release code as an open-source project. Additionally, we plan to develop a Linux port in the near future. 

Please see the project website for installation and usage instructions.



Version History
---------------

0.06
Added mouse support.  Fixed issue with keyboard events not being registered in games that use DirectInput.  Also removed key_special command; special keys now all have their own plain text keywords instead of having to use virtual key codes.

0.05
Minor bugfix release to fix compability with OpenNI's new Kinect drivers.  Changed skeleton joint units in VRPN from millimeters to meters, in order to fit with the VRPN standard.  Please note that if you are upgrading from an older version you will need to uninstall your current version of OpenNI, NITE, and the sensor driver, and then reinstall them using the new versions listed on our website.

0.04
Added support for 10 new actions, including jump, walk, and directional foot poses.  Fixed sensor initialization so that FAAST won't crash if the sensor is not hooked up or NITE is configured incorrectly.

0.03
Initial public release.