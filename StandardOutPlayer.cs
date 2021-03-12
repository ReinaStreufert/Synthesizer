using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Synthesizer
{
    class StandardOutPlayer
    {
        private Thread playbackThread;
        private Stream standardOut;

        private int position = 0;
        private int absPosition = 0;
        private float[] buf;
        private byte[] stdOutBuf;
        private int samplesPerBuffer;

        private DateTime playbackStart;

        public void Initialize(int SamplesPerBuffer)
        {
            samplesPerBuffer = SamplesPerBuffer;
            buf = new float[SamplesPerBuffer * 2];
            stdOutBuf = new byte[SamplesPerBuffer * 2 * 4];
        }

        public ISampleProvider Input;
        public void StartPlayback()
        {
            playbackThread = new Thread(playbackLoop);
            standardOut = Console.OpenStandardOutput();

            playbackThread.Start();
        }
        public void StopPlayback()
        {
            playbackThread.Abort();
            standardOut.Dispose();
        }
        public void ResetOffset()
        {
            position = 0;
        }

        private void playbackLoop()
        {
            for (int i = 0; i < 2; i++)
            {
                writeNextSamples(); // give lots of overhead
            }
            int bufferingAmount = (int)((samplesPerBuffer / 44100F) * 1000F);
            playbackStart = DateTime.Now;
            while (true)
            {
                Thread.Sleep(bufferingAmount / 2);
                int buffered = (absPosition / 2) - (int)((DateTime.Now - playbackStart).TotalSeconds * 44100);
                if (buffered < bufferingAmount)
                {
                    writeNextSamples();
                }
            }
        }
        private void writeNextSamples()
        {
            Input.FillBuffer(buf, 44100, position);
            Buffer.BlockCopy(buf, 0, stdOutBuf, 0, stdOutBuf.Length);
            position += stdOutBuf.Length / 4;
            absPosition += stdOutBuf.Length / 4;

            standardOut.Write(stdOutBuf, 0, stdOutBuf.Length);
        }
    }
}
