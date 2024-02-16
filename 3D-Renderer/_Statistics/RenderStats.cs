using OpenTK.Windowing.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Statistics
{
    internal class RenderStats
    {
        private Window window;
        private string originalTitle;
        private List<float> deltaTimes = new List<float>();
        private int removeAfterNFrames = 5000;

        public RenderStats() {
            window = Program.window;
            originalTitle = window.Title;
            window.RenderFrame += FrameRendered;
        }

        private int renderCallsMade;
        private int tris;
        public void RenderCall(int tris)
        {
            renderCallsMade++;
            this.tris += tris;
        }

        private void FrameRendered(FrameEventArgs args)
        {
            deltaTimes.Add((float)args.Time);
            if (deltaTimes.Count > removeAfterNFrames)
            {
                deltaTimes.RemoveAt(0);
            }
            float average = 0;
            float peak = float.MinValue;
            float minimum = float.MaxValue;
            for(int i = 0; i < deltaTimes.Count; i++)
            {
                float fps = 1f / deltaTimes[i];
                average += fps / deltaTimes.Count;
                if (fps > peak)
                    peak = fps;
                if (fps < minimum)
                    minimum = fps;
            }

            window.Title = $"{originalTitle}\t|\tFPS: " + 
                $"{MathF.Round(average, 1).ToString("N1")}\t Peak: {MathF.Round(peak, 1).ToString("N1")}" +
                $"\t Low: {MathF.Round(minimum, 1).ToString("N1")}" + 
                $"\t|\tRender calls: {renderCallsMade.ToString("N0")}\tIndices: {tris.ToString("N0")}" +
                $"\t";

            tris = 0;
            renderCallsMade = 0;
        }
    }
}
