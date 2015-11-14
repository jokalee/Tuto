﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Tuto.Model2
{
    public class Locations
    {
        readonly EditorModel model;
		internal Locations(EditorModel model) { this.model = model; }

        internal FileInfo Make(DirectoryInfo info, string fname)
        {
            return new FileInfo(Path.Combine(info.FullName, fname));
        }

        internal FileInfo GetSuffixedName(FileInfo source, string suffix)
        {
            var newPath = source.FullName.Split('\\');
            var nameAndExt = source.Name.Split('.');
            nameAndExt[0] = nameAndExt[0] + suffix;
            newPath[newPath.Length - 1] = string.Join(".", nameAndExt);
            return new FileInfo(string.Join("\\", newPath));
        }

        internal FileInfo GetThumbName(FileInfo source)
        {
            return GetSuffixedName(source, "-thumb");
        }

        
		public FileInfo ClearedSound { get { return Make(model.Locations.FaceVideo.Directory, "cleaned.mp3"); } }
        
        
        

        public FileInfo FaceVideo { get { return Make(model.RawLocation, Names.FaceFileName); } }
        public FileInfo DesktopVideo { get { return Make(model.RawLocation, Names.DesktopVideoFileName); } }

        public FileInfo FaceVideoThumb { get { return GetThumbName(FaceVideo); } }
        public FileInfo DesktopVideoThumb { get { return GetThumbName(DesktopVideo); } }
        
        public FileInfo ConvertedFaceVideo { get { return Make(model.TempFolder, "face-converted.avi"); } }
        public FileInfo ConvertedDesktopVideo { get { return Make(model.TempFolder, "desktop-converted.avi"); } }
        
        public FileInfo PraatVoice { get { return Make(model.RawLocation, "voice.mp3"); } }
        public FileInfo LocalFilePath { get { return model.ModelFileLocation; } }

 
        public DirectoryInfo PatchesDirectory
        {
            get
            {
                throw new NotImplementedException();
                //var relative = model.Global.Locations.RelativeTo(model.RawLocation.FullName, model.Global.Locations.InputFolder.FullName);
                //var name = Path.Combine(model.Global.Locations.PatchesFolder.FullName, relative);
                //return new DirectoryInfo(name);
            }
        }

        [Obsolete]
        public FileInfo PraatOutput { get { return Make(model.TempFolder, "praat.output"); } }

       


        public FileInfo GetOutputFile(int episodeNumber)
        {
            var fname = MyPath.RelativeTo(model.RawLocation.FullName, model.Global.RawFolder.FullName);
            fname = MyPath.CreateHierarchicalName(fname);
            fname += episodeNumber + " " + model.Montage.Information.Episodes[episodeNumber].Name+".avi";
            var file = new FileInfo(
            Path.Combine(
                   model.Global.OutputFolder.FullName,
                   fname));
            return file;
        }

        public FileInfo GetAvsStriptFile(int episodeNumber)
        {
            return new FileInfo(
                Path.Combine(
                    model.TempFolder.FullName,
                    string.Format("script_{0}.avs", episodeNumber)));
        }

        public FileInfo GetSrtFile(int episodeNumber)
        {
            return new FileInfo(
                Path.Combine(
                    model.TempFolder.FullName,
                    string.Format("subtitles_{0}.srt", episodeNumber)));
        }


    }
}