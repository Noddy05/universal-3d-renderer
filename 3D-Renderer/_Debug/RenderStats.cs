using OpenTK.Windowing.Common;

namespace _3D_Renderer._Debug
{
    internal class RenderStats
    {
        private Window window;
        private string originalTitle;
        private List<(float secondsIn, float time)> deltaTimesDated =
            new List<(float secondsIn, float time)>();
        private List<float> deltaTimes = new List<float>();
        private int removeAfterNSeconds = 5;
        private int removeAfterNFrames = 25;

        public RenderStats() {
            window = Program.GetWindow();
            originalTitle = window.Title;
            window.RenderFrame += FrameRendered;
        }

        private int renderCallsMade;
        private int tris;
        /// <summary>
        /// For keeping track of how many DrawCalls has been made this frame. Call this every DrawCall.
        /// </summary>
        /// <param name="tris"></param>
        public void NewDrawCall(int tris)
        {
            renderCallsMade++;
            this.tris += tris;
        }

        private void RemoveOldEntries(FrameEventArgs args)
        {
            deltaTimesDated.Add(((float)window.timeSinceStartup, (float)args.Time));
            deltaTimes.Add((float)args.Time);
            while ((float)window.timeSinceStartup - deltaTimesDated[0].secondsIn >= removeAfterNSeconds)
            {
                deltaTimesDated.RemoveAt(0);
            }
            while (deltaTimes.Count >= removeAfterNFrames)
            {
                deltaTimes.RemoveAt(0);
            }
        }
        private (float, float, float) CalculateAvgMinMax()
        {
            float average = 0;
            float peak = float.MinValue;
            float minimum = float.MaxValue;
            for (int i = 0; i < deltaTimesDated.Count; i++)
            {
                float fps = 1f / deltaTimesDated[i].time;
                average += fps / deltaTimesDated.Count;
                if (fps > peak)
                    peak = fps;
                if (fps < minimum)
                    minimum = fps;
            }

            return (average, minimum, peak);
        }

        /// <summary>
        /// Updates every frame. Changes title to show frame statistics.
        /// </summary>
        /// <param name="args"></param>
        private void FrameRendered(FrameEventArgs args)
        {
            RemoveOldEntries(args);
            (float average, float minimum, float peak) = CalculateAvgMinMax();
            float averageLastNFrames = 0;
            for (int i = 0; i < deltaTimes.Count; i++)
            {
                float fps = 1f / deltaTimes[i];
                averageLastNFrames += fps / deltaTimes.Count;
            }

            window.Title = $"{originalTitle}\t|\tFPS: " + 
                $"{MathF.Round(averageLastNFrames, 1).ToString("N1")} | " +
                $"{MathF.Round(average, 1).ToString("N1")}\t " +
                $"Peak: {MathF.Round(peak, 1).ToString("N1")}" +
                $"\t Low: {MathF.Round(minimum, 1).ToString("N1")}" + 
                $"\t|\tDraw calls: {renderCallsMade.ToString("N0")}\tIndices: " +
                $"{(tris / 3).ToString("N0")}" +
                $"\t";

            tris = 0;
            renderCallsMade = 0;
        }
    }
}
