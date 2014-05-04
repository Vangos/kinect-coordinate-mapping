Kinect Coordinate Mapping
=========================

This project demonstrates Kinect for Windows coordinate mapping using SDK version 1.8.

Learn how to draw the joints on screen and PERFECTLY align them with the color or depth frame.

Example using CoordinateMapper:

    foreach (Joint joint in body.Joints)
    {
        // 3D coordinates in meters
        SkeletonPoint skeletonPoint = joint.Position;

        // 2D coordinates in pixels
        Point point = new Point();

        if (_mode == CameraMode.Color)
        {
            // Skeleton-to-Color mapping
            ColorImagePoint colorPoint = _sensor.CoordinateMapper.MapSkeletonPointToColorPoint(
                                                                  skeletonPoint,
                                                                  ColorImageFormat.RgbResolution640x480Fps30);

            point.X = colorPoint.X;
            point.Y = colorPoint.Y;
        }
        else if (_mode == CameraMode.Depth)
        {
            // Skeleton-to-Depth mapping
            DepthImagePoint depthPoint = _sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(
                                                                  skeletonPoint,
                                                                  DepthImageFormat.Resolution320x240Fps30);

            point.X = depthPoint.X;
            point.Y = depthPoint.Y;
        }
    }

Tutorial
---
* Read a thorough tutorial to understand Kinect coordiante mapping: [http://pterneas.com/?p=1554] (http://pterneas.com/?p=1554)

Credits
---
* Developed by [Vangos Pterneas](http://pterneas.com) for [LightBuzz](http://lightbuzz.com)

License
---
You are free to use these libraries in personal and commercial projects by attributing the original creator of Vitruvius. Licensed under [MIT License](https://github.com/Vangos/kinect-coordinate-mapping/blob/master/LICENSE).
