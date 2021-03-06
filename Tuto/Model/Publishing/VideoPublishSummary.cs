﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Tuto.Publishing
{
	[DataContract]
	public class VideoPublishSummary
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public Guid Guid { get; set; }

		[DataMember]
		public TimeSpan Duration { get; set; }

		[DataMember]
		public string OrdinalSuffix { get; set; }

		public VideoPublishSummary() { }

		public VideoPublishSummary(FinishedVideo fv)
		{
			Guid = fv.Guid;
			Name = fv.Name;
			Duration = fv.Duration;
							
		}
	}


}
