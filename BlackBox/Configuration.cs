﻿using BlackBox.CodeGeneration;

namespace BlackBox
{
    public class Configuration
    {
        public static RecordingMode RecordingMode { get; set; }
        public static TestFlavour TestFlavour { get; set; }

        public static bool IsRecording()
        {
            return RecordingMode == RecordingMode.Recording;
        }

        public static bool IsPlayback()
        {
            return RecordingMode == RecordingMode.Playback;
        }

        static Configuration()
        {
            RecordingMode = RecordingMode.Recording;
            TestFlavour = new MSTestFlavour();
        }
    }
}