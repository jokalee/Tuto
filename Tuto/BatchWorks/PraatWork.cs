﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tuto.Model;

namespace Tuto.BatchWorks
{
    public class PraatWork : ProcessBatchWork
    {
        public PraatWork(EditorModel model)
        {
            Model = model;
            Name = "Praat working: " + model.RawLocation.Name;
        }

        const string SilentLabel = "--";
        const string SoundLabel = "++";

        const double MinPitch = 100;
        const double TimeStep = 0;

        const double SilenceThreshold = -27;
        const double MinSilentInterval = 0.4;
        const double MinSoundInterval = 0.1;

        public event EventHandler PraatCreated;

        public override void Work()
        {
            Model.Locations.PraatOutput.Delete();

            var convert = "";
            var ffmpegExecutable = Model.Videotheque.Locations.FFmpegExecutable;
            var args = string.Format("-i \"{0}\" -vn -q:a 0 \"{1}\" -y", Model.Locations.FaceVideo, Model.Locations.PraatVoice);
            RunProcess(args, ffmpegExecutable.ToString());
            var work =
                String.Format(
                    CultureInfo.InvariantCulture,
                    " --run \"{0}\" \"{1}\" \"{2}\" {3} {4} {5} {6} {7} {8} {9}",
                    Model.Locations.PraatScriptSource,
                    Model.Locations.PraatVoice,
                    Model.Locations.PraatOutput,
                    SilentLabel,
                    SoundLabel,
                    MinPitch,
                    TimeStep,
                    SilenceThreshold,
                    MinSilentInterval,
                    MinSoundInterval);
            RunProcess(work, Model.Videotheque.Locations.PraatExecutable.ToString());
            Model.Montage.SoundIntervals.Clear();
            using (var reader = new StreamReader(Model.Locations.PraatOutput.FullName))
            {

                for (var i = 0; i < 11; i++)
                    reader.ReadLine();

                var intervalCount = int.Parse(reader.ReadLine());
                for (int i = 0; i < intervalCount; i++)
                {
                    var startTime = double.Parse(reader.ReadLine(), CultureInfo.InvariantCulture);
                    var endTime = double.Parse(reader.ReadLine(), CultureInfo.InvariantCulture);
                    var hasVoice = reader.ReadLine() == '"' + SoundLabel + '"';
                    Model.Montage.SoundIntervals.Add(
                        new SoundInterval(
                            (int)Math.Round(startTime * 1000),
                            (int)Math.Round(1000 * endTime),
                            hasVoice));
                }
            }


            //  model.Locations.PraatVoice.Delete();
            Model.Locations.PraatOutput.Delete();
            OnTaskFinished();
        }

        public override void Clean()
        {
            if (Process != null && !Process.HasExited)
                Process.Kill();
            if (File.Exists(Model.Locations.PraatOutput.FullName))
            {
                while (File.Exists(Model.Locations.PraatOutput.FullName))
                    try
                    {
                        File.Delete(Model.Locations.PraatOutput.FullName);
                    }
                    catch { }
            }
        }
    }
}
