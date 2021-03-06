﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Tuto.Publishing
{
	[DataContract]
	public class VideoToTopicRelation
	{
		[DataMember]
		public Guid VideoGuid { get; set; }

		[DataMember]
		public Guid TopicGuid { get; set; }

		[DataMember]
		public int NumberInTopic { get; set; }
	}

	[DataContract]
	public class CourseStructure
	{
		[DataMember]
		public Topic RootTopic { get; set; }

		[DataMember]
		public List< VideoToTopicRelation> VideoToTopicRelations { get; set; }

		public CourseStructure()
		{
			RootTopic = new Topic();
			VideoToTopicRelations = new List<VideoToTopicRelation>();
		}
	}

	public class CourseTreeData
	{

        public const string VideoListName = "VideoSummaries.txt";

		public List<VideoPublishSummary> Videos { get; set; }
		public CourseStructure Structure { get; set; }
		
		public CourseTreeData(){}

		public static CourseTreeData Load(DirectoryInfo directory)
		{
			var result = new CourseTreeData();
			result.Videos = HeadedJsonFormat.Read<List<VideoPublishSummary>>(
				new FileInfo(Path.Combine(directory.FullName, VideoListName)));
			result.Structure = HeadedJsonFormat.Read<CourseStructure>(directory);
			return result;
		}

	
	}
}
