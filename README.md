# HL2-RM-ROS
Hololens2 Research Mode ROS - The objective of this project was to visualise and command a robot using an Augmented Reality Digital Twin. The entire project is made up of two repositories, this repository contains the Unity application, and the other contains the ROS application: https://github.com/lc252/robot-referencing.
This Unity application streams HL2 sensor data to a ROS server, where the data is used to accurately reference between the virtual and physical robot. The ROS server then provides coordinates and link transformations of the robot in the real world.

This project was developed as part of my undergraduate thesis, please see https://www.youtube.com/watch?v=ZO-2PuSj33c for a demonstration.

This repository builds on the work of:
- Gu, Wenhao - HL2UnityPlugin - https://github.com/petergu684/HoloLens2-ResearchMode-Unity
- Unity Technologies - ROS-Unity Integration - https://github.com/Unity-Technologies/ROS-TCP-Connector

