//#define sound
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using boolean = System.Boolean; // for java source
using System.IO;


using SharpDX;
using SharpDX.DirectSound;
using SharpDX.Multimedia;

using System.Windows.Forms;

using System.Threading;

namespace AprEmu.GB
{

    //sound ref http://www.millstone.demon.co.uk/download/javaboy/download.htm  (ported from java source)

    public class NoiseGenerator
    {
        /** Indicates sound is to be played on the left channel of a stereo sound */
        public int CHAN_LEFT = 1;

        /** Indictaes sound is to be played on the right channel of a stereo sound */
        public int CHAN_RIGHT = 2;

        /** Indicates that sound is mono */
        public int CHAN_MONO = 4;

        /** Indicates the length of the sound in frames */
        int totalLength;
        int cyclePos;

        /** The length of one cycle, in samples */
        int cycleLength;

        /** Amplitude of the wave function */
        int amplitude;

        /** Channel being played on.  Combination of CHAN_LEFT and CHAN_RIGHT, or CHAN_MONO */
        int channel;

        /** Sampling rate of the output channel */
        int sampleRate;

        /** Initial value of the envelope */
        int initialEnvelope;

        int numStepsEnvelope;

        /** Whether the envelope is an increase/decrease in amplitude */
        boolean increaseEnvelope;

        int counterEnvelope;

        /** Creates a white noise generator with the specified wavelength, amplitude, channel, and sample rate */
        public NoiseGenerator(int waveLength, int ampl, int chan, int rate)
        {
            cycleLength = waveLength;
            amplitude = ampl;
            cyclePos = 0;
            channel = chan;
            sampleRate = rate;
        }

        /** Creates a white noise generator with the specified sample rate */
        public NoiseGenerator(int rate)
        {
            cyclePos = 0;
            channel = CHAN_LEFT | CHAN_RIGHT;
            cycleLength = 2;
            totalLength = 0;
            sampleRate = rate;
            amplitude = 32;
        }


        public void setSampleRate(int sr)
        {
            sampleRate = sr;
        }

        /** Set the channel that the white noise is playing on */
        public void setChannel(int chan)
        {
            channel = chan;
        }

        /** Setup the envelope, and restart it from the beginning */
        public void setEnvelope(int initialValue, int numSteps, boolean increase)
        {
            initialEnvelope = initialValue;
            numStepsEnvelope = numSteps;
            increaseEnvelope = increase;
            amplitude = initialValue * 2;
        }

        /** Set the length of the sound */
        public void setLength(int gbLength)
        {
            if (gbLength == -1)
            {
                totalLength = -1;
            }
            else
            {
                totalLength = (64 - gbLength) / 4;
            }
        }

        /** Output a single frame of samples, of specified length.  Start at position indicated in the
         *  output array.
         */
        //Random rd = new Random();
        public void play(byte[] b, int length, int offset)
        {
            int val;

            if (totalLength != 0)
            {
                totalLength--;

                counterEnvelope++;
                if (numStepsEnvelope != 0)
                {
                    if (((counterEnvelope % numStepsEnvelope) == 0) && (amplitude > 0))
                    {
                        if (!increaseEnvelope)
                        {
                            if (amplitude > 0) amplitude -= 2;
                        }
                        else
                        {
                            if (amplitude < 16) amplitude += 2;
                        }
                    }
                }
                for (int r = offset; r < offset + length; r++)
                {

                    //need fix
                    val = (int)((new Random(Guid.NewGuid().GetHashCode()).NextDouble() * amplitude * 2) - amplitude);
                    if ((channel & CHAN_LEFT) != 0) b[r * 2] += (byte)val;
                    if ((channel & CHAN_RIGHT) != 0) b[r * 2 + 1] += (byte)val;
                    if ((channel & CHAN_MONO) != 0) b[r] += (byte)val;

                    cyclePos = (cyclePos + 256) % cycleLength;
                }
            }
        }

    }

    public class SquareWaveGenerator
    {
        /** Sound is to be played on the left channel of a stereo sound */
        public int CHAN_LEFT = 1;

        /** Sound is to be played on the right channel of a stereo sound */
        public int CHAN_RIGHT = 2;

        /** Sound is to be played back in mono */
        public int CHAN_MONO = 4;

        /** Length of the sound (in frames) */
        int totalLength;

        /** Current position in the waveform (in samples) */
        int cyclePos;

        /** Length of the waveform (in samples) */
        int cycleLength;

        /** Amplitude of the waveform */
        int amplitude;

        /** Amount of time the sample stays high in a single waveform (in eighths) */
        int dutyCycle;

        /** The channel that the sound is to be played back on */
        int channel;

        /** Sample rate of the sound buffer */
        int sampleRate;

        /** Initial amplitude */
        int initialEnvelope;

        /** Number of envelope steps */
        int numStepsEnvelope;

        /** If true, envelope will increase amplitude of sound, false indicates decrease */
        boolean increaseEnvelope;

        /** Current position in the envelope */
        int counterEnvelope;

        /** Frequency of the sound in internal GB format */
        int gbFrequency;

        /** Amount of time between sweep steps. */
        int timeSweep;

        /** Number of sweep steps */
        int numSweep;

        /** If true, sweep will decrease the sound frequency, otherwise, it will increase */
        boolean decreaseSweep;

        /** Current position in the sweep */
        int counterSweep;

        /** Create a square wave generator with the supplied parameters */
        public SquareWaveGenerator(int waveLength, int ampl, int duty, int chan, int rate)
        {
            cycleLength = waveLength;
            amplitude = ampl;
            cyclePos = 0;
            dutyCycle = duty;
            channel = chan;
            sampleRate = rate;
        }

        /** Create a square wave generator at the specified sample rate */
        public SquareWaveGenerator(int rate)
        {
            dutyCycle = 4;
            cyclePos = 0;
            channel = CHAN_LEFT | CHAN_RIGHT;
            cycleLength = 2;
            totalLength = 0;
            sampleRate = rate;
            amplitude = 32;
            counterSweep = 0;
        }

        /** Set the sound buffer sample rate */
        public void setSampleRate(int sr)
        {
            sampleRate = sr;
        }

        /** Set the duty cycle */
        public void setDutyCycle(int duty)
        {
            switch (duty)
            {
                case 0: dutyCycle = 1;
                    break;
                case 1: dutyCycle = 2;
                    break;
                case 2: dutyCycle = 4;
                    break;
                case 3: dutyCycle = 6;
                    break;
            }
            //  System.out.println(dutyCycle);
        }

        /** Set the sound frequency, in internal GB format */
        public void setFrequency(int gbFrequency)
        {
            try
            {
                float frequency = 131072 / 2048;

                if (gbFrequency != 2048)
                {
                    frequency = ((float)131072 / (float)(2048 - gbFrequency));
                }
                //  System.out.println("gbFrequency: " + gbFrequency + "");
                this.gbFrequency = gbFrequency;
                if (frequency != 0)
                {
                    cycleLength = (256 * sampleRate) / (int)frequency;
                }
                else
                {
                    cycleLength = 65535;
                }
                if (cycleLength == 0) cycleLength = 1;
                //  System.out.println("Cycle length : " + cycleLength + " samples");
            }
            catch (ArithmeticException e)
            {
                // Skip ip
            }
        }

        /** Set the channel for playback */
        public void setChannel(int chan)
        {
            channel = chan;
        }

        /** Set the envelope parameters */
        public void setEnvelope(int initialValue, int numSteps, boolean increase)
        {
            initialEnvelope = initialValue;
            numStepsEnvelope = numSteps;
            increaseEnvelope = increase;
            amplitude = initialValue * 2;
        }

        /** Set the frequency sweep parameters */
        public void setSweep(int time, int num, boolean decrease)
        {
            timeSweep = (time + 1) / 2;
            numSweep = num;
            decreaseSweep = decrease;
            counterSweep = 0;
            //  System.out.println("Sweep: " + time + ", " + num + ", " + decrease);
        }

        public void setLength(int gbLength)
        {
            if (gbLength == -1)
            {
                totalLength = -1;
            }
            else
            {
                totalLength = (64 - gbLength) / 4;
            }
        }

        public void setLength3(int gbLength)
        {
            if (gbLength == -1)
            {
                totalLength = -1;
            }
            else
            {
                totalLength = (256 - gbLength) / 4;
            }
        }

        public void setVolume3(int volume)
        {
            switch (volume)
            {
                case 0: amplitude = 0;
                    break;
                case 1: amplitude = 32;
                    break;
                case 2: amplitude = 16;
                    break;
                case 3: amplitude = 8;
                    break;
            }
            //  System.out.println("A:"+volume);
        }

        /** Output a frame of sound data into the buffer using the supplied frame length and array offset. */
        public void play(byte[] b, int length, int offset)
        {
            int val = 0;

            if (totalLength != 0)
            {
                totalLength--;

                if (timeSweep != 0)
                {
                    counterSweep++;
                    if (counterSweep > timeSweep)
                    {
                        if (decreaseSweep)
                        {
                            setFrequency(gbFrequency - (gbFrequency >> numSweep));
                        }
                        else
                        {
                            setFrequency(gbFrequency + (gbFrequency >> numSweep));
                        }
                        counterSweep = 0;
                    }
                }

                counterEnvelope++;
                if (numStepsEnvelope != 0)
                {
                    if (((counterEnvelope % numStepsEnvelope) == 0) && (amplitude > 0))
                    {
                        if (!increaseEnvelope)
                        {
                            if (amplitude > 0) amplitude -= 2;
                        }
                        else
                        {
                            if (amplitude < 16) amplitude += 2;
                        }
                    }
                }
                for (int r = offset; r < offset + length; r++)
                {

                    if (cycleLength != 0)
                    {
                        if (((8 * cyclePos) / cycleLength) >= dutyCycle)
                        {
                            val = amplitude;
                        }
                        else
                        {
                            val = -amplitude;
                        }
                    }




                    //need fix
                    if ((channel & CHAN_LEFT) != 0) b[r * 2] += (byte)val;
                    if ((channel & CHAN_RIGHT) != 0) b[r * 2 + 1] += (byte)val;
                    if ((channel & CHAN_MONO) != 0) b[r] += (byte)val;



                    cyclePos = (cyclePos + 256) % cycleLength;
                }
            }
        }

    }

    public class VoluntaryWaveGenerator
    {
        public int CHAN_LEFT = 1;
        public int CHAN_RIGHT = 2;
        public int CHAN_MONO = 4;

        int totalLength;
        int cyclePos;
        int cycleLength;
        int amplitude;
        int channel;
        int sampleRate;
        int volumeShift;

        byte[] waveform = new byte[32];

        public VoluntaryWaveGenerator(int waveLength, int ampl, int duty, int chan, int rate)
        {
            cycleLength = waveLength;
            amplitude = ampl;
            cyclePos = 0;
            channel = chan;
            sampleRate = rate;
        }

        public VoluntaryWaveGenerator(int rate)
        {
            cyclePos = 0;
            channel = CHAN_LEFT | CHAN_RIGHT;
            cycleLength = 2;
            totalLength = 0;
            sampleRate = rate;
            amplitude = 32;
        }

        public void setSampleRate(int sr)
        {
            sampleRate = sr;
        }

        public void setFrequency(int gbFrequency)
        {
            //  cyclePos = 0;
            float frequency = (int)((float)65536 / (float)(2048 - gbFrequency));
            //  System.out.println("gbFrequency: " + gbFrequency + "");
            cycleLength = (int)((float)(256f * sampleRate) / (float)frequency);
            if (cycleLength == 0) cycleLength = 1;
            //  System.out.println("Cycle length : " + cycleLength + " samples");
        }

        public void setChannel(int chan)
        {
            channel = chan;
        }

        public void setLength(int gbLength)
        {
            if (gbLength == -1)
            {
                totalLength = -1;
            }
            else
            {
                totalLength = (256 - gbLength) / 4;
            }
        }

        public void setSamplePair(int address, int value)
        {
            waveform[address * 2] = (byte)((value & 0xF0) >> 4);
            waveform[address * 2 + 1] = (byte)((value & 0x0F));
        }

        public void setVolume(int volume)
        {
            switch (volume)
            {
                case 0: volumeShift = 5;
                    break;
                case 1: volumeShift = 0;
                    break;
                case 2: volumeShift = 1;
                    break;
                case 3: volumeShift = 2;
                    break;
            }
            //  System.out.println("A:"+volume);
        }

        public void play(byte[] b, int length, int offset)
        {
            int val;

            if (totalLength != 0)
            {
                totalLength--;

                for (int r = offset; r < offset + length; r++)
                {

                    int samplePos = (31 * cyclePos) / cycleLength;

                    //need fix
                    val = waveform[samplePos % 32] >> volumeShift << 1;

                    if ((channel & CHAN_LEFT) != 0) b[r * 2] += (byte)val;
                    if ((channel & CHAN_RIGHT) != 0) b[r * 2 + 1] += (byte)val;
                    if ((channel & CHAN_MONO) != 0) b[r] += (byte)val;

                    cyclePos = (cyclePos + 256) % cycleLength;
                }
            }
        }
    }



    //--

    public class SoundChip
    {



        public WaveFormat wav;
        public DirectSound _SoundDevice;
        public SecondarySoundBuffer buffer;
        public BufferCapabilities capabilities;
        public DataStream dataPart2;
        public DataStream dataPart1;
        public int numberOfSamples; // = capabilities.BufferBytes / wav.BlockAlign;
        public int numSamples;


        // = new byte[numSamples];

        /** The DataLine for outputting the sound */
        //SourceDataLine soundLine;
        public SquareWaveGenerator channel1;
        public SquareWaveGenerator channel2;
        public VoluntaryWaveGenerator channel3;
        public NoiseGenerator channel4;
        public boolean soundEnabled = true;



        /** If true, channel is enabled */
        boolean channel1Enable = true, channel2Enable = true,
                channel3Enable = true, channel4Enable = true;

        /** Current sampling rate that sound is output at */
        int sampleRate = 48000; //ORG 44100

        /** Amount of sound data to buffer before playback */
        //int bufferLengthMsec = 400; //ORG 200

        /** Initialize sound emulation, and allocate sound hardware */
        public SoundChip(IntPtr shandle)
        {
            channel1 = new SquareWaveGenerator(sampleRate);
            channel2 = new SquareWaveGenerator(sampleRate);
            channel3 = new VoluntaryWaveGenerator(sampleRate);
            channel4 = new NoiseGenerator(sampleRate);


            // MessageBox.Show(shandle.ToString());

            _SoundDevice = new DirectSound();
            _SoundDevice.SetCooperativeLevel(shandle, CooperativeLevel.Normal);
            wav = new WaveFormat(48000, 8, 2);
            SoundBufferDescription des = new SoundBufferDescription();
            des.Format = wav;
            des.BufferBytes = (int)(48000 * 2 * 0.1);
            des.Flags = BufferFlags.ControlVolume | BufferFlags.ControlFrequency | BufferFlags.ControlPan | BufferFlags.Software;
            buffer = new SecondarySoundBuffer(_SoundDevice, des);
            capabilities = buffer.Capabilities;



            numberOfSamples = capabilities.BufferBytes / wav.BlockAlign;

            numSamples =  (sampleRate / 28) & 0xFFFE;

            

           // buffer.Play(0, PlayFlags.None );

        }


        List<byte> sbuffer = new List<byte>();

        int count = 0;

        

        public void outputSound()
        {
            if (soundEnabled)
            {

               // MessageBox.Show(numberOfSamples.ToString());

                //1714


                byte[] buf = new byte[numSamples]; 

                if (channel1Enable) channel1.play(buf, numSamples / 2, 0);
                if (channel2Enable) channel2.play(buf, numSamples / 2, 0);
                if (channel3Enable) channel3.play(buf, numSamples / 2, 0);
                if (channel4Enable) channel4.play(buf, numSamples / 2, 0);

                //---------
                for (int i = 0; i < numSamples; i++)               
                    sbuffer.Add(buf[i]);

                
                if (sbuffer.Count >= 9600 ) //20568  19200 9600
                {

                    

                    //Console.WriteLine(sbuffer.Count);
                    dataPart1 = buffer.Lock(0, capabilities.BufferBytes, LockFlags.EntireBuffer, out dataPart2);
                    for (int n = 0; n < numberOfSamples ; n++)
                    {
                        dataPart1.Write((byte)(((byte)(sbuffer[n * 2] + 128)) >> 1));
                        dataPart1.Write((byte)(((byte)(sbuffer[n * 2 + 1] + 128)) >> 1));
                    }
                    buffer.Unlock(dataPart1, dataPart2);

                    buffer.Play(0, PlayFlags.None);
                    sbuffer.Clear();
                }
            }
        }

    }




    //--


}
