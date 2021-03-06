﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;
using Tuto.Publishing;

namespace Tuto.TreeEditor
{
    public enum MoveDestination
    {
        Into,
        Before,
        After,
    }

    public class PublishViewModel
    {
        public TopicWrap[] Root { get; private set; }
        public ObservableCollection<VideoWrap> UnassignedVideos { get; private set; }
        public Wrap SelectedItem { get; set; }
        public VideoWrap SelectedItemInUnassignedList { get; set; }
        List<VideoPublishSummary> givenVideos;



		public void Commit(TopicWrap wrap, CourseStructure structure, List<VideoPublishSummary> videos)
		{
			wrap.Topic.Items.Clear();
			foreach (var e in wrap.Items)
			{
				if (e is TopicWrap)
				{
					var tw = e as TopicWrap;
					Commit(tw,structure,videos);
					wrap.Topic.Items.Add(tw.Topic);
					continue;
				}
				if (e is VideoWrap)
				{
					var vw = e as VideoWrap;
					var relation = new VideoToTopicRelation();
					relation.TopicGuid = wrap.Topic.Guid;
					relation.NumberInTopic = wrap.Items.IndexOf(vw);
					relation.VideoGuid = vw.Video.Guid;
					structure.VideoToTopicRelations.Add(relation);
                    videos.Add(vw.Video);
				}
			}
		}

		public Tuple<CourseStructure,List<VideoPublishSummary>> Commit()
		{
			var result = new CourseStructure();
            var videos = new List<VideoPublishSummary>();
			Commit(Root[0], result,videos);
			result.RootTopic = Root[0].Topic;
            return Tuple.Create(result, videos);
		}

        public PublishViewModel(CourseTreeData globalData)
        {
            this.givenVideos = globalData.Videos;
            Root = new TopicWrap[] { new TopicWrap(globalData.Structure.RootTopic) };
            UnassignedVideos = new ObservableCollection<VideoWrap>();
            foreach (var e in globalData.Videos.Where(z=>!globalData.Structure.VideoToTopicRelations.Any(x=>x.VideoGuid==z.Guid)))
                UnassignedVideos.Add(new VideoWrap(e));
            foreach(var e in Root[0].Subtree.OfType<TopicWrap>())
                foreach (var v in globalData.Structure.VideoToTopicRelations.Where(z => z.TopicGuid == e.Topic.Guid).OrderBy(z => z.NumberInTopic))
                {
					var pv = globalData.Videos.Where(z=>z.Guid==v.VideoGuid).FirstOrDefault();
					if (pv==null) continue;
					var vw = new VideoWrap(pv);
                    e.Items.Add(vw);
                    vw.Parent = e;
                }


            AddCommand = new RelayCommand(Add, () => SelectedItem != null && SelectedItem is TopicWrap);
            RemoveCommand = new RelayCommand(Remove, () => SelectedItem != null && SelectedItem != Root[0]);
            DeleteCommand = new RelayCommand(DeleteFromList, () => SelectedItemInUnassignedList != null);
        }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }

        //void MoveTopic(TopicViewModel what, TopicViewModel where, int index)
        //{
        //    //проверяем, что не переносим корень
        //    if (what.Parent == null) return;

            

        //    //проверяем, что не переносим родителя в ребенка
        //    var parent=where;
        //    while (true)
        //    {
        //        if (what == parent) return;
        //        parent=parent.Parent;
        //        if (parent == null) break;
        //    }

        //    what.Parent.Items.Remove(what);
        //    if (index < where.Items.Count)
        //        where.Items.Insert(index, what);
        //    else if (index == where.Items.Count)
        //        where.Items.Add(what);
        //    else throw new ArgumentException();
        //    what.Parent = where;
        //}

        bool CheckMoveConsistancy(Wrap what, Wrap where)
        {
            //Корень перемещать нельзя
            if (what == Root[0]) return false;

            //Нельзя перемещать в сына
            var parent = where;
            while (true)
            {
                if (parent == null) break;
                if (parent == what) return false;
                parent = parent.Parent;
            }

            //Нельзя перемещать в видео
            if (where is VideoWrap) return false;

            return true;
        }

        void Remove(Wrap what)
        {
            if (what.Parent == null) return;
            what.Parent.Items.Remove(what);
            what.Parent = null;
        }

        void Delete(Wrap wrap)
        {
            foreach (var e in wrap.Subtree.OfType<VideoWrap>()) UnassignedVideos.Add(e);
        }

        public void Insert(Wrap what, Wrap where, Wrap beforeOf)
        {
            var index = where.Items.IndexOf(beforeOf);
            if (index == -1) index = where.Items.Count;
            where.Items.Insert(index, what);
            what.Parent = where;
        }

        Tuple<Wrap, Wrap> GetWhereAndBeforeOf(Wrap what, Wrap anchor, MoveDestination dst)
        {

            if (dst == MoveDestination.Into)
            {
                if (what is VideoWrap && anchor is VideoWrap)
                    return GetWhereAndBeforeOf(what, anchor, MoveDestination.After);
                return new Tuple<Wrap, Wrap>(anchor, null);
            }

            if (dst == MoveDestination.Before)
            {
                return Tuple.Create(anchor.Parent, anchor);
            }

            if (dst == MoveDestination.After)
            {
                if (anchor.Items.Count == 0)
                {
                    if (anchor.Parent == null) return new Tuple<Wrap, Wrap>(null, null);
                    var index = anchor.Parent.Items.IndexOf(anchor);
                    if (index == anchor.Parent.Items.Count - 1)
                        return new Tuple<Wrap, Wrap>(anchor.Parent, null);
                    return
                        Tuple.Create(anchor.Parent, anchor.Parent.Items[index + 1]);
                }
                return Tuple.Create(anchor, anchor.Items[0]);
            }

            throw new Exception("Cannot fall here");
        }

        public void MoveToTree(Wrap what, Wrap anchor, MoveDestination dst)
        {
            var resp = GetWhereAndBeforeOf(what, anchor, dst);
            Wrap where = resp.Item1;
            Wrap beforeOf = resp.Item2;


            if (where == null) return;
            if (!CheckMoveConsistancy(what, where)) return;

            if (what.Parent!=null)
                Remove(what);
            if (UnassignedVideos.Contains(what))
                UnassignedVideos.Remove(what as VideoWrap);


            Insert(what, where, beforeOf);
        }

        void DeleteFromList()
        {
            if (SelectedItemInUnassignedList != null)
            {
                //GlobalData.VideoData.Remove(SelectedItemInUnassignedList.Video);
                UnassignedVideos.Remove(SelectedItemInUnassignedList);
            }
        }

        void Add()
        {
            SelectedItem.Items.Add(new TopicWrap(new Topic()));
        }

        void Remove()
        {
            if (SelectedItem is VideoWrap)
            {
                SelectedItem.Parent.Items.Remove(SelectedItem);
                UnassignedVideos.Add(SelectedItem as VideoWrap);
            }
            else
            {
                Remove(SelectedItem);
            }
        }
    }
}
